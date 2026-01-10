using System;
using System.Collections.ObjectModel;
using System.Linq;
using EVEMon.Common.Abstractions;
using EVEMon.Common.Abstractions.Events;
using EVEMon.Common.Abstractions.Services;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Models;
using EVEMon.Common.ViewModels.Character;

namespace EVEMon.Common.ViewModels
{
    /// <summary>
    /// ViewModel for the main application window.
    /// Manages the character list, server status, and global application state.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ICharacterService _characterService;
        private CharacterViewModel _selectedCharacter;
        private string _serverStatus;
        private string _serverStatusText;
        private string _currentTime;
        private bool _serverOnline;
        private string _windowTitle;

        /// <summary>
        /// Initializes a new instance of <see cref="MainWindowViewModel"/>.
        /// </summary>
        public MainWindowViewModel()
        {
            _characterService = ServiceLocator.TryGetService<ICharacterService>();

            Characters = new ObservableCollection<CharacterViewModel>();
            _windowTitle = "EVEMon";
            _serverStatus = "Unknown";
            _serverStatusText = "Server: Unknown";
            _currentTime = DateTime.UtcNow.ToString("HH:mm:ss") + " EVE Time";

            // Load initial characters
            RefreshCharacters();

            // Subscribe to events
            Subscribe<CharacterCollectionChangedEvent>(OnCharacterCollectionChanged);
            Subscribe<MonitoredCharacterCollectionChangedEvent>(OnMonitoredCharacterCollectionChanged);
            Subscribe<CharacterUpdatedEvent>(OnCharacterUpdated);
            Subscribe<ServerStatusUpdatedEvent>(OnServerStatusUpdated);
            Subscribe<SettingsChangedEvent>(OnSettingsChanged);
            Subscribe<SecondTickEvent>(OnSecondTick);
        }

        /// <summary>
        /// Gets the collection of monitored characters.
        /// </summary>
        public ObservableCollection<CharacterViewModel> Characters { get; }

        /// <summary>
        /// Gets or sets the currently selected character.
        /// </summary>
        public CharacterViewModel SelectedCharacter
        {
            get => _selectedCharacter;
            set
            {
                if (SetProperty(ref _selectedCharacter, value))
                {
                    OnSelectedCharacterChanged();
                }
            }
        }

        /// <summary>
        /// Gets the window title.
        /// </summary>
        public string WindowTitle
        {
            get => _windowTitle;
            private set => SetProperty(ref _windowTitle, value);
        }

        /// <summary>
        /// Gets the server status display text.
        /// </summary>
        public string ServerStatus
        {
            get => _serverStatus;
            private set => SetProperty(ref _serverStatus, value);
        }

        /// <summary>
        /// Gets whether the server is online.
        /// </summary>
        public bool ServerOnline
        {
            get => _serverOnline;
            private set => SetProperty(ref _serverOnline, value);
        }

        /// <summary>
        /// Gets the server status text for display.
        /// </summary>
        public string ServerStatusText
        {
            get => _serverStatusText;
            private set => SetProperty(ref _serverStatusText, value);
        }

        /// <summary>
        /// Gets the current EVE time display string.
        /// </summary>
        public string CurrentTime
        {
            get => _currentTime;
            private set => SetProperty(ref _currentTime, value);
        }

        /// <summary>
        /// Gets whether there are any monitored characters.
        /// </summary>
        public bool HasCharacters => Characters.Count > 0;

        /// <summary>
        /// Refreshes the character list from the service.
        /// </summary>
        public void RefreshCharacters()
        {
            if (_characterService == null)
                return;

            var monitoredCharacters = _characterService.MonitoredCharacters;

            // Remove characters that are no longer monitored
            var toRemove = Characters.Where(vm =>
                !monitoredCharacters.Any(c => c.Guid == vm.CharacterGuid)).ToList();

            foreach (var vm in toRemove)
            {
                Characters.Remove(vm);
                vm.Dispose();
            }

            // Add new characters
            foreach (var character in monitoredCharacters)
            {
                if (!Characters.Any(vm => vm.CharacterGuid == character.Guid))
                {
                    var vm = new CharacterViewModel(character);
                    Characters.Add(vm);
                }
            }

            // Select first character if none selected
            if (SelectedCharacter == null && Characters.Count > 0)
            {
                SelectedCharacter = Characters[0];
            }

            OnPropertyChanged(nameof(HasCharacters));
        }

        /// <summary>
        /// Selects a character by their GUID.
        /// </summary>
        /// <param name="characterGuid">The character's GUID.</param>
        public void SelectCharacter(Guid characterGuid)
        {
            var character = Characters.FirstOrDefault(c => c.CharacterGuid == characterGuid);
            if (character != null)
            {
                SelectedCharacter = character;
            }
        }

        private void OnCharacterCollectionChanged(CharacterCollectionChangedEvent e)
        {
            RefreshCharacters();
        }

        private void OnMonitoredCharacterCollectionChanged(MonitoredCharacterCollectionChangedEvent e)
        {
            RefreshCharacters();
        }

        private void OnCharacterUpdated(CharacterUpdatedEvent e)
        {
            // Find and refresh the corresponding ViewModel
            var vm = Characters.FirstOrDefault(c => c.CharacterGuid == e.Character?.Guid);
            vm?.Refresh();

            UpdateWindowTitle();
        }

        private void OnServerStatusUpdated(ServerStatusUpdatedEvent e)
        {
            if (e.Server == null)
                return;

            ServerOnline = e.CurrentStatus == Enumerations.ServerStatus.Online;

            // Use the server's built-in StatusText which includes name and player count
            ServerStatus = e.Server.StatusText;
            ServerStatusText = e.Server.StatusText;
        }

        private void OnSecondTick(SecondTickEvent e)
        {
            CurrentTime = DateTime.UtcNow.ToString("HH:mm:ss") + " EVE Time";
        }

        private void OnSettingsChanged(SettingsChangedEvent e)
        {
            UpdateWindowTitle();
        }

        private void OnSelectedCharacterChanged()
        {
            UpdateWindowTitle();
        }

        private void UpdateWindowTitle()
        {
            if (SelectedCharacter != null)
            {
                WindowTitle = $"EVEMon - {SelectedCharacter.Name}";
            }
            else
            {
                WindowTitle = "EVEMon";
            }
        }

        /// <inheritdoc />
        protected override void OnDisposing()
        {
            foreach (var character in Characters)
            {
                character.Dispose();
            }
            Characters.Clear();

            base.OnDisposing();
        }
    }
}
