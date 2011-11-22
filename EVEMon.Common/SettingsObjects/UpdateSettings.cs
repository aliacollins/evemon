using System;
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
        private readonly SerializableDictionary<String, UpdatePeriod> m_periods;

        private string m_updatesUrl;
        private int m_updateFrequency;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateSettings"/> class.
        /// </summary>
        public UpdateSettings()
        {
            CheckTimeOnStartup = true;
            CheckEVEMonVersion = true;
            HttpTimeout = 20;
            m_periods = new SerializableDictionary<String, UpdatePeriod>();
            IgnoreNetworkStatus = false;
            UpdateFrequency = 240;
            UseCustomUpdatesUrl = false;
            UpdatesAddress = NetworkConstants.EVEMonUpdates;
        }

        /// <summary>
        /// When true, EVEMon will check its version from BattleClinic
        /// </summary>
        [XmlElement("checkEVEMonVersion")]
        public bool CheckEVEMonVersion { get; set; }

        /// <summary>
        /// When true, EVEMon will check its version from BattleClinic
        /// </summary>
        [XmlElement("checkTimeOnStartup")]
        public bool CheckTimeOnStartup { get; set; }

        /// <summary>
        /// Gets or sets the latest upgrade version the user choose to reject.
        /// </summary>
        [XmlElement("mostRecentDeniedUpdgrade")]
        public string MostRecentDeniedUpgrade { get; set; }

        /// <summary>
        /// Gets or sets the length of time between updates in minutes.
        /// </summary>
        /// <remarks>
        /// Hidden Setting. The value of this setting must be equal to or higher than 240 minutes, the default is 240 minutes (4 hours).
        /// </remarks>
        [XmlElement("updateFrequency")]
        public int UpdateFrequency
        {
            get { return m_updateFrequency < 240 ? 240 : m_updateFrequency; }
            set { m_updateFrequency = value; }
        }

        [XmlElement("useCustomUpdatesUrl")]
        public bool UseCustomUpdatesUrl { get; set; }

        /// <summary>
        /// Url to patch.xml
        /// </summary>
        /// <remarks>
        /// Hidden Setting.
        /// </remarks>
        [XmlElement("updatesUrl")]
        public string UpdatesAddress
        {
            get
            {
                if (!UseCustomUpdatesUrl)
                    return NetworkConstants.EVEMonUpdates;

                if (String.IsNullOrEmpty(m_updatesUrl))
                    return NetworkConstants.EVEMonUpdates;

                // We don't want this to be abused, so we lock the custom update url to localhost.
                // For convenience any localhost path can be used on any port. file:// does not work anyway.
                return !m_updatesUrl.StartsWith("http://localhost:", StringComparison.OrdinalIgnoreCase)
                           ? NetworkConstants.EVEMonUpdates
                           : m_updatesUrl;
            }
            set { m_updatesUrl = value; }
        }

        /// <summary>
        /// Gets or sets the HTTP timeout.
        /// </summary>
        /// <value>The HTTP timeout.</value>
        [XmlElement("httpTimeout")]
        public int HttpTimeout { get; set; }

        /// <summary>
        /// Short circuit the check for network connectivity and try and connect anyway.
        /// </summary>
        /// <value><c>true</c> if [ignore network status]; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// Hidden setting, no UI. Used for the hand full of people using Wine/Darwine with a broken .NET Network Stack.
        /// </remarks>
        [XmlElement("ignoreNetworkStatus")]
        public bool IgnoreNetworkStatus { get; set; }

        /// <summary>
        /// Gets or sets the periods.
        /// </summary>
        /// <value>The periods.</value>
        [XmlElement("periods")]
        public SerializableDictionary<String, UpdatePeriod> Periods
        {
            get { return m_periods; }
        }
    }
}