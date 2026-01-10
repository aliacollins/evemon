using System;
using EVEMon.Common.Enumerations.UISettings;

namespace EVEMon.Common.ViewModels.Settings
{
    /// <summary>
    /// ViewModel for the application settings.
    /// </summary>
    public class SettingsViewModel : ViewModelBase
    {
        private bool _safeForWork;
        private CloseBehaviour _closeBehaviour;
        private bool _runAtStartup;
        private int _skillQueueWarningThresholdDays;
        private bool _showTrayIcon;
        private bool _minimizeToTray;
        private bool _playNotificationSound;
        private bool _sendDesktopAlert;
        private bool _useProxyServer;
        private string _proxyHost;
        private int _proxyPort;
        private bool _proxyAuthentication;
        private string _proxyUsername;

        /// <summary>
        /// Initializes a new instance of <see cref="SettingsViewModel"/>.
        /// </summary>
        public SettingsViewModel()
        {
            LoadFromSettings();
        }

        #region UI Settings

        /// <summary>
        /// Gets or sets whether Safe for Work mode is enabled.
        /// Removes images and colors for a more professional appearance.
        /// </summary>
        public bool SafeForWork
        {
            get => _safeForWork;
            set => SetProperty(ref _safeForWork, value);
        }

        /// <summary>
        /// Gets or sets the main window close behavior.
        /// </summary>
        public CloseBehaviour CloseBehaviour
        {
            get => _closeBehaviour;
            set => SetProperty(ref _closeBehaviour, value);
        }

        /// <summary>
        /// Gets or sets whether EVEMon runs at system startup.
        /// </summary>
        public bool RunAtStartup
        {
            get => _runAtStartup;
            set => SetProperty(ref _runAtStartup, value);
        }

        /// <summary>
        /// Gets or sets the skill queue warning threshold in days.
        /// </summary>
        public int SkillQueueWarningThresholdDays
        {
            get => _skillQueueWarningThresholdDays;
            set => SetProperty(ref _skillQueueWarningThresholdDays, Math.Max(1, Math.Min(365, value)));
        }

        #endregion

        #region Tray Settings

        /// <summary>
        /// Gets or sets whether to show the system tray icon.
        /// </summary>
        public bool ShowTrayIcon
        {
            get => _showTrayIcon;
            set => SetProperty(ref _showTrayIcon, value);
        }

        /// <summary>
        /// Gets or sets whether to minimize to system tray.
        /// </summary>
        public bool MinimizeToTray
        {
            get => _minimizeToTray;
            set => SetProperty(ref _minimizeToTray, value);
        }

        #endregion

        #region Notification Settings

        /// <summary>
        /// Gets or sets whether to play notification sounds.
        /// </summary>
        public bool PlayNotificationSound
        {
            get => _playNotificationSound;
            set => SetProperty(ref _playNotificationSound, value);
        }

        /// <summary>
        /// Gets or sets whether to send desktop alerts.
        /// </summary>
        public bool SendDesktopAlert
        {
            get => _sendDesktopAlert;
            set => SetProperty(ref _sendDesktopAlert, value);
        }

        #endregion

        #region Proxy Settings

        /// <summary>
        /// Gets or sets whether to use a proxy server.
        /// </summary>
        public bool UseProxyServer
        {
            get => _useProxyServer;
            set => SetProperty(ref _useProxyServer, value);
        }

        /// <summary>
        /// Gets or sets the proxy host.
        /// </summary>
        public string ProxyHost
        {
            get => _proxyHost;
            set => SetProperty(ref _proxyHost, value);
        }

        /// <summary>
        /// Gets or sets the proxy port.
        /// </summary>
        public int ProxyPort
        {
            get => _proxyPort;
            set => SetProperty(ref _proxyPort, Math.Max(1, Math.Min(65535, value)));
        }

        /// <summary>
        /// Gets or sets whether proxy authentication is required.
        /// </summary>
        public bool ProxyAuthentication
        {
            get => _proxyAuthentication;
            set => SetProperty(ref _proxyAuthentication, value);
        }

        /// <summary>
        /// Gets or sets the proxy username.
        /// </summary>
        public string ProxyUsername
        {
            get => _proxyUsername;
            set => SetProperty(ref _proxyUsername, value);
        }

        #endregion

        /// <summary>
        /// Loads settings from the application settings.
        /// </summary>
        public void LoadFromSettings()
        {
            // UI Settings
            SafeForWork = Common.Settings.UI.SafeForWork;
            CloseBehaviour = Common.Settings.UI.MainWindowCloseBehaviour;
            SkillQueueWarningThresholdDays = Common.Settings.UI.MainWindow.SkillQueueWarningThresholdDays;

            // Tray settings
            ShowTrayIcon = Common.Settings.UI.SystemTrayIcon != SystemTrayBehaviour.Disabled;
            MinimizeToTray = Common.Settings.UI.SystemTrayIcon == SystemTrayBehaviour.AlwaysVisible;

            // Notification settings
            PlayNotificationSound = Common.Settings.Notifications.PlaySoundOnSkillCompletion;
            SendDesktopAlert = Common.Settings.Notifications.SendMailAlert;

            // Proxy settings
            UseProxyServer = Common.Settings.Proxy.Enabled;
            ProxyHost = Common.Settings.Proxy.Host ?? string.Empty;
            ProxyPort = Common.Settings.Proxy.Port;
            ProxyAuthentication = Common.Settings.Proxy.Authentication == SettingsObjects.ProxyAuthentication.Specified;
            ProxyUsername = Common.Settings.Proxy.Username ?? string.Empty;

            // Startup settings (platform-specific - may not be available everywhere)
            RunAtStartup = false; // Will be set by platform-specific code
        }

        /// <summary>
        /// Saves the current settings to the application settings.
        /// </summary>
        public void SaveToSettings()
        {
            // UI Settings
            Common.Settings.UI.SafeForWork = SafeForWork;
            Common.Settings.UI.MainWindowCloseBehaviour = CloseBehaviour;
            Common.Settings.UI.MainWindow.SkillQueueWarningThresholdDays = SkillQueueWarningThresholdDays;

            // Tray settings
            Common.Settings.UI.SystemTrayIcon = ShowTrayIcon
                ? (MinimizeToTray ? SystemTrayBehaviour.AlwaysVisible : SystemTrayBehaviour.ShowWhenMinimized)
                : SystemTrayBehaviour.Disabled;

            // Notification settings
            Common.Settings.Notifications.PlaySoundOnSkillCompletion = PlayNotificationSound;
            Common.Settings.Notifications.SendMailAlert = SendDesktopAlert;

            // Proxy settings
            Common.Settings.Proxy.Enabled = UseProxyServer;
            Common.Settings.Proxy.Host = ProxyHost;
            Common.Settings.Proxy.Port = ProxyPort;
            Common.Settings.Proxy.Authentication = ProxyAuthentication
                ? SettingsObjects.ProxyAuthentication.Specified
                : SettingsObjects.ProxyAuthentication.None;
            Common.Settings.Proxy.Username = ProxyUsername;

            // Trigger save
            Common.Settings.Save();
        }

        /// <summary>
        /// Resets all settings to defaults.
        /// </summary>
        public void ResetToDefaults()
        {
            SafeForWork = false;
            CloseBehaviour = CloseBehaviour.Exit;
            SkillQueueWarningThresholdDays = 1;
            ShowTrayIcon = true;
            MinimizeToTray = false;
            PlayNotificationSound = true;
            SendDesktopAlert = false;
            UseProxyServer = false;
            ProxyHost = string.Empty;
            ProxyPort = 8080;
            ProxyAuthentication = false;
            ProxyUsername = string.Empty;
            RunAtStartup = false;
        }
    }
}
