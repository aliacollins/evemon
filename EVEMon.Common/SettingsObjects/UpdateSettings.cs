using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using EVEMon.Common.Attributes;

namespace EVEMon.Common.SettingsObjects
{
    /// <summary>
    /// Settings for the updates from CCP and others
    /// </summary>
    [EnforceUIThreadAffinity]
    public sealed class UpdateSettings
    {
        public UpdateSettings()
        {
            CheckTimeOnStartup = true;
            CheckEVEMonVersion = true;
            HttpTimeout = 10;
            Periods = new SerializableDictionary<APIMethods, UpdatePeriod>();
            IgnoreNetworkStatus = false;
        }

        /// <summary>
        /// When true, EVEMon will check its version from Battleclinic
        /// </summary>
        [XmlElement("checkEVEMonVersion")]
        public bool CheckEVEMonVersion
        {
            get;
            set;
        }

        /// <summary>
        /// When true, EVEMon will check its version from Battleclinic
        /// </summary>
        [XmlElement("checkTimeOnStartup")]
        public bool CheckTimeOnStartup
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the latest upgrade version the user choose to reject.
        /// </summary>
        [XmlElement("mostRecentDeniedUpdgrade")]
        public string MostRecentDeniedUpgrade
        {
            get;
            set;
        }

        [XmlElement("periods")]
        public SerializableDictionary<APIMethods, UpdatePeriod> Periods
        {
            get;
            set;
        }

        [XmlElement("httpTimeout")]
        public int HttpTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// Short circuit the check for network connectivity and try and connect anyway.
        /// </summary>
        /// <remarks>
        /// Hidden setting, no UI. Used for the hand full of people using Wine/Darwine with a broken .NET Network Stack.
        /// </remarks>
        [XmlElement("ignoreNetworkStatus")]
        public bool IgnoreNetworkStatus
        {
            get;
            set;
        }
        
        internal UpdateSettings Clone()
        {
            var clone = new UpdateSettings();
            clone.CheckEVEMonVersion = this.CheckEVEMonVersion;
            clone.CheckTimeOnStartup = this.CheckTimeOnStartup;
            clone.MostRecentDeniedUpgrade = this.MostRecentDeniedUpgrade;
            clone.HttpTimeout = this.HttpTimeout;
            clone.IgnoreNetworkStatus = this.IgnoreNetworkStatus;

            foreach (var pair in this.Periods)
            {
                clone.Periods.Add(pair.Key, pair.Value);
            }
            return clone;
        }
    }

    public enum UpdatePeriod
    {
        [Header("Never")]
        Never,
        [Header("5 Minutes")]
        Minutes5,
        [Header("15 Minutes")]
        Minutes15,
        [Header("30 Minutes")]
        Minutes30,
        [Header("1 Hour")]
        Hours1,
        [Header("2 Hours")]
        Hours2,
        [Header("3 Hours")]
        Hours3,
        [Header("6 Hours")]
        Hours6,
        [Header("12 Hours")]
        Hours12,
        [Header("Day")]
        Day,
        [Header("Week")]
        Week
    }

    public enum CacheStyle
    {
        /// <summary>
        /// Short cache style, data will always be returned from CCP,
        /// however it will only be updated once the cache timer
        /// expires.
        /// </summary>
        [Header("Short")]
        Short,
        /// <summary>
        /// Long cache style, data will only be returned from CCP after
        /// the cahce timer has expired.
        /// </summary>
        [Header("Long")]
        Long
    }

    /// <summary>
    /// Provides conversions to durations.
    /// </summary>
    public static class UpdatePeriodExtensions
    {
        public static TimeSpan ToDuration(this UpdatePeriod period)
        {
            switch (period)
            {
                case UpdatePeriod.Never:
                    return TimeSpan.MaxValue;
                case UpdatePeriod.Minutes5:
                    return TimeSpan.FromMinutes(5);
                case UpdatePeriod.Minutes15:
                    return TimeSpan.FromMinutes(15);
                case UpdatePeriod.Minutes30:
                    return TimeSpan.FromMinutes(30);
                case UpdatePeriod.Hours1:
                    return TimeSpan.FromHours(1);
                case UpdatePeriod.Hours2:
                    return TimeSpan.FromHours(2);
                case UpdatePeriod.Hours3:
                    return TimeSpan.FromHours(3);
                case UpdatePeriod.Hours6:
                    return TimeSpan.FromHours(6);
                case UpdatePeriod.Hours12:
                    return TimeSpan.FromHours(12);
                case UpdatePeriod.Day:
                    return TimeSpan.FromDays(1);
                case UpdatePeriod.Week:
                    return TimeSpan.FromDays(7);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
