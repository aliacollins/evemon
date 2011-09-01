using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents a certificate class from our datafiles
    /// </summary>
    public sealed class SerializableCertificateClass
    {
        [XmlAttribute("id")]
        public int ID { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("descr")]
        public string Description { get; set; }

        [XmlElement("certificate")]
        public SerializableCertificate[] Certificates { get; set; }
    }
}