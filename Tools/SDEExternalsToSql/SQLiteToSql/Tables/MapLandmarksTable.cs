using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

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
            DateTime startTime = DateTime.Now;
            Util.ResetCounters();

            try
            {
                s_total = Database.UniverseDataContext.mapLandmarks.Count();
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
            SqlCommand command = new SqlCommand { Connection = Database.SqlConnection };

            using (var tx = Database.SqlConnection.BeginTransaction())
            {
                command.Transaction = tx;
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
                        parameters["radius"] = Database.Null;
                        parameters["iconID"] = mLandmark.iconID.GetValueOrDefaultString();
                        parameters["importance"] = Database.Null;

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