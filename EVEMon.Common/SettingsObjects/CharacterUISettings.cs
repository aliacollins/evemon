using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using EVEMon.Common.Attributes;

namespace EVEMon.Common.SettingsObjects
{
    /// <summary>
    /// Represents the settings for a character.
    /// </summary>
    public sealed class CharacterUISettings
    {
        public CharacterUISettings()
        {
            CollapsedGroups = new List<String>();
            FullAPIKeyEnabledPages = new List<String>();
            SelectedPage = String.Empty;
        }

        [XmlElement("selectedPage")]
        public string SelectedPage
        {
            get;
            set;
        }

        [XmlElement("collapsedGroup")]
        public List<String> CollapsedGroups
        {
            get;
            set;
        }

        [XmlElement("fullAPIKeyEnabledPage")]
        public List<String> FullAPIKeyEnabledPages
        {
            get;
            set;
        }

        [XmlElement("ordersGroupBy")]
        public MarketOrderGrouping OrdersGroupBy
        {
            get;
            set;
        }

        [XmlElement("jobsGroupBy")]
        public IndustryJobGrouping JobsGroupBy
        {
            get;
            set;
        }

        /// <summary>
        /// Clones this serialization object.
        /// </summary>
        /// <returns></returns>
        internal CharacterUISettings Clone()
        {
            var clone = new CharacterUISettings();
            clone.CollapsedGroups.AddRange(this.CollapsedGroups);
            clone.FullAPIKeyEnabledPages.AddRange(this.FullAPIKeyEnabledPages);
            clone.SelectedPage = this.SelectedPage;
            clone.OrdersGroupBy = this.OrdersGroupBy;
            clone.JobsGroupBy = this.JobsGroupBy;
            return clone;
        }
    }
}
