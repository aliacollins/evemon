using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents a prerequisite skill for a blueprint.
    /// </summary>
    public sealed class SerializablePrereqSkill
    {
        [XmlAttribute("id")]
        public int ID { get; set; }

        [XmlAttribute("lv")]
        public int Level { get; set; }

        [XmlAttribute("activity")]
        public int Activity { get; set; }
    }
}