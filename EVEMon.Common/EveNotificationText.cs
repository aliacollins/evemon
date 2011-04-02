using System;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class EveNotificationText
    {
        public EveNotificationText(SerializableNotificationTextsListItem src)
        {
            NotificationID = src.NotificationID;
            NotificationText = src.NotificationText.Normalize();
        }

        private long NotificationID { get; set; }

        public string NotificationText { get; private set; }
    }
}
