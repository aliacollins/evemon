using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents a certificate class from our datafiles
    /// </summary>
    /// <remarks>
    /// This is the optimized way to implement the object as serializable and satisfy all FxCop rules.
    /// Don't use auto-property with private setter for the collections as it does not work with XmlSerializer.
    /// </remarks>
    public sealed class SerializableCertificateClass
    {
        private readonly Collection<SerializableCertificate> m_certificates;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableCertificateClass"/> class.
        /// </summary>
        public SerializableCertificateClass()
        {
            m_certificates = new Collection<SerializableCertificate>();
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        [XmlAttribute("id")]
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [XmlAttribute("descr")]
        public string Description { get; set; }

        [XmlElement("certificate")]
        public Collection<SerializableCertificate> Certificates
        {
            get { return m_certificates; }
        }

        /// <summary>
        /// Adds the specified certificates.
        /// </summary>
        /// <param name="certificates">The certificates.</param>
        public void AddRange(IEnumerable<SerializableCertificate> certificates)
        {
            m_certificates.Clear();
            certificates.ToList().ForEach(certificate => m_certificates.Add(certificate));
        }
    }
}