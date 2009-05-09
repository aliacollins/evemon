using System;
using System.Collections.Generic;
using System.Text;

namespace EVEMon.Common
{
    public enum PlanSort
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

    public sealed class PlanSorter
    {
        #region Node<T>
        private sealed class Node<T>
        {
            public readonly T Item;

            private readonly List<Node<T>> prerequisites = new List<Node<T>>();
            private readonly List<Node<T>> postrequisites = new List<Node<T>>();
            private bool inserted;
            private bool deferred;

            public Node(T item)
            {
                this.Item = item;
            }

            public void AddPrerequisite(Node<T> prereq)
            {
                this.prerequisites.Add(prereq);
                prereq.postrequisites.Add(this);
            }

            public bool CanInsert
            {
                get
                {
                    foreach (var prereq in prerequisites)
                    {
                        if (!prereq.inserted) return false;
                    }
                    return true;
                }
            }

            public void TryAddTo(List<T> list)
            {
                // If not inserted yet
                if (!this.inserted)
                {
                    // If all prereqs have been inserted so far, we can add this skill
                    if (this.CanInsert)
                    {
                        list.Add(this.Item);
                        this.inserted = true;
                        this.deferred = false;

                        // We now add every postreq that have been previously tried but were pending because of this very skill
                        foreach (var postreq in this.postrequisites)
                        {
                            if (postreq.deferred) postreq.TryAddTo(list);
                        }
                    }
                    else
                    {
                        this.deferred = true;
                    }
                }
            }

            public void CleanUp()
            {
                this.prerequisites.Clear();
                this.postrequisites.Clear();
            }
        }
        #endregion

        #region Static methods
        public static void PutOrderedLearningSkillsAhead(Plan plan, bool groupByPriority)
        {
            Sort(plan, PlanSort.None, false, groupByPriority, true);
        }

        public static void Sort(Plan plan, PlanSort sort, bool reverseOrder, bool groupByPriority, bool learningSkillsFirst)
        {
            Sort(plan, sort, reverseOrder, groupByPriority, learningSkillsFirst, null);
        }

        public static void Sort(Plan plan, PlanSort sort, bool reverseOrder, bool groupByPriority, bool learningSkillsFirst, List<TimeSpan> timeDifferences)
        {
            // Perform the sort
            var sortedEntries = PlanSorter.Sort(plan.Entries, sort, reverseOrder, groupByPriority, learningSkillsFirst, plan.GrandCharacterInfo.SkillPointTotal, timeDifferences);

            // Update plan
            plan.SuppressEvents();
            try
            {
                plan.Entries.Clear();
                foreach (var entry in sortedEntries) plan.Entries.Add(entry);
            }
            finally
            {
                // Resume, will also refresh the plan
                plan.ResumeEvents();
            }
        }

        public static IEnumerable<Plan.Entry> Sort(IEnumerable<Plan.Entry> entries, PlanSort sort, bool reverseOrder, bool groupByPriority, bool learningSkillsFirst, int startSp, List<TimeSpan> timeDifferences)
        {
            var sorter = new PlanSorter(entries, sort, reverseOrder, groupByPriority, learningSkillsFirst, timeDifferences);
            return sorter.Sort(startSp);
        }
        #endregion



        private PlanSort sort;
        private bool reverseOrder;
        private bool groupByPriority;
        private bool learningSkillsFirst;
        private IEnumerable<Plan.Entry> entries;
        private Dictionary<Plan.Entry, TimeSpan> timeDifferences = new Dictionary<Plan.Entry,TimeSpan>();
        private EveAttributeScratchpad scratchPadAfterLearning = new EveAttributeScratchpad();
        private Dictionary<SkillGroup, TimeSpan> skillGroupsDurations = new Dictionary<SkillGroup,TimeSpan>();

        private PlanSorter(IEnumerable<Plan.Entry> entries, PlanSort sort, bool reverseOrder, bool groupByPriority, bool learningSkillsFirst, List<TimeSpan> timeDifferencesList)
        {
            this.sort = sort;
            this.entries = entries;
            this.reverseOrder = reverseOrder;
            this.groupByPriority = groupByPriority;
            this.learningSkillsFirst = learningSkillsFirst;
            if (sort == PlanSort.TimeDifference)
            {
                if (timeDifferencesList == null) this.sort = PlanSort.None;

                int index = 0;
                foreach(var entry in entries)
                {
                    this.timeDifferences[entry] = timeDifferencesList[index];
                    index++;
                }
            }
        }

        private IEnumerable<Plan.Entry> Sort(int startSp)
        {
            bool ignoreReverse = false;
            bool ignoreGroupByPriority = false;
            bool ignoreLearningSkillsFirst = false;

            // First, we apply the required filter
            List<Plan.Entry> list = new List<Plan.Entry>(entries);
            switch (sort)
            {
                case PlanSort.Name:
                    StableSort(list, CompareByName);
                    break;
                case PlanSort.Cost:
                    StableSort(list, CompareByCost);
                    break;
                case PlanSort.PrimaryAttribute:
                    StableSort(list, CompareByPrimaryAttribute);
                    break;
                case PlanSort.SecondaryAttribute:
                    StableSort(list, CompareBySecondaryAttribute);
                    break;
                case PlanSort.Priority:
                    StableSort(list, CompareByPriority);
                    break;
                case PlanSort.PlanGroup:
                    StableSort(list, CompareByPlanGroup);
                    break;
                case PlanSort.PercentCompleted:
                    StableSort(list, CompareByPercentCompleted);
                    break;
                case PlanSort.Rank:
                    StableSort(list, CompareByRank);
                    break;
                case PlanSort.Notes:
                    StableSort(list, CompareByNotes);
                    break;
                case PlanSort.PlanType:
                    StableSort(list, CompareByPlanType);
                    break;
                case PlanSort.TimeDifference:
                    StableSort(list, CompareByTimeDifference);
                    break;
                case PlanSort.TrainingTime:
                    // When learning skills are on top, we use the scratchpad they will generate after we optimized them. It's dealt in PutLearningOnTop
                    if (!this.learningSkillsFirst) StableSort(list, CompareByTrainingTime);
                    else ignoreReverse = true;
                    break;
                case PlanSort.TrainingTimeNatural:
                    // When learning skills are on top, we use the scratchpad they will generate after we optimized them. It's dealt in PutLearningOnTop
                    if (!this.learningSkillsFirst) StableSort(list, CompareByTrainingTimeNatural);
                    else ignoreReverse = true;
                    break;
                case PlanSort.SkillGroupDuration:
                    // When learning skills are on top, we use the scratchpad they will generate after we optimized them. It's dealt in PutLearningOnTop
                    if (!this.learningSkillsFirst) StableSort(list, CompareBySkillGroupDuration);
                    else ignoreReverse = true;
                    break;
                case PlanSort.SPPerHour:
                    // When learning skills are on top, we use the scratchpad they will generate after we optimized them. It's dealt in PutLearningOnTop
                    if (!this.learningSkillsFirst) StableSort(list, CompareBySPPerHour);
                    else ignoreReverse = true;
                    break;
                default:
                    ignoreReverse = true;
                    break;
            }

            // Reverse sort order
            if (reverseOrder && !ignoreReverse) list.Reverse();

            // Group by priority
            if (groupByPriority && !ignoreGroupByPriority)
            {
                GroupByPriority(list);
            }

            // Put learning skills first
            if (learningSkillsFirst && !ignoreLearningSkillsFirst)
            {
                PutLearningsOnTop(list, startSp);
            }

            // Rebuild prerequisites order
            ReorderPorstrequisites(list);
            return list;
        }

        private void ReorderPorstrequisites(List<Plan.Entry> list)
        {
            int count = list.Count;

            // Transform the list as another one of disjointed graph nodes
            List<Node<Plan.Entry>> nodes = new List<Node<Plan.Entry>>();
            foreach (var entry in list)
            {
                nodes.Add(new Node<Plan.Entry>(entry));
            }

            // Gather prerequisites/postrequisites relationships and use them to connect nodes - O(n²) operation
            foreach (var node in nodes)
            {
                // Scan all items in the tree
                foreach (var prereqCandidate in nodes)
                {
                    if (node != prereqCandidate)
                    {
                        // First, we test whether the prereq candidate is the previous level
                        bool isPrereq = (node.Item.Skill == prereqCandidate.Item.Skill && node.Item.Level == prereqCandidate.Item.Level + 1);

                        // If it's not, then we test whether it is an immediate prerequisite
                        if (!isPrereq) 
                        {
                            int neededLevel;
                            isPrereq = node.Item.Skill.HasAsImmedPrereq(prereqCandidate.Item.Skill, out neededLevel);
                            isPrereq &= (prereqCandidate.Item.Level == neededLevel);
                        }

                        // Finally, if it is indeed a prereq, we add it to the list
                        if (isPrereq)
                        {
                            node.AddPrerequisite(prereqCandidate);
                        }
                    }
                }
            }

            // Flatten the graph in a tree
            list.Clear();
            foreach (var tree in nodes) tree.TryAddTo(list);
            System.Diagnostics.Debug.Assert(list.Count == count);

            // Clean up to take care of the bidirectionnal mess for the GC
            foreach (var tree in nodes) tree.CleanUp();
        }

        private void PutLearningsOnTop(List<Plan.Entry> list, int startSP)
        {
            List<Plan.Entry> learningSkills = new List<Plan.Entry>();
            List<Plan.Entry> nonLearningSkills = new List<Plan.Entry>();

            foreach (var entry in list)
            {
                if (entry.Skill.SkillGroup.Name == "Learning") learningSkills.Add(entry);
                else nonLearningSkills.Add(entry);
            }

            list.Clear();
            list.AddRange(OptimizeLearningSkills(learningSkills, startSP));
            switch(this.sort)
            {
                case PlanSort.TrainingTime:
                    StableSort(nonLearningSkills, CompareByTrainingTime);
                    if (reverseOrder) nonLearningSkills.Reverse();
                    break;
                case PlanSort.TrainingTimeNatural:
                    StableSort(nonLearningSkills, CompareByTrainingTimeNatural);
                    if (reverseOrder) nonLearningSkills.Reverse();
                    break;
                case PlanSort.SkillGroupDuration:
                    StableSort(nonLearningSkills, CompareBySkillGroupDuration);
                    if (reverseOrder) nonLearningSkills.Reverse();
                    break;
                case PlanSort.SPPerHour:
                    StableSort(nonLearningSkills, CompareBySPPerHour);
                    if (reverseOrder) nonLearningSkills.Reverse();
                    break;
                default:
                    break;
            }
            list.AddRange(nonLearningSkills);
        }

        private static void GroupByPriority(List<Plan.Entry> list)
        {
            Dictionary<int, List<Plan.Entry>> dictionary = new Dictionary<int, List<Plan.Entry>>();
            int min = Int32.MaxValue, max = 0;

            foreach (var entry in list)
            {
                min = Math.Min(min, entry.Priority);
                max = Math.Max(max, entry.Priority);

                if (!dictionary.ContainsKey(entry.Priority))
                {
                    dictionary[entry.Priority] = new List<Plan.Entry>(1);
                }
                dictionary[entry.Priority].Add(entry);
            }

            list.Clear();
            for (int i = min; i <= max; i++)
            {
                if (dictionary.ContainsKey(i))
                {
                    list.AddRange(dictionary[i]);
                }
            }
        }


        #region Learning skills sorting
        private IEnumerable<Plan.Entry> OptimizeLearningSkills(IEnumerable<Plan.Entry> baseEntries, int startSP)
        {
            var orderedEntries = new List<Plan.Entry>(55);
            var entries = PrepareLearningSkillsInsertionQueue(baseEntries);

            while (entries.Count != 0)
            {
                // Pops the next level when there is one, quit otherwise
                Plan.Entry entry = entries.Dequeue();

                // Look at prerequisites to search min and max insertion positions
                int minPosition = 0, maxPosition = orderedEntries.Count;
                for (int i = 0; i < orderedEntries.Count; i++)
                {
                    var insertedEntry = orderedEntries[i];
                    if (TestPrerequisite(entry, insertedEntry))
                    {
                        minPosition = Math.Max(minPosition, i + 1);
                    }
                    if (TestPrerequisite(insertedEntry, entry))
                    {
                        maxPosition = Math.Min(maxPosition, i);
                    }
                }

                // We now search for the best insertion position
                //if (nextLevel.Skill.Name == "Focus") Console.Write("");
                TimeSpan bestTime = TimeSpan.MaxValue;
                int bestCandidatePosition = orderedEntries.Count;
                for (int index = maxPosition; index >= minPosition; index--)
                {
                    // Compute list's training time if the next item was inserted at index
                    int skillPoint = startSP;
                    TimeSpan candidateTime = TimeSpan.Zero;
                    EveAttributeScratchpad scratchpad = new EveAttributeScratchpad();

                    for (int i = 0; i <= orderedEntries.Count; i++)
                    {
                        if (i < index) Train(orderedEntries[i], scratchpad, ref candidateTime, ref skillPoint);
                        else if (i > index) Train(orderedEntries[i - 1], scratchpad, ref candidateTime, ref skillPoint);
                        else Train(entry, scratchpad, ref candidateTime, ref skillPoint);
                    }

                    // Is it better with this index ? Then, we retain this as the best candidate
                    if (bestTime > candidateTime)
                    {
                        bestTime = candidateTime;
                        bestCandidatePosition = index;
                        if (entries.Count == 0) this.scratchPadAfterLearning = scratchpad;
                    }
                }

                // Insert at the best candidate position
                orderedEntries.Insert(bestCandidatePosition, entry);
            }

            // Found the best path, gather data
            return orderedEntries;
        }

        private static void Train(Plan.Entry entry, EveAttributeScratchpad scratchpad, ref TimeSpan time, ref int sp)
        {
            time += entry.Skill.GetTrainingTimeOfLevelOnly(entry.Level, sp, true, scratchpad);
            sp += entry.Skill.GetPointsForLevelOnly(entry.Level, true);
            scratchpad.ApplyALevelOf(entry.Skill);
        }

        private static bool TestPrerequisite(Plan.Entry entry, Plan.Entry prerequisiteCandidate)
        {
            int entryFamily = GetLearningFamily(entry.SkillName);
            int prereqFamily = GetLearningFamily(prerequisiteCandidate.SkillName);

            if (entryFamily == prereqFamily)
            {
                if (prerequisiteCandidate.Skill.Rank == entry.Skill.Rank) return prerequisiteCandidate.Level < entry.Level;
                if (prerequisiteCandidate.Skill.Rank < entry.Skill.Rank) return prerequisiteCandidate.Level <= 4;
            }
            return false;
        }

        private static int GetLearningFamily(string skillName)
        {
            switch (skillName)
            {
                case "Analytical Mind":
                case "Logic":
                    return 0;

                case "Spatial Awareness":
                case "Clarity":
                    return 1;

                case "Instant Recall":
                case "Eidetic Memory":
                    return 2;

                case "Iron Will":
                case "Focus":
                    return 3;

                case "Empathy":
                case "Presence":
                    return 4;

                case "Learning":
                    return 5;

                default:
                    return -1;
            }
        }

        private static Queue<Plan.Entry> PrepareLearningSkillsInsertionQueue(IEnumerable<Plan.Entry> baseEntries)
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
            var entries = new Queue<Plan.Entry>();
            foreach (var pair in queue)
            {
                foreach (var entry in baseEntries)
                {
                    if (pair.A == entry.SkillName && pair.B == entry.Level - 1)
                    {
                        entries.Enqueue(entry);
                        break;
                    }
                }
            }

            return entries;
        }
        #endregion


        #region Simple sort operators by category
        private int CompareByName(Plan.Entry x, Plan.Entry y)
        {
            int nameDiff = String.CompareOrdinal(x.SkillName, y.SkillName);
            if (nameDiff != 0) return nameDiff;
            return x.Level - y.Level;
        }

        private int CompareByCost(Plan.Entry x, Plan.Entry y)
        {
            long xCost = (x.Level == 1 ? x.Skill.Cost : 0);
            long yCost = (y.Level == 1 ? y.Skill.Cost : 0);
            if (xCost > yCost) return 1;
            if (xCost < yCost) return -1;
            return 0;
        }

        private int CompareByTrainingTime(Plan.Entry x, Plan.Entry y)
        {
            // We ignore the newbies' bonus
            var xDuration = x.Skill.GetTrainingTimeOfLevelOnly(x.Level, EveConstants.NewCharacterTrainingThreshold, true, this.scratchPadAfterLearning, true);
            var yDuration = y.Skill.GetTrainingTimeOfLevelOnly(y.Level, EveConstants.NewCharacterTrainingThreshold, true, this.scratchPadAfterLearning, true);

            if (xDuration > yDuration) return 1;
            if (xDuration < yDuration) return -1;
            return 0;
        }

        private int CompareByTrainingTimeNatural(Plan.Entry x, Plan.Entry y)
        {
            // We ignore the newbies' bonus
            var xDuration = x.Skill.GetTrainingTimeOfLevelOnly(x.Level, EveConstants.NewCharacterTrainingThreshold, true, this.scratchPadAfterLearning, false);
            var yDuration = y.Skill.GetTrainingTimeOfLevelOnly(y.Level, EveConstants.NewCharacterTrainingThreshold, true, this.scratchPadAfterLearning, false);

            if (xDuration > yDuration) return 1;
            if (xDuration < yDuration) return -1;
            return 0;
        }

        private int CompareBySPPerHour(Plan.Entry x, Plan.Entry y)
        {
            // We ignore the newbies' bonus
            var character = x.Plan.GrandCharacterInfo;
            var xSpeed = character.GetEffectiveAttribute(x.Skill.PrimaryAttribute, this.scratchPadAfterLearning, true, true) * 2 +
                character.GetEffectiveAttribute(x.Skill.SecondaryAttribute, this.scratchPadAfterLearning, true, true);
            var ySpeed = character.GetEffectiveAttribute(y.Skill.PrimaryAttribute, this.scratchPadAfterLearning, true, true) * 2 +
                character.GetEffectiveAttribute(y.Skill.SecondaryAttribute, this.scratchPadAfterLearning, true, true);

            if (xSpeed > ySpeed) return 1;
            if (xSpeed < ySpeed) return -1;
            return 0;
        }

        private int CompareByPrimaryAttribute(Plan.Entry x, Plan.Entry y)
        {
            return (int)x.Skill.PrimaryAttribute - (int)y.Skill.PrimaryAttribute;
        }

        private int CompareBySecondaryAttribute(Plan.Entry x, Plan.Entry y)
        {
            return (int)x.Skill.SecondaryAttribute - (int)y.Skill.SecondaryAttribute;
        }

        private int CompareByPriority(Plan.Entry x, Plan.Entry y)
        {
            return x.Priority - y.Priority;
        }

        private int CompareByPlanGroup(Plan.Entry x, Plan.Entry y)
        {
            return String.Compare(x.PlanGroupsDescription, y.PlanGroupsDescription);
        }

        private int CompareByPlanType(Plan.Entry x, Plan.Entry y)
        {
            return (int)x.EntryType - (int)y.EntryType;
        }

        private int CompareByNotes(Plan.Entry x, Plan.Entry y)
        {
            return String.Compare(x.Notes, y.Notes);
        }

        private int CompareByTimeDifference(Plan.Entry x, Plan.Entry y)
        {
            var xDuration = this.timeDifferences[x];
            var yDuration = this.timeDifferences[y];

            if (xDuration > yDuration) return 1;
            if (xDuration < yDuration) return -1;
            return 0;
        }

        private int CompareByPercentCompleted(Plan.Entry x, Plan.Entry y)
        {
            float xRatio = 0.0f, yRatio = 0.0f;

            if (x.Skill.LastConfirmedLvl == 0) xRatio = x.Skill.CurrentSkillPoints / (float)x.Skill.GetPointsRequiredForLevel(x.Level);
            else xRatio = (x.Skill.CurrentSkillPoints - x.Skill.GetPointsRequiredForLevel(x.Level - 1)) / (float)x.Skill.GetPointsRequiredForLevel(x.Level);

            if (y.Skill.LastConfirmedLvl == 0) yRatio = y.Skill.CurrentSkillPoints / (float)y.Skill.GetPointsRequiredForLevel(y.Level);
            else yRatio = (y.Skill.CurrentSkillPoints - y.Skill.GetPointsRequiredForLevel(y.Level - 1)) / (float)y.Skill.GetPointsRequiredForLevel(y.Level);

            if (xRatio > yRatio) return 1;
            else if (xRatio < yRatio) return -1;
            return 0;
        }


        private int CompareBySkillGroupDuration(Plan.Entry x, Plan.Entry y)
        {
            var xDuration = GetSkillGroupDuration(x.Skill.SkillGroup);
            var yDuration = GetSkillGroupDuration(y.Skill.SkillGroup);

            if (xDuration > yDuration) return 1;
            if (xDuration < yDuration) return -1;
            return 0;
        }

        private int CompareByRank(Plan.Entry x, Plan.Entry y)
        {
            return x.Skill.Rank - y.Skill.Rank;
        }

        private TimeSpan GetSkillGroupDuration(SkillGroup group)
        {
            if (!skillGroupsDurations.ContainsKey(group))
            {
                TimeSpan time = TimeSpan.Zero;
                foreach (var entry in entries)
                {
                    if (entry.Skill.SkillGroup == group)
                    {
                        EveAttributeScratchpad scratchpad = (group.Name == "Learning" ? null : this.scratchPadAfterLearning);
                        time += entry.Skill.GetTrainingTimeOfLevelOnly(entry.Level, EveConstants.NewCharacterTrainingThreshold, true, scratchpad, false);
                    }
                }
                skillGroupsDurations[group] = time;
            }
            
            return skillGroupsDurations[group];
        }

        /// <summary>
        /// Uses an insertion sort algorithm to perform a stable sort (keep the initial order of the keys with equal values).
        /// </summary>
        /// <remarks>Memory overhead is null, average complexity is O(n.ln(n)), worst-case is O(n²).</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="comparison"></param>
        private void StableSort<T>(List<T> list, Comparison<T> comparison)
        {
            // For every key
            for (int i = 1; i < list.Count; i++)
            {
                var value = list[i];
                int j = i - 1;

                // Move the key backward while the previous items are lesser than it, shifting those items to thr right
                while (j >= 0 && comparison(list[j], value) > 0)
                {
                    list[j + 1] = list[j];
                    j--;
                }

                // Insert at the left of the scrolled sequence, immediately on the right of the first lesser or equal value it found
                list[j + 1] = value;
            }
        }
        #endregion
    }
}
