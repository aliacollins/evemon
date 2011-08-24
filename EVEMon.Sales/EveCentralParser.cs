using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using EVEMon.Common;
using EVEMon.Common.Net;

namespace EVEMon.Sales
{
    [DefaultMineralParser("evecentral")]
    public class EveCentralParser : IMineralParser
    {

        private static readonly Regex s_mineralTokenizer =
            new Regex(@"<name>(?<name>.+?)</name>.+?<price>(?<price>.+?)</price>",
                      RegexOptions.Compiled
                      | RegexOptions.IgnorePatternWhitespace
                      | RegexOptions.Singleline
                      | RegexOptions.Multiline
                      | RegexOptions.IgnoreCase);


        #region Parser Information

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
        public string CourtesyUrl
        {
            get { return "http://eve-central.com"; }
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
        /// Gets the prices.
        /// </summary>
        /// <returns>
        /// An enumerable collection of Minerals and Prices.
        /// </returns>
        public IEnumerable<Pair<string, decimal>> GetPrices()
        {
            string content;
            try
            {
                content = EveMonClient.HttpWebService.DownloadString(
                    "http://api.eve-central.com/api/evemon");
            }
            catch (HttpWebServiceException ex)
            {
                ExceptionHandler.LogException(ex, true);
                throw new MineralParserException(ex.Message);
            }

            // Scan for prices
            MatchCollection mc = s_mineralTokenizer.Matches(content);

            return mc.Cast<Match>().Select(
                mineral => new
                               {
                                   mineral,
                                   name = mineral.Groups["name"].Value
                               }).Select(
                                   mineral => new
                                                  {
                                                      mineral,
                                                      price = Decimal.Parse(mineral.mineral.Groups["price"].Value,
                                                                            NumberStyles.Currency, CultureInfo.InvariantCulture)
                                                  }).Select(
                                                      mineral => new Pair<string, Decimal>(mineral.mineral.name, mineral.price));
        }

        #endregion
    }
}
