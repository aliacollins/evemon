using System;
using System.Linq;
using System.Collections.Generic;
using EVEMon.Common.Attributes;
using EVEMon.Common.Serialization.Settings;
using EVEMon.Common.Serialization;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.Net;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents a player account
    /// </summary>
    [EnforceUIThreadAffinity]
    public sealed class Account
    {
        private long m_userId;
        private string m_apiKey;
        private CredentialsLevel m_keyLevel;

        private bool m_hasCharacterInTraining;
        private bool m_firstCheck = true;
        private int m_counter;

        private readonly AccountIgnoreList m_ignoreList;
        private readonly AccountQueryMonitor<SerializableCharacterList> m_charactersListMonitor;

        private DateTime m_lastKeyLevelUpdate = DateTime.MinValue;

        /// <summary>
        /// Common constructor base.
        /// </summary>
        private Account()
        {
            m_charactersListMonitor = new AccountQueryMonitor<SerializableCharacterList>(this, APIMethods.CharacterList);
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
        /// <param name="apiKey"></param>
        /// <param name="isFullKey"></param>
        internal Account(long userID)
            : this()
        {
            m_userId = userID;
            m_apiKey = String.Empty;
        }

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
            get { return m_hasCharacterInTraining; }
        }

        /// <summary>
        /// Gets the character identities for this account
        /// </summary>
        public IEnumerable<CharacterIdentity> CharacterIdentities
        {
            get 
            {
                foreach (var characterID in EveClient.CharacterIdentities)
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
                foreach (var id in CharacterIdentities)
                {
                    var ccpCharacter = id.CCPCharacter;
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
                foreach (var id in CharacterIdentities)
                {
                    var ccpCharacter = id.CCPCharacter;
                    if (ccpCharacter != null && ccpCharacter.IsTraining)
                        return ccpCharacter;
                }
                return null;
            }
        }

        /// <summary>
        /// Query each account character's skill in training.
        /// </summary>
        internal void CharacterInTraining()
        {
            m_counter = 0;
            m_hasCharacterInTraining = false;

            foreach (var id in CharacterIdentities)
            {
                EveClient.APIProviders.CurrentProvider.QueryMethodAsync<SerializableSkillInTraining>(
                    APIMethods.CharacterSkillInTraining, m_userId, m_apiKey, id.CharacterID,
                    OnSkillInTrainingUpdated);

                m_counter++;
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
                var characterID = CharacterIdentities.FirstOrDefault();
                if (characterID == null)
                    return;

                // Query the wallet balance
                EveClient.APIProviders.CurrentProvider.QueryMethodAsync<SerializableAccountBalanceList>(
                    APIMethods.CharacterAccountBalance, m_userId, m_apiKey, characterID.CharacterID,
                    OnKeyLevelUpdated);
            }
        }

        /// <summary>
        /// Used when the character list has been queried.
        /// </summary>
        /// <param name="result"></param>
        private void OnCharactersListUpdated(APIResult<SerializableCharacterList> result)
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

            // On startup check for character in training on this account
            if (m_firstCheck)
            {
                CharacterInTraining();
                m_firstCheck = false;
            }
        }

        /// <summary>
        /// Update when we can update the key level.
        /// </summary>
        /// <param name="result"></param>
        private void OnKeyLevelUpdated(APIResult<SerializableAccountBalanceList> result)
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
            foreach (var id in CharacterIdentities)
            {
                var ccpCharacter = id.CCPCharacter;
                if (ccpCharacter != null)
                    EveClient.OnCharacterChanged(ccpCharacter);
            }
        }

        /// <summary>
        /// Updates the account is in training or send a notification.
        /// </summary>
        /// <param name="result"></param>
        private void OnSkillInTrainingUpdated(APIResult<SerializableSkillInTraining> result)
        {
            // Return on error
            if (result.HasError)
                return;

            m_hasCharacterInTraining |= result.Result.SkillInTraining != 0;
            m_counter--;

            // Notify on account none character in training
            if (!m_hasCharacterInTraining && m_counter == 0)
            {
                EveClient.Notifications.NotifyAccountNotInTraining(this);
            }
            else
            {
                EveClient.Notifications.InvalidateAccountNotInTraining(this);
            }
        }

        /// <summary>
        /// Gets the credential level from the given result.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        internal static CredentialsLevel GetCredentialsLevel(APIResult<SerializableAccountBalanceList> result)
        {
            // No error ? Then it is a full key
            if (!result.HasError)
                return CredentialsLevel.Full;

            // Error code 200 means it was a limited key
            if (result.CCPError != null && result.CCPError.IsLimitedKeyError) 
                return CredentialsLevel.Limited;

            // Another error occured 
            return CredentialsLevel.Unknown;
        }

        /// <summary>
        /// Updates the characters list with the given CCP data
        /// </summary>
        /// <param name="result"></param>
        internal void Import(APIResult<SerializableCharacterList> result)
        {
            if (result.HasError)
            {
                ImportIdentities(null);
            }
            else
            {
                ImportIdentities(result.Result.Characters.Cast<ISerializableCharacterIdentity>());
            }

            // Fires the event regarding the account character list update.
            EveClient.OnCharacterListChanged(this);
        }

        /// <summary>
        /// Updates the characters list with the given CCP data
        /// </summary>
        /// <param name="result"></param>
        private void ImportIdentities(IEnumerable<ISerializableCharacterIdentity> identities)
        {
            // Clear the accounts on this character
            foreach (var id in EveClient.CharacterIdentities)
            {
                if (id.Account == this)
                    id.Account = null;
            }

            // Return if there were errors in the query
            if (identities == null)
                return;

            // Assign owned identities to this account
            foreach (var serialID in identities)
            {
                var id = EveClient.CharacterIdentities[serialID.ID];
                if (id == null)
                    id = EveClient.CharacterIdentities.Add(serialID.ID, serialID.Name);

                id.Account = this;
            }
        }

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
        internal void UpdateAPIKey(string apiKey, CredentialsLevel keyLevel, IEnumerable<CharacterIdentity> identities, APIResult<SerializableCharacterList> queryResult)
        {
            m_apiKey = apiKey;
            m_keyLevel = keyLevel;
            m_charactersListMonitor.UpdateWith(queryResult);

            // Clear the account for the currently associated identities
            foreach (var id in EveClient.CharacterIdentities)
            {
                if (id.Account == this)
                    id.Account = null;
            }

            // Assign this account to the new identities and create CCP characters
            foreach (var id in identities)
            {
                // Skip if in the ignore list
                id.Account = this;
                if (m_ignoreList.Contains(id))
                    continue;

                // Retrieves the ccp character and create one if none.
                var ccpCharacter = id.CCPCharacter;
                if (ccpCharacter == null)
                {
                    ccpCharacter = new CCPCharacter(id);
                    EveClient.Characters.Add(ccpCharacter, true);
                    ccpCharacter.Monitored = true;
                }
            }
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
            foreach (var id in CharacterIdentities)
            {
                names += id.Name;
                names += ", ";
            }
            return String.Format("{0} ({1})", m_userId, names.TrimEnd(", ".ToCharArray()));
        }
    }
}
