using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents a certificate group.
    /// </summary>
    public sealed class StaticCertificateGroup : ReadonlyCollection<StaticCertificateClass>
    {
        #region Constructor

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="src"></param>
        internal StaticCertificateGroup(SerializableCertificateGroup src)
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
        /// Gets this group's id.
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// Gets this group's name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets this group's description.
        /// </summary>
        public string Description { get; private set; }

        #endregion


        #region Overridden Methods

        /// <summary>
        /// Gets a string representation of this class.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}