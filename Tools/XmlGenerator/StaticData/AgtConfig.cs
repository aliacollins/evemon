using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class AgtConfig
    {
        [XmlElement("agentID")]
        public int ID { get; set; }

        [XmlElement("k")]
        public string Key { get; set; }

        [XmlElement("v")]
        public string Value { get; set; }
    }
}