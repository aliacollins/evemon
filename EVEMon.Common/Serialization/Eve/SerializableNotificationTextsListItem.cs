using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Eve
{
    public sealed class SerializableNotificationTextsListItem
    {
        [XmlAttribute("notificationID")]
        public long NotificationID { get; set; }

        [XmlText]
        public string NotificationText { get; set; }
    }
}