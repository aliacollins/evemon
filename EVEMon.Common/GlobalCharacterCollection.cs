using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Serialization.Settings;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.Serialization.Importation;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents the global collection of characters
    /// </summary>
    public sealed class GlobalCharacterCollection : ReadonlyCollection<Character>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        internal GlobalCharacterCollection()
        {
        }

        /// <summary>
        /// Gets a character by its guid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public Character this[Guid guid]
        {
            get { return Items.FirstOrDefault(character => character.Guid == guid); }
        }

        /// <summary>
        /// Adds a character to this collection.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="notify"></param>
        internal void Add(Character character, bool notify)
        {
            Items.Add(character);
            if (notify)
                EveMonClient.OnCharacterCollectionChanged();
        }

        /// <summary>
        /// Removes a character from all the collections.
        /// Also removes it from the monitored characters collection, and assign it to the ignore list of its account.
        /// </summary>
        /// <param name="character"></param>
        public void Remove(Character character)
        {
            Remove(character, true);
        }

        /// <summary>
        /// Removes a character from this collection.
        /// Also removes it from the monitored characters collection, and assign it to the ignore list of its account.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="notify"></param>
        internal void Remove(Character character, bool notify)
        {
            Items.Remove(character);
            character.Monitored = false;

            // For CCP characters, also put it on the account's ignore list.
            if (character is CCPCharacter)
            {
                Account account = character.Identity.Account;
                if (account != null)
                    account.IgnoreList.Add(character as CCPCharacter);
            }

            if (notify)
                EveMonClient.OnCharacterCollectionChanged();
        }

        /// <summary>
        /// Asynchronously adds a character from the given uri, adding a new identity when needed.
        /// </summary>
        /// <param name="uri">The uri to load the character sheet from</param>
        /// <param name="callback">A callback invoked on the UI thread (whatever the result, success or failure)</param>
        public void TryAddOrUpdateFromUriAsync(Uri uri, EventHandler<UriCharacterEventArgs> callback)
        {
            // We have a file, let's just deserialize it synchronously
            if (uri.IsFile)
            {
                string format = Util.GetXmlRootElement(uri.LocalPath);

                switch (format.ToLower(CultureConstants.DefaultCulture))
                {
                    case "eveapi":
                        APIResult<SerializableAPICharacterSheet> apiResult =
                            Util.DeserializeAPIResult<SerializableAPICharacterSheet>(uri.ToString(), APIProvider.RowsetsTransform);
                        callback(null, new UriCharacterEventArgs(uri, apiResult));
                        break;
                    case "serializableccpcharacter":
                        try
                        {
                            SerializableCCPCharacter ccpResult = Util.DeserializeXML<SerializableCCPCharacter>(uri.ToString());
                            callback(null, new UriCharacterEventArgs(uri, ccpResult));
                        }
                        catch (NullReferenceException ex)
                        {
                            callback(null,
                                     new UriCharacterEventArgs(uri,
                                                               String.Format(CultureConstants.DefaultCulture,
                                                                             "Unable to load file (SerializableCCPCharacter). ({0})",
                                                                             ex.Message)));
                        }
                        break;
                    case "character":
                        try
                        {
                            OldExportedCharacter oldCharacterResult = Util.DeserializeXML<OldExportedCharacter>(uri.ToString());
                            SerializableCCPCharacter ccpCharacterResult = oldCharacterResult.ToSerializableCCPCharacter();
                            callback(null, new UriCharacterEventArgs(uri, ccpCharacterResult));
                        }
                        catch (NullReferenceException ex)
                        {
                            callback(null,
                                     new UriCharacterEventArgs(uri,
                                                               String.Format(CultureConstants.DefaultCulture,
                                                                             "Unable to load file (Character). ({0})", ex.Message)));
                        }
                        break;
                    default:
                        callback(null, new UriCharacterEventArgs(uri, "Format Not Recognized"));
                        break;
                }
            }
                // So, it's a web address, let's do it in an async way.
            else
            {
                Util.DownloadAPIResultAsync<SerializableAPICharacterSheet>(uri.ToString(), null, APIProvider.RowsetsTransform,
                                                                           result =>
                                                                           callback(null, new UriCharacterEventArgs(uri, result)));
            }
        }

        /// <summary>
        /// Imports the character identities from a serialization object
        /// </summary>
        /// <param name="serial"></param>
        internal void Import(IEnumerable<SerializableSettingsCharacter> serial)
        {
            // Clear the accounts on every identity
            foreach (CharacterIdentity id in EveMonClient.CharacterIdentities)
            {
                id.Account = null;
            }

            // Import the characters, their identies, etc
            Items.Clear();
            foreach (SerializableSettingsCharacter serialCharacter in serial)
            {
                // Gets the identity or create it
                CharacterIdentity id = EveMonClient.CharacterIdentities[serialCharacter.ID] ??
                                       EveMonClient.CharacterIdentities.Add(serialCharacter.ID, serialCharacter.Name);

                // Imports the character
                SerializableCCPCharacter ccpCharacter = serialCharacter as SerializableCCPCharacter;
                if (ccpCharacter != null)
                    Items.Add(new CCPCharacter(id, ccpCharacter));
                else
                {
                    SerializableUriCharacter uriCharacter = serialCharacter as SerializableUriCharacter;
                    Items.Add(new UriCharacter(id, uriCharacter));
                }
            }

            // Notify the change
            EveMonClient.OnCharacterCollectionChanged();
        }

        /// <summary>
        /// Exports this collection to a serialization object
        /// </summary>
        /// <returns></returns>
        internal List<SerializableSettingsCharacter> Export()
        {
            return Items.Select(character => character.Export()).ToList();
        }

        /// <summary>
        /// imports the plans from serialization objects
        /// </summary>
        /// <param name="serial"></param>
        internal void ImportPlans(List<SerializablePlan> serial)
        {
            foreach (Character character in Items)
            {
                character.ImportPlans(serial);
            }
        }

        /// <summary>
        /// Exports the plans as serialization objects
        /// </summary>
        /// <returns></returns>
        internal List<SerializablePlan> ExportPlans()
        {
            List<SerializablePlan> serial = new List<SerializablePlan>();
            foreach (Character character in Items)
            {
                character.ExportPlans(serial);
            }

            return serial;
        }
    }
}