using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    public sealed class MainWindowSettings
    {
        public MainWindowSettings()
        {
            ShowOverview = true;
            ShowMenuBar = true;
            MarketOrders = new MarketOrderSettings();
            IndustryJobs = new IndustryJobSettings();
            Research = new ResearchSettings();
            OverviewItemSize = PortraitSizes.x96;
            TitleFormat = MainWindowTitleFormat.NextCharToFinish;
            ShowOverviewSkillQueueFreeRoom = true;
            ShowOverviewPortrait = true;
            ShowOverviewWallet = true;
            PutTrainingSkillsFirstOnOverview = true;
            ShowRelocationMenu = true;
            EnableAutomaticRelocation = false;
            AutomaticRelocationInterval = (int)TimeSpan.FromSeconds(10).TotalSeconds;
        }

        [XmlElement("showCharacterInfoInTitleBar")]
        public bool ShowCharacterInfoInTitleBar
        {
            get;
            set;
        }

        [XmlElement("showRelocationMenu")]
        public bool ShowRelocationMenu
        {
            get;
            set;
        }

        [XmlElement("enableAutomaticRelocation")]
        public bool EnableAutomaticRelocation
        {
            get;
            set;
        }

        [XmlElement("automaticRelocationInterval")]
        public int AutomaticRelocationInterval
        {
            get;
            set;
        }

        [XmlElement("autoRelocateDefaultMonitor")]
        public int AutoRelocateDefaultMonitor
        {
            get;
            set;
        }

        [XmlElement("showOverview")]
        public bool ShowOverview
        {
            get;
            set;
        }

        [XmlElement("showMenuBar")]
        public bool ShowMenuBar
        {
            get;
            set;
        }

        [XmlElement("showToolBar")]
        public bool ShowToolBar
        {
            get;
            set;
        }

        [XmlElement("titleFormat")]
        public MainWindowTitleFormat TitleFormat
        {
            get;
            set;
        }

        [XmlElement("showSkillNameInWindowTitle")]
        public bool ShowSkillNameInWindowTitle
        {
            get;
            set;
        }

        [XmlElement("showAllPublicSkills")]
        public bool ShowAllPublicSkills
        {
            get;
            set;
        }

        [XmlElement("showNonPublicSkills")]
        public bool ShowNonPublicSkills
        {
            get;
            set;
        }

        [XmlElement("showPrereqMetSkills")]
        public bool ShowPrereqMetSkills
        {
            get;
            set;
        }

        [XmlElement("highlightPartialSkills")]
        public bool HighlightPartialSkills
        {
            get;
            set;
        }

        [XmlElement("highlightQueuedSkills")]
        public bool HighlightQueuedSkills
        {
            get;
            set;
        }

        [XmlElement("alwaysShowSkillQueueTime")]
        public bool AlwaysShowSkillQueueTime
        {
            get;
            set;
        }

        [XmlElement("overviewIndex")]
        public int OverviewIndex
        {
            get;
            set;
        }

        [XmlElement("overviewItemSize")]
        public PortraitSizes OverviewItemSize
        {
            get;
            set;
        }

        [XmlElement("showWalletOnOverview")]
        public bool ShowOverviewWallet
        {
            get;
            set;
        }

        [XmlElement("showPortraitOnOverview")]
        public bool ShowOverviewPortrait
        {
            get;
            set;
        }

        [XmlElement("showOverviewSkillQueueFreeRoom")]
        public bool ShowOverviewSkillQueueFreeRoom
        {
            get;
            set;
        }

        [XmlElement("putTrainingSkillsFirstOnOverview")]
        public bool PutTrainingSkillsFirstOnOverview
        {
            get;
            set;
        }

        [XmlElement("useIncreasedContrastOnOverview")]
        public bool UseIncreasedContrastOnOverview
        {
            get;
            set;
        }

        [XmlElement("marketOrders")]
        public MarketOrderSettings MarketOrders
        {
            get;
            set;
        }

        [XmlElement("industryJobs")]
        public IndustryJobSettings IndustryJobs
        {
            get;
            set;
        }

        [XmlElement("research")]
        public ResearchSettings Research
        {
            get;
            set;
        }

        internal MainWindowSettings Clone()
        {
            return (MainWindowSettings)MemberwiseClone();
        }
    }

    /// <summary>
    /// Represents what is displayed in the main window title.
    /// </summary>
    public enum MainWindowTitleFormat
    {
        Default = 0,
        NextCharToFinish = 1,
        SelectedChar = 2,
        AllCharacters = 3,
        AllCharactersButSelectedOneAhead = 4
    }
}