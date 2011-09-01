using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class AgtConfig
    {
        [XmlElement("agentID")]
        public int ID;

        [XmlElement("k")]
        public string Key;

        [XmlElement("v")]
        public string Value;
    }
}