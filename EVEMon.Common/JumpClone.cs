using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class JumpClone
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JumpClone"/> class.
        /// </summary>
        /// <param name="src">The source.</param>
        internal JumpClone(SerializableCharacterJumpClone src)
        {
            ID = src.JumpCloneID;
            Name = src.CloneName;
            TypeID = src.TypeID;
            LocationID = src.LocationID;
        }


        #region Public Properties

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public long ID { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type identifier.
        /// </summary>
        /// <value>
        /// The type identifier.
        /// </value>
        public int TypeID { get; set; }

        /// <summary>
        /// Gets or sets the location identifier.
        /// </summary>
        /// <value>
        /// The location identifier.
        /// </value>
        public long LocationID { get; set; }


        #endregion


        #region Exportation

        /// <summary>
        /// Exports this instance.
        /// </summary>
        /// <returns></returns>
        internal SerializableCharacterJumpClone Export()
        {
            return new SerializableCharacterJumpClone
            {
                JumpCloneID = ID,
                TypeID = TypeID,
                LocationID = LocationID,
                CloneName = Name
            };
        }

        #endregion
    }
}
