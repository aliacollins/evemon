using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using EVEMon.SDEToSQL.Importers;
using EVEMon.SDEToSQL.Importers.DataDumpToSQL;
using EVEMon.SDEToSQL.Importers.SQLiteToSQL;
using EVEMon.SDEToSQL.Importers.YamlToSQL;
using EVEMon.SDEToSQL.Providers;
using EVEMon.SDEToSQL.Utils;
using Microsoft.SqlServer.Management.Smo;

namespace EVEMon.SDEToSQL
{
    internal static class Importer
    {
        private static readonly SafeNativeMethods.EventHandler s_handler;
        private static readonly SqlConnectionProvider s_sqlConnectionProvider;
        private static readonly SqliteConnectionProvider s_sqliteConnectionProvider;
        private static readonly DataDumpImporter s_dataDumpImporter;

        private static bool s_isClosing;

        /// <summary>
        /// Initializes the <see cref="Importer"/> class.
        /// </summary>
        static Importer()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            Util.Closing += Util_Closing;
            s_handler += CtrlHandler;
            SafeNativeMethods.SetConsoleCtrlHandler(s_handler, add: true);
            
            string assemblyDirectory = Path.GetDirectoryName(typeof(Program).Assembly.Location) ?? Directory.GetCurrentDirectory();

            if (Directory.GetCurrentDirectory() != assemblyDirectory)
                Directory.SetCurrentDirectory(assemblyDirectory);
            
            string connectionString = String.Format(CultureInfo.InvariantCulture, "data source={0}",
                Path.Combine(Directory.GetCurrentDirectory(), @"SDEFiles\universeDataDx.db"));

            s_sqliteConnectionProvider = new SqliteConnectionProvider(connectionString);
            s_sqlConnectionProvider = new SqlConnectionProvider("name=EveStaticData");
            s_dataDumpImporter = new DataDumpImporter(s_sqlConnectionProvider, new Restore());
        }

        /// <summary>
        /// Imports the SDE with the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        internal static void Import(string[] args)
        {
            using (StreamWriter traceStream = File.CreateText("trace.txt"))
            {
                TextWriterTraceListener traceListener = new TextWriterTraceListener(traceStream);
                Trace.Listeners.Add(traceListener);
                Trace.AutoFlush = true;

                StartImporter(args);
            }
        }

        /// <summary>
        /// Starts the importer with the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <exception cref="System.Exception">test</exception>
        private static void StartImporter(string[] args)
        {
            Trace.WriteLine("SDEToSql.Starting");

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
                ImportSDEFiles(args);
            }
            catch (Exception ex)
            {
                Trace.Write(ex.GetRecursiveStackTrace());

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine(@"An unhandled exception was thrown.");
                Console.WriteLine(@"For more info refer to the 'trace.txt' file.");
                Util.PressAnyKey(-1);
            }

            if (args.Any(x => x == "-norestore") && args.Any(x => x == "-noyaml") && args.Any(x => x == "-nosqlite"))
                return;

            if (!s_isClosing)
            {
                Console.WriteLine(String.Format(CultureInfo.InvariantCulture, "Importing files completed in {0:g}",
                    stopwatch.Elapsed));
            }

            if (s_isClosing)
                return;

            Util.PressAnyKey();
        }

        /// <summary>
        /// Imports the sde files.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void ImportSDEFiles(string[] args)
        {
            if (!args.Any() || args.All(x => x != "-norestore"))
            {
                s_dataDumpImporter.ImportFiles();
            }

            if (!args.Any() || args.All(x => x != "-noyaml"))
            {
                if (s_isClosing)
                    return;

                s_sqlConnectionProvider.OpenConnection();
                IImporter yamlImporter = new YamlImporter(s_sqlConnectionProvider);
                yamlImporter.ImportFiles();
            }

            if (!args.Any() || args.All(x => x != "-nosqlite"))
            {
                if (s_isClosing)
                    return;

                if (s_sqlConnectionProvider.Connection.State == ConnectionState.Closed)
                    s_sqlConnectionProvider.OpenConnection();

                Console.WriteLine();

                s_sqliteConnectionProvider.OpenConnection();

                IImporter sqliteImporter = new SqliteImporter(s_sqliteConnectionProvider, s_sqlConnectionProvider);
                sqliteImporter.ImportFiles();
            }

            if (s_sqlConnectionProvider.Connection == null ||
                s_sqlConnectionProvider.Connection.State == ConnectionState.Closed)
            {
                return;
            }

            s_sqlConnectionProvider.CloseConnection();
            Console.WriteLine();

            if (s_sqliteConnectionProvider == null || s_sqliteConnectionProvider.Connection == null)
                Console.WriteLine();
        }

        /// <summary>
        /// Shows the help.
        /// </summary>
        private static void ShowHelp()
        {
            Console.WriteLine(
                @"   _____   ______  _______  __________     ________    __     ______  ______  ____  ___  _____________ ");
            Console.WriteLine(
                @"  / __/ | / / __/ / __/ _ \/ __/_  __/__  / __/ __ \  / /    /  _/  |/  / _ \/ __ \/ _ \/_  __/ __/ _ \");
            Console.WriteLine(
                @" / _/ | |/ / _/  _\ \/ // / _/  / / / _ \_\ \/ /_/ / / /__  _/ // /|_/ / ___/ /_/ / , _/ / / / _// , _/");
            Console.WriteLine(
                @"/___/ |___/___/ /___/____/___/ /_/  \___/___/\___\_\/____/ /___/_/  /_/_/   \____/_/|_| /_/ /___/_/|_| ");
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
        /// Handles the Closing event of the Program control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private static void Util_Closing(object sender, EventArgs e)
        {
            s_isClosing = true;
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
                    {
                        Util.OnClosing();
                        Console.WriteLine();

                        if (s_dataDumpImporter != null && s_dataDumpImporter.Restore != null)
                            s_dataDumpImporter.Restore.Abort();

                        if (s_sqliteConnectionProvider != null)
                            s_sqliteConnectionProvider.CloseConnection();

                        if (s_sqlConnectionProvider != null)
                            s_sqlConnectionProvider.CloseConnection();

                        Util.DeleteSDEFilesIfZipExists();
                        Environment.Exit(0);
                        break;
                    }
                default:
                    return false;
            }

            return true;
        }
    }
}
