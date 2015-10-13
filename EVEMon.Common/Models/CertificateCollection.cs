using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Attributes;
using EVEMon.Common.Collections;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common.Models
{
    /// <summary>
    /// Represents a collection of certificates
    /// </summary>
    [EnforceUIThreadAffinity]
    public sealed class CertificateCollection : ReadonlyKeyedCollection<int, Certificate>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        internal CertificateCollection(Character character)
        {
            // Builds the list
            foreach (Certificate cert in character.CertificateCategories.SelectMany(
                category => category, (category, certClass) => new { category, certClass }).SelectMany(
                    category => category.certClass))
            {
                Items[cert.ID] = cert;
            }

            // Builds the prerequisites certificates list
            foreach (Certificate cert in Items.Values)
            {
                cert.CompleteInitialization(Items);
            }
        }

        /// <summary>
        /// Gets a certificate from its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Certificate this[int id]
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
            return Items.Values.Where(cert => cert.Status == status);
        }

        /// <summary>
        /// Gets the certificates granted to that character.
        /// </summary>
        public IEnumerable<Certificate> GrantedCertificates
        {
            get { return FilterByStatus(CertificateStatus.Granted); }
        }

        /// <summary>
        /// Exports this collection to a serialization object.
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<SerializableCharacterCertificate> Export()
        {
            return Items.Values.Where(x => x.IsGranted).Select(
                cert => new SerializableCharacterCertificate { CertificateID = cert.ID });
        }

        /// <summary>
        /// Imports data from a serialization object.
        /// </summary>
        /// <param name="certificates">The serial.</param>
        internal void Import(IEnumerable<SerializableCharacterCertificate> certificates)
        {
            // Certificates : reset > mark the granted ones > update the other ones
            foreach (Certificate cert in Items.Values)
            {
                cert.Reset();
            }

            foreach (SerializableCharacterCertificate serialCert in certificates.Where(x => this[x.CertificateID] != null))
            {
                // Take care of the new certs not in our datafiles yet
                // Mark as granted if it exists
                Items[serialCert.CertificateID].MarkAsGranted();
            }

            while (true)
            {
                bool updatedAnything = Items.Values.Aggregate(
                    false, (current, cert) => current | cert.TryUpdateCertificateStatus());

                if (!updatedAnything)
                    break;
            }
        }
    }
}