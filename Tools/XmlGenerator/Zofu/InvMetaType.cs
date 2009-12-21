using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using EVEMon.XmlImporter.Zofu;

namespace EVEMon.XmlGenerator.Zofu
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
        int IRelation.Left
        {
            get { return ItemID; }
        }

        int IRelation.Right
        {
            get { return MetaGroupID; }
        }
        #endregion

    }
}
