using System.Collections.Generic;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common.Models.Collections
{
    public sealed class UpcomingCalendarEventCollection : ReadonlyCollection<UpcomingCalendarEvent>
    {
        private readonly CCPCharacter m_character;

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="character">The character.</param>
        internal UpcomingCalendarEventCollection(CCPCharacter character)
        {
            m_character = character;
        }

        /// <summary>
        /// Imports an enumeration of API objects.
        /// </summary>
        /// <param name="src">The enumeration of serializable standings from the API.</param>
        internal void Import(IEnumerable<SerializableUpcomingCalendarEventsListItem> src)
        {
            Items.Clear();

            // Import the standings from the API
            foreach (SerializableUpcomingCalendarEventsListItem srcEvent in src)
            {
                Items.Add(new UpcomingCalendarEvent(m_character, srcEvent));
            }
        }

    }
}
