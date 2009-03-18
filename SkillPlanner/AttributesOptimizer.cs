using System;
using System.Collections.Generic;
using System.Text;
using EVEMon.Common;

namespace EVEMon.SkillPlanner
{
    public static class AttributesOptimizer
    {
        #region SkillTraining
        public struct SkillTraining
        {
            public int PointsToTrain;
            public Skill Skill;

            public SkillTraining(Skill skill, int sp)
            {
                this.Skill = skill;
                this.PointsToTrain = sp;
            }
        }
        #endregion

        private const int MinPerSkill = 5;
        private const int MaxPerSkill = 10;


        /// <summary>
        /// Generate a trainings array from a plan
        /// </summary>
        /// <param name="plan"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static SkillTraining[] GetTraining(Plan plan, int startIndex)
        {
            int index = 0;
            var list = new List<SkillTraining>(plan.Entries.Count);
            foreach (var entry in plan.Entries)
            {
                if (entry.Skill != null)
                {
                    if (index >= startIndex)
                    {
                        int sp = entry.Skill.GetPointsForLevelOnly(entry.Level, true);
                        list.Add(new SkillTraining(entry.Skill, sp));
                    }
                    index++;
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// Generate a trainings array from the skills already know by a character
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public static SkillTraining[] GetTraining(CharacterInfo character)
        {
            var list = new List<SkillTraining>();
            foreach (var group in character.SkillGroups.Values)
            {
                foreach (var skill in group)
                {
                    list.Add(new SkillTraining(skill, skill.CurrentSkillPoints));
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// Extract a trainings range from the given trainings array, starting at the given index, ensuring the total training time will be right above the given duration
        /// </summary>
        /// <param name="training"></param>
        /// <param name="character"></param>
        /// <param name="scratchpad"></param>
        /// <param name="isPlan"></param>
        /// <param name="maxDuration"></param>
        /// <returns></returns>
        public static SkillTraining[] GetSubTraining(SkillTraining[] training, CharacterInfo character, EveAttributeScratchpad scratchpad, bool isPlan, TimeSpan maxDuration)
        {
            TimeSpan time = TimeSpan.Zero;
            List<SkillTraining> newTraining = new List<SkillTraining>();
            int cumulativeSkillTotal = (isPlan ? character.SkillPointTotal : 0);

            // Scroll through the entries
            foreach (var entry in training)
            {
                // Compute training time
                time += entry.Skill.GetTimeSpanForPoints(entry.PointsToTrain, cumulativeSkillTotal, scratchpad, true);
                cumulativeSkillTotal += entry.PointsToTrain;
                if (isPlan) scratchpad.ApplyALevelOf(entry.Skill);

                // Add the entry
                newTraining.Add(entry);

                // Break if we just went further than the given limit
                if (time >= maxDuration) break;
            }

            // Create the plan and return it
            return newTraining.ToArray();
        }

        /// <summary>
        /// Compute the time required to achieve the specified trainings
        /// </summary>
        /// <param name="skills"></param>
        /// <param name="character"></param>
        /// <param name="scratchpad"></param>
        /// <param name="isPlan">True if the trainings have been generated from a plan, false either</param>
        /// <returns></returns>
        private static TimeSpan ComputeTotalTime(SkillTraining[] skills, CharacterInfo character, EveAttributeScratchpad scratchpad, bool isPlan)
        {
            TimeSpan time = TimeSpan.Zero;
            int cumulativeSkillTotal = (isPlan ? character.SkillPointTotal : 0);

            foreach (var training in skills)
            {
                time += training.Skill.GetTimeSpanForPoints(training.PointsToTrain, cumulativeSkillTotal, scratchpad, true);
                cumulativeSkillTotal += training.PointsToTrain;
                if (isPlan) scratchpad.ApplyALevelOf(training.Skill);
            }

            return time;
        }

        /// <summary>
        /// Compute the best possible attributes to fulfill the given trainings array
        /// </summary>
        /// <param name="skills"></param>
        /// <param name="character"></param>
        /// <param name="maxDuration"></param>
        /// <param name="isPlan"></param>
        /// <param name="bestTime"></param>
        /// <param name="baseTime"></param>
        /// <returns></returns>
        public static EveAttributeScratchpad Optimize(SkillTraining[] skills, CharacterInfo character, TimeSpan maxDuration, bool isPlan, out TimeSpan bestTime, out TimeSpan baseTime)
        {
            baseTime = TimeSpan.MaxValue;
            bestTime = TimeSpan.MaxValue;

            // Get the number of points currently spent into each
            // attribute
            int memBase = character.GetBaseAttribute(EveAttribute.Memory) - MinPerSkill;
            int chaBase = character.GetBaseAttribute(EveAttribute.Charisma) - MinPerSkill;
            int wilBase = character.GetBaseAttribute(EveAttribute.Willpower) - MinPerSkill;
            int perBase = character.GetBaseAttribute(EveAttribute.Perception) - MinPerSkill;
            int intBase = character.GetBaseAttribute(EveAttribute.Intelligence) - MinPerSkill;

            // calculate the total points to spend in case CCP do something
            // odd with attributes in the future.
            int totalPoints = memBase + chaBase + wilBase + perBase + intBase;

            // 1. We start with a plan shorter than the given limit. 
            // 2. Then, we scroll through misc attributes combination to find the best possible attributes.
            // 3. Since those new attributes make the training faster, we take a bigger plan, still shorter than the given limit and 
            EveAttributeScratchpad bestScratchpad = new EveAttributeScratchpad();
            var training = GetSubTraining(skills, character, new EveAttributeScratchpad(), isPlan, maxDuration);
            while (true)
            {
                // Recompute base time
                bool foundBetter = false;
                var tempScratchpad = new EveAttributeScratchpad();
                baseTime = ComputeTotalTime(training, character, tempScratchpad, isPlan);
                bestTime = baseTime;

                // Now, we have the points to spend, let's perform all the
                // combinations (less than 11^4 = 14,641)
                for (int per = 0; per <= MaxPerSkill; per++)
                {
                    // WIL
                    int maxWillpower = totalPoints - per;
                    for (int will = 0; will <= maxWillpower && will <= MaxPerSkill; will++)
                    {
                        // INT
                        int maxIntelligence = maxWillpower - will;
                        for (int intell = 0; intell <= maxIntelligence && intell <= MaxPerSkill; intell++)
                        {
                            // MEM
                            int maxMemory = maxIntelligence - intell;
                            for (int mem = 0; mem <= maxMemory && mem <= MaxPerSkill; mem++)
                            {
                                // CHA
                                int cha = maxMemory - mem;

                                // Reject invalid combinations
                                if (cha <= MaxPerSkill)
                                {
                                    // Compute plan time
                                    tempScratchpad.Reset(per - perBase, will - wilBase, intell - intBase, mem - memBase, cha - chaBase);
                                    TimeSpan tempTime = ComputeTotalTime(training, character, tempScratchpad, isPlan);

                                    // Compare it to the best time so far
                                    if (tempTime < bestTime)
                                    {
                                        bestTime = tempTime;
                                        foundBetter = true;

                                        // Store the attributes for the best time
                                        bestScratchpad.Reset(per - perBase, will - wilBase, intell - intBase, mem - memBase, cha - chaBase);
                                    }
                                }
                            }
                        }
                    }
                }

                // If we didn't find any better combination this time or if the current time was already below one year, no need to gat a longer plan
                if (!foundBetter) break;
                if (baseTime < maxDuration) break;

                // We need a bigger plan, still under the max duration
                int oldTrainingLength = training.Length;
                training = GetSubTraining(skills, character, bestScratchpad, isPlan, maxDuration);

                // break if it is not shorter than the previous plan
                if (training.Length == oldTrainingLength) break;
                bestScratchpad = new EveAttributeScratchpad();
            }

            // Return the best scratchpad found
            return bestScratchpad;
        }

    }
}
