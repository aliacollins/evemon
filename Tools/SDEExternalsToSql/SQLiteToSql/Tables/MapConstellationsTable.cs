using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace EVEMon.SDEExternalsToSql.SQLiteToSql.Tables
{
    internal static class MapConstellationsTable
    {
        private static int s_total;
        private const string TableName = "mapConstellations";

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
                s_total = Database.UniverseDataContext.mapConstellations.Count();
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine(e.InnerException.Message);
                return;
            }

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
            SqlCommand command = new SqlCommand { Connection = connection };

            using (var tx = connection.BeginTransaction())
            {
                command.Transaction = tx;
                try
                {
                    foreach (mapConstellations mConsts in Database.UniverseDataContext.mapConstellations)
                    {
                        Util.UpdatePercentDone(s_total);

                        Dictionary<string, string> parameters = new Dictionary<string, string>();
                        parameters["constellationID"] = mConsts.constellationID.ToString(CultureInfo.InvariantCulture);
                        parameters["regionID"] = mConsts.regionID.GetValueOrDefaultString();
                        parameters["constellationName"] = String.Format("'{0}'", mConsts.constellationName.Replace("'", Database.StringEmpty));
                        parameters["x"] = mConsts.x.GetValueOrDefaultString();
                        parameters["y"] = mConsts.y.GetValueOrDefaultString();
                        parameters["z"] = mConsts.z.GetValueOrDefaultString();
                        parameters["xMin"] = mConsts.xMin.GetValueOrDefaultString();
                        parameters["xMax"] = mConsts.xMax.GetValueOrDefaultString();
                        parameters["yMin"] = mConsts.yMin.GetValueOrDefaultString();
                        parameters["yMax"] = mConsts.yMax.GetValueOrDefaultString();
                        parameters["zMin"] = mConsts.zMin.GetValueOrDefaultString();
                        parameters["zMax"] = mConsts.zMax.GetValueOrDefaultString();
                        parameters["factionID"] = mConsts.factionID.GetValueOrDefaultString();
                        parameters["radius"] = mConsts.radius.GetValueOrDefaultString();

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