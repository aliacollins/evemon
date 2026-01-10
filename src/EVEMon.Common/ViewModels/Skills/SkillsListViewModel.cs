using System;
using System.Collections.ObjectModel;
using System.Linq;
using EVEMon.Common.Abstractions.Events;
using EVEMon.Common.Models;

namespace EVEMon.Common.ViewModels.Skills
{
    /// <summary>
    /// ViewModel for the complete skills list of a character.
    /// </summary>
    public class SkillsListViewModel : ViewModelBase
    {
        private readonly Models.Character _character;
        private bool _showAllSkills;
        private bool _showOnlyGroupsWithKnownSkills;
        private string _filterText;
        private bool _isCharacterAlpha;

        /// <summary>
        /// Initializes a new instance of <see cref="SkillsListViewModel"/>.
        /// </summary>
        /// <param name="character">The character.</param>
        public SkillsListViewModel(Models.Character character)
        {
            _character = character ?? throw new ArgumentNullException(nameof(character));
            SkillGroups = new ObservableCollection<SkillGroupViewModel>();
            _showOnlyGroupsWithKnownSkills = true;

            // Determine if character is Alpha clone
            _isCharacterAlpha = _character.EffectiveCharacterStatus == AccountStatus.Alpha;

            Refresh();

            // Subscribe to events
            Subscribe<CharacterUpdatedEvent>(OnCharacterUpdated);
            Subscribe<CharacterSkillQueueUpdatedEvent>(OnSkillQueueUpdated);
        }

        /// <summary>
        /// Gets the collection of skill groups.
        /// </summary>
        public ObservableCollection<SkillGroupViewModel> SkillGroups { get; }

        /// <summary>
        /// Gets the total number of known skills.
        /// </summary>
        public int TotalKnownSkills => _character.KnownSkillCount;

        /// <summary>
        /// Gets the total skill points.
        /// </summary>
        public long TotalSkillPoints => _character.SkillPoints;

        /// <summary>
        /// Gets formatted total skill points.
        /// </summary>
        public string TotalSkillPointsFormatted => _character.SkillPoints.ToString("N0") + " SP";

        /// <summary>
        /// Gets the total number of skill groups with known skills.
        /// </summary>
        public int GroupsWithKnownSkillsCount => _character.SkillGroups.Count(g => g.Any(s => s.IsKnown));

        /// <summary>
        /// Gets or sets whether to show all skills (including unknown).
        /// </summary>
        public bool ShowAllSkills
        {
            get => _showAllSkills;
            set
            {
                if (SetProperty(ref _showAllSkills, value))
                {
                    Refresh();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to show only groups that have known skills.
        /// </summary>
        public bool ShowOnlyGroupsWithKnownSkills
        {
            get => _showOnlyGroupsWithKnownSkills;
            set
            {
                if (SetProperty(ref _showOnlyGroupsWithKnownSkills, value))
                {
                    Refresh();
                }
            }
        }

        /// <summary>
        /// Gets or sets the filter text for searching skills.
        /// </summary>
        public string FilterText
        {
            get => _filterText;
            set
            {
                if (SetProperty(ref _filterText, value))
                {
                    Refresh();
                }
            }
        }

        /// <summary>
        /// Gets the summary text.
        /// </summary>
        public string SummaryText => $"{TotalKnownSkills} skills known across {GroupsWithKnownSkillsCount} groups";

        /// <summary>
        /// Gets whether the character is an Alpha clone.
        /// </summary>
        public bool IsCharacterAlpha
        {
            get => _isCharacterAlpha;
            private set
            {
                if (SetProperty(ref _isCharacterAlpha, value))
                {
                    // Update all skill groups when Alpha status changes
                    foreach (var group in SkillGroups)
                    {
                        group.IsCharacterAlpha = value;
                    }
                }
            }
        }

        /// <summary>
        /// Refreshes all property values from the model.
        /// </summary>
        public void Refresh()
        {
            // Dispose existing ViewModels
            foreach (var vm in SkillGroups)
            {
                vm.Dispose();
            }
            SkillGroups.Clear();

            var groups = _character.SkillGroups.AsEnumerable();

            // Filter to only groups with known skills if requested
            if (ShowOnlyGroupsWithKnownSkills && !ShowAllSkills)
            {
                groups = groups.Where(g => g.Any(s => s.IsKnown));
            }

            // Apply text filter if provided
            if (!string.IsNullOrWhiteSpace(FilterText))
            {
                var filter = FilterText.Trim();
                groups = groups.Where(g =>
                    g.Name.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                    g.Any(s => s.Name.Contains(filter, StringComparison.OrdinalIgnoreCase)));
            }

            // Create ViewModels for each group
            foreach (var group in groups.OrderBy(g => g.Name))
            {
                var groupVm = new SkillGroupViewModel(group, ShowAllSkills, _isCharacterAlpha);

                // If filtering by text, check if any skills in the group match
                if (!string.IsNullOrWhiteSpace(FilterText))
                {
                    var filter = FilterText.Trim();
                    if (!group.Name.Contains(filter, StringComparison.OrdinalIgnoreCase))
                    {
                        // Filter skills within the group
                        var matchingSkills = groupVm.Skills.Where(s =>
                            s.Name.Contains(filter, StringComparison.OrdinalIgnoreCase)).ToList();

                        if (matchingSkills.Count == 0)
                            continue;
                    }
                }

                SkillGroups.Add(groupVm);
            }

            OnPropertiesChanged(
                nameof(TotalKnownSkills),
                nameof(TotalSkillPoints),
                nameof(TotalSkillPointsFormatted),
                nameof(GroupsWithKnownSkillsCount),
                nameof(SummaryText)
            );
        }

        /// <summary>
        /// Expands all groups.
        /// </summary>
        public void ExpandAll()
        {
            foreach (var group in SkillGroups)
            {
                group.IsExpanded = true;
            }
        }

        /// <summary>
        /// Collapses all groups.
        /// </summary>
        public void CollapseAll()
        {
            foreach (var group in SkillGroups)
            {
                group.IsExpanded = false;
            }
        }

        private void OnCharacterUpdated(CharacterUpdatedEvent e)
        {
            if (e.Character?.Guid == _character.Guid)
            {
                // Update Alpha status if it changed
                IsCharacterAlpha = _character.EffectiveCharacterStatus == AccountStatus.Alpha;
                Refresh();
            }
        }

        private void OnSkillQueueUpdated(CharacterSkillQueueUpdatedEvent e)
        {
            if (e.Character?.Guid == _character.Guid)
            {
                // Update training status for skills
                foreach (var group in SkillGroups)
                {
                    foreach (var skill in group.Skills)
                    {
                        skill.Refresh();
                    }
                }
            }
        }

        /// <inheritdoc />
        protected override void OnDisposing()
        {
            foreach (var group in SkillGroups)
            {
                group.Dispose();
            }
            SkillGroups.Clear();

            base.OnDisposing();
        }
    }
}
