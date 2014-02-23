using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace EVEMon.SDEExternalsToSql.YamlToSql.Tables
{
    internal static class EveIcons
    {
        private const string EveIconsTableName = "eveIcons";
        
        private const string IconFileText = "iconFile";
        private const string DescriptionText = "description";

        /// <summary>
        /// Imports the icon ids.
        /// </summary>
        internal static void Import(SqlConnection connection)
        {
            DateTime startTime = DateTime.Now;
            Util.ResetCounters();

            var yamlFile = YamlFilesConstants.iconIDS;
            var filePath = Util.CheckYamlFileExists(yamlFile);

            if (String.IsNullOrEmpty(filePath))
                return;

            YamlMappingNode rNode = Util.ParseYamlFile(filePath);

            if (rNode == null)
            {
                Console.WriteLine(@"Unable to parse {0}.", yamlFile);
                return;
            }

            Console.WriteLine();
            Console.Write(@"Importing {0}... ", yamlFile);

            Database.CreateTable(connection, EveIconsTableName);

            ImportData(connection, rNode);

            Util.DisplayEndTime(startTime);

            Console.WriteLine();
        }

        /// <summary>
        /// Imports the data.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="rNode">The r node.</param>
        private static void ImportData(SqlConnection connection, YamlMappingNode rNode)
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

                        YamlMappingNode cNode = rNode.Children[pair.Key] as YamlMappingNode;

                        if (cNode == null)
                            continue;

                        var parameters = new Dictionary<string, string>();
                        parameters["iconID"] = pair.Key.ToString();
                        parameters[IconFileText] = cNode.Children.Keys.Any(key => key.ToString() == IconFileText)
                            ? String.Format("'{0}'",
                                cNode.Children[new YamlScalarNode(IconFileText)].ToString().Replace("'", Database.StringEmpty))
                            : Database.StringEmpty;
                        parameters[DescriptionText] = cNode.Children.Keys.Any(key => key.ToString() == DescriptionText)
                            ? String.Format("'{0}'",
                                cNode.Children[new YamlScalarNode(DescriptionText)].ToString().Replace("'", Database.StringEmpty))
                            : Database.StringEmpty;

                        command.CommandText = Database.SqlInsertCommandText(EveIconsTableName, parameters);
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
                    Console.ReadLine();
                    Environment.Exit(-1);
                }
            }
        }
    }
}
