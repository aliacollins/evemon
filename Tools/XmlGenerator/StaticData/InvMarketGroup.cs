using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class InvMarketGroup : IHasID
    {
        [XmlElement("marketGroupID")]
        public int ID { get; set; }

        [XmlElement("marketGroupName")]
        public string Name { get; set; }

        [XmlElement("parentGroupID")]
        public int? ParentID { get; set; }

        [XmlElement("iconID")]
        public int? IconID { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }
    }
}