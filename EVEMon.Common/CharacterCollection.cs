using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Attributes;
using EVEMon.Common.Serialization.Settings;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents the list of characters for this identiity
    /// </summary>
    [EnforceUIThreadAffinity]
    public sealed class CharacterCollection : IEnumerable<Character>
    {
        private readonly List<UriCharacter> m_uriCharacters = new List<UriCharacter>();
        private readonly CharacterIdentity m_characterID;

        /// <summary>
        /// Default constructor, this class is only instantiated by CharacterIdentity
        /// </summary>
        /// <param name="characterID">The character identitity this collection is bound to.</param>
        internal CharacterCollection(CharacterIdentity characterID)
        {
            m_characterID = characterID;
            CCPCharacter = new CCPCharacter(characterID);
        }

        /// <summary>
        /// Gets the CCP character
        /// </summary>
        public CCPCharacter CCPCharacter { get; private set; }

        /// <summary>
        /// Gets an enumeration over the URI character
        /// </summary>
        public IEnumerable<UriCharacter> UriCharacters
        {
            get { return m_uriCharacters; }
        }

        /// <summary>
        /// Gets a character by its guid.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public Character this[Guid guid]
        {
            get
            {
                if (CCPCharacter.Guid == guid)
                    return CCPCharacter;

                return m_uriCharacters.FirstOrDefault(source => source.Guid == guid);
            }
        }

        /// <summary>
        /// Gets an uri character with the given uri
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public UriCharacter this[Uri uri]
        {
            get { return m_uriCharacters.FirstOrDefault(source => source.Uri == uri); }
        }

        /// <summary>
        /// Clears the characters before settings refresh
        /// </summary>
        internal void Clear()
        {
            m_uriCharacters.Clear();
        }

        /// <summary>
        /// Adds a character from a deserialization object
        /// </summary>
        /// <param name="serial"></param>
        internal void Add(SerializableUriCharacter serial)
        {
            var uriCharacter = this[new Uri(serial.Uri)];
            if (uriCharacter == null)
            {
                m_uriCharacters.Add(new UriCharacter(m_characterID, serial));
                EveMonClient.OnCharacterCollectionChanged();
            }
            else
            {
                uriCharacter.Import(serial);
            }
        }

        /// <summary>
        /// Adds a character from a deserialization object
        /// </summary>
        /// <param name="ccpCharacter"></param>
        internal void Add(SerializableCCPCharacter ccpCharacter)
        {
            CCPCharacter = new CCPCharacter(m_characterID, ccpCharacter);
        }

        /// <summary>
        /// Addsa new UriCharacter with the specified Uri and deserialization object, then returns it
        /// </summary>
        /// <param name="uri">The source uri</param>
        /// <param name="result">The deserialization object acquired from the given uri</param>
        /// <returns>The created character, or null if there was errors on the provided CCP data.</returns>
        internal UriCharacter Add(Uri uri, APIResult<SerializableAPICharacterSheet> result)
        {
            if (result.HasError)
                return null;

            UriCharacter character = new UriCharacter(m_characterID, uri, result);
            m_uriCharacters.Add(character);

            EveMonClient.OnCharacterCollectionChanged();
            return character;
        }

        /// <summary>
        /// Removes the provided character for the uri characters
        /// </summary>
        /// <param name="character">The character to remove</param>
        /// <exception cref="InvalidOperationException">This character does not have that identity</exception>
        public void Remove(UriCharacter character)
        {
            if (!m_uriCharacters.Remove(character))
                throw new InvalidOperationException("This source does not belong to this character's sources");

            EveMonClient.OnCharacterCollectionChanged();
        }


        #region Enumerators

        IEnumerator<Character> IEnumerable<Character>.GetEnumerator()
        {
            List<Character> result = new List<Character> { CCPCharacter };
            result.AddRange(m_uriCharacters);

            return result.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            List<Character> result = new List<Character> { CCPCharacter };
            result.AddRange(m_uriCharacters);

            return ((IEnumerable)result).GetEnumerator();
        }

        #endregion
    }
}
