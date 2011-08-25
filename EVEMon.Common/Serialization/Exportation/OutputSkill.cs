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
        public int Rank { get; set; }

        [XmlElement("level")]
        public int Level { get; set; }

        [XmlElement("romanLevel")]
        public string RomanLevel { get; set; }

        [XmlElement("SP")]
        public int SkillPoints { get; set; }

        [XmlElement("maxSP")]
        public int MaxSkillPoints { get; set; }
    }
}