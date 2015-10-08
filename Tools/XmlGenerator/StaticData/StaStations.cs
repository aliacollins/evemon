using System.Xml.Serialization;
using EVEMon.XmlGenerator.Interfaces;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class StaStations : IHasID
    {
        [XmlElement("stationID")]
        public int ID { get; set; }

        [XmlElement("stationName")]
        public string Name { get; set; }

        [XmlElement("security")]
        public int SecurityLevel { get; set; }

        [XmlElement("corporationID")]
        public int CorporationID { get; set; }

        [XmlElement("solarSystemID")]
        public int SolarSystemID { get; set; }

        [XmlElement("reprocessingEfficiency")]
        public float ReprocessingEfficiency { get; set; }

        [XmlElement("reprocessingStationsTake")]
        public float ReprocessingStationsTake { get; set; }
    }
}