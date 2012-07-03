using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableStandingsListItem
    {
        [XmlAttribute("fromID")]
        public int ID { get; set; }

        [XmlAttribute("fromName")]
        public string Name { get; set; }

        [XmlAttribute("standing")]
        public double StandingValue { get; set; }

        [XmlIgnore]
        public StandingGroup Group { get; set; }
    }
}