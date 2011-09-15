using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableAPIKeyInfo
    {
        [XmlElement("key")]
        public SerializableAPIKeyItem Key { get; set; }
    }
}
