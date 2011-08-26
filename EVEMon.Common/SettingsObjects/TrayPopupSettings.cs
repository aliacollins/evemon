using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    /// <summary>
    /// Configuration settings for the tray icon popup window
    /// </summary>
    public sealed class TrayPopupSettings
    {
        public TrayPopupSettings()
        {
            PortraitSize = PortraitSizes.x48;
            HighlightConflicts = true;
            ShowSkillQueueTrainingTime = true;
            ShowSkillInTraining = true;
            ShowRemainingTime = true;
            ShowPortrait = true;
            ShowServerStatus = true;
            ShowEveTime = true;
            ShowWarning = true;
            IndentGroupedAccounts = true;
        }

        [XmlElement("style")]
        public TrayPopupStyles Style { get; set; }

        [XmlElement("groupBy")]
        public TrayPopupGrouping GroupBy { get; set; }

        [XmlElement("primarySortOrder")]
        public TrayPopupSort PrimarySortOrder { get; set; }

        [XmlElement("secondarySortOrder")]
        public TrayPopupSort SecondarySortOrder { get; set; }

        [XmlElement("portraitSize")]
        public PortraitSizes PortraitSize { get; set; }

        [XmlElement("showCharNotTraining")]
        public bool ShowCharNotTraining { get; set; }

        [XmlElement("showSkillInTraining")]
        public bool ShowSkillInTraining { get; set; }

        [XmlElement("showRemainingTime")]
        public bool ShowRemainingTime { get; set; }

        [XmlElement("showTimeToCompletion")]
        public bool ShowCompletionTime { get; set; }

        [XmlElement("showWallet")]
        public bool ShowWallet { get; set; }

        [XmlElement("showPortrait")]
        public bool ShowPortrait { get; set; }

        [XmlElement("showSkillQueueTrainingTime")]
        public bool ShowSkillQueueTrainingTime { get; set; }

        [XmlElement("showWarning")]
        public bool ShowWarning { get; set; }

        [XmlElement("showServerStatus")]
        public bool ShowServerStatus { get; set; }

        [XmlElement("showEveTime")]
        public bool ShowEveTime { get; set; }

        [XmlElement("highlightConflicts")]
        public bool HighlightConflicts { get; set; }

        [XmlElement("indentGroupedAccounts")]
        public bool IndentGroupedAccounts { get; set; }

        [XmlElement("useIncreasedContrast")]
        public bool UseIncreasedContrast { get; set; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public TrayPopupSettings Clone()
        {
            return (TrayPopupSettings)MemberwiseClone();
        }
    }
}