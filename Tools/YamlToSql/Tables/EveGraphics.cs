using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace EVEMon.YamlToSql.Tables
{
    internal static class EveGraphics
    {
        /// <summary>
        /// Imports the graphic ids.
        /// </summary>
        internal static void ImportGraphicIds(SqlConnection connection)
        {
            DateTime startTime = DateTime.Now;
            Util.ResetCounters();

            var yamlFile = YamlFilesConstants.graphicsIDs;
            var filePath = Util.CheckYamlFileExists(yamlFile);

            if (String.IsNullOrEmpty(filePath))
                return;

            var command = new SqlCommand { Connection = connection };
            DataTable dataTable = connection.GetSchema("columns");

            const string TableName = "eveGraphics";
            if (dataTable.Select(String.Format("TABLE_NAME = '{0}'", TableName)).Length == 0)
                Database.CreateTable(command, TableName);
            else
            {
                Database.DropTable(command, TableName);
                Database.CreateTable(command, TableName);
            }

            Console.WriteLine();
            Console.Write(@"Importing {0}... ", yamlFile);

            YamlMappingNode rNode = Util.ParseYamlFile(filePath);

            if (rNode == null)
            {
                Console.WriteLine(@"Unable to parse {0}.", yamlFile);
                return;
            }

            const string GraphicFileText = "graphicFile";
            const string DescriptionText = "description";
            const string ObsoleteText = "obsolete";
            const string GraphicTypeText = "graphicType";
            const string CollidableText = "collidable";
            const string DirectoryIDText = "directoryID";
            const string GraphicNameText = "graphicName";
            const string GfxRaceIDText = "gfxRaceID";
            const string ColorSchemeText = "colorScheme";

            using (var tx = connection.BeginTransaction())
            {
                command.Transaction = tx;
                try
                {
                    foreach (KeyValuePair<YamlNode, YamlNode> pair in rNode.Children)
                    {
                        Util.UpdatePercentDone(rNode.Count());

                        string graphicFile = String.Format("'{0}'", String.Empty);
                        string description = String.Format("'{0}'", String.Empty);
                        string obsolete = "0";
                        string graphicType = "null";
                        string collidable = "null";
                        string directoryID = "null";
                        string graphicName = String.Format("'{0}'", String.Empty);
                        string gfxRaceID = "null";
                        string colorScheme = "null";

                        YamlMappingNode cNode = rNode.Children[pair.Key] as YamlMappingNode;

                        if (cNode == null)
                            continue;

                        YamlNode graphicFileNode = new YamlScalarNode(GraphicFileText);
                        if (cNode.Children.ContainsKey(graphicFileNode))
                        {
                            graphicFile = String.Format("'{0}'",
                                ((YamlScalarNode)cNode.Children[graphicFileNode]).Value.Replace("'", "''"));
                        }

                        YamlNode descriptionNode = new YamlScalarNode(DescriptionText);
                        if (cNode.Children.ContainsKey(descriptionNode))
                        {
                            description = String.Format("'{0}'",
                                ((YamlScalarNode)cNode.Children[descriptionNode]).Value.Replace("'", "''"));
                        }

                        YamlNode obsoleteNode = new YamlScalarNode(ObsoleteText);
                        if (cNode.Children.ContainsKey(obsoleteNode))
                        {
                            obsolete = ((YamlScalarNode)cNode.Children[obsoleteNode]).Value == "true"
                                ? "1"
                                : "0";
                        }

                        YamlNode graphicTypeNode = new YamlScalarNode(GraphicTypeText);
                        if (cNode.Children.ContainsKey(graphicTypeNode))
                        {
                            graphicType = String.Format("'{0}'",
                                ((YamlScalarNode)cNode.Children[graphicTypeNode]).Value.Replace("'", "''"));
                        }

                        YamlNode directoryIDNode = new YamlScalarNode(DirectoryIDText);
                        if (cNode.Children.ContainsKey(directoryIDNode))
                            directoryID = ((YamlScalarNode)cNode.Children[directoryIDNode]).Value;

                        YamlNode collidableNode = new YamlScalarNode(CollidableText);
                        if (cNode.Children.ContainsKey(collidableNode))
                        {
                            collidable = ((YamlScalarNode)cNode.Children[collidableNode]).Value == "true"
                                ? "1"
                                : "null";
                        }

                        YamlNode graphicNameNode = new YamlScalarNode(GraphicNameText);
                        if (cNode.Children.ContainsKey(graphicNameNode))
                        {
                            graphicName = String.Format("'{0}'",
                                ((YamlScalarNode)cNode.Children[graphicNameNode]).Value.Replace("'", "''"));
                        }

                        YamlNode gfxRaceIDNode = new YamlScalarNode(GfxRaceIDText);
                        if (cNode.Children.ContainsKey(gfxRaceIDNode))
                        {
                            gfxRaceID = String.Format("'{0}'",
                                ((YamlScalarNode)cNode.Children[gfxRaceIDNode]).Value.Replace("'", "''"));
                        }

                        YamlNode colorSchemeNode = new YamlScalarNode(ColorSchemeText);
                        if (cNode.Children.ContainsKey(colorSchemeNode))
                        {
                            colorScheme = String.Format("'{0}'",
                                ((YamlScalarNode)cNode.Children[colorSchemeNode]).Value.Replace("'", "''"));
                        }

                        var parameters = new Dictionary<string, string>();
                        parameters["graphicID"] = ((YamlScalarNode)pair.Key).Value;
                        parameters[GraphicFileText] = graphicFile;
                        parameters[DescriptionText] = description;
                        parameters[ObsoleteText] = obsolete;
                        parameters[GraphicTypeText] = graphicType;
                        parameters[DirectoryIDText] = directoryID;
                        parameters[CollidableText] = collidable;
                        parameters[GraphicNameText] = graphicName;
                        parameters[GfxRaceIDText] = gfxRaceID;
                        parameters[ColorSchemeText] = colorScheme;

                        command.CommandText = Database.SqlInsertCommandText(TableName, parameters);
                        command.ExecuteNonQuery();
                    }

                    tx.Commit();
                    Util.DisplayEndTime(startTime);
                }
                catch (SqlException e)
                {
                    tx.Rollback();
                    Console.WriteLine();
                    Console.WriteLine(@"Unable to execute SQL command: {0}", command.CommandText);
                    Console.WriteLine(e.Message);
                }
            }

            Console.WriteLine();
        }
    }
}
