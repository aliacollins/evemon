using System.Xml.Serialization;
using EVEMon.XmlGenerator.Interfaces;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class MapRegions : IHasID
    {
        [XmlElement("regionID")]
        public int ID { get; set; }

        [XmlElement("regionName")]
        public string Name { get; set; }

        [XmlElement("factionID")]
        public int? FactionID { get; set; }
    }
}