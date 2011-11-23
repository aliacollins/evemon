using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class InvTypeMaterials
    {
        [XmlElement("typeID")]
        public int TypeID { get; set; }

        [XmlElement("materialTypeID")]
        public int MaterialTypeID { get; set; }

        [XmlElement("quantity")]
        public int Quantity { get; set; }
    }
}