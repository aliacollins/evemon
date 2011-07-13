using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Attributes;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents a collection of certificates
    /// </summary>
    [EnforceUIThreadAffinity]
    public sealed class CertificateCollection : ReadonlyKeyedCollection<long, Certificate>
    {
        private readonly Character m_character;

        /// <summary>
        /// Constructor
        /// </summary>
        internal CertificateCollection(Character character)
        {
            m_character = character;

            // Builds the list
            foreach (CertificateCategory category in character.CertificateCategories)
            {
                foreach (CertificateClass certClass in category)
                {
                    foreach (Certificate cert in certClass)
                    {
                        m_items[cert.ID] = cert;
                    }
                }
            }

            // Builds the prerequisites certificates list.
            foreach (Certificate cert in m_items.Values)
            {
                cert.CompleteInitialization(m_items);
            }
        }

        /// <summary>
        /// Gets a certificate from its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Certificate this[long id]
        {
            get { return GetByKey(id); }
        }

        /// <summary>
        /// Gets the certificates with the specified status.
        /// </summary>
        /// <param name="status">The status the certificates must have</param>
        /// <returns></returns>
        public IEnumerable<Certificate> FilterByStatus(CertificateStatus status)
        {
            foreach (Certificate cert in m_items.Values)
            {
                if (cert.Status == status) 
                    yield return cert;
            }
        }

        /// <summary>
        /// Gets the certificates granted to that character.
        /// </summary>
        public IEnumerable<Certificate> GrantedCertificates
        {
            get
            {
                return FilterByStatus(CertificateStatus.Granted);
            }
        }

        /// <summary>
        /// Exports this collection to a serialization object.
        /// </summary>
        /// <returns></returns>
        internal List<SerializableCharacterCertificate> Export()
        {
            List < SerializableCharacterCertificate > certificates = new List<SerializableCharacterCertificate>();
            foreach (var cert in m_items.Values.Where(x => x.IsGranted))
            {
                certificates.Add(new SerializableCharacterCertificate { CertificateID = cert.ID });
            }
            return certificates;
        }

        /// <summary>
        /// Imports data from a serialization object.
        /// </summary>
        /// <param name="serial">The serial.</param>
        internal void Import(List<SerializableCharacterCertificate> certificates)
        {
            // Certificates : reset > mark the granted ones > update the other ones
            foreach (Certificate cert in m_items.Values)
            {
                cert.Reset();
            }

            foreach (SerializableCharacterCertificate serialCert in certificates.Where(x => m_items[x.CertificateID] != null))
            {
                // Take care of the new certs not in our datafiles yet. Mark as granted if it exists.
                m_items[serialCert.CertificateID].MarkAsGranted();
            }

            while (true)
            {
                bool updatedAnything = false;
                foreach (Certificate cert in m_items.Values)
                {
                    updatedAnything |= cert.TryUpdateCertificateStatus();
                }

                if (!updatedAnything)
                    break;
            }
        }
    }
}
