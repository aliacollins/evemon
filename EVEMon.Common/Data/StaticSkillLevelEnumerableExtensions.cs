using System.Collections.Generic;
using System.Linq;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Extension methods for objects of <see cref="StaticSkillLevel">StaticSkillLevel</see> objects.
    /// </summary>
    public static class StaticSkillLevelEnumerableExtensions
    {
        #region Methods on Enumerations of StaticSkillLevel

        /// <summary>
        /// Returns an equivalent enumeration with character skills.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="character"></param>
        /// <returns></returns>
        public static IEnumerable<SkillLevel> ToCharacter(this IEnumerable<StaticSkillLevel> src, Character character)
        {
            return src.Select(item => new SkillLevel(character.Skills[item.Skill.ID], item.Level));
        }

        /// <summary>
        /// Gets all the dependencies, in a way matching the hirarchical order and without redudancies.
        /// I.e, for eidetic memory II, it will return <c>{ instant recall I, instant recall II, instant recall III, instant recall IV,  eidetic memory I, eidetic memory II }</c>.
        /// </summary>
        /// <param name="src">The source.</param>
        /// <param name="includeRoots">When true, the levels in this enumeration are also included.</param>
        /// <returns></returns>
        public static IEnumerable<StaticSkillLevel> GetAllDependencies(this IEnumerable<StaticSkillLevel> src, bool includeRoots)
        {
            SkillLevelSet<StaticSkillLevel> set = new SkillLevelSet<StaticSkillLevel>();
            List<StaticSkillLevel> list = new List<StaticSkillLevel>();

            // Fill the set and list
            foreach (StaticSkillLevel item in src)
            {
                FillDependencies(set, list, item, includeRoots);
            }

            // Return the results
            return list;
        }

        #endregion


        #region Internal Static Methods

        /// <summary>
        /// Add the item, its previous levels and its prerequisites to the given set and list.
        /// </summary>
        /// <param name="set"></param>
        /// <param name="list"></param>
        /// <param name="item"></param>
        /// <param name="includeRoots"></param>
        internal static void FillDependencies(SkillLevelSet<StaticSkillLevel> set, List<StaticSkillLevel> list,
                                              StaticSkillLevel item, bool includeRoots)
        {
            StaticSkill skill = item.Skill;

            // Add first level and prerequisites
            if (!set.Contains(skill, 1))
            {
                // Prerequisites
                foreach (StaticSkillLevel prereq in skill.Prerequisites.Where(prereq => skill != prereq.Skill))
                {
                    FillDependencies(set, list, prereq, true);
                }

                // Include the first level
                StaticSkillLevel newItem = new StaticSkillLevel(skill, 1);
                list.Add(newItem);
                set.Set(newItem);
            }

            // Add greater levels
            int max = (includeRoots ? item.Level : item.Level - 1);
            for (int i = 2; i <= max; i++)
            {
                if (set.Contains(skill, i))
                    continue;

                StaticSkillLevel newItem = new StaticSkillLevel(skill, i);
                list.Add(newItem);
                set.Set(newItem);
            }
        }

        #endregion
    }
}