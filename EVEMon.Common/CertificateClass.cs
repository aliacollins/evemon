using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Attributes;
using EVEMon.Common.Collections;
using EVEMon.Common.Data;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents a certificate class from a character's point of view.
    /// </summary>
    [EnforceUIThreadAffinity]
    public sealed class CertificateClass : ReadonlyVirtualCollection<Certificate>
    {
        private readonly Certificate[] m_items = new Certificate[4];

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="src"></param>
        /// <param name="category"></param>
        internal CertificateClass(Character character, StaticCertificateClass src, CertificateCategory category)
        {
            Category = category;
            StaticData = src;

            foreach (Certificate cert in src.Select(srcCert => new Certificate(character, srcCert, this)))
            {
                m_items[(int)cert.Grade] = cert;
            }
        }

        /// <summary>
        /// Core implementation of the <see cref="ReadonlyVirtualCollection{T}"/> collection.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<Certificate> Enumerate()
        {
            return m_items.Where(cert => cert != null);
        }

        /// <summary>
        /// Gets the static data associated with this object.
        /// </summary>
        public StaticCertificateClass StaticData { get; private set; }

        /// <summary>
        /// Gets the category for this certificate class.
        /// </summary>
        public CertificateCategory Category { get; private set; }

        /// <summary>
        /// Gets this skill's id.
        /// </summary>
        public long ID
        {
            get { return StaticData.ID; }
        }

        /// <summary>
        /// Gets this skill's name.
        /// </summary>
        public string Name
        {
            get { return StaticData.Name; }
        }

        /// <summary>
        /// Gets this skill's description.
        /// </summary>
        public string Description
        {
            get { return StaticData.Description; }
        }

        /// <summary>
        /// Gets a certificate from this class by its grade.
        /// May be null if there is no such grade for this class.
        /// </summary>
        /// <param name="grade"></param>
        /// <returns></returns>
        public Certificate this[CertificateGrade grade]
        {
            get { return m_items[(int)grade]; }
        }

        /// <summary>
        /// Gets the lowest grade certificate.
        /// </summary>
        public Certificate LowestGradeCertificate
        {
            get
            {
                StaticCertificate scert = StaticData.LowestGradeCertificate;
                return m_items[(int)scert.Grade];
            }
        }

        /// <summary>
        /// Gets the highest grade certificate.
        /// </summary>
        public Certificate HighestGradeCertificate
        {
            get
            {
                StaticCertificate scert = StaticData.HighestGradeCertificate;
                return m_items[(int)scert.Grade];
            }
        }

        /// <summary>
        /// Gets the lowest untrained (neither granted nor claimable).
        /// Null if all certificates have been granted or are claimable.
        /// </summary>
        public Certificate LowestUntrainedGrade
        {
            get
            {
                return m_items.Where(cert => cert != null).FirstOrDefault(
                    cert => cert.Status != CertificateStatus.Claimable && cert.Status != CertificateStatus.Granted);
            }
        }

        /// <summary>
        /// Gets the highest claimed grade.
        /// May be null if no grade has been granted.
        /// </summary>
        public Certificate HighestClaimedGrade
        {
            get
            {
                Certificate lastCert = null;
                foreach (Certificate cert in m_items.Where(cert => cert != null))
                {
                    if (cert.Status != CertificateStatus.Granted)
                        return lastCert;
                    lastCert = cert;
                }
                return lastCert;
            }
        }

        /// <summary>
        /// Gets true if the provided character has completed this class.
        /// </summary>
        public bool IsCompleted
        {
            get { return m_items.All(cert => cert == null || cert.Status == CertificateStatus.Granted); }
        }

        /// <summary>
        /// Gets true if the provided character can train to the next grade,
        /// false if the class has already been completed or if the next grade is untrainable.
        /// </summary>
        /// <returns></returns>
        public bool IsFurtherTrainable
        {
            get
            {
                foreach (Certificate cert in m_items.Where(cert => cert != null))
                {
                    switch (cert.Status)
                    {
                        case CertificateStatus.PartiallyTrained:
                            return true;
                        case CertificateStatus.Untrained:
                            return false;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Gets the name of the class.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return StaticData.Name;
        }
    }
}