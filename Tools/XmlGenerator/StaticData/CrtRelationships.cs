using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class CrtRelationships : IHasID
    {
        [XmlElement("relationshipID")]
        public long ID { get; set; }

        [XmlElement("parentID")]
        public int? ParentID { get; set; }

        [XmlElement("parentTypeID")]
        public int? ParentTypeID { get; set; }

        [XmlElement("parentLevel")]
        public int? ParentLevel { get; set; }

        [XmlElement("childID")]
        public int ChildID { get; set; }
    }
}