using System;
using System.Collections.Generic;
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
        private readonly APIKeyQueryMonitor<SerializableAPIKeyInfo> m_apiKeyInfoMonitor;
        private readonly APIKeyQueryMonitor<SerializableAPIAccountStatus> m_accountStatusMonitor;

        private readonly Dictionary<string, SkillInTrainingResponse> m_skillInTrainingCache =
            new Dictionary<string, SkillInTrainingResponse>();

        private bool m_updatePending;
        private bool m_characterListUpdated;


        #region Constructors

        /// <summary>
        /// Common constructor base.
        /// </summary>
        private APIKey()
        {
            m_apiKeyInfoMonitor = new APIKeyQueryMonitor<SerializableAPIKeyInfo>(this, APIMethods.APIKeyInfo);
            m_apiKeyInfoMonitor.Updated += OnAPIKeyInfoUpdated;

            m_accountStatusMonitor = new APIKeyQueryMonitor<SerializableAPIAccountStatus>(this, APIMethods.AccountStatus);
            m_accountStatusMonitor.Updated += OnAccountStatusUpdated;

            IdentityIgnoreList = new CharacterIdentityIgnoreList(this);

            EveMonClient.TimerTick += EveMonClient_TimerTick;
        }

        /// <summary>
        /// Updates the API key info and account status on a timer tick.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_TimerTick(object sender, EventArgs e)
        {
            m_apiKeyInfoMonitor.Enabled = Monitored;

            // We trigger the account status check when we have the character list of the API key
            // in order to have better API key related info in the trace file
            m_accountStatusMonitor.Enabled = (m_characterListUpdated && Monitored);
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
            Monitored = serial.Monitored;
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
            Monitored = true;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public long ID { get; set; }

        /// <summary>
        /// Gets or sets the verification code.
        /// </summary>
        /// <value>The verification code.</value>
        public string VerificationCode { get; set; }

        /// <summary>
        /// Gets or sets the access mask.
        /// </summary>
        /// <value>The access mask.</value>
        public long AccessMask { get; set; }

        /// <summary>
        /// Gets or sets the type of the key.
        /// </summary>
        /// <value>The type of the key.</value>
        public APIKeyType Type { get; set; }

        /// <summary>
        /// Gets or sets the key expiration.
        /// </summary>
        /// <value>The key expiration.</value>
        public DateTime Expiration { get; set; }

        /// <summary>
        /// Gets the account expiration date and time.
        /// </summary>
        public DateTime AccountExpires { get; set; }

        /// <summary>
        /// Gets the account creation date and time.
        /// </summary>
        public DateTime AccountCreated { get; set; }

        /// <summary>
        /// Gets the list of items to never import.
        /// </summary>
        public CharacterIdentityIgnoreList IdentityIgnoreList { get; set; }

        /// <summary>
        /// Gets the character identities for this API key.
        /// </summary>
        public IEnumerable<CharacterIdentity> CharacterIdentities
        {
            get { return EveMonClient.CharacterIdentities.Where(characterID => characterID.APIKey == this); }
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
        public bool Monitored { get; set; }

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

            // Quits if access denied
            if ((int)APIMethods.CharacterSkillInTraining != (AccessMask & (int)APIMethods.CharacterSkillInTraining))
                return;

            foreach (CharacterIdentity id in CharacterIdentities)
            {
                string identity = id.Name;

                if (!m_skillInTrainingCache.ContainsKey(identity))
                    m_skillInTrainingCache.Add(identity, new SkillInTrainingResponse());

                EveMonClient.APIProviders.CurrentProvider.QueryMethodAsync<SerializableAPISkillInTraining>(
                    APIMethods.CharacterSkillInTraining, ID, VerificationCode, id.CharacterID,
                    x => OnSkillInTrainingUpdated(x, identity));
            }
        }

        #endregion


        #region Queries response

        /// <summary>
        /// Used when the API key info (character list) has been queried.
        /// </summary>
        /// <param name="result"></param>
        private void OnAPIKeyInfoUpdated(APIResult<SerializableAPIKeyInfo> result)
        {
            // Notify on error
            if (result.HasError)
            {
                // Checks if EVE Backend Database is temporarily disabled
                if (result.EVEBackendDatabaseDisabled)
                    return;

                EveMonClient.Notifications.NotifyCharacterListError(this, result);

                return;
            }

            // Invalidates the notification
            EveMonClient.Notifications.InvalidateAPIKeyInfoError(this);

            // Update
            Import(result);

            // Notifies for the API key expiration
            NotifyAPIKeyExpiration();
        }

        /// <summary>
        /// Called when the account status has been updated.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnAccountStatusUpdated(APIResult<SerializableAPIAccountStatus> result)
        {
            // Return on error
            if (result.HasError)
            {
                // Checks if EVE Backend Database is temporarily disabled
                if (result.EVEBackendDatabaseDisabled)
                    return;

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
            CCPCharacter ccpCharacter = EveMonClient.Characters.FirstOrDefault(x => x.Name == characterName) as CCPCharacter;

            // Return on error
            if (result.HasError)
            {
                // Checks if EVE Backend Database is temporarily disabled
                if (result.EVEBackendDatabaseDisabled)
                    return;

                if (ccpCharacter != null)
                    EveMonClient.Notifications.NotifySkillInTrainingError(ccpCharacter, result);

                m_skillInTrainingCache[characterName].State = ResponseState.InError;
                return;
            }

            if (ccpCharacter != null)
                EveMonClient.Notifications.InvalidateCharacterAPIError(ccpCharacter);

            m_skillInTrainingCache[characterName].State = result.Result.SkillInTraining == 1
                                                              ? ResponseState.Training
                                                              : ResponseState.NotTraining;

            // In the event this becomes a very long running process because of latency
            // and characters have been removed from the API key since they were queried
            // remove those characters from the cache
            IEnumerable<KeyValuePair<string, SkillInTrainingResponse>> toRemove =
                m_skillInTrainingCache.Where(x => !CharacterIdentities.Any(y => y.Name == x.Key));

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
                APIMethods.APIKeyInfo, id, verificationCode,
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

            IEnumerable<APIKey> accountsNotTraining = EveMonClient.APIKeys.Where(x => x.Type == APIKeyType.Account &&
                                                                                      !x.CharacterIdentities.IsEmpty() &&
                                                                                      !x.HasCharacterInTraining);

            // All accounts are training ?
            if (accountsNotTraining.Count() == 0)
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

            // Fires the event regarding the API key info update
            EveMonClient.OnAPIKeyInfoUpdated(this);

            ImportIdentities(result.HasError ? null : result.Result.Key.Characters);

            // Fires the event regarding the character list update
            EveMonClient.OnCharacterListUpdated(this);
            m_characterListUpdated = true;
        }

        /// <summary>
        /// Updates the characters list with the given CCP data.
        /// </summary>
        /// <param name="identities"></param>
        private void ImportIdentities(IEnumerable<ISerializableCharacterIdentity> identities)
        {
            // Clear the API key on this character
            foreach (CharacterIdentity id in EveMonClient.CharacterIdentities.Where(id => id.APIKey == this))
            {
                id.APIKey = null;
            }

            // Return if there were errors in the query
            if (identities == null)
                return;

            // Assign owned identities to this API Key
            foreach (CharacterIdentity id in identities.Select(
                serialID => EveMonClient.CharacterIdentities[serialID.ID] ??
                            EveMonClient.CharacterIdentities.Add(serialID.ID, serialID.Name)))
            {
                id.APIKey = this;
            }
        }

        /// <summary>
        /// Exports the data to a serialization object.
        /// </summary>
        /// <returns></returns>
        internal SerializableAPIKey Export()
        {
            return new SerializableAPIKey
                       {
                           ID = ID,
                           VerificationCode = VerificationCode,
                           Type = Type,
                           AccessMask = AccessMask,
                           Expiration = Expiration,
                           Monitored = Monitored,
                           LastUpdate = m_apiKeyInfoMonitor.LastUpdate,
                           IgnoreList = IdentityIgnoreList.Export()
                       };
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
        /// Updates the API Key.
        /// </summary>
        /// <param name="e">The <see cref="APIKeyCreationEventArgs"/> instance containing the event data.</param>
        public void Update(APIKeyCreationEventArgs e)
        {

            VerificationCode = e.VerificationCode;
            AccessMask = e.AccessMask;
            Type = e.Type;
            Expiration = e.Expiration;
            m_apiKeyInfoMonitor.UpdateWith(e.APIKeyInfo);

            // Notifies for the API key expiration
            NotifyAPIKeyExpiration();

            // Clear the API key for the currently associated identities
            foreach (CharacterIdentity id in EveMonClient.CharacterIdentities.Where(id => id.APIKey == this))
            {
                id.APIKey = null;
            }

            // Assign this API key to the new identities and create CCP characters
            foreach (CharacterIdentity id in e.Identities)
            {
                // Skip if in the ignore list
                id.APIKey = this;
                if (IdentityIgnoreList.Contains(id))
                    continue;

                // Retrieves the ccp character and create one if none
                CCPCharacter ccpCharacter = id.CCPCharacter;
                if (ccpCharacter != null)
                    continue;

                ccpCharacter = new CCPCharacter(id);
                EveMonClient.Characters.Add(ccpCharacter, true);
                ccpCharacter.Monitored = true;
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
            if (CharacterIdentities.Count() == 0)
                return ID.ToString();

            // Otherwise, return the chars' names into parenthesis
            StringBuilder names = new StringBuilder();
            foreach (CharacterIdentity id in CharacterIdentities)
            {
                names.Append(id.Name);
                if (id != CharacterIdentities.Last())
                    names.Append(", ");
            }
            return String.Format("{0} ({1})", ID, names);
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
