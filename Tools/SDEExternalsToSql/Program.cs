using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace EVEMon.SDEExternalsToSql
{
    internal class Program
    {
        private delegate bool EventHandler(CtrlType ctrlType);
        private static EventHandler s_handler;

        internal static bool IsClosing;

        /// <summary>
        /// The entry point.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void Main(string[] args)
        {
            s_handler += CtrlHandler;
            SetConsoleCtrlHandler(s_handler, true);

            Util.StartTraceLogging();

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            if (args.Any(x => x != "-norestore" && x != "-noyaml" && x != "-nosqlite")
                || (args.Any() && (args[0] == "-help" || args[0] == "/?")))
            {
                ShowHelp();
                Console.ReadKey(true);
                return;
            }

            Util.DeleteSDEFilesIfZipExists();
            Util.InflateZipFileIfExists(args);

            Stopwatch stopwatch = Stopwatch.StartNew();
            Util.ResetCounters();

            try
            {
                Database.ImportSDEFiles(args);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.GetRecursiveStackTrace());
                Util.PressAnyKey(-1);
            }

            if (args.Any(x => x == "-norestore") && args.Any(x => x == "-noyaml") && args.Any(x => x == "-nosqlite"))
                return;

            if (!IsClosing)
            {
                Console.WriteLine(String.Format(CultureInfo.CurrentCulture, "Importing files completed in {0}",
                    stopwatch.Elapsed.ToString("g")));
            }

            if (IsClosing)
                return;

            Util.PressAnyKey();
        }

        /// <summary>
        /// Shows the help.
        /// </summary>
        private static void ShowHelp()
        {
            Console.WriteLine(@"   _____   ______  _______  ____  ______  ______  ____  ___  _____________ ");
            Console.WriteLine(@"  / __/ | / / __/ / __/ _ \/ __/ /  _/  |/  / _ \/ __ \/ _ \/_  __/ __/ _ \");
            Console.WriteLine(@" / _/ | |/ / _/  _\ \/ // / _/  _/ // /|_/ / ___/ /_/ / , _/ / / / _// , _/");
            Console.WriteLine(@"/___/ |___/___/ /___/____/___/ /___/_/  /_/_/   \____/_/|_| /_/ /___/_/|_| ");
            Console.WriteLine(@"EVE Static Data Export To SQL Server Importer");
            Console.WriteLine(@"By Jimi ""Desmont McCallock"" C");
            Console.WriteLine();
            Console.WriteLine(@"usage: {0} [<arguments...>]", typeof(Program).Assembly.GetName().Name);
            Console.WriteLine();
            Console.WriteLine(@"arguments:");
            Console.WriteLine(@"        -help       Displays a list of available arguments");
            Console.WriteLine(@"        -norestore  Excludes the restoration of the SQL DATADUMP backup file");
            Console.WriteLine(@"        -noyaml     Excludes the importation of the yaml files");
            Console.WriteLine(@"        -nosqlite   Excludes the importation of the sqlite files");
            Util.PressAnyKey();
        }

        /// <summary>
        /// Handlers the specified control type.
        /// </summary>
        /// <param name="ctrlType">Type of the control.</param>
        /// <returns></returns>
        private static bool CtrlHandler(CtrlType ctrlType)
        {
            switch (ctrlType)
            {
                case CtrlType.CtrlCEvent:
                case CtrlType.CtrlBreakEvent:
                case CtrlType.CtrlCloseEvent:
                case CtrlType.CtrlLogoffEvent:
                case CtrlType.CtrlShutdownEvent:
                    IsClosing = true;
                    Console.WriteLine();
                    if (Database.ServerRestore != null)
                        Database.ServerRestore.Abort();

                    if (Database.SqliteConnection != null)
                        Database.Disconnect(Database.SqliteConnection);

                    if (Database.SqlConnection != null)
                        Database.Disconnect(Database.SqlConnection);

                    Util.DeleteSDEFilesIfZipExists();
                    Environment.Exit(0);
                    break;
                default:
                    return false;
            }

            return true;
        }

        private enum CtrlType
        {
            CtrlCEvent = 0,
            CtrlBreakEvent = 1,
            CtrlCloseEvent = 2,
            CtrlLogoffEvent = 5,
            CtrlShutdownEvent = 6
        }

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);
    }
}
