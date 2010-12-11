using EVEMon.Common.Attributes;

namespace EVEMon.Common.SettingsObjects
{
    #region UISettings

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

    #endregion


    #region MarketOrderSettings

    /// <summary>
    /// Enumeration for the market orders to be group by.
    /// </summary>
    /// <remarks>The integer value determines the sort order.</remarks>
    public enum MarketOrderGrouping
    {
        [Header("Group by order status")]
        State = 0,
        [Header("Group by order status (Desc)")]
        StateDesc = 1,
        [Header("Group by buying/selling")]
        OrderType = 2,
        [Header("Group by buying/selling (Desc)")]
        OrderTypeDesc = 3,
        [Header("Group by issue day")]
        Issued = 4,
        [Header("Group by issue day (Desc)")]
        IssuedDesc = 5,
        [Header("Group by item type")]
        ItemType = 6,
        [Header("Group by item type (Desc)")]
        ItemTypeDesc = 7,
        [Header("Group by station")]
        Location = 8,
        [Header("Group by station (Desc)")]
        LocationDesc = 9
    }
    
    #endregion


    #region IndustryJobSettings

    /// <summary>
    /// Enumeration for the industry jobs to be group by.
    /// </summary>
    /// <remarks>The integer value determines the sort order.</remarks>
    public enum IndustryJobGrouping
    {
        [Header("Group by job state")]
        State = 0,
        [Header("Group by job state (Desc)")]
        StateDesc = 1,
        [Header("Group by ending date")]
        EndDate = 2,
        [Header("Group by ending date (Desc)")]
        EndDateDesc = 3,
        [Header("Group by installed blueprint type")]
        InstalledItemType = 4,
        [Header("Group by installed blueprint type (Desc)")]
        InstalledItemTypeDesc = 5,
        [Header("Group by output item type")]
        OutputItemType = 6,
        [Header("Group by output item type (Desc)")]
        OutputItemTypeDesc = 7,
        [Header("Group by job activity")]
        Activity = 8,
        [Header("Group by job activity (Desc)")]
        ActivityDesc = 9,
        [Header("Group by installed location")]
        Location = 10,
        [Header("Group by installed location (Desc)")]
        LocationDesc = 11
    }
    
    #endregion

    #region Full API Key Features

    public enum FullAPIKeyFeatures
    {
        [Header ("Market")]
        MarketOrders,
        [Header("Industry")]
        IndustryJobs,
        [Header("Research")]
        ResearchPoints,   
    }

    #endregion

}
