using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableNewImplant
    {
        [XmlAttribute("typeID")]
        public int ID { get; set; }

        [XmlAttribute("typeName")]
        public string Name { get; set; }
    }
}