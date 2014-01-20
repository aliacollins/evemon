using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace EVEMon.YamlToSql
{
    internal static class Database
    {
        private static string s_text = String.Empty;
        private static SqlConnection s_connection;

        /// <summary>
        /// Connects the database.
        /// </summary>
        internal static SqlConnection Connect()
        {
            s_text = "Connecting to SQL Database... ";
            Console.Write(s_text);

            s_connection = GetSqlConnection("EveStaticData");

            try
            {
                s_connection.Open();

                Console.SetCursorPosition(Console.CursorLeft - s_text.Length, Console.CursorTop);
                Console.WriteLine(@"Connection to SQL Database: Successful");
                Console.WriteLine();
            }
            catch (SqlException)
            {
                Console.SetCursorPosition(Console.CursorLeft - s_text.Length, Console.CursorTop);
                Console.WriteLine(@"Connection to SQL Database: Failed");
                Console.Write(@"Press any key to exit.");
                Console.ReadLine();
                Environment.Exit(-1);
            }

            return s_connection;
        }

        /// <summary>
        /// Disconnects the database.
        /// </summary>
        internal static void Disconnect()
        {
            Console.WriteLine();

            try
            {
                s_connection.Close();

                Console.WriteLine(@"Disconnection from SQL Database: Successful");
                Console.Write(@"Press any key to exit.");
                Console.ReadLine();
            }
            catch (SqlException)
            {
                Console.WriteLine(@"Disconnection from SQL Database: Failed");
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
        private static SqlConnection GetSqlConnection(string connectionName)
        {
            ConnectionStringSettings connectionStringSetting = ConfigurationManager.ConnectionStrings[connectionName];
            if (connectionStringSetting != null)
                return new SqlConnection(connectionStringSetting.ConnectionString);

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
        internal static string SqlInsertCommandText(string tableName, IDictionary<string, string> parameters)
        {
            var sb = new StringBuilder();
            sb.Append("(");
            foreach (var parameter in parameters)
            {
                sb.Append(parameter.Key);
                if (!parameter.Equals(parameters.Last()))
                    sb.Append(", ");
            }
            sb.Append(") VALUES (");

            foreach (var parameter in parameters)
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
        internal static string SqlUpdateCommandText(string tableName, IDictionary<string, string> parameters)
        {
            var sb = new StringBuilder();
            var pars = parameters.Where(par => par.Key != "columnFilter" && par.Key != "id")
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
        public static void DropTable(SqlCommand command, string tableName)
        {
            using (var tx = s_connection.BeginTransaction())
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
                }
            }
        }

        /// <summary>
        /// Creates the table.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="tableName">Name of the table.</param>
        internal static void CreateTable(IDbCommand command, string tableName)
        {
            var query = Util.GetScriptFor(tableName);

            using (var tx = s_connection.BeginTransaction())
            {
                command.Transaction = tx;
                command.CommandText = query;

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
                }
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
        internal static void CreateColumn(DataTable dataTable, IDbCommand command, string tableName, string columnName, string columnType)
        {
            if (dataTable.Select(String.Format("COLUMN_NAME = '{0}' AND TABLE_NAME = '{1}'", columnName, tableName)).Length != 0)
                return;

            using (var tx = s_connection.BeginTransaction())
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
                }
            }
        }
    }
}