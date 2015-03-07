using System;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class EveNotificationText
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EveNotificationText"/> class.
        /// </summary>
        /// <param name="src">The source.</param>
        public EveNotificationText(SerializableNotificationTextsListItem src)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            NotificationID = src.NotificationID;
            NotificationText = FormatText(src.NotificationText);
        }


        #region Public Properties

        /// <summary>
        /// Gets or sets the notification ID.
        /// </summary>
        /// <value>The notification ID.</value>
        public long NotificationID { get; private set; }

        /// <summary>
        /// Gets or sets the notification text.
        /// </summary>
        /// <value>The notification text.</value>
        public string NotificationText { get; private set; }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Formats the notification text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private static string FormatText(string text)
        {
            text = text.NewLinesToBreakLines();
            text = text.DecodeUnicodeCharacters();
            return text;
        }

        #endregion
    }
}