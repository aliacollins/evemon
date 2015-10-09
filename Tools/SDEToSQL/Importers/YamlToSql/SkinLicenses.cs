using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using EVEMon.SDEToSQL.Providers;
using EVEMon.SDEToSQL.Utils;
using YamlDotNet.RepresentationModel;

namespace EVEMon.SDEToSQL.Importers.YamlToSQL
{
    internal static class SkinLicenses
    {
        private const string SknLicensesTableName = "sknLicenses";

        private const string LicenseTypeIDText = "licenseTypeID";
        private const string SkinIDText = "skinID";
        private const string DurationText = "duration";

        private static SqlConnectionProvider s_sqlConnectionProvider;
        private static bool s_isClosing;

        /// <summary>
        /// Initializes the <see cref="Util"/> class.
        /// </summary>
        static SkinLicenses()
        {
            Util.Closing += Util_Closing;
        }

        /// <summary>
        /// Handles the Closing event of the Program control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private static void Util_Closing(object sender, EventArgs e)
        {
            s_isClosing = true;
        }

        /// <summary>
        /// Imports the skin licenses.
        /// </summary>
        internal static void Import(DbConnectionProvider sqlConnectionProvider)
        {
            if (sqlConnectionProvider == null)
                throw new ArgumentNullException("sqlConnectionProvider");

            s_sqlConnectionProvider = (SqlConnectionProvider)sqlConnectionProvider;

            if (s_isClosing)
                return;

            Stopwatch stopwatch = Stopwatch.StartNew();
            Util.ResetCounters();

            string yamlFile = YamlFilesConstants.SkinLicenses;
            string filePath = Util.CheckYamlFileExists(yamlFile);

            if (String.IsNullOrEmpty(filePath))
                return;

            string text = String.Format(CultureInfo.InvariantCulture, "Parsing {0}... ", yamlFile);
            Console.Write(text);
            YamlMappingNode rNode = Util.ParseYamlFile(filePath);

            if (s_isClosing)
                return;

            Util.SetConsoleCursorPosition(text);

            if (rNode == null)
            {
                Console.WriteLine(@"Unable to parse {0}.", yamlFile);
                return;
            }

            Console.Write(@"Importing {0}... ", yamlFile);

            s_sqlConnectionProvider.DropAndCreateTable(SknLicensesTableName);

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
            if (s_isClosing)
                return;

            Util.UpdatePercentDone(0);

            DataTable sknLicensesTable = GetSknLicensesDataTable();

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

            s_sqlConnectionProvider.ImportDataBulk(SknLicensesTableName, sknLicensesTable);

            Util.UpdatePercentDone(sknLicensesTable.Rows.Count);
        }

        /// <summary>
        /// Gets the data table for the sknLicenses table.
        /// </summary>
        /// <returns></returns>
        private static DataTable GetSknLicensesDataTable()
        {
            using (DataTable sknLicensesTable = new DataTable())
            {
                sknLicensesTable.Columns.AddRange(
                    new[]
                {
                    new DataColumn(LicenseTypeIDText, typeof(SqlInt32)),
                    new DataColumn(SkinIDText, typeof(SqlInt32)),
                    new DataColumn(DurationText, typeof(SqlInt32)),
                });
                return sknLicensesTable;
            }
        }
    }
}
