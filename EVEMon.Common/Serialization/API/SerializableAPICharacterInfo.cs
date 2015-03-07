using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    /// <summary>
    /// Represents a serializable version of a character's info. Used for querying CCP and settings.
    /// </summary>
    public sealed class SerializableAPICharacterInfo
    {
        private readonly Collection<SerializableEmploymentHistoryListItem> m_employmentHistory;

        public SerializableAPICharacterInfo()
        {
            m_employmentHistory = new Collection<SerializableEmploymentHistoryListItem>();
        }

        [XmlElement("shipName")]
        public string ShipNameXml
        {
            get { return ShipName; }
            set { ShipName = value == null ? String.Empty : value.HtmlDecode(); }
        }

        [XmlElement("shipTypeName")]
        public string ShipTypeName { get; set; }

        [XmlElement("lastKnownLocation")]
        public string LastKnownLocation { get; set; }

        [XmlElement("securityStatus")]
        public double SecurityStatus { get; set; }

        [XmlArray("employmentHistory")]
        [XmlArrayItem("record")]
        public Collection<SerializableEmploymentHistoryListItem> EmploymentHistory
        {
            get { return m_employmentHistory; }
        }

        [XmlIgnore]
        public string ShipName { get; set; }
    }
}