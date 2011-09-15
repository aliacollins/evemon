using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EVEMon.Common.Collections;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.Serialization.Settings;
using EVEMon.Common.Threading;

namespace EVEMon.Common
{
    public class GlobalAPIKeyCollection : ReadonlyKeyedCollection<long, APIKey>
    {
        /// <summary>
        /// Check whether some accounts are not in training.
        /// </summary>
        /// <param name="message">Message describing the accounts not in training.</param>
        /// <returns>True if one or more accounts is not in training, otherwise false.</returns>
        /// <remarks>This condition applied only to those API keys of type 'Account'</remarks>
        public static bool HasAccountsNotTraining(out string message)
        {
            message = String.Empty;

            IEnumerable<APIKey> accountsNotTraining = EveMonClient.APIKeys.Where(x => x.Type == APIKeyType.Account &&
                                                                                      !x.CharacterIdentities.IsEmpty() &&
                                                                                      !x.HasMonitoredCharacters &&
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

            // return
            message = builder.ToString();
            return true;
        }


        #region Indexer

        /// <summary>
        /// Gets the API key with the provided id, null when not found
        /// </summary>
        /// <param name="id">The id to look for</param>
        /// <returns>The searched API key when found; null otherwise.</returns>
        public APIKey this[long id]
        {
            get { return Items.Values.FirstOrDefault(apiKey => apiKey.ID == id); }
        }

        #endregion


        #region Public Static Methods

        public static void TryAddOrUpdateAsync(long id, string verificationCode,
                                               EventHandler<APIKeyCreationEventArgs> callback)
        {
            // Invokes on the thread pool
            Dispatcher.BackgroundInvoke(() =>
                                            {
                                                APIResult<SerializableAPIKeyInfo> apiKeyInfoResult =
                                                    EveMonClient.APIProviders.CurrentProvider.QueryAPIKeyInfo(id, verificationCode);

                                                // Invokes the callback on the UI thread
                                                Dispatcher.Invoke(
                                                    () =>
                                                    callback(null,
                                                             new APIKeyCreationEventArgs(id, verificationCode, apiKeyInfoResult)));
                                            });
        }

        #endregion


        #region Addition / Removal Methods

        /// <summary>
        /// Removes the provided API key from this collection.
        /// </summary>
        /// <param name="apiKey">The API key to remove</param>
        /// <exception cref="InvalidOperationException">The API key does not exist in the list.</exception>
        public void Remove(APIKey apiKey)
        {
            // Clears the API key on the owned identities
            foreach (CharacterIdentity identity in apiKey.CharacterIdentities.Where(x => x.APIKey == apiKey))
            {
                identity.APIKey = null;
            }

            // Remove the API key
            if (!Items.Remove(apiKey.ID))
                throw new InvalidOperationException("This API key does not exist in the list.");

            EveMonClient.OnAPIKeyCollectionChanged();
        }

        /// <summary>
        /// Adds an API key to this collection.
        /// </summary>
        /// <param name="apiKey"></param>
        internal void Add(APIKey apiKey)
        {
            Items.Add(apiKey.ID, apiKey);
            EveMonClient.OnAPIKeyCollectionChanged();
        }

        #endregion


        #region Import / Export Methods

        /// <summary>
        /// Imports the serialized API key.
        /// </summary>
        /// <param name="serial"></param>
        internal void Import(IEnumerable<SerializableAPIKey> serial)
        {
            Items.Clear();
            foreach (SerializableAPIKey apikey in serial)
            {
                try
                {
                    Items.Add(apikey.ID, new APIKey(apikey));
                }
                catch (ArgumentException ex)
                {
                    EveMonClient.Trace("GlobalAPIKeyCollection.Import - " +
                                       "An API key with id {0} already existed; additional instance ignored.", apikey.ID);
                    ExceptionHandler.LogException(ex, true);
                }
            }

            EveMonClient.OnAPIKeyCollectionChanged();
        }

        /// <summary>
        /// Exports the data to a serialization object.
        /// </summary>
        /// <returns></returns>
        internal List<SerializableAPIKey> Export()
        {
            return Items.Values.Select(apikey => apikey.Export()).ToList();
        }

        #endregion
    }
}