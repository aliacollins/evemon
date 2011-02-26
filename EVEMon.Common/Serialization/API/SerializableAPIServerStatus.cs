using System;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    /// <summary>
    /// This class represents a server status. Used for querying CCP.
    /// </summary>
    public sealed class SerializableAPIServerStatus
    {
        /// <summary>
        /// Default constructor for XML serializer
        /// </summary>
        public SerializableAPIServerStatus()
        {
        }

        [XmlElement("serverOpen")]
        public string CCPOpen
        {
            get;
            set;
        }

        [XmlIgnore]
        public bool Open
        {
            get { return Boolean.Parse(CCPOpen.ToLower(CultureConstants.DefaultCulture)); }
            set { CCPOpen = (value ? "True" : "False"); }
        }

        [XmlElement("onlinePlayers")]
        public int Players
        {
            get;
            set;
        }

    }
}
