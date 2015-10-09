using System.Xml.Serialization;
using EVEMon.XmlGenerator.Interfaces;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class AgtAgentTypes : IHasID
    {
        [XmlElement("agentTypeID")]
        public int ID { get; set; }

        [XmlElement("agentType")]
        public string AgentType { get; set; }
    }
}