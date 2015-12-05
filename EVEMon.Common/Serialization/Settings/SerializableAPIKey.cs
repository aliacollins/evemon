using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Enumerations.CCPAPI;

namespace EVEMon.Common.Serialization.Settings
{
    public sealed class SerializableAPIKey
    {
        private readonly Collection<SerializableCharacterIdentity> m_ignoreList;

        public SerializableAPIKey()
        {
            m_ignoreList = new Collection<SerializableCharacterIdentity>();
        }

        [XmlAttribute("id")]
        public long ID { get; set; }

        [XmlAttribute("vCode")]
        public string VerificationCode { get; set; }

        [XmlAttribute("accessMask")]
        public long AccessMask { get; set; }

        [XmlAttribute("type")]
        public CCPAPIKeyType Type { get; set; }

        [XmlAttribute("expires")]
        public DateTime Expiration { get; set; }

        [XmlAttribute("lastUpdate")]
        public DateTime LastUpdate { get; set; }

        [XmlAttribute("monitored")]
        public bool Monitored { get; set; }

        [XmlArray("ignoredCharacters")]
        [XmlArrayItem("character")]
        public Collection<SerializableCharacterIdentity> IgnoreList
        {
            get { return m_ignoreList; }
        }
    }
}
