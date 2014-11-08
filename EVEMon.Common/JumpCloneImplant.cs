using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class JumpCloneImplant
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JumpCloneImplant"/> class.
        /// </summary>
        /// <param name="src">The source.</param>
        internal JumpCloneImplant(SerializableCharacterJumpCloneImplant src)
        {
            ID = src.JumpCloneID;
            Name = src.TypeName;
            TypeID = src.TypeID;
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


        #endregion


        #region Exportation

        /// <summary>
        /// Exports this instance.
        /// </summary>
        /// <returns></returns>
        internal SerializableCharacterJumpCloneImplant Export()
        {
            return new SerializableCharacterJumpCloneImplant
            {
                JumpCloneID = ID,
                TypeID = TypeID,
                TypeName = Name
            };
        }

        #endregion

    }
}
