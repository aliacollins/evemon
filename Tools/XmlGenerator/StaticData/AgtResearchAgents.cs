using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class AgtResearchAgents : IHasID
    {
        [XmlElement("agentID")]
        public long ID { get; set; }

        [XmlElement("typeID")]
        public int ResearchSkillID { get; set; }
    }
}