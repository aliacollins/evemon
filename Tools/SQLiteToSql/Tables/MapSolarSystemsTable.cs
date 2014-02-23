using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace EVEMon.SQLiteToSql.Tables
{
    internal static class MapSolarSystemsTable
    {
        private const string TableName = "mapSolarSystems";

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
            var total = Database.UniverseDataContext.mapSolarSystems.Count();

            using (var tx = connection.BeginTransaction())
            {
                command.Transaction = tx;
                try
                {
                    foreach (mapSolarSystems mSolarSystem in Database.UniverseDataContext.mapSolarSystems)
                    {
                        Util.UpdatePercentDone(total);

                        Dictionary<string, string> parameters = new Dictionary<string, string>();
                        parameters["regionID"] = mSolarSystem.regionID.GetValueOrDefaultString();
                        parameters["constellationID"] =
                            mSolarSystem.constellationID.GetValueOrDefaultString();
                        parameters["solarSystemID"] = mSolarSystem.solarSystemID.ToString(CultureInfo.InvariantCulture);
                        parameters["solarSystemName"] = String.Format("'{0}'", mSolarSystem.solarSystemName.Replace("'", Database.StringEmpty));
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
                        parameters["border"] = Convert.ToByte(mSolarSystem.border.GetValueOrDefault()).ToString(CultureInfo.InvariantCulture);
                        parameters["fringe"] =  Convert.ToByte(mSolarSystem.fringe.GetValueOrDefault()).ToString(CultureInfo.InvariantCulture);
                        parameters["corridor"] =  Convert.ToByte(mSolarSystem.corridor.GetValueOrDefault()).ToString(CultureInfo.InvariantCulture);
                        parameters["hub"] =  Convert.ToByte(mSolarSystem.hub.GetValueOrDefault()).ToString(CultureInfo.InvariantCulture);
                        parameters["international"] =  Convert.ToByte(mSolarSystem.international.GetValueOrDefault()).ToString(CultureInfo.InvariantCulture);
                        parameters["regional"] =  Convert.ToByte(mSolarSystem.regional.GetValueOrDefault()).ToString(CultureInfo.InvariantCulture);
                        parameters["constellation"] = Convert.ToByte(mSolarSystem.constellation.GetValueOrDefault()).ToString(CultureInfo.InvariantCulture);
                        parameters["security"] = mSolarSystem.security.GetValueOrDefaultString();
                        parameters["factionID"] = mSolarSystem.factionID.GetValueOrDefaultString();
                        parameters["radius"] = mSolarSystem.radius.GetValueOrDefaultString();
                        parameters["sunTypeID"] = mSolarSystem.sunTypeID.GetValueOrDefaultString();
                        parameters["securityClass"] = mSolarSystem.securityClass != null
                            ? String.Format("'{0}'", mSolarSystem.securityClass)
                            : Database.Null;

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