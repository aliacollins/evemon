using EVEMon.Common.Serialization.Eve;

namespace EVEMon.Common.Models
{
    public sealed class CalendarEventAttendee
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarEventAttendee"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        internal CalendarEventAttendee(SerializableCalendarEventAttendeeListItem src)
        {
            CharacterID = src.CharacterID;
            CharacterName = src.CharacterName;
            Response = src.Response;
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the character ID.
        /// </summary>
        public long CharacterID { get; private set; }

        /// <summary>
        /// Gets the name of the character.
        /// </summary>
        public string CharacterName { get; private set; }

        /// <summary>
        /// Gets the response.
        /// </summary>
        public string Response { get; private set; }

        #endregion
    }
}