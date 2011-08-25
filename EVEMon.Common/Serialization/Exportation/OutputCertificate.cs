using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Exportation
{
    /// <summary>
    /// A serialization class designed for HTML exportation.
    /// </summary>
    public sealed class OutputCertificate
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("grade")]
        public string Grade { get; set; }
    }
}