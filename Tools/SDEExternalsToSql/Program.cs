using System;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading;
using EVEMon.SDEExternalsToSql.SQLiteToSql;
using EVEMon.SDEExternalsToSql.SQLiteToSql.Tables;
using EVEMon.SDEExternalsToSql.YamlToSql.Tables;

namespace EVEMon.SDEExternalsToSql
{
    internal static class Program
    {
        private static void Main()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            SqlConnection sqlConnection = Database.Connect<SqlConnection>("EveStaticData");

            if (sqlConnection != null)
            {
                InvTypes.Import(sqlConnection);
                EveGraphics.Import(sqlConnection);
                EveIcons.Import(sqlConnection);
                Certificates.Import(sqlConnection);
            }

            Console.WriteLine();

            EntityConnection sqliteConnection = Database.Connect<EntityConnection>("UniverseDataEntities");

            if (sqlConnection != null && sqliteConnection != null)
            {
                Database.UniverseDataContext = new UniverseDataEntities();

                MapCelestialStatisticsTable.Import(sqlConnection);
                MapConstellationJumpsTable.Import(sqlConnection);
                MapConstellationsTable.Import(sqlConnection);
                MapDenormalizeTable.Import(sqlConnection);
                MapJumpsTable.Import(sqlConnection);
                MapLandmarksTable.Import(sqlConnection);
                MapLocationScenesTable.Import(sqlConnection);
                MapLocationWormholeClassesTable.Import(sqlConnection);
                MapRegionJumpsTable.Import(sqlConnection);
                MapRegionTable.Import(sqlConnection);
                MapSolarSystemJumpsTable.Import(sqlConnection);
                MapSolarSystemsTable.Import(sqlConnection);
            }

            if (sqliteConnection != null)
                Database.Disconnect(sqliteConnection);

            if (sqlConnection != null)
                Database.Disconnect(sqlConnection);

            Console.WriteLine();
            Console.Write(@"Press any key to exit.");
            Console.ReadLine();
        }
    }
}
