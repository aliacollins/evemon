using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using EVEMon.Common;
using EVEMon.Common.Net;

namespace EVEMon.Sales
{
    [DefaultMineralParser("battleclinic")]
    public class BattleclinicParser : IMineralParser
    {

        private static Regex mineralTokenizer =
            new Regex(@"<name>(?<name>.+?)</name>.+?<price>(?<price>.+?)</price>",
                      RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline 
                        | RegexOptions.Multiline
                        | RegexOptions.IgnoreCase);

        #region BattleclinicParser Members
        public string Title
        {
            get { return "Battleclinic.com EVE Averages"; }
        }

        public string CourtesyUrl
        {
            get { return "http://eve.battleclinic.com/eve_online/market.php"; }
        }

        public string CourtesyText
        {
            get { return "Battleclinic.com"; }
        }

        public IEnumerable<Pair<string, decimal>> GetPrices()
        {
            string content;
            try
            {
                content = CommonContext.HttpWebService.DownloadString(
                    "http://www.battleclinic.com/eve_online/market.php?feed=xml");
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

                Decimal price = Decimal.Parse(mineral.Groups["price"].Value, NumberStyles.Currency, CultureInfo.InvariantCulture);
                yield return new Pair<string, Decimal>(name, price);
            }
        }
        #endregion
    }
}
