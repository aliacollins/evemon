using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class InvMetaType : IRelation
    {
        [XmlElement("typeID")]
        public int ItemID;

        [XmlElement("parentTypeID")]
        public int ParentItemID;

        [XmlElement("metaGroupID")]
        public int MetaGroupID;


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
