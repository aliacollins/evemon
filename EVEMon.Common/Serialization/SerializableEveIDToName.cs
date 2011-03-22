using System.Collections.Generic;
using System.Xml.Serialization;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common.Serialization.Settings
{
    [XmlRoot("EveIDToName")]
    public sealed class SerializableEveIDToName
    {
        public SerializableEveIDToName()
        {
            Entities = new List<SerializableCharacterNameListItem>();
        }

        [XmlArray("entities")]
        [XmlArrayItem("entity")]
        public List<SerializableCharacterNameListItem> Entities
        {
            get;
            set;
        }

    }
}
