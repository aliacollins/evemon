using System;
using System.Xml.Serialization;
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Serialization.Eve
{
    public sealed class SerializableAPIKeyItem : SerializableAPICharacters
    {
        [XmlAttribute("accessMask")]
        public int AccessMask { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("expires")]
        public string ExpirationXml
        {
            get { return Expiration.DateTimeToTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    Expiration = value.TimeStringToDateTime();
            }
        }

        [XmlIgnore]
        public DateTime Expiration { get; set; }
    }
}