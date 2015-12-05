using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    /// <summary>
    /// Represents a serializable version of upcoming calendar events. Used for querying CCP.
    /// </summary>
    public sealed class SerializableAPIUpcomingCalendarEvents
    {
        private readonly Collection<SerializableUpcomingCalendarEventsListItem> m_upcomingEvents;

        public SerializableAPIUpcomingCalendarEvents()
        {
            m_upcomingEvents = new Collection<SerializableUpcomingCalendarEventsListItem>();
        }

        [XmlArray("upcomingEvents")]
        [XmlArrayItem("upcomingEvent")]
        public Collection<SerializableUpcomingCalendarEventsListItem> UpcomingEvents
        {
            get { return m_upcomingEvents; }
        }
    }
}
