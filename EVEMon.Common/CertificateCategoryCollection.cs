using System.Linq;
using EVEMon.Common.Attributes;
using EVEMon.Common.Collections;
using EVEMon.Common.Data;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents a certificate category.
    /// </summary>
    [EnforceUIThreadAffinity]
    public sealed class CertificateCategoryCollection : ReadonlyKeyedCollection<long, CertificateCategory>
    {
        /// <summary>
        /// Constructor for the character initialization.
        /// </summary>
        /// <param name="character"></param>
        internal CertificateCategoryCollection(Character character)
        {
            foreach (CertificateCategory category in StaticCertificates.Categories.Select(
                srcCategory => new CertificateCategory(character, srcCategory)))
            {
                Items[category.ID] = category;
            }
        }
    }
}