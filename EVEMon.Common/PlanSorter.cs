using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EVEMon.Common.Data;

namespace EVEMon.Common
{
    public enum PlanEntrySort
    {
        None,
        Cost,
        Rank,
        Name,
        Priority,
        PlanGroup,
        SPPerHour,
        TrainingTime,
        TrainingTimeNatural,
        PrimaryAttribute,
        SecondaryAttribute,
        SkillGroupDuration,
        PercentCompleted,
        TimeDifference,
        PlanType,
        Notes
    }

    /// <summary>
    /// This classes holds the responsibility to sort enumerations of plan entries
    /// </summary>
    internal sealed class PlanSorter
    {
        private PlanEntrySort m_sort;
        private bool m_reverseOrder;
        private bool m_groupByPriority;
        private bool m_learningSkillsFirst;
        private IEnumerable<PlanEntry> m_entries;
        private Dictionary<StaticSkillGroup, TimeSpan> m_skillGroupsDurations = new Dictionary<StaticSkillGroup, TimeSpan>();
        private BaseCharacter m_character;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="character"></param>
        /// <param name="entries"></param>
        /// <param name="sort"></param>
        /// <param name="reverseOrder"></param>
        /// <param name="groupByPriority"></param>
        /// <param name="learningSkillsFirst"></param>
        internal PlanSorter(BaseCharacter character, IEnumerable<PlanEntry> entries, PlanEntrySort sort, bool reverseOrder, bool groupByPriority, bool learningSkillsFirst)
        {
            m_sort = sort;
            m_entries = entries;
            m_reverseOrder = reverseOrder;
            m_groupByPriority = groupByPriority;
            m_learningSkillsFirst = learningSkillsFirst;
            m_character = character;
        }

        /// <summary>
        /// Performs the sort
        /// </summary>
        /// <param name="startSp"></param>
        /// <returns></returns>
        public IEnumerable<PlanEntry> Sort(int startSp)
        {
            int initialCount = m_entries.Count();

            // Apply first pass (learning skills)
            // We split the entries into a head (learnings) and a tail (non-learnings)
            PlanScratchpad headPlan = new PlanScratchpad(m_character);
            List<PlanEntry> tailEntries = new List<PlanEntry>();

            if (m_learningSkillsFirst)
            {
                tailEntries.AddRange(m_entries.Where(x => x.Skill.LearningClass == LearningClass.None));

                var learningSkills = m_entries.Where(x => x.Skill.LearningClass != LearningClass.None);
                headPlan = OptimizeLearningSkills(learningSkills, startSp);
            }
            else
            {
                tailEntries.AddRange(m_entries);
            }


            // Apply second pass (priorities grouping)
            // We split the tail into multiple tail groups
            List<PlanScratchpad> tailEntryPlans = new List<PlanScratchpad>();
            var scratchpad = new CharacterScratchpad(m_character);
            scratchpad.Train(headPlan);

            if (m_groupByPriority)
            {
                foreach (var group in tailEntries.GroupBy(x => x.Priority))
                {
                    tailEntryPlans.Add(new PlanScratchpad(scratchpad, group));
                }
            }
            else
            {
                tailEntryPlans.Add(new PlanScratchpad(scratchpad, tailEntries));
            }


            // Apply third pass (sorts)
            // We sort every tail group, and merge them once they're sorted.
            List<PlanEntry> list = new List<PlanEntry>();
            list.AddRange(headPlan);

            foreach (var tailPlan in tailEntryPlans)
            {
                tailPlan.UpdateStatistics(scratchpad, false, false);
                tailPlan.SimpleSort(m_sort, m_reverseOrder);
                list.AddRange(tailPlan);
            }

            // This is actually what GroupByPriority should do
            if (m_groupByPriority) list.StableSort(PlanSorter.CompareByPriority);

            // Fix prerequisites order
            FixPrerequisitesOrder(list);

            // Check we didn't mess up anything
            if (initialCount != list.Count)
            {
                throw new UnauthorizedAccessException("The sort algorithm messed up and deleted items");
            }

            // Return
            return list;
        }

        /// <summary>
        /// Ensures the prerequsiites order is correct
        /// </summary>
        /// <param name="list"></param>
        private void FixPrerequisitesOrder(List<PlanEntry> list)
        {
            // Gather prerequisites/postrequisites relationships and use them to connect nodes - O(n²) operation
            var dependencies = new Dictionary<PlanEntry,List<PlanEntry>>();
            foreach (var entry in list)
            {
                dependencies[entry] = new List<PlanEntry>(list.Where(x => entry.IsDependentOf(x)));
            }


            // Insert entries
            var entriesToAdd = new LinkedList<PlanEntry>(list);
            var set = new SkillLevelSet<PlanEntry>();
            list.Clear();

            while (entriesToAdd.Count != 0)
            {
                // Gets the first entry which has all its prerequisites satisfied.
                var item = entriesToAdd.First(x => dependencies[x].All(y => set[y.Skill, y.Level] != null));

                // Add it to the set and list, and remove it from the entries to add
                set[item.Skill, item.Level] = item;
                entriesToAdd.Remove(item);
                list.Add(item);
            }
        }

        /// <summary>
        /// Optimize the learning skills order
        /// </summary>
        /// <param name="baseEntries"></param>
        /// <param name="startSP"></param>
        /// <returns></returns>
        private PlanScratchpad OptimizeLearningSkills(IEnumerable<PlanEntry> baseEntries, int startSP)
        {
            var learningPlan = new PlanScratchpad(m_character);
            var entries = PrepareLearningSkillsInsertionQueue(baseEntries);

            // Insert all the entries in an optimal order
            while (entries.Count != 0)
            {
                // Pops the next level when there is one, quit otherwise
                PlanEntry entry = entries.Dequeue();
                learningPlan.InsertAtBestPosition(entry.Skill, entry.Level);
            }

            return learningPlan;
        }

        /// <summary>
        /// To be sorted, the learning skills are inserted each one after the other at the best insertion position. 
        /// However, to be truly optimal (or near optimal, it's not proven), the algorithm needs to get the skills in a certain order.
        /// This method collects the entries in the appropriate order.
        /// </summary>
        /// <param name="baseEntries"></param>
        /// <returns></returns>
        private static Queue<PlanEntry> PrepareLearningSkillsInsertionQueue(IEnumerable<PlanEntry> baseEntries)
        {
            // Here is the prerequisites table (attributes benefit | primary / secondary for lower | primary / secondary for upper)
            // INT          MEM/INT     INT/MEM
            // MEM          MEM/INT     MEM/INT
            // PER          MEM/INT     PER/WIL
            // WIL          MEM/INT     WIL/CHA
            // CHA          MEM/INT     CHA/PER
            // Learning     MEM/INT


            var queue = new Queue<Pair<string, int>>(55);

            // Interlaces int/mem/learning as first candidates (both lower and upper)
            for (int i = 0; i < 5; i++)
            {
                queue.Enqueue(new Pair<string, int>("Analytical Mind", i));
                queue.Enqueue(new Pair<string, int>("Instant Recall", i));
                queue.Enqueue(new Pair<string, int>("Learning", i));
            }
            for (int i = 0; i < 5; i++)
            {
                queue.Enqueue(new Pair<string, int>("Eidetic Memory", i));
                queue.Enqueue(new Pair<string, int>("Logic", i));
            }

            // Pushes lower will skills as next candidates
            for (int i = 0; i < 5; i++) queue.Enqueue(new Pair<string, int>("Iron Will", i));

            // Pushes lower per skills as next candidates
            for (int i = 0; i < 5; i++) queue.Enqueue(new Pair<string, int>("Spatial Awareness", i));

            // Pushes lower cha skills as next candidates
            for (int i = 0; i < 5; i++) queue.Enqueue(new Pair<string, int>("Empathy", i));

            // Interlaces upper per/wil/cha skills as last candidates
            for (int i = 0; i < 5; i++)
            {
                queue.Enqueue(new Pair<string, int>("Clarity", i));
                queue.Enqueue(new Pair<string, int>("Focus", i));
                queue.Enqueue(new Pair<string, int>("Presence", i));
            }

            // Now retrieve the plan's entries from the queue's pairs
            var entries = new Queue<PlanEntry>();
            foreach (var pair in queue)
            {
                foreach (var entry in baseEntries)
                {
                    if (pair.A == entry.Skill.Name && pair.B == entry.Level - 1)
                    {
                        entries.Enqueue(entry);
                        break;
                    }
                }
            }

            return entries;
        }


        #region Simple sort operators
        public static int CompareByName(PlanEntry x, PlanEntry y)
        {
            int nameDiff = String.CompareOrdinal(x.Skill.Name, y.Skill.Name);
            if (nameDiff != 0) return nameDiff;
            return x.Level - y.Level;
        }

        public static int CompareByCost(PlanEntry x, PlanEntry y)
        {
            long xCost = (x.Level == 1 && !x.CharacterSkill.IsOwned ? x.Skill.Cost : 0);
            long yCost = (y.Level == 1 && !x.CharacterSkill.IsOwned ? y.Skill.Cost : 0);
            return xCost.CompareTo(yCost);
        }

        public static int CompareByTrainingTime(PlanEntry x, PlanEntry y)
        {
            return x.TrainingTime.CompareTo(y.TrainingTime);
        }

        public static int CompareByTrainingTimeNatural(PlanEntry x, PlanEntry y)
        {
            return x.NaturalTrainingTime.CompareTo(y.NaturalTrainingTime);
        }

        public static int CompareBySPPerHour(PlanEntry x, PlanEntry y)
        {
            return x.SpPerHour - y.SpPerHour;
        }

        public static int CompareByPrimaryAttribute(PlanEntry x, PlanEntry y)
        {
            return (int)x.Skill.PrimaryAttribute - (int)y.Skill.PrimaryAttribute;
        }

        public static int CompareBySecondaryAttribute(PlanEntry x, PlanEntry y)
        {
            return (int)x.Skill.SecondaryAttribute - (int)y.Skill.SecondaryAttribute;
        }

        public static int CompareByPriority(PlanEntry x, PlanEntry y)
        {
            return x.Priority - y.Priority;
        }

        public static int CompareByPlanGroup(PlanEntry x, PlanEntry y)
        {
            return String.Compare(x.PlanGroupsDescription, y.PlanGroupsDescription);
        }

        public static int CompareByPlanType(PlanEntry x, PlanEntry y)
        {
            return (int)x.Type - (int)y.Type;
        }

        public static int CompareByNotes(PlanEntry x, PlanEntry y)
        {
            return String.Compare(x.Notes, y.Notes);
        }

        public static int CompareByTimeDifference(PlanEntry x, PlanEntry y)
        {
            var xDuration = x.TrainingTime - x.OldTrainingTime;
            var yDuration = y.TrainingTime - y.OldTrainingTime;
            return xDuration.CompareTo(yDuration);
        }

        public static int CompareByPercentCompleted(PlanEntry x, PlanEntry y)
        {
            float xRatio = x.CharacterSkill.FractionCompleted;
            float yRatio = y.CharacterSkill.FractionCompleted;
            return xRatio.CompareTo(yRatio);
        }

        public static int CompareByRank(PlanEntry x, PlanEntry y)
        {
            return x.Skill.Rank - y.Skill.Rank;
        }

        public static int CompareBySkillGroupDuration(PlanEntry x, PlanEntry y, IEnumerable<PlanEntry> entries, Dictionary<StaticSkillGroup, TimeSpan> skillGroupsDurations)
        {
            var xDuration = GetSkillGroupDuration(x.Skill.Group, entries, skillGroupsDurations);
            var yDuration = GetSkillGroupDuration(y.Skill.Group, entries, skillGroupsDurations);
            return xDuration.CompareTo(yDuration);
        }

        private static TimeSpan GetSkillGroupDuration(StaticSkillGroup group, IEnumerable<PlanEntry> entries, Dictionary<StaticSkillGroup, TimeSpan> skillGroupsDurations)
        {
            if (!skillGroupsDurations.ContainsKey(group))
            {
                TimeSpan time = TimeSpan.Zero;
                foreach (var entry in entries) time += entry.TrainingTime;
                skillGroupsDurations[group] = time;
            }
            
            return skillGroupsDurations[group];
        }
        #endregion
    }
}
