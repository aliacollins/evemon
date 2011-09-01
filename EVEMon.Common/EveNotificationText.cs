using System.IO;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class EveNotificationText
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EveNotificationText"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        public EveNotificationText(SerializableNotificationTextsListItem src)
        {
            NotificationID = src.NotificationID;
            NotificationText = NewLinesToBreakLines(src.NotificationText.Normalize());
        }

        /// <summary>
        /// Gets or sets the notification ID.
        /// </summary>
        /// <value>The notification ID.</value>
        public long NotificationID { get; set; }

        /// <summary>
        /// Gets or sets the notification text.
        /// </summary>
        /// <value>The notification text.</value>
        public string NotificationText { get; private set; }

        private static string NewLinesToBreakLines(string text)
        {
            StringReader sr = new StringReader(text);
            StringWriter sw = new StringWriter();

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
    }
}