using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace EVEMon.XmlImporter.Zofu
{
    public sealed class CrtRelationships : IHasID
    {
        [XmlElement("relationshipID")]
        public int ID
        {
            get;
            set;
        }

        [XmlElement("parentID")]
        public Nullable<int> ParentID;

        [XmlElement("parentTypeID")]
        public Nullable<int> ParentTypeID;

        [XmlElement("parentLevel")]
        public Nullable<int> ParentLevel;

        [XmlElement("childID")]
        public int ChildID;
    }

}
