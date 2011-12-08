using System;
using System.Text.RegularExpressions;

namespace EVEMon.Sales
{
    public class MatariParser : IMineralParser
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return "matari"; }
        }


        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get { return "Matari Mineral Index"; }
        }

        /// <summary>
        /// Gets the courtesy URL.
        /// </summary>
        /// <value>The courtesy URL.</value>
        public Uri CourtesyUrl
        {
            get { return new Uri("http://www.evegeek.com/"); }
        }

        /// <summary>
        /// Gets the courtesy text.
        /// </summary>
        /// <value>The courtesy text.</value>
        public string CourtesyText
        {
            get { return "EVE[geek]"; }
        }

        /// <summary>
        /// Gets the tokenizer.
        /// </summary>
        /// <value>The tokenizer.</value>
        public Regex Tokenizer
        {
            get
            {
                return new Regex(@"\<td.*?\>(?<name>\w*)\</td\>\<td.*?align.*?\>(?<price>(\d|\.|,)*)\</td\>\<td",
                    RegexOptions.Compiled);
            }
        }

        /// <summary>
        /// Gets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public Uri URL
        {
            get { return new Uri("http://www.evegeek.com/mineralindex.php"); }
        }
    }
}