using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using EVEMon.SDEToSQL.Utils;

namespace EVEMon.SDEToSQL.Providers
{
    internal abstract class DbConnectionProvider
    {
        protected string Text;

        internal DbConnection Connection { get; private set; }

        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nameOrConnectionString">The name or connection string.</param>
        /// <exception cref="System.ArgumentNullException">nameOrConnectionString</exception>
        protected void CreateConnection<T>(String nameOrConnectionString) where T : DbConnection
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
                var match = Regex.Match(connectionString, "data source=(?<filePath>.*\\.[a-z]+)",
                    RegexOptions.IgnoreCase | RegexOptions.Compiled).Groups;

                if (match.Count != 2 || !File.Exists(match["filePath"].Value))
                {
                    if (Console.CursorLeft > 0)
                        Util.SetConsoleCursorPosition(Text);

                    Console.WriteLine(@"Database {0}file does not exists!",
                        match.Count != 2
                            ? String.Empty
                            : String.Format(CultureInfo.InvariantCulture,
                            "{0} ", Path.GetFileName(match["filePath"].Value)));

                    Console.WriteLine();
                    Console.WriteLine();

                    return;
                }
            }

            if (String.IsNullOrWhiteSpace(connectionString))
            {
                Util.SetConsoleCursorPosition(Text);
                Console.WriteLine(@"Can not find connection string: {0}", nameOrConnectionString);
                Util.PressAnyKey(-1);
            }

            ConstructorInfo ci = typeof(T).GetConstructor(new[]
            {
                typeof(string)
            });

            if (ci == null)
                return;

            Connection = (T)ci.Invoke(new object[]
            {
                connectionString
            });
        }

        /// <summary>
        /// Opens the connection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal void OpenConnection()
        {
            if (Connection == null)
                return;

            string databaseTypeName = Connection is SqlConnection ? "SQL Server" : "SQLite";

            Text = String.Format(CultureInfo.InvariantCulture, "Connecting to {0} '{1}' Database... ",
                databaseTypeName, Connection.Database);
            Console.Write(Text);

            try
            {
                Connection.Open();

                Util.SetConsoleCursorPosition(Text);
                Console.WriteLine(@"Connection to {0} '{1}' Database: Successful", databaseTypeName, Connection.Database);
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                string text = String.Format(CultureInfo.InvariantCulture, "Connection to {0} '{1}' Database: Failed",
                    databaseTypeName, Connection.Database);
                Util.HandleExceptionWithReason(Text, text, ex.Message);
            }
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        internal void CloseConnection()
        {
            if (Connection == null || Connection.State == ConnectionState.Closed)
                return;

            Console.WriteLine();

            string databaseTypeName = Connection is SqlConnection ? "SQL Server" : "SQLite";

            Text = String.Format(CultureInfo.InvariantCulture, "Disconnecting from {0} '{1}' Database... ",
                databaseTypeName, Connection.Database);
            Console.Write(Text);

            try
            {
                Connection.Close();

                Util.SetConsoleCursorPosition(Text);
                Console.WriteLine(@"Disconnection from {0} '{1}' Database: Successful", databaseTypeName,
                    Connection.Database);
            }
            catch (Exception ex)
            {
                string text = String.Format(CultureInfo.InvariantCulture, "Disconnection from {0} '{1}' Database: Failed",
                    databaseTypeName, Connection.Database);
                Util.HandleExceptionWithReason(Text, text, ex.Message);
            }
            finally
            {
                Connection = null;
            }
        }
    }
}
