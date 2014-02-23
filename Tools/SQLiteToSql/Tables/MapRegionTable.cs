using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace EVEMon.SQLiteToSql.Tables
{
    internal static class MapRegionTable
    {
        private const string TableName = "mapRegions";

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
            var total = Database.UniverseDataContext.mapRegions.Count();

            using (var tx = connection.BeginTransaction())
            {
                command.Transaction = tx;
                try
                {
                    foreach (mapRegions mRegion in Database.UniverseDataContext.mapRegions)
                    {
                        Util.UpdatePercentDone(total);

                        Dictionary<string, string> parameters = new Dictionary<string, string>();
                        parameters["regionID"] = mRegion.regionID.ToString(CultureInfo.InvariantCulture);
                        parameters["regionName"] = String.Format("'{0}'", mRegion.regionName.Replace("'", Database.StringEmpty));
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