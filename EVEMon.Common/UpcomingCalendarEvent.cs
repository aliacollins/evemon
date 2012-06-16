using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class UpcomingCalendarEvent
    {
        #region Fields

        private readonly CCPCharacter m_ccpCharacter;
        private readonly List<CalendarEventAttendee> m_eventAttendees;
        private readonly long m_eventID;
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
            OwnerName = src.OwnerName;
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
        public long OwnerID { get; private set; }

        /// <summary>
        /// Gets the name of the owner.
        /// </summary>
        public string OwnerName { get; private set; }

        /// <summary>
        /// Gets the event title.
        /// </summary>
        public string EventTitle { get; private set; }

        /// <summary>
        /// Gets the event text.
        /// </summary>
        public string EventText { get; private set; }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        public int Duration { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="UpcomingCalendarEvent"/> is important.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this <see cref="UpcomingCalendarEvent"/> is important; otherwise, <c>false</c>.
        /// </value>
        public bool Importance { get; private set; }

        /// <summary>
        /// Gets the response.
        /// </summary>
        public string Response { get; private set; }

        /// <summary>
        /// Gets the event date.
        /// </summary>
        public DateTime EventDate { get; private set; }

        /// <summary>
        /// Gets the event attendees.
        /// </summary>
        public IEnumerable<CalendarEventAttendee> EventAttendees
        {
            get { return m_eventAttendees; }
        }

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
            APIKey apiKey = m_ccpCharacter.Identity.FindAPIKeyWithAccess(APICharacterMethods.CalendarEventAttendees);
            if (apiKey == null)
                return;

            EveMonClient.APIProviders.CurrentProvider.QueryMethodAsync<SerializableAPICalendarEventAttendees>(
                APICharacterMethods.CalendarEventAttendees,
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
        private void OnCalendarEventAttendeesDownloaded(APIResult<SerializableAPICalendarEventAttendees> result)
        {
            m_queryPending = false;

            // Notify an error occured
            if (m_ccpCharacter.ShouldNotifyError(result, APICharacterMethods.CalendarEventAttendees))
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
