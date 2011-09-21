using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents out certificates datafile
    /// </summary>
    /// <remarks>
    /// This is the optimized way to implement the object as serializable and satisfy all FxCop rules.
    /// Don't use auto-property with private setter for the collections as it does not work with XmlSerializer.
    /// </remarks>
    [XmlRoot("certificates")]
    public sealed class CertificatesDatafile
    {
        private Collection<SerializableCertificateCategory> m_categories;

        /// <summary>
        /// Initializes a new instance of the <see cref="CertificatesDatafile"/> class.
        /// </summary>
        public CertificatesDatafile()
        {
            m_categories = new Collection<SerializableCertificateCategory>();
        }

        /// <summary>
        /// Gets the categories.
        /// </summary>
        /// <value>The categories.</value>
        [XmlElement("certificateCategory")]
        public Collection<SerializableCertificateCategory> Categories
        {
            get { return m_categories; }
        }

        /// <summary>
        /// Adds the specified categories.
        /// </summary>
        /// <param name="categories">The categories.</param>
        public void Add(List<SerializableCertificateCategory> categories)
        {
            m_categories = new Collection<SerializableCertificateCategory>(categories);
        }
    }
}