using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Service;
using EVEMon.Common.Constants;

namespace EVEMon.Common.Models
{
    public sealed class UpcomingCalendarEvent
    {
        #region Fields

        private readonly CCPCharacter m_ccpCharacter;
        private readonly List<CalendarEventAttendee> m_eventAttendees;
        private readonly long m_eventID;
        private string m_ownerName;
        private bool m_queryPending;

        #endregion


        #region Constructor

        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="ccpCharacter">The CCP character.</param>
        /// <param name="src">The source.</param>
        internal UpcomingCalendarEvent(CCPCharacter ccpCharacter, SerializableUpcomingCalendarEventsListItem src)
        {
            m_ccpCharacter = ccpCharacter;

            m_eventID = src.EventID;
            OwnerID = src.OwnerID;
            m_ownerName = EveIDToName.GetIDToName(OwnerID);
            EventTitle = src.EventTitle;
            EventText = src.EventText;
            Duration = src.Duration;
            Importance = src.Importance;
            Response = src.Response;
            EventDate = src.EventDate;
            m_eventAttendees = new List<CalendarEventAttendee>();
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the owner ID.
        /// </summary>
        public long OwnerID { get; }

        /// <summary>
        /// Gets the name of the owner.
        /// </summary>
        public string OwnerName => (m_ownerName == EveMonConstants.UnknownText) ?
            (m_ownerName = EveIDToName.GetIDToName(OwnerID)) : m_ownerName);

        /// <summary>
        /// Gets the event title.
        /// </summary>
        public string EventTitle { get; }

        /// <summary>
        /// Gets the event text.
        /// </summary>
        public string EventText { get; }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        public int Duration { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="UpcomingCalendarEvent"/> is important.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this <see cref="UpcomingCalendarEvent"/> is important; otherwise, <c>false</c>.
        /// </value>
        public bool Importance { get; }

        /// <summary>
        /// Gets the response.
        /// </summary>
        public string Response { get; }

        /// <summary>
        /// Gets the event date.
        /// </summary>
        public DateTime EventDate { get; }

        /// <summary>
        /// Gets the event attendees.
        /// </summary>
        public IEnumerable<CalendarEventAttendee> Attendees => m_eventAttendees;

        #endregion


        #region Querying

        /// <summary>
        /// Gets the attendees.
        /// </summary>
        public void GetEventAttendees()
        {
            // Exit if we are already trying to download the calendar event attendees
            if (m_queryPending)
                return;

            m_queryPending = true;

            // Quits if access denied
            APIKey apiKey = m_ccpCharacter.Identity.FindAPIKeyWithAccess(CCPAPICharacterMethods.CalendarEventAttendees);
            if (apiKey == null)
                return;

            EveMonClient.APIProviders.CurrentProvider.QueryMethodAsync<SerializableAPICalendarEventAttendees>(
                CCPAPICharacterMethods.CalendarEventAttendees,
                apiKey.ID,
                apiKey.VerificationCode,
                m_ccpCharacter.CharacterID,
                m_eventID,
                OnCalendarEventAttendeesDownloaded);
        }

        /// <summary>
        /// Processes the queried calendar event attendees.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnCalendarEventAttendeesDownloaded(CCPAPIResult<SerializableAPICalendarEventAttendees> result)
        {
            m_queryPending = false;

            // Notify an error occured
            if (m_ccpCharacter.ShouldNotifyError(result, CCPAPICharacterMethods.CalendarEventAttendees))
                EveMonClient.Notifications.NotifyCharacterCalendarEventAttendeesError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Quit if there are no attendees
            if (!result.Result.EventAttendees.Any())
                return;

            // Import the data
            m_eventAttendees.Clear();
            m_eventAttendees.AddRange(result.Result.EventAttendees.Select(attendee => new CalendarEventAttendee(attendee)));

            EveMonClient.OnCharacterCalendarEventAttendeesDownloaded(m_ccpCharacter);
        }

        #endregion
    }
}
