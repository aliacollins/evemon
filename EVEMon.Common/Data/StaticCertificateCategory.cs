using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents a certificate category.
    /// Every category (i.e. "Business and Industry")
    /// contains certificate classes (i.e. "Production Manager"),
    /// which contain certificates (i.e. "Production Manager Basic").
    /// </summary>
    public sealed class StaticCertificateCategory : ReadonlyCollection<StaticCertificateClass>
    {
        #region Constructor

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="src"></param>
        internal StaticCertificateCategory(SerializableCertificateCategory src)
            : base(src.Classes.Count)
        {
            ID = src.ID;
            Name = src.Name;
            Description = src.Description;

            foreach (SerializableCertificateClass srcClass in src.Classes)
            {
                Items.Add(new StaticCertificateClass(this, srcClass));
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets this category's id.
        /// </summary>
        public long ID { get; private set; }

        /// <summary>
        /// Gets this category's name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets this category's description.
        /// </summary>
        public string Description { get; private set; }

        #endregion


        #region Overridden Methods

        /// <summary>
        /// Gets a string representation of this category.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}