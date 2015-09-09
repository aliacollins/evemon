using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using EVEMon.SDEToSQL.SQLiteToSQL.Models;
using EVEMon.SDEToSQL.YamlToSQL.Tables;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using YamlDotNet.RepresentationModel;

namespace EVEMon.SDEToSQL
{
    internal class Database
    {
        private static string s_text;


        /// <summary>
        /// Gets or sets the server restore.
        /// </summary>
        /// <value>
        /// The server restore.
        /// </value>
        internal static Restore ServerRestore { get; private set; }

        /// <summary>
        /// Gets or sets the SQL connection.
        /// </summary>
        /// <value>
        /// The SQL connection.
        /// </value>
        internal static SqlConnection SqlConnection { get; private set; }

        /// <summary>
        /// Gets or sets the sqlite connection.
        /// </summary>
        /// <value>
        /// The sqlite connection.
        /// </value>
        internal static SQLiteConnection SqliteConnection { get; private set; }


        /// <summary>
        /// Imports the SDE files.
        /// </summary>
        /// <param name="args">The arguments.</param>
        internal static void ImportSDEFiles(IList<string> args)
        {
            if (!args.Any() || args.All(x => x != "-norestore"))
                RestoreSqlServerDataDump();

            if (!args.Any() || args.All(x => x != "-noyaml"))
                ImportYamlFiles();

            if (!args.Any() || args.All(x => x != "-nosqlite"))
                ImportSQLiteFiles();

            if (SqlConnection != null)
            {
                Disconnect(SqlConnection);
                Console.WriteLine();

                if (SqliteConnection == null)
                    Console.WriteLine();
            }
        }

        /// <summary>
        /// Imports the sqlite files.
        /// </summary>
        private static void ImportSQLiteFiles()
        {
            if (Program.IsClosing)
                return;

            if (SqlConnection == null)
                SqlConnection = CreateConnection<SqlConnection>("name=EveStaticData");

            if (SqlConnection == null)
                return;

            Console.WriteLine();

            string connectionString = @"data source=SDEFiles\universeDataDx.db";
            SqliteConnection = CreateConnection<SQLiteConnection>(connectionString);

            if (SqliteConnection == null)
                return;

            using (var universeDataContext = new UniverseData(SqliteConnection))
            {
                if (Debugger.IsAttached)
                    Import(universeDataContext.mapRegions);
                else
                {
                    typeof(UniverseData).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                        .ToList()
                        .ForEach(property => Import(property.GetValue(universeDataContext, null) as IQueryable<object>));
                }

                if (SqliteConnection == null)
                    return;

                Disconnect(SqliteConnection);
            }
        }

        /// <summary>
        /// Imports the yaml files.
        /// </summary>
        private static void ImportYamlFiles()
        {
            if (Program.IsClosing)
                return;

            SqlConnection = CreateConnection<SqlConnection>("name=EveStaticData");

            if (SqlConnection == null)
                return;

            //Categories.Import();

            //if (Debugger.IsAttached)
            //    return;

            //Groups.Import();
            //Graphics.Import();
            //Icons.Import();
            //Skins.Import();
            //SkinMaterials.Import();
            //SkinLicenses.Import();
            Types.Import();
            //Certificates.Import();
            //Blueprints.Import();
        }

        /// <summary>
        /// Restores the SQL server data dump.
        /// </summary>
        private static void RestoreSqlServerDataDump()
        {
            if (Program.IsClosing)
                return;

            Stopwatch stopwatch = Stopwatch.StartNew();
            Util.ResetCounters();

            string filePath = Util.CheckDataDumpExists().Single();
            DbConnection connection = GetConnection<SqlConnection>("name=EveStaticData");

            s_text = String.Format(@"Restoring data dump to '{0}' Database... ", connection.Database);
            Console.Write(s_text);

            try
            {
                ServerConnection serverConnection = new ServerConnection(connection.DataSource);
                Server server = new Server(serverConnection);

                string defaultDataPath = String.IsNullOrEmpty(server.Settings.DefaultFile)
                    ? server.MasterDBPath
                    : server.Settings.DefaultFile;
                string defaultLogPath = String.IsNullOrEmpty(server.Settings.DefaultLog)
                    ? server.MasterDBLogPath
                    : server.Settings.DefaultLog;

                Restore restore = new Restore
                {
                    Database = connection.Database,
                    ReplaceDatabase = true,
                    PercentCompleteNotification = 1,
                };
                restore.PercentComplete += Restore_PercentComplete;
                restore.Devices.AddDevice(filePath, DeviceType.File);
                restore.RelocateFiles.AddRange(
                    new[]
                    {
                        new RelocateFile("ebs_DATADUMP", String.Format("{0}{1}.mdf", defaultDataPath, restore.Database)),
                        new RelocateFile("ebs_DATADUMP_log", String.Format("{0}{1}_log.ldf", defaultLogPath, restore.Database))
                    });

                ServerRestore = restore;
                restore.SqlRestore(server);

                Util.DisplayEndTime(stopwatch);
                Console.WriteLine();
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                string text = String.Format("Restoring data dump to '{0}' Database: Failed\n{1}", connection.Database,
                    ex.Message);
                Util.HandleExceptionWithReason(s_text, text, ex.InnerException.Message);
            }
        }

        /// <summary>
        /// Handles the PercentComplete event of the Restore control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PercentCompleteEventArgs"/> instance containing the event data.</param>
        private static void Restore_PercentComplete(object sender, PercentCompleteEventArgs e)
        {
            Util.UpdatePercentDone(100);
        }

        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nameOrConnectionString">The name or connection string.</param>
        /// <returns></returns>
        private static T CreateConnection<T>(String nameOrConnectionString) where T : DbConnection
        {
            DbConnection connection = GetConnection<T>(nameOrConnectionString);

            if (connection == null)
                return null;

            string databaseTypeName = connection is SqlConnection ? "SQL Server" : "SQLite";

            s_text = String.Format("Connecting to {0} '{1}' Database... ", databaseTypeName, connection.Database);
            Console.Write(s_text);

            try
            {
                connection.Open();

                Console.SetCursorPosition(Console.CursorLeft - s_text.Length, Console.CursorTop);
                Console.WriteLine(@"Connection to {0} '{1}' Database: Successful", databaseTypeName, connection.Database);
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                string text = String.Format("Connection to {0} '{1}' Database: Failed", databaseTypeName, connection.Database);
                Util.HandleExceptionWithReason(s_text, text, ex.Message);
            }

            return (T)connection;
        }

        /// <summary>
        /// Disconnects the database.
        /// </summary>
        /// <param name="connection">The connection.</param>
        internal static void Disconnect(DbConnection connection)
        {
            Console.WriteLine();

            string databaseTypeName = connection is SqlConnection ? "SQL Server" : "SQLite";

            s_text = String.Format("Disconnecting from {0} '{1}' Database... ", databaseTypeName, connection.Database);
            Console.Write(s_text);

            try
            {
                connection.Close();

                Console.SetCursorPosition(Console.CursorLeft - s_text.Length, Console.CursorTop);
                Console.WriteLine(@"Disconnection from {0} '{1}' Database: Successful", databaseTypeName,
                    connection.Database);
            }
            catch (Exception ex)
            {
                string text = String.Format("Disconnection from {0} '{1}' Database: Failed", databaseTypeName,
                    connection.Database);
                Util.HandleExceptionWithReason(s_text, text, ex.Message);
            }
            finally
            {
                connection.Dispose();

                if (connection is SQLiteConnection)
                    SqliteConnection = null;
                else
                    SqlConnection = null;
            }
        }

        /// <summary>
        /// Gets the database connection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nameOrConnectionString">The name or connection string.</param>
        /// <returns></returns>
        private static DbConnection GetConnection<T>(String nameOrConnectionString) where T : DbConnection
        {
            if (String.IsNullOrWhiteSpace(nameOrConnectionString))
                throw new ArgumentNullException("nameOrConnectionString");

            if (nameOrConnectionString.StartsWith("name=", StringComparison.OrdinalIgnoreCase))
                nameOrConnectionString = nameOrConnectionString.Replace("name=", String.Empty);

            ConnectionStringSettings connectionStringSetting = ConfigurationManager.ConnectionStrings[nameOrConnectionString];

            string connectionString = connectionStringSetting == null
                ? nameOrConnectionString
                : connectionStringSetting.ConnectionString;

            if (typeof(T) != typeof(SqlConnection))
            {
                var match = Regex.Match(connectionString, "data source=(.*)",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase).Groups;

                if (match.Count != 2 || !File.Exists(match[1].Value.TrimEnd(new[] { '\"' })))
                {
                    if (Console.CursorLeft > 0)
                    {
                        var position = Console.CursorLeft - s_text.Length;
                        Console.SetCursorPosition(position > -1 ? position : 0, Console.CursorTop);
                    }

                    Console.WriteLine(@"Database {0}file does not exists!",
                        match.Count != 2
                            ? String.Empty
                            : String.Format("{0} ", match[1].Value.TrimEnd(new[] { '\"' }).Replace("SDEFiles\\", String.Empty)));

                    Console.WriteLine();
                    Console.WriteLine();

                    return null;
                }
            }

            if (String.IsNullOrWhiteSpace(connectionString))
            {
                Console.SetCursorPosition(Console.CursorLeft - s_text.Length, Console.CursorTop);
                Console.WriteLine(@"Can not find connection string: {0}", nameOrConnectionString);
                Util.PressAnyKey(-1);
            }

            ConstructorInfo ci = typeof(T).GetConstructor(new[]
            {
                typeof(string)
            });

            if (ci != null)
            {
                return (T)ci.Invoke(new object[]
                {
                    connectionString
                });
            }

            return null;
        }

        /// <summary>
        /// SQL insert command text.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        internal static string SqlInsertCommandText(String tableName, IDictionary<string, string> parameters)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            foreach (KeyValuePair<string, string> parameter in parameters)
            {
                sb.Append(parameter.Key);
                if (!parameter.Equals(parameters.Last()))
                    sb.Append(", ");
            }
            sb.Append(") VALUES (");

            foreach (KeyValuePair<string, string> parameter in parameters)
            {
                sb.Append(parameter.Value);
                sb.Append(!parameter.Equals(parameters.Last()) ? ", " : ")");
            }

            return String.Format("INSERT INTO {0} {1}", tableName, sb);
        }

        /// <summary>
        /// SQL update command text.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        internal static string SqlUpdateCommandText(String tableName, IDictionary<string, string> parameters)
        {
            StringBuilder sb = new StringBuilder();
            Dictionary<string, string> pars = parameters
                .Where(par =>
                    !par.Key.StartsWith("columnFilter", StringComparison.OrdinalIgnoreCase) &&
                    !par.Key.StartsWith("id", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            foreach (var parameter in pars)
            {
                sb.AppendFormat("{0} = {1}", parameter.Key, parameter.Value);
                if (!parameter.Equals(pars.Last()))
                    sb.Append(", ");
            }

            if (!String.IsNullOrWhiteSpace(parameters["columnFilter1"]) && !String.IsNullOrWhiteSpace(parameters["id1"]))
            {
                for (int i = 1; i < 5; i++)
                {
                    string filterName = "columnFilter" + i;
                    string idName = "id" + i;

                    if (parameters.ContainsKey(filterName) &&
                        parameters.ContainsKey(idName) &&
                        !String.IsNullOrWhiteSpace(parameters[filterName]) &&
                        !String.IsNullOrWhiteSpace(parameters[idName]))
                    {
                        sb.AppendFormat("{0}{1} = {2}", i == 1 ? " WHERE " : " AND ", parameters[filterName], parameters[idName]);
                    }
                }
            }

            return String.Format("UPDATE {0} SET {1}", tableName, sb);
        }

        /// <summary>
        /// SQL delete command text.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        internal static string SqlDeleteCommandText(String tableName, IDictionary<string, string> parameters)
        {
            StringBuilder sb = new StringBuilder();
            
            if (!String.IsNullOrWhiteSpace(parameters["columnFilter1"]) && !String.IsNullOrWhiteSpace(parameters["id1"]))
            {
                for (int i = 1; i < 5; i++)
                {
                    string filterName = "columnFilter" + i;
                    string idName = "id" + i;

                    if (parameters.ContainsKey(filterName) &&
                        parameters.ContainsKey(idName) &&
                        !String.IsNullOrWhiteSpace(parameters[filterName]) &&
                        !String.IsNullOrWhiteSpace(parameters[idName]))
                    {
                        sb.AppendFormat("{0}{1} = {2}", i == 1 ? " WHERE " : " AND ", parameters[filterName], parameters[idName]);
                    }
                }
            }

            return String.Format("DELETE {0}{1}", tableName, sb);
        }

        /// <summary>
        /// Creates the table.
        /// </summary>
        /// <param name="rNode">The r node.</param>
        /// <param name="searchKey">The search key.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columns">The columns.</param>
        /// <returns>
        ///   <c>true</c> if the table is empty; otherwise, <c>false</c>.
        /// </returns>
        internal static bool CreateTableOrColumns(YamlMappingNode rNode, string searchKey, string tableName,
            IDictionary<string, string> columns)
        {
            if (DropAndCreateTable(rNode, searchKey, tableName))
                return true;

            CreateColumns(tableName, columns);
            return false;
        }

        /// <summary>
        /// Drops and Creates the specified table.
        /// </summary>
        /// <param name="rNode">The r node.</param>
        /// <param name="searchKey">The search key.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        internal static bool DropAndCreateTable(YamlMappingNode rNode, string searchKey, string tableName)
        {
            if (!rNode.Children.Select(pair => pair.Value)
                .OfType<YamlMappingNode>()
                .Select(cNode => cNode.Any(x => x.Key.ToString() == searchKey))
                .Any(createTable => createTable))
            {
                return false;
            }

            DropAndCreateTable(tableName);
            return true;
        }

        /// <summary>
        /// Drops and Creates the specified table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        internal static void DropAndCreateTable(String tableName)
        {
            if (Program.IsClosing)
                return;

            if (SqlConnection == null)
                return;

            using (IDbCommand command = new SqlCommand(
                Util.GetScriptFor(tableName),
                SqlConnection,
                SqlConnection.BeginTransaction()))
            {
                try
                {
                    command.ExecuteNonQuery();
                    command.Transaction.Commit();
                }
                catch (SqlException e)
                {
                    Util.HandleExceptionForCommand(command, e);

                    if (command.Transaction != null)
                        command.Transaction.Rollback();
                }
            }
        }

        /// <summary>
        /// Creates the columns.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columns">The columns.</param>
        private static void CreateColumns(String tableName, IDictionary<string, string> columns)
        {
            if (Program.IsClosing)
                return;

            if (SqlConnection == null)
                return;

            if (SqlConnection.GetSchema("columns").Select(String.Format("TABLE_NAME = '{0}'", tableName)).Length == 0)
            {
                Console.WriteLine(@"Can't find table '{0}'.", tableName);
                Util.PressAnyKey(-1);
            }

            foreach (KeyValuePair<string, string> column in columns)
            {
                CreateColumn(tableName, column.Key, column.Value);
            }
        }

        /// <summary>
        /// Creates the column.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="columnType">Type of the column.</param>
        /// <param name="defaultValue">The default value.</param>
        private static void CreateColumn(String tableName, String columnName, String columnType, string defaultValue = "Null")
        {
            if (Program.IsClosing)
                return;

            if (SqlConnection == null)
                return;

            if (SqlConnection.GetSchema("columns")
                .Select(String.Format("TABLE_NAME = '{0}' AND COLUMN_NAME = '{1}'", tableName, columnName))
                .Length != 0)
            {
                return;
            }

            double number;
            string commandText = String.Format("ALTER TABLE {0} ADD {1} {2} {3} NULL {4}", tableName, columnName, columnType,
                defaultValue != SqlString.Null.ToString() ? "NOT" : String.Empty,
                defaultValue != SqlString.Null.ToString()
                    ? String.Format("DEFAULT ({0})",
                        Double.TryParse(defaultValue, out number)
                            ? String.Format("({0})", defaultValue)
                            : String.Format("'{0}'", defaultValue)
                        )
                    : String.Empty);

            using (IDbCommand command = new SqlCommand(
                commandText,
                SqlConnection,
                SqlConnection.BeginTransaction()))
            {
                try
                {
                    command.ExecuteNonQuery();
                    command.Transaction.Commit();
                }
                catch (SqlException e)
                {
                    Util.HandleExceptionForCommand(command, e);

                    if (command.Transaction != null)
                        command.Transaction.Rollback();
                }
            }
        }

        /// <summary>
        /// Imports the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <exception cref="System.ArgumentNullException">data</exception>
        private static void Import(IQueryable<object> data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (SqliteConnection == null)
                return;

            string tableName = data.GetType().GetGenericArguments().First().Name;
            Stopwatch stopwatch = Stopwatch.StartNew();
            Util.ResetCounters();

            Console.Write(@"Importing {0}... ", tableName);

            DropAndCreateTable(tableName);

            DataTable table = data.ToDataTable();

            if (SqliteConnection == null)
                return;

            ImportDataBulk(tableName, table);

            Util.UpdatePercentDone(table.Rows.Count);

            Util.DisplayEndTime(stopwatch);

            Console.WriteLine();
        }

        /// <summary>
        /// Imports the data bulk.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="data">The data.</param>
        internal static void ImportDataBulk(string tableName, DataTable data)
        {
            if (SqlConnection == null)
                return;

            using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(
                SqlConnection.ConnectionString,
                SqlBulkCopyOptions.UseInternalTransaction))
            {
                sqlBulkCopy.DestinationTableName = tableName;

                try
                {
                    sqlBulkCopy.WriteToServer(data);
                }
                catch (Exception e)
                {
                    string text = String.Format(@"Unable to import {0}", tableName);
                    Util.HandleExceptionWithReason(s_text, text, e.Message);
                }
            }
        }
    }
}
