using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private readonly Int64[] m_skillLevels;
        private readonly Int64[] m_skillSP;

        private readonly BaseCharacter m_character;
        private Int64 m_skillPoints;

        /// <summary>
        /// Constructor from a character.
        /// </summary>
        /// <param name="character"></param>
        public CharacterScratchpad(BaseCharacter character)
        {
            TrainedSkills = new Collection<StaticSkillLevel>();
            TrainingTime = TimeSpan.Zero;
            m_character = character;
            m_skillSP = new Int64[StaticSkills.ArrayIndicesCount];
            m_skillLevels = new Int64[StaticSkills.ArrayIndicesCount];

            for (int i = 0; i < m_attributes.Length; i++)
            {
                m_attributes[i] = new CharacterAttributeScratchpad((EveAttribute)i);
            }

            Reset();
        }


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
            if (point == null)
                throw new ArgumentNullException("point");

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
        protected override Int64 TotalSkillPoints
        {
            get { return m_skillPoints; }
        }

        /// <summary>
        /// Gets the current level of the given skill.
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        public override Int64 GetSkillLevel(StaticSkill skill)
        {
            if (skill == null)
                throw new ArgumentNullException("skill");

            return m_skillLevels[skill.ArrayIndex];
        }

        /// <summary>
        /// Gets the current level of the given skill.
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        public override Int64 GetSkillPoints(StaticSkill skill)
        {
            if (skill == null)
                throw new ArgumentNullException("skill");

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

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        internal override void Dispose()
        {
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
        public Collection<StaticSkillLevel> TrainedSkills { get; private set; }

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
            if (trainings == null)
                throw new ArgumentNullException("trainings");

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
            if (entries == null)
                throw new ArgumentNullException("entries");

            foreach (PlanEntry entry in entries)
            {
                if (entry.Remapping != null && entry.Remapping.Status == RemappingPointStatus.UpToDate &&
                    applyRemappingPoints)
                    Remap(entry.Remapping);

                Train(entry.Skill, entry.Level);
            }
        }

        /// <summary>
        /// Performs the given training.
        /// Same as <see cref="SetSkillLevel"/> but only applied when the given level is greater than the current one.
        /// </summary>
        /// <param name="training"></param>
        public void Train(ISkillLevel training)
        {
            if (training == null)
                throw new ArgumentNullException("training");

            Train(training.Skill, training.Level);
        }

        /// <summary>
        /// Performs the given training. Same as <see cref="SetSkillLevel"/> but only applied when the given level is greater than the current one.
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="level"></param>
        public void Train(StaticSkill skill, Int64 level)
        {
            SetSkillLevel(skill, level, LearningOptions.UpgradeOnly);
        }

        /// <summary>
        /// Changes the level of the provided skill, updating the results.
        /// </summary>
        /// <param name="skill">The skill.</param>
        /// <param name="level">The level.</param>
        /// <param name="options">The options.</param>
        private void SetSkillLevel(StaticSkill skill, Int64 level, LearningOptions options = LearningOptions.None)
        {
            int index = skill.ArrayIndex;

            if (index > m_skillLevels.Length)
                return;

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
        private void UpdateSP(StaticSkill staticSkill, Int64 level)
        {
            Int64 targetSP = staticSkill.GetPointsRequiredForLevel(level);
            Int64 difference = targetSP - m_skillSP[staticSkill.ArrayIndex];

            m_skillSP[staticSkill.ArrayIndex] = targetSP;
            m_skillPoints += difference;
        }

        #endregion


        #region Cloning, reseting, temporary changes

        /// <summary>
        /// Clear all the skills.
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

            m_skillPoints = 0;
            TrainingTime = TimeSpan.Zero;

            foreach (CharacterAttributeScratchpad attribute in m_attributes)
            {
                attribute.UpdateEffectiveAttribute();
            }
        }

        /// <summary>
        /// Returns a clone of this scratchpad.
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
            CharacterScratchpad character = m_character as CharacterScratchpad;
            if (character != null)
                Reset(character);
            else
                ResetFromCharacter();
        }

        /// <summary>
        /// Resets this scratchpad using the provided scratchpad.
        /// </summary>
        /// <param name="scratchpad"></param>
        private void Reset(CharacterScratchpad scratchpad)
        {
            m_skillPoints = scratchpad.m_skillPoints;
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
                ICharacterAttribute attrib = m_character[(EveAttribute)i];
                m_attributes[i].Reset(attrib.Base, attrib.ImplantBonus);
            }

            // Initialize skills
            m_skillPoints = 0;
            foreach (StaticSkill skill in StaticSkills.AllSkills)
            {
                Int64 sp = m_character.GetSkillPoints(skill);
                Int64 level = m_character.GetSkillLevel(skill);

                m_skillPoints += sp;
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