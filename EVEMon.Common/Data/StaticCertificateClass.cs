using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents a certificate class.
    /// </summary>
    public sealed class StaticCertificateClass
    {
        #region Constructor

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="src"></param>
        internal StaticCertificateClass(StaticCertificateGroup group, SerializableCertificateClass src)
        {
            ID = src.ID;
            Name = src.Name;
            Description = src.Description;
            Group = group;
            Certificate = new StaticCertificate(this, src.Certificate);
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets this class's id.
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// Gets this class's name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets this class's description.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the certificates class's Group.
        /// </summary>
        public StaticCertificateGroup Group { get; private set; }

        public StaticCertificate Certificate { get; private set; }

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