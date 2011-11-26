using System.Text.RegularExpressions;
using System.Web;

namespace EVEMon.Common
{
    public static class StringExtensions
    {
        /// <summary>
        ///Converts a string that has been HTML-encoded for HTTP transmission into a decoded string.        
        /// </summary>
        /// <param name="text">The string to decode.</param>
        /// <returns></returns>
        public static string HtmlDecode(this string text)
        {
            while (text != HttpUtility.HtmlDecode(text))
            {
                text = HttpUtility.HtmlDecode(text);
            }
            return text;
        }

        /// <summary>
        /// Determines whether the string is of a valid email format.
        /// </summary>
        /// <param name="strIn">The string.</param>
        /// <returns>
        /// 	<c>true</c> if the string is of a valid email format; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidEmail(this string strIn)
        {
            // Return true if strIn is in valid e-mail format
            return Regex.IsMatch(strIn,
                   @"^(?("")(""[^""]+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
                   @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");
        }
    }
}
