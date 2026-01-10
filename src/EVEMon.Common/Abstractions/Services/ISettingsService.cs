namespace EVEMon.Common.Abstractions.Services
{
    /// <summary>
    /// Service interface for settings access.
    /// Abstracts access to the Settings static class.
    /// </summary>
    public interface ISettingsService
    {
        #region UI Settings

        /// <summary>
        /// Gets or sets whether the overview is shown in the main window.
        /// </summary>
        bool ShowOverview { get; set; }

        /// <summary>
        /// Gets or sets whether to group training skills first on overview.
        /// </summary>
        bool PutTrainingSkillsFirstOnOverview { get; set; }

        /// <summary>
        /// Gets or sets whether to show portraits on the overview.
        /// </summary>
        bool ShowOverviewPortrait { get; set; }

        /// <summary>
        /// Gets or sets whether safe for work mode is enabled.
        /// </summary>
        bool SafeForWork { get; set; }

        /// <summary>
        /// Gets or sets whether to highlight queued skills.
        /// </summary>
        bool HighlightQueuedSkills { get; set; }

        /// <summary>
        /// Gets or sets whether to highlight partial skills.
        /// </summary>
        bool HighlightPartialSkills { get; set; }

        #endregion

        #region Notifications

        /// <summary>
        /// Gets or sets whether to send notification on skill completion.
        /// </summary>
        bool NotifyOnSkillCompletion { get; set; }

        /// <summary>
        /// Gets or sets whether to send notification on skill queue empty.
        /// </summary>
        bool NotifyOnSkillQueueEmpty { get; set; }

        /// <summary>
        /// Gets or sets whether to use system tray.
        /// </summary>
        bool UseSystemTray { get; set; }

        /// <summary>
        /// Gets or sets whether to minimize to tray on close.
        /// </summary>
        bool MinimizeToTray { get; set; }

        #endregion

        #region Scheduler

        /// <summary>
        /// Gets or sets whether the scheduler is enabled.
        /// </summary>
        bool SchedulerEnabled { get; set; }

        #endregion

        #region Operations

        /// <summary>
        /// Saves the current settings.
        /// </summary>
        void Save();

        /// <summary>
        /// Reloads settings from storage.
        /// </summary>
        void Reload();

        /// <summary>
        /// Resets settings to defaults.
        /// </summary>
        void Reset();

        #endregion
    }
}
