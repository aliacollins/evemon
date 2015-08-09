using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using EVEMon.SDEExternalsToSql.SQLiteToSql.Models;

namespace EVEMon.SDEExternalsToSql.SQLiteToSql.Tables
{
    internal static class MapDenormalizeTable
    {
        private static int s_total;
        private const string TableName = "mapDenormalize";

        /// <summary>
        /// Imports data in table of specified connection.
        /// </summary>
        public static void Import()
        {
            DateTime startTime = DateTime.Now;
            Util.ResetCounters();

            try
            {
                s_total = Database.UniverseDataContext.mapDenormalize.Count();
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
                    foreach (mapDenormalize mDenorm in Database.UniverseDataContext.mapDenormalize)
                    {
                        Util.UpdatePercentDone(s_total);

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
                        parameters["itemName"] = mDenorm.itemName.GetTextOrDefaultString(isUnicode: true);
                        parameters["security"] = mDenorm.security.GetValueOrDefaultString();
                        parameters["celestialIndex"] = mDenorm.celestialIndex.GetValueOrDefaultString();
                        parameters["orbitIndex"] = mDenorm.orbitIndex.GetValueOrDefaultString();
                        
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