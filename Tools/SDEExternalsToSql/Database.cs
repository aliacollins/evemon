using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using EVEMon.SDEExternalsToSql.SQLiteToSql;

namespace EVEMon.SDEExternalsToSql
{
    internal static class Database
    {
        private static string s_text;
        internal const string StringEmpty = "''";
        internal const string Null = "NULL";

        /// <summary>
        /// Gets or sets the SQL connection.
        /// </summary>
        /// <value>
        /// The SQL connection.
        /// </value>
        internal static SqlConnection SqlConnection { get; set; }

        /// <summary>
        /// Gets or sets the sqlite connection.
        /// </summary>
        /// <value>
        /// The sqlite connection.
        /// </value>
        internal static EntityConnection SqliteConnection { get; set; }


        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        internal static UniverseDataEntities UniverseDataContext { get; set; }

        /// <summary>
        /// Connects the database.
        /// </summary>
        /// <param name="connectionName">Name of the connection.</param>
        /// <returns></returns>
        internal static T Connect<T>(String connectionName) where T : DbConnection
        {
            String databaseTypeName = typeof(T) == typeof(SqlConnection) ? "SQL" : "SQLite";
            s_text = String.Format("Connecting to {0} Database... ", databaseTypeName);
            Console.Write(s_text);

            DbConnection connection = GetConnection<T>(connectionName);

            if (connection == null)
                return null;

            try
            {
                connection.Open();

                Console.SetCursorPosition(Console.CursorLeft - s_text.Length, Console.CursorTop);
                Console.WriteLine(@"Connection to {0} Database: Successful", databaseTypeName);
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.SetCursorPosition(Console.CursorLeft - s_text.Length, Console.CursorTop);
                Console.WriteLine(@"Connection to {0} Database: Failed", databaseTypeName);
                Console.WriteLine(@"Reason was: {0}", ex.Message);
                Console.Write(@"Press any key to exit.");
                Console.ReadLine();
                Environment.Exit(-1);
            }

            return (T)connection;
        }

        /// <summary>
        /// Disconnects the database.
        /// </summary>
        internal static void Disconnect(DbConnection connection)
        {
            Console.WriteLine();

            string databaseTypeName = connection is SqlConnection ? "SQL" : "SQLite";

            try
            {
                connection.Close();

                Console.WriteLine(@"Disconnection from {0} Database: Successful", databaseTypeName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"Disconnection from {0} Database: Failed", databaseTypeName);
                Console.WriteLine(@"Reason was: {0}", ex.Message);
                Console.Write(@"Press any key to exit.");
                Console.ReadLine();
                Environment.Exit(-1);
            }
        }

        /// <summary>
        /// Gets the SQL connection.
        /// </summary>
        /// <param name="connectionName">Name of the connection.</param>
        /// <returns></returns>
        private static DbConnection GetConnection<T>(String connectionName) where T : DbConnection
        {
            ConnectionStringSettings connectionStringSetting = ConfigurationManager.ConnectionStrings[connectionName];

            if (typeof(T) != typeof(SqlConnection))
            {
                var match = Regex.Match(connectionStringSetting.ConnectionString, "data source=(.*)",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase).Groups;

                if (match.Count != 2 || !File.Exists(match[1].Value.TrimEnd(new[] { '\"' })))
                {
                    Console.SetCursorPosition(Console.CursorLeft - s_text.Length, Console.CursorTop);
                    Console.WriteLine(@"Database {0}file does not exists!",
                        match.Count != 2
                            ? String.Empty
                            : String.Format("{0} ", match[1].Value.TrimEnd(new[] { '\"' }).Replace("SQLiteFiles\\", String.Empty)));
                    return null;
                }
            }

            if (connectionStringSetting != null)
            {
                ConstructorInfo ci = typeof(T).GetConstructor(new[]
                {
                    typeof(string)
                });

                if (ci != null)
                {
                    return (T)ci.Invoke(new object[]
                    {
                        connectionStringSetting.ConnectionString
                    });
                }
            }

            Console.SetCursorPosition(Console.CursorLeft - s_text.Length, Console.CursorTop);
            Console.WriteLine(@"Can not find connection string with name: {0}", connectionName);
            Console.Write(@"Press any key to exit.");
            Console.ReadLine();
            Environment.Exit(-1);
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
            Dictionary<string, string> pars = parameters.Where(par => par.Key != "columnFilter" && par.Key != "id")
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            foreach (var parameter in pars)
            {
                sb.AppendFormat("{0} = {1}", parameter.Key, parameter.Value);
                if (!parameter.Equals(pars.Last()))
                    sb.Append(", ");
            }

            if (!String.IsNullOrWhiteSpace(parameters["columnFilter"]) && !String.IsNullOrWhiteSpace(parameters["id"]))
                sb.AppendFormat(" WHERE {0} = {1}", parameters["columnFilter"], parameters["id"]);

            return String.Format("UPDATE {0} SET {1}", tableName, sb);
        }

        /// <summary>
        /// Drops the table.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="tableName">Name of the table.</param>
        private static void DropTable(IDbCommand command, string tableName)
        {
            using (var tx = SqlConnection.BeginTransaction())
            {
                command.Transaction = tx;
                command.CommandText = String.Format("DROP TABLE {0}", tableName);

                try
                {
                    command.ExecuteNonQuery();
                    tx.Commit();
                }
                catch (SqlException e)
                {
                    tx.Rollback();
                    Console.WriteLine();
                    Console.WriteLine(@"Unable to execute SQL command: {0}", command.CommandText);
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                    Environment.Exit(-1);
                }
            }
        }

        /// <summary>
        /// Creates the table.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="tableName">Name of the table.</param>
        private static void CreateTable(IDbCommand command, String tableName)
        {
            using (var tx = SqlConnection.BeginTransaction())
            {
                command.Transaction = tx;
                command.CommandText = Util.GetScriptFor(tableName);

                try
                {
                    command.ExecuteNonQuery();
                    tx.Commit();
                }
                catch (SqlException e)
                {
                    tx.Rollback();
                    Console.WriteLine();
                    Console.WriteLine(@"Unable to execute SQL command: {0}", command.CommandText);
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                    Environment.Exit(-1);
                }
            }
        }

        /// <summary>
        /// Creates the table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        internal static void CreateTable(String tableName)
        {
            var command = new SqlCommand { Connection = SqlConnection };
            DataTable dataTable = SqlConnection.GetSchema("columns");

            if (dataTable.Select(String.Format("TABLE_NAME = '{0}'", tableName)).Length == 0)
                CreateTable(command, tableName);
            else
            {
                DropTable(command, tableName);
                CreateTable(command, tableName);
            }
        }

        /// <summary>
        /// Creates the column.
        /// </summary>
        /// <param name="dataTable">The data table.</param>
        /// <param name="command">The command.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="columnType">Type of the column.</param>
        private static void CreateColumn(DataTable dataTable, IDbCommand command,
            String tableName, String columnName, String columnType)
        {
            if (dataTable.Select(String.Format("COLUMN_NAME = '{0}' AND TABLE_NAME = '{1}'", columnName, tableName)).Length != 0)
                return;

            using (var tx = SqlConnection.BeginTransaction())
            {
                command.Transaction = tx;
                command.CommandText = String.Format("ALTER TABLE {0} ADD {1} {2} null", tableName, columnName, columnType);

                try
                {
                    command.ExecuteNonQuery();
                    tx.Commit();
                }
                catch (SqlException e)
                {
                    tx.Rollback();
                    Console.WriteLine();
                    Console.WriteLine(@"Unable to execute SQL command: {0}", command.CommandText);
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                    Environment.Exit(-1);
                }
            }
        }

        /// <summary>
        /// Creates the columns.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columns">The columns.</param>
        internal static void CreateColumns(String tableName, IEnumerable<KeyValuePair<string, string>> columns)
        {
            var command = new SqlCommand { Connection = SqlConnection };
            DataTable dataTable = SqlConnection.GetSchema("columns");

            if (dataTable.Select(String.Format("TABLE_NAME = '{0}'", tableName)).Length == 0)
            {
                Console.WriteLine(@"Can't find table '{0}'.", tableName);
                Console.ReadLine();
                Environment.Exit(-1);
            }

            foreach (KeyValuePair<string, string> column in columns)
            {
                CreateColumn(dataTable, command, tableName, column.Key, column.Value);
            }
        }
    }
}
