using System;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Exportation
{
    /// <summary>
    /// A serialization class designed for HTML exportation.
    /// </summary>
    public sealed class OutputSkill
    {
        [XmlAttribute("typeName")]
        public string Name { get; set; }

        [XmlElement("rank")]
        public Int64 Rank { get; set; }

        [XmlElement("level")]
        public Int64 Level { get; set; }

        [XmlElement("romanLevel")]
        public string RomanLevel { get; set; }

        [XmlElement("SP")]
        public string SkillPoints { get; set; }

        [XmlElement("maxSP")]
        public string MaxSkillPoints { get; set; }
    }
}