using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using EVEMon.Common;

namespace EVEMon.Sales
{
    /// <summary>
    /// Gets mineral data from parsers.
    /// </summary>
    public static class MineralDataRequest
    {
        private static readonly Dictionary<string, IMineralParser> s_parsers = new Dictionary<string, IMineralParser>();

        /// <summary>
        /// Gets the parsers.
        /// </summary>
        /// <value>The parsers.</value>
        public static IEnumerable<Pair<string, IMineralParser>> Parsers
        {
            get { return s_parsers.Select(kvp => new Pair<string, IMineralParser>(kvp.Key, kvp.Value)); }
        }

        /// <summary>
        /// Initializes the data source.
        /// </summary>
        internal static void Initialize()
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach (DefaultMineralParserAttribute dmpa in type.GetCustomAttributes(
                    typeof(DefaultMineralParserAttribute), false))
                {
                    IMineralParser mp = Activator.CreateInstance(type) as IMineralParser;
                    if (mp != null)
                        s_parsers.Add(dmpa.Name, mp);
                }
            }
        }

        /// <summary>
        /// The prices of mineral from the given source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>An enumerable collection of Minerals and Prices.</returns>
        public static IEnumerable<Pair<string, decimal>> Prices(string source)
        {
            if (!s_parsers.ContainsKey(source))
                throw new ArgumentException("That is not a registered mineraldatasource.", "source");

            IMineralParser parser = s_parsers[source];
            return GetPrices(parser);
        }

        /// <summary>
        /// Gets the courtesy text.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>The courtesy text.</returns>
        public static string GetCourtesyText(string source)
        {
            if (!s_parsers.ContainsKey(source))
                throw new ArgumentException("That is not a registered mineraldatasource.", "source");

            IMineralParser p = s_parsers[source];
            return p.CourtesyText;
        }

        /// <summary>
        /// Gets the courtesy URL.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>The courtesy URL.</returns>
        public static Uri GetCourtesyUrl(string source)
        {
            if (!s_parsers.ContainsKey(source))
                throw new ArgumentException("That is not a registered mineraldatasource.", "source");

            IMineralParser p = s_parsers[source];
            return p.CourtesyUrl;
        }

        /// <summary>
        /// Scans for prices.
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<Pair<string, decimal>> GetPrices(IMineralParser parser)
        {
            string content = EveMonClient.HttpWebService.DownloadString(parser.URL.AbsoluteUri);

            // Scan for prices
            MatchCollection mc = parser.Tokenizer.Matches(content);

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
                                                             NumberStyles.Currency,
                                                             CultureInfo.InvariantCulture)
                                   }).Select(
                                                      mineral =>
                                                      new Pair<string, Decimal>(mineral.mineral.name, mineral.price));
        }
    }
}