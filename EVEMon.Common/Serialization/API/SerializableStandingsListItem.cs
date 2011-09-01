using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public abstract class SerializableStandingsListItem
    {
        [XmlAttribute("fromID")]
        public int ID { get; set; }

        [XmlAttribute("fromName")]
        public string Name { get; set; }

        [XmlAttribute("standing")]
        public double StandingValue { get; set; }

        [XmlIgnore]
        public string GroupType { get; protected set; }
    }
}