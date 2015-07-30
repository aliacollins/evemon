using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace EVEMon.SDEExternalsToSql.SQLiteToSql.Tables
{
    internal static class MapConstellationJumpsTable
    {
        private static int s_total;
        private const string TableName = "mapConstellationJumps";

        /// <summary>
        /// Imports data in table of specified connection.
        /// </summary>
        public static void Import()
        {
            DateTime startTime = DateTime.Now;
            Util.ResetCounters();

            try
            {
                s_total = Database.UniverseDataContext.mapConstellationJumps.Count();
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine(e.InnerException.Message);
                return;
            }

            Console.WriteLine();
            Console.Write(@"Importing {0}... ", TableName);

            Database.CreateTable(TableName);

            ImportData();

            Util.DisplayEndTime(startTime);

            Console.WriteLine();
        }

        /// <summary>
        /// Imports the data.
        /// </summary>
        private static void ImportData()
        {
            using (IDbCommand command = new SqlCommand(
                String.Empty,
                Database.SqlConnection,
                Database.SqlConnection.BeginTransaction()))
            {
                try
                {
                    foreach (mapConstellationJumps mConstJump in Database.UniverseDataContext.mapConstellationJumps)
                    {
                        Util.UpdatePercentDone(s_total);

                        Dictionary<string, string> parameters = new Dictionary<string, string>();
                        parameters["fromConstellationID"] = mConstJump.fromConstellationID.ToString(CultureInfo.InvariantCulture);
                        parameters["fromRegionID"] = mConstJump.fromRegionID.GetValueOrDefaultString();
                        parameters["toConstellationID"] = mConstJump.toConstellationID.ToString(CultureInfo.InvariantCulture);
                        parameters["toRegionID"] = mConstJump.toRegionID.GetValueOrDefaultString();

                        command.CommandText = Database.SqlInsertCommandText(TableName, parameters);
                        command.ExecuteNonQuery();
                    }

                    command.Transaction.Commit();
                }
                catch (SqlException e)
                {
                    command.Transaction.Rollback();
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