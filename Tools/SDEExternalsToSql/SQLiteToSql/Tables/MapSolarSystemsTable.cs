using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace EVEMon.SDEExternalsToSql.SQLiteToSql.Tables
{
    internal static class MapSolarSystemsTable
    {
        private static int s_total;
        private const string TableName = "mapSolarSystems";

        /// <summary>
        /// Imports data in table of specified connection.
        /// </summary>
        public static void Import()
        {
            DateTime startTime = DateTime.Now;
            Util.ResetCounters();

            try
            {
                s_total = Database.UniverseDataContext.mapSolarSystems.Count();
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
                    foreach (mapSolarSystems mSolarSystem in Database.UniverseDataContext.mapSolarSystems)
                    {
                        Util.UpdatePercentDone(s_total);

                        Dictionary<string, string> parameters = new Dictionary<string, string>();
                        parameters["regionID"] = mSolarSystem.regionID.GetValueOrDefaultString();
                        parameters["constellationID"] =
                            mSolarSystem.constellationID.GetValueOrDefaultString();
                        parameters["solarSystemID"] = mSolarSystem.solarSystemID.ToString(CultureInfo.InvariantCulture);
                        parameters["solarSystemName"] = mSolarSystem.solarSystemName.GetTextOrDefaultString();
                        parameters["x"] = mSolarSystem.x.GetValueOrDefaultString();
                        parameters["y"] = mSolarSystem.y.GetValueOrDefaultString();
                        parameters["z"] = mSolarSystem.z.GetValueOrDefaultString();
                        parameters["xMin"] = mSolarSystem.xMin.GetValueOrDefaultString();
                        parameters["xMax"] = mSolarSystem.xMax.GetValueOrDefaultString();
                        parameters["yMin"] = mSolarSystem.yMin.GetValueOrDefaultString();
                        parameters["yMax"] = mSolarSystem.yMax.GetValueOrDefaultString();
                        parameters["zMin"] = mSolarSystem.zMin.GetValueOrDefaultString();
                        parameters["zMax"] = mSolarSystem.zMax.GetValueOrDefaultString();
                        parameters["luminosity"] = mSolarSystem.luminosity.GetValueOrDefaultString();
                        parameters["border"] = mSolarSystem.border.GetValueOrDefaultString();
                        parameters["fringe"] =  mSolarSystem.fringe.GetValueOrDefaultString();
                        parameters["corridor"] =  mSolarSystem.corridor.GetValueOrDefaultString();
                        parameters["hub"] =  mSolarSystem.hub.GetValueOrDefaultString();
                        parameters["international"] =  mSolarSystem.international.GetValueOrDefaultString();
                        parameters["regional"] =  mSolarSystem.regional.GetValueOrDefaultString();
                        parameters["constellation"] = mSolarSystem.constellation.GetValueOrDefaultString();
                        parameters["security"] = mSolarSystem.security.GetValueOrDefaultString();
                        parameters["factionID"] = mSolarSystem.factionID.GetValueOrDefaultString();
                        parameters["radius"] = mSolarSystem.radius.GetValueOrDefaultString();
                        parameters["sunTypeID"] = mSolarSystem.sunTypeID.GetValueOrDefaultString();
                        parameters["securityClass"] = mSolarSystem.securityClass.GetTextOrDefaultString();

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