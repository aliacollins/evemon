using System.Linq;
using EVEMon.Common.Collections;

namespace EVEMon.Common
{
    /// <summary>
    /// Represetns a collection of character's certificate class
    /// </summary>
    public sealed class CertificateClassCollection : ReadonlyKeyedCollection<long, CertificateClass>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        internal CertificateClassCollection(Character character)
        {
            foreach (CertificateClass certClass in character.CertificateCategories.SelectMany(category => category))
            {
                Items[certClass.ID] = certClass;
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