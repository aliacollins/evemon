using System;
using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class CrtRelationships : IHasID
    {
        [XmlElement("relationshipID")]
        public int ID { get; set; }

        [XmlElement("parentID")]
        public int? ParentID;

        [XmlElement("parentTypeID")]
        public int? ParentTypeID;

        [XmlElement("parentLevel")]
        public int? ParentLevel;

        [XmlElement("childID")]
        public int ChildID;
    }
}
