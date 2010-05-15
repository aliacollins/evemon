namespace EVEMon.Common.SettingsObjects
{
    /// <summary>
    /// Describes the target platform to allow EVEMon to apply different tweaks at runtime
    /// </summary>
    public enum CompatibilityMode
    {
        /// <summary>
        /// Windows and Linux + Wine
        /// </summary>
        Default = 0,
        /// <summary>
        /// Wine
        /// </summary>
        Wine = 1
    }

    /// <summary>
    /// Describes the behaviour employed to remove Obsolete Entries from plans.
    /// </summary>
    public enum ObsoleteEntryRemovalBehaviour
    {
        /// <summary>
        /// Never remove entries from the plan, always ask the user.
        /// </summary>
        AlwaysAsk = 0,
        /// <summary>
        /// Only remove confirmed completed (by API) entries from the plan, ask about unconfirmed entries.
        /// </summary>
        RemoveConfirmed = 1,
        /// <summary>
        /// Always remove all entries automatically.
        /// </summary>
        RemoveAll = 2
    }

    /// <summary>
    /// Represents the behaviour for the system tray icon
    /// </summary>
    public enum SystemTrayBehaviour
    {
        /// <summary>
        /// The tray icon is always hidden
        /// </summary>
        Disabled = 0,
        /// <summary>
        /// The tray icon is visible when the main window is minimized
        /// </summary>
        ShowWhenMinimized = 1,
        /// <summary>
        /// The tray icon is always visible
        /// </summary>
        AlwaysVisible = 2
    }

    /// <summary>
    /// Represents the behaviour when closing the main form
    /// </summary>
    public enum CloseBehaviour
    {
        /// <summary>
        /// Exit the application
        /// </summary>
        Exit = 0,
        /// <summary>
        /// Minimize to the system tray
        /// </summary>
        MinimizeToTray = 1,
        /// <summary>
        /// Minimize to the task bar
        /// </summary>
        MinimizeToTaskbar = 2
    }
}
