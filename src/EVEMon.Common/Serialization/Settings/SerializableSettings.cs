using System.Collections.ObjectModel;
using System.Xml.Serialization;
using EVEMon.Common.Enumerations.UISettings;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.Common.Serialization.Settings
{
    /// <summary>
    /// This class is a temporary representation of the <see cref="EVEMon.Common.Settings" /> class for serialization purposes through automatic serialization
    /// </summary>
    [XmlRoot("Settings")]
    public sealed class SerializableSettings
    {
        private readonly Collection<SerializablePlan> m_plans;
        private readonly Collection<SerializableAPIKey> m_apiKeys;
        private readonly Collection<SerializableSettingsCharacter> m_characters;
        private readonly Collection<MonitoredCharacterSettings> m_monitoredCharacters;

        public SerializableSettings()
        {
            m_plans = new Collection<SerializablePlan>();
            m_apiKeys = new Collection<SerializableAPIKey>();
            m_characters = new Collection<SerializableSettingsCharacter>();
            m_monitoredCharacters = new Collection<MonitoredCharacterSettings>();
            CloudStorageServiceProvider = new CloudStorageServiceProviderSettings();
            PortableEveInstallations = new PortableEveInstallationsSettings();
            Notifications = new NotificationSettings();
            APIProviders = new APIProvidersSettings();
            LoadoutsProvider = new LoadoutsProviderSettings();
            MarketPricer = new MarketPricerSettings();
            Exportation = new ExportationSettings();
            Scheduler = new SchedulerSettings();
            Calendar = new CalendarSettings();
            Updates = new UpdateSettings();
            Proxy = new ProxySettings();
            G15 = new G15Settings();
            UI = new UISettings();
        }

        [XmlAttribute("revision")]
        public int Revision { get; set; }

        [XmlElement("compatibility")]
        public CompatibilityMode Compatibility { get; set; }

        [XmlArray("apiKeys")]
        [XmlArrayItem("apikey")]
        public Collection<SerializableAPIKey> APIKeys => m_apiKeys;

        [XmlArray("characters")]
        [XmlArrayItem("ccp", typeof(SerializableCCPCharacter))]
        [XmlArrayItem("uri", typeof(SerializableUriCharacter))]
        public Collection<SerializableSettingsCharacter> Characters => m_characters;

        [XmlArray("plans")]
        [XmlArrayItem("plan")]
        public Collection<SerializablePlan> Plans => m_plans;

        [XmlArray("monitoredCharacters")]
        [XmlArrayItem("character")]
        public Collection<MonitoredCharacterSettings> MonitoredCharacters => m_monitoredCharacters;

        [XmlElement("apiProviders")]
        public APIProvidersSettings APIProviders { get; set; }

        [XmlElement("updates")]
        public UpdateSettings Updates { get; set; }

        [XmlElement("notifications")]
        public NotificationSettings Notifications { get; set; }

        [XmlElement("scheduler")]
        public SchedulerSettings Scheduler { get; set; }

        [XmlElement("calendar")]
        public CalendarSettings Calendar { get; set; }

        [XmlElement("exportation")]
        public ExportationSettings Exportation { get; set; }

        [XmlElement("marketPricer")]
        public MarketPricerSettings MarketPricer { get; set; }

        [XmlElement("loadoutsProvider")]
        public LoadoutsProviderSettings LoadoutsProvider { get; set; }

        [XmlElement("cloudStorageServiceProvider")]
        public CloudStorageServiceProviderSettings CloudStorageServiceProvider { get; set; }

        [XmlElement("portableEveInstallations")]
        public PortableEveInstallationsSettings PortableEveInstallations { get; set; }

        [XmlElement("G15")]
        public G15Settings G15 { get; set; }

        [XmlElement("UI")]
        public UISettings UI { get; set; }

        [XmlElement("proxy")]
        public ProxySettings Proxy { get; set; }
    }
}