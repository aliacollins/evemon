using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace EVEMon.SDEExternalsToSql.YamlToSql.Tables
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

        /// <summary>
        /// Imports the graphic ids.
        /// </summary>
        internal static void Import()
        {
            DateTime startTime = DateTime.Now;
            Util.ResetCounters();

            var yamlFile = YamlFilesConstants.graphicsIDs;
            var filePath = Util.CheckYamlFileExists(yamlFile);

            if (String.IsNullOrEmpty(filePath))
                return;

            var text = String.Format("Parsing {0}... ", yamlFile);
            Console.Write(text);
            YamlMappingNode rNode = Util.ParseYamlFile(filePath);

            if (rNode == null)
            {
                Console.WriteLine(@"Unable to parse {0}.", yamlFile);
                return;
            }

            Console.SetCursorPosition(Console.CursorLeft - text.Length, Console.CursorTop);
            Console.Write(@"Importing {0}... ", yamlFile);

            Database.CreateTable(EveGraphicsTableName);

            ImportData(rNode);

            Util.DisplayEndTime(startTime);

            Console.WriteLine();
        }

        /// <summary>
        /// Imports the data.
        /// </summary>
        /// <param name="rNode">The r node.</param>
        private static void ImportData(YamlMappingNode rNode)
        {
            using (IDbCommand command = new SqlCommand(
                String.Empty,
                Database.SqlConnection,
                Database.SqlConnection.BeginTransaction()))
            {
                try
                {
                    foreach (KeyValuePair<YamlNode, YamlNode> pair in rNode.Children)
                    {
                        Util.UpdatePercentDone(rNode.Count());

                        YamlMappingNode cNode = rNode.Children[pair.Key] as YamlMappingNode;

                        if (cNode == null)
                            continue;

                        Dictionary<string, string> parameters = new Dictionary<string, string>();
                        parameters[GraphicIDText] = pair.Key.ToString();
                        parameters[GraphicFileText] = cNode.Children.Keys.Any(key => key.ToString() == GraphicFileText)
                            ? String.Format("'{0}'",
                                cNode.Children[new YamlScalarNode(GraphicFileText)].ToString().Replace("'", Database.StringEmpty))
                            : Database.StringEmpty;
                        parameters[DescriptionText] = cNode.Children.Keys.Any(key => key.ToString() == DescriptionText)
                            ? String.Format("N'{0}'",
                                cNode.Children[new YamlScalarNode(DescriptionText)].ToString().Replace("'", Database.StringEmpty))
                            : Database.StringEmpty;
                        parameters[ObsoleteText] = cNode.Children.Keys.Any(key => key.ToString() == ObsoleteText)
                            ? Convert.ToByte(Convert.ToBoolean(cNode.Children[new YamlScalarNode(ObsoleteText)].ToString())).ToString()
                            : "0";
                        parameters[GraphicTypeText] = cNode.Children.Keys.Any(key => key.ToString() == GraphicTypeText)
                            ? String.Format("'{0}'",
                                cNode.Children[new YamlScalarNode(GraphicTypeText)].ToString().Replace("'", Database.StringEmpty))
                            : Database.Null;
                        parameters[DirectoryIDText] = cNode.Children.Keys.Any(key => key.ToString() == DirectoryIDText)
                            ? cNode.Children[new YamlScalarNode(DirectoryIDText)].ToString()
                            : Database.Null;
                        parameters[CollidableText] = cNode.Children.Keys.Any(key => key.ToString() == CollidableText)
                            ? Convert.ToByte(Convert.ToBoolean(cNode.Children[new YamlScalarNode(CollidableText)].ToString())).ToString()
                            : Database.Null;
                        parameters[GraphicNameText] = cNode.Children.Keys.Any(key => key.ToString() == GraphicNameText)
                            ? String.Format("N'{0}'",
                                cNode.Children[new YamlScalarNode(GraphicNameText)].ToString().Replace("'", Database.StringEmpty))
                            : Database.StringEmpty;
                        parameters[GfxRaceIDText] = cNode.Children.Keys.Any(key => key.ToString() == GfxRaceIDText)
                            ? String.Format("'{0}'",
                                cNode.Children[new YamlScalarNode(GfxRaceIDText)].ToString().Replace("'", Database.StringEmpty))
                            : Database.Null;
                        parameters[ColorSchemeText] = cNode.Children.Keys.Any(key => key.ToString() == ColorSchemeText)
                            ? String.Format("'{0}'",
                                cNode.Children[new YamlScalarNode(ColorSchemeText)].ToString().Replace("'", Database.StringEmpty))
                            : Database.Null;
                        parameters[SofHullNameText] = cNode.Children.Keys.Any(key => key.ToString() == SofHullNameText)
                            ? String.Format("'{0}'",
                                cNode.Children[new YamlScalarNode(SofHullNameText)].ToString().Replace("'", Database.StringEmpty))
                            : Database.Null;

                        command.CommandText = Database.SqlInsertCommandText(EveGraphicsTableName, parameters);
                        command.ExecuteNonQuery();
                    }

                    command.Transaction.Commit();
                }
                catch (SqlException e)
                {
                    command.Transaction.Rollback();
                    Util.HandleException(command, e);
                }
            }
        }
    }
}
