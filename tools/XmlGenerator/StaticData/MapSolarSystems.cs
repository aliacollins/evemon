using System.Xml.Serialization;
using EVEMon.XmlGenerator.Interfaces;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class MapSolarSystems : IHasID
    {
        [XmlElement("solarSystemID")]
        public int ID { get; set; }

        [XmlElement("solarSystemName")]
        public string Name { get; set; }

        [XmlElement("security")]
        public float SecurityLevel { get; set; }

        [XmlElement("constellationID")]
        public int ConstellationID { get; set; }

        [XmlElement("x")]
        public double X { get; set; }

        [XmlElement("y")]
        public double Y { get; set; }

        [XmlElement("z")]
        public double Z { get; set; }
    }
}