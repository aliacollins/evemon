using System.Collections.ObjectModel;
using System.Xml.Serialization;
using EVEMon.Common.Enumerations.UISettings;

namespace EVEMon.Common.SettingsObjects
{
    /// <summary>
    /// Root UI Settings Class
    /// </summary>
    public sealed class UISettings
    {
        private readonly Collection<string> m_confirmedTips;

        /// <summary>
        /// Initializes a new instance of the <see cref="UISettings"/> class.
        /// </summary>
        public UISettings()
        {
            MainWindowCloseBehaviour = CloseBehaviour.Exit;

            WindowLocations = new ModifiedSerializableDictionary<string, WindowLocationSettings>();
            Splitters = new ModifiedSerializableDictionary<string, int>();
            m_confirmedTips = new Collection<string>();

            CertificateBrowser = new CertificateBrowserSettings();
            BlueprintBrowser = new BlueprintBrowserSettings();
            SystemTrayTooltip = new TrayTooltipSettings();
            SkillPieChart = new SkillPieChartSettings();
            SystemTrayPopup = new TrayPopupSettings();
            SkillBrowser = new SkillBrowserSettings();
            ShipBrowser = new ShipBrowserSettings();
            ItemBrowser = new ItemBrowserSettings();
            MainWindow = new MainWindowSettings();
            PlanWindow = new PlanWindowSettings();
            Scheduler = new SchedulerUISettings();

            UseStoredSearchFilters = true;
        }

        /// <summary>
        /// When true, removes images and colours to make EVEMon looks like some boring business application.
        /// </summary>
        [XmlElement("safeForWork")]
        public bool SafeForWork { get; set; }

        /// <summary>
        /// Gets or sets the main window close behaviour.
        /// </summary>
        /// <value>The main window close behaviour.</value>
        [XmlElement("closeBehaviour")]
        public CloseBehaviour MainWindowCloseBehaviour { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use stored search filters].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [use stored search filters]; otherwise, <c>false</c>.
        /// </value>
        [XmlElement("useStoredSearchFilters")]
        public bool UseStoredSearchFilters { get; set; }

        /// <summary>
        /// Gets or sets the main window.
        /// </summary>
        /// <value>The main window.</value>
        [XmlElement("mainWindow")]
        public MainWindowSettings MainWindow { get; set; }

        /// <summary>
        /// Gets or sets the plan window.
        /// </summary>
        /// <value>The plan window.</value>
        [XmlElement("planWindow")]
        public PlanWindowSettings PlanWindow { get; set; }

        /// <summary>
        /// Gets or sets the certificate browser.
        /// </summary>
        /// <value>The certificate browser.</value>
        [XmlElement("certificateBrowser")]
        public CertificateBrowserSettings CertificateBrowser { get; set; }

        /// <summary>
        /// Gets or sets the ship browser.
        /// </summary>
        /// <value>The ship browser.</value>
        [XmlElement("shipBrowser")]
        public ShipBrowserSettings ShipBrowser { get; set; }

        /// <summary>
        /// Gets or sets the skill browser.
        /// </summary>
        /// <value>The skill browser.</value>
        [XmlElement("skillBrowser")]
        public SkillBrowserSettings SkillBrowser { get; set; }

        /// <summary>
        /// Gets or sets the item browser.
        /// </summary>
        /// <value>The item browser.</value>
        [XmlElement("itemBrowser")]
        public ItemBrowserSettings ItemBrowser { get; set; }

        /// <summary>
        /// Gets or sets the blueprint browser.
        /// </summary>
        /// <value>The blueprint browser.</value>
        [XmlElement("blueprintBrowser")]
        public BlueprintBrowserSettings BlueprintBrowser { get; set; }

        /// <summary>
        /// Gets or sets the system tray icon.
        /// </summary>
        /// <value>The system tray icon.</value>
        [XmlElement("systemTrayIcon")]
        public SystemTrayBehaviour SystemTrayIcon { get; set; }

        /// <summary>
        /// Gets or sets the system tray popup.
        /// </summary>
        /// <value>The system tray popup.</value>
        [XmlElement("systemTrayPopup")]
        public TrayPopupSettings SystemTrayPopup { get; set; }

        /// <summary>
        /// Gets or sets the scheduler.
        /// </summary>
        /// <value>The scheduler.</value>
        [XmlElement("calendar")]
        public SchedulerUISettings Scheduler { get; set; }

        /// <summary>
        /// Gets or sets the system tray tooltip.
        /// </summary>
        /// <value>The system tray tooltip.</value>
        [XmlElement("systemTrayTooltip")]
        public TrayTooltipSettings SystemTrayTooltip { get; set; }

        /// <summary>
        /// Gets or sets the skill pie chart.
        /// </summary>
        /// <value>The skill pie chart.</value>
        [XmlElement("skillPieChart")]
        public SkillPieChartSettings SkillPieChart { get; set; }

        /// <summary>
        /// Gets or sets the window locations.
        /// </summary>
        /// <value>The window locations.</value>
        [XmlElement("locations")]
        public ModifiedSerializableDictionary<string, WindowLocationSettings> WindowLocations { get; set; }

        /// <summary>
        /// Gets or sets the splitters.
        /// </summary>
        /// <value>The splitters.</value>
        [XmlElement("splitters")]
        public ModifiedSerializableDictionary<string, int> Splitters { get; set; }

        /// <summary>
        /// Gets or sets the confirmed tips.
        /// </summary>
        /// <value>The confirmed tips.</value>
        [XmlArray("confirmedTips")]
        [XmlArrayItem("tip")]
        public Collection<string> ConfirmedTips => m_confirmedTips;
    }
}