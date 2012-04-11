using System;
using System.Globalization;
using System.Threading;
using EVEMon.XmlGenerator.Datafiles;
using EVEMon.XmlGenerator.Xmlfiles;

namespace EVEMon.XmlGenerator
{
    internal static class Program
    {
        private static void Main()
        {
            DateTime startTime = DateTime.Now;

            // Setting a standard format for the generated files
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            // Create tables from database
            Database.CreateTables();

            Console.WriteLine();

            // Generate datafiles
            Properties.GenerateDatafile();
            Items.GenerateDatafile(); // Requires GenerateProperties()
            Skills.GenerateDatafile();
            Certificates.GenerateDatafile();
            Blueprints.GenerateDatafile();
            Geography.GenerateDatafile();
            Reprocessing.GenerateDatafile(); // Requires GenerateItems()
            
            // Generate support xml files
            Flags.GenerateXMLfile();

            Util.CreateMD5SumsFile("MD5Sums.txt");

            Console.WriteLine(String.Format(CultureInfo.CurrentCulture, "Generating files completed in {0}",
                                            DateTime.Now.Subtract(startTime)).TrimEnd('0'));
            Console.ReadLine();
        }
    }
}