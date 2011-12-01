using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class InvMarketGroup : IHasID
    {
        [XmlElement("marketGroupID")]
        public long ID { get; set; }

        [XmlElement("marketGroupName")]
        public string Name { get; set; }

        [XmlElement("parentGroupID")]
        public long? ParentID { get; set; }

        [XmlElement("iconID")]
        public long? IconID { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }
    }
}