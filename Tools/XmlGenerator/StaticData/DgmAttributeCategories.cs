using System.Xml.Serialization;
using EVEMon.XmlGenerator.Interfaces;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class DgmAttributeCategories : IHasID
    {
        [XmlElement("categoryID")]
        public int ID { get; set; }

        [XmlElement("categoryName")]
        public string Name { get; set; }

        [XmlElement("categoryDescription")]
        public string Description { get; set; }
    }
}