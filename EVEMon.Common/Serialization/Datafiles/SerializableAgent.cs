using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents an eve agent.
    /// </summary>
    public sealed class SerializableAgent
    {
        [XmlAttribute("agentID")]
        public int ID
        {
            get;
            set;
        }

        [XmlAttribute("agentName")]
        public string Name
        {
            get;
            set;
        }

        [XmlAttribute("divisionName")]
        public string DivisionName
        {
            get;
            set;
        }

        [XmlAttribute("level")]
        public int Level
        {
            get;
            set;
        }

        [XmlAttribute("quality")]
        public int Quality
        {
            get;
            set;
        }

        [XmlAttribute("agentType")]
        public string AgentType
        {
            get;
            set;
        }

        [XmlAttribute("researchSkillID")]
        public int ResearchSkillID
        {
            get;
            set;
        }

        [XmlAttribute("locatorService")]
        public bool LocatorService
        {
            get;
            set;
        }
    }
}
