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

                        var groupNameNodes = cNode.Children.Keys.Any(key => key.ToString() == NameText)
                            ? cNode.Children[new YamlScalarNode(NameText)] as YamlMappingNode
                            : null;

                        Dictionary<string, string> parameters = new Dictionary<string, string>();
                        parameters[GroupIDText] = pair.Key.ToString();
                        parameters[CategoryIDText] = cNode.Children.GetTextOrDefaultString(CategoryIDText);
                        parameters[GroupNameText] = groupNameNodes == null
                            ? cNode.Children.GetTextOrDefaultString(NameText, isUnicode: true)
                            : groupNameNodes.Children.GetTextOrDefaultString(Translations.EnglishLanguageIDText);
                        parameters[DescriptionText] = cNode.Children.GetTextOrDefaultString(DescriptionText, isUnicode: true);
                        parameters[IconIDText] = cNode.Children.GetTextOrDefaultString(IconIDText);
                        parameters[UseBasePriceText] = cNode.Children.GetTextOrDefaultString(UseBasePriceText);
                        parameters[AllowManufactureText] = cNode.Children.GetTextOrDefaultString(AllowManufactureText);
                        parameters[AllowRecyclerText] = cNode.Children.GetTextOrDefaultString(AllowRecyclerText);
                        parameters[AnchoredText] = cNode.Children.GetTextOrDefaultString(AnchoredText);
                        parameters[AnchorableText] = cNode.Children.GetTextOrDefaultString(AnchorableText);
                        parameters[FittableNonSingletonText] =
                            cNode.Children.GetTextOrDefaultString(FittableNonSingletonText);
                        parameters[PublishedText] = cNode.Children.GetTextOrDefaultString(PublishedText);

                        if (groupNameNodes != null)
                            Translations.InsertTranslations(command, Translations.TranslationGroupsID, pair.Key, groupNameNodes);

                        command.CommandText = Database.SqlInsertCommandText(InvGroupsTableName, parameters);
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
            const string TableText = "dbo." + InvGroupsTableName;
            var baseParameters = new Dictionary<string, string>();
            baseParameters[Translations.TcGroupIDText] = Translations.TranslationGroupsGroupID;
            baseParameters[Translations.TcIDText] = Translations.TranslationGroupsID;

            var parameters = new Dictionary<string, string>(baseParameters);
            parameters[Translations.SourceTableText] = "inventory.groupsTx".GetTextOrDefaultString();
            parameters[Translations.DestinationTableText] = TableText.GetTextOrDefaultString();
            parameters[Translations.TranslatedKeyText] = GroupNameText.GetTextOrDefaultString();
            parameters["id"] = parameters[Translations.SourceTableText];
            parameters["id2"] = parameters[Translations.TranslatedKeyText];
            parameters["columnFilter"] = Translations.SourceTableText;
            parameters["columnFilter2"] = Translations.TranslatedKeyText;

            Translations.ImportData(Translations.TranslationTableName, parameters);

            parameters = new Dictionary<string, string>(baseParameters);
            parameters[Translations.TableNameText] = TableText.GetTextOrDefaultString();
            parameters[Translations.ColumnNameText] = GroupNameText.GetTextOrDefaultString();
            parameters[Translations.MasterIDText] = GroupIDText.GetTextOrDefaultString();
            parameters["id"] = parameters[Translations.TcIDText];
            parameters["columnFilter"] = Translations.TcIDText;

            Translations.ImportData(Translations.TrnTranslationColumnsTableName, parameters);
        }
    }
}
