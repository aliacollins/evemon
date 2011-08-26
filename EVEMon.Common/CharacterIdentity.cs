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
        private Account m_account;

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
        /// Gets the account this identity is associated with.
        /// </summary>
        public Account Account
        {
            get { return m_account; }
            internal set
            {
                if (m_account == value)
                    return;

                m_account = value;

                // Notify subscribers
                CCPCharacter ccpCharacter = CCPCharacter;
                if (ccpCharacter == null)
                    return;

                EveMonClient.OnCharacterUpdated(ccpCharacter);
                EveMonClient.OnCharacterInfoUpdated(ccpCharacter);
            }
        }

        /// <summary>
        /// Gets the CCP character representing this identity, or null when there is none.
        /// </summary>
        public CCPCharacter CCPCharacter
        {
            get
            {
                return EveMonClient.Characters.Where(
                    character => character is CCPCharacter && character.CharacterID == m_id).Cast<CCPCharacter>().FirstOrDefault();
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