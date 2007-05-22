using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace EVEMon.Common
{
    [Flags]
    public enum SerializableEveAttributeAdjustment
    {
        Base = 1,
        Skills = 2,
        Implants = 4,
        AllWithoutLearning = 7,
        Learning = 8,
        AllWithLearning = 15
    }

    public abstract class SerializableEveAttributeBonus
    {
        private string m_name;
        private int m_amount;

        [XmlIgnore]
        public abstract EveAttribute EveAttribute { get; }

        [XmlElement("augmentatorName")]
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        [XmlElement("augmentatorValue")]
        public int Amount
        {
            get { return m_amount; }
            set { m_amount = value; }
        }

        private bool m_manual = false;

        [XmlAttribute]
        public bool Manual
        {
            get { return m_manual; }
            set { m_manual = value; }
        }
    }

    [XmlRoot("intelligenceBonus")]
    public class SerializableIntelligenceBonus : SerializableEveAttributeBonus
    {
        public override EveAttribute EveAttribute
        {
            get { return EveAttribute.Intelligence; }
        }
    }

    [XmlRoot("charismaBonus")]
    public class SerializableCharismaBonus : SerializableEveAttributeBonus
    {
        public override EveAttribute EveAttribute
        {
            get { return EveAttribute.Charisma; }
        }
    }

    [XmlRoot("perceptionBonus")]
    public class SerializablePerceptionBonus : SerializableEveAttributeBonus
    {
        public override EveAttribute EveAttribute
        {
            get { return EveAttribute.Perception; }
        }
    }

    [XmlRoot("memoryBonus")]
    public class SerializableMemoryBonus : SerializableEveAttributeBonus
    {
        public override EveAttribute EveAttribute
        {
            get { return EveAttribute.Memory; }
        }
    }

    [XmlRoot("willpowerBonus")]
    public class SerializableWillpowerBonus : SerializableEveAttributeBonus
    {
        public override EveAttribute EveAttribute
        {
            get { return EveAttribute.Willpower; }
        }
    }

    [XmlRoot("attributeEnhancers")]
    public class SerializableAttributeBonusCollection
    {
        private List<SerializableEveAttributeBonus> m_attributeBonuses = new List<SerializableEveAttributeBonus>();

        [XmlElement("intelligenceBonus", typeof (SerializableIntelligenceBonus))]
        [XmlElement("charismaBonus", typeof (SerializableCharismaBonus))]
        [XmlElement("perceptionBonus", typeof (SerializablePerceptionBonus))]
        [XmlElement("memoryBonus", typeof (SerializableMemoryBonus))]
        [XmlElement("willpowerBonus", typeof (SerializableWillpowerBonus))]
        public List<SerializableEveAttributeBonus> Bonuses
        {
            get { return m_attributeBonuses; }
            set { m_attributeBonuses = value; }
        }
    }

    [XmlRoot("character")]
    public class SerializableCharacterInfo
    {
        public SerializableCharacterInfo()
        {
            m_attributes = new EveAttributes();
            m_attributes.SetOwner(this);
        }

        private int m_timeLeftInCache = -1;

        [XmlIgnore]
        public int TimeLeftInCache
        {
            get { return m_timeLeftInCache; }
            set { m_timeLeftInCache = value; }
        }

        private DateTime m_expires = DateTime.MinValue;

        [XmlAttribute("expires")]
        public DateTime XMLExpires
        {
            get { return m_expires; }
            set { m_expires = value; }
        }

        private string m_name = String.Empty;

        [XmlAttribute("name")]
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        private int m_characterId;

        [XmlAttribute("characterID")]
        public int CharacterId
        {
            get { return m_characterId; }
            set { m_characterId = value; }
        }

        private bool m_isCached = false;

        [XmlAttribute("fromCache")]
        public bool IsCached
        {
            get { return m_isCached; }
            set { m_isCached = value; }
        }

        private SerializableSkillTrainingInfo m_TrainingSkillInfo;

        [XmlElement("skillTraining", typeof (SerializableSkillTrainingInfo))]
        public SerializableSkillTrainingInfo TrainingSkillInfo
        {
            get { return m_TrainingSkillInfo; }
            set { m_TrainingSkillInfo = value; }
        }

        private string m_race = String.Empty;

        [XmlElement("race")]
        public string Race
        {
            get { return m_race; }
            set { m_race = value; }
        }

        private string m_bloodLine = String.Empty;

        [XmlElement("bloodLine")]
        public string BloodLine
        {
            get { return m_bloodLine; }
            set { m_bloodLine = value; }
        }

        private string m_gender = String.Empty;

        [XmlElement("gender")]
        public string Gender
        {
            get { return m_gender; }
            set { m_gender = value; }
        }

        private string m_corpName = String.Empty;

        [XmlElement("corporationName")]
        public string CorpName
        {
            get { return m_corpName; }
            set { m_corpName = value; }
        }

        private string m_EVEFolder = String.Empty;

        [XmlElement("EVEFolder")]
        public string EVEFolder
        {
            get { return m_EVEFolder; }
            set { m_EVEFolder = value; }
        }

        private Decimal m_balance;

        [XmlElement("balance")]
        public Decimal Balance
        {
            get { return m_balance; }
            set { m_balance = value; }
        }

        private List<SerializableImplantSet> m_implantSets = new List<SerializableImplantSet>();

        [XmlArrayItem("ImplantSet")]
        public List<SerializableImplantSet> ImplantSets
        {
            get { return m_implantSets; }
            set { m_implantSets = value; }
        }

        private EveAttributes m_attributes;

        [XmlElement("attributes")]
        public EveAttributes Attributes
        {
            get { return m_attributes; }
            set
            {
                m_attributes = value;
                if (value != null)
                {
                    value.SetOwner(this);
                }
            }
        }

        private SerializableAttributeBonusCollection m_attributeBonuses = new SerializableAttributeBonusCollection();

        [XmlElement("attributeEnhancers")]
        public SerializableAttributeBonusCollection AttributeBonuses
        {
            get { return m_attributeBonuses; }
            set { m_attributeBonuses = value; }
        }

        [XmlIgnore]
        [Obsolete]
        public int Intelligence
        {
            get { return m_attributes.BaseIntelligence; }
            set { m_attributes.BaseIntelligence = value; }
        }

        [XmlIgnore]
        [Obsolete]
        public int Charisma
        {
            get { return m_attributes.BaseCharisma; }
            set { m_attributes.BaseCharisma = value; }
        }

        [XmlIgnore]
        [Obsolete]
        public int Perception
        {
            get { return m_attributes.BasePerception; }
            set { m_attributes.BasePerception = value; }
        }

        [XmlIgnore]
        [Obsolete]
        public int Memory
        {
            get { return m_attributes.BaseMemory; }
            set { m_attributes.BaseMemory = value; }
        }

        [XmlIgnore]
        [Obsolete]
        public int Willpower
        {
            get { return m_attributes.BaseWillpower; }
            set { m_attributes.BaseWillpower = value; }
        }

        private List<SerializableSkillGroup> m_skillGroups = new List<SerializableSkillGroup>();

        [XmlArray("skills")]
        [XmlArrayItem("skillGroup")]
        public List<SerializableSkillGroup> SkillGroups
        {
            get { return m_skillGroups; }
            set { m_skillGroups = value; }
        }

        public SerializableSkill GetSkill(string skillName)
        {
            foreach (SerializableSkillGroup sg in m_skillGroups)
            {
                foreach (SerializableSkill s in sg.Skills)
                {
                    if (s.Name == skillName)
                    {
                        return s;
                    }
                }
            }
            return null;
        }

        //anders - new function
        public static XmlElement FindCharacterElement(XmlDocument xdoc)
        {
            //evemon file
            if (xdoc.DocumentElement.Name == "character")
            {
                return xdoc.DocumentElement;
            }
                //eve-o file
            else if (xdoc.DocumentElement.Name == "charactersheet")
            {
                return (XmlElement) xdoc.SelectSingleNode("charactersheet/characters/character[count(child::*)>0]");
            }
            return null;
        }

        public static SerializableCharacterInfo CreateFromFile(string fileName)
        {
            try
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.Load(fileName);
                XmlElement charRootEl = FindCharacterElement(xdoc);

                if (charRootEl == null)
                {
                    return null;
                }

                SerializableCharacterInfo sci = null;
                using (XmlNodeReader nxr = new XmlNodeReader(charRootEl))
                {
                    XmlSerializer xs = new XmlSerializer(typeof (SerializableCharacterInfo));

                    sci = (SerializableCharacterInfo) xs.Deserialize(nxr);
                
                    // recover implant sets from the character cache if present
                    List<SerializableCharacterInfo> cciList = Settings.GetInstance().CachedCharacterInfo;
                    if (cciList != null && cciList.Count > 0)
                    {
                        foreach (SerializableCharacterInfo csci in  cciList)
                        {
                            if (csci.CharacterId == sci.CharacterId)
                            {
                                sci.ImplantSets = csci.ImplantSets;
                                break;
                            }
                        }
                    }
                    foreach (SerializableSkillGroup sg in sci.SkillGroups)
                    {
                        foreach (SerializableSkill s in sg.Skills)
                        {
                            s.LastConfirmedLevel = s.Level;
                        }
                    }
                    return sci;
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e, true);
                return null;
            }
        }
    }

    [XmlRoot("skillGroup")]
    public class SerializableSkillGroup
    {
        private string m_name = String.Empty;
        private int m_id;
        private List<SerializableSkill> m_skills = new List<SerializableSkill>();

        [XmlAttribute("groupName")]
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        [XmlAttribute("groupID")]
        public int Id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        [XmlElement("skill")]
        public List<SerializableSkill> Skills
        {
            get { return m_skills; }
            set { m_skills = value; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(m_name);
            sb.Append(" - ");
            sb.Append(m_skills.Count);
            sb.Append(" Skill");
            if (m_skills.Count > 1)
            {
                sb.Append("s");
            }
            sb.Append(", ");
            int points = 0;
            foreach (SerializableSkill s in m_skills)
            {
                points += s.SkillPoints;
            }
            sb.Append(points.ToString("#,##0"));
            sb.Append(" points");
            return sb.ToString();
        }

        public int GetTotalPoints()
        {
            int result = 0;
            foreach (SerializableSkill s in m_skills)
            {
                result += s.SkillPoints;
            }
            return result;
        }
    }

    [XmlRoot("skill")]
    public class SerializableSkill
    {
        private string m_name = String.Empty;
        private int m_id;
        private int m_groupId;
        private int m_flag;
        private int m_rank;
        private int m_skillPoints;
        private int m_level;
        private int m_lastConfirmedLvl;

        [XmlAttribute("typeName")]
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        [XmlAttribute("typeID")]
        public int Id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        [XmlElement("groupID")]
        public int GroupId
        {
            get { return m_groupId; }
            set { m_groupId = value; }
        }

        [XmlElement("flag")]
        public int Flag
        {
            get { return m_flag; }
            set { m_flag = value; }
        }

        [XmlElement("rank")]
        public int Rank
        {
            get { return m_rank; }
            set
            {
                m_rank = value;
                for (int i = 0; i < 5; i++)
                {
                    if (m_skillLevel[i] == 0)
                    {
                        m_skillLevel[i] = GetSkillPointsForLevel(m_rank, i + 1);
                    }
                }
            }
        }

        public static int GetSkillPointsForLevel(int rank, int level)
        {
            int pointsForLevel = Convert.ToInt32(250*rank*Math.Pow(32, Convert.ToDouble(level - 1)/2));
            // There's some sort of weird rounding error
            // these values need to be corrected by one.
            if (pointsForLevel == 1414)
            {
                pointsForLevel = 1415;
            }
            else if (pointsForLevel == 2828)
            {
                pointsForLevel = 2829;
            }
            else if (pointsForLevel == 7071)
            {
                pointsForLevel = 7072;
            }
            else if (pointsForLevel == 181019)
            {
                pointsForLevel = 181020;
            }
            else if (pointsForLevel == 226274)
            {
                pointsForLevel = 226275;
            }
            return pointsForLevel;
        }

        [XmlElement("skillpoints")]
        public int SkillPoints
        {
            get { return m_skillPoints; }
            set { m_skillPoints = value; }
        }

        [XmlElement("level")]
        public int Level
        {
            get { return m_level; }
            set { m_level = value; }
        }

        [XmlElement("lastconfirmedlevel")]
        public int LastConfirmedLevel
        {
            get { return m_lastConfirmedLvl; }
            set { m_lastConfirmedLvl = value; }
        }

        private int[] m_skillLevel = new int[5] {0, 0, 0, 0, 0};

        [XmlElement("skilllevel1")]
        public int SkillLevel1
        {
            get { return m_skillLevel[0]; }
            set { m_skillLevel[0] = value; }
        }

        [XmlElement("skilllevel2")]
        public int SkillLevel2
        {
            get { return m_skillLevel[1]; }
            set { m_skillLevel[1] = value; }
        }

        [XmlElement("skilllevel3")]
        public int SkillLevel3
        {
            get { return m_skillLevel[2]; }
            set { m_skillLevel[2] = value; }
        }

        [XmlElement("skilllevel4")]
        public int SkillLevel4
        {
            get { return m_skillLevel[3]; }
            set { m_skillLevel[3] = value; }
        }

        [XmlElement("skilllevel5")]
        public int SkillLevel5
        {
            get { return m_skillLevel[4]; }
            set { m_skillLevel[4] = value; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\t");
            sb.Append(m_name);
            sb.Append(" ");
            sb.Append(Skill.GetRomanForInt(m_level));
            sb.Append(" (Rank ");
            sb.Append(m_rank.ToString());
            sb.Append(") ");
            sb.Append(m_skillPoints.ToString("#,##0"));
            sb.Append(" points");
            return sb.ToString();
        }
    }

    [XmlRoot("skillInTraining")]
    [Obsolete]
    public class SerializableSkillInTraining
    {
        private string m_skillName = String.Empty;

        public string SkillName
        {
            get { return m_skillName; }
            set { m_skillName = value; }
        }

        private int m_trainingToLevel;

        public int TrainingToLevel
        {
            get { return m_trainingToLevel; }
            set { m_trainingToLevel = value; }
        }

        private DateTime m_estimatedCompletion;

        public DateTime EstimatedCompletion
        {
            get { return m_estimatedCompletion; }
            set { m_estimatedCompletion = value; }
        }

        private int m_currentPoints;

        public int CurrentPoints
        {
            get { return m_currentPoints; }
            set { m_currentPoints = value; }
        }

        private int m_neededPoints;

        public int NeededPoints
        {
            get { return m_neededPoints; }
            set { m_neededPoints = value; }
        }
    }

    [XmlRoot("ImplantSet")]
    public class SerializableImplantSet
    {
        private int m_number;

        [XmlElement("Number")]
        public int Number
        {
            get { return m_number; }
            set { m_number = value; }
        }

        private UserImplant[] m_values;

        public SerializableImplantSet()
        {
            m_values = new UserImplant[10];
        }

        public SerializableImplantSet(UserImplant[] a)
        {
            m_values = a;
        }

        [XmlIgnore]
        public UserImplant this[int name]
        {
            get { return m_values[name]; }
            set { m_values[name] = value; }
        }

        [XmlArray]
        [XmlArrayItem("UserImplant")]
        public UserImplant[] Implants
        {
            get { return m_values; }
            set { m_values = value; }
        }
    }
}
