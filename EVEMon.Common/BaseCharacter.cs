using System;
using System.Collections.Generic;
using EVEMon.Common.Data;

namespace EVEMon.Common
{
    public abstract class BaseCharacter : IDisposable
    {
        #region Abstract methods and properties

        protected abstract int TotalSkillPoints { get; }
        protected abstract ICharacterAttribute GetAttribute(EveAttribute attribute);

        public abstract int GetSkillLevel(StaticSkill skill);
        public abstract int GetSkillPoints(StaticSkill skill);

        public abstract void Dispose();

        #endregion


        #region Computation methods

        /// <summary>
        /// Gets the total skill points for this character.
        /// </summary>
        public int SkillPoints
        {
            get { return TotalSkillPoints; }
        }

        /// <summary>
        /// Computes the SP per hour for the given skill, without factoring out the newbies bonus.
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        public float GetBaseSPPerHour(StaticSkill skill)
        {
            if (skill == null)
                throw new ArgumentNullException("skill");

            if (skill.PrimaryAttribute == EveAttribute.None || skill.SecondaryAttribute == EveAttribute.None)
                return 0.0f;

            float primAttr = GetAttribute(skill.PrimaryAttribute).EffectiveValue;
            float secondaryAttr = GetAttribute(skill.SecondaryAttribute).EffectiveValue;
            return primAttr * 60.0f + secondaryAttr * 30.0f;
        }

        /// <summary>
        /// Gets a character scratchpad representing this character after the provided skill levels trainings.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="trainings"></param>
        /// <returns></returns>
        public CharacterScratchpad After<T>(IEnumerable<T> trainings)
            where T : ISkillLevel
        {
            CharacterScratchpad scratchpad = new CharacterScratchpad(this);
            scratchpad.Train(trainings);
            return scratchpad;
        }

        /// <summary>
        /// Gets a character scratchpad representing this character after a switch to the provided implant set.
        /// </summary>
        /// <param name="set">The set.</param>
        /// <returns></returns>
        public CharacterScratchpad After(ImplantSet set)
        {
            if (set == null)
                throw new ArgumentNullException("set");

            CharacterScratchpad scratchpad = new CharacterScratchpad(this);
            for (int i = 0; i < 5; i++)
            {
                EveAttribute attribute = (EveAttribute)i;
                scratchpad[attribute].ImplantBonus = set[attribute].Bonus;
            }
            return scratchpad;
        }

        /// <summary>
        /// Gets the time span for a specific number of skill points.
        /// </summary>
        /// <param name="points">The points to calculate points.</param>
        /// <param name="skill">The skill to train.</param>
        /// <returns></returns>
        public TimeSpan GetTimeSpanForPoints(StaticSkill skill, int points)
        {
            return GetTrainingTime(points, GetBaseSPPerHour(skill));
        }

        #endregion


        #region GetSPToTrain

        /// <summary>
        /// Computes the number of SP to train.
        /// </summary>
        /// <param name="skillLevel"></param>
        /// <returns></returns>
        public int GetSPToTrain(ISkillLevel skillLevel)
        {
            if (skillLevel == null)
                throw new ArgumentNullException("skillLevel");

            return GetSPToTrain(skillLevel.Skill, skillLevel.Level);
        }

        /// <summary>
        /// Computes the number of SP to train.
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="level"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        private int GetSPToTrain(StaticSkill skill, int level, TrainingOrigin origin = TrainingOrigin.FromCurrent)
        {
            if (level == 0)
                return 0;
            int sp = skill.GetPointsRequiredForLevel(level);

            // Deals with the origin
            int result;
            switch (origin)
            {
                    // Include current SP
                case TrainingOrigin.FromCurrent:
                    result = sp - GetSkillPoints(skill);
                    break;

                    // This level only (previous are known)
                case TrainingOrigin.FromPreviousLevel:
                    result = sp - skill.GetPointsRequiredForLevel(level - 1);
                    break;

                case TrainingOrigin.FromPreviousLevelOrCurrent:
                    result = sp - Math.Max(GetSkillPoints(skill), skill.GetPointsRequiredForLevel(level - 1));
                    break;

                    // Include nothing
                default:
                    result = sp;
                    break;
            }

            // Returns result
            return result < 0 ? 0 : result;
        }

        #endregion


        #region GetTrainingTime & GetTrainingTimeToMultipleSkills

        /// <summary>
        /// Computes the training time for the given skill.
        /// </summary>
        /// <param name="skillLevel"></param>
        /// <returns></returns>
        public TimeSpan GetTrainingTime(ISkillLevel skillLevel)
        {
            if (skillLevel == null)
                throw new ArgumentNullException("skillLevel");

            return GetTrainingTime(skillLevel.Skill, skillLevel.Level);
        }

        /// <summary>
        /// Computes the training time for the given skill.
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="level"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public TimeSpan GetTrainingTime(StaticSkill skill, int level, TrainingOrigin origin = TrainingOrigin.FromCurrent)
        {
            float spPerHour = GetBaseSPPerHour(skill);
            int sp = GetSPToTrain(skill, level, origin);
            return GetTrainingTime(sp, spPerHour);
        }

        /// <summary>
        /// Gets the time to train the given SP at the provided speed.
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="spPerHour"></param>
        /// <returns></returns>
        private static TimeSpan GetTrainingTime(int sp, float spPerHour)
        {
            return Math.Abs(spPerHour) < float.Epsilon ? TimeSpan.FromDays(999.0) : TimeSpan.FromHours(sp / spPerHour);
        }

        /// <summary>
        /// Gets the time require to train the given skills and their prerequisites.
        /// </summary>
        /// <param name="trainings">A sequence of pairs of skills and the target levels.</param>
        /// <returns></returns>
        public TimeSpan GetTrainingTimeToMultipleSkills(IEnumerable<ISkillLevel> trainings)
        {
            CharacterScratchpad scratchpad = After(trainings);
            return scratchpad.TrainingTime;
        }

        #endregion


        #region ICharacter non-abstract explicit members

        /// <summary>
        /// Gets the <see cref="EVEMon.Common.ICharacterAttribute"/> with the specified attribute.
        /// </summary>
        /// <value></value>
        public ICharacterAttribute this[EveAttribute attribute]
        {
            get { return GetAttribute(attribute); }
        }

        /// <summary>
        /// Gets the intelligence.
        /// </summary>
        /// <value>The intelligence.</value>
        public ICharacterAttribute Intelligence
        {
            get { return GetAttribute(EveAttribute.Intelligence); }
        }

        /// <summary>
        /// Gets the perception.
        /// </summary>
        /// <value>The perception.</value>
        public ICharacterAttribute Perception
        {
            get { return GetAttribute(EveAttribute.Perception); }
        }

        /// <summary>
        /// Gets the willpower.
        /// </summary>
        /// <value>The willpower.</value>
        public ICharacterAttribute Willpower
        {
            get { return GetAttribute(EveAttribute.Willpower); }
        }

        /// <summary>
        /// Gets the charisma.
        /// </summary>
        /// <value>The charisma.</value>
        public ICharacterAttribute Charisma
        {
            get { return GetAttribute(EveAttribute.Charisma); }
        }

        /// <summary>
        /// Gets the memory.
        /// </summary>
        /// <value>The memory.</value>
        public ICharacterAttribute Memory
        {
            get { return GetAttribute(EveAttribute.Memory); }
        }

        #endregion
    }
}