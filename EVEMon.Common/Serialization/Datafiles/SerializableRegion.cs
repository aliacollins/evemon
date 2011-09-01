using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents a region of the eve universe.
    /// </summary>
    public sealed class SerializableRegion
    {
        [XmlAttribute("id")]
        public int ID { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("constellations")]
        public SerializableConstellation[] Constellations { get; set; }
    }
}