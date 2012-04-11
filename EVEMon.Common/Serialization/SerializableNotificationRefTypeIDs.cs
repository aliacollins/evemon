using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization
{
    [XmlRoot("NotificationRefTypeIDs")]
    public sealed class SerializableNotificationRefTypeIDs
    {
        private readonly Collection<SerializableNotificationRefTypeIDsListItem> m_types;

        public SerializableNotificationRefTypeIDs()
        {
            m_types = new Collection<SerializableNotificationRefTypeIDsListItem>();
        }

        [XmlArray("refTypes")]
        [XmlArrayItem("refType")]
        public Collection<SerializableNotificationRefTypeIDsListItem> Types
        {
            get { return m_types; }
        }
    }
}