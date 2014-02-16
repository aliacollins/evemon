using System;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Globalization;
using System.Reflection;
using System.Threading;

namespace EVEMon.SQLiteToSql
{
    internal static class Program
    {
        private static void Main()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            //SQLiteConnection sqLiteConnection = Database.Connect<SQLiteConnection>("UniverseData");
            //Database.Disconnect();

            EntityConnection connection = Database.Connect<EntityConnection>("UniverseDataEntities");
            SqlConnection sqlConnection = Database.Connect<SqlConnection>("EveStaticData");

            Database.Disconnect(connection);
            Database.Disconnect(sqlConnection);

            Console.WriteLine();
            Console.Write(@"Press any key to exit.");
            Console.ReadLine();
        }
    }

    internal static class Database
    {
        private static string s_text;

        /// <summary>
        /// Connects the database.
        /// </summary>
        /// <param name="connectionName">Name of the connection.</param>
        /// <returns></returns>
        internal static T Connect<T>(string connectionName) where T : DbConnection
        {
            s_text = "Connecting to Database... ";
            Console.Write(s_text);

            DbConnection connection = GetConnection<T>(connectionName);

            string databaseTypeName = connection is SqlConnection ? "SQL" : "SQLite";
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
        private static DbConnection GetConnection<T>(string connectionName) where T : DbConnection
        {
            ConnectionStringSettings connectionStringSetting = ConfigurationManager.ConnectionStrings[connectionName];
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
    }
}
