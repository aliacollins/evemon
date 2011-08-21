using System;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    #region EvePropertyValue
    /// <summary>
    /// Describes a property of a ship/item (e.g. CPU size)
    /// </summary>
    public struct EvePropertyValue
    {
        #region Constructor

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="src"></param>
        internal EvePropertyValue(SerializablePropertyValue src)
            : this()
        {
            Property = StaticProperties.GetPropertyByID(src.ID);
            Value = String.Intern(src.Value);
        }
        
        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the property.
        /// </summary>
        public EveProperty Property { get; private set; }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Gets the integer value.
        /// </summary>
        public int IValue
        {
            get { return Int32.Parse(Value); }
        }

        /// <summary>
        /// Gets the floating point value.
        /// </summary>
        public float FValue
        {
            get { return Single.Parse(Value); }
        }

        #endregion


        # region Overridden Methods
        /// <summary>
        /// Gets a string representation of this prerequisite.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Property.Name;
        }

        #endregion
    }

    # endregion
}
