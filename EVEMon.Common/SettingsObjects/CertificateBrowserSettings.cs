using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    public sealed class CertificateBrowserSettings
    {
        /// <summary>
        /// Gets or sets the text search.
        /// </summary>
        /// <value>The text search.</value>
        [XmlElement("textSearch")]
        public string TextSearch { get; set; }

        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>The filter.</value>
        [XmlElement("filter")]
        public CertificateFilter Filter { get; set; }

        /// <summary>
        /// Gets or sets the sort.
        /// </summary>
        /// <value>The sort.</value>
        [XmlElement("sort")]
        public CertificateSort Sort { get; set; }
    }
}