using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace EVEMon.SDEExternalsToSql.YamlToSql.Tables
{
    internal class Icons
    {
        private const string EveIconsTableName = "eveIcons";

        private const string IconIDText = "iconID";
        private const string IconFileText = "iconFile";
        private const string DescriptionText = "description";

        /// <summary>
        /// Imports the icon ids.
        /// </summary>
        internal static void Import()
        {
            if (Program.IsClosing)
                return;

            Stopwatch stopwatch = Stopwatch.StartNew();
            Util.ResetCounters();

            string yamlFile = YamlFilesConstants.iconIDS;
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

            Database.CreateTable(EveIconsTableName);

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

            DataTable eveIconsTable = new DataTable();
            eveIconsTable.Columns.AddRange(
                new[]
                {
                    new DataColumn(IconIDText, typeof(SqlInt32)),
                    new DataColumn(IconFileText, typeof(SqlString)),
                    new DataColumn(DescriptionText, typeof(SqlString)),
                });

            int total = rNode.Count();
            total = (int)Math.Ceiling(total + (total * 0.01));

            foreach (KeyValuePair<YamlNode, YamlNode> pair in rNode.Children)
            {
                Util.UpdatePercentDone(total);

                YamlMappingNode cNode = pair.Value as YamlMappingNode;

                if (cNode == null)
                    continue;

                DataRow row = eveIconsTable.NewRow();
                row[IconIDText] = SqlInt32.Parse(pair.Key.ToString());
                row[IconFileText] = cNode.Children.GetSqlTypeOrDefault<SqlString>(IconFileText, defaultValue: "");
                row[DescriptionText] = cNode.Children.GetSqlTypeOrDefault<SqlString>(DescriptionText, defaultValue: "");

                eveIconsTable.Rows.Add(row);
            }

            Database.ImportDataBulk(EveIconsTableName, eveIconsTable);

            Util.UpdatePercentDone(eveIconsTable.Rows.Count);
        }
    }
}
