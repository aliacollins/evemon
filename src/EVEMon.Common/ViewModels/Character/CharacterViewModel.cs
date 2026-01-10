using System;
using EVEMon.Common.Abstractions.Events;
using EVEMon.Common.Models;
using EVEMon.Common.ViewModels.Skills;

namespace EVEMon.Common.ViewModels.Character
{
    /// <summary>
    /// ViewModel for a single character, exposing data for UI binding.
    /// </summary>
    public class CharacterViewModel : ViewModelBase
    {
        private readonly Models.Character _character;
        private SkillQueueViewModel _skillQueue;
        private SkillsListViewModel _skillsList;

        // Cached property values
        private string _name;
        private long _skillPoints;
        private int _knownSkillCount;
        private string _corporationName;
        private string _allianceName;
        private decimal _balance;
        private string _shipTypeName;
        private string _locationName;
        private bool _isTraining;
        private string _trainingSkillName;
        private string _trainingTimeRemaining;
        private string _skillQueueTimeRemaining;
        private int _skillQueueCount;
        private double _securityStatus;
        private string _label;

        /// <summary>
        /// Initializes a new instance of <see cref="CharacterViewModel"/>.
        /// </summary>
        /// <param name="character">The character model.</param>
        public CharacterViewModel(Models.Character character)
        {
            _character = character ?? throw new ArgumentNullException(nameof(character));

            // Initialize the skill queue ViewModel
            if (_character is CCPCharacter ccpCharacter)
            {
                _skillQueue = new SkillQueueViewModel(ccpCharacter);
            }

            // Initialize the skills list ViewModel
            _skillsList = new SkillsListViewModel(_character);

            // Load initial values
            Refresh();

            // Subscribe to relevant events
            Subscribe<CharacterUpdatedEvent>(OnCharacterUpdated);
            Subscribe<CharacterSkillQueueUpdatedEvent>(OnSkillQueueUpdated);
            Subscribe<SecondTickEvent>(OnSecondTick);
        }

        /// <summary>
        /// Gets the character's GUID.
        /// </summary>
        public Guid CharacterGuid => _character.Guid;

        /// <summary>
        /// Gets the character's EVE ID.
        /// </summary>
        public long CharacterID => _character.CharacterID;

        /// <summary>
        /// Gets the underlying character model.
        /// </summary>
        public Models.Character Character => _character;

        /// <summary>
        /// Gets the character's name.
        /// </summary>
        public string Name
        {
            get => _name;
            private set => SetProperty(ref _name, value);
        }

        /// <summary>
        /// Gets the character's total skill points.
        /// </summary>
        public long SkillPoints
        {
            get => _skillPoints;
            private set => SetProperty(ref _skillPoints, value);
        }

        /// <summary>
        /// Gets the formatted skill points string.
        /// </summary>
        public string SkillPointsFormatted => $"{SkillPoints:N0} SP";

        /// <summary>
        /// Gets the number of known skills.
        /// </summary>
        public int KnownSkillCount
        {
            get => _knownSkillCount;
            private set => SetProperty(ref _knownSkillCount, value);
        }

        /// <summary>
        /// Gets the corporation name.
        /// </summary>
        public string CorporationName
        {
            get => _corporationName;
            private set => SetProperty(ref _corporationName, value);
        }

        /// <summary>
        /// Gets the alliance name.
        /// </summary>
        public string AllianceName
        {
            get => _allianceName;
            private set => SetProperty(ref _allianceName, value);
        }

        /// <summary>
        /// Gets the wallet balance.
        /// </summary>
        public decimal Balance
        {
            get => _balance;
            private set => SetProperty(ref _balance, value);
        }

        /// <summary>
        /// Gets the formatted balance string.
        /// </summary>
        public string BalanceFormatted => $"{Balance:N2} ISK";

        /// <summary>
        /// Gets the current ship type name.
        /// </summary>
        public string ShipTypeName
        {
            get => _shipTypeName;
            private set => SetProperty(ref _shipTypeName, value);
        }

        /// <summary>
        /// Gets the current location name.
        /// </summary>
        public string LocationName
        {
            get => _locationName;
            private set => SetProperty(ref _locationName, value);
        }

        /// <summary>
        /// Gets whether the character is currently training.
        /// </summary>
        public bool IsTraining
        {
            get => _isTraining;
            private set => SetProperty(ref _isTraining, value);
        }

        /// <summary>
        /// Gets the name of the currently training skill.
        /// </summary>
        public string TrainingSkillName
        {
            get => _trainingSkillName;
            private set => SetProperty(ref _trainingSkillName, value);
        }

        /// <summary>
        /// Gets the time remaining for the current skill.
        /// </summary>
        public string TrainingTimeRemaining
        {
            get => _trainingTimeRemaining;
            private set => SetProperty(ref _trainingTimeRemaining, value);
        }

        /// <summary>
        /// Gets the total time remaining in the skill queue.
        /// </summary>
        public string SkillQueueTimeRemaining
        {
            get => _skillQueueTimeRemaining;
            private set => SetProperty(ref _skillQueueTimeRemaining, value);
        }

        /// <summary>
        /// Gets the number of skills in the queue.
        /// </summary>
        public int SkillQueueCount
        {
            get => _skillQueueCount;
            private set => SetProperty(ref _skillQueueCount, value);
        }

        /// <summary>
        /// Gets the security status.
        /// </summary>
        public double SecurityStatus
        {
            get => _securityStatus;
            private set => SetProperty(ref _securityStatus, value);
        }

        /// <summary>
        /// Gets the formatted security status.
        /// </summary>
        public string SecurityStatusFormatted => $"Security: {SecurityStatus:N2}";

        /// <summary>
        /// Gets whether the character has an alliance.
        /// </summary>
        public bool HasAlliance => !string.IsNullOrEmpty(AllianceName);

        /// <summary>
        /// Gets the current skill training progress (0-100).
        /// </summary>
        public double CurrentSkillProgress
        {
            get
            {
                if (_character is CCPCharacter ccpChar)
                {
                    var currentSkill = ccpChar.CurrentlyTrainingSkill;
                    if (currentSkill != null)
                    {
                        return currentSkill.FractionCompleted * 100.0;
                    }
                }
                return 0;
            }
        }

        /// <summary>
        /// Gets the formatted skill queue end time.
        /// </summary>
        public string SkillQueueEndTime
        {
            get
            {
                if (_character is CCPCharacter ccpChar)
                {
                    var queue = ccpChar.SkillQueue;
                    if (queue != null && queue.Count > 0 && queue.EndTime > DateTime.UtcNow)
                    {
                        return queue.EndTime.ToLocalTime().ToString("g");
                    }
                }
                return "N/A";
            }
        }

        /// <summary>
        /// Gets the character's custom label.
        /// </summary>
        public string Label
        {
            get => _label;
            private set => SetProperty(ref _label, value);
        }

        /// <summary>
        /// Gets the skill queue ViewModel.
        /// </summary>
        public SkillQueueViewModel SkillQueue => _skillQueue;

        /// <summary>
        /// Gets the skills list ViewModel.
        /// </summary>
        public SkillsListViewModel SkillsList => _skillsList;

        /// <summary>
        /// Gets whether this is a CCP character (has ESI data).
        /// </summary>
        public bool IsCCPCharacter => _character is CCPCharacter;

        /// <summary>
        /// Gets whether this character is an Alpha clone.
        /// </summary>
        public bool IsAlpha => _character.EffectiveCharacterStatus == AccountStatus.Alpha;

        /// <summary>
        /// Gets whether this character is an Omega clone.
        /// </summary>
        public bool IsOmega => _character.EffectiveCharacterStatus == AccountStatus.Omega;

        /// <summary>
        /// Gets the clone state display text.
        /// </summary>
        public string CloneStateText => _character.EffectiveCharacterStatus switch
        {
            AccountStatus.Alpha => "Alpha Clone",
            AccountStatus.Omega => "Omega Clone",
            _ => "Unknown"
        };

        /// <summary>
        /// Refreshes all property values from the model.
        /// </summary>
        public void Refresh()
        {
            Name = _character.Name;
            SkillPoints = _character.SkillPoints;
            KnownSkillCount = _character.KnownSkillCount;
            CorporationName = _character.CorporationName;
            AllianceName = _character.AllianceName;
            Balance = _character.Balance;
            ShipTypeName = _character.ShipTypeName;
            SecurityStatus = _character.SecurityStatus;
            Label = _character.Label;

            // Location
            var location = _character.LastKnownSolarSystem;
            LocationName = location?.Name ?? "Unknown";

            // Training status (CCP character only)
            if (_character is CCPCharacter ccpChar)
            {
                IsTraining = ccpChar.IsTraining;

                var currentSkill = ccpChar.CurrentlyTrainingSkill;
                if (currentSkill != null)
                {
                    TrainingSkillName = $"{currentSkill.SkillName} {GetRomanNumeral(currentSkill.Level)}";
                    UpdateTrainingTimes(ccpChar);
                }
                else
                {
                    TrainingSkillName = "Not Training";
                    TrainingTimeRemaining = string.Empty;
                    SkillQueueTimeRemaining = string.Empty;
                }

                SkillQueueCount = ccpChar.SkillQueue?.Count ?? 0;
            }
            else
            {
                IsTraining = false;
                TrainingSkillName = "N/A";
                TrainingTimeRemaining = string.Empty;
                SkillQueueTimeRemaining = string.Empty;
                SkillQueueCount = 0;
            }

            // Notify formatted and computed properties
            OnPropertiesChanged(
                nameof(SkillPointsFormatted),
                nameof(BalanceFormatted),
                nameof(SecurityStatusFormatted),
                nameof(HasAlliance),
                nameof(CurrentSkillProgress),
                nameof(SkillQueueEndTime));

            _skillQueue?.Refresh();
        }

        private void UpdateTrainingTimes(CCPCharacter ccpChar)
        {
            var currentSkill = ccpChar.CurrentlyTrainingSkill;
            if (currentSkill != null)
            {
                var remaining = currentSkill.EndTime - DateTime.UtcNow;
                if (remaining.TotalSeconds > 0)
                {
                    TrainingTimeRemaining = FormatTimeSpan(remaining);
                }
                else
                {
                    TrainingTimeRemaining = "Complete";
                }
            }
            else
            {
                TrainingTimeRemaining = string.Empty;
            }

            // Queue end time
            var queue = ccpChar.SkillQueue;
            if (queue != null && queue.Count > 0)
            {
                var queueEndTime = queue.EndTime;
                if (queueEndTime > DateTime.UtcNow)
                {
                    var queueRemaining = queueEndTime - DateTime.UtcNow;
                    SkillQueueTimeRemaining = FormatTimeSpan(queueRemaining);
                }
                else
                {
                    SkillQueueTimeRemaining = string.Empty;
                }
            }
            else
            {
                SkillQueueTimeRemaining = string.Empty;
            }

            // Notify progress change
            OnPropertyChanged(nameof(CurrentSkillProgress));
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

        private void OnCharacterUpdated(CharacterUpdatedEvent e)
        {
            if (e.Character?.Guid == _character.Guid)
            {
                Refresh();
            }
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
            // Update training times every second
            if (_character is CCPCharacter ccpChar && ccpChar.IsTraining)
            {
                UpdateTrainingTimes(ccpChar);
            }
        }

        /// <inheritdoc />
        protected override void OnDisposing()
        {
            _skillQueue?.Dispose();
            _skillsList?.Dispose();
            base.OnDisposing();
        }
    }
}
