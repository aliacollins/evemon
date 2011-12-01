using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class DgmTypeAttribute : IRelation
    {
        [XmlElement("typeID")]
        public long ItemID { get; set; }

        [XmlElement("attributeID")]
        public long AttributeID { get; set; }

        [XmlElement("valueInt")]
        public int? ValueInt { get; set; }

        [XmlElement("valueFloat")]
        public double? ValueFloat { get; set; }

        /// <summary>
        /// Returns the value as an integer. 
        /// Some int values are actually stored as floats in the DB, hence this trick.
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public int GetIntValue
        {
            get { return ValueInt.HasValue ? ValueInt.Value : (int)(ValueFloat.HasValue ? ValueFloat.Value : 0); }
        }


        #region IRelation Members

        /// <summary>
        /// Gets the left column value.
        /// </summary>
        /// <value>The left.</value>
        long IRelation.Left
        {
            get { return ItemID; }
        }

        /// <summary>
        /// Gets the right column value.
        /// </summary>
        /// <value>The right.</value>
        long IRelation.Right
        {
            get { return AttributeID; }
        }

        #endregion
    }
}