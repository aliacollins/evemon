using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class DgmTypeTraits : IRelation
    {
        [XmlElement("typeID")]
        public int ItemID { get; set; }

        [XmlElement("parentTypeID")]
        public int ParentItemID { get; set; }

        [XmlElement("traitID")]
        public int TraitID { get; set; }

        [XmlElement("bonus")]
        public double? Bonus { get; set; }


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
            get { return TraitID; }
        }

        #endregion
    }
}
