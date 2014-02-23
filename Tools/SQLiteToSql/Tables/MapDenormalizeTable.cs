using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace EVEMon.SQLiteToSql.Tables
{
    internal static class MapDenormalizeTable
    {
        private const string TableName = "mapDenormalize";

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
            var total = Database.UniverseDataContext.mapDenormalize.Count();

            using (var tx = connection.BeginTransaction())
            {
                command.Transaction = tx;
                try
                {
                    foreach (mapDenormalize mDenorm in Database.UniverseDataContext.mapDenormalize)
                    {
                        Util.UpdatePercentDone(total);

                        Dictionary<string, string> parameters = new Dictionary<string, string>();
                        parameters["itemID"] = mDenorm.itemID.ToString(CultureInfo.InvariantCulture);
                        parameters["typeID"] = mDenorm.typeID.GetValueOrDefaultString();
                        parameters["groupID"] = mDenorm.groupID.GetValueOrDefaultString();
                        parameters["solarSystemID"] = mDenorm.solarSystemID.GetValueOrDefaultString();
                        parameters["constellationID"] = mDenorm.constellationID.GetValueOrDefaultString();
                        parameters["regionID"] = mDenorm.regionID.GetValueOrDefaultString();
                        parameters["orbitID"] = mDenorm.orbitID.GetValueOrDefaultString();
                        parameters["x"] = mDenorm.x.GetValueOrDefaultString();
                        parameters["y"] = mDenorm.y.GetValueOrDefaultString();
                        parameters["z"] = mDenorm.z.GetValueOrDefaultString();
                        parameters["radius"] = mDenorm.radius.GetValueOrDefaultString();
                        parameters["itemName"] = String.Format("'{0}'", mDenorm.itemName.Replace("'", Database.StringEmpty));
                        parameters["security"] = mDenorm.security.GetValueOrDefaultString();
                        parameters["celestialIndex"] = mDenorm.celestialIndex.GetValueOrDefaultString();
                        parameters["orbitIndex"] = mDenorm.orbitIndex.GetValueOrDefaultString();
                        
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