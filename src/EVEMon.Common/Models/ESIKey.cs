using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EVEMon.Common.Attributes;
using EVEMon.Common.Constants;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Extensions;
using EVEMon.Common.Interfaces;
using EVEMon.Common.Net;
using EVEMon.Common.QueryMonitor;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Serialization.Settings;
using System.Xml.Serialization;
using EVEMon.Common.Service;

namespace EVEMon.Common.Models
{
    /// <summary>
    /// Represents a player ESI key.
    /// </summary>
    [EnforceUIThreadAffinity]
    public sealed class ESIKey
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
        private ESIKey()
        {
            m_apiKeyInfoMonitor = new APIKeyQueryMonitor<SerializableAPIKeyInfo>(this, CCPAPIGenericMethods.APIKeyInfo,
                                                                                 OnAPIKeyInfoUpdated);

            m_accountStatusMonitor = new APIKeyQueryMonitor<SerializableAPIAccountStatus>(this, CCPAPICharacterMethods.AccountStatus,
                                                                                          OnAccountStatusUpdated);

            EveMonClient.TimerTick += EveMonClient_TimerTick;
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="serial"></param>
        internal ESIKey(SerializableESIKey serial)
            : this()
        {
            ID = serial.ID;
            RefreshToken = serial.RefreshToken;
            AccessMask = serial.AccessMask;
            m_monitored = serial.Monitored;
            Type = CCPAPIKeyType.Character;
        }

        /// <summary>
        /// Constructor from the provided informations.
        /// </summary>
        /// <param name="id"></param>
        public ESIKey(long id)
            : this()
        {
            ID = id;
            RefreshToken = string.Empty;
            m_monitored = true;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public long ID { get; }

        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        /// <value>The access token.</value>
        [XmlIgnore]
        public string AccessToken { get; private set; }

        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        /// <value>The refresh token.</value>
        [XmlIgnore]
        public string RefreshToken { get; private set; }

        /// <summary>
        /// Gets or sets the access mask.
        /// </summary>
        /// <value>The access mask.</value>
        [XmlIgnore]
        public ulong AccessMask { get; private set; }

        /// <summary>
        /// Gets or sets the type of the key.
        /// </summary>
        /// <value>The type of the key.</value>
        [XmlIgnore]
        public CCPAPIKeyType Type { get; private set; }

#if false
        /// <summary>
        /// Gets the account expiration date and time. RIP Account status API.
        /// </summary>
        public DateTime AccountExpires { get; set; }
#endif

        /// <summary>
        /// Gets the character identities for this API key.
        /// </summary>
        public IEnumerable<CharacterIdentity> CharacterIdentities 
            => EveMonClient.CharacterIdentities.Where(characterID => characterID.ESIKeys.Contains(this));

        /// <summary>
        /// Gets the cached until date and time of the last result.
        /// </summary>
        public DateTime CachedUntil => m_apiKeyInfoMonitor.LastResult?.CachedUntil ?? DateTime.UtcNow;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ESIKey"/> is monitored.
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
        public bool HasMonitoredCharacters => CharacterIdentities.Select(id => id.CCPCharacter)
            .Any(ccpCharacter => ccpCharacter != null && ccpCharacter.Monitored);

        /// <summary>
        /// Gets the character in training on this API key, or null if none are in training.
        /// </summary>
        /// <remarks>Returns null if the character is in the ignored list.</remarks>
        // Scroll through owned identities
        public CCPCharacter TrainingCharacter => CharacterIdentities.Select(id => id.CCPCharacter)
            .FirstOrDefault(ccpCharacter => ccpCharacter != null && ccpCharacter.IsTraining);

        /// <summary>
        /// Gets true if this API key has a character in training.
        /// </summary>
        public bool HasCharacterInTraining => TrainingCharacter != null;
        
        /// <summary>
        /// Gets true if this API key got queried or is not monitored.
        /// </summary>
        public bool IsProcessed => m_queried || !m_monitored;

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
            
            foreach (CharacterIdentity id in CharacterIdentities)
            {
                string identity = id.CharacterName;

                if (!m_skillInTrainingCache.ContainsKey(identity))
                    m_skillInTrainingCache.Add(identity, new SkillInTrainingResponse());

                EveMonClient.APIProviders.CurrentProvider.QueryMethodAsync<SerializableAPISkillInTraining>(
                    CCPAPICharacterMethods.SkillInTraining, ID, AccessToken, id.CharacterID,
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
            m_accountStatusMonitor.Enabled = m_characterListUpdated && m_monitored;
        }

        #endregion


        #region Queries response

        /// <summary>
        /// Used when the API key info (character list) has been queried.
        /// </summary>
        /// <param name="result"></param>
        private void OnAPIKeyInfoUpdated(CCPAPIResult<SerializableAPIKeyInfo> result)
        {
            m_queried = true;

            // Quit if the API key was deleted while it was updating
            if (!EveMonClient.ESIKeys.Contains(this))
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

            // Fires the event regarding the API key info update
            EveMonClient.OnAPIKeyInfoUpdated(this);
        }

        /// <summary>
        /// Called when the account status has been updated.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnAccountStatusUpdated(CCPAPIResult<SerializableAPIAccountStatus> result)
        {
            // Quit if the API key was deleted while it was updating
            if (!EveMonClient.ESIKeys.Contains(this))
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
        private void OnSkillInTrainingUpdated(CCPAPIResult<SerializableAPISkillInTraining> result, string characterName)
        {
            // Quit if the API key was deleted while it was updating
            if (!EveMonClient.ESIKeys.Contains(this))
                return;

            CCPCharacter ccpCharacter = EveMonClient.Characters.OfType<CCPCharacter>().FirstOrDefault(x => x.Name == characterName);

            // Checks if EVE database is out of service
            if (result.EVEDatabaseError)
                return;

            // Return on error
            if (result.HasError)
            {
                if (ccpCharacter != null && ccpCharacter.ShouldNotifyError(result, CCPAPICharacterMethods.SkillInTraining))
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
        /// Notifies for the account expiration.
        /// </summary>
        private void NotifyAccountExpiration()
        {
            // No Account Status API in ESI
#if false
            DateTime AccountExpires = DateTime.MaxValue;

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
#endif
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
        /// Tries to add or update the ESI key.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="accessResponse">The access and refresh token.</param>
        /// <param name="callback">The callback.</param>
        public static void TryAddOrUpdateAsync(long id, AccessResponse accessResponse,
                                               EventHandler<ESIKeyCreationEventArgs> callback)
        {
            accessResponse.ThrowIfNull(nameof(accessResponse));
            SSOAuthenticationService.BeginGetTokenInfo(accessResponse.AccessToken,
                (result) => callback(null, new ESIKeyCreationEventArgs(id, accessResponse.
                RefreshToken, result)));
        }

        /// <summary>
        /// Gets the credential level from the given result.
        /// </summary>
        internal static CCPAPIKeyType GetCredentialsType(CCPAPIResult<SerializableAPIKeyInfo> apiKeyInfo)
        {
            // An error occurred
            if (apiKeyInfo.HasError)
                return CCPAPIKeyType.Unknown;

            if (Enum.IsDefined(typeof(CCPAPIKeyType), apiKeyInfo.Result.Key.Type))
                return (CCPAPIKeyType)Enum.Parse(typeof(CCPAPIKeyType), apiKeyInfo.Result.Key.Type);

            // Another error occurred
            return CCPAPIKeyType.Unknown;
        }

        /// <summary>
        /// Check whether some accounts are not in training.
        /// </summary>
        /// <param name="message">Message describing the accounts not in training.</param>
        /// <returns>True if one or more accounts is not in training, otherwise false.</returns>
        /// <remarks>This condition applied only to those API keys of type 'Account'</remarks>
        public static bool HasCharactersNotTraining(out string message)
        {
            message = string.Empty;

            List<ESIKey> accountsNotTraining = EveMonClient.ESIKeys.Where(
                esiKey => esiKey.CharacterIdentities.Any() && !esiKey.HasCharacterInTraining)
                .ToList();

            // All accounts are training ?
            if (!accountsNotTraining.Any())
                return false;

            // Creates the string, scrolling through every not training account
            StringBuilder builder = new StringBuilder();
            builder.Append(accountsNotTraining.Count == 1
                ? $"{(EveMonClient.ESIKeys.Count == 1 ? "The account" : "One of the accounts")} is not in training"
                : "Some of the accounts are not in training.");

            foreach (ESIKey esiKey in accountsNotTraining)
            {
                builder.AppendLine().Append($"ESI key : {esiKey}");
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
        private void Import(CCPAPIResult<SerializableAPIKeyInfo> result)
        {
            Type = GetCredentialsType(result);
            AccessMask = result.Result.Key.AccessMask;

            ImportIdentities(result.HasError ? null : result.Result.Key.Characters);
        }

        /// <summary>
        /// Updates the characters list with the given CCP data.
        /// </summary>
        /// <param name="identities"></param>
        private void ImportIdentities(IEnumerable<ISerializableCharacterIdentity> identities)
        {
            // Clear the API key on this character
            foreach (CharacterIdentity id in EveMonClient.CharacterIdentities.Where(id => id.ESIKeys.Contains(this)))
            {
                id.ESIKeys.Remove(this);
            }

            // Return if there were errors in the query
            if (identities == null)
                return;

            // Assign owned identities to this API key
            List<ISerializableCharacterIdentity> serializableCharacterIdentities = identities.ToList();
            foreach (CharacterIdentity id in serializableCharacterIdentities.Select(
                serialID => EveMonClient.CharacterIdentities[serialID.ID] ??
                            EveMonClient.CharacterIdentities.Add(serialID.ID, serialID.Name)))
            {
                // Update the other info as they may have changed
                ISerializableCharacterIdentity characterIdentity = serializableCharacterIdentities.First(x => x.ID == id.CharacterID);
                id.CorporationID = characterIdentity.CorporationID;
                id.CorporationName = characterIdentity.CorporationName;
                id.AllianceID = characterIdentity.AllianceID;
                id.AllianceName = characterIdentity.AllianceName;
                id.FactionID = characterIdentity.FactionID;
                id.FactionName = characterIdentity.FactionName;

                // Add the API key to the identity
                id.ESIKeys.Add(this);

                if (id.CCPCharacter == null)
                    continue;

                // Update the other info
                id.CCPCharacter.CorporationID = id.CorporationID;
                id.CCPCharacter.CorporationName = id.CorporationName;
                id.CCPCharacter.AllianceID = id.AllianceID;
                id.CCPCharacter.AllianceName = id.AllianceName;
                id.CCPCharacter.FactionID = id.FactionID;
                id.CCPCharacter.FactionName = id.FactionName;

                // Notify subscribers
                EveMonClient.OnCharacterUpdated(id.CCPCharacter);
            }

            m_characterListUpdated = true;

            // Fires the event regarding the character list update
            EveMonClient.OnCharacterListUpdated(this);

            // API collection changed, so we'll need to reprocess accounts.
            EveMonClient.OnAPIKeyCollectionChanged();
        }

        /// <summary>
        /// Exports the data to a serialization object.
        /// </summary>
        /// <returns></returns>
        internal SerializableESIKey Export()
        {
            SerializableESIKey serial = new SerializableESIKey
            {
                ID = ID,
                RefreshToken = AccessToken,
                AccessMask = AccessMask,
                Monitored = m_monitored,
                LastUpdate = m_apiKeyInfoMonitor.LastUpdate,
            };

            return serial;
        }

        #endregion


        #region Update Methods

        /// <summary>
        /// Asynchronously updates this ESI key through a <see cref="ESIKeyCreationEventArgs"/>.
        /// </summary>
        /// <param name="accessResponse">The access and refresh token.</param>
        /// <param name="callback">A callback invoked on the UI thread (whatever the result, success or failure)</param>
        /// <returns></returns>
        public void TryUpdateAsync(AccessResponse accessResponse, EventHandler<ESIKeyCreationEventArgs> callback)
        {
            TryAddOrUpdateAsync(ID, accessResponse, callback);
        }

        /// <summary>
        /// Updates the ESI key.
        /// </summary>
        /// <param name="e">The <see cref="ESIKeyCreationEventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.ArgumentNullException">e</exception>
        public void Update(ESIKeyCreationEventArgs e)
        {
            e.ThrowIfNull(nameof(e));

            AccessToken = e.RefreshToken;
            AccessMask = e.AccessMask;
            //m_apiKeyInfoMonitor.UpdateWith(e.APIKeyInfo);

            // Clear the ESI key for the currently associated identities
            foreach (CharacterIdentity id in EveMonClient.CharacterIdentities.Where(id => id.ESIKeys.Contains(this)))
                id.ESIKeys.Remove(this);

            // Assign this API key to the new identities and create CCP characters
            var cid = e.Identity;
            // Update the corporation info as they may have changed
            cid.CorporationID = e.Identity.CorporationID;
            cid.CorporationName = e.Identity.CorporationName;

            cid.ESIKeys.Add(this);
                
            // Retrieves the ccp character and create one if none
            if (cid.CCPCharacter != null)
            {
                // Update the corporation info
                cid.CCPCharacter.CorporationID = cid.CorporationID;
                cid.CCPCharacter.CorporationName = cid.CorporationName;

                // Notify subscribers
                EveMonClient.OnCharacterUpdated(cid.CCPCharacter);
            }
            else
                EveMonClient.Characters.Add(new CCPCharacter(cid));
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
            return $"{ID} ({names})";
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
