using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace EVEMon.Common
{
    public class CharacterInfo
    {
        private string m_name;
        private int m_characterId;
        private string m_race;
        private string m_bloodLine;
        private string m_gender;
        private string m_corporationName;
        private string m_EVEFolder;
        private Decimal m_balance;
        private GrandEveAttributes m_attributes = new GrandEveAttributes();
               
        // These are the new replacements for m_attributeBonuses
        private MonitoredList<UserImplant> m_CurrentImplants = new MonitoredList<UserImplant>();
        private Dictionary<string, ImplantSet> m_implantSets = new Dictionary<string, ImplantSet>();

        // This is to keep track of the number of times you've tried to dl this character and been unsuccessful.
        private int m_downloadfailed = 0;

        private Dictionary<string, SkillGroup> m_skillGroups = new Dictionary<string, SkillGroup>();
        private Dictionary<int, Skill> m_AllSkillsByID = new Dictionary<int, Skill>();
        private Dictionary<string, Skill> m_AllSkillsByName = new Dictionary<string, Skill>();

        public CharacterInfo(int characterId, string name)
        {
            m_characterId = characterId;
            m_name = name;

            BuildSkillTree();
            m_CurrentImplants.Changed +=
                new EventHandler<ChangedEventArgs<UserImplant>>(m_implantSet_Changed);
        }

        private void BuildSkillTree()
        {
            bool bookOwned = false;
            StaticSkill.LoadStaticSkills();


            // Get owned books before we add SkillChange handlers
            List<string> ownedBooks = new List<string>();
            foreach (string os in Settings.GetInstance().GetOwnedBooksForCharacter(m_name))
            {
                ownedBooks.Add(os);
            }

            foreach (StaticSkillGroup sgs in StaticSkillGroup.AllStaticGroups)
            {
                List<Skill> skills = new List<Skill>();
                foreach (StaticSkill ss in sgs)
                {
                    List<Skill.Prereq> prereqs = new List<Skill.Prereq>();
                    foreach (StaticSkill.Prereq ssp in ss.Prereqs)
                    {
                        Skill.Prereq pr = new Skill.Prereq(ssp);
                        prereqs.Add(pr);
                    }
                    bookOwned = ownedBooks.Contains(ss.Name);
                    Skill cs = new Skill(this, ss, bookOwned, prereqs);
                    cs.Changed += new EventHandler(gs_Changed);
                    cs.TrainingStatusChanged += new EventHandler(gs_TrainingStatusChanged);
                    m_AllSkillsByID[ss.Id] = cs;
                    m_AllSkillsByName[ss.Name] = cs;
                    skills.Add(cs);
                }
                SkillGroup gsg = new SkillGroup(sgs, skills);
                m_skillGroups[gsg.Name] = gsg;
            }

            foreach (Skill s in m_AllSkillsByID.Values)
            {
                foreach (Skill.Prereq pr in s.Prereqs)
                {
                    Skill gs = m_AllSkillsByName[pr.Name];
                    pr.SetSkill(gs);
                }
            }

            // cleanup on aisle 3!
            AutoShrink.Dirty();
        }
        
        private int m_suppressed;
        private Queue<InternalEvent> m_events = new Queue<InternalEvent>();
        private Dictionary<string, bool> m_coalescedEventTable = new Dictionary<string, bool>();

        #region custom event handling - to allow events to be suppressed and queued
        private delegate void InternalEvent();

        private void FireEvent(InternalEvent evt, string coalesceKey)
        {
            lock (m_events)
            {
                if (m_suppressed == 0)
                {
                    // we're not suppressing events so kick it off.
                    evt();
                }
                else
                {
                    // We're supressing events
                    // have we already got a queued event of this key?
                    if (String.IsNullOrEmpty(coalesceKey) || !m_coalescedEventTable.ContainsKey(coalesceKey))
                    {
                        // no, add it to the pending queue
                        m_events.Enqueue(evt);
                        if (!String.IsNullOrEmpty(coalesceKey))
                        {
                            // If we have a key, flag that we've queued an event for this key.
                            m_coalescedEventTable[coalesceKey] = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Switch off event firing - cvan be callled cumulativly.
        /// A supression counter is incremented every time this is invoked.
        /// </summary>
        public void SuppressEvents()
        {
            lock (m_events)
            {
                m_suppressed++;
            }
        }

        /// <summary>
        /// Resume events. Will fire any queued events when the supression counter is 0
        /// </summary>
        public void ResumeEvents()
        {
            lock (m_events)
            {
                m_suppressed--;
                if (m_suppressed <= 0)
                {
                    m_suppressed = 0;
                    // fire any queued events
                    while (m_events.Count > 0)
                    {
                        // dequeue and fire.
                        m_events.Dequeue()();
                    }
                    // and empty the cache of event type keys
                    m_coalescedEventTable.Clear();
                }
            }
        }

        #endregion

        /// <summary>
        /// Called when skill changes sp, changes known status or starts/stops training
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gs_Changed(object sender, EventArgs e)
        {
            Skill gs = (Skill)sender;
            if (gs == m_currentlyTrainingSkill && gs.InTraining == false)
            {
                m_currentlyTrainingSkill = null;
            }
            else if (gs.InTraining)
            {
                m_currentlyTrainingSkill = gs;
            }

            OnSkillChanged(gs);
        }

        public string Name
        {
            get { return m_name; }
            set
            {
                if (m_name != value)
                {
                    m_name = value;
                    OnBioInfoChanged();
                }
            }
        }

        public Dictionary<string,ImplantSet> implantSets
        {
            get { return m_implantSets; }
            set 
            {
                if (m_implantSets != value)
                {
                    m_implantSets = value;
                    OnBioInfoChanged();
                }
            }
        }

        public int CharacterId
        {
            get { return m_characterId; }
            set
            {
                if (m_characterId != value)
                {
                    m_characterId = value;
                    OnBioInfoChanged();
                }
            }
        }

        public string Race
        {
            get { return m_race; }
            set
            {
                if (m_race != value)
                {
                    m_race = value;
                    OnBioInfoChanged();
                }
            }
        }

        public string Bloodline
        {
            get { return m_bloodLine; }
            set
            {
                if (m_bloodLine != value)
                {
                    m_bloodLine = value;
                    OnBioInfoChanged();
                }
            }
        }

        public string Gender
        {
            get { return m_gender; }
            set
            {
                if (m_gender != value)
                {
                    m_gender = value;
                    OnBioInfoChanged();
                }
            }
        }

        public string CorporationName
        {
            get { return m_corporationName; }
            set
            {
                if (m_corporationName != value)
                {
                    m_corporationName = value;
                    OnBioInfoChanged();
                }
            }
        }

        private void OnBioInfoChanged()
        {
            FireEvent(delegate
                          {
                              if (BioInfoChanged != null)
                              {
                                  BioInfoChanged(this, new EventArgs());
                              }
                          }, "bioinfo");
        }

        public event EventHandler BioInfoChanged;

        public string EVEFolder
        {
            get { return m_EVEFolder; }
            set
            {
                if (m_EVEFolder != value)
                {
                    m_EVEFolder = value;
                }
            }
        }

        public Decimal Balance
        {
            get { return m_balance; }
            set
            {
                m_balance = value;
                OnBalanceChanged();
            }
        }

        private DateTime m_xmlExpires = DateTime.MinValue;
        public DateTime XMLExpires
        {
            get { return m_xmlExpires; }
            set { m_xmlExpires = value; }
        }

        private void OnBalanceChanged()
        {
            FireEvent(delegate
                          {
                              if (BalanceChanged != null)
                              {
                                  BalanceChanged(this, new EventArgs());
                              }
                          }, "balance");
        }

        public event EventHandler BalanceChanged;

        public int GetBaseAttribute(EveAttribute attribute)
        {
            return m_attributes[attribute];
        }

        public void SetBaseAttribute(EveAttribute attribute, int value)
        {
            if (m_attributes[attribute] != value)
            {
                m_attributes[attribute] = value;
                OnAttributeChanged();
            }
        }

        private void OnAttributeChanged()
        {
            FireEvent(delegate
                          {
                              if (AttributeChanged != null)
                              {
                                  AttributeChanged(this, new EventArgs());
                              }
                          }, "attribute");
        }

        public event EventHandler AttributeChanged;

        public int BaseIntelligence
        {
            get { return GetBaseAttribute(EveAttribute.Intelligence); }
            set { SetBaseAttribute(EveAttribute.Intelligence, value); }
        }

        public int BaseCharisma
        {
            get { return GetBaseAttribute(EveAttribute.Charisma); }
            set { SetBaseAttribute(EveAttribute.Charisma, value); }
        }

        public int BasePerception
        {
            get { return GetBaseAttribute(EveAttribute.Perception); }
            set { SetBaseAttribute(EveAttribute.Perception, value); }
        }

        public int BaseMemory
        {
            get { return GetBaseAttribute(EveAttribute.Memory); }
            set { SetBaseAttribute(EveAttribute.Memory, value); }
        }

        public int BaseWillpower
        {
            get { return GetBaseAttribute(EveAttribute.Willpower); }
            set { SetBaseAttribute(EveAttribute.Willpower, value); }
        }

        public double GetEffectiveAttribute(EveAttribute attribute)
        {
            return GetEffectiveAttribute(attribute, null);
        }

        public double GetEffectiveAttribute(EveAttribute attribute, EveAttributeScratchpad scratchpad)
        {
            return GetEffectiveAttribute(attribute, scratchpad, true, true);
        }

        public double GetEffectiveAttribute(EveAttribute attribute, EveAttributeScratchpad scratchpad,
                                            bool includeLearning, bool includeImplants)
        {
            double result = Convert.ToDouble(m_attributes[attribute]);
            if (includeImplants)
            {
                result += getImplantValue(attribute);
            }
 
            return ApplyLearningToAttribute(attribute, scratchpad, includeLearning, result);
        }

        public double GetEffectiveAttribute(EveAttribute attribute, EveAttributeScratchpad scratchpad,
                                            bool includeLearning, string implantSet)
        {
            double result = Convert.ToDouble(m_attributes[attribute]);

            if (m_implantSets.ContainsKey(implantSet))
            {
                ImplantSet impSet = m_implantSets[implantSet];
                int ind = (int)attribute;
                UserImplant imp = impSet[ind];
                if (imp != null)
                    result += imp.Bonus;
            }
            return ApplyLearningToAttribute(attribute, scratchpad, includeLearning, result);
        }


        private double ApplyLearningToAttribute(EveAttribute attribute, EveAttributeScratchpad scratchpad,
                                    bool includeLearning, double attributeValue)
        {
            double result = attributeValue;
            double learningBonus = 1.0F;

            // XXX: include implants on scratchpad?
            SkillGroup learningSg = m_skillGroups["Learning"];
            switch (attribute)
            {
                case EveAttribute.Intelligence:
                    result += learningSg["Analytical Mind"].Level;
                    result += learningSg["Logic"].Level;
                    break;
                case EveAttribute.Charisma:
                    result += learningSg["Empathy"].Level;
                    result += learningSg["Presence"].Level;
                    break;
                case EveAttribute.Memory:
                    result += learningSg["Instant Recall"].Level;
                    result += learningSg["Eidetic Memory"].Level;
                    break;
                case EveAttribute.Willpower:
                    result += learningSg["Iron Will"].Level;
                    result += learningSg["Focus"].Level;
                    break;
                case EveAttribute.Perception:
                    result += learningSg["Spatial Awareness"].Level;
                    result += learningSg["Clarity"].Level;
                    break;
            }
            if (scratchpad != null)
            {
                result += scratchpad.GetAttributeBonus(attribute);
            }

            if (includeLearning)
            {
                int learningLevel = learningSg["Learning"].Level;
                if (scratchpad != null)
                {
                    learningLevel += scratchpad.LearningLevelBonus;
                }

                learningBonus = 1.0 + (0.02 * learningLevel);

                return (result * learningBonus);
            }
            else
            {
                return result;
            }
        }

        public double getImplantValue(EveAttribute eveAttribute)
        {
            double result = 0.0F;
            foreach (UserImplant geab in ImplantBonuses)
            {
                if (geab.Slot - 1 == (int)eveAttribute)
                {
                    result = geab.Bonus;
                    break;
                }
            }
            return result;
        }

        public string getImplantName(EveAttribute eveAttribute)
        {
            if (eveAttribute == EveAttribute.None)
                return "???";
            string result = string.Empty;
            foreach (UserImplant geab in ImplantBonuses)
            {
                if (geab.Slot - 1 == (int)eveAttribute)
                {
                    result = geab.Name;
                    break;
                }
            }
            return result;
        }

        public double LearningBonus
        {
            get { return 1 + (m_skillGroups["Learning"]["Learning"].Level * 0.02); }
        }

        public double EffectiveIntelligence
        {
            get { return GetEffectiveAttribute(EveAttribute.Intelligence); }
        }

        public double EffectiveCharisma
        {
            get { return GetEffectiveAttribute(EveAttribute.Charisma); }
        }

        public double EffectivePerception
        {
            get { return GetEffectiveAttribute(EveAttribute.Perception); }
        }

        public double EffectiveMemory
        {
            get { return GetEffectiveAttribute(EveAttribute.Memory); }
        }

        public double EffectiveWillpower
        {
            get { return GetEffectiveAttribute(EveAttribute.Willpower); }
        }

        public double GetAttributeBonusFromImplants(EveAttribute eveAttribute)
        {
            double result = 0.0F;
            double learningBonus = 1.0F;

            result = getImplantValue(eveAttribute);

            int learningLevel = m_skillGroups["Learning"]["Learning"].Level;
            learningBonus = 1.0 + (0.02 * learningLevel);
            return result * learningBonus;
        }

        public IList<UserImplant> ImplantBonuses
        {
            get { return m_CurrentImplants; }
        }

        private void m_implantSet_Changed(object sender, EventArgs e)
        {
            OnImplantSetChanged();
        }

        private void OnImplantSetChanged()
        {
            OnAttributeChanged();
            FireEvent(delegate
                          {
                              if (ImplantSetChanged != null)
                              {
                                  ImplantSetChanged(this, new EventArgs());
                              }
                          }, "implantset");
        }

        public event EventHandler ImplantSetChanged;

        public Dictionary<string, SkillGroup> SkillGroups
        {
            get { return m_skillGroups; }
        }

        public Dictionary<int, Skill> AllSkillsByTypeID
        {
            get { return m_AllSkillsByID; }
        }

        private int m_cachedSkillPointTotal = -1;
        private int m_cachedKnownSkillCount = -1;
        private Dictionary<int, int> m_cachedLevelSkillsCount = null;

        private List<Skill> m_skillsChanged = new List<Skill>();

        /// <summary>
        /// Fired when skill changes sp, changes Known status or starts/stops training
        /// </summary>
        /// <param name="gs"></param>
        private void OnSkillChanged(Skill gs)
        {
            m_cachedSkillPointTotal = -1;
            m_cachedKnownSkillCount = -1;
            m_cachedLevelSkillsCount = null;
            lock (m_skillsChanged)
            {
                m_skillsChanged.Add(gs);
            }

            FireEvent(delegate
                          {
                              Skill[] mySkillsChanged;
                              lock (m_skillsChanged)
                              {
                                  mySkillsChanged = new Skill[m_skillsChanged.Count];
                                  m_skillsChanged.CopyTo(mySkillsChanged);
                                  m_skillsChanged.Clear();
                              }
                              if (SkillChanged != null)
                              {
                                  SkillChanged(this, new SkillChangedEventArgs(mySkillsChanged));
                              }
                          }, "skill");
            if (m_skillGroups["Learning"].Contains(gs.Name))
            {
                OnAttributeChanged();
            }
        }

        void gs_TrainingStatusChanged(object sender, EventArgs e)
        {
            FireEvent(delegate
            {
                if (TrainingSkillChanged != null)
                {
                    TrainingSkillChanged(this, new EventArgs());
                }
            }, "trainingSkill");
        }

        public event SkillChangedHandler SkillChanged;
        public event EventHandler TrainingSkillChanged;

        public Skill GetSkill(string skillName)
        {
            foreach (SkillGroup gsg in m_skillGroups.Values)
            {
                Skill gs = gsg[skillName];
                if (gs != null)
                {
                    return gs;
                }
            }
            return null;
        }

        public int SkillPointTotal
        {
            get
            {
                if (m_cachedSkillPointTotal == -1)
                {
                    m_cachedSkillPointTotal = 0;
                    foreach (SkillGroup gsg in m_skillGroups.Values)
                    {
                        foreach (Skill gs in gsg)
                        {
                            if (!gs.InTraining)
                            {
                                m_cachedSkillPointTotal += gs.CurrentSkillPoints;
                            }
                        }
                    }
                }

                Skill cts = this.CurrentlyTrainingSkill;
                if (cts == null)
                {
                    return m_cachedSkillPointTotal;
                }
                else
                {
                    return m_cachedSkillPointTotal + cts.CurrentSkillPoints;
                }
            }
        }

        public int KnownSkillCount
        {
            get
            {
                if (m_cachedKnownSkillCount == -1)
                {
                    m_cachedKnownSkillCount = 0;
                    foreach (SkillGroup gsg in m_skillGroups.Values)
                    {
                        m_cachedKnownSkillCount += gsg.KnownCount;
                    }
                }
                return m_cachedKnownSkillCount;
            }
        }

        public int SkillCountAtLevel(int level)
        {
            if (m_cachedLevelSkillsCount == null)
            {
                m_cachedLevelSkillsCount = new Dictionary<int, int>();
                for (int i=1;i<6;i++)
                {
                    m_cachedLevelSkillsCount.Add(i,0);
                    foreach (SkillGroup sg in m_skillGroups.Values)
                    {
                        m_cachedLevelSkillsCount[i] += sg.GetSkillsAtLevel(i);
                    }
                }
            }
            return m_cachedLevelSkillsCount[level];
        }

        private Skill m_currentlyTrainingSkill;

        public Skill CurrentlyTrainingSkill
        {
            get { return m_currentlyTrainingSkill; }
        }

        public void CancelCurrentSkillTraining()
        {
            if (m_currentlyTrainingSkill != null)
            {
                m_currentlyTrainingSkill.StopTraining();
            }
        }

        private bool m_isCached = false;

        public bool IsCached
        {
            get { return m_isCached; }
            set
            {
                if (m_isCached != value)
                {
                    m_isCached = value;
                    OnBioInfoChanged();
                }
            }
        }

        public void AssignFromSerializableSkillTrainingInfo(SerializableSkillTrainingInfo sti)
        {
            this.SuppressEvents();
            this.checkTrainingSkills(sti);
            this.ResumeEvents();
        }


        //public void AssignFromSerializableCharacterInfo(SerializableCharacterInfo ci)
        //{
        //    this.SuppressEvents();
        //    this.Name = ci.Name;
        //    this.CharacterId = ci.CharacterId;
        //    this.IsCached = ci.IsCached;
        //    this.Gender = ci.Gender;
        //    this.Race = ci.Race;
        //    this.Bloodline = ci.BloodLine;
        //    this.CorporationName = ci.CorpName;
            
        //    if (ci.IsCached == true)
        //    {
        //        this.EVEFolder = ci.EVEFolder;
        //    }
        //    this.Balance = ci.Balance;

        //    bool getcurrent = false;

        //    if (this.implantSets.Count == 0)
        //    {
        //        if (ci.ImplantSets.Count == 0)
        //            getcurrent = true;
        //        foreach (SerializableImplantSet x in ci.ImplantSets)
        //        {
        //            UserImplant[] z = new UserImplant[10];
        //            foreach (UserImplant y in x.Implants)
        //            {
        //                if (y != null)
        //                    z[y.Slot - 1] = y;
        //            }
        //            string key = string.Empty;

        //            // WARNING! If (or when) clones get proper names, then the implant calc
        //            // will need amending too - see WARNING comment in ImplantCalculator.cs
        //            // in the menu event handlers
        //            switch (x.Number)
        //            {
        //                case 0:
        //                    key = "Auto";
        //                    break;
        //                case 1:
        //                    key = "Current";
        //                    break;
        //                case 2:
        //                case 3:
        //                case 4:
        //                case 5:
        //                case 6:
        //                    key = "Clone " + (x.Number - 1);
        //                    break;
        //                default:
        //                    key = "Unknown";
        //                    break;
        //            }
        //            this.implantSets.Add(key, new ImplantSet(z));
        //        }
        //    }

        //    this.BasePerception = ci.Attributes.BasePerception;
        //    this.BaseMemory = ci.Attributes.BaseMemory;
        //    this.BaseWillpower = ci.Attributes.BaseWillpower;
        //    this.BaseIntelligence = ci.Attributes.BaseIntelligence;
        //    this.BaseCharisma = ci.Attributes.BaseCharisma;

        //    List<UserImplant> manualBonuses = new List<UserImplant>();
        //    foreach (UserImplant x in this.ImplantBonuses)
        //    {
        //        if (x.Manual && getcurrent)
        //            manualBonuses.Add(x);
        //    }

        //    this.ImplantBonuses.Clear();

        //    Slot[] Implants = Slot.GetImplants();

        //    if (this.implantSets.ContainsKey("Auto"))
        //        this.implantSets.Remove("Auto");

        //    foreach (SerializableEveAttributeBonus bonus in ci.AttributeBonuses.Bonuses)
        //    {
        //        int slot = UserImplant.AttribToSlot(bonus.EveAttribute);
        //        if (!bonus.Manual)
        //        {
        //            if (slot != 0)
        //            {
        //                if (!this.implantSets.ContainsKey("Auto"))
        //                {
        //                    this.implantSets.Add("Auto", new ImplantSet());
        //                }
        //                this.implantSets["Auto"][slot - 1] = new UserImplant(slot, Implants[slot - 1][bonus.Name], bonus.Manual);
        //            }
        //        }
        //        else
        //        {
        //            if (slot != 0 && getcurrent)
        //            {
        //                if (!this.implantSets.ContainsKey("Current"))
        //                {
        //                    this.implantSets.Add("Current", new ImplantSet());
        //                }
        //                Implant x = Implants[slot - 1][bonus.Name];
        //                if (x == null)
        //                {
        //                    x = new Implant();
        //                    x.Name = bonus.Name;
        //                    x.Bonus = bonus.Amount;
        //                }
        //                this.implantSets["Current"][slot - 1] = new UserImplant(slot, x, bonus.Manual);
        //            }
        //        }
        //    }
        //    foreach (UserImplant tb in manualBonuses)
        //    {
        //        if (tb.Manual)
        //        {
        //            if (tb.Slot != 0)
        //            {
        //                if (!this.implantSets.ContainsKey("Current"))
        //                {
        //                    this.implantSets.Add("Current", new ImplantSet());
        //                }
        //                Implant x = Implants[tb.Slot - 1][tb.Name];
        //                if (x == null)
        //                {
        //                    x = new Implant();
        //                    x.Name = tb.Name;
        //                    x.Bonus = tb.Bonus;
        //                }
        //                this.implantSets["Current"][tb.Slot - 1] = new UserImplant(tb.Slot, x, tb.Manual);
        //            }
        //        }
        //    }
        //    if (this.implantSets.ContainsKey("Auto"))
        //    {
        //        if (!this.implantSets.ContainsKey("Current"))
        //        {
        //            for (int i = 0; i < this.implantSets["Auto"].Array.GetLength(0); i++)
        //            {
        //                UserImplant x = this.implantSets["Auto"].Array[i];
        //                if (x != null)
        //                    this.ImplantBonuses.Add(x);
        //            }
        //        }
        //        else
        //        {
        //            for (int i = 0; i < Math.Max(this.implantSets["Auto"].Array.GetLength(0), this.implantSets["Current"].Array.GetLength(0)); i++)
        //            {
        //                UserImplant x = null;
        //                if (i < this.implantSets["Auto"].Array.GetLength(0))
        //                    x = this.implantSets["Auto"].Array[i];
        //                UserImplant y = null;
        //                if (i < this.implantSets["Current"].Array.GetLength(0))
        //                    y = this.implantSets["Current"].Array[i];
        //                if (y != null)
        //                    this.ImplantBonuses.Add(y);
        //                else if (x != null)
        //                    this.ImplantBonuses.Add(x);
        //            }
        //        }
        //    }
        //    else if (this.implantSets.ContainsKey("Current"))
        //    {
        //        for (int i = 0; i < this.implantSets["Current"].Array.GetLength(0); i++)
        //        {
        //            UserImplant x = this.implantSets["Current"].Array[i];
        //            if (x != null)
        //                this.ImplantBonuses.Add(x);
        //        }
        //    }

        //    foreach (SerializableSkillGroup sg in ci.SkillGroups)
        //    {
        //        SkillGroup gsg = m_skillGroups[sg.Name];
        //        foreach (SerializableSkill s in sg.Skills)
        //        {
        //            Skill gs = gsg[s.Name];
        //            if (gs == null)
        //            {
        //                MessageBox.Show("The character cache contains the unknown skill " + s.Name + ", which will be removed.",
        //                            "Unknown skill", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        //            }
        //            else
        //            {
        //                gs.CurrentSkillPoints = s.SkillPoints;
        //                gs.Known = true;
        //                if (ci.TimeLeftInCache != -1)
        //                    gs.LastConfirmedLvl = s.Level;
        //                else
        //                    gs.LastConfirmedLvl = s.LastConfirmedLevel;
        //            }
        //        }
        //    }
        //    checkTrainingSkills(ci.TrainingSkillInfo);
        //    this.ResumeEvents();
        //}

  
        /// <summary>
        /// Called from CharcterMonitor.start() to load from settings cache for both online and file based
        /// Called from CharcterMonitor.RelaodFromFile() (when file is updated)
        /// Called from EveSession.UpdateGrandCharacterInfo when xml is retrieved and there is more than 3 minutes 30 secs left in it's cache
        /// Called from GetPlanByName in settings.cs (!!! Fix that!!!)
        /// </summary>
        /// <param name="ci"></param>
        public void AssignFromSerializableCharacterSheet(SerializableCharacterSheet ci)
        {
            this.SuppressEvents();
            
            this.Name = ci.CharacterSheet.Name;
            this.CharacterId = ci.CharacterSheet.CharacterId;
            this.IsCached = ci.XMLExpires < DateTime.Now.ToUniversalTime();
            this.Gender = ci.CharacterSheet.Gender;
            this.Race = ci.CharacterSheet.Race;
            this.Bloodline = ci.CharacterSheet.BloodLine;
            this.CorporationName = ci.CharacterSheet.CorpName;
            this.XMLExpires = ci.XMLExpires;

        
            if (IsCached)
            {
                this.EVEFolder = ci.EVEFolder;
            }
            this.Balance = ci.CharacterSheet.Balance;

            bool getcurrent = false;

            if (this.implantSets.Count == 0)
            {
                if (ci.ImplantSets.Count == 0)
                    getcurrent = true;
                foreach (SerializableImplantSet x in ci.ImplantSets)
                {
                    UserImplant[] z = new UserImplant[10];
                    foreach (UserImplant y in x.Implants)
                    {
                        if (y != null)
                            z[y.Slot - 1] = y;
                    }
                    string key = string.Empty;

                    // WARNING! If (or when) clones get proper names, then the implant calc
                    // will need amending too - see WARNING comment in ImplantCalculator.cs
                    // in the menu event handlers
                    switch (x.Number)
                    {
                        case 0:
                            key = "Auto";
                            break;
                        case 1:
                            key = "Current";
                            break;
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                            key = "Clone " + (x.Number - 1);
                            break;
                        default:
                            key = "Unknown";
                            break;
                    }
                    this.implantSets.Add(key, new ImplantSet(z));
                }
            }

            this.BasePerception = ci.CharacterSheet.Attributes.BasePerception;
            this.BaseMemory = ci.CharacterSheet.Attributes.BaseMemory;
            this.BaseWillpower = ci.CharacterSheet.Attributes.BaseWillpower;
            this.BaseIntelligence = ci.CharacterSheet.Attributes.BaseIntelligence;
            this.BaseCharisma = ci.CharacterSheet.Attributes.BaseCharisma;

            List<UserImplant> manualBonuses = new List<UserImplant>();
            foreach (UserImplant x in this.ImplantBonuses)
            {
                if (x.Manual && getcurrent)
                    manualBonuses.Add(x);
            }

            this.ImplantBonuses.Clear();

            Slot[] Implants = Slot.GetImplants();

            if (this.implantSets.ContainsKey("Auto"))
                this.implantSets.Remove("Auto");

            foreach (SerializableEveAttributeBonus bonus in ci.CharacterSheet.AttributeBonuses.Bonuses)
            {
                int slot = UserImplant.AttribToSlot(bonus.EveAttribute);
                if (!bonus.Manual)
                {
                    if (slot != 0)
                    {
                        if (!this.implantSets.ContainsKey("Auto"))
                        {
                            this.implantSets.Add("Auto", new ImplantSet());
                        }
                        this.implantSets["Auto"][slot - 1] = new UserImplant(slot, Implants[slot - 1][bonus.Name], bonus.Manual);
                    }
                }
                else
                {
                    if (slot != 0 && getcurrent)
                    {
                        if (!this.implantSets.ContainsKey("Current"))
                        {
                            this.implantSets.Add("Current", new ImplantSet());
                        }
                        Implant x = Implants[slot - 1][bonus.Name];
                        if (x == null)
                        {
                            x = new Implant();
                            x.Name = bonus.Name;
                            x.Bonus = bonus.Amount;
                        }
                        this.implantSets["Current"][slot - 1] = new UserImplant(slot, x, bonus.Manual);
                    }
                }
            }
            foreach (UserImplant tb in manualBonuses)
            {
                if (tb.Manual)
                {
                    if (tb.Slot != 0)
                    {
                        if (!this.implantSets.ContainsKey("Current"))
                        {
                            this.implantSets.Add("Current", new ImplantSet());
                        }
                        Implant x = Implants[tb.Slot - 1][tb.Name];
                        if (x == null)
                        {
                            x = new Implant();
                            x.Name = tb.Name;
                            x.Bonus = tb.Bonus;
                        }
                        this.implantSets["Current"][tb.Slot - 1] = new UserImplant(tb.Slot, x, tb.Manual);
                    }
                }
            }
            if (this.implantSets.ContainsKey("Auto"))
            {
                if (!this.implantSets.ContainsKey("Current"))
                {
                    for (int i = 0; i < this.implantSets["Auto"].Array.GetLength(0); i++)
                    {
                        UserImplant x = this.implantSets["Auto"].Array[i];
                        if (x != null)
                            this.ImplantBonuses.Add(x);
                    }
                }
                else
                {
                    for (int i = 0; i < Math.Max(this.implantSets["Auto"].Array.GetLength(0), this.implantSets["Current"].Array.GetLength(0)); i++)
                    {
                        UserImplant x = null;
                        if (i < this.implantSets["Auto"].Array.GetLength(0))
                            x = this.implantSets["Auto"].Array[i];
                        UserImplant y = null;
                        if (i < this.implantSets["Current"].Array.GetLength(0))
                            y = this.implantSets["Current"].Array[i];
                        if (y != null)
                            this.ImplantBonuses.Add(y);
                        else if (x != null)
                            this.ImplantBonuses.Add(x);
                    }
                }
            }
            else if (this.implantSets.ContainsKey("Current"))
            {
                for (int i = 0; i < this.implantSets["Current"].Array.GetLength(0); i++)
                {
                    UserImplant x = this.implantSets["Current"].Array[i];
                    if (x != null)
                        this.ImplantBonuses.Add(x);
                }
            }

            foreach (SerializableSkillGroup sg in ci.SkillGroups)
            {
                SkillGroup gsg = m_skillGroups[sg.Name];
                foreach (SerializableSkill s in sg.Skills)
                {
                    Skill gs = gsg[s.Name];
                    if (gs == null)
                    {
                        MessageBox.Show("The character cache contains the unknown skill " + s.Name + ", which will be removed.",
                                    "Unknown skill", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                    else
                    {
                        gs.CurrentSkillPoints = s.SkillPoints;
                        gs.Known = true;
                        if (ci.FromCCP)
                            gs.LastConfirmedLvl = s.Level;
                        else
                            gs.LastConfirmedLvl = s.LastConfirmedLevel;
                    }
                }
            }
            checkTrainingSkills(ci.TrainingSkillInfo);
            this.ResumeEvents();
        }
        public void checkOldSkill()
        {
            DateTime _OSITLocalCompleteTime = DateTime.MinValue;
            if (m_OldSkillInTraining != null)
            {
                _OSITLocalCompleteTime = m_OldSkillInTraining.getTrainingEndTime.ToLocalTime();
            }
			if (m_SkillInTraining != null && m_OldSkillInTraining != null && m_OldSkillInTraining.isSkillInTraining &&
						(m_OldSkillInTraining.TrainingSkillWithTypeID != m_SkillInTraining.TrainingSkillWithTypeID ||
						m_OldSkillInTraining.getTrainingEndTime != m_SkillInTraining.getTrainingEndTime))
            {
				this.CancelCurrentSkillTraining();
            }
            if (!firstRun && m_OldSkillInTraining != null && m_OldSkillInTraining.isSkillInTraining)
            {
                Skill _OSIT = null;
				if (m_AllSkillsByID.ContainsKey(m_OldSkillInTraining.TrainingSkillWithTypeID))
					_OSIT = m_AllSkillsByID[m_OldSkillInTraining.TrainingSkillWithTypeID];
                if (_OSIT != null)
                {
                    bool add = false;
                    bool check = false;
                    string skillName = _OSIT.Name;
                    int level = m_OldSkillInTraining.TrainingSkillToLevel;
                    if (m_OldSkillInTraining.AlertRaisedAlready)
                    {
                        if (_OSIT.UnadjustedCurrentSkillPoints < _OSIT.GetPointsRequiredForLevel(level))
                            check = true;
                    }
                    else
                    {
                        if (_OSIT.UnadjustedCurrentSkillPoints >= _OSIT.GetPointsRequiredForLevel(level))
                        {
                            if (_OSIT.InTraining && _OSIT.TrainingToLevel == level)
                            {
                                _OSIT.CurrentSkillPoints = _OSIT.GetPointsRequiredForLevel(level);
                                this.CancelCurrentSkillTraining();
                            }
                            m_OldSkillInTraining.AlertRaisedAlready = true;
                            add = true;
                        }
                        check = true;
                    }
                    if (check)
                    {
                        OnDownloadAttemptComplete(this.Name, skillName, add);
                    }
                }
                if (m_OldSkillInTraining != null && this.CurrentlyTrainingSkill == null)
                {
                    // Check we actually have a skill in training
                    if (_OSIT != null)
                    {
                        string skillName = _OSIT.Name;
                        int level = m_OldSkillInTraining.TrainingSkillToLevel;
                        // See if the old_skill in the current details has completed it's training
                        if (_OSIT.CurrentSkillPoints >= _OSIT.GetPointsRequiredForLevel(level))
                        {
                            if (m_OldSkillInTraining.AlertRaisedAlready)
                            {
                                _OSIT.CurrentSkillPoints = _OSIT.CurrentSkillPoints;
                            }
                            if (!m_OldSkillInTraining.AlertRaisedAlready)
                            {
                                // Right, so the skill needs to be flagged as done.
                                // Oh yeah, if you don't do this skill points for some odd reason reset to the old XML values when you cancel the skill training so...
                                _OSIT.CurrentSkillPoints = _OSIT.CurrentSkillPoints;
                                m_OldSkillInTraining.AlertRaisedAlready = true;
                                // Oh, yeah, we need to add this skill to the alerts...
                                // The alerter takes care of whether it's already there or not.
                                OnDownloadAttemptComplete(this.Name, skillName, true);
                            }
                        }
                        else if (_OSIT.CurrentSkillPoints < _OSIT.GetPointsRequiredForLevel(level))
                        {
                            // Here is where we set the currently training skill according to the last
                            // known skill in training.
                            // To make doubly sure we have no old training skills lurking ...out with the old...
                            this.CancelCurrentSkillTraining();
                            // ...and in with the new
							m_SkillInTraining = (SerializableSkillTrainingInfo)m_OldSkillInTraining.Clone();
							_OSIT.SetTrainingInfo(level, _OSITLocalCompleteTime,m_SkillInTraining);
                        }
                    }
                }
                // Now to activate normal runtime skill completion monitoring
                m_attemptedDLComplete = true;
            }
        }

        public void UpdateOwnedSkills()
        {
            List<String> owned = new List<string>();
            foreach (SkillGroup sg in m_skillGroups.Values)
            {
                owned.AddRange(sg.OwnedSkills());
            }
            Settings.GetInstance().SetOwnedBooks(m_name, owned);
        }

        private SerializableSkillTrainingInfo m_SkillInTraining = null;

        public SerializableSkillTrainingInfo SerialSIT
        {
            get { return m_SkillInTraining; }
        }

        private SerializableSkillTrainingInfo m_OldSkillInTraining = null;

        public SerializableSkillTrainingInfo OldSerialSIT
        {
            get { return m_OldSkillInTraining; }
            set { m_OldSkillInTraining = value; }
        }

		/// <summary>  
        /// Recursively check that a skill has all it's prereqs changed.  
        /// </summary>  
		private void MarkPrereqsTrained(Skill s)
		{
			foreach (Skill.Prereq pReq in s.Prereqs)
			{
				if (pReq != null)
				{
					if (pReq.Skill.UnadjustedCurrentSkillPoints < pReq.Skill.GetPointsRequiredForLevel(pReq.Level))
					{
						if (pReq.Skill.UnadjustedCurrentSkillPoints == 0)
							MarkPrereqsTrained(pReq.Skill);
						pReq.Skill.CurrentSkillPoints = pReq.Skill.GetPointsRequiredForLevel(pReq.Level);
						pReq.Skill.Known = true;
						OnSkillChanged(pReq.Skill);
					}
				}
			}
		}  

        public void checkTrainingSkills(SerializableSkillTrainingInfo SkillInTraining)
        {
            // This is called from AssignFromSerializableCharacterInfo(SerializableCharacterInfo ci)
            // This is where normal running takes you in the standard run of the mill operation of EVEMon
            
            // First one thing we can do no matter what
            Skill _SkillInTraining = null;
            DateTime _SITLocalCompleteTime = DateTime.MinValue;
            DateTime _SITLocalStartTime = DateTime.MinValue;
			DateTime _SITLocalUpdateTime = DateTime.MinValue;
			if (SkillInTraining != null)
			{
				_SITLocalUpdateTime = SkillInTraining.GetDateTimeAtUpdate.ToLocalTime();
				if (SkillInTraining.isSkillInTraining)
				{
					_SkillInTraining = this.AllSkillsByTypeID[SkillInTraining.TrainingSkillWithTypeID];
					_SITLocalCompleteTime = SkillInTraining.getTrainingEndTime.ToLocalTime();
					_SITLocalStartTime = SkillInTraining.getTrainingStartTime.ToLocalTime();
					// This would be a good place to change the prereqs too so they are also trained up fully.
					// This just does one level of prereqs... we really need recursive updating...
					if (_SkillInTraining != null)
					{
						// All PreReqs must have been trained for this to be training.
						MarkPrereqsTrained(_SkillInTraining);

						// Once we have done the pre-reqs, we can set this skill's current skill points
						if (_SkillInTraining.UnadjustedCurrentSkillPoints < SkillInTraining.EstimatedPointsAtUpdate)
						{
							_SkillInTraining.CurrentSkillPoints = SkillInTraining.EstimatedPointsAtUpdate;
							_SkillInTraining.Known = true;
							OnSkillChanged(_SkillInTraining);
						}
					}
					else
					{
						// unknown skill in training...
					}
					// Now look at the previous skill we were training and set accordingly.
					// We haven't changed the current and old skills yet, so...
					if (m_SkillInTraining != null && m_SkillInTraining.TrainingSkillWithTypeID != SkillInTraining.TrainingSkillWithTypeID)
					{
						if (m_SkillInTraining.isSkillInTraining)
						{
							if (SkillInTraining.getTrainingStartTime < m_SkillInTraining.getTrainingEndTime)
							{
								// We need to adjust the SP of the previously training skill as it was changed before completion
								// I'm assuming here that you haven't changed implants (actually, that wouldn't matter) or something and it's not a prereq of the current skill in training
								// You could stick some extra checks in here to make sure.
								Skill theOneImInterestedIn = null;
								if (m_AllSkillsByID.ContainsKey(m_SkillInTraining.TrainingSkillWithTypeID))
									theOneImInterestedIn = this.m_AllSkillsByID[m_SkillInTraining.TrainingSkillWithTypeID];
								if (theOneImInterestedIn != null)
								{
									theOneImInterestedIn.CurrentSkillPoints = m_SkillInTraining.EstimatedPointsAtTime(_SITLocalStartTime);
									OnSkillChanged(theOneImInterestedIn);
								}
							}
						}
					}
				}
				else
				{
					// No currently training skill
					// Did we have a previous one?
					if (m_SkillInTraining != null && m_SkillInTraining.isSkillInTraining)
					{
						Skill theOneImInterestedIn = null;
						if (m_AllSkillsByID.ContainsKey(m_SkillInTraining.TrainingSkillWithTypeID))
							theOneImInterestedIn = this.m_AllSkillsByID[m_SkillInTraining.TrainingSkillWithTypeID];
						if (theOneImInterestedIn != null)
						{
							if (SkillInTraining.GetDateTimeAtUpdate < m_SkillInTraining.getTrainingEndTime)
								theOneImInterestedIn.CurrentSkillPoints = m_SkillInTraining.EstimatedPointsAtTime(_SITLocalUpdateTime);
							OnSkillChanged(theOneImInterestedIn);
						}
					}
				}
			}

            // check if old skill is complete in the current character data and if not, set to currenttrainingskill
			if (m_SkillInTraining != null &&
                    (SkillInTraining == null || 
                    (SkillInTraining != null &&
						(!SkillInTraining.isSkillInTraining ||
						SkillInTraining.TrainingSkillWithTypeID != m_SkillInTraining.TrainingSkillWithTypeID ||
						SkillInTraining.getTrainingEndTime != m_SkillInTraining.getTrainingEndTime))))
            {
                // Skill or current expected completion time changed since previous update.
                this.CancelCurrentSkillTraining();
            }
            if (!firstRun)
            {
                if (SkillInTraining != null && SkillInTraining.isSkillInTraining)
                {
                    if (m_SkillInTraining != null && m_SkillInTraining.PreWarningGiven && m_SkillInTraining.TrainingSkillWithTypeID == SkillInTraining.TrainingSkillWithTypeID && SkillInTraining.getTrainingEndTime == m_SkillInTraining.getTrainingEndTime)
                    {
                        SkillInTraining.PreWarningGiven = true;
                    }
                    if (m_OldSkillInTraining != null && m_OldSkillInTraining.AlertRaisedAlready && m_OldSkillInTraining.TrainingSkillWithTypeID == SkillInTraining.TrainingSkillWithTypeID && SkillInTraining.getTrainingEndTime == m_OldSkillInTraining.getTrainingEndTime)
                    {
                        SkillInTraining.PreWarningGiven = true;
                        SkillInTraining.AlertRaisedAlready = true;
                    }
                }
                // Go through the m_OldSkillInTraining stuff.
                if (m_OldSkillInTraining != null && this.m_AllSkillsByID.ContainsKey(m_OldSkillInTraining.TrainingSkillWithTypeID))
                {
                    bool add = false;
                    bool check = false;
                    Skill oldskill = this.m_AllSkillsByID[m_OldSkillInTraining.TrainingSkillWithTypeID];
                    int level = m_OldSkillInTraining.TrainingSkillToLevel;
                    DateTime _OSITLocalCompleteTime = m_OldSkillInTraining.getTrainingEndTime.ToLocalTime();
                    if (m_OldSkillInTraining.AlertRaisedAlready)
                    {
                        // if a skill completed when evemon was shutdown and there is currently no new skill is training
                        // then _SITlocalTime is 0 (because skillInTraining is null) and this next block of code results in a large negative skillpoint.
                        // if (_OSITLocalCompleteTime <= DateTime.Now && _OSITLocalCompleteTime > _SITLocalStartTime)
                        if (SkillInTraining != null && _OSITLocalCompleteTime <= DateTime.Now && _OSITLocalCompleteTime > _SITLocalStartTime)
                        {
                            check = true;
                            oldskill.CurrentSkillPoints = m_OldSkillInTraining.EstimatedPointsAtTime(_SITLocalStartTime);
                            OnSkillChanged(oldskill);
                            m_OldSkillInTraining.AlertRaisedAlready = false;
                        }
                    }
                    else
                    {
                        if ((SkillInTraining == null && _OSITLocalCompleteTime <= DateTime.Now) ||
                             (SkillInTraining != null && _OSITLocalCompleteTime <= _SITLocalStartTime))
                        {
                            if (m_OldSkillInTraining.EstimatedPointsAtUpdate >= oldskill.GetPointsRequiredForLevel(level))
                            {
                                m_OldSkillInTraining.AlertRaisedAlready = true;
                                add = true;
                            }
                            else
                            {
                                m_OldSkillInTraining = null;
                            }
                            check = true;
                        }
                    }

                    if (check)
                    {
                        OnDownloadAttemptComplete(this.Name, oldskill.Name, add);
                    }
                }

                // Now for the real meaty bit.
                if (SkillInTraining != null && this.CurrentlyTrainingSkill == null)
                {
                    // Now we depart even more from the version above.
                    // We have to deal with making this character actually show that he is learning the
                    // skill the XML file says he's learning. But we do this carefully as it may be complete
					Skill newTrainingSkill = null;
					if (m_AllSkillsByID.ContainsKey(SkillInTraining.TrainingSkillWithTypeID))
						newTrainingSkill = m_AllSkillsByID[SkillInTraining.TrainingSkillWithTypeID];
					if (newTrainingSkill == null)
					{
						// no recorgnised skill in training
						if (SkillInTraining.APIError.ErrorCode  == 0)
						{
							// Actually had a report that no skill is training
							if (m_SkillInTraining != null && m_SkillInTraining.isSkillInTraining)
							{
								Skill oldskill = null;
								if (m_AllSkillsByID.ContainsKey(m_SkillInTraining.TrainingSkillWithTypeID))
								{
									oldskill = m_AllSkillsByID[m_SkillInTraining.TrainingSkillWithTypeID];
									if (m_SkillInTraining.getTrainingEndTime < SkillInTraining.GetDateTimeAtUpdate)
									{
										oldskill.CurrentSkillPoints = m_SkillInTraining.TrainingSkillDestinationSP;
										OnSkillChanged(oldskill);
										m_SkillInTraining.AlertRaisedAlready = true;
										OnDownloadAttemptComplete(this.Name, oldskill.Name, true);
									}
									else
									{
										oldskill.CurrentSkillPoints = m_SkillInTraining.EstimatedPointsAtTime(_SITLocalUpdateTime);
										OnSkillChanged(oldskill);
									}
								}
							}
							if (m_SkillInTraining != null)
								m_OldSkillInTraining = (SerializableSkillTrainingInfo)m_SkillInTraining.Clone();
							if (SkillInTraining != null)
								m_SkillInTraining = (SerializableSkillTrainingInfo)SkillInTraining.Clone();
						}
						else
						{
							// there was an error getting the info about the currently training skill
						}
					}
					else if (newTrainingSkill != null)
					{
						int level = SkillInTraining.TrainingSkillToLevel;
						int EstCurrentSP = SkillInTraining.EstimatedCurrentPoints;
						bool SkillComplete = (_SITLocalCompleteTime < DateTime.Now);
						DateTime _OSITLocalCompleteTime = DateTime.MinValue;
						if (SkillInTraining.TrainingSkillDestinationSP <= EstCurrentSP)
						{
							if (m_SkillInTraining != null && m_SkillInTraining.AlertRaisedAlready && newTrainingSkill.Id == m_SkillInTraining.TrainingSkillWithTypeID && level == m_SkillInTraining.TrainingSkillToLevel)
							{
								newTrainingSkill.CurrentSkillPoints = SkillInTraining.TrainingSkillDestinationSP;
								OnSkillChanged(newTrainingSkill);
							}
							if (m_SkillInTraining == null || (m_SkillInTraining != null && (!m_SkillInTraining.AlertRaisedAlready || newTrainingSkill.Id != m_SkillInTraining.TrainingSkillWithTypeID || (newTrainingSkill.Id == m_SkillInTraining.TrainingSkillWithTypeID && SkillInTraining.TrainingSkillToLevel != m_SkillInTraining.TrainingSkillToLevel))))
							{
								newTrainingSkill.CurrentSkillPoints = SkillInTraining.TrainingSkillDestinationSP;
								OnSkillChanged(newTrainingSkill);
								m_OldSkillInTraining = (SerializableSkillTrainingInfo)SkillInTraining.Clone();
								m_OldSkillInTraining.AlertRaisedAlready = true;
								m_SkillInTraining = null;
								OnDownloadAttemptComplete(this.Name, newTrainingSkill.Name, true);
							}
						}
						else if (SkillInTraining.TrainingSkillDestinationSP > EstCurrentSP)
						{
							m_SkillInTraining = (SerializableSkillTrainingInfo)SkillInTraining.Clone();
							newTrainingSkill.SetTrainingInfo(level, _SITLocalCompleteTime,m_SkillInTraining);
						}
					}
                }
                // Now to activate normal runtime skill completion monitoring
                m_attemptedDLComplete = true;
            }
            if (firstRun)
            {
                // This is where the old_skill values are initalised,
                // it's here to avoid accidentally triggering any other code on this pass.
                // Order is everything in this section!!
                if (SkillInTraining != null)
                {
                    m_OldSkillInTraining = (SerializableSkillTrainingInfo)SkillInTraining.Clone();
                    m_SkillInTraining = (SerializableSkillTrainingInfo)SkillInTraining.Clone();
                }
                firstRun = false;
            }
        }

        private bool firstRun = true;
        private bool m_attemptedDLComplete = false;

        public bool DLComplete
        {
            get { return m_attemptedDLComplete; }
        }

        public delegate void DownloadAttemptCompletedHandler(object sender, DownloadAttemptCompletedEventArgs oldskill);

        public event DownloadAttemptCompletedHandler DownloadAttemptCompleted;

        public class DownloadAttemptCompletedEventArgs : EventArgs
        {
            private string m_skillName;

            public string SkillName
            {
                get { return m_skillName; }
            }

            private string m_characterName;

            public string CharacterName
            {
                get { return m_characterName; }
            }

            private bool m_complete;

            public bool Complete
            {
                get { return m_complete; }
            }

            public DownloadAttemptCompletedEventArgs(string CharacterName, string skillName, bool Complete)
            {
                m_skillName = skillName;
                m_characterName = CharacterName;
                m_complete = Complete;
            }
        }

        public void triggerSkillComplete(string CharacterName)
        { // Basically trigger the event when a skill completes between downloads
            if (m_SkillInTraining != null && m_SkillInTraining.isSkillInTraining)
            {
                Skill newlyCompletedSkill = null;
                if (m_AllSkillsByID.ContainsKey(m_SkillInTraining.TrainingSkillWithTypeID))
                    newlyCompletedSkill = this.m_AllSkillsByID[m_SkillInTraining.TrainingSkillWithTypeID];
                string name = null;
                if (newlyCompletedSkill != null)
                {
                    name = newlyCompletedSkill.Name;
                    newlyCompletedSkill.CurrentSkillPoints = newlyCompletedSkill.GetPointsRequiredForLevel(m_SkillInTraining.TrainingSkillToLevel);
                    m_OldSkillInTraining = (SerializableSkillTrainingInfo)m_SkillInTraining.Clone();
                    m_SkillInTraining = null;
                    this.CancelCurrentSkillTraining();
                }
                OnDownloadAttemptComplete(CharacterName, name, true);
            }
        }

        private void OnDownloadAttemptComplete(string CharacterName, string skillName, bool Complete)
        {
            if (String.IsNullOrEmpty(CharacterName) || String.IsNullOrEmpty(skillName))
            {
                return;
            }
            Skill temp = this.GetSkill(skillName);
            if (DownloadAttemptCompleted != null && temp != null)
            {
                DownloadAttemptCompletedEventArgs e = new DownloadAttemptCompletedEventArgs(CharacterName, skillName, Complete);
                DownloadAttemptCompleted(this, e);
            }
        }

        /// <summary>
        /// Create a serializable character sheet from CharacterInfo.
        /// Called by: SaveTextFile,SaveFile, UpdateCachedCopy, Plan.tsbExportToXml_Click
        /// </summary>
        /// <returns></returns>
        public SerializableCharacterSheet ExportSerializableCharacterSheet()
        {
            SerializableCharacterSheet ci = new SerializableCharacterSheet();

            ci.CharacterSheet.Name = this.Name;
            ci.CharacterSheet.CharacterId = this.CharacterId;
            ci.CharacterSheet.Gender = this.Gender;
            ci.CharacterSheet.Race = this.Race;
            ci.CharacterSheet.BloodLine = this.Bloodline;
            ci.CharacterSheet.CorpName = this.CorporationName;
            ci.EVEFolder = this.EVEFolder; // to CI
            ci.CharacterSheet.Balance = this.Balance;
            ci.currentTime = SerializableSkillTrainingInfo.ConvertDateTimeToTimeString(DateTime.Now.ToUniversalTime());
            ci.CachedUntilTime = SerializableSkillTrainingInfo.ConvertDateTimeToTimeString(m_xmlExpires.ToUniversalTime());

            ci.ImplantSets.Clear();
            foreach (string x in this.implantSets.Keys)
            {
                SerializableImplantSet z = new SerializableImplantSet(this.implantSets[x].Array);
                switch (x)
                {
                    case "Auto":
                        z.Number = 0;
                        break;
                    case "Current":
                        z.Number = 1;
                        break;
                    case "Clone 1":
                        z.Number = 2;
                        break;
                    case "Clone 2":
                        z.Number = 3;
                        break;
                    case "Clone 3":
                        z.Number = 4;
                        break;
                    case "Clone 4":
                        z.Number = 5;
                        break;
                    case "Clone 5":
                        z.Number = 6;
                        break;
                    default:
                        z.Number = -1;
                        break;
                }
                ci.ImplantSets.Add(z);
            }

            ci.CharacterSheet.Attributes.BaseIntelligence = this.BaseIntelligence;
            ci.CharacterSheet.Attributes.BaseCharisma = this.BaseCharisma;
            ci.CharacterSheet.Attributes.BasePerception = this.BasePerception;
            ci.CharacterSheet.Attributes.BaseMemory = this.BaseMemory;
            ci.CharacterSheet.Attributes.BaseWillpower = this.BaseWillpower;

            foreach (UserImplant geab in this.ImplantBonuses)
            {
                SerializableEveAttributeBonus eab = null;
                switch (geab.Slot)
                {
                    case 1:
                        eab = new SerializablePerceptionBonus();
                        break;
                    case 2:
                        eab = new SerializableMemoryBonus();
                        break;
                    case 3:
                        eab = new SerializableWillpowerBonus();
                        break;
                    case 4:
                        eab = new SerializableIntelligenceBonus();
                        break;
                    case 5:
                        eab = new SerializableCharismaBonus();
                        break;
                }
                if (eab != null)
                {
                    eab.Name = geab.Name;
                    eab.Amount = geab.Bonus;
                    eab.Manual = geab.Manual;
                    ci.CharacterSheet.AttributeBonuses.Bonuses.Add(eab);
                }
            }

            foreach (SkillGroup gsg in this.SkillGroups.Values)
            {
                foreach (Skill gs in gsg)
                {
                    if (gs.CurrentSkillPoints > 0)
                    {
                        SerializableKnownSkill s = new SerializableKnownSkill();
                        s.SkillId = gs.Id;
                        s.SkillLevel = gs.Level;
                        s.Skillpoints = gs.CurrentSkillPoints;
                        s.LastConfirmedLevel = gs.LastConfirmedLvl;
                        ci.CharacterSheet.KnownSkillsSet.KnownSkills.Add(s);
                    }
                }
            }
            if (this.m_SkillInTraining != null)
                ci.TrainingSkillInfo = (SerializableSkillTrainingInfo)this.m_SkillInTraining.Clone();
            else
                ci.TrainingSkillInfo = null;
            return ci;
        }

        public TimeSpan GetTrainingTimeToMultipleSkills(IEnumerable<Pair<Skill, int>> skills)
        {
            Dictionary<Skill, int> trainedAlready = new Dictionary<Skill, int>();
            TimeSpan result = TimeSpan.Zero;

            foreach (Pair<Skill, int> ts in skills)
            {
                result += ts.A.GetPrerequisiteTime(trainedAlready);
                for (int i = 1; i <= ts.B; i++)
                {
                    int pointsReq = ts.A.GetPointsRequiredForLevel(i);
                    bool needTrain = true;
                    if ((trainedAlready.ContainsKey(ts.A) &&
                         trainedAlready[ts.A] >= pointsReq) ||
                        ts.A.Level >= ts.B)
                    {
                        needTrain = false;
                    }
                    if (needTrain)
                    {
                        result += ts.A.GetTrainingTimeOfLevelOnly(i, true);
                        trainedAlready[ts.A] = ts.A.GetPointsRequiredForLevel(i);
                    }
                }
            }

            return result;
        }

        private string m_downloadError = null;
        public string DownloadError
        {
            get { return m_downloadError; }
            set { m_downloadError = value; }
        }


        public int DownloadFailed
        {
            get
            {
                return m_downloadfailed;
            }
            set
            {
                m_downloadfailed = value;
            }
        }
    }

    public delegate void SkillChangedHandler(object sender, SkillChangedEventArgs e);

    public class SkillChangedEventArgs : EventArgs
    {
        private Skill[] m_skillList;

        public ICollection<Skill> SkillList
        {
            get { return m_skillList; }
        }

        public SkillChangedEventArgs(Skill gs)
        {
            m_skillList = new Skill[1];
            m_skillList[0] = gs;
        }

        public SkillChangedEventArgs(Skill[] skillList)
        {
            m_skillList = new Skill[skillList.Length];
            skillList.CopyTo(m_skillList, 0);
        }
    }

    public enum ChangeType
    {
        Added,
        Removed,
        Cleared
    }

    public class ChangedEventArgs<T> : EventArgs
    {
        private T m_item;
        private ChangeType m_changeType;

        public T Item
        {
            get { return m_item; }
        }

        public ChangeType ChangeType
        {
            get { return m_changeType; }
        }

        public ChangedEventArgs(T item, ChangeType changeType)
        {
            m_item = item;
            m_changeType = changeType;
        }
    }

    public class ClearedEventArgs<T> : EventArgs
    {
        private IEnumerable<T> m_items;

        public IEnumerable<T> Items
        {
            get { return m_items; }
        }

        public ClearedEventArgs(IEnumerable<T> items)
        {
            m_items = items;
        }
    }

    public class GrandEveAttributes
    {
        private int[] m_values = new int[5] { 0, 0, 0, 0, 0 };

        public int this[EveAttribute attribute]
        {
            get { return m_values[(int)attribute]; }
            set { m_values[(int)attribute] = value; }
        }
    }

    public class GrandEveAttributeBonus
    {
        private string m_name;
        private EveAttribute m_attribute;
        private int m_amount;
        private bool m_manual = false;

        public string Name
        {
            get { return m_name; }
        }

        public EveAttribute EveAttribute
        {
            get { return m_attribute; }
        }

        public int Amount
        {
            get { return m_amount; }
        }

        public bool Manual
        {
            get { return m_manual; }
        }

        public GrandEveAttributeBonus(string name, EveAttribute attr, int amount)
            : this(name, attr, amount, false)
        {
        }

        public GrandEveAttributeBonus(string name, EveAttribute attr, int amount, bool manual)
        {
            m_name = name;
            m_attribute = attr;
            m_amount = amount;
            m_manual = manual;
        }
    }
}
