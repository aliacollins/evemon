using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Attributes;
using EVEMon.Common.Collections;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Models;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common.Data
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
            foreach (var certClass in character.CertificateCategories.SelectMany(
                category => category, (category, certClass) => new { category, certClass }).Select(
                    category => category.certClass))
            {
                Items[certClass.Certificate.ID] = certClass.Certificate;
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
        public IEnumerable<CertificateLevel> FilterByStatus(CertificateStatus status)
        {
            return Items.Values.SelectMany(cert => cert.AllLevel).Where(certLevel => certLevel.Status == status);
        }

        /// <summary>
        /// Gets the certificates granted to that character.
        /// </summary>
        public IEnumerable<CertificateLevel> GrantedCertificates
        {
            get { return FilterByStatus(CertificateStatus.Trained); }
        }

        /// <summary>
        /// Exports this collection to a serialization object.
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<SerializableCharacterCertificate> Export()
        {
            return Items.Values.Where(x => x.AllLevel.Any(certLevel => certLevel.IsTrained)).Select(
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
                foreach (var certLevel in cert.AllLevel)
                {
                    certLevel.Reset();
                }                
            }

            foreach (SerializableCharacterCertificate serialCert in certificates.Where(x => this[x.CertificateID] != null))
            {
                // Take care of the new certs not in our datafiles yet
                // Mark as granted if it exists
                foreach (var certLevel in Items[serialCert.CertificateID].AllLevel)
                {
                    certLevel.MarkAsTrained();
                }
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