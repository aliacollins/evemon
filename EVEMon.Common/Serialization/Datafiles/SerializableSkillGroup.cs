using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents a skill group in our datafile
    /// </summary>
    public sealed class SerializableSkillGroup
    {
        [XmlAttribute("id")]
        public int ID
        {
            get;
            set;
        }

        [XmlAttribute("name")]
        public string Name
        {
            get;
            set;
        }

        [XmlElement("skill")]
        public SerializableSkill[] Skills
        {
            get;
            set;
        }
    }
}
