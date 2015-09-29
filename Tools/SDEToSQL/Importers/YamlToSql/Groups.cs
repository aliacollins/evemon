using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using EVEMon.SDEToSQL.Providers;
using EVEMon.SDEToSQL.Utils;
using YamlDotNet.RepresentationModel;

namespace EVEMon.SDEToSQL.Importers.YamlToSQL
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

        // Translations
        private const string TranslationGroupsID = "7";
        private const string TranslationGroupsGroupID = "5";

        private static SqlConnectionProvider s_sqlConnectionProvider;
        private static bool s_isClosing;

        /// <summary>
        /// Initializes the <see cref="Util"/> class.
        /// </summary>
        static Groups()
        {
            Util.Closing += Util_Closing;
        }

        /// <summary>
        /// Handles the Closing event of the Program control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private static void Util_Closing(object sender, EventArgs e)
        {
            s_isClosing = true;
        }

        /// <summary>
        /// Imports the groups ids.
        /// </summary>
        internal static void Import(DbConnectionProvider sqlConnectionProvider)
        {
            if (sqlConnectionProvider == null)
                throw new ArgumentNullException("sqlConnectionProvider");

            s_sqlConnectionProvider = (SqlConnectionProvider)sqlConnectionProvider;

            if (s_isClosing)
                return;

            Stopwatch stopwatch = Stopwatch.StartNew();
            Util.ResetCounters();

            string yamlFile = YamlFilesConstants.GroupIDs;
            string filePath = Util.CheckYamlFileExists(yamlFile);

            if (String.IsNullOrEmpty(filePath))
                return;

            string text = String.Format(CultureInfo.InvariantCulture, "Parsing {0}... ", yamlFile);
            Console.Write(text);
            YamlMappingNode rNode = Util.ParseYamlFile(filePath);

            if (s_isClosing)
                return;

            Util.SetConsoleCursorPosition(text);

            if (rNode == null)
            {
                Console.WriteLine(@"Unable to parse {0}.", yamlFile);
                return;
            }

            Translations.InsertTranslationsStaticData(s_sqlConnectionProvider, new TranslationsParameters
            {
                TableName = InvGroupsTableName,
                SourceTable = "inventory.groupsTx",
                ColumnName = GroupNameText,
                MasterID = GroupIDText,
                TcGroupID = TranslationGroupsGroupID,
                TcID = TranslationGroupsID
            });

            Console.Write(@"Importing {0}... ", yamlFile);

            s_sqlConnectionProvider.DropAndCreateTable(InvGroupsTableName);

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
            if (s_isClosing)
                return;

            Util.UpdatePercentDone(0);

            DataTable invGroupsTable = GetInvGroupsDataTable();

            DataTable trnTranslationsTable = Translations.GetTrnTranslationsDataTable();

            int total = rNode.Count();
            total = (int)Math.Ceiling(total + (total * 0.01));

            foreach (KeyValuePair<YamlNode, YamlNode> pair in rNode.Children)
            {
                Util.UpdatePercentDone(total);

                YamlMappingNode cNode = pair.Value as YamlMappingNode;

                if (cNode == null)
                    continue;

                YamlMappingNode groupNameNodes = cNode.Children.Keys.Any(key => key.ToString() == NameText)
                    ? cNode.Children[new YamlScalarNode(NameText)] as YamlMappingNode
                    : null;

                DataRow row = invGroupsTable.NewRow();
                row[GroupIDText] = SqlInt32.Parse(pair.Key.ToString());
                row[CategoryIDText] = cNode.Children.GetSqlTypeOrDefault<SqlInt32>(CategoryIDText);
                row[GroupNameText] = groupNameNodes == null
                    ? cNode.Children.GetSqlTypeOrDefault<SqlString>(GroupNameText)
                    : groupNameNodes.Children.GetSqlTypeOrDefault<SqlString>(Translations.EnglishLanguageIDText);
                row[DescriptionText] = cNode.Children.GetSqlTypeOrDefault<SqlString>(DescriptionText);
                row[IconIDText] = cNode.Children.GetSqlTypeOrDefault<SqlInt32>(IconIDText);
                row[UseBasePriceText] = cNode.Children.GetSqlTypeOrDefault<SqlBoolean>(UseBasePriceText);
                row[AllowManufactureText] = cNode.Children.GetSqlTypeOrDefault<SqlBoolean>(AllowManufactureText);
                row[AllowRecyclerText] = cNode.Children.GetSqlTypeOrDefault<SqlBoolean>(AllowRecyclerText);
                row[AnchoredText] = cNode.Children.GetSqlTypeOrDefault<SqlBoolean>(AnchoredText);
                row[AnchorableText] = cNode.Children.GetSqlTypeOrDefault<SqlBoolean>(AnchorableText);
                row[FittableNonSingletonText] = cNode.Children.GetSqlTypeOrDefault<SqlBoolean>(FittableNonSingletonText);
                row[PublishedText] = cNode.Children.GetSqlTypeOrDefault<SqlBoolean>(PublishedText);

                invGroupsTable.Rows.Add(row);

                if (groupNameNodes != null)
                    Translations.AddTranslationsParameters(TranslationGroupsID, pair.Key, groupNameNodes, trnTranslationsTable);
            }

            if (trnTranslationsTable.Rows.Count > 0)
            {
                Translations.DeleteTranslations(s_sqlConnectionProvider, TranslationGroupsID);
                Translations.ImportDataBulk(s_sqlConnectionProvider, trnTranslationsTable);
            }

            s_sqlConnectionProvider.ImportDataBulk(InvGroupsTableName, invGroupsTable);

            Util.UpdatePercentDone(invGroupsTable.Rows.Count);
        }

        /// <summary>
        /// Gets the data table for the invGroups table.
        /// </summary>
        /// <returns></returns>
        private static DataTable GetInvGroupsDataTable()
        {
            using (DataTable invGroupsTable = new DataTable())
            {
                invGroupsTable.Columns.AddRange(
                    new[]
                {
                    new DataColumn(GroupIDText, typeof(SqlInt32)),
                    new DataColumn(CategoryIDText, typeof(SqlInt32)),
                    new DataColumn(GroupNameText, typeof(SqlString)),
                    new DataColumn(DescriptionText, typeof(SqlString)),
                    new DataColumn(IconIDText, typeof(SqlInt32)),
                    new DataColumn(UseBasePriceText, typeof(SqlBoolean)),
                    new DataColumn(AllowManufactureText, typeof(SqlBoolean)),
                    new DataColumn(AllowRecyclerText, typeof(SqlBoolean)),
                    new DataColumn(AnchoredText, typeof(SqlBoolean)),
                    new DataColumn(AnchorableText, typeof(SqlBoolean)),
                    new DataColumn(FittableNonSingletonText, typeof(SqlBoolean)),
                    new DataColumn(PublishedText, typeof(SqlBoolean)),
                });
                return invGroupsTable;
            }
        }
    }
}
