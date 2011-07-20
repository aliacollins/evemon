using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents a skill in our datafile
    /// </summary>
    public sealed class SerializableSkill
    {
        [XmlAttribute("public")]
        public bool Public
        {
            get;
            set;
        }

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

        [XmlAttribute("description")]
        public string Description
        {
            get;
            set;
        }

        [XmlAttribute("primaryAttr")]
        public EveAttribute PrimaryAttribute
        {
            get;
            set;
        }

        [XmlAttribute("secondaryAttr")]
        public EveAttribute SecondaryAttribute
        {
            get;
            set;
        }

        [XmlAttribute("rank")]
        public int Rank
        {
            get;
            set;
        }

        [XmlAttribute("cost")]
        public long Cost
        {
            get;
            set;
        }

        [XmlAttribute("canTrainOnTrial")]
        public bool CanTrainOnTrial
        {
            get;
            set;
        }


        [XmlElement("prereq")]
        public SerializableSkillPrerequisite[] Prereqs
        {
            get;
            set;
        }
    }
}
