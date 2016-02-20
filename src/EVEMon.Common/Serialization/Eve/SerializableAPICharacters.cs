using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Eve
{
    /// <summary>
    /// Represents a serializable version of a character's list. Used for querying CCP.
    /// </summary>
    public class SerializableAPICharacters
    {
        private readonly Collection<SerializableCharacterListItem> m_characters;

        public SerializableAPICharacters()
        {
            m_characters = new Collection<SerializableCharacterListItem>();
        }

        [XmlArray("characters")]
        [XmlArrayItem("character")]
        public Collection<SerializableCharacterListItem> Characters => m_characters;
    }
}