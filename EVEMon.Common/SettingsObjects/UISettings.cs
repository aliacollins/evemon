using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    /// <summary>
    /// Root UI Settings Class
    /// </summary>
    public sealed class UISettings
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public UISettings()
        {
            MainWindowCloseBehaviour = CloseBehaviour.Exit;

            WindowLocations = new SerializableDictionary<string, SerializableRectangle>();
            Splitters = new SerializableDictionary<string, int>();

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
            ConfirmedTips = new List<String>();

            UseStoredSearchFilters = true;
            ShowTextInToolStrip = true;
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// When true, removes images and colours to make EVEMon looks like some boring business application.
        /// </summary>
        [XmlElement("safeForWork")]
        public bool SafeForWork { get; set; }

        [XmlElement("showTextInToolStrip")]
        public bool ShowTextInToolStrip { get; set; }

        [XmlElement("closeBehaviour")]
        public CloseBehaviour MainWindowCloseBehaviour { get; set; }

        [XmlElement("useStoredSearchFilters")]
        public bool UseStoredSearchFilters { get; set; }

        [XmlElement("mainWindow")]
        public MainWindowSettings MainWindow { get; set; }

        [XmlElement("planWindow")]
        public PlanWindowSettings PlanWindow { get; set; }

        [XmlElement("certificateBrowser")]
        public CertificateBrowserSettings CertificateBrowser { get; set; }

        [XmlElement("shipBrowser")]
        public ShipBrowserSettings ShipBrowser { get; set; }

        [XmlElement("skillBrowser")]
        public SkillBrowserSettings SkillBrowser { get; set; }

        [XmlElement("itemBrowser")]
        public ItemBrowserSettings ItemBrowser { get; set; }

        [XmlElement("blueprintBrowser")]
        public BlueprintBrowserSettings BlueprintBrowser { get; set; }

        [XmlElement("systemTrayIcon")]
        public SystemTrayBehaviour SystemTrayIcon { get; set; }

        [XmlElement("systemTrayPopup")]
        public TrayPopupSettings SystemTrayPopup { get; set; }

        [XmlElement("calendar")]
        public SchedulerUISettings Scheduler { get; set; }

        [XmlElement("systemTrayTooltip")]
        public TrayTooltipSettings SystemTrayTooltip { get; set; }

        [XmlElement("skillPieChart")]
        public SkillPieChartSettings SkillPieChart { get; set; }

        [XmlElement("locations")]
        public SerializableDictionary<String, SerializableRectangle> WindowLocations { get; set; }

        [XmlElement("splitters")]
        public SerializableDictionary<String, int> Splitters { get; set; }

        [XmlArray("confirmedTips")]
        [XmlArrayItem("tip")]
        public List<String> ConfirmedTips { get; set; }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal UISettings Clone()
        {
            UISettings clone = (UISettings)MemberwiseClone();
            clone.CertificateBrowser = CertificateBrowser.Clone();
            clone.SystemTrayTooltip = SystemTrayTooltip.Clone();
            clone.BlueprintBrowser = BlueprintBrowser.Clone();
            clone.SystemTrayPopup = SystemTrayPopup.Clone();
            clone.SkillPieChart = SkillPieChart.Clone();
            clone.SkillBrowser = SkillBrowser.Clone();
            clone.ShipBrowser = ShipBrowser.Clone();
            clone.ItemBrowser = ItemBrowser.Clone();
            clone.MainWindow = MainWindow.Clone();
            clone.PlanWindow = PlanWindow.Clone();
            clone.Scheduler = Scheduler.Clone();

            clone.WindowLocations = new SerializableDictionary<string, SerializableRectangle>();
            foreach (KeyValuePair<string, SerializableRectangle> pair in WindowLocations)
            {
                clone.WindowLocations[pair.Key] = pair.Value.Clone();
            }

            return clone;
        }

        #endregion
    }
}