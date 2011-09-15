using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    /// <summary>
    /// Represents the settings for a character.
    /// </summary>
    public sealed class CharacterUISettings
    {
        public CharacterUISettings()
        {
            SelectedPage = String.Empty;
            CollapsedGroups = new List<string>();
            AdvancedFeaturesEnabledPages = new List<string>();
        }

        [XmlElement("selectedPage")]
        public string SelectedPage { get; set; }

        [XmlElement("collapsedGroup")]
        public List<string> CollapsedGroups { get; set; }

        [XmlElement("ordersGroupBy")]
        public MarketOrderGrouping OrdersGroupBy { get; set; }

        [XmlElement("jobsGroupBy")]
        public IndustryJobGrouping JobsGroupBy { get; set; }

        [XmlElement("mailMessagesGroupBy")]
        public EVEMailMessagesGrouping EVEMailMessagesGroupBy { get; set; }

        [XmlElement("eveNotificationsGroupBy")]
        public EVENotificationsGrouping EVENotificationsGroupBy { get; set; }

        [XmlElement("advancedFeaturesEnabledPages")]
        public List<string> AdvancedFeaturesEnabledPages { get; set; }

        /// <summary>
        /// Clones this serialization object.
        /// </summary>
        /// <returns></returns>
        internal CharacterUISettings Clone()
        {
            return (CharacterUISettings)MemberwiseClone();
        }
    }
}