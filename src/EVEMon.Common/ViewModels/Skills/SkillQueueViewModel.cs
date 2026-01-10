using System;
using System.Collections.ObjectModel;
using System.Linq;
using EVEMon.Common.Abstractions.Events;
using EVEMon.Common.Models;

namespace EVEMon.Common.ViewModels.Skills
{
    /// <summary>
    /// ViewModel for a character's skill queue.
    /// </summary>
    public class SkillQueueViewModel : ViewModelBase
    {
        private readonly CCPCharacter _character;
        private bool _isTraining;
        private bool _isPaused;
        private DateTime _endTime;
        private string _totalTimeRemaining;
        private bool _lessThanWarningThreshold;
        private int _warningThresholdDays;
        private QueuedSkillViewModel _currentlyTraining;

        /// <summary>
        /// Initializes a new instance of <see cref="SkillQueueViewModel"/>.
        /// </summary>
        /// <param name="character">The CCP character.</param>
        public SkillQueueViewModel(CCPCharacter character)
        {
            _character = character ?? throw new ArgumentNullException(nameof(character));
            Skills = new ObservableCollection<QueuedSkillViewModel>();
            _warningThresholdDays = Common.Settings.UI.MainWindow.SkillQueueWarningThresholdDays;

            Refresh();

            // Subscribe to events
            Subscribe<CharacterSkillQueueUpdatedEvent>(OnSkillQueueUpdated);
            Subscribe<SecondTickEvent>(OnSecondTick);
            Subscribe<SettingsChangedEvent>(OnSettingsChanged);
        }

        /// <summary>
        /// Gets the collection of queued skills.
        /// </summary>
        public ObservableCollection<QueuedSkillViewModel> Skills { get; }

        /// <summary>
        /// Gets the number of skills in the queue.
        /// </summary>
        public int Count => Skills.Count;

        /// <summary>
        /// Gets whether the queue is currently training.
        /// </summary>
        public bool IsTraining
        {
            get => _isTraining;
            private set => SetProperty(ref _isTraining, value);
        }

        /// <summary>
        /// Gets whether the queue is paused.
        /// </summary>
        public bool IsPaused
        {
            get => _isPaused;
            private set => SetProperty(ref _isPaused, value);
        }

        /// <summary>
        /// Gets the queue end time (UTC).
        /// </summary>
        public DateTime EndTime
        {
            get => _endTime;
            private set => SetProperty(ref _endTime, value);
        }

        /// <summary>
        /// Gets the total time remaining in the queue.
        /// </summary>
        public string TotalTimeRemaining
        {
            get => _totalTimeRemaining;
            private set => SetProperty(ref _totalTimeRemaining, value);
        }

        /// <summary>
        /// Gets whether the queue has less than the warning threshold worth of training.
        /// </summary>
        public bool LessThanWarningThreshold
        {
            get => _lessThanWarningThreshold;
            private set => SetProperty(ref _lessThanWarningThreshold, value);
        }

        /// <summary>
        /// Gets the warning threshold in days.
        /// </summary>
        public int WarningThresholdDays
        {
            get => _warningThresholdDays;
            private set => SetProperty(ref _warningThresholdDays, value);
        }

        /// <summary>
        /// Gets the currently training skill ViewModel.
        /// </summary>
        public QueuedSkillViewModel CurrentlyTraining
        {
            get => _currentlyTraining;
            private set => SetProperty(ref _currentlyTraining, value);
        }

        /// <summary>
        /// Gets whether the queue is empty.
        /// </summary>
        public bool IsEmpty => Skills.Count == 0;

        /// <summary>
        /// Gets the status text for the queue.
        /// </summary>
        public string StatusText
        {
            get
            {
                if (IsEmpty)
                    return "No skills in queue";
                if (IsPaused)
                    return $"Paused - {Count} skill{(Count != 1 ? "s" : "")} in queue";
                if (IsTraining)
                    return $"Training - {Count} skill{(Count != 1 ? "s" : "")} in queue";
                return $"{Count} skill{(Count != 1 ? "s" : "")} in queue";
            }
        }

        /// <summary>
        /// Refreshes all property values from the model.
        /// </summary>
        public void Refresh()
        {
            var queue = _character.SkillQueue;
            if (queue == null)
            {
                Skills.Clear();
                IsTraining = false;
                IsPaused = false;
                EndTime = DateTime.UtcNow;
                TotalTimeRemaining = string.Empty;
                LessThanWarningThreshold = false;
                CurrentlyTraining = null;
                OnPropertiesChanged(nameof(Count), nameof(IsEmpty), nameof(StatusText));
                return;
            }

            IsTraining = queue.IsTraining;
            IsPaused = queue.IsPaused;
            EndTime = queue.EndTime;
            LessThanWarningThreshold = queue.LessThanWarningThreshold;
            WarningThresholdDays = Common.Settings.UI.MainWindow.SkillQueueWarningThresholdDays;

            // Sync the skills collection
            SyncSkillsCollection(queue);

            // Update currently training reference
            CurrentlyTraining = Skills.FirstOrDefault();

            // Update time remaining
            UpdateTimeRemaining();

            OnPropertiesChanged(nameof(Count), nameof(IsEmpty), nameof(StatusText));
        }

        private void SyncSkillsCollection(SkillQueue queue)
        {
            // Simple approach: clear and rebuild
            // For better performance with large queues, could do incremental updates
            foreach (var vm in Skills)
            {
                vm.Dispose();
            }
            Skills.Clear();

            foreach (var queuedSkill in queue)
            {
                Skills.Add(new QueuedSkillViewModel(queuedSkill));
            }
        }

        private void UpdateTimeRemaining()
        {
            if (!IsTraining || IsEmpty)
            {
                TotalTimeRemaining = string.Empty;
                return;
            }

            var remaining = EndTime - DateTime.UtcNow;
            if (remaining <= TimeSpan.Zero)
            {
                TotalTimeRemaining = "Complete";
            }
            else
            {
                TotalTimeRemaining = FormatTimeSpan(remaining);
            }

            // Update individual skill times
            foreach (var skill in Skills)
            {
                skill.UpdateRemainingTime();
            }

            // Update warning threshold status
            var queue = _character.SkillQueue;
            if (queue != null)
            {
                LessThanWarningThreshold = queue.LessThanWarningThreshold;
            }
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

        private void OnSkillQueueUpdated(CharacterSkillQueueUpdatedEvent e)
        {
            if (e.Character?.Guid == _character.Guid)
            {
                Refresh();
            }
        }

        private void OnSecondTick(SecondTickEvent e)
        {
            if (IsTraining)
            {
                UpdateTimeRemaining();
            }
        }

        private void OnSettingsChanged(SettingsChangedEvent e)
        {
            var newThreshold = Common.Settings.UI.MainWindow.SkillQueueWarningThresholdDays;
            if (WarningThresholdDays != newThreshold)
            {
                WarningThresholdDays = newThreshold;
                var queue = _character.SkillQueue;
                if (queue != null)
                {
                    LessThanWarningThreshold = queue.LessThanWarningThreshold;
                }
            }
        }

        /// <inheritdoc />
        protected override void OnDisposing()
        {
            foreach (var skill in Skills)
            {
                skill.Dispose();
            }
            Skills.Clear();

            base.OnDisposing();
        }
    }
}
