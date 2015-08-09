using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using EVEMon.SDEExternalsToSql.SQLiteToSql.Models;

namespace EVEMon.SDEExternalsToSql.SQLiteToSql.Tables
{
    internal static class MapSolarSystemJumpsTable
    {
        private static int s_total;
        private const string TableName = "mapSolarSystemJumps";

        /// <summary>
        /// Imports data in table of specified connection.
        /// </summary>
        public static void Import()
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
                Console.WriteLine(Util.GetExceptionMessage(e));
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