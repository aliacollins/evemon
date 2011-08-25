using System;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Settings
{
    /// <summary>
    /// Represents an API method and the last time we updated it from CCP.
    /// </summary>
    public sealed class SerializableAPIUpdate
    {
        [XmlAttribute("method")]
        public APIMethods Method { get; set; }

        [XmlAttribute("time")]
        public DateTime Time { get; set; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public SerializableAPIUpdate Clone()
        {
            return (SerializableAPIUpdate)MemberwiseClone();
        }
    }
}
