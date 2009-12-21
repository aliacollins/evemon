using System;
using System.Collections.Generic;
using System.Reflection;
using EVEMon.Common;

namespace EVEMon.Sales
{
    public class DefaultMineralParserAttribute : Attribute
    {
        private string m_name = String.Empty;

        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        public DefaultMineralParserAttribute(string name)
        {
            m_name = name;
        }
    }

    public static class MineralDataRequest
    {
        static MineralDataRequest()
        {
            sm_parsers = new Dictionary<string, IMineralParser>();

            Assembly asm = Assembly.GetExecutingAssembly();
            foreach (Type tt in asm.GetTypes())
            {
                foreach (
                    DefaultMineralParserAttribute dmpa in
                        tt.GetCustomAttributes(typeof (DefaultMineralParserAttribute), false))
                {
                    IMineralParser mp = Activator.CreateInstance(tt) as IMineralParser;
                    if (mp != null)
                    {
                        RegisterDataSource(dmpa.Name, mp);
                    }
                }
            }
        }

        private static Dictionary<string, IMineralParser> sm_parsers;

        public static IEnumerable<Pair<string, IMineralParser>> Parsers
        {
            get
            {
                foreach (KeyValuePair<string, IMineralParser> kvp in sm_parsers)
                {
                    yield return new Pair<string, IMineralParser>(kvp.Key, kvp.Value);
                }
            }
        }

        public static void RegisterDataSource(string name, IMineralParser parser)
        {
            sm_parsers.Add(name, parser);
        }

        public static IEnumerable<Pair<string, Decimal>> GetPrices(string source)
        {
            if (!sm_parsers.ContainsKey(source))
            {
                throw new ArgumentException("that is not a registered mineraldatasource", "source");
            }

            IMineralParser p = sm_parsers[source];
            return p.GetPrices();
        }

        public static string GetCourtesyText(string source)
        {
            if (!sm_parsers.ContainsKey(source))
            {
                throw new ArgumentException("that is not a registered mineraldatasource", "source");
            }

            IMineralParser p = sm_parsers[source];
            return p.CourtesyText;
        }

        public static string GetCourtesyUrl(string source)
        {
            if (!sm_parsers.ContainsKey(source))
            {
                throw new ArgumentException("that is not a registered mineraldatasource", "source");
            }

            IMineralParser p = sm_parsers[source];
            return p.CourtesyUrl;
        }
    }

    public interface IMineralParser
    {
        string Title { get; }

        string CourtesyUrl { get; }
        string CourtesyText { get; }

        IEnumerable<Pair<string, Decimal>> GetPrices();
    }

    public class MineralParserException : ApplicationException
    {
        public MineralParserException(string msg)
            : base(msg)
        {
        }
    }
}