using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace EVEMon.YamlToSql.Tables
{
    internal static class EveIcons
    {
        private const string EveIconsTableName = "eveIcons";
        
        private const string IconFileText = "iconFile";
        private const string DescriptionText = "description";

        private const string StringEmpty = "''";

        /// <summary>
        /// Imports the icon ids.
        /// </summary>
        internal static void ImportIconIds(SqlConnection connection)
        {
            DateTime startTime = DateTime.Now;
            Util.ResetCounters();

            var yamlFile = YamlFilesConstants.iconIDS;
            var filePath = Util.CheckYamlFileExists(yamlFile);

            if (String.IsNullOrEmpty(filePath))
                return;

            CreateEveIconsTable(connection);

            Console.WriteLine();
            Console.Write(@"Importing {0}... ", yamlFile);

            YamlMappingNode rNode = Util.ParseYamlFile(filePath);

            if (rNode == null)
            {
                Console.WriteLine(@"Unable to parse {0}.", yamlFile);
                return;
            }

            ImportEveIconsData(connection, rNode);

            Util.DisplayEndTime(startTime);

            Console.WriteLine();
        }

        /// <summary>
        /// Imports the eve icons data.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="rNode">The r node.</param>
        private static void ImportEveIconsData(SqlConnection connection, YamlMappingNode rNode)
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
                        parameters[IconFileText] = String.Format("'{0}'",
                            cNode.Children[new YamlScalarNode(IconFileText)].ToString().Replace("'", StringEmpty));
                        parameters[DescriptionText] = cNode.Children.Keys.Any(key => key.ToString() == DescriptionText)
                            ? String.Format("'{0}'",
                                cNode.Children[new YamlScalarNode(DescriptionText)].ToString().Replace("'", StringEmpty))
                            : StringEmpty;

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
                    Environment.Exit(-1);
                }
            }
        }

        /// <summary>
        /// Creates the eve icons table.
        /// </summary>
        /// <param name="connection">The connection.</param>
        private static void CreateEveIconsTable(SqlConnection connection)
        {
            var command = new SqlCommand { Connection = connection };
            DataTable dataTable = connection.GetSchema("columns");

            if (dataTable.Select(String.Format("TABLE_NAME = '{0}'", EveIconsTableName)).Length == 0)
                Database.CreateTable(command, EveIconsTableName);
            else
            {
                Database.DropTable(command, EveIconsTableName);
                Database.CreateTable(command, EveIconsTableName);
            }
        }
    }
}
