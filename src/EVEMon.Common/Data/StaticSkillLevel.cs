using System;
using System.Collections.Generic;
using EVEMon.Common.Constants;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Interfaces;
using EVEMon.Common.Models;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents a skill and level tuple.
    /// </summary>
    public class StaticSkillLevel : ISkillLevel
    {
        #region Constructors

        /// <summary>
        /// Constructor from the skill id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="level"></param>
        public StaticSkillLevel(Int64 id, Int64 level)
        {
            Skill = StaticSkills.GetSkillByID(id) ?? StaticSkill.UnknownStaticSkill;
            Level = level;
        }

        /// <summary>
        /// Constructor from the skill name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="level"></param>
        public StaticSkillLevel(string name, Int64 level)
        {
            Skill = StaticSkills.GetSkillByName(name) ?? StaticSkill.UnknownStaticSkill;
            Level = level;
        }

        /// <summary>
        /// Constructor from the static skill object.
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="level"></param>
        public StaticSkillLevel(StaticSkill skill, Int64 level)
        {
            Skill = skill;
            Level = level;
        }

        /// <summary>
        /// Constructor from an <see cref="ISkillLevel"/> object.
        /// </summary>
        /// <param name="obj"></param>
        public StaticSkillLevel(ISkillLevel obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            Skill = obj.Skill;
            Level = obj.Level;
        }

        /// <summary>
        /// Constructor from the skill id with activity.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="level"></param>
        /// <param name="activityId"></param>
        public StaticSkillLevel(Int64 id, Int64 level, int activityId)
        {
            Skill = StaticSkills.GetSkillByID(id) ?? StaticSkill.UnknownStaticSkill;
            Level = level;
            Activity = (BlueprintActivity)Enum.ToObject(typeof(BlueprintActivity), activityId);
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets or sets the skill.
        /// </summary>
        public StaticSkill Skill { get; }

        /// <summary>
        /// Gets or sets the skill level.
        /// </summary>
        public Int64 Level { get; }

        /// <summary>
        /// Gets or sets the activity for the skill.
        /// </summary>
        public BlueprintActivity Activity { get; }

        /// <summary>
        /// Gets all the dependencies, in a way matching the hirarchical order and without redudancies.
        /// I.e, for eidetic memory II, it will return <c>{ instant recall I, instant recall II, instant recall III, instant recall IV,  eidetic memory I, eidetic memory II }</c>.
        /// </summary>
        public IEnumerable<StaticSkillLevel> AllDependencies
        {
            get
            {
                SkillLevelSet<StaticSkillLevel> set = new SkillLevelSet<StaticSkillLevel>();
                List<StaticSkillLevel> list = new List<StaticSkillLevel>();

                // Fill the set and list
                list.FillDependencies(set, this, false);

                // Return the results
                return list;
            }
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Gets true if this skill level is, in any way, dependent of the provided skill level. Checks prerequisites but also same skill's lower levels.
        /// </summary>
        /// <param name="skillLevel"><see cref="ISkillLevel">ISkillLevel</see> to check if current skill is a dependant of the SkillLevel pass</param>
        /// <returns>True if the given item's skill is a prerequisite of this one or if it is a lower level of the same skill.</returns>
        public bool IsDependentOf(ISkillLevel skillLevel)
        {
            if (skillLevel == null)
                throw new ArgumentNullException("skillLevel");

            // Same skill, lower level ?
            if (Skill == skillLevel.Skill)
                return Level > skillLevel.Level;

            // Prerequisite
            Int64 neededLevel;
            Skill.HasAsPrerequisite(skillLevel.Skill, out neededLevel);
            return skillLevel.Level <= neededLevel;
        }

        #endregion


        #region Public Operators

        /// <summary>
        /// Implicitly converts from a non-static training.
        /// </summary>
        /// <param name="training"></param>
        /// <returns></returns>
        public static implicit operator StaticSkillLevel(SkillLevel training)
            => training == null ? null : new StaticSkillLevel(training.Skill.StaticData, training.Level);

        #endregion


        #region Overridden Methods

        /// <summary>
        /// Gets a string representation of this prerequisite.
        /// </summary>
        /// <returns>Skill Name and Level</returns>
        public override string ToString() => $"{Skill.Name} {Models.Skill.GetRomanFromInt(Level)}";

        #endregion
    }
}