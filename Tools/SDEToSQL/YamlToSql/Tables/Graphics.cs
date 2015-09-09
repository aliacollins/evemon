using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace EVEMon.SDEToSQL.YamlToSQL.Tables
{
    internal class Graphics
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

        /// <summary>
        /// Imports the graphic ids.
        /// </summary>
        internal static void Import()
        {
            if (Program.IsClosing)
                return;

            Stopwatch stopwatch = Stopwatch.StartNew();
            Util.ResetCounters();

            string yamlFile = YamlFilesConstants.graphicsIDs;
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

            Database.DropAndCreateTable(EveGraphicsTableName);

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

            DataTable eveGraphicsTable = new DataTable();
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

            Database.ImportDataBulk(EveGraphicsTableName, eveGraphicsTable);

            Util.UpdatePercentDone(eveGraphicsTable.Rows.Count);
        }
    }
}
