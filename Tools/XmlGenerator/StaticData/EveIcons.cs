using System.Xml.Serialization;
using EVEMon.XmlGenerator.Interfaces;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class EveIcons : IHasID
    {
        [XmlElement("iconID")]
        public int ID { get; set; }

        [XmlElement("iconFile")]
        public string Icon { get; set; }
    }
}