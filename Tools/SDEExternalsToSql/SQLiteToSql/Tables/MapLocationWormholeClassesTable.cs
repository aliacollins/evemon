using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace EVEMon.SDEExternalsToSql.SQLiteToSql.Tables
{
    internal static class MapLocationWormholeClassesTable
    {
        private static int s_total;
        private const string TableName = "mapLocationWormholeClasses";

        /// <summary>
        /// Imports data in table of specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public static void Import(SqlConnection connection)
        {
            DateTime startTime = DateTime.Now;
            Util.ResetCounters();

            try
            {
                s_total = Database.UniverseDataContext.mapLocationWormholeClasses.Count();
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine(e.InnerException.Message);
                return;
            }

            Console.WriteLine();
            Console.Write(@"Importing {0}... ", TableName);

            Database.CreateTable(connection, TableName);

            ImportData(connection);

            Util.DisplayEndTime(startTime);

            Console.WriteLine();
        }

        /// <summary>
        /// Imports the data.
        /// </summary>
        /// <param name="connection">The connection.</param>
        private static void ImportData(SqlConnection connection)
        {
            SqlCommand command = new SqlCommand { Connection = connection };

            using (var tx = connection.BeginTransaction())
            {
                command.Transaction = tx;
                try
                {
                    foreach (mapLocationWormholeClasses mLocWormClass in Database.UniverseDataContext.mapLocationWormholeClasses)
                    {
                        Util.UpdatePercentDone(s_total);

                        Dictionary<string, string> parameters = new Dictionary<string, string>();
                        parameters["locationID"] = mLocWormClass.locationID.ToString(CultureInfo.InvariantCulture);
                        parameters["wormholeClassID"] = mLocWormClass.wormholeClassID.GetValueOrDefaultString();

                        command.CommandText = Database.SqlInsertCommandText(TableName, parameters);
                        command.ExecuteNonQuery();
                    }

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
    }
}
