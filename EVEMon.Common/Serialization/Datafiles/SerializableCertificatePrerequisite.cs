using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents a skill prerequisite for a certificate
    /// </summary>
    public sealed class SerializableCertificatePrerequisite
    {
        [XmlAttribute("id")]
        public int ID
        {
            get;
            set;
        }

        [XmlAttribute("type")]
        public SerializableCertificatePrerequisiteKind Kind
        {
            get;
            set;
        }

        [XmlAttribute("name")]
        public string Name
        {
            get;
            set;
        }

        [XmlAttribute("level")]
        public string Level
        {
            get;
            set;
        }
    }
}
