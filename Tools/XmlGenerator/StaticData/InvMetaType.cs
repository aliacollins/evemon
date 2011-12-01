using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class InvMetaType : IRelation
    {
        [XmlElement("typeID")]
        public long ItemID { get; set; }

        [XmlElement("parentTypeID")]
        public long ParentItemID { get; set; }

        [XmlElement("metaGroupID")]
        public long MetaGroupID { get; set; }


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
        /// Gets the right.
        /// </summary>
        /// <value>The right column value.</value>
        long IRelation.Right
        {
            get { return MetaGroupID; }
        }

        #endregion
    }
}