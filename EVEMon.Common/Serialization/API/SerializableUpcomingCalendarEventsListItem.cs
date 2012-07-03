using System;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableUpcomingCalendarEventsListItem
    {
        [XmlAttribute("eventID")]
        public long EventID { get; set; }

        [XmlAttribute("ownerID")]
        public long OwnerID { get; set; }

        [XmlAttribute("ownerName")]
        public string OwnerName { get; set; }

        [XmlAttribute("eventTitle")]
        public string EventTitle { get; set; }

        [XmlAttribute("eventText")]
        public string EventText { get; set; }

        [XmlAttribute("duration")]
        public int Duration { get; set; }

        [XmlAttribute("importance")]
        public bool Importance { get; set; }

        [XmlAttribute("response")]
        public string Response { get; set; }

        [XmlAttribute("eventDate")]
        public string EventDateXml
        {
            get { return EventDate.DateTimeToTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    EventDate = value.TimeStringToDateTime();
            }
        }

        [XmlIgnore]
        public DateTime EventDate { get; set; }
    }
}