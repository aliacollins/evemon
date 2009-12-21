using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Globalization;

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
            get { return TimeUtil.ConvertDateTimeToCCPTimeString(m_curTime); }
            set
            {
                m_curTime = TimeUtil.ConvertCCPTimeStringToDateTime(value);
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
            get { return TimeUtil.ConvertDateTimeToCCPTimeString(m_cachedUntilTime); }
            set
            {
                m_cachedUntilTime = TimeUtil.ConvertCCPTimeStringToDateTime(value);
            }
        }

        public DateTime UpdatedAt
        {
            get { return m_curTime; }
        }

        public DateTime XMLExpires
        {
            get { return m_cachedUntilTime.ToUniversalTime(); }
            set { m_cachedUntilTime = value.ToUniversalTime(); }
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
        public string PortraitFolder
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
                if (m_cachedUntilTime > m_curTime)
                {
                    TimeSpan ts = m_cachedUntilTime - m_curTime;
                    return Convert.ToInt32(ts.TotalMilliseconds);
                }
                else
                {
                    return 0;
                }
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
                    scs = CreateFromXmlDocument(xdoc);
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

        /// <summary>
        /// Read a xmldoc from CCP and transforms it into a <see cref="SerializableCharacterSheet"/>
        /// </summary>
        /// <param name="xdoc"></param>
        /// <returns></returns>
        public static SerializableCharacterSheet CreateFromXmlDocument(XmlDocument xdoc)
        {
            SerializableCharacterSheet scs = new SerializableCharacterSheet();

            // Header
            var root = xdoc["eveapi"];
            scs.currentTime = root["currentTime"].InnerText;
            scs.CachedUntilTime = root["cachedUntil"].InnerText;

            // Result
            root = root["result"];
            var cs = scs.CharacterSheet;

            cs.Name = root["name"].InnerText;
            cs.Race = root["race"].InnerText;
            cs.Gender = root["gender"].InnerText;
            cs.BloodLine = root["bloodLine"].InnerText;
            cs.CorpName = root["corporationName"].InnerText;
            cs.CloneName = root["cloneName"].InnerText;

            cs.CloneSkillPoints = Int32.Parse(root["cloneSkillPoints"].InnerText);
            cs.CharacterId = Int32.Parse(root["characterID"].InnerText);
            cs.CorpId = root["corporationID"].InnerText;
            cs.Balance = Decimal.Parse(root["balance"].InnerText, CultureInfo.InvariantCulture.NumberFormat);

            // Implants
            var implantsRoot = root["attributeEnhancers"];
            var iRoot = implantsRoot["intelligenceBonus"];
            if (iRoot != null)
            {
                cs.AttributeBonuses.Bonuses.Add(new SerializableIntelligenceBonus { Name = iRoot["augmentatorName"].InnerText, Amount = Int32.Parse(iRoot["augmentatorValue"].InnerText) });
            }

            var pRoot = implantsRoot["perceptionBonus"];
            if (pRoot != null)
            {
                cs.AttributeBonuses.Bonuses.Add(new SerializablePerceptionBonus { Name = pRoot["augmentatorName"].InnerText, Amount = Int32.Parse(pRoot["augmentatorValue"].InnerText) });
            }

            var wRoot = implantsRoot["willpowerBonus"];
            if (wRoot != null)
            {
                cs.AttributeBonuses.Bonuses.Add(new SerializableWillpowerBonus { Name = wRoot["augmentatorName"].InnerText, Amount = Int32.Parse(wRoot["augmentatorValue"].InnerText) });
            }

            var mRoot = implantsRoot["memoryBonus"];
            if (mRoot != null)
            {
                cs.AttributeBonuses.Bonuses.Add(new SerializableMemoryBonus { Name = mRoot["augmentatorName"].InnerText, Amount = Int32.Parse(mRoot["augmentatorValue"].InnerText) });
            }

            var cRoot = implantsRoot["charismaBonus"];
            if (cRoot != null)
            {
                cs.AttributeBonuses.Bonuses.Add(new SerializableCharismaBonus { Name = cRoot["augmentatorName"].InnerText, Amount = Int32.Parse(cRoot["augmentatorValue"].InnerText) });
            }

            // Attributes
            var attributesRoot = root["attributes"];
            cs.Attributes.BaseIntelligence = Int32.Parse(attributesRoot["intelligence"].InnerText);
            cs.Attributes.BasePerception = Int32.Parse(attributesRoot["perception"].InnerText);
            cs.Attributes.BaseWillpower = Int32.Parse(attributesRoot["willpower"].InnerText);
            cs.Attributes.BaseCharisma = Int32.Parse(attributesRoot["charisma"].InnerText);
            cs.Attributes.BaseMemory = Int32.Parse(attributesRoot["memory"].InnerText);

            // Skills, certificates, etc
            foreach (XmlNode rowset in root.SelectNodes("rowset"))
            {
                var setname = rowset.Attributes["name"].InnerText;
                if (setname == "skills")
                {
                    foreach (XmlNode row in rowset.ChildNodes)
                    {
                        Int32 mySkillLevel = 0;
                        if ((row.Attributes["level"]!=null)){
                            mySkillLevel = Int32.Parse(row.Attributes["level"].InnerText);
                        }
                        cs.KnownSkillsSet.KnownSkills.Add(new SerializableKnownSkill
                        {
                            SkillId = Int32.Parse(row.Attributes["typeID"].InnerText),
                            Skillpoints = Int32.Parse(row.Attributes["skillpoints"].InnerText),
                            SkillLevel = mySkillLevel
                        });
                    }
                }
                else if (setname == "certificates")
                {
                    foreach (XmlNode row in rowset.ChildNodes)
                    {
                        cs.CertificatesID.Add(Int32.Parse(row.Attributes["certificateID"].InnerText));
                    }
                }
            }

            return scs;
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
        /// Creates an old style xml document from a new style one
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

        /// <summary>
        /// Saves character data as a text file
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void SaveTextFile(string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName, false))
            {
                MethodInvoker writeSep = new MethodInvoker(delegate
                {
                    sw.WriteLine("=======================================================================");
                });
                MethodInvoker writeSubSep = new MethodInvoker(delegate
                {
                    sw.WriteLine("-----------------------------------------------------------------------");
                });
                sw.WriteLine("BASIC INFO");
                writeSep();
                sw.WriteLine("     Name: {0}", this.CharacterSheet.Name);
                sw.WriteLine("   Gender: {0}", this.CharacterSheet.Gender);
                sw.WriteLine("     Race: {0}", this.CharacterSheet.Race);
                sw.WriteLine("Bloodline: {0}", this.CharacterSheet.BloodLine);
                sw.WriteLine("  Balance: {0} ISK", this.CharacterSheet.Balance.ToString("#,##0.00"));
                sw.WriteLine();
                sw.WriteLine("Intelligence: {0}", this.CharacterSheet.Attributes.AdjustedIntelligence.ToString("#0.00").PadLeft(5));
                sw.WriteLine("    Charisma: {0}", this.CharacterSheet.Attributes.AdjustedCharisma.ToString("#0.00").PadLeft(5));
                sw.WriteLine("  Perception: {0}", this.CharacterSheet.Attributes.AdjustedPerception.ToString("#0.00").PadLeft(5));
                sw.WriteLine("      Memory: {0}", this.CharacterSheet.Attributes.AdjustedMemory.ToString("#0.00").PadLeft(5));
                sw.WriteLine("   Willpower: {0}", this.CharacterSheet.Attributes.AdjustedWillpower.ToString("#0.00").PadLeft(5));
                sw.WriteLine();
                if (this.CharacterSheet.AttributeBonuses.Bonuses.Count > 0)
                {
                    sw.WriteLine("IMPLANTS");
                    writeSep();
                    foreach (SerializableEveAttributeBonus tb in this.CharacterSheet.AttributeBonuses.Bonuses)
                    {
                        sw.WriteLine("+{0} {1} : {2}", tb.Amount, tb.EveAttribute.ToString().PadRight(13), tb.Name);
                    }
                    sw.WriteLine();
                }
                sw.WriteLine("SKILLS");
                writeSep();
                foreach (SerializableSkillGroup sg in this.SkillGroups)
                {
                    sw.WriteLine("{0}, {1} Skill{2}, {3} Points",
                                 sg.Name, sg.Skills.Count, sg.Skills.Count > 1 ? "s" : "",
                                 sg.GetTotalPoints().ToString("#,##0"));
                    foreach (SerializableSkill s in sg.Skills)
                    {
                        StaticSkill ss = StaticSkill.GetStaticSkillById(s.Id);
                        string skillDesc = ss.Name + " " + Skill.GetRomanForInt(s.Level) + " (" + ss.Rank.ToString() + ")";
                        sw.WriteLine(": {0} {1}/{2} Points",
                                     skillDesc.PadRight(40), s.SkillPoints.ToString("#,##0"),
                                     ss.GetPointsRequiredForLevel(5).ToString("#,##0"));
                        if (this.TrainingSkillInfo != null && this.TrainingSkillInfo.TrainingSkillWithTypeID == s.Id)
                        {
                            DateTime adjustedEndTime = this.TrainingSkillInfo.getTrainingEndTime.ToLocalTime();
                            sw.WriteLine(":  (Currently training to level {0}, completes {1})",
                                         Skill.GetRomanForInt(this.TrainingSkillInfo.TrainingSkillToLevel),
                                         adjustedEndTime);
                        }
                    }
                    writeSubSep();
                }
            }
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
        
        private string m_cloneName;

        [XmlElement("cloneName")]
        public string CloneName
        {
            get { return m_cloneName; }
            set { m_cloneName = value; }
        }

        private int m_cloneSkillPoints;

        [XmlElement("cloneSkillPoints")]
        public int CloneSkillPoints
        {
            get { return m_cloneSkillPoints; }
            set { m_cloneSkillPoints = value; }
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

        private List<int> m_certificatesID = new List<int>();

        [XmlArray("certificatesID")]
        public List<int> CertificatesID
        {
            get { return m_certificatesID; }
            set { m_certificatesID = value; }
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
                if (skill == null)
                {
                    StaticDataErrorForm.ShowError(string.Format("The character info for {0} contains an unknown skill with TypeID {1}.", m_name, sks.SkillId));
                    continue;
                }
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

        /*
        private string m_rowsetID = "typeID"
;
        [XmlAttribute("key")]
        public string RowsetTypeID
        {
            get { return m_rowsetID; }
            set { m_rowsetID = value; ; }
        }*/

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
        public SerializableKnownSkill() { }

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
