using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class EveIcons : IHasID
    {
        [XmlElement("iconID")]
        public long ID { get; set; }

        [XmlElement("iconFile")]
        public string Icon { get; set; }
    }
}