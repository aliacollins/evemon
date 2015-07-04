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

            Database.SqlConnection = Database.Connect<SqlConnection>("EveStaticData");

            if (Database.SqlConnection != null)
            {
                InvTypes.Import();
                EveGraphics.Import();
                EveIcons.Import();
                Certificates.Import();
                Blueprints.Import();
                Skins.Import();
                SkinMaterials.Import();
                SkinLicenses.Import();
            }

            Console.WriteLine();

            Database.SqliteConnection = Database.Connect<EntityConnection>("UniverseDataEntities");

            if (Database.SqlConnection != null && Database.SqliteConnection != null)
            {
                Database.UniverseDataContext = new UniverseDataEntities();

                MapCelestialStatisticsTable.Import();
                MapConstellationJumpsTable.Import();
                MapConstellationsTable.Import();
                MapDenormalizeTable.Import();
                MapJumpsTable.Import();
                MapLandmarksTable.Import();
                MapLocationScenesTable.Import();
                MapLocationWormholeClassesTable.Import();
                MapRegionJumpsTable.Import();
                MapRegionTable.Import();
                MapSolarSystemJumpsTable.Import();
                MapSolarSystemsTable.Import();
            }

            if (Database.SqliteConnection != null)
                Database.Disconnect(Database.SqliteConnection);

            if (Database.SqlConnection != null)
                Database.Disconnect(Database.SqlConnection);

            Console.WriteLine();
            Console.Write(@"Press any key to exit.");
            Console.ReadLine();
        }
    }
}
