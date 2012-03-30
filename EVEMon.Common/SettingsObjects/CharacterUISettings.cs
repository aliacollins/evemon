using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    /// <summary>
    /// Represents the settings for a character.
    /// </summary>
    public sealed class CharacterUISettings
    {
        private readonly Collection<string> m_collapsedGroups;
        private readonly Collection<string> m_advancedFeaturesEnabledPages;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterUISettings"/> class.
        /// </summary>
        public CharacterUISettings()
        {
            SelectedPage = String.Empty;
            m_collapsedGroups = new Collection<string>();
            m_advancedFeaturesEnabledPages = new Collection<string>();
        }

        /// <summary>
        /// Gets or sets the selected page.
        /// </summary>
        /// <value>The selected page.</value>
        [XmlElement("selectedPage")]
        public string SelectedPage { get; set; }

        /// <summary>
        /// Gets or sets the collapsed groups.
        /// </summary>
        /// <value>The collapsed groups.</value>
        [XmlElement("collapsedGroup")]
        public Collection<string> CollapsedGroups
        {
            get { return m_collapsedGroups; }
        }

        /// <summary>
        /// Gets or sets the orders group by.
        /// </summary>
        /// <value>The orders group by.</value>
        [XmlElement("ordersGroupBy")]
        public MarketOrderGrouping OrdersGroupBy { get; set; }

        /// <summary>
        /// Gets or sets the contracts group by.
        /// </summary>
        /// <value>The contracts group by.</value>
        [XmlElement("contractsGroupBy")]
        public ContractGrouping ContractsGroupBy { get; set; }

        /// <summary>
        /// Gets or sets the jobs group by.
        /// </summary>
        /// <value>The jobs group by.</value>
        [XmlElement("jobsGroupBy")]
        public IndustryJobGrouping JobsGroupBy { get; set; }

        /// <summary>
        /// Gets or sets the EVE mail messages group by.
        /// </summary>
        /// <value>The EVE mail messages group by.</value>
        [XmlElement("mailMessagesGroupBy")]
        public EVEMailMessagesGrouping EVEMailMessagesGroupBy { get; set; }

        /// <summary>
        /// Gets or sets the EVE notifications group by.
        /// </summary>
        /// <value>The EVE notifications group by.</value>
        [XmlElement("eveNotificationsGroupBy")]
        public EVENotificationsGrouping EVENotificationsGroupBy { get; set; }

        /// <summary>
        /// Gets or sets the advanced features enabled pages.
        /// </summary>
        /// <value>The advanced features enabled pages.</value>
        [XmlElement("advancedFeaturesEnabledPages")]
        public Collection<string> AdvancedFeaturesEnabledPages
        {
            get { return m_advancedFeaturesEnabledPages; }
        }
    }
}