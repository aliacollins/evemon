using System;

namespace EVEMon.Common.SettingsObjects
{
    /// <summary>
    /// Represents the behavior of the tool tip notifications (alerts for skills completion, etc)
    /// </summary>
    public enum ToolTipNotificationBehaviour
    {
        /// <summary>
        /// Never notify
        /// </summary>
        Never = 0,
        /// <summary>
        /// Notify once only 
        /// </summary>
        Once = 1,
        /// <summary>
        /// Every minutes, the warning is repeated until the user clicks the tool tip
        /// </summary>
        RepeatUntilClicked = 2
    }
}
