using System;
using System.Collections.Generic;
using System.Linq;

using EVEMon.Common.Attributes;
using EVEMon.Common.Net;
using EVEMon.Common.Serialization;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents a player account
    /// </summary>
    [EnforceUIThreadAffinity]
    public sealed class Account
    {
        private readonly AccountQueryMonitor<SerializableAPICharacters> m_charactersListMonitor;
        private readonly AccountIgnoreList m_ignoreList;

        private readonly Dictionary<String, SkillQueueResponse> m_skillInTrainingCache =
            new Dictionary<String, SkillQueueResponse>();

        private readonly long m_userId;
        private string m_apiKey;
        private bool m_firstCheck = true;
        private CredentialsLevel m_keyLevel;
        private DateTime m_lastKeyLevelUpdate = DateTime.MinValue;
        private bool m_updatePending;

        #region Constructors

        /// <summary>
        /// Common constructor base.
        /// </summary>
        private Account()
        {
            m_charactersListMonitor = new AccountQueryMonitor<SerializableAPICharacters>(this, APIMethods.CharacterList);
            m_charactersListMonitor.Updated += OnCharactersListUpdated;

            m_ignoreList = new AccountIgnoreList(this);
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="serial"></param>
        internal Account(SerializableAccount serial)
            : this()
        {
            m_userId = serial.ID;
            m_apiKey = serial.Key;
            m_keyLevel = serial.KeyLevel;
            m_ignoreList.Import(serial.IgnoreList);
        }

        /// <summary>
        /// Constructor from the provided informations
        /// </summary>
        /// <param name="userID"></param>
        internal Account(long userID)
            : this()
        {
            m_userId = userID;
            m_apiKey = String.Empty;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets true whether this key is a full key.
        /// </summary>
        public CredentialsLevel KeyLevel
        {
            get { return m_keyLevel; }
        }

        /// <summary>
        /// Gets / sets the user's id
        /// </summary>
        public long UserID
        {
            get { return m_userId; }
        }

        /// <summary>
        /// Gets / sets the API key
        /// </summary>
        public string APIKey
        {
            get { return m_apiKey; }
        }

        /// <summary>
        /// Gets the list of items to never import.
        /// </summary>
        public AccountIgnoreList IgnoreList
        {
            get { return m_ignoreList; }
        }

        /// <summary>
        /// Gets true if this account has a character in training
        /// </summary>
        public bool HasCharacterInTraining
        {
            get { return TrainingCharacter != null; }
        }

        /// <summary>
        /// Gets the character identities for this account
        /// </summary>
        public IEnumerable<CharacterIdentity> CharacterIdentities
        {
            get
            {
                foreach (CharacterIdentity characterID in EveClient.CharacterIdentities)
                {
                    if (characterID.Account == this)
                        yield return characterID;
                }
            }
        }

        /// <summary>
        /// Gets true if at least one of the CCP characters is monitored.
        /// </summary>
        public bool HasMonitoredCharacters
        {
            get
            {
                foreach (CharacterIdentity id in CharacterIdentities)
                {
                    CCPCharacter ccpCharacter = id.CCPCharacter;
                    if (ccpCharacter != null && ccpCharacter.Monitored)
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Gets the character in training on this account, or null if none are in training.
        /// </summary>
        /// <remarks>Returns null if the character is in the ignored list.</remarks>
        public CCPCharacter TrainingCharacter
        {
            get
            {
                // Scroll through owned identities
                foreach (CharacterIdentity id in CharacterIdentities)
                {
                    CCPCharacter ccpCharacter = id.CCPCharacter;
                    if (ccpCharacter != null && ccpCharacter.IsTraining)
                        return ccpCharacter;
                }
                return null;
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Query skills in training for characters on this account.
        /// </summary>
        internal void CharacterInTraining()
        {
            if (m_updatePending)
                return;

            m_updatePending = true;
            m_skillInTrainingCache.Clear();

            EveClient.Trace("Account.CharacterInTraining - {0}", this);

            foreach (CharacterIdentity id in CharacterIdentities)
            {
                string identity = id.Name;

                if (!m_skillInTrainingCache.ContainsKey(identity))
                    m_skillInTrainingCache.Add(identity, new SkillQueueResponse());

                EveClient.APIProviders.CurrentProvider.QueryMethodAsync<SerializableAPISkillInTraining>(
                    APIMethods.CharacterSkillInTraining,
                    m_userId,
                    m_apiKey,
                    id.CharacterID,
                    x => OnSkillInTrainingUpdated(x, identity));

                EveClient.Trace("Account.CharacterInTraining - Querying {0}", identity);
            }
        }

        /// <summary>
        /// Updates the characters list on a timer tick
        /// </summary>
        internal void UpdateOnOneSecondTick()
        {
            m_charactersListMonitor.UpdateOnOneSecondTick();

            // While the key status is unknown, every five minutes, we try to update it.
            if (m_keyLevel == CredentialsLevel.Unknown && DateTime.UtcNow >= m_lastKeyLevelUpdate.AddMinutes(5))
            {
                // Quits if no network
                if (!NetworkMonitor.IsNetworkAvailable)
                    return;

                // Use the first character ID
                CharacterIdentity characterID = CharacterIdentities.FirstOrDefault();
                if (characterID == null)
                    return;

                // Query the wallet balance
                EveClient.APIProviders.CurrentProvider.QueryMethodAsync<SerializableAPIAccountBalance>(
                    APIMethods.CharacterAccountBalance, m_userId, m_apiKey, characterID.CharacterID,
                    OnKeyLevelUpdated);
            }
        }

        /// <summary>
        /// Gets the credential level from the given result.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        internal static CredentialsLevel GetCredentialsLevel(APIResult<SerializableAPIAccountBalance> result)
        {
            // No error ? Then it is a full key
            if (!result.HasError)
                return CredentialsLevel.Full;

            // Error code 200 means it was a limited key
            if (result.CCPError != null && result.CCPError.IsLimitedKeyError)
                return CredentialsLevel.Limited;

            // Another error occurred 
            return CredentialsLevel.Unknown;
        }

        /// <summary>
        /// Updates the characters list with the given CCP data
        /// </summary>
        /// <param name="result"></param>
        internal void Import(APIResult<SerializableAPICharacters> result)
        {
            if (result.HasError)
            {
                ImportIdentities(null);
            }
            else
            {
                ImportIdentities(result.Result.Characters);
            }

            // Fires the event regarding the account character list update.
            EveClient.OnCharacterListChanged(this);
        }

        /// <summary>
        /// Exports the data to a serialization object
        /// </summary>
        /// <returns></returns>
        internal SerializableAccount Export()
        {
            return new SerializableAccount
                       {
                           ID = m_userId,
                           Key = m_apiKey,
                           KeyLevel = m_keyLevel,
                           LastCharacterListUpdate = m_charactersListMonitor.LastUpdate,
                           IgnoreList = m_ignoreList.Export()
                       };
        }

        #endregion

        #region Response To Events

        /// <summary>
        /// Called when character's skill in training gets updated.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="character">The character.</param>
        private void OnSkillInTrainingUpdated(APIResult<SerializableAPISkillInTraining> result, string character)
        {
            // Return on error
            if (result.HasError)
            {
                EveClient.Trace("Account.OnSkillInTrainingUpdated - {0}\\{1} : Error Response", this, character);
                m_skillInTrainingCache[character].State = ResponseState.InError;
                return;
            }

            m_skillInTrainingCache[character].State = result.Result.SkillInTraining == 1
                                                     ? ResponseState.Training
                                                     : ResponseState.NotTraining;

            EveClient.Trace("Account.OnSkillInTrainingUpdated - {0}\\{1} : {2} ({3}\\{4})",
                            this,
                            character,
                            m_skillInTrainingCache[character].State,
                            m_skillInTrainingCache.Count(x => x.Value.State != ResponseState.Unknown),
                            CharacterIdentities.Count());


            // in the event this becomes a very long running process because of latency
            // and characters have been removed from the account since they were queried
            // remove those characters from the cache.
            IEnumerable<KeyValuePair<string, SkillQueueResponse>> toRemove =
                m_skillInTrainingCache.Where(x => !CharacterIdentities.Any(y => y.Name == x.Key));

            foreach (var charToRemove in toRemove)
            {
                m_skillInTrainingCache.Remove(charToRemove.Key);
            }

            // if we did not get response from any characters yet we are not sure
            // so wait until next time.
            if (m_skillInTrainingCache.Any(x => x.Value.State == ResponseState.Unknown))
                return;

            OnAllCharactersSkillTrainingUpdated();

            // Reset update pending flag
            m_updatePending = false;
        }

        /// <summary>
        /// Used when the character list has been queried.
        /// </summary>
        /// <param name="result"></param>
        private void OnCharactersListUpdated(APIResult<SerializableAPICharacters> result)
        {
            // Notify on error
            if (result.HasError)
            {
                EveClient.Notifications.NotifyCharacterListError(this, result);
                return;
            }

            // Invalidates the notification and update
            EveClient.Notifications.InvalidateAccountError(this);
            Import(result);
        }

        /// <summary>
        /// Update when we can update the key level.
        /// </summary>
        /// <param name="result"></param>
        private void OnKeyLevelUpdated(APIResult<SerializableAPIAccountBalance> result)
        {
            m_lastKeyLevelUpdate = DateTime.UtcNow;
            m_keyLevel = GetCredentialsLevel(result);

            // Notify error if any
            if (m_keyLevel == CredentialsLevel.Unknown)
            {
                EveClient.Notifications.NotifyKeyLevelError(this, result);
                return;
            }

            // Notify characters changed
            foreach (CharacterIdentity id in CharacterIdentities)
            {
                CCPCharacter ccpCharacter = id.CCPCharacter;
                if (ccpCharacter != null)
                    EveClient.OnCharacterChanged(ccpCharacter);
            }
        }

        /// <summary>
        /// Called when all characters skill training has been updated.
        /// </summary>
        private void OnAllCharactersSkillTrainingUpdated()
        {
            foreach (string key in m_skillInTrainingCache.Keys)
                EveClient.Trace("Account.OnAllCharactersSkillTrainingUpdated - {0}\\{1} = {2} ({3})",
                                this,
                                key,
                                m_skillInTrainingCache[key].State,
                                m_skillInTrainingCache[key].Timestamp);


            // one of the remaining characters was training; account is training.
            if (m_skillInTrainingCache.Any(x => x.Value.State == ResponseState.Training))
            {
                EveClient.Trace("Account.OnAllCharactersSkillTrainingUpdated - {0} : Training character found.", this);
                EveClient.Notifications.InvalidateAccountNotInTraining(this);
                return;
            }

            if (m_skillInTrainingCache.Any(x => x.Value.State == ResponseState.InError))
            {
                EveClient.Trace(
                    "Account.OnAllCharactersSkillTrainingUpdated - {0} : One or more characters returned an error.",
                    this);
                return;
            }

            // no training characters found up until
            EveClient.Notifications.NotifyAccountNotInTraining(this);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Updates the characters list with the given CCP data
        /// </summary>
        /// <param name="identities"></param>
        private void ImportIdentities(IEnumerable<ISerializableCharacterIdentity> identities)
        {
            // Clear the accounts on this character
            foreach (CharacterIdentity id in EveClient.CharacterIdentities)
            {
                if (id.Account == this)
                    id.Account = null;
            }

            // Return if there were errors in the query
            if (identities == null)
                return;

            // Assign owned identities to this account
            foreach (ISerializableCharacterIdentity serialID in identities)
            {
                CharacterIdentity id = EveClient.CharacterIdentities[serialID.ID];
                if (id == null)
                    id = EveClient.CharacterIdentities.Add(serialID.ID, serialID.Name);

                id.Account = this;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Asynchronously updates this account through a <see cref="AccountCreationEventArgs"/>.
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="callback">A callback invoked on the UI thread (whatever the result, success or failure)</param>
        /// <returns></returns>
        public void TryUpdateAsync(string apiKey, EventHandler<AccountCreationEventArgs> callback)
        {
            EveClient.Accounts.TryAddOrUpdateAsync(m_userId, apiKey, callback);
        }

        /// <summary>
        /// Updates the account with the informations extracted from the API by <see cref="AccountCreationEventArgs"/>.
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="keyLevel"></param>
        /// <param name="identities"></param>
        /// <param name="queryResult"></param>
        internal void UpdateAPIKey(string apiKey, CredentialsLevel keyLevel, IEnumerable<CharacterIdentity> identities,
                                   APIResult<SerializableAPICharacters> queryResult)
        {
            m_apiKey = apiKey;
            m_keyLevel = keyLevel;
            m_charactersListMonitor.UpdateWith(queryResult);

            // Clear the account for the currently associated identities
            foreach (CharacterIdentity id in EveClient.CharacterIdentities)
            {
                if (id.Account == this)
                    id.Account = null;
            }

            // Assign this account to the new identities and create CCP characters
            foreach (CharacterIdentity id in identities)
            {
                // Skip if in the ignore list
                id.Account = this;
                if (m_ignoreList.Contains(id))
                    continue;

                // Retrieves the ccp character and create one if none.
                CCPCharacter ccpCharacter = id.CCPCharacter;
                if (ccpCharacter == null)
                {
                    ccpCharacter = new CCPCharacter(id);
                    EveClient.Characters.Add(ccpCharacter, true);
                    ccpCharacter.Monitored = true;
                }
            }
        }

        #endregion

        #region Overridden Methods

        /// <summary>
        /// Gets a string representation of this account, under the given format : 123456 (John Doe, Joe Dohn).
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // If no characters on this account, return a "no characters" mention
            if (CharacterIdentities.Count() == 0)
                return String.Format("{0} (no characters)", m_userId);

            // Otherwise, return the chars' names into parenthesis.
            string names = String.Empty;
            foreach (CharacterIdentity id in CharacterIdentities)
            {
                names += id.Name;
                names += ", ";
            }
            return String.Format("{0} ({1})", m_userId, names.TrimEnd(", ".ToCharArray()));
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

        #region Nested type: SkillQueueResponse

        private class SkillQueueResponse
        {
            private ResponseState m_state;

            public SkillQueueResponse()
            {
                State = ResponseState.Unknown;
                Timestamp = DateTime.MinValue;
            }

            public ResponseState State
            {
                get { return m_state; }
                set
                {
                    Timestamp = DateTime.Now;
                    m_state = value;
                }
            }

            public DateTime Timestamp { get; set; }
        }

        #endregion

        #endregion
    }
}