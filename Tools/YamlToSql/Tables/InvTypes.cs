using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace EVEMon.YamlToSql.Tables
{
    internal static class InvTypes
    {
        private const string InvTypesTableName = "invTypes";
        
        private const string GraphicIDText = "graphicID";
        private const string IconIDText = "iconID";
        private const string RadiusText = "radius";
        private const string SoundIDText = "soundID";

        /// <summary>
        /// Imports the type ids.
        /// </summary>
        internal static void ImportTypeIds(SqlConnection connection)
        {
            DateTime startTime = DateTime.Now;
            Util.ResetCounters();

            var yamlFile = YamlFilesConstants.typeIDs;
            var filePath = Util.CheckYamlFileExists(yamlFile);

            if (String.IsNullOrEmpty(filePath))
                return;

            CreateInvTypesColumns(connection);

            Console.WriteLine();
            Console.Write(@"Importing {0}... ", yamlFile);

            YamlMappingNode rNode = Util.ParseYamlFile(filePath);

            if (rNode == null)
            {
                Console.WriteLine(@"Unable to parse {0}.", yamlFile);
                return;
            }

            ImportInvTypesData(connection, rNode);

            Util.DisplayEndTime(startTime);

            Console.WriteLine();
        }

        /// <summary>
        /// Creates the inv types columns.
        /// </summary>
        /// <param name="connection">The connection.</param>
        private static void CreateInvTypesColumns(SqlConnection connection)
        {
            var command = new SqlCommand { Connection = connection };
            DataTable dataTable = connection.GetSchema("columns");

            if (dataTable.Select(String.Format("TABLE_NAME = '{0}'", InvTypesTableName)).Length == 0)
            {
                Console.WriteLine(@"Can't find table '{0}'.", InvTypesTableName);
                Environment.Exit(-1);
            }

            Database.CreateColumn(dataTable, command, InvTypesTableName, GraphicIDText, "int");
            Database.CreateColumn(dataTable, command, InvTypesTableName, IconIDText, "int");
            Database.CreateColumn(dataTable, command, InvTypesTableName, RadiusText, "float");
            Database.CreateColumn(dataTable, command, InvTypesTableName, SoundIDText, "int");
        }

        /// <summary>
        /// Imports the inv types data.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="rNode">The r node.</param>
        private static void ImportInvTypesData(SqlConnection connection,YamlMappingNode rNode)
        {
            var command = new SqlCommand { Connection = connection };

            using (var tx = connection.BeginTransaction())
            {
                command.Transaction = tx;

                try
                {
                    foreach (KeyValuePair<YamlNode, YamlNode> pair in rNode.Children)
                    {
                        Util.UpdatePercentDone(rNode.Count());

                        string graphicID = "null";
                        string iconID = "null";
                        string radius = "null";
                        string soundID = "null";

                        YamlMappingNode cNode = rNode.Children[pair.Key] as YamlMappingNode;

                        if (cNode == null)
                            continue;

                        YamlNode graphicIDNode = new YamlScalarNode(GraphicIDText);
                        if (cNode.Children.ContainsKey(graphicIDNode))
                            graphicID = ((YamlScalarNode)cNode.Children[graphicIDNode]).Value;

                        YamlNode iconIDNode = new YamlScalarNode(IconIDText);
                        if (cNode.Children.ContainsKey(iconIDNode))
                            iconID = ((YamlScalarNode)cNode.Children[iconIDNode]).Value;

                        YamlNode radiusNode = new YamlScalarNode(RadiusText);
                        if (cNode.Children.ContainsKey(radiusNode))
                            radius = ((YamlScalarNode)cNode.Children[radiusNode]).Value;

                        YamlNode soundIDNode = new YamlScalarNode(SoundIDText);
                        if (cNode.Children.ContainsKey(soundIDNode))
                            soundID = ((YamlScalarNode)cNode.Children[soundIDNode]).Value;

                        var parameters = new Dictionary<string, string>();
                        parameters["id"] = ((YamlScalarNode)pair.Key).Value;
                        parameters[GraphicIDText] = graphicID;
                        parameters[IconIDText] = iconID;
                        parameters[RadiusText] = radius;
                        parameters[SoundIDText] = soundID;
                        parameters["columnFilter"] = "typeID";

                        command.CommandText = Database.SqlUpdateCommandText(InvTypesTableName, parameters);
                        command.ExecuteNonQuery();
                    }

                    tx.Commit();
                }
                catch (SqlException e)
                {
                    tx.Rollback();
                    Console.WriteLine();
                    Console.WriteLine(@"Unable to execute SQL command: {0}", command.CommandText);
                    Console.WriteLine(e.Message);
                    Environment.Exit(-1);
                }
            }
        }
    }
}
