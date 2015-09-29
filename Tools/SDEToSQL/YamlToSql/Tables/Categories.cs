using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace EVEMon.SDEToSQL.YamlToSQL.Tables
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

        // Translations
        private const string TranslationCategoriesGroupID = "4";
        private const string TranslationCategoriesID = "6";


        /// <summary>
        /// Imports the categories ids.
        /// </summary>
        internal static void Import()
        {
            if (Program.IsClosing)
                return;

            Stopwatch stopwatch = Stopwatch.StartNew();
            Util.ResetCounters();

            string yamlFile = YamlFilesConstants.categoryIDs;
            string filePath = Util.CheckYamlFileExists(yamlFile);

            if (String.IsNullOrEmpty(filePath))
                return;

            string text = String.Format(CultureInfo.InvariantCulture, "Parsing {0}... ", yamlFile);
            Console.Write(text);
            YamlMappingNode rNode = Util.ParseYamlFile(filePath);

            if (rNode == null)
            {
                Console.WriteLine(@"Unable to parse {0}.", yamlFile);
                return;
            }

            Translations.InsertTranslationsStaticData(new TranslationsParameters
            {
                TableName = InvCategoriesTableName,
                SourceTable = "inventory.categoriesTx",
                ColumnName = CategoryNameText,
                MasterID = CategoryIDText,
                TcGroupID = TranslationCategoriesGroupID,
                TcID = TranslationCategoriesID
            });

            Console.SetCursorPosition(Console.CursorLeft - text.Length, Console.CursorTop);

            Console.Write(@"Importing {0}... ", yamlFile);

            Database.DropAndCreateTable(InvCategoriesTableName);

            ImportDataBulk(rNode);

            Util.DisplayEndTime(stopwatch);

            Console.WriteLine();
        }

        /// <summary>
        /// Imports the data bulk.
        /// </summary>
        /// <param name="rNode">The r node.</param>
        private static void ImportDataBulk(YamlMappingNode rNode)
        {
            Util.UpdatePercentDone(0);

            DataTable invCategoriesTable = GetInvCategoriesDataTable();

            DataTable trnTranslationsTable = Translations.GetTrnTranslationsDataTable();

            int total = rNode.Count();
            total = (int)Math.Ceiling(total + (total * 0.01));

            foreach (KeyValuePair<YamlNode, YamlNode> pair in rNode.Children)
            {
                Util.UpdatePercentDone(total);

                YamlMappingNode cNode = pair.Value as YamlMappingNode;

                if (cNode == null)
                    continue;

                YamlMappingNode categoriesNameNodes = cNode.Children.Keys.Any(key => key.ToString() == NameText)
                    ? cNode.Children[new YamlScalarNode(NameText)] as YamlMappingNode
                    : null;

                DataRow row = invCategoriesTable.NewRow();
                row[CategoryIDText] = SqlInt32.Parse(pair.Key.ToString());
                row[CategoryNameText] = categoriesNameNodes == null
                    ? cNode.Children.GetSqlTypeOrDefault<SqlString>(CategoryNameText)
                    : categoriesNameNodes.Children.GetSqlTypeOrDefault<SqlString>(Translations.EnglishLanguageIDText);
                row[DescriptionText] = cNode.Children.GetSqlTypeOrDefault<SqlString>(DescriptionText);
                row[IconIDText] = cNode.Children.GetSqlTypeOrDefault<SqlInt32>(IconIDText);
                row[PublishedText] = cNode.Children.GetSqlTypeOrDefault<SqlBoolean>(PublishedText);

                invCategoriesTable.Rows.Add(row);

                if (categoriesNameNodes != null)
                    Translations.InsertTranslations(TranslationCategoriesID, pair.Key, categoriesNameNodes, trnTranslationsTable);
            }
            if (trnTranslationsTable.Rows.Count > 0)
            {
                Translations.DeleteTranslations(TranslationCategoriesID);
                Translations.ImportDataBulk(trnTranslationsTable);
            }

            Database.ImportDataBulk(InvCategoriesTableName, invCategoriesTable);

            Util.UpdatePercentDone(invCategoriesTable.Rows.Count);
        }

        /// <summary>
        /// Gets the data table for the invCategories table.
        /// </summary>
        /// <returns></returns>
        private static DataTable GetInvCategoriesDataTable()
        {
            using (DataTable invCategoriesTable = new DataTable())
            {
                invCategoriesTable.Columns.AddRange(
                    new[]
                {
                    new DataColumn(CategoryIDText, typeof(SqlInt32)),
                    new DataColumn(CategoryNameText, typeof(SqlString)),
                    new DataColumn(DescriptionText, typeof(SqlString)),
                    new DataColumn(IconIDText, typeof(SqlInt32)),
                    new DataColumn(PublishedText, typeof(SqlBoolean)),
                });
                return invCategoriesTable;
            }
        }
    }
}
