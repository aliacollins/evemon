using System;
using System.Collections.ObjectModel;
using System.Linq;
using EVEMon.Common.Models;

namespace EVEMon.Common.ViewModels.Skills
{
    /// <summary>
    /// ViewModel for a skill group.
    /// </summary>
    public class SkillGroupViewModel : ViewModelBase
    {
        private readonly SkillGroup _skillGroup;
        private bool _isExpanded;
        private bool _isCharacterAlpha;

        /// <summary>
        /// Initializes a new instance of <see cref="SkillGroupViewModel"/>.
        /// </summary>
        /// <param name="skillGroup">The skill group model.</param>
        /// <param name="showAllSkills">If true, shows all skills. If false, only shows known skills.</param>
        /// <param name="isCharacterAlpha">True if the character is an Alpha clone.</param>
        public SkillGroupViewModel(SkillGroup skillGroup, bool showAllSkills = false, bool isCharacterAlpha = false)
        {
            _skillGroup = skillGroup ?? throw new ArgumentNullException(nameof(skillGroup));
            Skills = new ObservableCollection<SkillViewModel>();
            ShowAllSkills = showAllSkills;
            _isCharacterAlpha = isCharacterAlpha;

            Refresh();
        }

        /// <summary>
        /// Gets the group ID.
        /// </summary>
        public int ID => _skillGroup.ID;

        /// <summary>
        /// Gets the group name.
        /// </summary>
        public string Name => _skillGroup.Name;

        /// <summary>
        /// Gets the collection of skills in this group.
        /// </summary>
        public ObservableCollection<SkillViewModel> Skills { get; }

        /// <summary>
        /// Gets the number of known skills in this group.
        /// </summary>
        public int KnownSkillCount => _skillGroup.Count(s => s.IsKnown);

        /// <summary>
        /// Gets the total number of skills in this group.
        /// </summary>
        public int TotalSkillCount => _skillGroup.Count;

        /// <summary>
        /// Gets the skill count summary (e.g., "5 / 10").
        /// </summary>
        public string SkillCountSummary => $"{KnownSkillCount} / {TotalSkillCount}";

        /// <summary>
        /// Gets the total skill points in this group.
        /// </summary>
        public long TotalSkillPoints => _skillGroup.TotalSP;

        /// <summary>
        /// Gets formatted total skill points.
        /// </summary>
        public string TotalSkillPointsFormatted => _skillGroup.TotalSP.ToString("N0");

        /// <summary>
        /// Gets whether to show all skills or only known skills.
        /// </summary>
        public bool ShowAllSkills { get; set; }

        /// <summary>
        /// Gets or sets whether the group is expanded in the UI.
        /// </summary>
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        /// <summary>
        /// Gets whether this group has any known skills.
        /// </summary>
        public bool HasKnownSkills => KnownSkillCount > 0;

        /// <summary>
        /// Gets or sets whether the character is an Alpha clone.
        /// </summary>
        public bool IsCharacterAlpha
        {
            get => _isCharacterAlpha;
            set
            {
                if (SetProperty(ref _isCharacterAlpha, value))
                {
                    // Update all skill ViewModels when Alpha status changes
                    foreach (var skill in Skills)
                    {
                        skill.IsCharacterAlpha = value;
                    }
                }
            }
        }

        /// <summary>
        /// Refreshes all property values from the model.
        /// </summary>
        public void Refresh()
        {
            // Rebuild skills collection
            foreach (var vm in Skills)
            {
                vm.Dispose();
            }
            Skills.Clear();

            var skillsToShow = ShowAllSkills
                ? _skillGroup.AsEnumerable()
                : _skillGroup.Where(s => s.IsKnown);

            foreach (var skill in skillsToShow.OrderBy(s => s.Name))
            {
                Skills.Add(new SkillViewModel(skill, _isCharacterAlpha));
            }

            OnPropertiesChanged(
                nameof(KnownSkillCount),
                nameof(TotalSkillCount),
                nameof(SkillCountSummary),
                nameof(TotalSkillPoints),
                nameof(TotalSkillPointsFormatted),
                nameof(HasKnownSkills)
            );
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
