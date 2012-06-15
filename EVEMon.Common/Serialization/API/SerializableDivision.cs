using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public class SerializableDivision
    {
        [XmlAttribute("accountKey")]
        public int AccountKey { get; set; }

        [XmlAttribute("description")]
        public string Description { get; set; }
    }
}