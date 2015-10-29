using System.Linq;
using EVEMon.Common.Attributes;
using EVEMon.Common.Collections;
using EVEMon.Common.Models;

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
        /// <param name="character">The character</param>
        /// </summary>
        internal CertificateCollection(Character character)
        {
            // Builds the list
            foreach (CertificateClass certClass in character.CertificateCategories.SelectMany(
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
        /// Initializes the certificates.
        /// </summary>
        internal void Initialize()
        {
            while (true)
            {
                bool updatedAnything = Items.Values
                    .Aggregate(false, (current, cert) => current | cert.TryUpdateCertificateStatus());

                if (!updatedAnything)
                    break;
            }
        }
    }
}