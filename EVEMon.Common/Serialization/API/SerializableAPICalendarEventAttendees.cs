using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableAPICalendarEventAttendees
    {
        private readonly Collection<SerializableCalendarEventAttendeeListItem> m_eventAttendees;

        public SerializableAPICalendarEventAttendees()
        {
            m_eventAttendees = new Collection<SerializableCalendarEventAttendeeListItem>();
        }

        [XmlArray("eventAttendees")]
        [XmlArrayItem("eventAttendee")]
        public Collection<SerializableCalendarEventAttendeeListItem> EventAttendees
        {
            get { return m_eventAttendees; }
        }
    }
}
