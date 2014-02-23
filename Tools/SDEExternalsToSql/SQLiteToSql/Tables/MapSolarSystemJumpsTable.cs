using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace EVEMon.SDEExternalsToSql.SQLiteToSql.Tables
{
    internal static class MapSolarSystemJumpsTable
    {
        private static int s_total;
        private const string TableName = "mapSolarSystemJumps";

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
                s_total = Database.UniverseDataContext.mapSolarSystemJumps.Count();
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
                    foreach (mapSolarSystemJumps mSolarSystemJump in Database.UniverseDataContext.mapSolarSystemJumps)
                    {
                        Util.UpdatePercentDone(s_total);

                        Dictionary<string, string> parameters = new Dictionary<string, string>();
                        parameters["fromRegionID"] = mSolarSystemJump.fromRegionID.GetValueOrDefaultString();
                        parameters["fromConstellationID"] = mSolarSystemJump.fromConstellationID.GetValueOrDefaultString();
                        parameters["fromSolarSystemID"] = mSolarSystemJump.fromSolarSystemID.ToString(CultureInfo.InvariantCulture);
                        parameters["toSolarSystemID"] = mSolarSystemJump.toSolarSystemID.ToString(CultureInfo.InvariantCulture);
                        parameters["toConstellationID"] = mSolarSystemJump.toConstellationID.GetValueOrDefaultString();
                        parameters["toRegionID"] = mSolarSystemJump.toRegionID.GetValueOrDefaultString();

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