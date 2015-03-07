using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common
{
    public sealed class CharacterIdentityIgnoreList : ReadonlyCollection<CharacterIdentity>
    {
        private readonly APIKey m_owner;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="apiKey"></param>
        internal CharacterIdentityIgnoreList(APIKey apiKey)
        {
            m_owner = apiKey;
        }

        /// <summary>
        /// Checks whether the given character's associated identity is contained in this list.
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public bool Contains(Character character)
        {
            if (character == null)
                throw new ArgumentNullException("character");

            return Contains(character.Identity);
        }

        /// <summary>
        /// Removes this character and attempts to return a CCP character. 
        /// The resulting character will be the existing one matching this id, or if it does not exist, a new character. 
        /// If the identity was not in the collection, the method won't attempt to create a new character and will return either the existing one or null.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public void Remove(CharacterIdentity id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            // If the id was not in list, returns the existing character or null if it does not exist
            if (!Items.Remove(id))
                return;

            // If character exists, returns it
            if (id.CCPCharacter != null)
                return;

            // Create a new CCP character
            EveMonClient.Characters.Add(new CCPCharacter(id));
        }

        /// <summary>
        /// Adds a character to the ignore list and, if it belonged to this API key, removes it from the global collection 
        /// (all associated data and plans won't be written on next serialization !).
        /// </summary>
        /// <param name="character"></param>
        public void Add(Character character)
        {
            if (character == null)
                throw new ArgumentNullException("character");

            CharacterIdentity id = character.Identity;
            if (Items.Contains(id))
                return;

            Items.Add(id);

            // If the identity was belonging to this API key, remove the character (won't be serialized anymore !)
            if (id.APIKeys.Contains(m_owner))
                EveMonClient.Characters.Remove(character);
        }

        /// <summary>
        /// Imports the deserialization objects.
        /// </summary>
        /// <param name="serialIDList"></param>
        internal void Import(IEnumerable<SerializableCharacterIdentity> serialIDList)
        {
            Items.Clear();
            foreach (CharacterIdentity id in serialIDList.Select(
                serialID => EveMonClient.CharacterIdentities[serialID.ID] ??
                            EveMonClient.CharacterIdentities.Add(serialID.ID, serialID.Name)))
            {
                Items.Add(id);
            }
        }

        /// <summary>
        /// Create serialization objects.
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<SerializableCharacterIdentity> Export()
        {
            return Items.Select(id => new SerializableCharacterIdentity
                                          {
                                              ID = id.CharacterID,
                                              Name = id.CharacterName,
                                          });
        }
    }
}
