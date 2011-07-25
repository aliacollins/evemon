using EVEMon.Common.Collections;

namespace EVEMon.Common
{
    /// <summary>
    /// Represetns a collection of character's certificate class
    /// </summary>
    public sealed class CertificateClassCollection : ReadonlyKeyedCollection<long, CertificateClass>
    {
        private readonly Character m_character;

        /// <summary>
        /// Constructor.
        /// </summary>
        internal CertificateClassCollection(Character character)
        {
            m_character = character;

            foreach (CertificateCategory category in character.CertificateCategories)
            {
                foreach (CertificateClass certClass in category)
                {
                    m_items[certClass.ID] = certClass;
                }
            }
        }

        /// <summary>
        /// Gets a certificate class from its name.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CertificateClass this[long id]
        {
            get { return GetByKey(id); }
        }
    }
}
