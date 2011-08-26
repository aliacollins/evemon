using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    public sealed class ProxySettings
    {
        [XmlAttribute("enabled")]
        public bool Enabled { get; set; }

        [XmlElement("host")]
        public string Host { get; set; }

        [XmlElement("port")]
        public int Port { get; set; }

        [XmlElement("authenticationType")]
        public ProxyAuthentication Authentication { get; set; }

        [XmlElement("username")]
        public string Username { get; set; }

        [XmlElement("password")]
        public string Password { get; set; }

        [XmlElement("disableRequestsOnAuthenticationFailure")]
        public bool DisableRequestsOnAuthenticationFailure { get; set; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public ProxySettings Clone()
        {
            return (ProxySettings)MemberwiseClone();
        }
    }
}