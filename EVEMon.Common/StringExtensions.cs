using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace EVEMon.Common
{
    public static class StringExtensions
    {
        /// <summary>
        /// Removes the project local path from the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string RemoveProjectLocalPath(this string text)
        {
            return Regex.Replace(text, @"[a-zA-Z]+:.*\\(?=EVEMon)",
                String.Empty, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

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

        /// <summary>
        /// Converts the upper to lower camel case.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static string ConvertUpperToLowerCamelCase(this string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            return String.Concat(text.Substring(0, 1).ToLowerInvariant(), text.Substring(1, text.Length - 1));
        }

        /// <summary>
        /// Converts the specified string to titlecase.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static string ToTitleCase(this string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            string[] words = text.Split(" ".ToCharArray());
            StringBuilder sb = new StringBuilder();

            foreach (string word in words)
            {
                if (String.IsNullOrEmpty(word))
                {
                    sb.Append(" ");
                    continue;
                }

                sb.Append(String.Concat(word.Substring(0, 1).ToUpperInvariant(), word.Substring(1, word.Length - 1).ToLowerInvariant()));
                if (word != words.Last())
                    sb.Append(" ");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Convert an Int32 number to string with the specified decimals.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="decimals">The decimals.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public static string ToNumericString(this int number, int decimals, CultureInfo culture = null)
        {
            return ToNumericString(Convert.ToInt64(number), decimals, culture);
        }

        /// <summary>
        /// Convert an Single number to string with the specified decimals.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="decimals">The decimals.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public static string ToNumericString(this float number, int decimals, CultureInfo culture = null)
        {
            return ToNumericString(Convert.ToDouble(number), decimals, culture);
        }

        /// <summary>
        /// Convert an Decimal number to string with the specified decimals.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="decimals">The decimals.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public static string ToNumericString(this decimal number, int decimals, CultureInfo culture = null)
        {
            return ToNumericString(Convert.ToDouble(number), decimals, culture);
        }

        /// <summary>
        /// Convert an Int64 number to string with the specified decimals.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="decimals">The decimals.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public static string ToNumericString(this long number, int decimals, CultureInfo culture = null)
        {
            if (culture == null)
                culture = CultureConstants.DefaultCulture;

            string decimalsString = String.Format(culture, "N{0}", decimals);
            return number.ToString(decimalsString, culture);
        }

        /// <summary>
        /// Convert an Double number to string with the specified decimals.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="decimals">The decimals.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public static string ToNumericString(this double number, int decimals, CultureInfo culture = null)
        {
            if (culture == null)
                culture = CultureConstants.DefaultCulture;

            string decimalsString = String.Format(culture, "N{0}", decimals);
            return number.ToString(decimalsString, culture);
        }

        /// <summary>
        /// Remove line feeds and some other characters to format the string.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxLength"></param>
        /// <param name="removeNewLine"> </param>
        /// <returns></returns>
        public static string WordWrap(this string text, int maxLength, bool removeNewLine = true)
        {
            if (String.IsNullOrWhiteSpace(text))
                return String.Empty;

            text = removeNewLine
                       ? text.Replace(Environment.NewLine, " ")
                       : text.Replace(Environment.NewLine, " " + Environment.NewLine + " ");

            text = text.Replace(".", ". ");
            text = text.Replace(">", "> ");
            text = text.Replace("\t", " ");
            text = text.Replace(",", ", ");
            text = text.Replace(";", "; ");

            string[] words = text.Split(' ');
            List<string> lines = new List<string>();
            int currentLineLength = 0;
            string currentLine = String.Empty;
            bool inTag = false;

            foreach (string currentWord in words.Where(currentWord => currentWord.Length > 0))
            {
                if (currentWord.Substring(0, 1) == "<")
                    inTag = true;

                if (inTag)
                {
                    //Handle filenames inside html tags
                    if (currentLine.EndsWith(".", StringComparison.CurrentCulture))
                        currentLine += currentWord;
                    else
                        currentLine += currentWord + " ";

                    if (currentWord.IndexOf(">", StringComparison.CurrentCulture) > -1)
                        inTag = false;
                }
                else
                {
                    if (currentWord != Environment.NewLine && currentLine != Environment.NewLine &&
                        currentLineLength + currentWord.Length + 1 < maxLength)
                    {
                        currentLine += currentWord + " ";
                        currentLineLength += (currentWord.Length + 1);
                    }
                    else
                    {
                        lines.Add(currentLine.Trim());
                        currentLine = currentWord + " ";
                        currentLineLength = currentWord.Length;
                    }
                }
            }

            if (currentLine.Length != 0)
                lines.Add(currentLine.Trim());

            string[] textLinesStr = new string[lines.Count];
            lines.CopyTo(textLinesStr, 0);

            return textLinesStr.Aggregate(String.Empty,
                                          (current, line) => String.Format(CultureConstants.DefaultCulture,
                                                                           "{0}{1}{2}", current, line, Environment.NewLine));
        }
    }
}
