using System;
using System.Collections.Generic;
using System.Text;
using EVEMon.Common;

namespace EVEMon.Common
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

        #region Remapping
        public class Remapping
        {
            /// <summary>
            /// Gets the list of the trainng after this remap
            /// </summary>
            public readonly List<SkillTraining> Skills = new List<SkillTraining>();
            /// <summary>
            /// Gets the remapping point associated with that remapping. May be null if a remapping was automatically added at the beginning of the training
            /// </summary>
            public readonly Plan.RemappingPoint Point;
            /// <summary>
            /// Gets the base scratchpad before the remapping
            /// </summary>
            public readonly EveAttributeScratchpad BaseScratchpad;

            private EveAttributeScratchpad bestScratchpad;
            private TimeSpan bestDuration;
            private TimeSpan baseDuration;
            private TimeSpan time;
            private TimeSpan limit;
            private int startSp;

            public Remapping(Plan.RemappingPoint point, EveAttributeScratchpad baseScratchpad)
            {
                this.Point = point;
                this.BaseScratchpad = baseScratchpad ?? new EveAttributeScratchpad();
            }

            /// <summary>
            /// Gets skillpoints before the remapping
            /// </summary>
            public int StartSP
            {
                get { return startSp; }
            }

            /// <summary>
            /// Gets the best scratchpad after the remapping
            /// </summary>
            public EveAttributeScratchpad BestScratchpad
            {
                get { return this.bestScratchpad; }
            }

            /// <summary>
            /// Gets the training duration with the best remapping
            /// </summary>
            public TimeSpan BestDuration
            {
                get { return this.bestDuration; }
            }

            /// <summary>
            /// Gets the base training duration before the remapping
            /// </summary>
            public TimeSpan BaseDuration
            {
                get { return this.baseDuration; }
            }

            /// <summary>
            /// Gets the time when this remapping was done
            /// </summary>
            public TimeSpan Time
            {
                get { return this.time; }
            }

            /// <summary>
            /// Gets the maximum optimization duration
            /// </summary>
            public TimeSpan Limit
            {
                get { return this.limit; }
            }

            /// <summary>
            /// Computes remapping.
            /// </summary>
            /// <param name="character">Character information</param>
            /// <param name="time">Time when remapping begins</param>
            /// <param name="maxDuration">Maximum duration of optimization period</param>
            /// <param name="startSp">Start skillpoints</param>
            /// <returns>Training time after remapping</returns>
            internal TimeSpan Compute(CharacterInfo character, TimeSpan time, TimeSpan maxDuration, int startSp)
            {
                this.time = time;
                this.limit = maxDuration;
                this.startSp = startSp;
                var skills = this.Skills.ToArray();
                this.bestScratchpad = Optimize(skills, character, this.BaseScratchpad.Clone(), maxDuration, startSp, out this.bestDuration, out this.baseDuration);
                if (this.Point != null)
                    this.Point.SetBaseAttributes(character, this.BaseScratchpad, this.BestScratchpad);
                return this.time + ComputeTotalTime(skills, character, this.bestScratchpad.Clone(), startSp);
            }

            /// <summary>
            /// Computes remapping from given scratchpad.
            /// This method allows to make EVEMon.Common.AttributesOptimizer.Remapping object without performing an optimization.
            /// </summary>
            /// <param name="character">Character information</param>
            /// <param name="time">Time at which remapping starts</param>
            /// <param name="maxDuration">Maximum duration of optimization period</param>
            /// <param name="startSp">Start skillpoints</param>
            /// <param name="oldScratchpad">Optimized scratchpad from base remapping</param>
            /// <param name="newScratchpad">New scratchpad to compute time for</param>
            /// <returns>Training time after remapping</returns>
            internal TimeSpan ComputeManually(CharacterInfo character, TimeSpan time, TimeSpan maxDuration, int startSp, EveAttributeScratchpad oldScratchpad, EveAttributeScratchpad newScratchpad)
            {
                this.time = time;
                this.limit = maxDuration;
                this.startSp = startSp;
                var skills = this.Skills.ToArray();
                this.bestScratchpad = newScratchpad;

                // Compute time based on provided scratchpads
                var training = GetSubTraining(skills, character, oldScratchpad.Clone(), startSp, maxDuration);
                this.baseDuration = ComputeTotalTime(training, character, this.BaseScratchpad.Clone(), startSp);
                this.bestDuration = ComputeTotalTime(training, character, this.BestScratchpad.Clone(), startSp);

                if (this.Point != null)
                    this.Point.SetBaseAttributes(character, this.BaseScratchpad, this.BestScratchpad);
                return this.time + ComputeTotalTime(skills, character, this.bestScratchpad.Clone(), startSp);
            }

            public int GetBaseAttributeDifference(EveAttribute attrib)
            {
                return this.BestScratchpad.GetAttributeBonus(attrib) - this.BaseScratchpad.GetAttributeBonus(attrib);
            }

            public int GetBaseAttribute(EveAttribute attrib, CharacterInfo character)
            {
                return character.GetBaseAttribute(attrib) + GetBaseAttributeDifference(attrib);
            }

            public string ToString(CharacterInfo character)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("i").Append(this.GetBaseAttribute(EveAttribute.Intelligence, character).ToString()).
                    Append(" p").Append(this.GetBaseAttribute(EveAttribute.Perception, character).ToString()).
                    Append(" c").Append(this.GetBaseAttribute(EveAttribute.Charisma, character).ToString()).
                    Append(" w").Append(this.GetBaseAttribute(EveAttribute.Willpower, character).ToString()).
                    Append(" m").Append(this.GetBaseAttribute(EveAttribute.Memory, character).ToString());

                return builder.ToString();
            }
        }
        #endregion

        private const int MinPerSkill = 5;
        private const int MaxPerSkill = 10;

        /// <summary>
        /// Extract a trainings range from the given trainings array, starting at the given index, ensuring the total training time will be right above the given duration
        /// </summary>
        /// <param name="training"></param>
        /// <param name="character"></param>
        /// <param name="scratchpad"></param>
        /// <param name="startSp"></param>
        /// <param name="maxDuration"></param>
        /// <returns></returns>
        private static SkillTraining[] GetSubTraining(SkillTraining[] training, CharacterInfo character, EveAttributeScratchpad scratchpad, int startSp, TimeSpan maxDuration)
        {
            TimeSpan time = TimeSpan.Zero;
            List<SkillTraining> newTraining = new List<SkillTraining>();
            int cumulativeSkillTotal = startSp;

            // Scroll through the entries
            foreach (var entry in training)
            {
                // Compute training time
                time += entry.Skill.GetTimeSpanForPoints(entry.PointsToTrain, cumulativeSkillTotal, scratchpad, true);
                cumulativeSkillTotal += entry.PointsToTrain;
                scratchpad.ApplyALevelOf(entry.Skill);

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
        /// <param name="startSp"></param>
        /// <returns></returns>
        private static TimeSpan ComputeTotalTime(SkillTraining[] skills, CharacterInfo character, EveAttributeScratchpad scratchpad, int startSp)
        {
            TimeSpan time = TimeSpan.Zero;
            int cumulativeSkillTotal = startSp;

            foreach (var training in skills)
            {
                time += training.Skill.GetTimeSpanForPoints(training.PointsToTrain, cumulativeSkillTotal, scratchpad, true);
                cumulativeSkillTotal += training.PointsToTrain;
                scratchpad.ApplyALevelOf(training.Skill);
            }

            return time;
        }

        /// <summary>
        /// Compute the best possible attributes to fulfill the given trainings array
        /// </summary>
        /// <param name="skills"></param>
        /// <param name="character"></param>
        /// <param name="maxDuration"></param>
        /// <param name="startSp"></param>
        /// <param name="bestTime"></param>
        /// <param name="baseTime"></param>
        /// <returns></returns>
        private static EveAttributeScratchpad Optimize(SkillTraining[] skills, CharacterInfo character, EveAttributeScratchpad baseScratchpad, 
            TimeSpan maxDuration, int startSp, out TimeSpan bestTime, out TimeSpan baseTime)
        {
            baseTime = TimeSpan.MaxValue;
            bestTime = TimeSpan.MaxValue;

            // Get the number of points currently spent into each attribute
            int memBase = MinPerSkill + baseScratchpad.GetAttributeBonus(EveAttribute.Memory) - character.GetBaseAttribute(EveAttribute.Memory);
            int chaBase = MinPerSkill + baseScratchpad.GetAttributeBonus(EveAttribute.Charisma) - character.GetBaseAttribute(EveAttribute.Charisma);
            int wilBase = MinPerSkill + baseScratchpad.GetAttributeBonus(EveAttribute.Willpower) - character.GetBaseAttribute(EveAttribute.Willpower);
            int perBase = MinPerSkill + baseScratchpad.GetAttributeBonus(EveAttribute.Perception) - character.GetBaseAttribute(EveAttribute.Perception);
            int intBase = MinPerSkill + baseScratchpad.GetAttributeBonus(EveAttribute.Intelligence) - character.GetBaseAttribute(EveAttribute.Intelligence);
            int learning = baseScratchpad.LearningLevelBonus;

            // 1. We start with a plan shorter than the given limit. 
            // 2. Then, we scroll through misc attributes combination to find the best possible attributes.
            // 3. Since those new attributes make the training faster, we take a bigger plan, still shorter than the given limit and 
            EveAttributeScratchpad bestScratchpad = baseScratchpad.Clone();
            var training = GetSubTraining(skills, character, baseScratchpad.Clone(), startSp, maxDuration);
            while (true)
            {
                // Recompute base time
                bool foundBetter = false;
                var tempScratchpad = baseScratchpad.Clone();
                baseTime = ComputeTotalTime(training, character, tempScratchpad, startSp);
                bestTime = baseTime;

                // Now, we have the points to spend, let's perform all the
                // combinations (less than 11^4 = 14,641)
                for (int per = 0; per <= MaxPerSkill; per++)
                {
                    // WIL
                    int maxWillpower = EveConstants.SpareAttributePointsOnRemap - per;
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
                                    tempScratchpad.Reset(per + perBase, will + wilBase, intell + intBase, mem + memBase, cha + chaBase, learning);
                                    TimeSpan tempTime = ComputeTotalTime(training, character, tempScratchpad, startSp);

                                    // Compare it to the best time so far
                                    if (tempTime < bestTime)
                                    {
                                        bestTime = tempTime;
                                        foundBetter = true;

                                        // Store the attributes for the best time
                                        bestScratchpad.Reset(per + perBase, will + wilBase, intell + intBase, mem + memBase, cha + chaBase, learning);
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
                training = GetSubTraining(skills, character, bestScratchpad.Clone(), startSp, maxDuration);

                // break if it is not shorter than the previous plan
                if (training.Length == oldTrainingLength) break;
                bestScratchpad = baseScratchpad.Clone();
            }

            // Return the best scratchpad found
            return bestScratchpad;
        }

        /// <summary>
        /// Generate a trainings array from a plan and reampping points
        /// </summary>
        /// <param name="plan">Plan with remapping points</param>
        /// <param name="bestDuration">Best training time</param>
        /// <returns>Computed remapping</returns>
        public static List<Remapping> OptimizeFromPlanAndRemappingPoints(Plan plan, out TimeSpan bestDuration)
        {
            var time = TimeSpan.Zero;
            var limit = TimeSpan.MaxValue;
            var startSp = plan.GrandCharacterInfo.SkillPointTotal;
            var scratchpad = new EveAttributeScratchpad();
            var remappingList = new List<Remapping>();
            var list = new List<SkillTraining>();
            Remapping remapping = null;

            // Scroll through the entries and split it into remappings
            foreach (var entry in plan.Entries)
            {
                if (entry.Skill != null)
                {
                    // Ends the current remapping and start a new one
                    if (entry.Remapping != null)
                    {
                        // Updates the previous remapping and starting time
                        if (remapping != null)
                        {
                            time = remapping.Compute(plan.GrandCharacterInfo, time, limit, startSp);
                            remappingList.Add(remapping);
                        }
                        // At the beginning, we had other trainings
                        else
                        {
                            time = ComputeTotalTime(list.ToArray(), plan.GrandCharacterInfo, scratchpad.Clone(), startSp);
                        }

                        // Updates start sp and learning skills bonuses for the next remapping
                        foreach (var training in list)
                        {
                            startSp += training.PointsToTrain;
                            scratchpad.ApplyALevelOf(training.Skill);
                        }

                        // Creates a new remapping
                        remapping = new Remapping(entry.Remapping, scratchpad.Clone());
                        list = remapping.Skills;
                    }

                    // Add this skill to the training list
                    int sp = entry.Skill.GetPointsForLevelOnly(entry.Level, true);
                    list.Add(new SkillTraining(entry.Skill, sp));
                }
            }

            // Compute the current remapping and adds it to the list
            if (remapping != null)
            {
                time = remapping.Compute(plan.GrandCharacterInfo, time, limit, startSp);
                remappingList.Add(remapping);
            }

            bestDuration = time;
            return remappingList;
        }

        /// <summary>
        /// Compute the best remapping for the first year of this plan
        /// </summary>
        /// <param name="plan">Plan with skills for which to optimize</param>
        /// <returns>Computed remapping</returns>
        public static Remapping OptimizeFromPlan(Plan plan)
        {
            var remapping = new Remapping(null, null);

            // Scroll through the entries and split it into remappings
            foreach (var entry in plan.Entries)
            {
                if (entry.Skill != null)
                {
                    // Add this skill to the training list
                    int sp = entry.Skill.GetPointsForLevelOnly(entry.Level, true);
                    remapping.Skills.Add(new SkillTraining(entry.Skill, sp));
                }
            }

            remapping.Compute(plan.GrandCharacterInfo, TimeSpan.Zero, TimeSpan.FromDays(365.0), plan.GrandCharacterInfo.SkillPointTotal);
            return remapping;
        }

        /// <summary>
        /// Allows to create EVEMon.Common.AttributesOptimizer.Remapping object for a given attributes set
        /// </summary>
        /// <param name="baseRemapping">Optimized remapping on which to base new remapping</param>
        /// <param name="scratchpad">Scratchpad with attributes</param>
        /// <returns>Computed remapping</returns>
        public static Remapping OptimizeManually(CharacterInfo character, Remapping baseRemapping, EveAttributeScratchpad scratchpad)
        {
            var remapping = new Remapping(baseRemapping.Point, baseRemapping.BaseScratchpad);
            remapping.Skills.AddRange(baseRemapping.Skills);

            remapping.ComputeManually(character, baseRemapping.Time, baseRemapping.Limit, baseRemapping.StartSP, baseRemapping.BestScratchpad, scratchpad);
            return remapping;
        }

        /// <summary>
        /// Generate a trainings array from the skills already know by a character
        /// </summary>
        /// <param name="character">Character information</param>
        /// <returns>Computed remapping</returns>
        public static Remapping OptimizeFromCharacter(CharacterInfo character)
        {
            // Create a sorted plan for the learning skills
            var plan = new Plan();
            plan.GrandCharacterInfo = character;
            foreach (var learning in character.SkillGroups["Learning"])
            {
                plan.PlanTo(learning, learning.LastConfirmedLvl);
            }
            PlanSorter.PutOrderedLearningSkillsAhead(plan, false);


            // Remove bonuses from learning skills and add them to the training list
            List<SkillTraining> training = new List<SkillTraining>();
            var scratchpad = new EveAttributeScratchpad();
            foreach (var entry in plan.Entries) 
            {
                if (entry.Skill.Name == "Learning")
                {
                    scratchpad.AdjustLearningLevelBonus(-1);
                }
                else if (entry.Skill.IsLearningSkill)
                {
                    scratchpad.AdjustAttributeBonus(entry.Skill.AttributeModified, -1);
                }

                int sp = entry.Skill.GetPointsForLevelOnly(entry.Level, true);
                training.Add(new SkillTraining(entry.Skill, sp));
            }

            // Add non-training skills after that
            var remapping = new Remapping(null, scratchpad);
            remapping.Skills.AddRange(training);
            foreach (var group in character.SkillGroups.Values)
            {
                if (group.Name != "Learning")
                {
                    foreach (var skill in group)
                    {
                        remapping.Skills.Add(new SkillTraining(skill, skill.CurrentSkillPoints));
                    }
                }
            }

            remapping.Compute(character, TimeSpan.Zero, TimeSpan.FromDays(365.0), 0);
            return remapping;
        }
    }
}
