using System;
using System.Collections.Generic;
using System.Linq;

namespace EVEMon.Common
{
    public static class PlanEntryExtensions
    {
        /// <summary>
        /// Gets the total number of unique skills (two levels of same skill counts for one unique skill).
        /// </summary>
        /// <param name="items">List of <see cref="PlanEntry" />.</param>
        /// <returns>Count of unique skills.</returns>
        public static int GetUniqueSkillsCount(this IEnumerable<PlanEntry> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            return items.Distinct(new PlanEntryComparer()).Count();
        }

        /// <summary>
        /// Gets the number of not known skills selected (two levels of same skill counts for one unique skill).
        /// </summary>
        /// <param name="items">List of <see cref="PlanEntry" />.</param>
        /// <returns>Count of not known skills.</returns>
        public static int GetNotKnownSkillsCount(this IEnumerable<PlanEntry> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            return items.Distinct(new PlanEntryComparer()).Count(
                    entry => !entry.CharacterSkill.IsKnown && !entry.CharacterSkill.IsOwned);
        }

        /// <summary>
        /// Gets the total cost of the skill books, in ISK.
        /// </summary>
        /// <param name="items">List of <see cref="PlanEntry" />.</param>
        /// <returns>Cumulative cost of all skill books.</returns>
        public static long GetTotalBooksCost(this IEnumerable<PlanEntry> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            return items.Distinct(new PlanEntryComparer()).Sum(entry => entry.Skill.Cost);
        }

        /// <summary>
        /// Gets the cost of the not known skill books, in ISK.
        /// </summary>
        /// <param name="items">List of <see cref="PlanEntry" />.</param>
        /// <returns>Cumulative cost of not known skill books.</returns>
        public static long GetNotKnownSkillBooksCost(this IEnumerable<PlanEntry> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            return items.Distinct(new PlanEntryComparer()).Where
                (entry => !entry.CharacterSkill.IsKnown && !entry.CharacterSkill.IsOwned).Sum(entry => entry.Skill.Cost);
        }
    }
}