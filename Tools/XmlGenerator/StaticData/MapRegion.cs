using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class MapRegion : IHasID
    {
        [XmlElement("regionID")]
        public long ID { get; set; }

        [XmlElement("regionName")]
        public string Name { get; set; }

        [XmlElement("factionID")]
        public int? FactionID { get; set; }
    }
}