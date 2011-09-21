using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    public sealed class IGBSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IGBSettings"/> class.
        /// </summary>
        public IGBSettings()
        {
            IGBServerPort = 80;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [IGB server enabled].
        /// </summary>
        /// <value><c>true</c> if [IGB server enabled]; otherwise, <c>false</c>.</value>
        [XmlElement("igbServerEnabled")]
        public bool IGBServerEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [IGB server public].
        /// </summary>
        /// <value><c>true</c> if [IGB server public]; otherwise, <c>false</c>.</value>
        [XmlElement("igbServerPublic")]
        public bool IGBServerPublic { get; set; }

        /// <summary>
        /// Gets or sets the IGB server port.
        /// </summary>
        /// <value>The IGB server port.</value>
        [XmlElement("igbServerPort")]
        public int IGBServerPort { get; set; }
    }
}