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

            EntityConnection sqliteConnection = Database.Connect<EntityConnection>("UniverseDataEntities");
            SqlConnection sqlConnection = Database.Connect<SqlConnection>("EveStaticData");
            Database.UniverseDataContext = new UniverseDataEntities();

            //InvTypes.Import(sqlConnection);
            //EveGraphics.Import(sqlConnection);
            //EveIcons.Import(sqlConnection);
            //Certificates.Import(sqlConnection);
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

            Database.Disconnect(sqliteConnection);
            Database.Disconnect(sqlConnection);

            Console.WriteLine();
            Console.Write(@"Press any key to exit.");
            Console.ReadLine();

        }
    }
}
