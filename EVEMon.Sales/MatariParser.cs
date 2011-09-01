using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using EVEMon.Common;
using EVEMon.Common.Net;

namespace EVEMon.Sales
{
    [DefaultMineralParser("matari")]
    public class MatariParser : IMineralParser
    {
        private static readonly Regex s_mineralTokenizer =
            new Regex(@"\<td.*?\>(?<name>\w*)\</td\>\<td.*?align.*?\>(?<price>(\d|\.|,)*)\</td\>\<td",
                      RegexOptions.Compiled);


        #region IMineralParser Members

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
        public string CourtesyUrl
        {
            get { return "http://www.evegeek.com/"; }
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
        /// Gets the prices.
        /// </summary>
        /// <returns>
        /// An enumerable collection of Minerals and Prices.
        /// </returns>
        public IEnumerable<Pair<string, decimal>> GetPrices()
        {
            string phoenixContent;
            try
            {
                phoenixContent = EveMonClient.HttpWebService.DownloadString(
                    "http://www.evegeek.com/mineralindex.php");
            }
            catch (HttpWebServiceException ex)
            {
                ExceptionHandler.LogException(ex, true);
                throw new MineralParserException(ex.Message);
            }

            // Scan for prices
            MatchCollection mc = s_mineralTokenizer.Matches(phoenixContent);

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