using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents out certificates datafile
    /// </summary>
    [XmlRoot("certificates")]
    public sealed class CertificatesDatafile
    {
        [XmlElement("certificateCategory")]
        public SerializableCertificateCategory[] Categories
        {
            get;
            set;
        }
    }
}
