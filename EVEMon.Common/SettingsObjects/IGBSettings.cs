using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    public sealed class IGBSettings
    {
        public IGBSettings()
        {
            IGBServerPort = 80;
        }

        [XmlElement("igbServerEnabled")]
        public bool IGBServerEnabled { get; set; }

        [XmlElement("igbServerPublic")]
        public bool IGBServerPublic { get; set; }

        [XmlElement("igbServerPort")]
        public int IGBServerPort { get; set; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal IGBSettings Clone()
        {
            return (IGBSettings)MemberwiseClone();
        }
    }
}
