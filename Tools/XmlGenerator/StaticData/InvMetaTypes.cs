using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class InvMetaTypes : IRelation
    {
        [XmlElement("typeID")]
        public int ItemID { get; set; }

        [XmlElement("parentTypeID")]
        public int ParentItemID { get; set; }

        [XmlElement("metaGroupID")]
        public int MetaGroupID { get; set; }


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
            get { return ParentItemID; }
        }

        /// <summary>
        /// Gets the right.
        /// </summary>
        /// <value>The right column value.</value>
        int IRelation.Right
        {
            get { return MetaGroupID; }
        }

        #endregion
    }
}