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
        /// Convert an UpperCamelCase string to lowerCamelCase.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string ConvertUpperToLowerCamelCase(this string text)
        {
            return string.Format("{0}{1}", text.Substring(0, 1).ToLower(), text.Substring(1, text.Length - 1));
        }
    }
}
