using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class DgmTypeAttribute : IRelation
    {
        [XmlElement("typeID")]
        public int ItemID;

        [XmlElement("attributeID")]
        public int AttributeID;

        [XmlElement("valueInt")]
        public int? ValueInt;

        [XmlElement("valueFloat")]
        public double? ValueFloat;

        /// <summary>
        /// Returns the value as an integer. 
        /// Some int values are actually stored as floats in the DB, hence this trick.
        /// </summary>
        /// <returns></returns>
        public int GetIntValue()
        {
            return ValueInt.HasValue ? ValueInt.Value : (int)(ValueFloat.HasValue ? ValueFloat.Value : 0);
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