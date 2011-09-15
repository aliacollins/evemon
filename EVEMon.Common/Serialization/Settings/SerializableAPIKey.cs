using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Settings
{
    public sealed class SerializableAPIKey
    {
        public SerializableAPIKey()
        {
            IgnoreList = new List<SerializableCharacterIdentity>();
        }

        [XmlAttribute("id")]
        public long ID { get; set; }

        [XmlAttribute("vCode")]
        public string VerificationCode { get; set; }

        [XmlAttribute("accessMask")]
        public long AccessMask { get; set; }

        [XmlAttribute("type")]
        public APIKeyType Type { get; set; }

        [XmlAttribute("expires")]
        public DateTime Expiration { get; set; }

        [XmlAttribute("lastUpdate")]
        public DateTime LastUpdate { get; set; }

        [XmlAttribute("monitored")]
        public bool Monitored { get; set; }

        [XmlArray("ignoredCharacters")]
        [XmlArrayItem ("character")]
        public List<SerializableCharacterIdentity> IgnoreList { get; set; }
    }
}
