using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Models;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represetns a collection of character's certificate class
    /// </summary>
    public sealed class CertificateClassCollection : ReadonlyKeyedCollection<int, CertificateClass>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="character">The character.</param>
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
        public CertificateClass this[int id] => GetByKey(id);
    }
}