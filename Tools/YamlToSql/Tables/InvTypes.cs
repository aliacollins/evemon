using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace EVEMon.YamlToSql.Tables
{
    internal static class InvTypes
    {
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

            var command = new SqlCommand { Connection = connection };
            DataTable dataTable = connection.GetSchema("columns");

            const string TableName = "invTypes";
            if (dataTable.Select(String.Format("TABLE_NAME = '{0}'", TableName)).Length == 0)
            {
                Console.WriteLine(@"Can't find table '{0}'.", TableName);
                return;
            }

            Console.WriteLine();
            Console.Write(@"Importing {0}... ", yamlFile);

            YamlStream yStream = new YamlStream();
            yStream.Load(new StreamReader(filePath));
            YamlMappingNode rNode = yStream.Documents.First().RootNode as YamlMappingNode;

            if (rNode == null)
            {
                Console.WriteLine(@"Unable to parse {0}.", yamlFile);
                return;
            }

            const string GraphicIDText = "graphicID";
            const string IconIDText = "iconID";
            const string RadiusText = "radius";
            const string SoundIDText = "soundID";

            Database.CreateColumn(dataTable, command, TableName, GraphicIDText, "int");
            Database.CreateColumn(dataTable, command, TableName, IconIDText, "int");
            Database.CreateColumn(dataTable, command, TableName, RadiusText, "float");
            Database.CreateColumn(dataTable, command, TableName, SoundIDText, "int");

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

                        command.CommandText = Database.SqlUpdateCommandText(TableName, parameters);
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
