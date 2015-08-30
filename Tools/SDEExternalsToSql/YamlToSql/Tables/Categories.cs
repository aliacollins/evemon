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

        private const string NameText = "name";
        
        // invCategories
        private const string CategoryIDText = "categoryID";
        private const string CategoryNameText = "categoryName";
        private const string DescriptionText = "description";
        private const string IconIDText = "iconID";
        private const string PublishedText = "published";


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

            ImportTranslations();

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

                        YamlMappingNode cNode = pair.Value as YamlMappingNode;

                        if (cNode == null)
                            continue;

                        var categoriesNameNodes = cNode.Children.Keys.Any(key => key.ToString() == NameText)
                            ? cNode.Children[new YamlScalarNode(NameText)] as YamlMappingNode
                            : null;

                        Dictionary<string, string> parameters = new Dictionary<string, string>();
                        parameters[CategoryIDText] = pair.Key.ToString();
                        parameters[CategoryNameText] = categoriesNameNodes == null
                            ? cNode.Children.GetTextOrDefaultString(NameText, isUnicode: true)
                            : categoriesNameNodes.Children.GetTextOrDefaultString(Translations.EnglishLanguageIDText, isUnicode: true);
                        parameters[DescriptionText] = cNode.Children.GetTextOrDefaultString(DescriptionText, isUnicode: true);
                        parameters[IconIDText] = cNode.Children.GetTextOrDefaultString(IconIDText);
                        parameters[PublishedText] = cNode.Children.GetTextOrDefaultString(PublishedText);

                        if (categoriesNameNodes != null)
                            Translations.InsertTranslations(command, Translations.TranslationCategoriesID, pair.Key, categoriesNameNodes);

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

        /// <summary>
        /// Imports the translations.
        /// </summary>
        private static void ImportTranslations()
        {
            const string TableText = "dbo." + InvCategoriesTableName;
            var baseParameters = new Dictionary<string, string>();
            baseParameters[Translations.TcGroupIDText] = Translations.TranslationCategoriesGroupID;
            baseParameters[Translations.TcIDText] = Translations.TranslationCategoriesID;

            var parameters = new Dictionary<string, string>(baseParameters);
            parameters[Translations.SourceTableText] = "inventory.categoriesTx".GetTextOrDefaultString();
            parameters[Translations.DestinationTableText] = TableText.GetTextOrDefaultString();
            parameters[Translations.TranslatedKeyText] = CategoryNameText.GetTextOrDefaultString();
            parameters["id"] = parameters[Translations.SourceTableText];
            parameters["id2"] = parameters[Translations.TranslatedKeyText];
            parameters["columnFilter"] = Translations.SourceTableText;
            parameters["columnFilter2"] = Translations.TranslatedKeyText;

            Translations.ImportData(Translations.TranslationTableName, parameters);

            parameters = new Dictionary<string, string>(baseParameters);
            parameters[Translations.TableNameText] = TableText.GetTextOrDefaultString();
            parameters[Translations.ColumnNameText] = CategoryNameText.GetTextOrDefaultString();
            parameters[Translations.MasterIDText] = CategoryIDText.GetTextOrDefaultString();
            parameters["id"] = parameters[Translations.TcIDText];
            parameters["columnFilter"] = Translations.TcIDText;

            Translations.ImportData(Translations.TrnTranslationColumnsTableName, parameters);
        }
    }
}
