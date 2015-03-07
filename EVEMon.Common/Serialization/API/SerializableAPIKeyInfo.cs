using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    /// <summary>
    /// Represents a serializable version of API key info. Used for querying CCP.
    /// </summary>
    public sealed class SerializableAPIKeyInfo
    {
        [XmlElement("key")]
        public SerializableAPIKeyItem Key { get; set; }
    }
}
