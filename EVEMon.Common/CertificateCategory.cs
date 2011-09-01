using System.Linq;
using EVEMon.Common.Attributes;
using EVEMon.Common.Collections;
using EVEMon.Common.Data;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents a certificate category from a character's point of view.
    /// </summary>
    [EnforceUIThreadAffinity]
    public sealed class CertificateCategory : ReadonlyKeyedCollection<string, CertificateClass>
    {
        private readonly Character m_character;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="character"></param>
        /// <param name="src"></param>
        internal CertificateCategory(Character character, StaticCertificateCategory src)
        {
            m_character = character;
            StaticData = src;

            foreach (CertificateClass certClass in src.Select(srcClass => new CertificateClass(character, srcClass, this)))
            {
                Items[certClass.Name] = certClass;
            }
        }

        /// <summary>
        /// Gets the static data associated with this object.
        /// </summary>
        public StaticCertificateCategory StaticData { get; private set; }

        /// <summary>
        /// Gets this skill's id
        /// </summary>
        public long ID
        {
            get { return StaticData.ID; }
        }

        /// <summary>
        /// Gets this skill's name
        /// </summary>
        public string Name
        {
            get { return StaticData.Name; }
        }

        /// <summary>
        /// Gets this skill's description
        /// </summary>
        public string Description
        {
            get { return StaticData.Description; }
        }
    }
}