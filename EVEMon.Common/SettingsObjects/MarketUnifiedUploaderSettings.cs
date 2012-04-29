using System.Collections.ObjectModel;
using System.Xml.Serialization;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common.SettingsObjects
{
    public sealed class MarketUnifiedUploaderSettings
    {
        private readonly Collection<SerializableEndPoint> m_endpoints;

        public MarketUnifiedUploaderSettings()
        {
            Enabled = true;
            m_endpoints = new Collection<SerializableEndPoint>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MarketUnifiedUploaderSettings"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        [XmlElement("enabled")]
        public bool Enabled { get; set; }

        [XmlArray("endpoints")]
        [XmlArrayItem("endpoint")]
        public Collection<SerializableEndPoint> EndPoints { get { return m_endpoints; } }
    }

}
