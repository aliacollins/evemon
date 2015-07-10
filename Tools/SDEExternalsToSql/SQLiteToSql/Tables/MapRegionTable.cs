using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace EVEMon.SDEExternalsToSql.SQLiteToSql.Tables
{
    internal static class MapRegionTable
    {
        private static int s_total;
        private const string TableName = "mapRegions";

        /// <summary>
        /// Imports data in table of specified connection.
        /// </summary>
        public static void Import()
        {
            DateTime startTime = DateTime.Now;
            Util.ResetCounters();

            try
            {
                s_total = Database.UniverseDataContext.mapRegions.Count();
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
                    foreach (mapRegions mRegion in Database.UniverseDataContext.mapRegions)
                    {
                        Util.UpdatePercentDone(s_total);

                        Dictionary<string, string> parameters = new Dictionary<string, string>();
                        parameters["regionID"] = mRegion.regionID.ToString(CultureInfo.InvariantCulture);
                        parameters["regionName"] = mRegion.regionName.GetTextOrDefaultString(isUnicode: true);
                        parameters["x"] = mRegion.x.GetValueOrDefaultString();
                        parameters["y"] = mRegion.y.GetValueOrDefaultString();
                        parameters["z"] = mRegion.z.GetValueOrDefaultString();
                        parameters["xMin"] = mRegion.xMin.GetValueOrDefaultString();
                        parameters["xMax"] = mRegion.xMax.GetValueOrDefaultString();
                        parameters["yMin"] = mRegion.yMin.GetValueOrDefaultString();
                        parameters["yMax"] = mRegion.yMax.GetValueOrDefaultString();
                        parameters["zMin"] = mRegion.zMin.GetValueOrDefaultString();
                        parameters["zMax"] = mRegion.zMax.GetValueOrDefaultString();
                        parameters["factionID"] = mRegion.factionID.GetValueOrDefaultString();
                        parameters["radius"] = mRegion.radius.GetValueOrDefaultString();

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