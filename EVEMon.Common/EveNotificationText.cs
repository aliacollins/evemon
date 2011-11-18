using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
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
            text = NewLinesToBreakLines(text);
            text = DecodeUnicodeCharacters(text);
            return text;
        }

        /// <summary>
        /// Converts new lines to break lines.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private static string NewLinesToBreakLines(string text)
        {
            StringReader sr = new StringReader(text);
            StringWriter sw = new StringWriter(CultureConstants.InvariantCulture);

            //Loop while next character exists
            while (sr.Peek() > -1)
            {
                // Read a line from the string and store it to a temp variable
                string temp = sr.ReadLine();
                // Write the string with the HTML break tag
                // (method writes to an internal StringBuilder created automatically)
                sw.Write("{0}<br>", temp);
            }
            return sw.GetStringBuilder().ToString();
        }

        /// <summary>
        /// Decodes the unicode characters.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private static string DecodeUnicodeCharacters(string text)
        {
            return Regex.Replace(text, @"\\u(?<Value>[a-zA-Z0-9]{4})",
                                 m =>
                                 ((char)
                                  int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber, CultureConstants.InvariantCulture)).
                                     ToString(CultureConstants.DefaultCulture));
        }

        #endregion
    }
}