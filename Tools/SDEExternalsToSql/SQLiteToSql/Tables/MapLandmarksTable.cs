using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using EVEMon.SDEExternalsToSql.SQLiteToSql.Models;

namespace EVEMon.SDEExternalsToSql.SQLiteToSql.Tables
{
    internal static class MapLandmarksTable
    {
        private static int s_total;
        private const string TableName = "mapLandmarks";

        /// <summary>
        /// Imports data in table of specified connection.
        /// </summary>
        public static void Import()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            Util.ResetCounters();

            try
            {
                s_total = Database.UniverseDataContext.mapLandmarks.Count();
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine(Util.GetExceptionMessage(e));
                return;
            }

            Console.Write(@"Importing {0}... ", TableName);

            Database.CreateTable(TableName);

            ImportData();

            Util.DisplayEndTime(stopwatch);

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
                    foreach (mapLandmarks mLandmark in Database.UniverseDataContext.mapLandmarks)
                    {
                        Util.UpdatePercentDone(s_total);

                        Dictionary<string, string> parameters = new Dictionary<string, string>();
                        parameters["landmarkID"] = mLandmark.landmarkID.ToString(CultureInfo.InvariantCulture);
                        parameters["landmarkName"] = mLandmark.landmarkName.GetTextOrDefaultString();
                        parameters["description"] = mLandmark.description.GetTextOrDefaultString();
                        parameters["locationID"] = mLandmark.locationID.GetValueOrDefaultString();
                        parameters["x"] = mLandmark.x.GetValueOrDefaultString();
                        parameters["y"] = mLandmark.y.GetValueOrDefaultString();
                        parameters["z"] = mLandmark.z.GetValueOrDefaultString();
                        parameters["radius"] = Database.DbNull;
                        parameters["iconID"] = mLandmark.iconID.GetValueOrDefaultString();
                        parameters["importance"] = Database.DbNull;

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