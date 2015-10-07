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
    internal static class Skins
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

        private static SqlConnectionProvider s_sqlConnectionProvider;
        private static bool s_isClosing;

        /// <summary>
        /// Initializes the <see cref="Util"/> class.
        /// </summary>
        static Skins()
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
        /// Imports the skins.
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

            string yamlFile = YamlFilesConstants.Skins;
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

            s_sqlConnectionProvider.DropAndCreateTable(SknSkinsTableName);

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

            DataTable sknSkinsTable = GetSknSkinsDataTable();

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

            s_sqlConnectionProvider.ImportDataBulk(SknSkinsTableName, sknSkinsTable);

            Util.UpdatePercentDone(sknSkinsTable.Rows.Count);
        }

        /// <summary>
        /// Gets the data table for the sknSkins table.
        /// </summary>
        /// <returns></returns>
        private static DataTable GetSknSkinsDataTable()
        {
            using (DataTable sknSkinsTable = new DataTable())
            {
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
                return sknSkinsTable;
            }
        }
    }
}
