using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class MapSolarSystemsJump
    {
        [XmlElement("fromSolarSystemID")]
        public int A { get; set; }

        [XmlElement("toSolarSystemID")]
        public int B { get; set; }
    }
}