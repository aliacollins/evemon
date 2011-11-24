using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents a certificate class.
    /// Every category (i.e. "Business and Industry")
    /// contains certificate classes (i.e. "Production Manager"),
    /// which contain certificates (i.e. "Production Manager Basic").
    /// </summary>
    public sealed class StaticCertificateClass : ReadonlyVirtualCollection<StaticCertificate>
    {
        private readonly StaticCertificate[] m_certificates = new StaticCertificate[4];


        #region Constructor

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="src"></param>
        internal StaticCertificateClass(StaticCertificateCategory category, SerializableCertificateClass src)
        {
            ID = src.ID;
            Name = src.Name;
            Description = src.Description;
            Category = category;

            foreach (StaticCertificate cert in src.Certificates.Select(
                srcCert => new StaticCertificate(this, srcCert)))
            {
                m_certificates[(int)cert.Grade] = cert;
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets this category's id.
        /// </summary>
        public long ID { get; private set; }

        /// <summary>
        /// Gets this category's name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets this category's description.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the certificates class's category.
        /// </summary>
        public StaticCertificateCategory Category { get; private set; }

        /// <summary>
        /// Gets the lowest grade certificate.
        /// </summary>
        public StaticCertificate LowestGradeCertificate
        {
            get { return m_certificates.Where(cert => cert != null).FirstOrDefault(); }
        }

        /// <summary>
        /// Gets the highest grade certificate.
        /// </summary>
        public StaticCertificate HighestGradeCertificate
        {
            get { return m_certificates.Where(cert => cert != null).FirstOrDefault(); }
        }

        #endregion


        #region Indexer

        /// <summary>
        /// Gets the certificate with the specified grade.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        public StaticCertificate this[CertificateGrade grade]
        {
            get { return m_certificates[(int)grade]; }
        }

        #endregion


        #region Overridden Methods

        /// <summary>
        /// Enumerates over the items in this collection.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<StaticCertificate> Enumerate()
        {
            return m_certificates.Where(cert => cert != null);
        }

        /// <summary>
        /// Gets a string representation of this class.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}