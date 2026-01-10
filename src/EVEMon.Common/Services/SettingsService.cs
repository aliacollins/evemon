using System.Linq;
using EVEMon.Common.Abstractions.Services;

namespace EVEMon.Common.Services
{
    /// <summary>
    /// Implementation of <see cref="ISettingsService"/> that wraps the static Settings class.
    /// </summary>
    public sealed class SettingsService : ISettingsService
    {
        #region UI Settings

        /// <inheritdoc />
        public bool ShowOverview
        {
            get => Settings.UI.MainWindow.ShowOverview;
            set => Settings.UI.MainWindow.ShowOverview = value;
        }

        /// <inheritdoc />
        public bool PutTrainingSkillsFirstOnOverview
        {
            get => Settings.UI.MainWindow.PutTrainingSkillsFirstOnOverview;
            set => Settings.UI.MainWindow.PutTrainingSkillsFirstOnOverview = value;
        }

        /// <inheritdoc />
        public bool ShowOverviewPortrait
        {
            get => Settings.UI.MainWindow.ShowOverviewPortrait;
            set => Settings.UI.MainWindow.ShowOverviewPortrait = value;
        }

        /// <inheritdoc />
        public bool SafeForWork
        {
            get => Settings.UI.SafeForWork;
            set => Settings.UI.SafeForWork = value;
        }

        /// <inheritdoc />
        public bool HighlightQueuedSkills
        {
            get => Settings.UI.PlanWindow.HighlightQueuedSkills;
            set => Settings.UI.PlanWindow.HighlightQueuedSkills = value;
        }

        /// <inheritdoc />
        public bool HighlightPartialSkills
        {
            get => Settings.UI.PlanWindow.HighlightPartialSkills;
            set => Settings.UI.PlanWindow.HighlightPartialSkills = value;
        }

        #endregion

        #region Notifications

        /// <inheritdoc />
        public bool NotifyOnSkillCompletion
        {
            get => Settings.Notifications.PlaySoundOnSkillCompletion;
            set => Settings.Notifications.PlaySoundOnSkillCompletion = value;
        }

        /// <inheritdoc />
        public bool NotifyOnSkillQueueEmpty
        {
            get => Settings.Notifications.SendMailAlert;
            set => Settings.Notifications.SendMailAlert = value;
        }

        /// <inheritdoc />
        public bool UseSystemTray
        {
            // Check if system tray is not disabled
            get => Settings.UI.SystemTrayIcon != Enumerations.UISettings.SystemTrayBehaviour.Disabled;
            set
            {
                if (value)
                    Settings.UI.SystemTrayIcon = Enumerations.UISettings.SystemTrayBehaviour.AlwaysVisible;
                else
                    Settings.UI.SystemTrayIcon = Enumerations.UISettings.SystemTrayBehaviour.Disabled;
            }
        }

        /// <inheritdoc />
        public bool MinimizeToTray
        {
            get => Settings.UI.MainWindowCloseBehaviour == Enumerations.UISettings.CloseBehaviour.MinimizeToTaskbar;
            set
            {
                if (value)
                    Settings.UI.MainWindowCloseBehaviour = Enumerations.UISettings.CloseBehaviour.MinimizeToTaskbar;
                else
                    Settings.UI.MainWindowCloseBehaviour = Enumerations.UISettings.CloseBehaviour.Exit;
            }
        }

        #endregion

        #region Scheduler

        /// <inheritdoc />
        public bool SchedulerEnabled
        {
            // The scheduler settings are stored per-character and don't have a global enable
            // This is a placeholder that returns false by default
            get => false;
            set
            {
                // No simple way to enable/disable scheduler globally
                // This is a placeholder for future implementation
            }
        }

        #endregion

        #region Operations

        /// <inheritdoc />
        public void Save()
        {
            Settings.Save();
        }

        /// <inheritdoc />
        public void Reload()
        {
            // Settings are loaded via Settings.Initialize()
            // For now, this is a no-op as runtime reload isn't typically needed
        }

        /// <inheritdoc />
        public void Reset()
        {
            // Settings doesn't have a public Reset method
            // Would need to implement via Settings.Import with default settings
        }

        #endregion
    }
}
