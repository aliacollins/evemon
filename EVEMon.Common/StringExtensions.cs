using System.Globalization;
using System.IO;
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

        /// <summary>
        /// Converts new lines to break lines.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string NewLinesToBreakLines(this string text)
        {
            using (StringReader sr = new StringReader(text))
            using (StringWriter sw = new StringWriter(CultureConstants.InvariantCulture))
            {
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

        /// <summary>
        /// Decodes the unicode characters.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string DecodeUnicodeCharacters(this string text)
        {
            return Regex.Replace(text, @"\\u(?<Value>[a-zA-Z0-9]{4})",
                                 m => ((char)int.Parse(m.Groups["Value"].Value,
                                                       NumberStyles.HexNumber,
                                                       CultureConstants.InvariantCulture)).ToString(
                                                           CultureConstants.DefaultCulture));
        }
    }
}
