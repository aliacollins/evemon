using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace EVEMon.Common
{
    [XmlRoot("eveapi")]
    public class SerializableCharacterSheet
    {
        public SerializableCharacterSheet()
        {
            // make sure we have the static skill data loaded!
            StaticSkill.LoadStaticSkills();
        }
        public SerializableCharacterSheet(SerializableCharacterInfo sci)
        {
           CopyFromSerializableCharacterInfo(sci);
        }

        #region CCP xml stuff
        private DateTime m_curTime = DateTime.MinValue;
        [XmlElement("currentTime")]
        public string currentTime
        {
            get { return SerializableSkillTrainingInfo.ConvertDateTimeToTimeString(m_curTime); }
            set
            {
                m_curTime = SerializableSkillTrainingInfo.ConvertTimeStringToDateTime(value);
            }

        }

        private CharacterSheetResult m_charSheet = new CharacterSheetResult();

        [XmlElement("result")]
        public CharacterSheetResult CharacterSheet
        {
            get { return m_charSheet; }
            set { m_charSheet = value; }
        }

        private DateTime m_cachedUntilTime = DateTime.MinValue;
        [XmlElement("cachedUntil")]
        public string CachedUntilTime
        {
            get { return SerializableSkillTrainingInfo.ConvertDateTimeToTimeString(m_cachedUntilTime); }
            set
            {
                m_cachedUntilTime = SerializableSkillTrainingInfo.ConvertTimeStringToDateTime(value);
            }
        }

        public DateTime XMLExpires
        {
            get { return m_cachedUntilTime; }
            set { m_cachedUntilTime = value; }
        }

        #endregion


        #region EVEMon stuff

        private bool m_fromCCP = true;
        [XmlIgnore]
        public bool FromCCP
        {
            // Set to indicate that this was downloaded from CCP server
            get { return m_fromCCP; }
            set { m_fromCCP = value; }
        }


        private SerializableSkillTrainingInfo m_TrainingSkillInfo;
        [XmlElement("skillTraining", typeof(SerializableSkillTrainingInfo))]
        public SerializableSkillTrainingInfo TrainingSkillInfo
        {
            get { return m_TrainingSkillInfo; }
            set { m_TrainingSkillInfo = value; }
        }

        private string m_EVEFolder = String.Empty;

        [XmlElement("EVEFolder")]
        public string EVEFolder
        {
            get { return m_EVEFolder; }
            set { m_EVEFolder = value; }
        }


        private List<SerializableImplantSet> m_implantSets = new List<SerializableImplantSet>();

        [XmlArrayItem("ImplantSet")]
        public List<SerializableImplantSet> ImplantSets
        {
            get { return m_implantSets; }
            set { m_implantSets = value; }
        }

        [XmlIgnore]
        public List<SerializableSkillGroup> SkillGroups
        {
            get { return m_charSheet.SkillGroups; }
        }

        #endregion

        [XmlIgnore]
        public int TimeLeftInCache
        {
            get 
            { 
                TimeSpan ts = m_cachedUntilTime - m_curTime;
                return Convert.ToInt32(ts.TotalMilliseconds);
            }
        }

        public SerializableSkill GetSkill(string skillName)
        {
            foreach (SerializableSkillGroup sg in SkillGroups)
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

        /// <summary>
        /// Called by CharacterMonitor.ReloadFromFile() when a monitored file is changed
        /// Called by CharFileInfo.CharacterName when name is null (startup?)
        /// Called by MainWindow.AddTab for a file character
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static SerializableCharacterSheet CreateFromFile(string fileName)
        {
            SerializableCharacterSheet scs = null;
            try
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.Load(fileName);
                XmlElement charRootEl = SerializableCharacterInfo.FindCharacterElement(xdoc);

                if (charRootEl == null)
                {
                    // New style API xml
                    using (XmlNodeReader nxr = new XmlNodeReader(xdoc))
                    {
                        XmlSerializer xs = new XmlSerializer(typeof(SerializableCharacterSheet));
                        scs = (SerializableCharacterSheet)xs.Deserialize(nxr);
                    }
                }
                else
                {
                    // old style API xml
                    SerializableCharacterInfo sci = null;
                    using (XmlNodeReader nxr = new XmlNodeReader(charRootEl))
                    {
                        XmlSerializer xs = new XmlSerializer(typeof(SerializableCharacterInfo));
                        sci = (SerializableCharacterInfo)xs.Deserialize(nxr);
                    }
                    scs = new SerializableCharacterSheet(sci);

                }

                // recover implant sets from the character cache if present
            //        List<SerializableCharacterInfo> cciList = Settings.GetInstance().CachedCharacterInfo;
            //        if (cciList != null && cciList.Count > 0)
            //        {
            //            foreach (SerializableCharacterInfo csci in cciList)
            //            {
            //                if (csci.CharacterId == sci.CharacterId)
            //                {
            //                    sci.ImplantSets = csci.ImplantSets;
            //                    break;
            //                }
            //            }
            //        }
                foreach (SerializableSkillGroup sg in scs.SkillGroups)
                {
                    foreach (SerializableSkill s in sg.Skills)
                    {
                        s.LastConfirmedLevel = s.Level;
                    }
                }
                return scs;
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e, true);
                return null;
            } 
        }

        private void CopyFromSerializableCharacterInfo(SerializableCharacterInfo sci)
        {
            m_curTime = DateTime.Now.ToUniversalTime();
            m_fromCCP = false;
            m_charSheet.AttributeBonuses = sci.AttributeBonuses;
            m_charSheet.Attributes = sci.Attributes;
            m_charSheet.Balance = sci.Balance;
            m_charSheet.BloodLine = sci.BloodLine;
            m_charSheet.CharacterId = sci.CharacterId;
            m_charSheet.CorpName = sci.CorpName;
            m_charSheet.Gender = sci.Gender;
            m_charSheet.Name = sci.Name;
            m_charSheet.Race = sci.Race;

            foreach (SerializableSkillGroup sg in sci.SkillGroups)
            {
                foreach (SerializableSkill ss in sg.Skills)
                {
                    SerializableKnownSkill sk = new SerializableKnownSkill();
                    sk.SkillId = ss.Id;
                    sk.SkillLevel = ss.Level;
                    sk.Skillpoints = ss.SkillPoints;
                    sk.LastConfirmedLevel = ss.LastConfirmedLevel;
                    m_charSheet.KnownSkillsSet.KnownSkills.Add(sk);
                }
            }
            m_charSheet.CreateSkillGroups();
        }

        /// <summary>
        /// Creates an old style xml docu,ment from a new style one
        /// This is needed to export files in the old style xml, and also to ctrate html saves
        /// </summary>
        /// <param name="sci"></param>
        /// <returns></returns>
        public  SerializableCharacterInfo CreateSerializableCharacterInfo()
        {
            SerializableCharacterInfo sci = new SerializableCharacterInfo();
            sci.AttributeBonuses = m_charSheet.AttributeBonuses;
            sci.Attributes = m_charSheet.Attributes;
            sci.Balance = m_charSheet.Balance;
            sci.BloodLine = m_charSheet.BloodLine;
            sci.CharacterId = m_charSheet.CharacterId;
            sci.CorpName = m_charSheet.CorpName;
            sci.Gender = m_charSheet.Gender;
            sci.Name = m_charSheet.Name;
            sci.Race = m_charSheet.Race;
            sci.SkillGroups = m_charSheet.SkillGroups;
            return sci;
        }
    }

    [XmlRoot("result")]
    public class CharacterSheetResult
    {
        public CharacterSheetResult()
        {
            m_attributes = new EveAttributes();
            // owner is only needed for the save as text file feature.
            m_attributes.SetOwner(this);
        }

        #region CCP data
        private int m_characterId;

        [XmlElement("characterID")]
        public int CharacterId
        {
            get { return m_characterId; }
            set { m_characterId = value; }
        }

        private string m_name = String.Empty;

        [XmlElement("name")]
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
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

        private string m_corpId = String.Empty;

        [XmlElement("corporationID")]
        public string CorpId
        {
            get { return m_corpId; }
            set { m_corpId = value; }
        }
        private Decimal m_balance;

        [XmlElement("balance")]
        public Decimal Balance
        {
            get { return m_balance; }
            set { m_balance = value; }
        }

        private SerializableAttributeBonusCollection m_attributeBonuses = new SerializableAttributeBonusCollection();
        [XmlElement("attributeEnhancers")]
        public SerializableAttributeBonusCollection AttributeBonuses
        {
            get { return m_attributeBonuses; }
            set { m_attributeBonuses = value; }
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

        private KnownSkillSet m_knownSkills = new KnownSkillSet();
        [XmlElement("rowset")]
        public KnownSkillSet KnownSkillsSet
        {
            get { return m_knownSkills; }
            set { m_knownSkills = value; }
        }

#endregion

        #region EVEMon Data
        // Added by EVEMon

        private List<SerializableSkillGroup> m_skillGroups = new List<SerializableSkillGroup>();

        [XmlIgnore]
        public List<SerializableSkillGroup> SkillGroups
        {
            get 
            {
                if (m_skillGroups.Count == 0)
                {
                    CreateSkillGroups();
                }
                return m_skillGroups; 
            }
         }

        public void CreateSkillGroups()
        {
            foreach (SerializableKnownSkill sks in m_knownSkills.KnownSkills)
            {
                // now get the skill and Skill group
                StaticSkill skill = StaticSkill.GetStaticSkillById(sks.SkillId);
                StaticSkillGroup skillGroup = skill.SkillGroup;
                SerializableSkillGroup ssg = null;

                foreach (SerializableSkillGroup group in m_skillGroups)
                {
                    if (group.Id == skillGroup.ID)
                    {
                        ssg = group;
                        break;
                    }
                }

                if (ssg == null)
                {
                    // don't know this one...
                    ssg = new SerializableSkillGroup();
                    ssg.Id = skillGroup.ID;
                    ssg.Name = skillGroup.Name;
                    ssg.Skills = new List<SerializableSkill>();
                    m_skillGroups.Insert(m_skillGroups.Count,ssg);
                }

                // now add the skill
                SerializableSkill serSkill = new SerializableSkill();
                serSkill.Id = skill.Id;
                serSkill.LastConfirmedLevel = sks.SkillLevel;
                serSkill.Level = sks.SkillLevel;
                serSkill.SkillPoints = sks.Skillpoints;
      //          serSkill.Name = StaticSkill.GetStaticSkillById(skill.Id).Name;
                ssg.Skills.Add(serSkill);
            }
        }


        #endregion
    }

    [XmlRoot("rowset")]
    public class KnownSkillSet
    {
        private string m_rowsetName = "skills";
        [XmlAttribute("name")]
        public string RowsetName
        {
            get { return m_rowsetName; }
            set { m_rowsetName = "skills"; }
        }

        private string m_rowsetID = "typeID"
;
        [XmlAttribute("typeID")]
        public string RowsetTypeID
        {
            get { return m_rowsetID; }
            set { m_rowsetID = value; ; }
        }

        

        private List<SerializableKnownSkill> m_knownSkills = new List<SerializableKnownSkill>();
        [XmlElement("row")]
        public List<SerializableKnownSkill> KnownSkills
        {
            get { return m_knownSkills; }
            set { m_knownSkills = value; }
        }

    }

    [XmlRoot("row")]
    public class SerializableKnownSkill
    {
        public SerializableKnownSkill()
        {

        }

        public SerializableKnownSkill(Skill s)
        {
            m_skillId = s.Id;
            m_skillpoints = s.CurrentSkillPoints;
            m_level = s.Level;
            m_lastLevel = s.LastConfirmedLvl;
        }

        private int m_skillId;
        [XmlAttribute("typeID")]
        public int SkillId
        {
            get { return m_skillId; }
            set { m_skillId = value; }
        }

        private int m_skillpoints;
        [XmlAttribute("skillpoints")]
        public int Skillpoints
        {
            get { return m_skillpoints; }
            set { m_skillpoints = value; }
        }

        private int m_level;
        [XmlAttribute("level")]
        public int SkillLevel
        {
            get { return m_level; }
            set { m_level = value; }
        }

        private int m_lastLevel;
        [XmlAttribute("lastconfirmedlevel")]
        public int LastConfirmedLevel
        {
            get { return m_lastLevel; }
            set { m_lastLevel = value; }
        }


    }

}
