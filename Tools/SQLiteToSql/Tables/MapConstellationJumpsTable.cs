using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace EVEMon.SQLiteToSql.Tables
{
    internal static class MapConstellationJumpsTable
    {
        private const string TableName = "mapConstellationJumps";

        /// <summary>
        /// Imports data in table of specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public static void Import(SqlConnection connection)
        {
            DateTime startTime = DateTime.Now;
            Util.ResetCounters();

            Database.CreateTable(connection, TableName);

            Console.WriteLine();
            Console.Write(@"Importing {0}... ", TableName);

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
            SqlCommand command = new SqlCommand
                                 {
                                     Connection = connection
                                 };
            var total = Database.Context.mapConstellationJumps.Count();

            using (var tx = connection.BeginTransaction())
            {
                command.Transaction = tx;
                try
                {
                    foreach (mapConstellationJumps mConstJump in Database.Context.mapConstellationJumps)
                    {
                        Util.UpdatePercentDone(total);

                        Dictionary<string, string> parameters = new Dictionary<string, string>();
                        parameters["fromConstellationID"] = mConstJump.fromConstellationID.ToString(CultureInfo.InvariantCulture);
                        parameters["fromRegionID"] = mConstJump.fromRegionID.GetValueOrDefaultString();
                        parameters["toConstellationID"] = mConstJump.toConstellationID.ToString(CultureInfo.InvariantCulture);
                        parameters["toRegionID"] = mConstJump.toRegionID.GetValueOrDefaultString();

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