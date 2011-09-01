using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents a skill prerequisite for a skill
    /// </summary>
    public sealed class SerializableSkillPrerequisite
    {
        [XmlAttribute("id")]
        public int ID { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("level")]
        public int Level { get; set; }
    }
}