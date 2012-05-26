using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common
{
    public class GlobalAPIKeyCollection : ReadonlyKeyedCollection<long, APIKey>
    {

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


        #region Addition / Removal Methods

        /// <summary>
        /// Removes the provided API key from this collection.
        /// </summary>
        /// <param name="apiKey">The API key to remove</param>
        /// <exception cref="InvalidOperationException">The API key does not exist in the list.</exception>
        public void Remove(APIKey apiKey)
        {
            if (apiKey == null)
                throw new ArgumentNullException("apiKey");

            // Removes the API key on the owned identities
            foreach (CharacterIdentity identity in apiKey.CharacterIdentities.Where(x => x.APIKeys.Contains(apiKey)))
            {
                identity.APIKeys.Remove(apiKey);

                if (identity.CCPCharacter != null)
                    EveMonClient.OnCharacterUpdated(identity.CCPCharacter);
            }

            // Remove the API key
            if (!Items.Remove(apiKey.ID))
                throw new InvalidOperationException("This API key does not exist in the list.");

            // Dispose
            apiKey.Dispose();

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
            // Unsubscribe events
            foreach (APIKey apiKey in Items.Values)
            {
                apiKey.Dispose();
            }

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
        internal IEnumerable<SerializableAPIKey> Export()
        {
            return Items.Values.Select(apikey => apikey.Export());
        }

        #endregion
    }
}