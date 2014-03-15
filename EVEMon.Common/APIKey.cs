using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using EVEMon.Common.Attributes;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Net;
using EVEMon.Common.Notifications;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents a player API key.
    /// </summary>
    [EnforceUIThreadAffinity]
    public sealed class APIKey
    {
        #region Fields

        private readonly APIKeyQueryMonitor<SerializableAPIKeyInfo> m_apiKeyInfoMonitor;
        private readonly APIKeyQueryMonitor<SerializableAPIAccountStatus> m_accountStatusMonitor;

        private readonly Dictionary<string, SkillInTrainingResponse> m_skillInTrainingCache =
            new Dictionary<string, SkillInTrainingResponse>();

        private bool m_queried;
        private bool m_monitored;
        private bool m_updatePending;
        private bool m_characterListUpdated;

        #endregion


        #region Constructors

        /// <summary>
        /// Common constructor base.
        /// </summary>
        private APIKey()
        {
            m_apiKeyInfoMonitor = new APIKeyQueryMonitor<SerializableAPIKeyInfo>(this, APIGenericMethods.APIKeyInfo,
                                                                                 OnAPIKeyInfoUpdated);

            m_accountStatusMonitor = new APIKeyQueryMonitor<SerializableAPIAccountStatus>(this, APICharacterMethods.AccountStatus,
                                                                                          OnAccountStatusUpdated);

            IdentityIgnoreList = new CharacterIdentityIgnoreList(this);

            EveMonClient.TimerTick += EveMonClient_TimerTick;
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="serial"></param>
        internal APIKey(SerializableAPIKey serial)
            : this()
        {
            ID = serial.ID;
            VerificationCode = serial.VerificationCode;
            Type = serial.Type;
            Expiration = serial.Expiration;
            AccessMask = serial.AccessMask;
            m_monitored = serial.Monitored;
            IdentityIgnoreList.Import(serial.IgnoreList);
        }

        /// <summary>
        /// Constructor from the provided informations.
        /// </summary>
        /// <param name="id"></param>
        public APIKey(long id)
            : this()
        {
            ID = id;
            VerificationCode = String.Empty;
            m_monitored = true;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public long ID { get; private set; }

        /// <summary>
        /// Gets or sets the verification code.
        /// </summary>
        /// <value>The verification code.</value>
        public string VerificationCode { get; private set; }

        /// <summary>
        /// Gets or sets the access mask.
        /// </summary>
        /// <value>The access mask.</value>
        public long AccessMask { get; private set; }

        /// <summary>
        /// Gets or sets the type of the key.
        /// </summary>
        /// <value>The type of the key.</value>
        public APIKeyType Type { get; private set; }

        /// <summary>
        /// Gets or sets the key expiration.
        /// </summary>
        /// <value>The key expiration.</value>
        public DateTime Expiration { get; private set; }

        /// <summary>
        /// Gets the list of items to never import.
        /// </summary>
        public CharacterIdentityIgnoreList IdentityIgnoreList { get; private set; }

        /// <summary>
        /// Gets the account expiration date and time.
        /// </summary>
        public DateTime AccountExpires { get; set; }

        /// <summary>
        /// Gets the account creation date and time.
        /// </summary>
        public DateTime AccountCreated { get; set; }

        /// <summary>
        /// Gets the character identities for this API key.
        /// </summary>
        public IEnumerable<CharacterIdentity> CharacterIdentities
        {
            get { return EveMonClient.CharacterIdentities.Where(characterID => characterID.APIKeys.Contains(this)); }
        }

        /// <summary>
        /// Gets the cached until date and time of the last result.
        /// </summary>
        public DateTime CachedUntil
        {
            get { return m_apiKeyInfoMonitor.LastResult == null ? DateTime.UtcNow : m_apiKeyInfoMonitor.LastResult.CachedUntil; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="APIKey"/> is monitored.
        /// </summary>
        /// <value><c>true</c> if monitored; otherwise, <c>false</c>.</value>
        public bool Monitored
        {
            get { return m_monitored; }
            set
            {
                m_monitored = value;
                EveMonClient.OnAPIKeyMonitoredChanged();
            }
        }

        /// <summary>
        /// Gets true if at least one of the CCP characters is monitored.
        /// </summary>
        public bool HasMonitoredCharacters
        {
            get
            {
                return CharacterIdentities.Select(id => id.CCPCharacter).Any(
                    ccpCharacter => ccpCharacter != null && ccpCharacter.Monitored);
            }
        }

        /// <summary>
        /// Gets the character in training on this API key, or null if none are in training.
        /// </summary>
        /// <remarks>Returns null if the character is in the ignored list.</remarks>
        public CCPCharacter TrainingCharacter
        {
            get
            {
                // Scroll through owned identities
                return CharacterIdentities.Select(id => id.CCPCharacter).FirstOrDefault(
                    ccpCharacter => ccpCharacter != null && ccpCharacter.IsTraining);
            }
        }

        /// <summary>
        /// Gets true if this API key has a character in training.
        /// </summary>
        public bool HasCharacterInTraining
        {
            get { return TrainingCharacter != null; }
        }

        /// <summary>
        /// Gets true if this API key is a corporation type.
        /// </summary>
        public bool IsCorporationType
        {
            get { return Type == APIKeyType.Corporation; }
        }

        /// <summary>
        /// Gets true if this API key is a character or account type.
        /// </summary>
        public bool IsCharacterOrAccountType
        {
            get { return Type == APIKeyType.Account || Type == APIKeyType.Character; }
        }

        /// <summary>
        /// Gets true if this API key got queried or is not monitored.
        /// </summary>
        public bool IsProcessed
        {
            get { return m_queried || !m_monitored; }
        }

        #endregion


        #region Internal Methods

        /// <summary>
        /// Query skills in training for characters on this API key.
        /// </summary>
        internal void CharacterInTraining()
        {
            if (m_updatePending)
                return;

            m_updatePending = true;
            m_skillInTrainingCache.Clear();

            // Quits if no network
            if (!NetworkMonitor.IsNetworkAvailable)
                return;

            // Quits if not an 'Account' type API key
            if (Type != APIKeyType.Account)
                return;

            foreach (CharacterIdentity id in CharacterIdentities)
            {
                string identity = id.CharacterName;

                if (!m_skillInTrainingCache.ContainsKey(identity))
                    m_skillInTrainingCache.Add(identity, new SkillInTrainingResponse());

                EveMonClient.APIProviders.CurrentProvider.QueryMethodAsync<SerializableAPISkillInTraining>(
                    APICharacterMethods.SkillInTraining, ID, VerificationCode, id.CharacterID,
                    x => OnSkillInTrainingUpdated(x, identity));
            }
        }

        #endregion


        #region Inherited events

        /// <summary>
        /// Called when the object gets disposed.
        /// </summary>
        internal void Dispose()
        {
            // Unsubscribe events
            EveMonClient.TimerTick -= EveMonClient_TimerTick;

            // Dispose monitor events
            m_apiKeyInfoMonitor.Dispose();
            m_accountStatusMonitor.Dispose();
        }

        #endregion


        #region Global Events

        /// <summary>
        /// Updates the API key info and account status on a timer tick.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_TimerTick(object sender, EventArgs e)
        {
            m_apiKeyInfoMonitor.Enabled = m_monitored;

            // We trigger the account status check when we have the character list of the API key
            // in order to have better API key related info in the trace file
            m_accountStatusMonitor.Enabled = m_characterListUpdated && m_monitored && IsCharacterOrAccountType;
        }

        #endregion


        #region Queries response

        /// <summary>
        /// Used when the API key info (character list) has been queried.
        /// </summary>
        /// <param name="result"></param>
        private void OnAPIKeyInfoUpdated(APIResult<SerializableAPIKeyInfo> result)
        {
            m_queried = true;

            // Quit if the API key was deleted while it was updating
            if (!EveMonClient.APIKeys.Contains(this))
                return;

            // Checks if EVE database is out of service
            if (result.EVEDatabaseError)
                return;

            // Notify on error
            if (result.HasError)
            {
                // Fire the event in order to trigger the monitoring of a character's features
                // regardless of the result having error
                EveMonClient.OnAPIKeyInfoUpdated(this);
                
                // Notify the user
                EveMonClient.Notifications.NotifyCharacterListError(this, result);
                return;
            }

            // Invalidates the notification
            EveMonClient.Notifications.InvalidateAPIKeyInfoError(this);

            // Update
            Import(result);

            // Notifies for the API key expiration
            NotifyAPIKeyExpiration();

            // Fires the event regarding the API key info update
            EveMonClient.OnAPIKeyInfoUpdated(this);
        }

        /// <summary>
        /// Called when the account status has been updated.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnAccountStatusUpdated(APIResult<SerializableAPIAccountStatus> result)
        {
            // Quit if the API key was deleted while it was updating
            if (!EveMonClient.APIKeys.Contains(this))
                return;

            // Checks if EVE database is out of service
            if (result.EVEDatabaseError)
                return;

            // Return on error
            if (result.HasError)
            {
                EveMonClient.Notifications.NotifyAccountStatusError(this, result);
                return;
            }

            EveMonClient.Notifications.InvalidateAccountStatusError(this);

            AccountCreated = result.Result.CreateDate;
            AccountExpires = result.Result.PaidUntil;

            // Notifies for the account expiration
            NotifyAccountExpiration();

            // Fires the event regarding the account status update
            EveMonClient.OnAccountStatusUpdated(this);
        }

        /// <summary>
        /// Called when character's skill in training gets updated.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="characterName">The character's name.</param>
        private void OnSkillInTrainingUpdated(APIResult<SerializableAPISkillInTraining> result, string characterName)
        {
            // Quit if the API key was deleted while it was updating
            if (!EveMonClient.APIKeys.Contains(this))
                return;

            CCPCharacter ccpCharacter = EveMonClient.Characters.OfType<CCPCharacter>().FirstOrDefault(x => x.Name == characterName);

            // Checks if EVE database is out of service
            if (result.EVEDatabaseError)
                return;

            // Return on error
            if (result.HasError)
            {
                if (ccpCharacter != null && ccpCharacter.ShouldNotifyError(result, APICharacterMethods.SkillInTraining))
                    EveMonClient.Notifications.NotifySkillInTrainingError(ccpCharacter, result);

                m_skillInTrainingCache[characterName].State = ResponseState.InError;
                return;
            }

            m_skillInTrainingCache[characterName].State = result.Result.SkillInTraining == 1
                                                              ? ResponseState.Training
                                                              : ResponseState.NotTraining;

            // In the event this becomes a very long running process because of latency
            // and characters have been removed from the API key since they were queried
            // remove those characters from the cache
            IEnumerable<KeyValuePair<string, SkillInTrainingResponse>> toRemove =
                m_skillInTrainingCache.Where(x => CharacterIdentities.All(y => y.CharacterName != x.Key));

            foreach (KeyValuePair<string, SkillInTrainingResponse> charToRemove in toRemove)
            {
                m_skillInTrainingCache.Remove(charToRemove.Key);
            }

            // If we did not get response from a character in API key yet
            // or there was an error in any responce,
            // we are not sure so wait until next time
            if (m_skillInTrainingCache.Any(x => x.Value.State == ResponseState.Unknown
                                                || x.Value.State == ResponseState.InError))
                return;

            // We have successful responces from all characters in API key,
            // so we notify the user and fire the event
            NotifyAccountNotInTraining();

            // Fires the event regarding the API key characters skill in training update
            EveMonClient.OnCharactersSkillInTrainingUpdated(this);

            // Reset update pending flag
            m_updatePending = false;
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Forces the update.
        /// </summary>
        public void ForceUpdate()
        {
            ((IQueryMonitorEx)m_apiKeyInfoMonitor).ForceUpdate();
            ((IQueryMonitorEx)m_accountStatusMonitor).ForceUpdate();
        }

        /// <summary>
        /// Notifies for the API key expiration.
        /// </summary>
        private void NotifyAPIKeyExpiration()
        {
            // Is it to expire within 7 days? Send an informative notification
            TimeSpan daysToExpire = Expiration.Subtract(DateTime.UtcNow);
            if (daysToExpire < TimeSpan.FromDays(7) && daysToExpire > TimeSpan.FromDays(1))
            {
                EveMonClient.Notifications.NotifyAPIKeyExpiration(this, Expiration, NotificationPriority.Information);
                return;
            }

            // Is it to expire within the day? Send a warning notification
            if (daysToExpire <= TimeSpan.FromDays(1) && daysToExpire > TimeSpan.Zero)
            {
                EveMonClient.Notifications.NotifyAPIKeyExpiration(this, Expiration, NotificationPriority.Warning);
                return;
            }

            EveMonClient.Notifications.InvalidateAPIKeyExpiration(this);
        }

        /// <summary>
        /// Notifies for the account expiration.
        /// </summary>
        private void NotifyAccountExpiration()
        {
            // Is it to expire within 7 days? Send an informative notification
            TimeSpan daysToExpire = AccountExpires.Subtract(DateTime.UtcNow);
            if (daysToExpire < TimeSpan.FromDays(7) && daysToExpire > TimeSpan.FromDays(1))
            {
                EveMonClient.Notifications.NotifyAccountExpiration(this, AccountExpires, NotificationPriority.Information);
                return;
            }

            // Is it to expire within the day? Send a warning notification
            if (daysToExpire <= TimeSpan.FromDays(1) && daysToExpire > TimeSpan.Zero)
            {
                EveMonClient.Notifications.NotifyAccountExpiration(this, AccountExpires, NotificationPriority.Warning);
                return;
            }

            EveMonClient.Notifications.InvalidateAccountExpiration(this);
        }

        /// <summary>
        /// Notifies if an account is not in training.
        /// </summary>
        private void NotifyAccountNotInTraining()
        {
            // One of the remaining characters was training; account is training
            if (m_skillInTrainingCache.Any(x => x.Value.State == ResponseState.Training))
            {
                EveMonClient.Notifications.InvalidateAccountNotInTraining(this);
                return;
            }

            // No training characters found up until
            EveMonClient.Notifications.NotifyAccountNotInTraining(this);
        }

        #endregion


        #region Static Methods

        /// <summary>
        /// Tries the add or update async the API key.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="verificationCode">The verification code.</param>
        /// <param name="callback">The callback.</param>
        public static void TryAddOrUpdateAsync(long id, string verificationCode,
                                               EventHandler<APIKeyCreationEventArgs> callback)
        {
            EveMonClient.APIProviders.CurrentProvider.QueryMethodAsync<SerializableAPIKeyInfo>(
                APIGenericMethods.APIKeyInfo, id, verificationCode,
                result => callback(null, new APIKeyCreationEventArgs(id, verificationCode, result)));
        }

        /// <summary>
        /// Gets the credential level from the given result.
        /// </summary>
        internal static APIKeyType GetCredentialsType(APIResult<SerializableAPIKeyInfo> apiKeyInfo)
        {
            // An error occurred
            if (apiKeyInfo.HasError)
                return APIKeyType.Unknown;

            if (Enum.IsDefined(typeof(APIKeyType), apiKeyInfo.Result.Key.Type))
                return (APIKeyType)Enum.Parse(typeof(APIKeyType), apiKeyInfo.Result.Key.Type);

            // Another error occurred
            return APIKeyType.Unknown;
        }

        /// <summary>
        /// Check whether some accounts are not in training.
        /// </summary>
        /// <param name="message">Message describing the accounts not in training.</param>
        /// <returns>True if one or more accounts is not in training, otherwise false.</returns>
        /// <remarks>This condition applied only to those API keys of type 'Account'</remarks>
        public static bool HasCharactersNotTraining(out string message)
        {
            message = String.Empty;

            List<APIKey> accountsNotTraining = EveMonClient.APIKeys.Where(x => x.Type == APIKeyType.Account &&
                                                                               x.CharacterIdentities.Any() &&
                                                                               !x.HasCharacterInTraining).ToList();

            // All accounts are training ?
            if (!accountsNotTraining.Any())
                return false;

            // Creates the string, scrolling through every not training account
            StringBuilder builder = new StringBuilder();
            if (accountsNotTraining.Count() == 1)
            {
                builder.AppendFormat("{0} is not in training", (EveMonClient.APIKeys.Count == 1
                                                                    ? "The account"
                                                                    : "One of the accounts"));
            }
            else
                builder.Append("Some of the accounts are not in training.");

            foreach (APIKey apiKey in accountsNotTraining)
            {
                builder.AppendLine();
                builder.AppendFormat(CultureConstants.DefaultCulture, "API key : {0}", apiKey);
            }

            message = builder.ToString();
            return true;
        }

        #endregion


        #region Importation / Exportation

        /// <summary>
        /// Updates the API key info and characters list with the given CCP data.
        /// </summary>
        /// <param name="result"></param>
        private void Import(APIResult<SerializableAPIKeyInfo> result)
        {
            Type = GetCredentialsType(result);
            AccessMask = result.Result.Key.AccessMask;
            Expiration = result.Result.Key.Expiration;

            ImportIdentities(result.HasError ? null : result.Result.Key.Characters);
        }

        /// <summary>
        /// Updates the characters list with the given CCP data.
        /// </summary>
        /// <param name="identities"></param>
        private void ImportIdentities(IEnumerable<ISerializableCharacterIdentity> identities)
        {
            // Clear the API key on this character
            foreach (CharacterIdentity id in EveMonClient.CharacterIdentities.Where(id => id.APIKeys.Contains(this)))
            {
                id.APIKeys.Remove(this);
            }

            // Return if there were errors in the query
            if (identities == null)
                return;

            // Assign owned identities to this API key
            foreach (CharacterIdentity id in identities.Select(
                serialID => EveMonClient.CharacterIdentities[serialID.ID] ??
                            EveMonClient.CharacterIdentities.Add(serialID.ID, serialID.Name,
                                                                 serialID.CorporationID, serialID.CorporationName)))
            {
                // Update the corporation info as they may have changed
                ISerializableCharacterIdentity characterIdentity = identities.First(x => x.ID == id.CharacterID);
                id.CorporationID = characterIdentity.CorporationID;
                id.CorporationName = characterIdentity.CorporationName;

                // Add the API key to the identity
                id.APIKeys.Add(this);

                if (id.CCPCharacter == null)
                    continue;

                // Update the corporation info
                id.CCPCharacter.CorporationID = id.CorporationID;
                id.CCPCharacter.CorporationName = id.CorporationName;

                // Notify subscribers
                EveMonClient.OnCharacterUpdated(id.CCPCharacter);
            }

            m_characterListUpdated = true;

            // Fires the event regarding the character list update
            EveMonClient.OnCharacterListUpdated(this);
        }

        /// <summary>
        /// Exports the data to a serialization object.
        /// </summary>
        /// <returns></returns>
        internal SerializableAPIKey Export()
        {
            SerializableAPIKey serial = new SerializableAPIKey
                                            {
                                                ID = ID,
                                                VerificationCode = VerificationCode,
                                                Type = Type,
                                                AccessMask = AccessMask,
                                                Expiration = Expiration,
                                                Monitored = m_monitored,
                                                LastUpdate = m_apiKeyInfoMonitor.LastUpdate,
                                            };
            serial.IgnoreList.AddRange(IdentityIgnoreList.Export());

            return serial;
        }

        #endregion


        #region Update Methods

        /// <summary>
        /// Asynchronously updates this API key through a <see cref="APIKeyCreationEventArgs"/>.
        /// </summary>
        /// <param name="verificationCode"></param>
        /// <param name="callback">A callback invoked on the UI thread (whatever the result, success or failure)</param>
        /// <returns></returns>
        public void TryUpdateAsync(string verificationCode, EventHandler<APIKeyCreationEventArgs> callback)
        {
            TryAddOrUpdateAsync(ID, verificationCode, callback);
        }

        /// <summary>
        /// Updates the API key.
        /// </summary>
        /// <param name="e">The <see cref="APIKeyCreationEventArgs"/> instance containing the event data.</param>
        public void Update(APIKeyCreationEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            VerificationCode = e.VerificationCode;
            AccessMask = e.AccessMask;
            Type = e.Type;
            Expiration = e.Expiration;
            m_apiKeyInfoMonitor.UpdateWith(e.APIKeyInfo);

            // Notifies for the API key expiration
            NotifyAPIKeyExpiration();

            // Clear the API key for the currently associated identities
            foreach (CharacterIdentity id in EveMonClient.CharacterIdentities.Where(id => id.APIKeys.Contains(this)))
            {
                id.APIKeys.Remove(this);
            }

            // Assign this API key to the new identities and create CCP characters
            foreach (CharacterIdentity id in e.Identities)
            {
                // Update the corporation info as they may have changed
                id.CorporationID = e.Identities.First(x => x.CharacterID == id.CharacterID).CorporationID;
                id.CorporationName = e.Identities.First(x => x.CharacterID == id.CharacterID).CorporationName;

                id.APIKeys.Add(this);

                // Skip if in the ignore list
                if (IdentityIgnoreList.Contains(id))
                    continue;

                // Retrieves the ccp character and create one if none
                if (id.CCPCharacter != null)
                {
                    // Update the corporation info
                    id.CCPCharacter.CorporationID = id.CorporationID;
                    id.CCPCharacter.CorporationName = id.CorporationName;

                    // Notify subscribers
                    EveMonClient.OnCharacterUpdated(id.CCPCharacter);
                    continue;
                }

                EveMonClient.Characters.Add(new CCPCharacter(id));
            }
        }

        #endregion


        #region Overridden Methods

        /// <summary>
        /// Gets a string representation of this API key, under the given format : 123456 (John Doe, Jane Doe).
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // If no characters on this API key, return only the API key ID
            if (!CharacterIdentities.Any())
                return ID.ToString(CultureConstants.DefaultCulture);

            // Otherwise, return the chars' names into parenthesis
            StringBuilder names = new StringBuilder();
            foreach (CharacterIdentity id in CharacterIdentities)
            {
                names.Append(id.CharacterName);
                if (id != CharacterIdentities.Last())
                    names.Append(", ");
            }
            return String.Format(CultureConstants.DefaultCulture, "{0} ({1})", ID, names);
        }

        #endregion


        #region Helper Class


        #region Nested type: ResponseState

        private enum ResponseState
        {
            Unknown,
            InError,
            Training,
            NotTraining
        }

        #endregion


        #region Nested type: SkillInTrainingResponse

        private class SkillInTrainingResponse
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SkillInTrainingResponse"/> class.
            /// </summary>
            public SkillInTrainingResponse()
            {
                State = ResponseState.Unknown;
            }

            /// <summary>
            /// Gets or sets the state.
            /// </summary>
            /// <value>The state.</value>
            public ResponseState State { get; set; }
        }

        #endregion


        #endregion

    }
}
