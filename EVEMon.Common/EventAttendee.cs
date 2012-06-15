using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class EventAttendee
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EventAttendee"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        internal EventAttendee(SerializableCalendarEventAttendeeListItem src)
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