using System;
using EVEMon.Common.Models;

namespace EVEMon.Common.ViewModels.Skills
{
    /// <summary>
    /// ViewModel for a single queued skill in the training queue.
    /// </summary>
    public class QueuedSkillViewModel : ViewModelBase
    {
        private readonly QueuedSkill _queuedSkill;

        // Cached property values
        private string _skillName;
        private int _level;
        private string _levelRoman;
        private DateTime _startTime;
        private DateTime _endTime;
        private string _remainingTime;
        private float _fractionCompleted;
        private bool _isTraining;
        private bool _isCompleted;
        private int _startSP;
        private int _endSP;
        private int _currentSP;
        private double _skillPointsPerHour;

        /// <summary>
        /// Initializes a new instance of <see cref="QueuedSkillViewModel"/>.
        /// </summary>
        /// <param name="queuedSkill">The queued skill model.</param>
        public QueuedSkillViewModel(QueuedSkill queuedSkill)
        {
            _queuedSkill = queuedSkill ?? throw new ArgumentNullException(nameof(queuedSkill));
            Refresh();
        }

        /// <summary>
        /// Gets the underlying model.
        /// </summary>
        public QueuedSkill Model => _queuedSkill;

        /// <summary>
        /// Gets the skill name.
        /// </summary>
        public string SkillName
        {
            get => _skillName;
            private set => SetProperty(ref _skillName, value);
        }

        /// <summary>
        /// Gets the target level.
        /// </summary>
        public int Level
        {
            get => _level;
            private set => SetProperty(ref _level, value);
        }

        /// <summary>
        /// Gets the level as a Roman numeral.
        /// </summary>
        public string LevelRoman
        {
            get => _levelRoman;
            private set => SetProperty(ref _levelRoman, value);
        }

        /// <summary>
        /// Gets the display name (skill name + level).
        /// </summary>
        public string DisplayName => $"{SkillName} {LevelRoman}";

        /// <summary>
        /// Gets the training start time (UTC).
        /// </summary>
        public DateTime StartTime
        {
            get => _startTime;
            private set => SetProperty(ref _startTime, value);
        }

        /// <summary>
        /// Gets the training end time (UTC).
        /// </summary>
        public DateTime EndTime
        {
            get => _endTime;
            private set => SetProperty(ref _endTime, value);
        }

        /// <summary>
        /// Gets the formatted remaining time.
        /// </summary>
        public string RemainingTime
        {
            get => _remainingTime;
            private set => SetProperty(ref _remainingTime, value);
        }

        /// <summary>
        /// Gets the fraction completed (0 to 1).
        /// </summary>
        public float FractionCompleted
        {
            get => _fractionCompleted;
            private set => SetProperty(ref _fractionCompleted, value);
        }

        /// <summary>
        /// Gets the completion percentage (0 to 100).
        /// </summary>
        public int CompletionPercentage => (int)(FractionCompleted * 100);

        /// <summary>
        /// Gets whether this skill is currently training.
        /// </summary>
        public bool IsTraining
        {
            get => _isTraining;
            private set => SetProperty(ref _isTraining, value);
        }

        /// <summary>
        /// Gets whether this skill has completed training.
        /// </summary>
        public bool IsCompleted
        {
            get => _isCompleted;
            private set => SetProperty(ref _isCompleted, value);
        }

        /// <summary>
        /// Gets the starting skill points.
        /// </summary>
        public int StartSP
        {
            get => _startSP;
            private set => SetProperty(ref _startSP, value);
        }

        /// <summary>
        /// Gets the ending skill points.
        /// </summary>
        public int EndSP
        {
            get => _endSP;
            private set => SetProperty(ref _endSP, value);
        }

        /// <summary>
        /// Gets the current estimated skill points.
        /// </summary>
        public int CurrentSP
        {
            get => _currentSP;
            private set => SetProperty(ref _currentSP, value);
        }

        /// <summary>
        /// Gets the skill points required for this level.
        /// </summary>
        public int SkillPointsRequired => EndSP - StartSP;

        /// <summary>
        /// Gets the training rate in SP/hour.
        /// </summary>
        public double SkillPointsPerHour
        {
            get => _skillPointsPerHour;
            private set => SetProperty(ref _skillPointsPerHour, value);
        }

        /// <summary>
        /// Refreshes all property values from the model.
        /// </summary>
        public void Refresh()
        {
            SkillName = _queuedSkill.SkillName;
            Level = _queuedSkill.Level;
            LevelRoman = GetRomanNumeral(_queuedSkill.Level);
            StartTime = _queuedSkill.StartTime;
            EndTime = _queuedSkill.EndTime;
            FractionCompleted = _queuedSkill.FractionCompleted;
            IsTraining = _queuedSkill.IsTraining;
            IsCompleted = _queuedSkill.IsCompleted;
            StartSP = _queuedSkill.StartSP;
            EndSP = _queuedSkill.EndSP;
            CurrentSP = _queuedSkill.CurrentSP;
            SkillPointsPerHour = _queuedSkill.SkillPointsPerHour;

            UpdateRemainingTime();

            OnPropertiesChanged(
                nameof(DisplayName),
                nameof(CompletionPercentage),
                nameof(SkillPointsRequired));
        }

        /// <summary>
        /// Updates the remaining time display.
        /// </summary>
        public void UpdateRemainingTime()
        {
            var remaining = _queuedSkill.RemainingTime;
            if (remaining <= TimeSpan.Zero)
            {
                RemainingTime = "Complete";
                IsCompleted = true;
            }
            else
            {
                RemainingTime = FormatTimeSpan(remaining);
            }

            FractionCompleted = _queuedSkill.FractionCompleted;
            CurrentSP = _queuedSkill.CurrentSP;
            OnPropertyChanged(nameof(CompletionPercentage));
        }

        private static string FormatTimeSpan(TimeSpan span)
        {
            if (span.TotalDays >= 1)
            {
                return $"{(int)span.TotalDays}d {span.Hours}h {span.Minutes}m";
            }
            if (span.TotalHours >= 1)
            {
                return $"{span.Hours}h {span.Minutes}m {span.Seconds}s";
            }
            if (span.TotalMinutes >= 1)
            {
                return $"{span.Minutes}m {span.Seconds}s";
            }
            return $"{span.Seconds}s";
        }

        private static string GetRomanNumeral(int level)
        {
            return level switch
            {
                1 => "I",
                2 => "II",
                3 => "III",
                4 => "IV",
                5 => "V",
                _ => level.ToString()
            };
        }
    }
}
