using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Eve
{
    public sealed class SerializableAPICharacterName
    {
        private readonly Collection<SerializableCharacterNameListItem> m_entities;

        public SerializableAPICharacterName()
        {
            m_entities = new Collection<SerializableCharacterNameListItem>();
        }

        [XmlArray("characters")]
        [XmlArrayItem("character")]
        public Collection<SerializableCharacterNameListItem> Entities => m_entities;
    }
}