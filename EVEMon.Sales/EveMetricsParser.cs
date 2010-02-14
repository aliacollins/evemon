using System;
using System.Collections.Generic;
using System.Text;
using EVEMon.Common;
using System.Text.RegularExpressions;
using System.Globalization;
using EVEMon.Common.Net;

namespace EVEMon.Sales
{
    [DefaultMineralParser("evemetrics")]
    class EveMetricsParser : IMineralParser
    {

        private static Regex mineralTokenizer =
            new Regex(@"<name>(?<name>.+?)</name>.+?<global>.+?<buy>.+?<average>(?<average>.+?)</average>.+?</global>",
                      RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline
                        | RegexOptions.Multiline
                        | RegexOptions.IgnoreCase);


        #region Parser Information
        public string Title
        {
            get { return "EVE Metrics API"; }
        }

        public string CourtesyUrl
        {
            get { return "http://eve-metrics.com"; }
        }

        public string CourtesyText
        {
            get { return "EVE Metrics"; }
        }

        public IEnumerable<Pair<string, decimal>> GetPrices()
        {
            string content;
            try
            {
                content = EveClient.HttpWebService.DownloadString(
                    "http://www.eve-metrics.com/api/item.xml?type_ids=34,37,35,40,36,11399,38,39&key=BF1AE7C24D0A0EE10AC94");
            }
            catch (HttpWebServiceException ex)
            {
                ExceptionHandler.LogException(ex, true);
                throw new MineralParserException(ex.Message);
            }

            //scan for prices
            MatchCollection mc = mineralTokenizer.Matches(content);

            foreach (Match mineral in mc)
            {
                string name = mineral.Groups["name"].Value;

                Decimal price = Decimal.Parse(mineral.Groups["average"].Value, NumberStyles.Currency, CultureInfo.InvariantCulture);
                yield return new Pair<string, Decimal>(name, price);
            }
        }
        #endregion
    }
}
