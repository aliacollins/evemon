using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;

namespace EVEMon.Common
{
    public class Skill
    {
        private StaticSkill m_staticData;
        private CharacterInfo m_owner;
        private IEnumerable<Prereq> m_prereqs;
        private int m_currentSkillPoints;
        private int m_lastConfirmedLvl;
        bool m_owned;

        public Skill(CharacterInfo _gci, StaticSkill _ss, bool _owned, IEnumerable<Prereq> _prereqs)
        {
            m_staticData = _ss;
            m_owner = _gci;
            m_lastConfirmedLvl = 0;
            m_prereqs = _prereqs;
            m_owned = _owned;
            m_highlightPartials = false;
        }

        public void SetOwner(CharacterInfo gci)
        {
            m_owner = gci;
        }

        /// <summary>
        /// Gets or sets the most recent skill level that was confirmed from the CCP server or an XML file.
        /// </summary>
        public int LastConfirmedLvl
        {
            get { return m_lastConfirmedLvl; }
            set { m_lastConfirmedLvl = value; }
        }

        private SkillGroup m_skillGroup;

        /// <summary>
        /// Gets the skill group this skill is part of.
        /// </summary>
        public SkillGroup SkillGroup
        {
            get { return m_skillGroup; }
        }

        internal void SetSkillGroup(SkillGroup gsg)
        {
            if (m_skillGroup != null)
            {
                throw new InvalidOperationException("can only set skillgroup once");
            }

            m_skillGroup = gsg;
        }

        /// <summary>
        /// Gets the current skill points of this skill.
        /// </summary>
        public int CurrentSkillPoints
        {
            get
            {
                if (m_inTraining)
                {
                    TimeSpan timeRemainSpan = m_trainingSkillInfo.getTrainingEndTime.ToLocalTime().Subtract(DateTime.Now);
                    if (timeRemainSpan <= TimeSpan.Zero)
                    {
                        return GetPointsRequiredForLevel(m_trainingToLevel);
                    }
                    return m_trainingSkillInfo.EstimatedCurrentPoints;
                }
                else
                {
                    return m_currentSkillPoints;
                }
            }
            set
            {
                if (m_currentSkillPoints != value)
                {
                    m_currentSkillPoints = value;
                    OnChanged();
                }
            }
        }

        ///<summary>
        ///Gets the unadjusted skillpoints
        ///</summary>
        public int UnadjustedCurrentSkillPoints
        {
            get { return m_currentSkillPoints; }
        }

        /// <summary>
        /// Gets whether this skill is a learning skill.
        /// </summary>
        public bool IsLearningSkill
        {
            get { return (this.AttributeModified != EveAttribute.None); }
        }

        /// <summary>
        /// Gets which attribute this skill modifies.
        /// </summary>
        public EveAttribute AttributeModified
        {
            get { return m_staticData.AttributeModified; }
        }

        private bool m_known = false;

        /// <summary>
        /// Gets whether this skill is known.
        /// </summary>
        public bool Known
        {
            get { return m_known; }
            set
            {
                if (m_known != value)
                {
                    m_known = value;
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Calculate the amount of skill points that is trained during a certain timespan.
        /// </summary>
        /// <param name="span">The timespan.</param>
        /// <returns>Amount of skill points.</returns>
        public int GetPointsForTimeSpan(TimeSpan span)
        {
            // m = points/(primAttr+(secondaryAttr/2))
            // ... so ...
            // m * (primAttr+(secondaryAttr/2)) = points

            return GetPointsForTimeSpan(span, null);
        }

        public int GetPointsForTimeSpan(TimeSpan span, EveAttributeScratchpad scratchpad)
        {
            double primAttr = m_owner.GetEffectiveAttribute(m_staticData.PrimaryAttribute, scratchpad);
            double secondaryAttr = m_owner.GetEffectiveAttribute(m_staticData.SecondaryAttribute, scratchpad);
            double points = span.TotalMinutes * (primAttr + (secondaryAttr / 2));
            return Convert.ToInt32(Math.Ceiling(points));
        }

        /// <summary>
        /// Calculate the time it will take to train a certain amount of skill points.
        /// </summary>
        /// <remarks>
        /// Note: This does not take into account the attribute increase
        /// for each level of the learning skills!
        /// </remarks>
        /// <param name="points">The amount of skill points.</param>
        /// <returns>Time it will take.</returns>
        private TimeSpan GetTimeSpanForPoints(int points)
        {
            return GetTimeSpanForPoints(points, this.m_owner.SkillPointTotal, null, true);
        }

        /// <summary>
        /// Gets the time span for a specific number of skill points.
        /// </summary>
        /// <param name="points">The points to calculate points.</param>
        /// <param name="skillPointTotal">Current skill point total.</param>
        /// <param name="scratchpad">The EVE Attribute Scratchpad.</param>
        /// <param name="includeImplants">if set to <c>true</c> include implants.</param>
        /// <returns></returns>
        private TimeSpan GetTimeSpanForPoints(int points, int skillPointTotal, EveAttributeScratchpad scratchpad, Boolean includeImplants)
        {
            double primAttr = m_owner.GetEffectiveAttribute(m_staticData.PrimaryAttribute, scratchpad, true, includeImplants);
            double secondaryAttr = m_owner.GetEffectiveAttribute(m_staticData.SecondaryAttribute, scratchpad, true, includeImplants);
            double minutes = Convert.ToDouble(points) / (primAttr + (secondaryAttr / 2));
            double newCharacterTrainingBonus = GetNewCharacterSkillTrainingBonus(skillPointTotal, points);

            return TimeSpan.FromMinutes(minutes / newCharacterTrainingBonus);
        }

        /// <summary>
        /// Returns the skill training bonus based upon the total number of  skill points and  the number of  points to train the skill.
        /// </summary>
        /// <remarks>
        /// As with Apocrypha 1.0 (10 March 2008) a 100% skill training bonus is  applied to characters  with less than  1.6m skill points.
        /// </remarks>
        /// <param name="skillPointTotal">The total number of skill points.</param>
        /// <param name="skillPoints">The total  number of  points to  train this  skill from the current level to the next.</param>
        /// <returns>Double between 1.0 (no  bonus  applied) to 2.0 (bonus applied), in  the event that 1.6m SP is  passed  during training of the current skill a number between 1.0 and 2.0 will be returned.</returns>
        public double GetNewCharacterSkillTrainingBonus(int skillPointTotal, int pointsForThisSkill)
        {
            double newCharacterMultiplier = 1;

            if (skillPointTotal < EveConstants.NewCharacterTrainingThreshold)
            {
                if ((skillPointTotal + pointsForThisSkill) > EveConstants.NewCharacterTrainingThreshold)
                {
                    int pointsWithoutBonus = (skillPointTotal + pointsForThisSkill) - EveConstants.NewCharacterTrainingThreshold;
                    int pointsWithBonus = pointsForThisSkill - pointsWithoutBonus;

                    // ((pointsWithoutBonus * 1.0) + (pointsWithBonus * EveConstants.NewCharacterTrainingFactor)) / pointsForThisSkill;
                    // balances down to...
                    newCharacterMultiplier = ((double)pointsWithBonus / pointsForThisSkill) + 1;
                }
                else
                {
                    newCharacterMultiplier = EveConstants.NewCharacterTrainingFactor;
                }
            }
                
            return newCharacterMultiplier;
        }

        /// <summary>
        /// Calculates the points required for a level of this skill.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>The required nr. of points.</returns>
        public int GetPointsRequiredForLevel(int level)
        {
            return m_staticData.GetPointsRequiredForLevel(level);
        }

        /// <summary>
        /// Calculates the percentage trained (in terms of skill points) to the next level of this skill.
        /// If the skill is already at level 5, we return 100.0.
        /// </summary>
        /// <returns>Percentage of skill points to the next level that have already been trained.</returns>
        public double GetPercentDone()
        {
            if (Level == 5) return 100.0;

            int reqToThisLevel = GetPointsRequiredForLevel(Level);
            int pointsInThisLevel = CurrentSkillPoints - reqToThisLevel;
            int reqToNextLevel = GetPointsRequiredForLevel(Level + 1);
            double deltaPointsOfLevel = Convert.ToDouble(reqToNextLevel - reqToThisLevel);
            return pointsInThisLevel / deltaPointsOfLevel;
        }

        public int Id
        {
            get { return m_staticData.Id; }
        }

        public string Name
        {
            get { return m_staticData.Name; }
        }

        public string Description
        {
            get { return m_staticData.Description; }
        }

        public bool Public
        {
            get { return m_staticData.Public; }
        }

        public string DescriptionNl
        {
            get { return m_staticData.DescriptionNl; }
        }


        public long Cost
        {
            get { return m_staticData.Cost; }
        }

        public string FormattedCost
        {
            get { return m_staticData.FormattedCost; }
        }

        public bool Owned
        {
            get { return m_owned; }
            set { m_owned = value; }
        }

        private bool m_highlightPartials;
        public bool HighlightPartiallyTrained
        {
            get
            {
                return m_highlightPartials;
            }
            set
            {
                m_highlightPartials = value;
            }
        }

        /// <summary>
        /// Gets the primary attribute of this skill.
        /// </summary>
        public EveAttribute PrimaryAttribute
        {
            get { return m_staticData.PrimaryAttribute; }
        }

        /// <summary>
        /// Gets the secondary attribute of this skill.
        /// </summary>
        public EveAttribute SecondaryAttribute
        {
            get { return m_staticData.SecondaryAttribute; }
        }


        /// <summary>
        /// Gets the rank of this skill.
        /// </summary>
        public int Rank
        {
            get { return m_staticData.Rank; }
        }

        /// <summary>
        /// Gets the current level of this skill.
        /// </summary>
        public int Level
        {
            get
            {
                if (m_inTraining)
                {
                    return this.TrainingToLevel - 1;
                }
                int csp = this.CurrentSkillPoints;
                int result = 0;
                for (int i = 1; i <= 5; i++)
                {
                    if (GetPointsRequiredForLevel(i) <= csp)
                    {
                        result = i;
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Return current Level in Roman
        /// </summary>
        public string RomanLevel
        {
            get
            {
                return Skill.GetRomanForInt(this.Level);
            }
        }

        /// <summary>
        /// Gets whether this skill is partially trained (true) or fully trained (false).
        /// </summary>
        public bool PartiallyTrained
        {
            get
            {
                bool pt;
                if (Level == 0 && Known)
                {
                    pt = true;
                }
                else
                {
                    pt = CurrentSkillPoints > GetPointsRequiredForLevel(Level);
                }
                return pt;
            }
        }

        public IEnumerable<Prereq> Prereqs
        {
            get
            {
                return m_prereqs;
            }
        }

        private void OnChanged()
        {
            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        private void OnTrainingStatusChanged()
        {
            if (TrainingStatusChanged != null)
            {
                TrainingStatusChanged(this, new EventArgs());
            }
        }

        public event EventHandler Changed;
        public event EventHandler TrainingStatusChanged;

        public class Prereq : EntityRequiredSkill
        {
            private Skill m_pointedSkill;

            public Prereq()
            {
            }

            internal Prereq(StaticSkill.Prereq ssp)
            {
                _name = ssp.Name;
                _level = ssp.Level;
                m_pointedSkill = null;
            }


            public Skill Skill
            {
                get { return m_pointedSkill; }
            }

            internal void SetSkill(Skill gs)
            {
                if (m_pointedSkill != null)
                {
                    throw new InvalidOperationException("pointed skill can only be set once");
                }
                m_pointedSkill = gs;
            }

            internal Prereq(string name, int requiredLevel)
            {
                _name = name;
                m_pointedSkill = null;
                _level = requiredLevel;
            }
        }

        private bool m_inTraining = false;
        private int m_trainingToLevel = 0;
        private SerializableSkillTrainingInfo m_trainingSkillInfo = null;
        private DateTime m_estimatedCompletion = DateTime.MaxValue;


        /// <summary>
        /// Gets whether this skill is currently training.
        /// </summary>
        public bool InTraining
        {
            get { return m_inTraining; }
        }

        /// <summary>
        /// Gets the level this skill is training to.
        /// </summary>
        public int TrainingToLevel
        {
            get { return m_trainingToLevel; }
        }

        /// <summary>
        /// Gets the estimated time of completion.
        /// </summary>
        public DateTime EstimatedCompletion
        {
            get { return m_estimatedCompletion; }
        }

        internal void SetTrainingInfo(int trainingToLevel, DateTime estimatedCompletion, SerializableSkillTrainingInfo trainingSkill)
        {
            if (!m_inTraining || m_trainingToLevel != trainingToLevel)
            {
                OnTrainingStatusChanged();
            }
            m_inTraining = true;
            m_trainingToLevel = trainingToLevel;
            m_estimatedCompletion = estimatedCompletion;
            m_trainingSkillInfo = trainingSkill;
            OnChanged();
        }

        internal void StopTraining()
        {
            if (m_inTraining)
            {
                OnTrainingStatusChanged();
            }
            m_inTraining = false;
            m_trainingToLevel = 0;
            m_estimatedCompletion = DateTime.MaxValue;
            OnChanged();
        }

        /// <summary>
        /// Calculate the time to train this skill to a certain level.
        /// </summary>
        /// <param name="level">The level to calculate for.</param>
        /// <returns>Time it will take.</returns>
        public TimeSpan GetTrainingTimeToLevel(int level)
        {
            int currentSp = this.CurrentSkillPoints;
            int desiredSp = this.GetPointsRequiredForLevel(level);
            if (desiredSp <= currentSp)
            {
                return TimeSpan.Zero;
            }
            return this.GetTimeSpanForPoints(desiredSp - currentSp);
        }

        public TimeSpan GetTrainingTimeOfLevelOnly(int level)
        {
            return GetTrainingTimeOfLevelOnly(level, m_owner.SkillPointTotal, false);
        }

        public TimeSpan GetTrainingTimeOfLevelOnly(int level, bool includeCurrentSP, EveAttributeScratchpad scratchpad)
        {
            return GetTrainingTimeOfLevelOnly(level, m_owner.SkillPointTotal, includeCurrentSP, scratchpad);
        }

        public TimeSpan GetTrainingTimeOfLevelOnly(int level, int skillPointTotal)
        {
            return GetTrainingTimeOfLevelOnly(level, skillPointTotal, false);
        }

        public TimeSpan GetTrainingTimeOfLevelOnly(int level, int skillPointTotal, bool includeCurrentSP)
        {
            return GetTrainingTimeOfLevelOnly(level, skillPointTotal, includeCurrentSP, null);
        }

        public TimeSpan GetTrainingTimeOfLevelOnly(int level, int skillPointTotal, bool includeCurrentSP, EveAttributeScratchpad scratchpad)
        {
            return GetTrainingTimeOfLevelOnly(level, skillPointTotal, includeCurrentSP, scratchpad, true);
        }

        public TimeSpan GetTrainingTimeOfLevelOnly(int level, int skillPointTotal, bool includeCurrentSP, EveAttributeScratchpad scratchpad, Boolean includeImplants)
        {
            int pointsNeeded = GetPointsForLevelOnly(level, includeCurrentSP);
            return this.GetTimeSpanForPoints(pointsNeeded, skillPointTotal, scratchpad, includeImplants);
        }

        /// <summary>
        /// Calculate the time to train this skill to the next level including prerequisites.
        /// </summary>
        /// <returns>Time it will take</returns>
        public TimeSpan GetTrainingTimeToNextLevel()
        {
            return GetTrainingTimeToNextLevel(true);
        }

        /// <summary>
        /// Calculate the time to train this skill to the next level.
        /// </summary>
        /// <param name="includePrerequisites">Include prerequisites</param>
        /// <returns>Time it will take</returns>
        public TimeSpan GetTrainingTimeToNextLevel(bool includePrerequisites)
        {
            if (Level == 5)
                return TimeSpan.MaxValue;

            return GetPrerequisiteTime() + GetTrainingTimeToLevel(Level + 1);
        }

        public int GetPointsForLevelOnly(int level, bool includeCurrentSP)
        {
            int startSp = GetPointsRequiredForLevel(level - 1);
            int endSp = GetPointsRequiredForLevel(level);
            if (includeCurrentSP)
            {
                startSp = Math.Max(startSp, this.CurrentSkillPoints);
            }
            if (endSp <= startSp)
            {
                return 0;
            }
            else
            {
                return endSp - startSp;
            }
        }

        #region GetPrerequisiteTime overloads
        public TimeSpan GetPrerequisiteTime()
        {
            bool junk = false;
            return GetPrerequisiteTime(new Dictionary<Skill, int>(), ref junk);
        }

        public TimeSpan GetPrerequisiteTime(Dictionary<Skill, int> alreadyCountedList)
        {
            bool junk = false;
            return GetPrerequisiteTime(alreadyCountedList, ref junk);
        }

        public TimeSpan GetPrerequisiteTime(ref bool timeIsCurrentlyTraining)
        {
            return GetPrerequisiteTime(new Dictionary<Skill, int>(), ref timeIsCurrentlyTraining);
        }

        public TimeSpan GetPrerequisiteTime(Dictionary<Skill, int> alreadyCountedList,
                                            ref bool timeIsCurrentlyTraining)
        {
            TimeSpan result = TimeSpan.Zero;
            foreach (Prereq pp in this.Prereqs)
            {
                Skill gs = pp.Skill;
                if (gs.InTraining)
                {
                    timeIsCurrentlyTraining = true;
                }

                int fromPoints = gs.CurrentSkillPoints;
                int toPoints = gs.GetPointsRequiredForLevel(pp.Level);
                if (alreadyCountedList.ContainsKey(gs))
                {
                    fromPoints = alreadyCountedList[gs];
                }
                if (fromPoints < toPoints)
                {
                    result += gs.GetTimeSpanForPoints(toPoints - fromPoints);
                }
                alreadyCountedList[gs] = Math.Max(fromPoints, toPoints);

                if (pp.Skill.Id != this.Id)
                {
                    result += gs.GetPrerequisiteTime(alreadyCountedList, ref timeIsCurrentlyTraining);
                }
            }
            return result;
        }
        #endregion

        public bool PrerequisitesMet
        {
            get
            {
                foreach (Prereq pp in this.Prereqs)
                {
                    if (pp.Skill.Level < pp.Level)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Checks whether a certain skill is a prerequisite of this skill.
        /// The check is performed recursively through all prerequisites.
        /// </summary>
        /// <param name="gs">Skill to check.</param>
        /// <returns><code>true</code> if it is a prerequisite.</returns>
        public bool HasAsPrerequisite(Skill gs)
        {
            int neededLevel = 0;
            return HasAsPrerequisite(gs, ref neededLevel, true);
        }

        /// <summary>
        /// Checks whether a certain skill is an immediate prerequisite of this skill,
        /// and the level needed
        /// </summary>
        /// <param name="gs">Skill that may be an immediate prereq</param>
        /// <param name="neededLevel">needed level of skill</param>
        /// <returns>Skill gs is an immediate prereq of this skill</returns>
        public bool HasAsImmedPrereq(Skill gs, out int neededLevel)
        {
            neededLevel = 0;
            return HasAsPrerequisite(gs, ref neededLevel, false);
        }

        /// <summary>
        /// Checks whether a certain skill is a prerequisite of this skill, and what level it needs.
        /// The check is performed recursively through all prerequisites.
        /// </summary>
        /// <param name="gs">Skill to check.</param>
        /// <param name="neededLevel">The level that is needed. Out parameter.</param>
        /// <returns><code>true</code> if it is a prerequisite, needed level in <var>neededLevel</var> out parameter.</returns>
        public bool HasAsPrerequisite(Skill gs, out int neededLevel)
        {
            neededLevel = 0;
            return HasAsPrerequisite(gs, ref neededLevel, true);
        }

        /// <summary>
        /// Checks whether a certain skill is a prerequisite of this skill, and what level it needs.
        /// Find the highest level needed by searching entire prerequisite tree.
        /// </summary>
        /// <param name="gs">Skill to check.</param>
        /// <param name="neededLevel">The level that is needed. Out parameter.</param>
        /// <param name="recurse">Pass <code>true</code> to check recursively.</param>
        /// <returns><code>true</code> if it is a prerequisite, needed level in <var>neededLevel</var> out parameter.</returns>
        private bool HasAsPrerequisite(Skill gs, ref int neededLevel, bool recurse)
        {
            foreach (Prereq pp in this.Prereqs)
            {
                if (pp.Skill == gs)
                {
                    neededLevel = Math.Max(pp.Level, neededLevel);
                }

                if (recurse && neededLevel < 5 && pp.Skill.Id != this.Id) // check for neededLevel fixes recursuve akill bug (e.g polaris )
                {
                    pp.Skill.HasAsPrerequisite(gs, ref neededLevel, true);
                }
            }
            return (neededLevel > 0);
        }

        /// <summary>
        /// Convert a timespan into English text.
        /// </summary>
        /// <param name="ts">The timespan.</param>
        /// <param name="dto">Formatting options.</param>
        /// <returns>Timespan formatted as English text.</returns>
        public static string TimeSpanToDescriptiveText(TimeSpan ts, DescriptiveTextOptions dto)
        {
            StringBuilder sb = new StringBuilder();
            BuildDescriptiveFragment(sb, ts.Days, dto, "days");
            BuildDescriptiveFragment(sb, ts.Hours, dto, "hours");
            BuildDescriptiveFragment(sb, ts.Minutes, dto, "minutes");
            BuildDescriptiveFragment(sb, ts.Seconds, dto, "seconds");
            if (sb.Length == 0)
            {
                sb.Append("(none)");
            }
            return sb.ToString();
        }

        private static void BuildDescriptiveFragment(StringBuilder sb, int p, DescriptiveTextOptions dto, string dstr)
        {
            if (((dto & DescriptiveTextOptions.IncludeZeroes) == 0) && p == 0)
            {
                return;
            }
            if ((dto & DescriptiveTextOptions.IncludeCommas) != 0)
            {
                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }
            }
            sb.Append(p.ToString());
            if ((dto & DescriptiveTextOptions.SpaceText) != 0)
            {
                sb.Append(' ');
            }
            if ((dto & DescriptiveTextOptions.UppercaseText) != 0)
            {
                dstr = dstr.ToUpper();
            }
            if ((dto & DescriptiveTextOptions.FullText) != 0)
            {
                if (p == 1)
                {
                    dstr = dstr.Substring(0, dstr.Length - 1);
                }
                sb.Append(dstr);
            }
            else
            {
                sb.Append(dstr[0]);
            }
        }

        /// <summary>
        /// Converts an integer into a roman number.
        /// </summary>
        /// <param name="number">Number from 1 to 5.</param>
        /// <returns>Roman number string.</returns>
        public static string GetRomanForInt(int number)
        {
            switch (number)
            {
                case 1:
                    return "I";
                case 2:
                    return "II";
                case 3:
                    return "III";
                case 4:
                    return "IV";
                case 5:
                    return "V";
                default:
                    return "(none)";
            }
        }

        /// <summary>
        /// Converts a roman number into an integer.
        /// </summary>
        /// <param name="r">Roman number from I to V.</param>
        /// <returns>Integer number.</returns>
        public static int GetIntForRoman(string r)
        {
            switch (r)
            {
                case "I": return 1;
                case "II": return 2;
                case "III": return 3;
                case "IV": return 4;
                case "V": return 5;
                default: return 0;
            }
        }

        /// <summary>
        /// Returns the string representation of this skill (the name).
        /// </summary>
        /// <returns>The name of the skill.</returns>
        public override string ToString()
        {
            return this.Name;
        }

        #region Appearance in List Box
        // Region & text padding
        private const int PAD_TOP = 2;
        private const int PAD_LEFT = 6;
        private const int PAD_RIGHT = 7;
        private const int LINE_VPAD = 0;
        // Boxes
        private const int BOX_WIDTH = 57;
        private const int BOX_HEIGHT = 14;
        private const int SUBBOX_HEIGHT = 8;
        private const int BOX_HPAD = 6;
        private const int BOX_VPAD = 2;
        private const int SKILL_DETAIL_HEIGHT = 31;

        public static int Height
        {
            get
            {
                Font fontr = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
                return Math.Max(fontr.Height * 2 + PAD_TOP + LINE_VPAD + PAD_TOP, SKILL_DETAIL_HEIGHT);
            }
        }

        public void Draw(DrawItemEventArgs e)
        {
            Font fontr = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            Graphics g = e.Graphics;

            if (m_inTraining)
            {
                g.FillRectangle(Brushes.LightSteelBlue, e.Bounds);
            }
            else if ((e.Index % 2) == 0)
            {
                g.FillRectangle(Brushes.White, e.Bounds);
            }
            else
            {
                g.FillRectangle(Brushes.LightGray, e.Bounds);
            }

            using (Font boldf = new Font(fontr, FontStyle.Bold))
            {
                double percentComplete = 1.0f;
                if (this.Level == 0)
                {
                    int NextLevel = this.Level + 1;
                    percentComplete = Convert.ToDouble(m_currentSkillPoints) /
                                      Convert.ToDouble(GetPointsRequiredForLevel(NextLevel));
                }
                else if (this.Level < 5)
                {
                    int pointsToNextLevel = this.GetPointsRequiredForLevel(Math.Min(this.Level + 1, 5));
                    int pointsToThisLevel = this.GetPointsRequiredForLevel(this.Level);
                    int pointsDelta = pointsToNextLevel - pointsToThisLevel;
                    percentComplete = Convert.ToDouble(this.CurrentSkillPoints - pointsToThisLevel) /
                                      Convert.ToDouble(pointsDelta);
                }

                string skillName = this.Name + " " + GetRomanForInt(this.Level);
                string rankText = " (Rank " + this.Rank.ToString() + ")";
                string spText = "SP: " + this.CurrentSkillPoints.ToString("#,##0") + "/" +
                                this.GetPointsRequiredForLevel(Math.Min(this.Level + 1, 5)).ToString("#,##0");
                string levelText = "Level " + this.Level.ToString();
                string pctText = Math.Floor(percentComplete * 100).ToString("0") + "% Done";

                // Text
                Size skillNameSize =
                    TextRenderer.MeasureText(g, skillName, boldf, new Size(0, 0),
                                             TextFormatFlags.NoPadding | TextFormatFlags.NoClipping);
                Size levelTextSize =
                    TextRenderer.MeasureText(g, levelText, fontr, new Size(0, 0),
                                             TextFormatFlags.NoPadding | TextFormatFlags.NoClipping);
                Size pctTextSize =
                    TextRenderer.MeasureText(g, pctText, fontr, new Size(0, 0),
                                             TextFormatFlags.NoPadding | TextFormatFlags.NoClipping);

                Color highlightColor = Color.Black;
                if (m_highlightPartials)
                {
                    if (percentComplete > 0.0 && percentComplete < 1)
                    {
                        highlightColor = Color.Red;
                    }
                }
                TextRenderer.DrawText(g, skillName, boldf, new Point(e.Bounds.Left + PAD_LEFT, e.Bounds.Top + PAD_TOP),
                                      highlightColor);
                TextRenderer.DrawText(g, rankText, fontr,
                                       new Point(e.Bounds.Left + PAD_LEFT + skillNameSize.Width, e.Bounds.Top + PAD_TOP),
                                       highlightColor);
                TextRenderer.DrawText(g, spText, fontr,
                                      new Point(e.Bounds.Left + PAD_LEFT,
                                                e.Bounds.Top + PAD_TOP + skillNameSize.Height + LINE_VPAD), highlightColor);

                // Boxes
                g.DrawRectangle(Pens.Black,
                                new Rectangle(e.Bounds.Right - BOX_WIDTH - PAD_RIGHT, e.Bounds.Top + PAD_TOP, BOX_WIDTH,
                                              BOX_HEIGHT));
                int bWidth = (BOX_WIDTH - 4 - 3) / 5;
                for (int bn = 1; bn <= 5; bn++)
                {
                    Rectangle brect =
                        new Rectangle(e.Bounds.Right - BOX_WIDTH - PAD_RIGHT + 2 + (bWidth * (bn - 1)) + (bn - 1),
                                      e.Bounds.Top + PAD_TOP + 2, bWidth, BOX_HEIGHT - 3);
                    if (bn <= this.Level)
                    {
                        g.FillRectangle(Brushes.Black, brect);
                    }
                    else
                    {
                        g.FillRectangle(Brushes.DarkGray, brect);
                    }
                }

                // Percent Bar
                g.DrawRectangle(Pens.Black,
                                new Rectangle(e.Bounds.Right - BOX_WIDTH - PAD_RIGHT,
                                              e.Bounds.Top + PAD_TOP + BOX_HEIGHT + BOX_VPAD, BOX_WIDTH, SUBBOX_HEIGHT));
                Rectangle pctBarRect = new Rectangle(e.Bounds.Right - BOX_WIDTH - PAD_RIGHT + 2,
                                                     e.Bounds.Top + PAD_TOP + BOX_HEIGHT + BOX_VPAD + 2,
                                                     BOX_WIDTH - 3, SUBBOX_HEIGHT - 3);
                g.FillRectangle(Brushes.DarkGray, pctBarRect);
                int fillWidth = Convert.ToInt32(
                    Math.Round(Convert.ToDouble(pctBarRect.Width) * percentComplete));
                if (fillWidth > 0)
                {
                    Rectangle fillRect = new Rectangle(pctBarRect.X, pctBarRect.Y,
                                                       fillWidth, pctBarRect.Height);
                    g.FillRectangle(Brushes.Black, fillRect);
                }
                TextRenderer.DrawText(g, levelText, fontr,
                                      new Point(
                                          e.Bounds.Right - BOX_WIDTH - PAD_RIGHT - BOX_HPAD - levelTextSize.Width,
                                          e.Bounds.Top + PAD_TOP), Color.Black);
                TextRenderer.DrawText(g, pctText, fontr,
                                      new Point(e.Bounds.Right - BOX_WIDTH - PAD_RIGHT - BOX_HPAD - pctTextSize.Width,
                                                e.Bounds.Top + PAD_TOP + levelTextSize.Height + LINE_VPAD), Color.Black);
            }
        }
        #endregion
    }

    [Flags]
    public enum DescriptiveTextOptions
    {
        Default = 0,
        FullText = 1,
        UppercaseText = 2,
        SpaceText = 4,
        IncludeCommas = 8,
        IncludeZeroes = 16
    }

    public class EveAttributeScratchpad
    {
        private int m_learningLevelBonus;
        private int[] m_attributeBonuses = new int[5] { 0, 0, 0, 0, 0 };

        public int LearningLevelBonus
        {
            get { return m_learningLevelBonus; }
            set { m_learningLevelBonus = value; }
        }

        public void AdjustLearningLevelBonus(int adjustmentAmount)
        {
            m_learningLevelBonus += adjustmentAmount;
        }

        public void SetLearningLevelBonus(int level)
        {
            m_learningLevelBonus = level;
        }

        public int GetAttributeBonus(EveAttribute attribute)
        {
            return m_attributeBonuses[(int)attribute];
        }

        public int SetAttributeBonus(EveAttribute attribute, int value)
        {
            return m_attributeBonuses[(int)attribute] = value;
        }

        public void AdjustAttributeBonus(EveAttribute attribute, int adjustmentAmount)
        {
            m_attributeBonuses[(int)attribute] += adjustmentAmount;
        }

        public void ApplyALevelOf(Skill gs)
        {
            if (gs.Name == "Learning")
            {
                this.AdjustLearningLevelBonus(1);
            }
            else if (gs.IsLearningSkill)
            {
                this.AdjustAttributeBonus(gs.AttributeModified, 1);
            }
        }
    }
}
