using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EVEMon.Common.Attributes;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents a skill bound to a character, not only including the skill static data (represented by <see cref="StaticSkill" />) but also the number of SP and level the character has. 
    /// </summary>
    [EnforceUIThreadAffinity]
    public sealed class Skill : IStaticSkill
    {
        private readonly List<SkillLevel> m_prereqs = new List<SkillLevel>();
        private int m_currentSkillPoints;
        private int m_skillLevel;
        private int m_level;
        private bool m_owned;
        private bool m_known;


        #region Construction, initialization, exportation, updates

        /// <summary>
        /// Internal constructor, only used for character creation and updates
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="group"></param>
        /// <param name="skill"></param>
        internal Skill(Character owner, SkillGroup group, StaticSkill skill)
        {
            Character = owner;
            StaticData = skill;
            Group = group;
        }

        /// <summary>
        /// Completes the initialization once all the character's skills have been initialized
        /// </summary>
        /// <param name="skills">The array of the character's skills.</param>
        internal void CompleteInitialization(Skill[] skills)
        {
            m_prereqs.AddRange(StaticData.Prerequisites.Select(x => new SkillLevel(skills[x.Skill.ArrayIndex], x.Level)));
        }

        /// <summary>
        /// Imports and updates from the provided deserialization object
        /// </summary>
        /// <param name="src"></param>
        /// <param name="fromCCP"></param>
        internal void Import(SerializableCharacterSkill src, bool fromCCP)
        {
            m_owned = src.OwnsBook;
            m_known = (fromCCP | src.IsKnown);
            m_currentSkillPoints = src.Skillpoints;
            LastConfirmedLvl = src.Level;
            m_level = src.Level;
        }

        /// <summary>
        /// Resets the skill before we reimport data
        /// </summary>
        /// <param name="importFromCCP">When true, we will update it with up-to-date data from CCP</param>
        internal void Reset(bool importFromCCP)
        {
            m_known = false;
            m_currentSkillPoints = 0;
            LastConfirmedLvl = 0;
            m_level = 0;

            // Are we reloading the settings ?
            if (!importFromCCP)
                m_owned = false;
        }

        /// <summary>
        /// Marks the skill as completed
        /// </summary>
        internal void MarkAsCompleted()
        {
            m_known = true;
            m_level++;
            m_currentSkillPoints = StaticData.GetPointsRequiredForLevel(Math.Min(m_level, 5));
        }

        /// <summary>
        /// Exports the skill to a serialization object
        /// </summary>
        /// <returns></returns>
        internal SerializableCharacterSkill Export()
        {
            SerializableCharacterSkill dest = new SerializableCharacterSkill
                                                  {
                                                      ID = StaticData.ID,
                                                      Name = StaticData.Name,
                                                      Level = Math.Min(m_level, 5),
                                                      Skillpoints = m_currentSkillPoints,
                                                      OwnsBook = IsOwned,
                                                      IsKnown = m_known
                                                  };

            return dest;
        }

        /// <summary>
        /// Gets or sets true if the skill is owned.
        /// </summary>
        public bool IsOwned
        {
            get
            {
                return m_owned | (HasBookInAssets && !m_known);
            }
            set
            {
                m_owned = value;
                EveMonClient.OnCharacterUpdated(Character);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has book in assets.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has book in assets; otherwise, <c>false</c>.
        /// </value>
        public bool HasBookInAssets
        {
            get
            {
                CCPCharacter ccpCharacter = Character as CCPCharacter;
                return (ccpCharacter != null) && ccpCharacter.Assets.Any(asset => asset.Item.ID == ID);
            }
        }

        #endregion


        #region Core properties

        /// <summary>
        /// Gets the character this skill is bound to.
        /// </summary>
        public Character Character { get; private set; }

        /// <summary>
        /// Gets the underlying static data.
        /// </summary>
        public StaticSkill StaticData { get; private set; }

        /// <summary>
        /// Gets this skill's id.
        /// </summary>
        public long ID
        {
            get { return StaticData.ID; }
        }

        /// <summary>
        /// Gets a zero-based index for skills (allow the use of arrays to optimize computations).
        /// </summary>
        public int ArrayIndex
        {
            get { return StaticData.ArrayIndex; }
        }

        /// <summary>
        /// Gets this skill's name.
        /// </summary>
        public string Name
        {
            get { return StaticData.Name; }
        }

        /// <summary>
        /// Gets this skill's description.
        /// </summary>
        public string Description
        {
            get { return StaticData.Description; }
        }

        /// <summary>
        /// Gets whether this skill is known.
        /// </summary>
        public bool IsKnown
        {
            get { return m_known || IsTraining; }
        }

        /// <summary>
        /// Gets the skill group this skill is part of.
        /// </summary>
        public SkillGroup Group { get; private set; }

        /// <summary>
        /// Gets whether this skill and all its prereqs are trainable on a trial account.
        /// </summary>
        public bool IsTrainableOnTrialAccount
        {
            get { return StaticData.IsTrainableOnTrialAccount; }
        }

        /// <summary>
        /// Gets true if this is a public skill.
        /// </summary>
        public bool IsPublic
        {
            get { return StaticData.IsPublic; }
        }

        /// <summary>
        /// Gets the skill cost in ISK.
        /// </summary>
        public long Cost
        {
            get { return StaticData.Cost; }
        }

        /// <summary>
        /// Gets a formatted display of the ISK cost.
        /// </summary>
        public string FormattedCost
        {
            get { return StaticData.FormattedCost; }
        }

        /// <summary>
        /// Gets the primary attribute of this skill.
        /// </summary>
        public EveAttribute PrimaryAttribute
        {
            get { return StaticData.PrimaryAttribute; }
        }

        /// <summary>
        /// Gets the secondary attribute of this skill.
        /// </summary>
        public EveAttribute SecondaryAttribute
        {
            get { return StaticData.SecondaryAttribute; }
        }

        /// <summary>
        /// Gets the rank of this skill.
        /// </summary>
        public int Rank
        {
            get { return StaticData.Rank; }
        }

        /// <summary>
        /// Gets the current level of this skill, as gotten from CCP or possibly estimated by EVEMon according to training informations.
        /// </summary>
        public int Level
        {
            get
            {
                m_skillLevel = LastConfirmedLvl;
                int skillPointsToNextLevel = StaticData.GetPointsRequiredForLevel(Math.Min(LastConfirmedLvl + 1, 5));

                for (int i = 0; m_currentSkillPoints >= skillPointsToNextLevel && m_skillLevel < 5; i++)
                {
                    m_skillLevel++;
                    skillPointsToNextLevel = StaticData.GetPointsRequiredForLevel(Math.Min(m_skillLevel + 1, 5));
                }

                return m_skillLevel;
            }
        }

        /// <summary>
        /// Gets the level gotten from CCP during the last update.
        /// </summary>
        public int LastConfirmedLvl { get; private set; }

        /// <summary>
        /// Gets the skill's prerequisites
        /// </summary>
        public IEnumerable<SkillLevel> Prerequisites
        {
            get { return m_prereqs; }
        }

        /// <summary>
        /// Gets all the prerequisites. I.e, for eidetic memory, it will return <c>{ instant recall IV }</c>.
        /// The order matches the hierarchy.
        /// </summary>
        /// <remarks>Please notice, they may be redundancies.</remarks>
        public IEnumerable<SkillLevel> AllPrerequisites
        {
            get { return StaticData.AllPrerequisites.ToCharacter(Character); }
        }

        /// <summary>
        /// Gets the training speed.
        /// </summary>
        public int SkillPointsPerHour
        {
            get
            {
                float spPerHour = Character.GetBaseSPPerHour(this);
                return (int)Math.Round(spPerHour);
            }
        }

        #endregion


        #region Helper properties and methods

        /// <summary>
        /// Return current Level in Roman.
        /// </summary>
        public string RomanLevel
        {
            get { return GetRomanFromInt(Level); }
        }

        /// <summary>
        /// Gets true if the skill is queued.
        /// </summary>
        public bool IsQueued
        {
            get
            {
                CCPCharacter ccpCharacter = Character as CCPCharacter;

                // Current character isn't a CCP character, so can't have a Queue.
                if (ccpCharacter == null)
                    return false;

                SkillQueue skillQueue = ccpCharacter.SkillQueue;
                return skillQueue.Where(x => x.Skill != null).Any(skill => StaticData.ID == skill.Skill.ID);
            }
        }

        /// <summary>
        /// Gets true if the skill is currently in training.
        /// </summary>
        public bool IsTraining
        {
            get
            {
                CCPCharacter ccpCharacter = Character as CCPCharacter;
                if (ccpCharacter == null)
                    return false;

                return (ccpCharacter.IsTraining && ccpCharacter.CurrentlyTrainingSkill.Skill == this);
            }
        }

        /// <summary>
        /// Gets the current skill points of this skill (possibly estimated for skills in training).
        /// </summary>
        public int SkillPoints
        {
            get
            {
                // Is it in training ? Then we estimate the current SP
                if (IsTraining)
                {
                    CCPCharacter ccpCharacter = Character as CCPCharacter;
                    if (ccpCharacter != null)
                        return ccpCharacter.CurrentlyTrainingSkill.CurrentSP;
                }

                // Not in training
                return m_currentSkillPoints;
            }
        }

        /// <summary>
        /// Gets the completed fraction (between 0.0 and 1.0).
        /// </summary>
        public float FractionCompleted
        {
            get
            {
                // Lv 5 ? 
                if (m_level == 5)
                    return 1.0f;

                // Not partially trained ? Then it's 1.0
                int levelSp = StaticData.GetPointsRequiredForLevel(m_level);
                if (SkillPoints <= levelSp)
                    return 0.0f;

                // Partially trained, let's compute the difference with the previous level
                float nextLevelSp = StaticData.GetPointsRequiredForLevel(m_level + 1);
                float fraction = (SkillPoints - levelSp) / (nextLevelSp - levelSp);

                return (fraction <= 1 ? fraction : fraction % 1);
            }
        }

        /// <summary>
        /// Gets the percentage completion (between 0.0 and 100.0).
        /// </summary>
        public double PercentCompleted
        {
            get { return FractionCompleted * 100; }
        }

        /// <summary>
        /// Gets whether this skill is partially trained (true) or fully trained (false).
        /// </summary>
        public bool IsPartiallyTrained
        {
            get
            {
                if (Level == 5)
                    return false;

                bool partialLevel = SkillPoints > StaticData.GetPointsRequiredForLevel(Level),
                     isNotFullyTrained = (GetLeftPointsRequiredToLevel(Level + 1) != 0),
                     isPartiallyTrained = (partialLevel && isNotFullyTrained);
                return isPartiallyTrained;
            }
        }

        /// <summary>
        /// Gets true if all the prerequisites are met.
        /// </summary>
        public bool ArePrerequisitesMet
        {
            get { return m_prereqs.AreTrained(); }
        }

        /// <summary>
        /// Converts an integer into a roman number.
        /// </summary>
        /// <param name="number">Number from 1 to 5.</param>
        /// <returns>Roman number string.</returns>
        public static string GetRomanFromInt(int number)
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
                    return "(nulla)";
            }
        }

        /// <summary>
        /// Converts a roman number into an integer.
        /// </summary>
        /// <param name="r">Roman number from I to V.</param>
        /// <returns>Integer number.</returns>
        public static int GetIntFromRoman(string r)
        {
            switch (r)
            {
                case "I":
                    return 1;
                case "II":
                    return 2;
                case "III":
                    return 3;
                case "IV":
                    return 4;
                case "V":
                    return 5;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Calculate the time to train this skill to the next level including prerequisites.
        /// </summary>
        /// <returns>Time it will take</returns>
        public TimeSpan GetLeftTrainingTimeToNextLevel
        {
            get { return Level == 5 ? TimeSpan.Zero : GetLeftTrainingTimeToLevel(Level + 1); }
        }

        /// <summary>
        /// Returns the string representation of this skill (the name).
        /// </summary>
        /// <returns>The name of the skill.</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Gets this skill's representation for the provided character.
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public Skill ToCharacter(Character character)
        {
            if (character == null)
                throw new ArgumentNullException("character");

            return character.Skills[StaticData.ArrayIndex];
        }

        #endregion


        #region Computations

        /// <summary>
        /// Calculate the time it will take to train a certain amount of skill points.
        /// </summary>
        /// <param name="points">The amount of skill points.</param>
        /// <returns>Time it will take.</returns>
        public TimeSpan GetTimeSpanForPoints(int points)
        {
            return Character.GetTimeSpanForPoints(this, points);
        }

        /// <summary>
        /// Calculates the cumulative points required to reach the given level of this skill, starting from the current SP.
        /// </summary>
        /// <remarks>For a result starting from 0 SP, use the equivalent method on <see cref="StaticSkill"/>.</remarks>
        /// <param name="level">The level.</param>
        /// <returns>The required nr. of points.</returns>
        public int GetLeftPointsRequiredToLevel(int level)
        {
            int result = StaticData.GetPointsRequiredForLevel(level) - SkillPoints;

            return result < 0 ? 0 : result;
        }

        /// <summary>
        /// Calculates the cumulative points required for the only level of this skill, including the current SP if the level is partially trained.
        /// </summary>
        /// <remarks>For a result not including the current SP, use the equivalent method on <see cref="StaticSkill"/>.</remarks>
        /// <param name="level">The level.</param>
        /// <returns>The required nr. of points.</returns>
        private int GetLeftPointsRequiredForLevelOnly(int level)
        {
            if (level == 0)
                return 0;

            int startSP = Math.Max(SkillPoints, StaticData.GetPointsRequiredForLevel(level - 1));
            int result = StaticData.GetPointsRequiredForLevel(level) - startSP;

            return result < 0 ? 0 : result;
        }

        /// <summary>
        /// Calculates the time required to reach the given level of this skill, starting from the current SP.
        /// </summary>
        /// <remarks>For a result starting from 0 SP, use the equivalent method on <see cref="StaticSkill"/>.</remarks>
        /// <param name="level">The level.</param>
        /// <returns>The required time span.</returns>
        public TimeSpan GetLeftTrainingTimeToLevel(int level)
        {
            return GetTimeSpanForPoints(GetLeftPointsRequiredToLevel(level));
        }

        /// <summary>
        /// Calculates the time required for the only level of this skill, including the current SP if the level is partially trained.
        /// </summary>
        /// <remarks>For a result not including the current SP, use the equivalent method on <see cref="StaticSkill"/>.</remarks>
        /// <param name="level">The level.</param>
        /// <returns>The required time span.</returns>
        public TimeSpan GetLeftTrainingTimeForLevelOnly(int level)
        {
            return GetTimeSpanForPoints(GetLeftPointsRequiredForLevelOnly(level));
        }

        #endregion


        #region Conversion operators

        /// <summary>
        /// Returns the static skill the provided skill is based on.
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        public static implicit operator StaticSkill(Skill skill)
        {
            return skill == null ? null : skill.StaticData;
        }

        #endregion


        #region IStaticSkill Members

        Collection<StaticSkillLevel> IStaticSkill.Prerequisites
        {
            get { return StaticData.Prerequisites; }
        }

        StaticSkillGroup IStaticSkill.Group
        {
            get { return StaticData.Group; }
        }

        #endregion
    }
}