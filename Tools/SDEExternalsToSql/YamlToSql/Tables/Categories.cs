using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace EVEMon.SDEExternalsToSql.YamlToSql.Tables
{
    internal static class Categories
    {
        private const string InvCategoriesTableName = "invCategories";

        private const string CategoryIDText = "categoryID";
        private const string CategoryNameText = "categoryName";
        private const string DescriptionText = "description";
        private const string IconIDText = "iconID";
        private const string PublishedText = "published";

        private const string NameText = "name";

        private const string EnglishLanguageIDText = "en";

        /// <summary>
        /// Imports the categories ids.
        /// </summary>
        internal static void Import()
        {
            DateTime startTime = DateTime.Now;
            Util.ResetCounters();

            var yamlFile = YamlFilesConstants.categoryIDs;
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

            Database.CreateTable(InvCategoriesTableName);

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
            using (IDbCommand command = new SqlCommand { Connection = Database.SqlConnection })
            {
                command.Transaction = Database.SqlConnection.BeginTransaction();

                try
                {
                    foreach (KeyValuePair<YamlNode, YamlNode> pair in rNode.Children)
                    {
                        Util.UpdatePercentDone(rNode.Count());

                        YamlMappingNode cNode = rNode.Children[pair.Key] as YamlMappingNode;

                        if (cNode == null)
                            continue;

                        var categoriesNameNodes = cNode.Children.Keys.Any(key => key.ToString() == NameText)
                            ? cNode.Children[new YamlScalarNode(NameText)] as YamlMappingNode
                            : null;

                        Dictionary<string, string> parameters = new Dictionary<string, string>();
                        parameters[CategoryIDText] = pair.Key.ToString();
                        parameters[CategoryNameText] = cNode.Children.Keys.Any(key => key.ToString() == NameText)
                            ? String.Format("N'{0}'", (categoriesNameNodes == null
                                ? cNode.Children[new YamlScalarNode(NameText)].ToString().Replace("'", Database.StringEmpty)
                                : categoriesNameNodes.Children.Keys.Any(key => key.ToString() == EnglishLanguageIDText)
                                    ? categoriesNameNodes.Children[new YamlScalarNode(EnglishLanguageIDText)].ToString()
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
                        parameters[PublishedText] = cNode.Children.Keys.Any(key => key.ToString() == PublishedText)
                            ? Convert.ToByte(Convert.ToBoolean(cNode.Children[new YamlScalarNode(PublishedText)].ToString()))
                                .ToString()
                            : Database.Null;

                        command.CommandText = Database.SqlInsertCommandText(InvCategoriesTableName, parameters);
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
