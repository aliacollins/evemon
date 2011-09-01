using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents a certificate from our datafiles
    /// </summary>
    public sealed class SerializableCertificate
    {
        [XmlAttribute("id")]
        public int ID { get; set; }

        [XmlAttribute("grade")]
        public CertificateGrade Grade { get; set; }

        [XmlAttribute("descr")]
        public string Description { get; set; }

        [XmlElement("requires")]
        public SerializableCertificatePrerequisite[] Prerequisites { get; set; }

        [XmlElement("recommendation")]
        public SerializableCertificateRecommendation[] Recommendations { get; set; }
    }
}