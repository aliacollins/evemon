using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents the result of a remapping.
    /// </summary>
    public sealed class RemappingResult
    {
        /// <summary>
        /// Constructor without any remapping point associated.
        /// </summary>
        /// <param name="baseScratchpad"></param>
        public RemappingResult(CharacterScratchpad baseScratchpad)
        {
            if (baseScratchpad == null)
                throw new ArgumentNullException("baseScratchpad");

            Skills = new Collection<ISkillLevel>();
            BaseScratchpad = baseScratchpad;
            StartTime = BaseScratchpad.TrainingTime;
        }

        /// <summary>
        /// Constructor for a result bound to a remapping point.
        /// </summary>
        /// <param name="point">Associated remapping point, may be null.</param>
        /// <param name="baseScratchpad"></param>
        public RemappingResult(RemappingPoint point, CharacterScratchpad baseScratchpad)
            : this(baseScratchpad)
        {
            if (point == null)
                throw new ArgumentNullException("point");

            Point = point;
        }

        /// <summary>
        /// Constructor for a manually edited result from a base result.
        /// </summary>
        /// <param name="result">Associated remapping point, may be null.</param>
        /// <param name="bestScratchpad"></param>
        public RemappingResult(RemappingResult result, CharacterScratchpad bestScratchpad)
            : this(result != null ? result.Point : null, result != null ? result.BaseScratchpad : null)
        {
            if (result == null)
                throw new ArgumentNullException("result");

            result.Skills.ToList().ForEach(skillLevel => Skills.Add(skillLevel));
            BestScratchpad = bestScratchpad;
        }

        /// <summary>
        /// Gets the optimized plan.
        /// </summary>
        public Collection<ISkillLevel> Skills { get; private set; }

        /// <summary>
        /// Gets the remapping point associated with that remapping.
        /// May be null if a remapping was automatically added at the beginning of the training.
        /// </summary>
        public RemappingPoint Point { get; private set; }

        /// <summary>
        /// Gets the best scratchpad after the remapping.
        /// </summary>
        public CharacterScratchpad BaseScratchpad { get; private set; }

        /// <summary>
        /// Gets the best scratchpad after the remapping.
        /// </summary>
        public CharacterScratchpad BestScratchpad { get; private set; }

        /// <summary>
        /// Gets the training duration with the best remapping.
        /// </summary>
        public TimeSpan BestDuration { get; private set; }

        /// <summary>
        /// Gets the base training duration before the remapping.
        /// </summary>
        public TimeSpan BaseDuration { get; private set; }

        /// <summary>
        /// Gets the time when this remapping was done.
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
                Append("i").Append(BestScratchpad.Intelligence.Base.ToString(CultureConstants.DefaultCulture)).
                Append(" p").Append(BestScratchpad.Perception.Base.ToString(CultureConstants.DefaultCulture)).
                Append(" c").Append(BestScratchpad.Charisma.Base.ToString(CultureConstants.DefaultCulture)).
                Append(" w").Append(BestScratchpad.Willpower.Base.ToString(CultureConstants.DefaultCulture)).
                Append(" m").Append(BestScratchpad.Memory.Base.ToString(CultureConstants.DefaultCulture));

            return builder.ToString();
        }
    }
}