using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EVEMon.Common.Threading;
using EVEMon.Common.Attributes;
using EVEMon.Common.Serialization.Settings;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents the characters list
    /// </summary>
    [EnforceUIThreadAffinity]
    public sealed class GlobalAccountCollection : ReadonlyKeyedCollection<Int64, Account>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        internal GlobalAccountCollection()
        {
        }

        /// <summary>
        /// Check whether some accounts are not in training.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool HasAccountsNotTraining(out string message)
        {
            message = "";

            // Collect the accounts not in training
            var accountsNotTraining = new List<Account>();
            foreach (var account in EveClient.Accounts)
            {
                // Checks whether the account is training
                if (!account.CharacterIdentities.IsEmpty() && account.TrainingCharacter == null)
                {
                    accountsNotTraining.Add(account);
                }
            }

            // All accounts are training ?
            if (accountsNotTraining.Count == 0) return false;

            // Creates the string, scrolling through every not training account
            StringBuilder builder = new StringBuilder();
            if (accountsNotTraining.Count == 1) builder.Append("One of your account is not in training.");
            else builder.Append("Some of your accounts are not in training.");

            foreach (var account in accountsNotTraining)
            {
                builder.AppendLine().Append("* ").Append(account.ToString());
            }

            // return
            message = builder.ToString();
            return true;
        }

        /// <summary>
        /// Gets the account with the provided user id, null when not found
        /// </summary>
        /// <param name="userID">The user id to look for</param>
        /// <returns>The searched account when found; null otherwise.</returns>
        public Account this[long userID]
        {
            get
            {
                foreach (var account in m_items.Values)
                {
                    if (account.UserID == userID) return account;
                }
                return null;
            }
        }

        /// <summary>
        /// Asynchronously creates an account through a <see cref="AccountCreationEventArgs"/>.
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="apiKey"></param>
        /// <param name="callback">A callback invoked on the UI thread (whatever the result, success or failure)</param>
        /// <returns></returns>
        public void TryAddOrUpdateAsync(long userID, string apiKey, EventHandler<AccountCreationEventArgs> callback)
        {
            // Invokes on the thread pool
            Dispatcher.BackgroundInvoke(() =>
                {
                    var charListResult = EveClient.APIProviders.DefaultProvider.QueryCharactersList(userID, apiKey);

                    // Call char/AccountBalance.xml to check whether it is a full api key
                    APIResult<SerializableAccountBalanceList> balanceResult = null;
                    if (!charListResult.HasError && charListResult.Result.Characters.Count != 0)
                    {
                        var characterID = charListResult.Result.Characters[0].ID;
                        balanceResult = EveClient.APIProviders.DefaultProvider.QueryCharacterAccountBalance(userID, apiKey, characterID);
                    }

                    // Invokes the callback on the UI thread
                    Dispatcher.Invoke(() => callback(null, new AccountCreationEventArgs(userID, apiKey, charListResult, balanceResult)));
                });
        }

        /// <summary>
        /// Removes the provided account from this collection
        /// </summary>
        /// <param name="account">The account to remove</param>
        /// <param name="removeCharacters">When true, the associated characters will be removed.</param>
        /// <exception cref="InvalidOperationException">The account does not exist in the list.</exception>
        public void Remove(Account account, bool removeCharacters)
        {
            // Clears the account on the owned identities.
            foreach (var identity in account.CharacterIdentities.Where(x => x.Account == account))
            {
                identity.Account = null;
            }

            // Remove the account
            if (!m_items.Remove(account.UserID))
            {
                throw new InvalidOperationException("This account does not exist in the list.");
            }

            EveClient.OnAccountCollectionChanged();
        }

        /// <summary>
        /// Adds an account to this collection.
        /// </summary>
        /// <param name="account"></param>
        /// <param name="notify"></param>
        internal void Add(Account account, bool notify)
        {
            m_items.Add(account.UserID, account);
            if (notify) EveClient.OnAccountCollectionChanged();
        }

        /// <summary>
        /// Imports the serialized accounts
        /// </summary>
        /// <param name="serial"></param>
        internal void Import(IEnumerable<SerializableAccount> serial)
        {
            m_items.Clear();
            foreach (var serialAccount in serial)
            {
                m_items.Add(serialAccount.ID, new Account(serialAccount));
            }

            EveClient.OnAccountCollectionChanged();
        }

        /// <summary>
        /// Exports the data to a serialization object
        /// </summary>
        /// <returns></returns>
        internal List<SerializableAccount> Export()
        {
            var serial = new List<SerializableAccount>();

            foreach (var account in m_items.Values)
            {
                serial.Add(account.Export());
            }

            return serial;
        }
    }
}
