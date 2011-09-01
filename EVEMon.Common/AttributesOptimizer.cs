using System;
using System.Collections.Generic;
using System.Text;

namespace EVEMon.Common
{
    public static class AttributesOptimizer
    {
        #region Remapping

        /// <summary>
        /// Represents the result of a remapping
        /// </summary>
        public sealed class RemappingResult
        {
            /// <summary>
            /// Constructor without any remapping point associated 
            /// </summary>
            /// <param name="baseScratchpad"></param>
            public RemappingResult(CharacterScratchpad baseScratchpad)
            {
                Skills = new List<ISkillLevel>();
                BaseScratchpad = baseScratchpad;
                StartTime = BaseScratchpad.TrainingTime;
            }

            /// <summary>
            /// Constructor for a result bound to a remapping point
            /// </summary>
            /// <param name="point">Associated remapping point, may be null.</param>
            /// <param name="baseScratchpad"></param>
            public RemappingResult(RemappingPoint point, CharacterScratchpad baseScratchpad)
                : this(baseScratchpad)
            {
                Point = point;
            }

            /// <summary>
            /// Constructor for a manually edited result from a base result.
            /// </summary>
            /// <param name="result">Associated remapping point, may be null.</param>
            /// <param name="bestScratchpad"></param>
            public RemappingResult(RemappingResult result, CharacterScratchpad bestScratchpad)
                : this(result.Point, result.BaseScratchpad)
            {
                Skills.AddRange(result.Skills);
                BestScratchpad = bestScratchpad;
            }

            /// <summary>
            /// Gets the optimized plan
            /// </summary>
            public List<ISkillLevel> Skills { get; private set; }

            /// <summary>
            /// Gets the remapping point associated with that remapping. May be null if a remapping was automatically added at the beginning of the training
            /// </summary>
            public RemappingPoint Point { get; private set; }

            /// <summary>
            /// Gets the best scratchpad after the remapping
            /// </summary>
            public CharacterScratchpad BaseScratchpad { get; private set; }

            /// <summary>
            /// Gets the best scratchpad after the remapping
            /// </summary>
            public CharacterScratchpad BestScratchpad { get; private set; }

            /// <summary>
            /// Gets the training duration with the best remapping
            /// </summary>
            public TimeSpan BestDuration { get; private set; }

            /// <summary>
            /// Gets the base training duration before the remapping
            /// </summary>
            public TimeSpan BaseDuration { get; private set; }

            /// <summary>
            /// Gets the time when this remapping was done
            /// </summary>
            public TimeSpan StartTime { get; private set; }

            /// <summary>
            /// Computes an optimized scratchpad, then call <see cref="Update"/>.
            /// </summary>
            /// <param name="maxDuration">The max duration to take into account for optimization.</param>
            /// <returns></returns>
            public void Optimize(TimeSpan maxDuration)
            {
                BestScratchpad = AttributesOptimizer.Optimize(Skills, BaseScratchpad, maxDuration);
                Update();
            }

            /// <summary>
            /// Updates the times and, when any, the associated remapping point.
            /// </summary>
            /// <returns></returns>
            public void Update()
            {
                // Optimize
                BaseDuration = BaseScratchpad.After(Skills).TrainingTime.Subtract(StartTime);
                BestDuration = BestScratchpad.After(Skills).TrainingTime.Subtract(StartTime);

                // Update the underlying remapping point
                if (Point != null)
                    Point.SetBaseAttributes(BestScratchpad, BaseScratchpad);
            }

            /// <summary>
            /// Gets a string representation of this object.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                StringBuilder builder = new StringBuilder().
                    Append("i").Append(BestScratchpad.Intelligence.Base.ToString()).
                    Append(" p").Append(BestScratchpad.Perception.Base.ToString()).
                    Append(" c").Append(BestScratchpad.Charisma.Base.ToString()).
                    Append(" w").Append(BestScratchpad.Willpower.Base.ToString()).
                    Append(" m").Append(BestScratchpad.Memory.Base.ToString());

                return builder.ToString();
            }
        }

        #endregion


        #region Computations

        /// <summary>
        /// Compute the best possible attributes to fulfill the given trainings array
        /// </summary>
        /// <param name="skills"></param>
        /// <param name="baseScratchpad"></param>
        /// <param name="maxDuration"></param>
        /// <returns></returns>
        private static CharacterScratchpad Optimize<T>(IEnumerable<T> skills, CharacterScratchpad baseScratchpad,
                                                       TimeSpan maxDuration)
            where T : ISkillLevel
        {
            CharacterScratchpad bestScratchpad = new CharacterScratchpad(baseScratchpad);
            CharacterScratchpad tempScratchpad = new CharacterScratchpad(baseScratchpad);
            TimeSpan baseTime = baseScratchpad.TrainingTime;
            TimeSpan bestTime = TimeSpan.MaxValue;
            int bestSkillCount = 0;

            // Now, we have the points to spend, let's perform all the
            // combinations (less than 11^4 = 14,641)
            for (int per = 0; per <= EveConstants.MaxRemappablePointsPerAttribute; per++)
            {
                // WIL
                int maxWillpower = EveConstants.SpareAttributePointsOnRemap - per;
                for (int will = 0; will <= maxWillpower && will <= EveConstants.MaxRemappablePointsPerAttribute; will++)
                {
                    // INT
                    int maxIntelligence = maxWillpower - will;
                    for (int intell = 0;
                         intell <= maxIntelligence && intell <= EveConstants.MaxRemappablePointsPerAttribute;
                         intell++)
                    {
                        // MEM
                        int maxMemory = maxIntelligence - intell;
                        for (int mem = 0; mem <= maxMemory && mem <= EveConstants.MaxRemappablePointsPerAttribute; mem++)
                        {
                            // CHA
                            int cha = maxMemory - mem;

                            // Reject invalid combinations
                            if (cha > EveConstants.MaxRemappablePointsPerAttribute)
                                continue;

                            // Resets the scratchpad
                            tempScratchpad.Reset();

                            // Set new attributes
                            tempScratchpad.Memory.Base = mem + EveConstants.CharacterBaseAttributePoints;
                            tempScratchpad.Charisma.Base = cha + EveConstants.CharacterBaseAttributePoints;
                            tempScratchpad.Willpower.Base = will + EveConstants.CharacterBaseAttributePoints;
                            tempScratchpad.Perception.Base = per + EveConstants.CharacterBaseAttributePoints;
                            tempScratchpad.Intelligence.Base = intell + EveConstants.CharacterBaseAttributePoints;

                            // Train skills
                            int tempSkillCount = 0;
                            foreach (T skill in skills)
                            {
                                tempSkillCount++;
                                tempScratchpad.Train(skill);

                                // Did it go over max duration ?
                                if (tempScratchpad.TrainingTime - baseTime > maxDuration)
                                    break;

                                // Did it go over the best time so far without training more skills ?
                                if (tempSkillCount <= bestSkillCount && tempScratchpad.TrainingTime > bestTime)
                                    break;
                            }

                            // Did it manage to train more skills before the max duration, 
                            // or did it train the same number of skills in a lesser time ?
                            if (tempSkillCount <= bestSkillCount &&
                                (tempSkillCount != bestSkillCount || tempScratchpad.TrainingTime >= bestTime))
                                continue;

                            bestScratchpad.Reset();
                            bestScratchpad.Memory.Base = tempScratchpad.Memory.Base;
                            bestScratchpad.Charisma.Base = tempScratchpad.Charisma.Base;
                            bestScratchpad.Willpower.Base = tempScratchpad.Willpower.Base;
                            bestScratchpad.Perception.Base = tempScratchpad.Perception.Base;
                            bestScratchpad.Intelligence.Base = tempScratchpad.Intelligence.Base;
                            bestTime = tempScratchpad.TrainingTime;
                            bestSkillCount = tempSkillCount;
                        }
                    }
                }
            }

            // Return the best scratchpad found
            return bestScratchpad;
        }

        /// <summary>
        /// Generate a trainings array from a plan
        /// </summary>
        /// <param name="plan"></param>
        /// <returns></returns>
        public static List<RemappingResult> OptimizeFromPlanAndRemappingPoints(BasePlan plan)
        {
            List<RemappingResult> results = GetResultsFromRemappingPoints(plan);
            foreach (RemappingResult result in results)
            {
                result.Optimize(TimeSpan.MaxValue);
            }
            return results;
        }

        /// <summary>
        /// Gets the list of remapping results from a plan.
        /// </summary>
        /// <param name="plan"></param>
        /// <returns></returns>
        public static List<RemappingResult> GetResultsFromRemappingPoints(BasePlan plan)
        {
            CharacterScratchpad scratchpad = new CharacterScratchpad(plan.Character.After(plan.ChosenImplantSet));
            List<RemappingResult> remappingList = new List<RemappingResult>();
            List<ISkillLevel> list = new List<ISkillLevel>();

            // Scroll through the entries and split it into remappings
            foreach (PlanEntry entry in plan)
            {
                // Ends the current remapping and start a new one
                if (entry.Remapping != null)
                {
                    // Creates a new remapping
                    RemappingResult remapping = new RemappingResult(entry.Remapping, scratchpad.Clone());
                    remappingList.Add(remapping);
                    list = remapping.Skills;
                }

                // Add this skill to the training list
                scratchpad.Train(entry);
                list.Add(entry);
            }

            // Return
            return remappingList;
        }

        /// <summary>
        /// Compute the best remapping for the first year of this plan.
        /// </summary>
        /// <param name="plan"></param>
        /// <returns></returns>
        public static RemappingResult OptimizeFromFirstYearOfPlan(BasePlan plan)
        {
            RemappingResult remapping = new RemappingResult(new CharacterScratchpad(plan.Character.After(plan.ChosenImplantSet)));

            // Scroll through the entries and split it into remappings
            foreach (PlanEntry entry in plan)
            {
                remapping.Skills.Add(entry);
            }

            // Compute
            remapping.Optimize(TimeSpan.FromDays(365.0));
            return remapping;
        }

        /// <summary>
        /// Generate a trainings array from the skills already know by a character.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="plan">The plan.</param>
        /// <returns></returns>
        public static RemappingResult OptimizeFromCharacter(Character character, BasePlan plan)
        {
            // Create a character without any skill
            CharacterScratchpad scratchpad = new CharacterScratchpad(character.After(plan.ChosenImplantSet));
            scratchpad.ClearSkills();

            // Create a new plan
            Plan newPlan = new Plan(scratchpad);

            // Add all trained skill levels that the character has trained so far
            foreach (Skill skill in character.Skills)
            {
                newPlan.PlanTo(skill, skill.Level);
            }

            // Create a new remapping
            RemappingResult remapping = new RemappingResult(scratchpad);

            // Add those skills to the remapping
            foreach (PlanEntry entry in newPlan)
            {
                remapping.Skills.Add(entry);
            }

            // Optimize
            remapping.Optimize(TimeSpan.MaxValue);
            return remapping;
        }

        #endregion
    }
}