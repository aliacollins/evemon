using System;
using EVEMon.Common.Models;

namespace EVEMon.Common.ViewModels.Skills
{
    /// <summary>
    /// ViewModel for a single skill.
    /// </summary>
    public class SkillViewModel : ViewModelBase
    {
        private readonly Skill _skill;
        private bool _isCharacterAlpha;

        /// <summary>
        /// Initializes a new instance of <see cref="SkillViewModel"/>.
        /// </summary>
        /// <param name="skill">The skill model.</param>
        /// <param name="isCharacterAlpha">True if the character is an Alpha clone.</param>
        public SkillViewModel(Skill skill, bool isCharacterAlpha = false)
        {
            _skill = skill ?? throw new ArgumentNullException(nameof(skill));
            _isCharacterAlpha = isCharacterAlpha;
            Refresh();
        }

        /// <summary>
        /// Gets the skill ID.
        /// </summary>
        public int ID => _skill.ID;

        /// <summary>
        /// Gets the skill name.
        /// </summary>
        public string Name => _skill.Name;

        /// <summary>
        /// Gets the skill description.
        /// </summary>
        public string Description => _skill.Description;

        /// <summary>
        /// Gets the current skill level (0-5).
        /// </summary>
        public long Level => _skill.Level;

        /// <summary>
        /// Gets the skill level in Roman numerals.
        /// </summary>
        public string RomanLevel => _skill.RomanLevel;

        /// <summary>
        /// Gets the display name with level.
        /// </summary>
        public string DisplayName => $"{Name} {RomanLevel}";

        /// <summary>
        /// Gets the current skill points.
        /// </summary>
        public long SkillPoints => _skill.SkillPoints;

        /// <summary>
        /// Gets formatted skill points.
        /// </summary>
        public string SkillPointsFormatted => _skill.SkillPoints.ToString("N0");

        /// <summary>
        /// Gets the skill rank.
        /// </summary>
        public long Rank => _skill.Rank;

        /// <summary>
        /// Gets the primary attribute.
        /// </summary>
        public string PrimaryAttribute => _skill.PrimaryAttribute.ToString();

        /// <summary>
        /// Gets the secondary attribute.
        /// </summary>
        public string SecondaryAttribute => _skill.SecondaryAttribute.ToString();

        /// <summary>
        /// Gets the training speed in SP/hour.
        /// </summary>
        public int SkillPointsPerHour => _skill.SkillPointsPerHour;

        /// <summary>
        /// Gets whether the skill is known.
        /// </summary>
        public bool IsKnown => _skill.IsKnown;

        /// <summary>
        /// Gets whether the skill is currently training.
        /// </summary>
        public bool IsTraining => _skill.IsTraining;

        /// <summary>
        /// Gets whether the skill is queued.
        /// </summary>
        public bool IsQueued => _skill.IsQueued;

        /// <summary>
        /// Gets whether the skill is partially trained.
        /// </summary>
        public bool IsPartiallyTrained => _skill.IsPartiallyTrained;

        /// <summary>
        /// Gets the fraction completed toward the next level (0.0 - 1.0).
        /// </summary>
        public float FractionCompleted => _skill.FractionCompleted;

        /// <summary>
        /// Gets the completion percentage (0 - 100).
        /// </summary>
        public double PercentCompleted => _skill.PercentCompleted;

        /// <summary>
        /// Gets whether all prerequisites are met.
        /// </summary>
        public bool ArePrerequisitesMet => _skill.ArePrerequisitesMet;

        /// <summary>
        /// Gets whether this skill is trainable by Alpha clones.
        /// </summary>
        public bool IsAlphaFriendly => _skill.StaticData?.AlphaFriendly ?? false;

        /// <summary>
        /// Gets the maximum level an Alpha clone can train this skill to.
        /// 0 means not trainable by Alpha at all.
        /// </summary>
        public long AlphaLimit => _skill.StaticData?.AlphaLimit ?? 0;

        /// <summary>
        /// Gets whether this skill is Omega-only (not trainable by Alpha).
        /// </summary>
        public bool IsOmegaOnly => !IsAlphaFriendly;

        /// <summary>
        /// Gets whether the current level exceeds the Alpha limit.
        /// </summary>
        public bool IsAboveAlphaLimit => IsAlphaFriendly && Level > AlphaLimit;

        /// <summary>
        /// Gets or sets whether the character owning this skill is an Alpha clone.
        /// </summary>
        public bool IsCharacterAlpha
        {
            get => _isCharacterAlpha;
            set
            {
                if (SetProperty(ref _isCharacterAlpha, value))
                {
                    // Notify properties that depend on Alpha status
                    OnPropertiesChanged(nameof(IsRestrictedForAlpha), nameof(ShowYellowBoxes));
                }
            }
        }

        /// <summary>
        /// Gets whether this skill is restricted for Alpha (Omega-only skill while character is Alpha).
        /// </summary>
        public bool IsRestrictedForAlpha => IsCharacterAlpha && IsOmegaOnly;

        /// <summary>
        /// Gets whether to show yellow/gold boxes for this skill.
        /// True when character is Alpha and skill is Omega-only.
        /// </summary>
        public bool ShowYellowBoxes => IsRestrictedForAlpha;

        /// <summary>
        /// Gets the time remaining to train to next level.
        /// </summary>
        public TimeSpan TimeToNextLevel => _skill.GetLeftTrainingTimeToNextLevel;

        /// <summary>
        /// Gets the formatted time to next level.
        /// </summary>
        public string TimeToNextLevelFormatted => FormatTimeSpan(TimeToNextLevel);

        /// <summary>
        /// Refreshes all property values from the model.
        /// </summary>
        public void Refresh()
        {
            OnPropertiesChanged(
                nameof(Level),
                nameof(RomanLevel),
                nameof(DisplayName),
                nameof(SkillPoints),
                nameof(SkillPointsFormatted),
                nameof(IsKnown),
                nameof(IsTraining),
                nameof(IsQueued),
                nameof(IsPartiallyTrained),
                nameof(FractionCompleted),
                nameof(PercentCompleted),
                nameof(TimeToNextLevel),
                nameof(TimeToNextLevelFormatted)
            );
        }

        private static string FormatTimeSpan(TimeSpan span)
        {
            if (span <= TimeSpan.Zero)
                return "Complete";

            if (span.TotalDays >= 1)
                return $"{(int)span.TotalDays}d {span.Hours}h";
            if (span.TotalHours >= 1)
                return $"{span.Hours}h {span.Minutes}m";
            if (span.TotalMinutes >= 1)
                return $"{span.Minutes}m {span.Seconds}s";
            return $"{span.Seconds}s";
        }
    }
}
