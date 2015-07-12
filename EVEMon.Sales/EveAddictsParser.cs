using System;
using System.Text.RegularExpressions;

namespace EVEMon.Sales
{
    public class EveAddictsParser : IMineralParser
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return "eveaddicts"; }
        }
        
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get { return "Eve Addicts Mineral Feed"; }
        }

        /// <summary>
        /// Gets the courtesy URL.
        /// </summary>
        /// <value>The courtesy URL.</value>
        public Uri CourtesyUrl
        {
            get { return new Uri("http://eve.addicts.nl/"); }
        }

        /// <summary>
        /// Gets the courtesy text.
        /// </summary>
        /// <value>The courtesy text.</value>
        public string CourtesyText
        {
            get { return "Eve Addicts"; }
        }

        /// <summary>
        /// Gets the tokenizer.
        /// </summary>
        /// <value>The tokenizer.</value>
        public Regex Tokenizer
        {
            get
            {
                return new Regex(@"<typeID\sid='(?<name>\d+?)'>.+?<five_percent_sell_price>(?<price>.+?)</five_percent_sell_price>",
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
            get { return new Uri("http://eve.addicts.nl/api/prices.php?typeID=34,35,36,37,38,39,40,11399&detailed=true"); }
        }
    }
}