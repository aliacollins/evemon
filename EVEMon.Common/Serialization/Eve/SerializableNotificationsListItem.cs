using System;
using System.Xml.Serialization;
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Serialization.Eve
{
    public sealed class SerializableNotificationsListItem
    {
        [XmlAttribute("notificationID")]
        public long NotificationID { get; set; }

        [XmlAttribute("typeID")]
        public int TypeID { get; set; }

        [XmlAttribute("senderID")]
        public int SenderID { get; set; }

        [XmlAttribute("senderName")]
        public string SenderName { get; set; }

        [XmlAttribute("sentDate")]
        public string SentDateXml
        {
            get { return SentDate.DateTimeToTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    SentDate = value.TimeStringToDateTime();
            }
        }

        [XmlAttribute("read")]
        public bool Read { get; set; }

        [XmlIgnore]
        public DateTime SentDate { get; set; }
    }
}