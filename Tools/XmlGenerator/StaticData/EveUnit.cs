using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class EveUnit : IHasID
    {
        [XmlElement("unitID")]
        public long ID { get; set; }

        [XmlElement("unitName")]
        public string Name { get; set; }

        [XmlElement("displayName")]
        public string DisplayName { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }
    }
}