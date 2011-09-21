using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    /// <summary>
    /// Category settings for notifications
    /// </summary>
    public sealed class NotificationCategorySettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationCategorySettings"/> class.
        /// </summary>
        public NotificationCategorySettings()
        {
            ToolTipBehaviour = ToolTipNotificationBehaviour.Once;
            ShowOnMainWindow = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationCategorySettings"/> class.
        /// </summary>
        /// <param name="toolTipBehaviour">The tool tip behaviour.</param>
        public NotificationCategorySettings(ToolTipNotificationBehaviour toolTipBehaviour)
        {
            ToolTipBehaviour = toolTipBehaviour;
            ShowOnMainWindow = true;
        }

        /// <summary>
        /// Gets or sets the tool tip behaviour.
        /// </summary>
        /// <value>The tool tip behaviour.</value>
        [XmlAttribute("toolTipBehaviour")]
        public ToolTipNotificationBehaviour ToolTipBehaviour { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show on main window].
        /// </summary>
        /// <value><c>true</c> if [show on main window]; otherwise, <c>false</c>.</value>
        [XmlAttribute("showOnMainWindow")]
        public bool ShowOnMainWindow { get; set; }
    }
}