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
        private readonly Character m_character;

        /// <summary>
        /// Constructor for the character initialization.
        /// </summary>
        /// <param name="character"></param>
        internal CertificateCategoryCollection(Character character)
        {
            m_character = character;

            foreach (StaticCertificateCategory srcCategory in StaticCertificates.Categories)
            {
                CertificateCategory category = new CertificateCategory(character, srcCategory);
                Items[category.ID] = category;
            }
        }
    }
}
