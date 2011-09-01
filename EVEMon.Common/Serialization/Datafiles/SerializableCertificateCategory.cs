using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents a certificate category from our datafiles
    /// </summary>
    public sealed class SerializableCertificateCategory
    {
        [XmlAttribute("id")]
        public int ID { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("descr")]
        public string Description { get; set; }

        [XmlElement("certificateClass")]
        public SerializableCertificateClass[] Classes { get; set; }
    }
}