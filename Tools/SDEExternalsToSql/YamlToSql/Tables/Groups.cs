using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace EVEMon.SDEExternalsToSql.YamlToSql.Tables
{
    internal static class Groups
    {
        private const string InvGroupsTableName = "invGroups";

        private const string GroupIDText = "groupID";
        private const string CategoryIDText = "categoryID";
        private const string GroupNameText = "groupName";
        private const string DescriptionText = "description";
        private const string IconIDText = "iconID";
        private const string UseBasePriceText = "useBasePrice";
        private const string AllowManufactureText = "allowManufacture";
        private const string AllowRecyclerText = "allowRecycler";
        private const string AnchoredText = "anchored";
        private const string AnchorableText = "anchorable";
        private const string FittableNonSingletonText = "fittableNonSingleton";
        private const string PublishedText = "published";

        private const string NameText = "name";

        private const string EnglishLanguageIDText = "en";

        /// <summary>
        /// Imports the groups ids.
        /// </summary>
        internal static void Import()
        {
            DateTime startTime = DateTime.Now;
            Util.ResetCounters();

            var yamlFile = YamlFilesConstants.groupIDs;
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

            Database.CreateTable(InvGroupsTableName);

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
            using (SqlTransaction tx = Database.SqlConnection.BeginTransaction())
            {
                IDbCommand command = new SqlCommand { Connection = Database.SqlConnection, Transaction = tx };
                try
                {
                    foreach (KeyValuePair<YamlNode, YamlNode> pair in rNode.Children)
                    {
                        Util.UpdatePercentDone(rNode.Count());

                        YamlMappingNode cNode = rNode.Children[pair.Key] as YamlMappingNode;

                        if (cNode == null)
                            continue;

                        var groupNameNodes = cNode.Children.Keys.Any(key => key.ToString() == NameText)
                            ? cNode.Children[new YamlScalarNode(NameText)] as YamlMappingNode
                            : null;

                        Dictionary<string, string> parameters = new Dictionary<string, string>();
                        parameters[GroupIDText] = pair.Key.ToString();
                        parameters[CategoryIDText] = cNode.Children.Keys.Any(key => key.ToString() == CategoryIDText)
                            ? cNode.Children[new YamlScalarNode(CategoryIDText)].ToString()
                            : Database.Null;
                        parameters[GroupNameText] = cNode.Children.Keys.Any(key => key.ToString() == NameText)
                            ? String.Format("N'{0}'", (groupNameNodes == null
                                ? cNode.Children[new YamlScalarNode(NameText)].ToString().Replace("'", Database.StringEmpty)
                                : groupNameNodes.Children.Keys.Any(key => key.ToString() == EnglishLanguageIDText)
                                    ? groupNameNodes.Children[new YamlScalarNode(EnglishLanguageIDText)].ToString()
                                        .Replace("'", Database.StringEmpty)
                                    : Database.Null))
                            : Database.Null;
                        parameters[DescriptionText] = cNode.Children.Keys.Any(key => key.ToString() == DescriptionText)
                            ? String.Format("N'{0}'",
                                cNode.Children[new YamlScalarNode(DescriptionText)].ToString().Replace("'", Database.StringEmpty))
                            : Database.Null;
                        parameters[IconIDText] = cNode.Children.Keys.Any(key => key.ToString() == IconIDText)
                            ? cNode.Children[new YamlScalarNode(IconIDText)].ToString()
                            : Database.Null;
                        parameters[UseBasePriceText] = cNode.Children.Keys.Any(key => key.ToString() == UseBasePriceText)
                            ? Convert.ToByte(Convert.ToBoolean(cNode.Children[new YamlScalarNode(UseBasePriceText)].ToString()))
                                .ToString()
                            : Database.Null;
                        parameters[AllowManufactureText] = cNode.Children.Keys.Any(key => key.ToString() == AllowManufactureText)
                            ? Convert.ToByte(Convert.ToBoolean(cNode.Children[new YamlScalarNode(AllowManufactureText)].ToString()))
                                .ToString()
                            : Database.Null;
                        parameters[AllowRecyclerText] = cNode.Children.Keys.Any(key => key.ToString() == AllowRecyclerText)
                            ? Convert.ToByte(Convert.ToBoolean(cNode.Children[new YamlScalarNode(AllowRecyclerText)].ToString()))
                                .ToString()
                            : Database.Null;
                        parameters[AnchoredText] = cNode.Children.Keys.Any(key => key.ToString() == AnchoredText)
                            ? Convert.ToByte(Convert.ToBoolean(cNode.Children[new YamlScalarNode(AnchoredText)].ToString()))
                                .ToString()
                            : Database.Null;
                        parameters[AnchorableText] = cNode.Children.Keys.Any(key => key.ToString() == AnchorableText)
                            ? Convert.ToByte(Convert.ToBoolean(cNode.Children[new YamlScalarNode(AnchorableText)].ToString()))
                                .ToString()
                            : Database.Null;
                        parameters[FittableNonSingletonText] =
                            cNode.Children.Keys.Any(key => key.ToString() == FittableNonSingletonText)
                                ? Convert.ToByte(
                                    Convert.ToBoolean(cNode.Children[new YamlScalarNode(FittableNonSingletonText)].ToString()))
                                    .ToString()
                                : Database.Null;
                        parameters[PublishedText] = cNode.Children.Keys.Any(key => key.ToString() == PublishedText)
                            ? Convert.ToByte(Convert.ToBoolean(cNode.Children[new YamlScalarNode(PublishedText)].ToString()))
                                .ToString()
                            : Database.Null;

                        command.CommandText = Database.SqlInsertCommandText(InvGroupsTableName, parameters);
                        command.ExecuteNonQuery();
                    }

                    tx.Commit();
                }
                catch (SqlException e)
                {
                    tx.Rollback();
                    Util.HandleException(command, e);
                }
            }
        }
    }
}
