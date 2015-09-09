using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace EVEMon.SDEToSQL.YamlToSQL.Tables
{
    internal class SkinLicenses
    {
        private const string SknLicensesTableName = "sknLicenses";

        private const string LicenseTypeIDText = "licenseTypeID";
        private const string SkinIDText = "skinID";
        private const string DurationText = "duration";

        /// <summary>
        /// Imports the skin licenses.
        /// </summary>
        internal static void Import()
        {
            if (Program.IsClosing)
                return;

            Stopwatch stopwatch = Stopwatch.StartNew();
            Util.ResetCounters();

            string yamlFile = YamlFilesConstants.skinLicenses;
            string filePath = Util.CheckYamlFileExists(yamlFile);

            if (String.IsNullOrEmpty(filePath))
                return;

            string text = String.Format("Parsing {0}... ", yamlFile);
            Console.Write(text);
            YamlMappingNode rNode = Util.ParseYamlFile(filePath);

            if (rNode == null)
            {
                Console.WriteLine(@"Unable to parse {0}.", yamlFile);
                return;
            }

            Console.SetCursorPosition(Console.CursorLeft - text.Length, Console.CursorTop);

            Console.Write(@"Importing {0}... ", yamlFile);

            Database.DropAndCreateTable(SknLicensesTableName);

            ImportDataBulk(rNode);

            Util.DisplayEndTime(stopwatch);

            Console.WriteLine();
        }

        /// <summary>
        /// Imports the data bulk.
        /// </summary>
        /// <param name="rNode">The r node.</param>
        private static void ImportDataBulk(YamlMappingNode rNode)
        {
            Util.UpdatePercentDone(0);

            DataTable sknLicensesTable = new DataTable();
            sknLicensesTable.Columns.AddRange(
                new[]
                {
                    new DataColumn(LicenseTypeIDText, typeof(SqlInt32)),
                    new DataColumn(SkinIDText, typeof(SqlInt32)),
                    new DataColumn(DurationText, typeof(SqlInt32)),
                });

            int total = rNode.Count();
            total = (int)Math.Ceiling(total + (total * 0.01));

            foreach (KeyValuePair<YamlNode, YamlNode> pair in rNode.Children)
            {
                Util.UpdatePercentDone(total);

                YamlMappingNode cNode = pair.Value as YamlMappingNode;

                if (cNode == null)
                    continue;

                DataRow row = sknLicensesTable.NewRow();
                row[LicenseTypeIDText] = SqlInt32.Parse(pair.Key.ToString());
                row[SkinIDText] = cNode.Children.GetSqlTypeOrDefault<SqlInt32>(SkinIDText);
                row[DurationText] = cNode.Children.GetSqlTypeOrDefault<SqlInt32>(DurationText, defaultValue: "-1");

                sknLicensesTable.Rows.Add(row);
            }

            Database.ImportDataBulk(SknLicensesTableName, sknLicensesTable);

            Util.UpdatePercentDone(sknLicensesTable.Rows.Count);
        }
    }
}
