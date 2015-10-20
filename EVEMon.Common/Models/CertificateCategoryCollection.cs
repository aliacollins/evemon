using System.Linq;
using EVEMon.Common.Attributes;
using EVEMon.Common.Collections;
using EVEMon.Common.Data;

namespace EVEMon.Common.Models
{
    /// <summary>
    /// Represents a certificate category.
    /// </summary>
    [EnforceUIThreadAffinity]
    public sealed class CertificateCategoryCollection : ReadonlyKeyedCollection<int, CertificateGroup>
    {
        /// <summary>
        /// Constructor for the character initialization.
        /// </summary>
        /// <param name="character"></param>
        internal CertificateCategoryCollection(Character character)
        {
            foreach (CertificateGroup category in StaticCertificates.Groups.Select(
                srcCategory => new CertificateGroup(character, srcCategory)))
            {
                Items[category.ID] = category;
            }
        }
    }
}