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

            SqlConnection connection = Database.Connect();
            InvTypes.ImportTypeIds(connection);
            EveGraphics.ImportGraphicIds(connection);
            EveIcons.ImportIconIds(connection);
            Certificates.ImportCertificates(connection);
            Database.Disconnect();
        }
    }
}
