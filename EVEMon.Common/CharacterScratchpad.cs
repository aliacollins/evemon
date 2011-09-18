using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Data;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents a character class designed for computations and temporary modifications (new implants, remaps, etc)
    /// </summary>
    public sealed class CharacterScratchpad : BaseCharacter
    {
        private readonly CharacterAttributeScratchpad[] m_attributes = new CharacterAttributeScratchpad[5];
        private readonly int[] m_skillLevels;
        private readonly int[] m_skillSP;

        /// <summary>
        /// Constructor from a character
        /// </summary>
        /// <param name="character"></param>
        public CharacterScratchpad(BaseCharacter character)
        {
            TrainedSkills = new List<StaticSkillLevel>();
            TrainingTime = TimeSpan.Zero;
            Character = character;
            m_skillSP = new int[StaticSkills.ArrayIndicesCount];
            m_skillLevels = new int[StaticSkills.ArrayIndicesCount];

            for (int i = 0; i < m_attributes.Length; i++)
            {
                m_attributes[i] = new CharacterAttributeScratchpad((EveAttribute)i);
            }

            Reset();
        }


        #region Core properties

        /// <summary>
        /// Gets the character used to build this scratchpad
        /// </summary>
        public BaseCharacter Character { get; private set; }

        /// <summary>
        /// Gets or sets the total SP.
        /// </summary>
        public new int SkillPoints { get; set; }

        #endregion


        #region Attributes

        /// <summary>
        /// Gets the intelligence of the character.
        /// </summary> 
        public new CharacterAttributeScratchpad Intelligence
        {
            get { return m_attributes[(int)EveAttribute.Intelligence]; }
        }

        /// <summary>
        /// Gets the charisma of the character.
        /// </summary> 
        public new CharacterAttributeScratchpad Charisma
        {
            get { return m_attributes[(int)EveAttribute.Charisma]; }
        }

        /// <summary>
        /// Gets the perception of the character.
        /// </summary> 
        public new CharacterAttributeScratchpad Perception
        {
            get { return m_attributes[(int)EveAttribute.Perception]; }
        }

        /// <summary>
        /// Gets the memory of the character.
        /// </summary> 
        public new CharacterAttributeScratchpad Memory
        {
            get { return m_attributes[(int)EveAttribute.Memory]; }
        }

        /// <summary>
        /// Gets the willpower of the character.
        /// </summary> 
        public new CharacterAttributeScratchpad Willpower
        {
            get { return m_attributes[(int)EveAttribute.Willpower]; }
        }

        /// <summary>
        /// Gets the specified attribute.
        /// </summary>
        /// <param name="attribute">The attribute to retrieve</param>
        /// <returns></returns>
        public new CharacterAttributeScratchpad this[EveAttribute attribute]
        {
            get { return m_attributes[(int)attribute]; }
        }

        /// <summary>
        /// Performs the given remapping
        /// </summary>
        /// <param name="point"></param>
        public void Remap(RemappingPoint point)
        {
            for (int i = 0; i < m_attributes.Length; i++)
            {
                EveAttribute attrib = (EveAttribute)i;
                m_attributes[i].Base = point[attrib];
            }
        }

        /// <summary>
        /// Remove all the implants
        /// </summary>
        public void ClearImplants()
        {
            foreach (CharacterAttributeScratchpad attribute in m_attributes)
            {
                attribute.ImplantBonus = 0;
            }
        }

        #endregion


        #region Overriden methods

        /// <summary>
        /// Gets the total skill points.
        /// </summary>
        /// <returns></returns>
        protected override int TotalSkillPoints
        {
            get { return SkillPoints; }
        }

        /// <summary>
        /// Gets the current level of the given skill.
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        public override int GetSkillLevel(StaticSkill skill)
        {
            return m_skillLevels[skill.ArrayIndex];
        }

        /// <summary>
        /// Gets the current level of the given skill.
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        public override int GetSkillPoints(StaticSkill skill)
        {
            return m_skillSP[skill.ArrayIndex];
        }

        /// <summary>
        /// Gets the requested attribute.
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        protected override ICharacterAttribute GetAttribute(EveAttribute attribute)
        {
            return m_attributes[(int)attribute];
        }

        #endregion


        #region Training and skill levels updates

        /// <summary>
        /// Gets or sets the total training time. Note the training time is always zero when you create a scratchpad from a character.
        /// </summary>
        public TimeSpan TrainingTime { get; private set; }

        /// <summary>
        /// Gets the list of skills trained so far (by the <see cref="Train&lt;T&gt;"/> or <see cref="SetSkillLevel"/> methods).
        /// </summary>
        public List<StaticSkillLevel> TrainedSkills { get; private set; }

        /// <summary>
        /// Clears the training time and trained skills only. 
        /// Does not remove the benefits from those skills (use <see cref="Reset()"/> for that purpose).
        /// </summary>
        public void ClearTraining()
        {
            TrainingTime = TimeSpan.Zero;
            TrainedSkills.Clear();
        }

        /// <summary>
        /// Performs the given training.
        /// Rely on <see cref="SetSkillLevel"/> but only applied when the given level is greater than the current one.
        /// </summary>
        /// <param name="trainings"></param>
        public void Train<T>(IEnumerable<T> trainings)
            where T : ISkillLevel
        {
            foreach (T item in trainings)
            {
                Train(item.Skill, item.Level);
            }
        }

        /// <summary>
        /// Performs the given training, also apply remapping points.
        /// Rely on <see cref="SetSkillLevel"/> but only applied when the given level is greater than the current one.
        /// </summary>
        /// <param name="entries"></param>
        /// <param name="applyRemappingPoints"></param>
        public void TrainEntries(IEnumerable<PlanEntry> entries, bool applyRemappingPoints)
        {
            foreach (PlanEntry entry in entries)
            {
                if (entry.Remapping != null && entry.Remapping.Status == RemappingPointStatus.UpToDate &&
                    applyRemappingPoints)
                    Remap(entry.Remapping);

                Train(entry.Skill, entry.Level);
            }
        }

        /// <summary>
        /// Performs the given training. Same as <see cref="SetSkillLevel"/> but only applied when the given level is greater than the current one.
        /// </summary>
        /// <param name="training"></param>
        public void Train(ISkillLevel training)
        {
            Train(training.Skill, training.Level);
        }

        /// <summary>
        /// Performs the given training. Same as <see cref="SetSkillLevel"/> but only applied when the given level is greater than the current one.
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="level"></param>
        public void Train(StaticSkill skill, int level)
        {
            SetSkillLevel(skill, level, LearningOptions.UpgradeOnly);
        }

        /// <summary>
        /// Changes the level of the provided skill, updating the results
        /// </summary>
        /// <param name="skill">The skill.</param>
        /// <param name="level">The level.</param>
        /// <param name="options">The options.</param>
        private void SetSkillLevel(StaticSkill skill, int level, LearningOptions options = LearningOptions.None)
        {
            int index = skill.ArrayIndex;

            // May quit for if this level is alread equal (or greater, depending on the options)
            if ((options & LearningOptions.UpgradeOnly) != LearningOptions.None)
            {
                if (m_skillLevels[index] >= level)
                    return;
            }
            else
            {
                if (m_skillLevels[index] == level)
                    return;
            }

            // Update prerequisites
            if ((options & LearningOptions.IgnorePrereqs) == LearningOptions.None)
            {
                // Deal with recursive prereqs (like Polaris)
                foreach (StaticSkillLevel prereq in skill.Prerequisites.Where(prereq => prereq.Skill != skill))
                {
                    // Set the prereq's level
                    SetSkillLevel(prereq.Skill, prereq.Level, options | LearningOptions.UpgradeOnly);
                }
            }

            // Update training time
            if ((options & LearningOptions.IgnoreTraining) == LearningOptions.None)
            {
                TrainingTime += GetTrainingTime(skill, level);
                TrainedSkills.Add(new StaticSkillLevel(skill, level));
            }

            // Update skillpoints
            if ((options & LearningOptions.FreezeSP) == LearningOptions.None)
                UpdateSP(skill, level);

            // Updates the skill level
            m_skillLevels[index] = level;
        }

        /// <summary>
        /// Updates the total SP count for this character
        /// </summary>
        /// <param name="staticSkill"></param>
        /// <param name="level"></param>
        private void UpdateSP(StaticSkill staticSkill, int level)
        {
            int targetSP = staticSkill.GetPointsRequiredForLevel(level);
            int difference = targetSP - m_skillSP[staticSkill.ArrayIndex];

            m_skillSP[staticSkill.ArrayIndex] = targetSP;
            SkillPoints += difference;
        }

        #endregion


        #region Cloning, reseting, temporary changes

        /// <summary>
        /// Clear all the skills
        /// </summary>
        public void ClearSkills()
        {
            for (int i = 0; i < m_skillSP.Length; i++)
            {
                m_skillLevels[i] = 0;
            }

            for (int i = 0; i < m_skillLevels.Length; i++)
            {
                m_skillLevels[i] = 0;
            }

            SkillPoints = 0;
            TrainingTime = TimeSpan.Zero;

            foreach (CharacterAttributeScratchpad attribute in m_attributes)
            {
                attribute.UpdateEffectiveAttribute();
            }
        }

        /// <summary>
        /// Returns a clone of this scratchpad
        /// </summary>
        /// <returns></returns>
        public CharacterScratchpad Clone()
        {
            return new CharacterScratchpad(this);
        }

        /// <summary>
        /// Resets the scratchpad from the <see cref="BaseCharacter"/> it was built upon.
        /// </summary>
        public void Reset()
        {
            if (Character is CharacterScratchpad)
                Reset((CharacterScratchpad)Character);
            else
                ResetFromCharacter();
        }

        /// <summary>
        /// Resets this scratchpad using the provided scratchpad.
        /// </summary>
        /// <param name="scratchpad"></param>
        private void Reset(CharacterScratchpad scratchpad)
        {
            SkillPoints = scratchpad.SkillPoints;
            TrainingTime = scratchpad.TrainingTime;

            TrainedSkills.Clear();
            TrainedSkills.AddRange(scratchpad.TrainedSkills);

            for (int i = 0; i < m_attributes.Length; i++)
            {
                m_attributes[i].Reset(scratchpad.m_attributes[i]);
            }

            scratchpad.m_skillSP.CopyTo(m_skillSP, 0);
            scratchpad.m_skillLevels.CopyTo(m_skillLevels, 0);
        }

        /// <summary>
        /// Resets the scratchpad from the <see cref="BaseCharacter"/> it was built upon.
        /// </summary>
        private void ResetFromCharacter()
        {
            TrainingTime = TimeSpan.Zero;
            TrainedSkills.Clear();

            // Initialize attributes-related stuff
            for (int i = 0; i < m_attributes.Length; i++)
            {
                ICharacterAttribute attrib = Character[(EveAttribute)i];
                m_attributes[i].Reset(attrib.Base, attrib.ImplantBonus);
            }

            // Initialize skills
            SkillPoints = 0;
            foreach (StaticSkill skill in StaticSkills.AllSkills)
            {
                int sp = Character.GetSkillPoints(skill);
                int level = Character.GetSkillLevel(skill);

                SkillPoints += sp;
                m_skillSP[skill.ArrayIndex] = sp;
                m_skillLevels[skill.ArrayIndex] = level;
            }
        }

        /// <summary>
        /// Allow to store the state of this scratchpad to restore it later.
        /// The method returns an <see cref="IDisposable"/> object which, once disposed, will restore the state.
        /// </summary>
        /// <remarks>Use it in a <c>using</c> block to enforce an automatic restoration of the object even when exceptions are throw.</remarks>
        /// <returns>A disposable object which, once disposed, will restore the state of the </returns>
        public IDisposable BeginTemporaryChanges()
        {
            CharacterScratchpad clone = Clone();
            return new DisposableWithCallback(() => Reset(clone));
        }

        #endregion
    }
}