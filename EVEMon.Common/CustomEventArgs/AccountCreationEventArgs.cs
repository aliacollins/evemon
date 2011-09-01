using System;
using System.Collections.Generic;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common.CustomEventArgs
{
    /// <summary>
    /// Event arguments for the creation of a new account.
    /// </summary>
    public sealed class AccountCreationEventArgs : EventArgs
    {
        private readonly List<CharacterIdentity> m_identities = new List<CharacterIdentity>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="apiKey">The API key.</param>
        /// <param name="serialAccountStatus">The serial account status.</param>
        /// <param name="serialCharacterList">The serial character list.</param>
        internal AccountCreationEventArgs(long userID, string apiKey,
                                          APIResult<SerializableAPIAccountStatus> serialAccountStatus,
                                          APIResult<SerializableAPICharacters> serialCharacterList)
        {
            UserID = userID;
            ApiKey = apiKey;
            AccountStatus = serialAccountStatus;
            CharacterList = serialCharacterList;
            KeyLevel = CredentialsLevel.Unknown;
            FullKeyTestError = String.Empty;

            //Determine the API key level
            KeyLevel = Account.GetCredentialsLevel(serialAccountStatus);

            // On error, retrieve the error message
            if (KeyLevel == CredentialsLevel.Unknown)
                FullKeyTestError = serialAccountStatus.ErrorMessage;

            // Retrieves the characters list
            if (CharacterList.HasError)
                return;

            foreach (SerializableCharacterListItem serialID in CharacterList.Result.Characters)
            {
                // Look for an existing char ID and update its name
                CharacterIdentity id = EveMonClient.CharacterIdentities[serialID.ID];
                if (id != null)
                    id.Name = serialID.Name;
                else
                {
                    // Create an identity if necessary
                    id = EveMonClient.CharacterIdentities.Add(serialID.ID, serialID.Name);
                }

                m_identities.Add(id);
            }
        }

        /// <summary>
        /// Gets the account's user ID.
        /// </summary>
        public long UserID { get; private set; }

        /// <summary>
        /// Gets the API key.
        /// </summary>
        public string ApiKey { get; private set; }

        /// <summary>
        /// Gets the API key level.
        /// </summary>
        public CredentialsLevel KeyLevel { get; private set; }

        /// <summary>
        /// Gets the possible error message gotten when testing the key security (excluding the error meaning the key was a limited one).
        /// </summary>
        public string FullKeyTestError { get; private set; }

        /// <summary>
        /// Gets the result which occurred when the characters list was queried.
        /// </summary>
        public IAPIResult Result { get; private set; }

        /// <summary>
        /// Gets the result which occurred when the characters list was queried.
        /// </summary>
        public APIResult<SerializableAPICharacters> CharacterList { get; private set; }

        /// <summary>
        /// Gets the result which occurred when the account status was queried.
        /// </summary>
        public APIResult<SerializableAPIAccountStatus> AccountStatus { get; private set; }

        /// <summary>
        /// Gets the enumeration of identities available on this account.
        /// </summary>
        public IEnumerable<CharacterIdentity> Identities
        {
            get { return m_identities; }
        }

        /// <summary>
        /// Creates or updates the account.
        /// </summary>
        /// <returns></returns>
        public Account CreateOrUpdate()
        {
            // Checks whether this account already exists to update it.
            Account account = EveMonClient.Accounts[UserID];
            if (account != null)
            {
                account.UpdateAPIKey(this);

                // Collection did not change but there is no "AccountChanged" event
                EveMonClient.OnAccountCollectionChanged();
            }
            else
            {
                account = new Account(UserID);
                account.UpdateAPIKey(this);
                EveMonClient.Accounts.Add(account, true);
            }

            return account;
        }
    }
}