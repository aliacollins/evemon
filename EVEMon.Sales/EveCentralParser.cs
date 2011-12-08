using System;
using System.Text.RegularExpressions;

namespace EVEMon.Sales
{
    public class EveCentralParser : IMineralParser
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return "evecentral"; }
        }


        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get { return "EVE-Central Mineral Feed"; }
        }

        /// <summary>
        /// Gets the courtesy URL.
        /// </summary>
        /// <value>The courtesy URL.</value>
        public Uri CourtesyUrl
        {
            get { return new Uri("http://eve-central.com"); }
        }

        /// <summary>
        /// Gets the courtesy text.
        /// </summary>
        /// <value>The courtesy text.</value>
        public string CourtesyText
        {
            get { return "Eve-Central"; }
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
            get { return new Uri("http://api.eve-central.com/api/evemon"); }
        }
    }
}