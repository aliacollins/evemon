using System;
using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class DgmTypeAttributes : IRelation
    {
        [XmlElement("typeID")]
        public int ItemID { get; set; }

        [XmlElement("attributeID")]
        public int AttributeID { get; set; }

        [XmlElement("valueInt")]
        public long? ValueInt64 { get; set; }

        [XmlElement("valueFloat")]
        public double? ValueFloat { get; set; }

        /// <summary>
        /// Returns the value as an integer. 
        /// Some integer values are actually stored as floats in the DB, hence this trick.
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        internal Int64 GetInt64Value
        {
            get { return ValueFloat.HasValue ? Convert.ToInt64(ValueFloat.Value) : ValueInt64.HasValue ? ValueInt64.Value : 0; }
        }


        #region IRelation Members

        /// <summary>
        /// Gets the left column value.
        /// </summary>
        /// <value>The left.</value>
        int IRelation.Left
        {
            get { return ItemID; }
        }

        /// <summary>
        /// Gets the center column value.
        /// </summary>
        /// <value>
        /// The center.
        /// </value>
        int IRelation.Center
        {
            get { return 0; }
        }

        /// <summary>
        /// Gets the right column value.
        /// </summary>
        /// <value>The right.</value>
        int IRelation.Right
        {
            get { return AttributeID; }
        }

        #endregion
    }
}