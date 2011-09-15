using System.Collections.Generic;
using System.Linq;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents a character's identity, defined by its sole ID. 
    /// Character identities to ensure 
    /// </summary>
    public sealed class CharacterIdentity
    {
        private readonly long m_id;
        private APIKey m_apiKey;

        /// <summary>
        /// Constructor from an id and a name.
        /// </summary>
        /// <param name="id">The id for this identity</param>
        /// <param name="name"></param>
        internal CharacterIdentity(long id, string name)
        {
            m_id = id;
            Name = name;
        }

        /// <summary>
        /// Gets the character ID.
        /// </summary>
        public long CharacterID
        {
            get { return m_id; }
        }

        /// <summary>
        /// Gets the character's name.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the API key this identity is associated with.
        /// </summary>
        public APIKey APIKey
        {
            get { return m_apiKey; }
            internal set
            {
                if (m_apiKey == value)
                    return;

                m_apiKey = value;

                // Notify subscribers
                CCPCharacter ccpCharacter = CCPCharacter;
                if (ccpCharacter == null)
                    return;

                EveMonClient.OnCharacterUpdated(ccpCharacter);
            }
        }

        /// <summary>
        /// Gets the CCP character representing this identity, or null when there is none.
        /// </summary>
        public CCPCharacter CCPCharacter
        {
            get
            {
                return EveMonClient.Characters.OfType<CCPCharacter>().Where(
                    character => character.CharacterID == m_id).FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets the enumeration of the uri characters representing this identity.
        /// </summary>
        public IEnumerable<UriCharacter> UriCharacters
        {
            get { return EveMonClient.Characters.OfType<UriCharacter>(); }
        }
    }
}