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
        private EveProperty m_property;
        private string m_value;

        #region Constructor

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="src"></param>
        internal EvePropertyValue(SerializablePropertyValue src)
        {
            m_property = StaticProperties.GetPropertyByID(src.ID);
            m_value = String.Intern(src.Value);
        }
        
        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the property.
        /// </summary>
        public EveProperty Property
        {
            get { return m_property; }
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        public string Value
        {
            get { return m_value; }
        }

        /// <summary>
        /// Gets the integer value.
        /// </summary>
        public int IValue
        {
            get { return Int32.Parse(m_value); }
        }

        /// <summary>
        /// Gets the floating point value.
        /// </summary>
        public float FValue
        {
            get { return Single.Parse(m_value); }
        }

        #endregion


        # region Overridden Methods
        /// <summary>
        /// Gets a string representation of this prerequisite.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return m_property.Name;
        }

        #endregion
    }

    # endregion
}
