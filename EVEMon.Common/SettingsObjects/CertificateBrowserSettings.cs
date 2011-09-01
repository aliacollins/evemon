using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    public sealed class CertificateBrowserSettings
    {
        [XmlElement("textSearch")]
        public string TextSearch { get; set; }

        [XmlElement("filter")]
        public CertificateFilter Filter { get; set; }

        [XmlElement("sort")]
        public CertificateSort Sort { get; set; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal CertificateBrowserSettings Clone()
        {
            return (CertificateBrowserSettings)MemberwiseClone();
        }
    }
}