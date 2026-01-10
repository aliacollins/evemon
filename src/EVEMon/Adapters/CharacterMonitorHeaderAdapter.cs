using System;
using System.Drawing;
using System.Windows.Forms;
using EVEMon.Common.ViewModels.Character;

namespace EVEMon.Adapters
{
    /// <summary>
    /// Demonstrates how to adapt the CharacterMonitorHeader control to use ViewModels.
    /// This is a reference implementation showing the migration pattern.
    ///
    /// Migration Steps:
    /// 1. Create ViewModel bindings in SetupBindings()
    /// 2. Call SetupBindings() from the control's OnLoad or when character changes
    /// 3. Dispose bindings when control disposes or character changes
    /// 4. Gradually remove legacy event subscriptions as bindings take over
    /// </summary>
    /// <remarks>
    /// BEFORE (legacy pattern):
    /// <code>
    /// // In OnLoad:
    /// EveMonClient.CharacterUpdated += EveMonClient_CharacterUpdated;
    /// EveMonClient.SecondTick += EveMonClient_TimerTick;
    ///
    /// // In event handlers:
    /// private void EveMonClient_CharacterUpdated(object sender, CharacterChangedEventArgs e)
    /// {
    ///     if (e.Character != m_character) return;
    ///     CharacterNameLabel.Text = m_character.Name;
    ///     SkillPointsLabel.Text = $"{m_character.SkillPoints:N0} SP";
    /// }
    /// </code>
    ///
    /// AFTER (ViewModel pattern):
    /// <code>
    /// // In SetCharacter or OnLoad:
    /// _bindings?.Dispose();
    /// _viewModel = new CharacterViewModel(character);
    /// _bindings = new BindingCollection();
    /// _bindings.Add(CharacterNameLabel.BindTo(_viewModel, vm => vm.Name));
    /// _bindings.Add(SkillPointsLabel.BindTo(_viewModel, vm => vm.SkillPointsFormatted));
    ///
    /// // In OnDisposed:
    /// _bindings?.Dispose();
    /// _viewModel?.Dispose();
    /// </code>
    /// </remarks>
    public sealed class CharacterMonitorHeaderAdapter : IDisposable
    {
        private CharacterViewModel _viewModel;
        private BindingCollection _bindings;
        private bool _disposed;

        // Control references (passed in constructor)
        private readonly Label _characterNameLabel;
        private readonly Label _skillPointsLabel;
        private readonly Label _corporationLabel;
        private readonly Label _allianceLabel;
        private readonly Label _balanceLabel;
        private readonly Label _trainingLabel;
        private readonly Label _locationLabel;
        private readonly Control _trainingPanel;

        /// <summary>
        /// Creates a new adapter for a CharacterMonitorHeader-like control.
        /// </summary>
        /// <param name="characterNameLabel">Label for character name.</param>
        /// <param name="skillPointsLabel">Label for skill points.</param>
        /// <param name="corporationLabel">Label for corporation name.</param>
        /// <param name="allianceLabel">Label for alliance name (can be null).</param>
        /// <param name="balanceLabel">Label for wallet balance (can be null).</param>
        /// <param name="trainingLabel">Label for training status (can be null).</param>
        /// <param name="locationLabel">Label for location (can be null).</param>
        /// <param name="trainingPanel">Panel containing training info (can be null).</param>
        public CharacterMonitorHeaderAdapter(
            Label characterNameLabel,
            Label skillPointsLabel,
            Label corporationLabel,
            Label allianceLabel = null,
            Label balanceLabel = null,
            Label trainingLabel = null,
            Label locationLabel = null,
            Control trainingPanel = null)
        {
            _characterNameLabel = characterNameLabel ?? throw new ArgumentNullException(nameof(characterNameLabel));
            _skillPointsLabel = skillPointsLabel ?? throw new ArgumentNullException(nameof(skillPointsLabel));
            _corporationLabel = corporationLabel ?? throw new ArgumentNullException(nameof(corporationLabel));
            _allianceLabel = allianceLabel;
            _balanceLabel = balanceLabel;
            _trainingLabel = trainingLabel;
            _locationLabel = locationLabel;
            _trainingPanel = trainingPanel;
        }

        /// <summary>
        /// Gets the current ViewModel.
        /// </summary>
        public CharacterViewModel ViewModel => _viewModel;

        /// <summary>
        /// Sets the character and establishes bindings.
        /// </summary>
        /// <param name="character">The character to display.</param>
        public void SetCharacter(Common.Models.Character character)
        {
            // Clean up existing bindings
            _bindings?.Dispose();
            _viewModel?.Dispose();

            if (character == null)
            {
                _viewModel = null;
                _bindings = null;
                ClearControls();
                return;
            }

            // Create new ViewModel and bindings
            _viewModel = new CharacterViewModel(character);
            _bindings = new BindingCollection();

            SetupBindings();
        }

        private void SetupBindings()
        {
            // Core bindings - these are always set up
            _bindings.Add(_characterNameLabel.BindTo(_viewModel, vm => vm.Name));
            _bindings.Add(_skillPointsLabel.BindTo(_viewModel, vm => vm.SkillPointsFormatted));
            _bindings.Add(_corporationLabel.BindTo(_viewModel, vm => vm.CorporationName));

            // Optional bindings - only set up if controls exist
            if (_allianceLabel != null)
            {
                _bindings.Add(_allianceLabel.BindTo(_viewModel, vm => vm.AllianceName));
            }

            if (_balanceLabel != null)
            {
                _bindings.Add(_balanceLabel.BindTo(_viewModel, vm => vm.BalanceFormatted));
            }

            if (_trainingLabel != null)
            {
                // Bind training label with custom formatting
                _bindings.Add(ViewModelAdapter.OnPropertyChanged(_viewModel,
                    vm => vm.IsTraining,
                    isTraining => UpdateTrainingLabel(isTraining)));
            }

            if (_locationLabel != null)
            {
                _bindings.Add(_locationLabel.BindTo(_viewModel, vm => vm.LocationName));
            }

            if (_trainingPanel != null)
            {
                _bindings.Add(_trainingPanel.BindVisibleTo(_viewModel, vm => vm.IsTraining));
            }

            // Example of color binding based on training status
            if (_trainingLabel != null)
            {
                _bindings.Add(_trainingLabel.BindForeColorTo(_viewModel,
                    vm => vm.IsTraining,
                    isTraining => isTraining ? Color.Green : Color.Gray));
            }
        }

        private void UpdateTrainingLabel(bool isTraining)
        {
            if (_trainingLabel == null)
                return;

            if (isTraining)
            {
                _trainingLabel.Text = $"{_viewModel.TrainingSkillName} - {_viewModel.TrainingTimeRemaining}";
            }
            else
            {
                _trainingLabel.Text = "Not Training";
            }
        }

        private void ClearControls()
        {
            _characterNameLabel.Text = string.Empty;
            _skillPointsLabel.Text = string.Empty;
            _corporationLabel.Text = string.Empty;

            if (_allianceLabel != null)
                _allianceLabel.Text = string.Empty;

            if (_balanceLabel != null)
                _balanceLabel.Text = string.Empty;

            if (_trainingLabel != null)
                _trainingLabel.Text = string.Empty;

            if (_locationLabel != null)
                _locationLabel.Text = string.Empty;

            if (_trainingPanel != null)
                _trainingPanel.Visible = false;
        }

        /// <summary>
        /// Disposes the adapter and all bindings.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            _bindings?.Dispose();
            _viewModel?.Dispose();
            _bindings = null;
            _viewModel = null;
        }
    }
}
