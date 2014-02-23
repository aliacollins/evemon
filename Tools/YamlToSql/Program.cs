using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading;
using EVEMon.YamlToSql.Tables;

namespace EVEMon.YamlToSql
{
    internal static class Program
    {
        private static void Main()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            SqlConnection connection = Database.Connect<SqlConnection>("EveStaticData");
            
            InvTypes.Import(connection);
            EveGraphics.Import(connection);
            EveIcons.Import(connection);
            Certificates.Import(connection);

            Database.Disconnect(connection);

            Console.WriteLine();
            Console.Write(@"Press any key to exit.");
            Console.ReadLine();
        }
    }
}
