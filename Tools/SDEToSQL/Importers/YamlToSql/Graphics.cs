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
    internal static class Graphics
    {
        private const string EveGraphicsTableName = "eveGraphics";

        private const string GraphicIDText = "graphicID";
        private const string GraphicFileText = "graphicFile";
        private const string DescriptionText = "description";
        private const string ObsoleteText = "obsolete";
        private const string GraphicTypeText = "graphicType";
        private const string CollidableText = "collidable";
        private const string DirectoryIDText = "directoryID";
        private const string GraphicNameText = "graphicName";
        private const string GfxRaceIDText = "gfxRaceID";
        private const string ColorSchemeText = "colorScheme";
        private const string SofHullNameText = "sofHullName";

        private static SqlConnectionProvider s_sqlConnectionProvider;
        private static bool s_isClosing;

        /// <summary>
        /// Initializes the <see cref="Util"/> class.
        /// </summary>
        static Graphics()
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
        /// Imports the graphic ids.
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

            string yamlFile = YamlFilesConstants.GraphicsIDs;
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

            s_sqlConnectionProvider.DropAndCreateTable(EveGraphicsTableName);

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

            DataTable eveGraphicsTable = GetEveGraphicsDataTable();

            int total = rNode.Count();
            total = (int)Math.Ceiling(total + (total * 0.01));

            foreach (KeyValuePair<YamlNode, YamlNode> pair in rNode.Children)
            {
                Util.UpdatePercentDone(total);

                YamlMappingNode cNode = pair.Value as YamlMappingNode;

                if (cNode == null)
                    continue;

                DataRow row = eveGraphicsTable.NewRow();
                row[GraphicIDText] = SqlInt32.Parse(pair.Key.ToString());
                row[GraphicFileText] = cNode.Children.GetSqlTypeOrDefault<SqlString>(GraphicFileText, defaultValue: "");
                row[DescriptionText] = cNode.Children.GetSqlTypeOrDefault<SqlString>(DescriptionText, defaultValue: "");
                row[ObsoleteText] = cNode.Children.GetSqlTypeOrDefault<SqlBoolean>(ObsoleteText, defaultValue: "0");
                row[GraphicTypeText] = cNode.Children.GetSqlTypeOrDefault<SqlString>(GraphicTypeText);
                row[CollidableText] = cNode.Children.GetSqlTypeOrDefault<SqlBoolean>(CollidableText);
                row[DirectoryIDText] = cNode.Children.GetSqlTypeOrDefault<SqlInt32>(DirectoryIDText);
                row[GraphicNameText] = cNode.Children.GetSqlTypeOrDefault<SqlString>(GraphicNameText, defaultValue: "");
                row[GfxRaceIDText] = cNode.Children.GetSqlTypeOrDefault<SqlString>(GfxRaceIDText);
                row[ColorSchemeText] = cNode.Children.GetSqlTypeOrDefault<SqlString>(ColorSchemeText);
                row[SofHullNameText] = cNode.Children.GetSqlTypeOrDefault<SqlString>(SofHullNameText);

                eveGraphicsTable.Rows.Add(row);
            }

            s_sqlConnectionProvider.ImportDataBulk(EveGraphicsTableName, eveGraphicsTable);

            Util.UpdatePercentDone(eveGraphicsTable.Rows.Count);
        }

        /// <summary>
        /// Gets the data table for the eveGraphics table.
        /// </summary>
        /// <returns></returns>
        private static DataTable GetEveGraphicsDataTable()
        {
            using (DataTable eveGraphicsTable = new DataTable())
            {
                eveGraphicsTable.Columns.AddRange(
                    new[]
                {
                    new DataColumn(GraphicIDText, typeof(SqlInt32)),
                    new DataColumn(GraphicFileText, typeof(SqlString)),
                    new DataColumn(DescriptionText, typeof(SqlString)),
                    new DataColumn(ObsoleteText, typeof(SqlBoolean)),
                    new DataColumn(GraphicTypeText, typeof(SqlString)),
                    new DataColumn(CollidableText, typeof(SqlBoolean)),
                    new DataColumn(DirectoryIDText, typeof(SqlInt32)),
                    new DataColumn(GraphicNameText, typeof(SqlString)),
                    new DataColumn(GfxRaceIDText, typeof(SqlString)),
                    new DataColumn(ColorSchemeText, typeof(SqlString)),
                    new DataColumn(SofHullNameText, typeof(SqlString)),
                });
                return eveGraphicsTable;
            }
        }
    }
}
