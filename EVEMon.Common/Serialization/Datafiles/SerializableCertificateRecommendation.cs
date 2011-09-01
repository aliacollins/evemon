using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents a recommendation for a ship
    /// </summary>
    public sealed class SerializableCertificateRecommendation
    {
        [XmlAttribute("id")]
        public int ID { get; set; }

        [XmlAttribute("level")]
        public int Level { get; set; }

        [XmlAttribute("ship")]
        public string Ship { get; set; }
    }
}