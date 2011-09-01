using System.Collections.Generic;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Exportation
{
    /// <summary>
    /// A serialization class designed for HTML exportation.
    /// </summary>
    public sealed class OutputSkillGroup
    {
        public OutputSkillGroup()
        {
            Skills = new List<OutputSkill>();
        }

        [XmlAttribute("groupName")]
        public string Name { get; set; }

        [XmlAttribute("skillsCount")]
        public int SkillsCount { get; set; }

        [XmlAttribute("totalSP")]
        public int TotalSP { get; set; }

        [XmlElement("skill")]
        public List<OutputSkill> Skills { get; set; }
    }
}