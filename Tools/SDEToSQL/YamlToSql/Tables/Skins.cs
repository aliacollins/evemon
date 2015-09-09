using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace EVEMon.SDEToSQL.YamlToSQL.Tables
{
    internal class Skins
    {
        private const string SknSkinsTableName = "sknSkins";

        private const string SkinIDText = "skinID";
        private const string InternalNameText = "internalName";
        private const string SkinMaterialIDText = "skinMaterialID";
        private const string TypesText = "types";
        private const string TypeIDText = "typeID";
        private const string AllowCCPDevsText = "allowCCPDevs";
        private const string VisibleSerenityText = "visibleSerenity";
        private const string VisibleTranquilityText = "visibleTranquility";

        /// <summary>
        /// Imports the skins.
        /// </summary>
        internal static void Import()
        {
            if (Program.IsClosing)
                return;

            Stopwatch stopwatch = Stopwatch.StartNew();
            Util.ResetCounters();

            string yamlFile = YamlFilesConstants.skins;
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

            Database.CreateTable(SknSkinsTableName);

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

            DataTable sknSkinsTable = new DataTable();
            sknSkinsTable.Columns.AddRange(
                new[]
                {
                    new DataColumn(SkinIDText, typeof(SqlInt32)),
                    new DataColumn(InternalNameText, typeof(SqlString)),
                    new DataColumn(SkinMaterialIDText, typeof(SqlInt32)),
                    new DataColumn(TypeIDText, typeof(SqlInt32)),
                    new DataColumn(AllowCCPDevsText, typeof(SqlBoolean)),
                    new DataColumn(VisibleSerenityText, typeof(SqlBoolean)),
                    new DataColumn(VisibleTranquilityText, typeof(SqlBoolean)),
                });

            int total = rNode.Count();
            total = (int)Math.Ceiling(total + (total * 0.01));

            foreach (KeyValuePair<YamlNode, YamlNode> pair in rNode.Children)
            {
                Util.UpdatePercentDone(total);

                YamlMappingNode cNode = pair.Value as YamlMappingNode;

                if (cNode == null)
                    continue;

                DataRow row = sknSkinsTable.NewRow();
                row[SkinIDText] = SqlInt32.Parse(pair.Key.ToString());
                row[InternalNameText] = cNode.Children.GetSqlTypeOrDefault<SqlString>(InternalNameText, defaultValue: "");
                row[SkinMaterialIDText] = cNode.Children.GetSqlTypeOrDefault<SqlInt32>(SkinMaterialIDText);
                row[AllowCCPDevsText] = cNode.Children.GetSqlTypeOrDefault<SqlBoolean>(AllowCCPDevsText, defaultValue: "0");
                row[VisibleSerenityText] = cNode.Children.GetSqlTypeOrDefault<SqlBoolean>(VisibleSerenityText, defaultValue: "0");
                row[VisibleTranquilityText] = cNode.Children.GetSqlTypeOrDefault<SqlBoolean>(VisibleTranquilityText,defaultValue: "0");

                YamlNode typesNode = new YamlScalarNode(TypesText);
                if (cNode.Children.ContainsKey(typesNode))
                {
                    YamlSequenceNode typeIDsNode = cNode.Children[typesNode] as YamlSequenceNode;

                    row[TypeIDText] = typeIDsNode != null
                        ? typeIDsNode.Count() == 1
                            ? SqlInt32.Parse(typeIDsNode.Children.First().ToString())
                            : SqlInt32.Null
                        : SqlInt32.Null;
                }

                sknSkinsTable.Rows.Add(row);
            }

            Database.ImportDataBulk(SknSkinsTableName, sknSkinsTable);

            Util.UpdatePercentDone(sknSkinsTable.Rows.Count);
        }
    }
}
