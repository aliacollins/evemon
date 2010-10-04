using System;
using System.Collections.Generic;
using System.Text;
using EVEMon.Common.Serialization;
using EVEMon.Common.Attributes;
using EVEMon.Common.Serialization.Settings;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents a character's identity, defined by its sole ID. 
    /// Character identities to ensure 
    /// </summary>
    public sealed class CharacterIdentity
    {
        private readonly int m_id;
        private Account m_account;
        private string m_name;

        /// <summary>
        /// Constructor from an id and a name.
        /// </summary>
        /// <param name="id">The id for this identity</param>
        internal CharacterIdentity(int id, string name)
        {
            m_id = id;
            m_name = name;
        }

        /// <summary>
        /// Gets the character ID.
        /// </summary>
        public int CharacterID
        {
            get { return m_id; }
        }

        /// <summary>
        /// Gets the character's name.
        /// </summary>
        public string Name
        {
            get { return m_name; }
            internal set { m_name = value; }
        }

        /// <summary>
        /// Gets the account this identity is associated with.
        /// </summary>
        public Account Account
        {
            get { return m_account; }
            internal set 
            {
                if (m_account == value) return;
                m_account = value;

                // Notify subscribers
                var ccpCharacter = CCPCharacter;
                if (ccpCharacter != null)
                {
                    EveClient.OnCharacterChanged(ccpCharacter);
                }
            }
        }

        /// <summary>
        /// Gets the CCP character representing this identity, or null when there is none.
        /// </summary>
        public CCPCharacter CCPCharacter
        {
            get
            {
                foreach (var character in EveClient.Characters)
                {
                    if (character is CCPCharacter && character.CharacterID == m_id)
                    {
                        return (CCPCharacter)character;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the enumeration of the uri characters representing this identity.
        /// </summary>
        public IEnumerable<UriCharacter> UriCharacters
        {
            get
            {
                foreach (var character in EveClient.Characters)
                {
                    var uriCharacter = character as UriCharacter;
                    if (uriCharacter != null) yield return uriCharacter;
                }
            }
        }
    }
}
