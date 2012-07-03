using System.Collections.ObjectModel;
using System.Xml.Serialization;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common.SettingsObjects
{
    /// <summary>
    /// Represents a serializable version of the set of API providers. Used for settings persistence.
    /// </summary>
    public sealed class APIProvidersSettings
    {
        private readonly Collection<SerializableAPIProvider> m_customProviders;

        public APIProvidersSettings()
        {
            m_customProviders = new Collection<SerializableAPIProvider>();
        }

        [XmlElement("currentProvider")]
        public string CurrentProviderName { get; set; }

        [XmlArray("customProviders")]
        [XmlArrayItem("provider")]
        public Collection<SerializableAPIProvider> CustomProviders
        {
            get { return m_customProviders; }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return CurrentProviderName;
        }
    }
}