using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Eve
{
    /// <summary>
    /// Represents a serializable version of a characters' eve notifications. Used for querying CCP.
    /// </summary>
    public sealed class SerializableAPINotifications
    {
        private readonly Collection<SerializableNotificationsListItem> m_notifications;

        public SerializableAPINotifications()
        {
            m_notifications = new Collection<SerializableNotificationsListItem>();
        }

        [XmlArray("notifications")]
        [XmlArrayItem("notification")]
        public Collection<SerializableNotificationsListItem> Notifications
        {
            get { return m_notifications; }
        }
    }
}