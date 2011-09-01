using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Settings
{
    /// <summary>
    /// Represents a character identity in our settings file
    /// </summary>
    public sealed class SerializableCharacterIdentity
    {
        [XmlElement("id")]
        public long ID { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal SerializableCharacterIdentity Clone()
        {
            return (SerializableCharacterIdentity)MemberwiseClone();
        }
    }
}