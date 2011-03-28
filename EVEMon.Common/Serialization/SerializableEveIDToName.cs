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
            Entities = new List<SerializableEveIDToNameListItem>();
        }

        [XmlArray("entities")]
        [XmlArrayItem("entity")]
        public List<SerializableEveIDToNameListItem> Entities
        {
            get;
            set;
        }

    }
}
