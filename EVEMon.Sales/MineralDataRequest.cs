using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        /// Initializes the <see cref="MineralDataRequest"/> class.
        /// </summary>
        static MineralDataRequest()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            foreach (Type type in asm.GetTypes())
            {
                foreach (DefaultMineralParserAttribute dmpa in type.GetCustomAttributes(
                    typeof (DefaultMineralParserAttribute), false))
                {
                    IMineralParser mp = Activator.CreateInstance(type) as IMineralParser;
                    if (mp != null)
                        RegisterDataSource(dmpa.Name, mp);
                }
            }
        }

        /// <summary>
        /// Gets the parsers.
        /// </summary>
        /// <value>The parsers.</value>
        public static IEnumerable<Pair<string, IMineralParser>> Parsers
        {
            get { return s_parsers.Select(kvp => new Pair<string, IMineralParser>(kvp.Key, kvp.Value)); }
        }

        /// <summary>
        /// Registers the data source.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="parser">The parser.</param>
        private static void RegisterDataSource(string name, IMineralParser parser)
        {
            s_parsers.Add(name, parser);
        }

        /// <summary>
        /// Gets the prices.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>An enumerable collection of Minerals and Prices.</returns>
        public static IEnumerable<Pair<string, Decimal>> GetPrices(string source)
        {
            if (!s_parsers.ContainsKey(source))
                throw new ArgumentException("That is not a registered mineraldatasource.", "source");

            IMineralParser p = s_parsers[source];
            return p.GetPrices();
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
        public static string GetCourtesyUrl(string source)
        {
            if (!s_parsers.ContainsKey(source))
                throw new ArgumentException("That is not a registered mineraldatasource.", "source");

            IMineralParser p = s_parsers[source];
            return p.CourtesyUrl;
        }
    }
}
