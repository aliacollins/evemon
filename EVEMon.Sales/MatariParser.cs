using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using EVEMon.Common;
using EVEMon.Common.Net;

namespace EVEMon.Sales
{
    [DefaultMineralParser("matari")]
    public class MatariParser : IMineralParser
    {
        private static Regex mineralLineScan = new Regex(@"\<table.*last.updated", RegexOptions.Compiled);

        private static Regex mineralTokenizer =
            new Regex(@"\<td.*?\>(?<name>\w*)\</td\>\<td.*?align.*?\>(?<price>(\d|\.|,)*)\</td\>\<td",
                      RegexOptions.Compiled);

        #region IMineralParser Members
        public string Title
        {
            get { return "Matari Mineral Index"; }
        }

        public string CourtesyUrl
        {
            get { return "http://www.evegeek.com/"; }
        }

        public string CourtesyText
        {
            get { return "EVE[geek]"; }
        }

        public IEnumerable<Pair<string, decimal>> GetPrices()
        {
            string phoenixContent;
            try
            {
                phoenixContent = Singleton.Instance<EVEMonWebClient>().DownloadString(
                    "http://www.evegeek.com/mineralindex.php");
            }
            catch (EVEMonWebException ex)
            {
                ExceptionHandler.LogException(ex, true);
                throw new MineralParserException(ex.Message);
            }

            //scan for prices
            Match m = mineralLineScan.Match(phoenixContent);

            string mLine = m.Captures[0].Value;

            MatchCollection mc = mineralTokenizer.Matches(mLine);

            foreach (Match mineral in mc)
            {
                string name = mineral.Groups["name"].Value;

                Decimal price = Decimal.Parse(mineral.Groups["price"].Value, NumberStyles.Currency, CultureInfo.InvariantCulture);
                yield return new Pair<string, Decimal>(name, price);
            }
        }
        #endregion
    }
}
