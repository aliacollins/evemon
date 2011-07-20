using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents a prerequisite skill for ships, items, implants.
    /// </summary>
    public sealed class SerializablePrerequisiteSkill
    {
        [XmlAttribute("id")]
        public int ID
        {
            get;
            set;
        }

        [XmlAttribute("lv")]
        public int Level
        {
            get;
            set;
        }
    }
}
