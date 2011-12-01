using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class CrtCategories : IHasID
    {
        [XmlElement("categoryID")]
        public long ID { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlElement("categoryName")]
        public string CategoryName { get; set; }
    }
}