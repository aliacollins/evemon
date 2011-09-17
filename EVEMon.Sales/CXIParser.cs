using System;
using System.Text.RegularExpressions;

namespace EVEMon.Sales
{
    [DefaultMineralParser("cxi")]
    public class CXIParser : IMineralParser
    {
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get { return "CompleXion Industries"; }
        }

        /// <summary>
        /// Gets the courtesy URL.
        /// </summary>
        /// <value>The courtesy URL.</value>
        public Uri CourtesyUrl
        {
            get { return new Uri("http://cxa.supreme-eve.com/wiki/"); }
        }

        /// <summary>
        /// Gets the courtesy text.
        /// </summary>
        /// <value>The courtesy text.</value>
        public string CourtesyText
        {
            get { return "CompleXion Industries"; }
        }

        /// <summary>
        /// Gets the tokenizer.
        /// </summary>
        /// <value>The tokenizer.</value>
        public Regex Tokenizer
        {
            get
            {
                return new Regex(@"<name>(?<name>.+?)</name>.+?<price>(?<price>.+?)</price>",
                                 RegexOptions.Compiled
                                 | RegexOptions.IgnorePatternWhitespace
                                 | RegexOptions.Singleline
                                 | RegexOptions.Multiline
                                 | RegexOptions.IgnoreCase);
            }
        }

        /// <summary>
        /// Gets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public Uri URL
        {
            get { return new Uri("http://www.c-l-o-t.com/cxi/orecalc/evemon.php"); }
        }
    }
}