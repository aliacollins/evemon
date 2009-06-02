using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents a certificate class. Every category
    /// (i.e. "Business and Industry") contains certificate classes
    /// (i.e. "Production Manager"), which contain certificates
    /// (i.e. "Production Manager Basic").
    /// </summary>
    public sealed class CertificateClass
    {
        private readonly Certificate[] certificates = new Certificate[4];

        public readonly int ID;
        public readonly string Name;
        public readonly string Description;
        public readonly CertificateCategory Category;

        /// <summary>
        /// Constructor from XML
        /// </summary>
        /// <param name="category"></param>
        /// <param name="element"></param>
        internal CertificateClass(CertificateCategory category, XmlElement element)
        {
            this.Category = category;
            this.Name = element.GetAttribute("name");
            this.Description = element.GetAttribute("descr");
            this.ID = Int32.Parse(element.GetAttribute("id"));

            if (element.HasChildNodes)
            {
                foreach (var child in element.ChildNodes)
                {
                    var cert = new Certificate(this, (XmlElement)child);
                    this.certificates[(int)cert.Grade] = cert;
                }
            }
        }

        /// <summary>
        /// Gets the non-null certificates within this class, sorted by grade
        /// </summary>
        public IEnumerable<Certificate> Certificates
        {
            get
            {
                foreach (var cert in this.certificates)
                {
                    if (cert != null) yield return cert;
                }
            }
        }

        /// <summary>
        /// Gets the certificate with the specified grade
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Certificate this[CertificateGrade grade]
        {
            get { return this.certificates[(int)grade]; }
        }

        /// <summary>
        /// Gets true if the provided character has completed this class
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public bool HasBeenCompletedBy(CharacterInfo character)
        {
            foreach (var cert in Certificates)
            {
                if (character.GetCertificateStatus(cert) != CertificateStatus.Granted) return false;
            }
            return true;
        }

        /// <summary>
        /// Gets true if the provided character can train to the next
        /// grade, false if the class has already been completed or if
        /// the next grade is untrainable.
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public bool IsFurtherTrainableBy(CharacterInfo character)
        {
            foreach (var cert in Certificates)
            {
                var status = character.GetCertificateStatus(cert);
                if (status == CertificateStatus.PartiallyTrained) return true;
                else if (status == CertificateStatus.Untrained) return false;
            }
            return false;
        }

        /// <summary>
        /// Gets the lowest grade, untrained, certificate for the provided character
        /// </summary>
        /// <param name="character"></param>
        public Certificate GetLowestGradeUntrainedCertificate(CharacterInfo character)
        {
            // Look for the next grade
            foreach (var cert in Certificates)
            {
                var status = character.GetCertificateStatus(cert);
                if (status != CertificateStatus.Claimable && status != CertificateStatus.Granted) return cert;
            }
            return null;
        }

        /// <summary>
        /// Gets the highest grade, claimed, certificate for the provided character
        /// </summary>
        /// <param name="character"></param>
        public Certificate GetHighestGradeClaimedCertificate(CharacterInfo character)
        {
            // Look for the next grade
            Certificate lastCert = null;
            foreach (var cert in Certificates)
            {
                var status = character.GetCertificateStatus(cert);
                if (status != CertificateStatus.Granted) return lastCert;
                lastCert = cert;
            }
            return lastCert;
        }
        
        /// <summary>
        /// Gets the lowest grade certificate.
        /// </summary>
        public Certificate LowestGradeCertificate
        {
            get
            {
                foreach (var cert in Certificates) return cert;
                throw new NotImplementedException();
            }
        }
        
        /// <summary>
        /// Gets the highest grade certificate.
        /// </summary>
        public Certificate HighestGradeCertificate
        {
            get
            {
                // Look for the next grade
                Certificate lastCert = null;
                foreach (var cert in Certificates) lastCert = cert;
                return lastCert;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }
    } 
}
