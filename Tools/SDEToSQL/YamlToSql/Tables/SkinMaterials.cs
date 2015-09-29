using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace EVEMon.SDEToSQL.YamlToSQL.Tables
{
    internal static class SkinMaterials
    {
        private const string SknMaterialsTableName = "sknMaterials";

        private const string SkinMaterialIDText = "skinMaterialID";
        private const string MaterialSetIDText = "materialSetID";
        private const string DisplayNameIDText = "displayNameID";

        // Obsolete since Galatea 1.0
        private const string MaterialText = "material";
        private const string ColorHullText = "colorHull";
        private const string ColorWindowText = "colorWindow";
        private const string ColorPrimaryText = "colorPrimary";
        private const string ColorSecondaryText = "colorSecondary";

        /// <summary>
        /// Imports the skin materials.
        /// </summary>
        internal static void Import()
        {
            if (Program.IsClosing)
                return;

            Stopwatch stopwatch = Stopwatch.StartNew();
            Util.ResetCounters();

            string yamlFile = YamlFilesConstants.skinMaterials;
            string filePath = Util.CheckYamlFileExists(yamlFile);

            if (String.IsNullOrEmpty(filePath))
                return;

            string text = String.Format(CultureInfo.InvariantCulture, "Parsing {0}... ", yamlFile);
            Console.Write(text);
            YamlMappingNode rNode = Util.ParseYamlFile(filePath);

            if (rNode == null)
            {
                Console.WriteLine(@"Unable to parse {0}.", yamlFile);
                return;
            }

            Console.SetCursorPosition(Console.CursorLeft - text.Length, Console.CursorTop);

            Console.Write(@"Importing {0}... ", yamlFile);

            Database.DropAndCreateTable(SknMaterialsTableName);

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

            DataTable sknMaterialsTable = GetSknMaterialsDataTable();

            int total = rNode.Count();
            total = (int)Math.Ceiling(total + (total * 0.01));

            foreach (KeyValuePair<YamlNode, YamlNode> pair in rNode.Children)
            {
                Util.UpdatePercentDone(total);

                YamlMappingNode cNode = pair.Value as YamlMappingNode;

                if (cNode == null)
                    continue;

                DataRow row = sknMaterialsTable.NewRow();
                row[SkinMaterialIDText] = SqlInt32.Parse(pair.Key.ToString());
                row[MaterialSetIDText] = cNode.Children.GetSqlTypeOrDefault<SqlInt32>(MaterialSetIDText, defaultValue: "0");
                row[DisplayNameIDText] = cNode.Children.GetSqlTypeOrDefault<SqlInt32>(DisplayNameIDText, defaultValue: "0");

                // Obsolete since Galatea 1.0
                row[MaterialText] = cNode.Children.GetSqlTypeOrDefault<SqlString>(MaterialText, defaultValue: "");
                row[ColorHullText] = cNode.Children.GetSqlTypeOrDefault<SqlString>(ColorHullText);
                row[ColorWindowText] = cNode.Children.GetSqlTypeOrDefault<SqlString>(ColorWindowText);
                row[ColorPrimaryText] = cNode.Children.GetSqlTypeOrDefault<SqlString>(ColorPrimaryText);
                row[ColorSecondaryText] = cNode.Children.GetSqlTypeOrDefault<SqlString>(ColorSecondaryText);

                sknMaterialsTable.Rows.Add(row);
            }

            Database.ImportDataBulk(SknMaterialsTableName, sknMaterialsTable);

            Util.UpdatePercentDone(sknMaterialsTable.Rows.Count);
        }

        /// <summary>
        /// Gets the data table for the sknMaterials table.
        /// </summary>
        /// <returns></returns>
        private static DataTable GetSknMaterialsDataTable()
        {
            using (DataTable sknMaterialsTable = new DataTable())
            {
                sknMaterialsTable.Columns.AddRange(
                    new[]
                {
                    new DataColumn(SkinMaterialIDText, typeof(SqlInt32)),
                    new DataColumn(MaterialSetIDText, typeof(SqlInt32)),
                    new DataColumn(DisplayNameIDText, typeof(SqlInt32)),
                    
                    // Obsolete since Galatea 1.0
                    new DataColumn(MaterialText, typeof(SqlString)),
                    new DataColumn(ColorHullText, typeof(SqlString)),
                    new DataColumn(ColorWindowText, typeof(SqlString)),
                    new DataColumn(ColorPrimaryText, typeof(SqlString)),
                    new DataColumn(ColorSecondaryText, typeof(SqlString)),
                });
                return sknMaterialsTable;
            }
        }
    }
}
