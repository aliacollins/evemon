using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents an eve solar system.
    /// </summary>
    public sealed class SerializableSolarSystem
    {
        [XmlAttribute("id")]
        public int ID { get; set; }

        [XmlAttribute("x")]
        public int X { get; set; }

        [XmlAttribute("y")]
        public int Y { get; set; }

        [XmlAttribute("z")]
        public int Z { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("securityLevel")]
        public float SecurityLevel { get; set; }

        [XmlElement("stations")]
        public SerializableStation[] Stations { get; set; }
    }
}