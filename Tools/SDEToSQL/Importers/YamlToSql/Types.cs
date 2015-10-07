using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using EVEMon.SDEToSQL.Providers;
using EVEMon.SDEToSQL.Utils;
using YamlDotNet.RepresentationModel;

namespace EVEMon.SDEToSQL.Importers.YamlToSQL
{
    internal static class Types
    {
        private const string InvTypesTableName = "invTypes";
        private const string DgmMasteriesTableName = "dgmMasteries";
        private const string DgmTypeMasteriesTableName = "dgmTypeMasteries";
        private const string DgmTraitsTableName = "dgmTraits";
        private const string DgmTypeTraitsTableName = "dgmTypeTraits";

        private const string MasteriesText = "masteries";
        private const string TraitsText = "traits";
        private const string NameText = "name";

        // Types
        private const string TypeIDText = "typeID";
        private const string GroupIDText = "groupID";
        private const string TypeNameText = "typeName";
        private const string DescriptionText = "description";
        private const string MassText = "mass";
        private const string VolumeText = "volume";
        private const string CapacityText = "capacity";
        private const string PortionSizeText = "portionSize";
        private const string RaceIDText = "raceID";
        private const string BasePriceText = "basePrice";
        private const string PublishedText = "published";
        private const string MarketGroupIDText = "marketGroupID";
        private const string ChanceOfDuplicatingText = "chanceOfDuplicating";
        private const string FactionIDText = "factionID";
        private const string GraphicIDText = "graphicID";
        private const string IconIDText = "iconID";
        private const string RadiusText = "radius";
        private const string SoundIDText = "soundID";

        // Masteries
        private const string GradeText = "grade";
        private const string CertificateIDText = "certificateID";

        // Traits
        private const string ParentTypeIDText = "parentTypeID";
        private const string BonusText = "bonus";
        private const string BonusTextText = "bonusText";
        private const string UnitIDText = "unitID";

        // TypeTraits
        private const string TraitIDText = "traitID";

        // TypeMasteries
        private const string MasteryIDText = "masteryID";

        // Translations
        private const string TranslationTypesGroupID= "5";
        private const string TranslationTypesDescriptionID = "33";
        private const string TranslationTypesTypeNameID = "8";
        private const string TranslationTraitsGroupID = "90";
        private const string TranslationTraitsBonusTextID = "140";

        private static SqlConnectionProvider s_sqlConnectionProvider;
        private static bool s_isClosing;

        /// <summary>
        /// Initializes the <see cref="Util"/> class.
        /// </summary>
        static Types()
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
        /// Imports the type ids.
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

            string yamlFile = YamlFilesConstants.TypeIDs;
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
            
            Console.Write(@"Importing {0}... ", yamlFile);

            var columns = new Dictionary<string, string>
            {
                { FactionIDText, "int" },
                { GraphicIDText, "int" },
                { IconIDText, "int" },
                { RadiusText, "float" },
                { SoundIDText, "int" }
            };

            bool tableIsEmpty = s_sqlConnectionProvider.CreateTableOrColumns(rNode, "name", InvTypesTableName, columns);

            s_sqlConnectionProvider.DropAndCreateTable(rNode, "masteries", DgmMasteriesTableName);
            s_sqlConnectionProvider.DropAndCreateTable(rNode, "masteries", DgmTypeMasteriesTableName);

            bool traitsAdded = s_sqlConnectionProvider.DropAndCreateTable(rNode, "traits", DgmTraitsTableName);
            s_sqlConnectionProvider.DropAndCreateTable(rNode, "traits", DgmTypeTraitsTableName);

            if (tableIsEmpty)
                ImportTranslationsStaticData(traitsAdded);

            if (tableIsEmpty)
                ImportDataBulk(rNode);
            else
                ImportData(rNode);

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

            DataTable invTypesTable = GetInvTypesDataTable();
            DataTable dgmMasteriesTable = GetDgmMasteriesDataTable();
            DataTable dgmTypeMasteriesTable = GetDgmTypeMasteriesDataTable();
            DataTable dgmTraitsTable = GetDgmTraitsDataTable();
            DataTable dgmTypeTraitsTable = GetDgmTypeTraitsDataTable();
            DataTable trnTranslationsTable = Translations.GetTrnTranslationsDataTable();

            int total = rNode.Count();
            total = (int)Math.Ceiling(total + (total * 0.01));

            int masteryId = 0;
            int traitId = 0;
            Dictionary<int, Dictionary<string, string>> masteriesDict = new Dictionary<int, Dictionary<string, string>>();
            Dictionary<int, string> traitsDict = new Dictionary<int, string>();

            foreach (KeyValuePair<YamlNode, YamlNode> pair in rNode.Children)
            {
                Util.UpdatePercentDone(total);

                YamlMappingNode cNode = pair.Value as YamlMappingNode;

                if (cNode == null)
                    continue;

                // Masteries
                ImportMasteries(cNode, dgmMasteriesTable, dgmTypeMasteriesTable, pair, masteriesDict, ref masteryId);

                //Traits
                ImportTraits(cNode, dgmTraitsTable, dgmTypeTraitsTable, trnTranslationsTable, pair, traitsDict, ref traitId);

                // Types
                ImportTypes(cNode, invTypesTable, pair, trnTranslationsTable);
            }

            Translations.DeleteTranslations(s_sqlConnectionProvider, TranslationTypesDescriptionID);
            Translations.DeleteTranslations(s_sqlConnectionProvider, TranslationTypesTypeNameID);
            Translations.DeleteTranslations(s_sqlConnectionProvider, TranslationTraitsBonusTextID);
            Translations.ImportDataBulk(s_sqlConnectionProvider, trnTranslationsTable);

            s_sqlConnectionProvider.ImportDataBulk(InvTypesTableName, invTypesTable);
            s_sqlConnectionProvider.ImportDataBulk(DgmMasteriesTableName, dgmMasteriesTable);
            s_sqlConnectionProvider.ImportDataBulk(DgmTypeMasteriesTableName, dgmTypeMasteriesTable);
            s_sqlConnectionProvider.ImportDataBulk(DgmTraitsTableName, dgmTraitsTable);
            s_sqlConnectionProvider.ImportDataBulk(DgmTypeTraitsTableName, dgmTypeTraitsTable);

            Util.UpdatePercentDone(invTypesTable.Rows.Count);
        }

        /// <summary>
        /// Imports the data.
        /// </summary>
        /// <param name="rNode">The r node.</param>
        private static void ImportData(YamlMappingNode rNode)
        {
            SqlConnection sqlConnection = (SqlConnection)s_sqlConnectionProvider.Connection;

            DataTable dgmMasteriesTable = GetDgmMasteriesDataTable();
            DataTable dgmTypeMasteriesTable = GetDgmTypeMasteriesDataTable();
            DataTable dgmTraitsTable = GetDgmTraitsDataTable();
            DataTable dgmTypeTraitsTable = GetDgmTypeTraitsDataTable();
            DataTable trnTranslationsTable = Translations.GetTrnTranslationsDataTable();

            int masteryId = 0;
            int traitId = 0;
            Dictionary<int, Dictionary<string, string>> masteriesDict = new Dictionary<int, Dictionary<string, string>>();
            Dictionary<int, string> traitsDict = new Dictionary<int, string>();

            using (DbCommand command = new SqlCommand(
                String.Empty,
                sqlConnection,
                sqlConnection.BeginTransaction()))
            {
                try
                {
                    foreach (KeyValuePair<YamlNode, YamlNode> pair in rNode.Children)
                    {
                        Util.UpdatePercentDone(rNode.Count());

                        YamlMappingNode cNode = rNode.Children[pair.Key] as YamlMappingNode;

                        if (cNode == null)
                            continue;

                        ImportMasteries(cNode, dgmMasteriesTable, dgmTypeMasteriesTable, pair, masteriesDict, ref masteryId);

                        ImportTraits(cNode, dgmTraitsTable, dgmTypeTraitsTable, trnTranslationsTable, pair, traitsDict, ref traitId);

                        Dictionary<string, string> parameters = new Dictionary<string, string>();
                        parameters["columnFilter1"] = TypeIDText;
                        parameters["id1"] = pair.Key.ToString();

                        parameters[FactionIDText] = cNode.Children.GetTextOrDefaultString(FactionIDText);
                        parameters[GraphicIDText] = cNode.Children.GetTextOrDefaultString(GraphicIDText);
                        parameters[IconIDText] = cNode.Children.GetTextOrDefaultString(IconIDText);
                        parameters[RadiusText] = cNode.Children.GetTextOrDefaultString(RadiusText);
                        parameters[SoundIDText] = cNode.Children.GetTextOrDefaultString(SoundIDText);

                        command.CommandText = SqlConnectionProvider.SqlUpdateCommandText(InvTypesTableName, parameters);
                        command.ExecuteNonQuery();
                    }

                    command.Transaction.Commit();

                    if (trnTranslationsTable.Rows.Count > 0)
                    {
                        Translations.DeleteTranslations(s_sqlConnectionProvider, TranslationTraitsBonusTextID);
                        Translations.ImportDataBulk(s_sqlConnectionProvider, trnTranslationsTable);
                    }

                    s_sqlConnectionProvider.ImportDataBulk(DgmMasteriesTableName, dgmMasteriesTable);
                    s_sqlConnectionProvider.ImportDataBulk(DgmTypeMasteriesTableName, dgmTypeMasteriesTable);
                    s_sqlConnectionProvider.ImportDataBulk(DgmTraitsTableName, dgmTraitsTable);
                    s_sqlConnectionProvider.ImportDataBulk(DgmTypeTraitsTableName, dgmTypeTraitsTable);
                }
                catch (SqlException e)
                {
                    Util.HandleExceptionForCommand(command, e);

                    if (command.Transaction != null)
                        command.Transaction.Rollback();
                }
            }
        }

        /// <summary>
        /// Imports the types.
        /// </summary>
        /// <param name="cNode">The yaml node.</param>
        /// <param name="invTypesTable">The invTypes datatable.</param>
        /// <param name="pair">The pair.</param>
        /// <param name="trnTranslationsTable">The trnTranslations datatable.</param>
        private static void ImportTypes(YamlMappingNode cNode, DataTable invTypesTable, KeyValuePair<YamlNode, YamlNode> pair,
            DataTable trnTranslationsTable)
        {
            YamlMappingNode typeNameNodes = cNode.Children.Keys.Any(key => key.ToString() == NameText)
                ? cNode.Children[new YamlScalarNode(NameText)] as YamlMappingNode
                : null;
            YamlMappingNode descriptionNodes = cNode.Children.Keys.Any(key => key.ToString() == DescriptionText)
                ? cNode.Children[new YamlScalarNode(DescriptionText)] as YamlMappingNode
                : null;

            DataRow row = invTypesTable.NewRow();
            row[TypeIDText] = SqlInt32.Parse(pair.Key.ToString());
            row[GroupIDText] = cNode.Children.GetSqlTypeOrDefault<SqlInt32>(GroupIDText);
            row[TypeNameText] = typeNameNodes == null
                ? cNode.Children.GetSqlTypeOrDefault<SqlString>(NameText, defaultValue: "")
                : typeNameNodes.Children.GetSqlTypeOrDefault<SqlString>(Translations.EnglishLanguageIDText, defaultValue: "");
            row[DescriptionText] = descriptionNodes == null
                ? cNode.Children.GetSqlTypeOrDefault<SqlString>(DescriptionText, defaultValue: "")
                : descriptionNodes.Children.GetSqlTypeOrDefault<SqlString>(Translations.EnglishLanguageIDText,
                    defaultValue: "");
            row[MassText] = cNode.Children.GetSqlTypeOrDefault<SqlDouble>(MassText, defaultValue: "0");
            row[VolumeText] = cNode.Children.GetSqlTypeOrDefault<SqlDouble>(VolumeText, defaultValue: "0");
            row[CapacityText] = cNode.Children.GetSqlTypeOrDefault<SqlDouble>(CapacityText, defaultValue: "0");
            row[PortionSizeText] = cNode.Children.GetSqlTypeOrDefault<SqlInt32>(PortionSizeText);
            row[RaceIDText] = cNode.Children.GetSqlTypeOrDefault<SqlByte>(RaceIDText);
            row[BasePriceText] = cNode.Children.GetSqlTypeOrDefault<SqlMoney>(BasePriceText, defaultValue: "0.00");
            row[PublishedText] = cNode.Children.GetSqlTypeOrDefault<SqlBoolean>(PublishedText, defaultValue: "0");
            row[MarketGroupIDText] = cNode.Children.GetSqlTypeOrDefault<SqlInt32>(MarketGroupIDText);
            row[ChanceOfDuplicatingText] = cNode.Children.GetSqlTypeOrDefault<SqlDouble>(ChanceOfDuplicatingText,
                defaultValue: "0");
            row[FactionIDText] = cNode.Children.GetSqlTypeOrDefault<SqlInt32>(FactionIDText);
            row[GraphicIDText] = cNode.Children.GetSqlTypeOrDefault<SqlInt32>(GraphicIDText);
            row[IconIDText] = cNode.Children.GetSqlTypeOrDefault<SqlInt32>(IconIDText);
            row[RadiusText] = cNode.Children.GetSqlTypeOrDefault<SqlDouble>(RadiusText);
            row[SoundIDText] = cNode.Children.GetSqlTypeOrDefault<SqlInt32>(SoundIDText);

            invTypesTable.Rows.Add(row);

            if (typeNameNodes != null)
            {
                Translations.AddTranslationsParameters(TranslationTypesTypeNameID, pair.Key, typeNameNodes, trnTranslationsTable);
            }

            if (descriptionNodes != null)
            {
                Translations.AddTranslationsParameters(TranslationTypesDescriptionID, pair.Key, descriptionNodes,
                    trnTranslationsTable);
            }
        }

        /// <summary>
        /// Imports the traits.
        /// </summary>
        /// <param name="cNode">The yaml node.</param>
        /// <param name="dgmTraitsTable">The dgmTraits datatable.</param>
        /// <param name="dgmTypeTraitsTable">The dgmTypeTraits datatable.</param>
        /// <param name="trnTranslationsTable">The trnTranslations datatable.</param>
        /// <param name="pair">The pair.</param>
        /// <param name="traitsDict">The traits dictionary.</param>
        /// <param name="traitId">The trait identifier.</param>
        private static void ImportTraits(YamlMappingNode cNode, DataTable dgmTraitsTable, DataTable dgmTypeTraitsTable,
            DataTable trnTranslationsTable, KeyValuePair<YamlNode, YamlNode> pair, Dictionary<int, string> traitsDict,
            ref int traitId)
        {
            YamlNode traitsNode = new YamlScalarNode(TraitsText);
            if (cNode.Children.ContainsKey(traitsNode))
            {
                YamlMappingNode traitNode = cNode.Children[traitsNode] as YamlMappingNode;

                if (traitNode == null)
                    return;

                foreach (KeyValuePair<YamlNode, YamlNode> trait in traitNode)
                {
                    YamlMappingNode bonusesNode = traitNode.Children[trait.Key] as YamlMappingNode;

                    if (bonusesNode == null)
                        continue;

                    foreach (YamlMappingNode bonusNode in bonusesNode
                        .Select(bonuses => bonusesNode.Children[bonuses.Key])
                        .OfType<YamlMappingNode>())
                    {
                        YamlMappingNode bonusTextNodes = bonusNode.Children.Keys.Any(key => key.ToString() == BonusTextText)
                            ? bonusNode.Children[new YamlScalarNode(BonusTextText)] as YamlMappingNode
                            : null;

                        // dgmTraits
                        traitId++;
                        DataRow row = dgmTraitsTable.NewRow();
                        row[TraitIDText] = SqlInt32.Parse(traitId.ToString(CultureInfo.InvariantCulture));
                        row[BonusTextText] = bonusTextNodes == null
                            ? bonusNode.Children.GetSqlTypeOrDefault<SqlString>(BonusTextText)
                            : bonusTextNodes.Children.GetSqlTypeOrDefault<SqlString>(Translations.EnglishLanguageIDText);
                        row[UnitIDText] = bonusNode.Children.GetSqlTypeOrDefault<SqlByte>(UnitIDText);

                        if (!dgmTraitsTable.Rows.OfType<DataRow>()
                            .Select(x => x.ItemArray)
                            .Any(x => x[1].ToString() == row[BonusTextText].ToString() &&
                                      x[2].ToString() == row[UnitIDText].ToString()
                            ))
                        {
                            dgmTraitsTable.Rows.Add(row);

                            if (bonusTextNodes != null)
                            {
                                Translations.AddTranslationsParameters(TranslationTraitsBonusTextID,
                                    new YamlScalarNode(traitId.ToString(CultureInfo.InvariantCulture)),
                                    bonusTextNodes, trnTranslationsTable);
                            }
                        }

                        // dgmTypeTraits
                        string value = row[BonusTextText].ToString();
                        row = dgmTypeTraitsTable.NewRow();
                        row[TypeIDText] = SqlInt32.Parse(pair.Key.ToString());
                        row[ParentTypeIDText] = SqlInt32.Parse(trait.Key.ToString());

                        if (traitsDict.ContainsValue(value))
                        {
                            row[TraitIDText] = SqlInt32.Parse(
                                traitsDict.First(x => x.Value == value).Key.ToString(CultureInfo.InvariantCulture));
                            row[BonusText] = bonusNode.Children.GetSqlTypeOrDefault<SqlDouble>(BonusText);

                            if (!dgmTypeTraitsTable.Rows.OfType<DataRow>()
                                .Select(x => x.ItemArray)
                                .Any(x =>
                                    x[0].ToString() == row[TypeIDText].ToString() &&
                                    x[1].ToString() == row[ParentTypeIDText].ToString() &&
                                    x[2].ToString() == row[TraitIDText].ToString()))
                                dgmTypeTraitsTable.Rows.Add(row);

                            continue;
                        }

                        traitsDict[traitId] = value;

                        row[TraitIDText] = SqlInt32.Parse(traitId.ToString(CultureInfo.InvariantCulture));
                        row[BonusText] = bonusNode.Children.GetSqlTypeOrDefault<SqlDouble>(BonusText);

                        dgmTypeTraitsTable.Rows.Add(row);
                    }
                }
            }
        }

        /// <summary>
        /// Imports the masteries.
        /// </summary>
        /// <param name="cNode">The yaml node.</param>
        /// <param name="dgmMasteriesTable">The dgmMasteries datatable.</param>
        /// <param name="dgmTypeMasteriesTable">The dgmTypeMasteries datatable.</param>
        /// <param name="pair">The pair.</param>
        /// <param name="masteriesDict">The masteries dictionary.</param>
        /// <param name="masteryId">The mastery identifier.</param>
        private static void ImportMasteries(YamlMappingNode cNode, DataTable dgmMasteriesTable, DataTable dgmTypeMasteriesTable,
            KeyValuePair<YamlNode, YamlNode> pair, Dictionary<int, Dictionary<string, string>> masteriesDict, ref int masteryId)
        {
            YamlNode masteriesNode = new YamlScalarNode(MasteriesText);
            if (cNode.Children.ContainsKey(masteriesNode))
            {
                YamlMappingNode mastNode = cNode.Children[masteriesNode] as YamlMappingNode;

                if (mastNode == null)
                    return;

                foreach (KeyValuePair<YamlNode, YamlNode> mastery in mastNode)
                {
                    YamlSequenceNode certNode = mastNode.Children[mastery.Key] as YamlSequenceNode;

                    if (certNode == null)
                        continue;

                    foreach (YamlNode certificate in certNode.Distinct())
                    {
                        // dgmMasteries
                        masteryId++;
                        DataRow row = dgmMasteriesTable.NewRow();
                        row[MasteryIDText] = SqlInt32.Parse(masteryId.ToString(CultureInfo.InvariantCulture));
                        row[CertificateIDText] = SqlInt32.Parse(certificate.ToString());
                        row[GradeText] = SqlByte.Parse(mastery.Key.ToString());

                        if (!dgmMasteriesTable.Rows.OfType<DataRow>()
                            .Select(x => x.ItemArray)
                            .Any(x => x[1].ToString() == row[CertificateIDText].ToString() &&
                                      x[2].ToString() == row[GradeText].ToString()
                            ))
                        {
                            dgmMasteriesTable.Rows.Add(row);
                        }

                        // dgmTypeMasteries
                        row = dgmTypeMasteriesTable.NewRow();
                        row[TypeIDText] = SqlInt32.Parse(pair.Key.ToString());

                        Dictionary<string, string> value = new Dictionary<string, string>
                        {
                            { mastery.Key.ToString(), certificate.ToString() }
                        };

                        if (masteriesDict.Values.Any(
                            x => x.Any(y => y.Key == mastery.Key.ToString()
                                            && y.Value == certificate.ToString())))
                        {
                            row[MasteryIDText] = SqlInt16.Parse(
                                masteriesDict.First(
                                    x => value.Any(y => x.Value.ContainsKey(y.Key) && x.Value.ContainsValue(y.Value)))
                                    .Key.ToString(CultureInfo.InvariantCulture));

                            dgmTypeMasteriesTable.Rows.Add(row);

                            continue;
                        }

                        masteriesDict[masteryId] = value;

                        row[MasteryIDText] = SqlInt16.Parse(masteryId.ToString(CultureInfo.InvariantCulture));

                        dgmTypeMasteriesTable.Rows.Add(row);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the data table for the dgmTypeTraits table.
        /// </summary>
        /// <returns></returns>
        private static DataTable GetDgmTypeTraitsDataTable()
        {
            using (DataTable dgmTypeTraits = new DataTable())
            {
                dgmTypeTraits.Columns.AddRange(
                    new[]
                {
                    new DataColumn(TypeIDText, typeof(SqlInt32)),
                    new DataColumn(ParentTypeIDText, typeof(SqlInt32)),
                    new DataColumn(TraitIDText, typeof(SqlInt32)),
                    new DataColumn(BonusText, typeof(SqlDouble)),
                });
                return dgmTypeTraits;
            }
        }

        /// <summary>
        /// Gets the data table for the dgmTraits table.
        /// </summary>
        /// <returns></returns>
        private static DataTable GetDgmTraitsDataTable()
        {
            using (DataTable dgmTraits = new DataTable())
            {
                dgmTraits.Columns.AddRange(
                    new[]
                {
                    new DataColumn(TraitIDText, typeof(SqlInt32)),
                    new DataColumn(BonusTextText, typeof(SqlString)),
                    new DataColumn(UnitIDText, typeof(SqlByte)),
                });
                return dgmTraits;
            }
        }

        /// <summary>
        /// Gets the data table for the dgmTypeMasteries table.
        /// </summary>
        /// <returns></returns>
        private static DataTable GetDgmTypeMasteriesDataTable()
        {
            using (DataTable dgmTypeMasteries = new DataTable())
            {
                dgmTypeMasteries.Columns.AddRange(
                    new[]
                {
                    new DataColumn(TypeIDText, typeof(SqlInt32)),
                    new DataColumn(MasteryIDText, typeof(SqlInt16)),
                });
                return dgmTypeMasteries;
            }
        }

        /// <summary>
        /// Gets the data table for the dgmMsteries table.
        /// </summary>
        /// <returns></returns>
        private static DataTable GetDgmMasteriesDataTable()
        {
            using (DataTable dgmMasteries = new DataTable())
            {
                dgmMasteries.Columns.AddRange(
                    new[]
                {
                    new DataColumn(MasteryIDText, typeof(SqlInt32)),
                    new DataColumn(CertificateIDText, typeof(SqlInt32)),
                    new DataColumn(GradeText, typeof(SqlByte)),
                });
                return dgmMasteries;
            }
        }

        /// <summary>
        /// Gets the data table for the invTypes table.
        /// </summary>
        /// <returns></returns>
        private static DataTable GetInvTypesDataTable()
        {
            using (DataTable invTypesTable = new DataTable())
            {
                invTypesTable.Columns.AddRange(
                    new[]
                {
                    new DataColumn(TypeIDText, typeof(SqlInt32)),
                    new DataColumn(GroupIDText, typeof(SqlInt32)),
                    new DataColumn(TypeNameText, typeof(SqlString)),
                    new DataColumn(DescriptionText, typeof(SqlString)),
                    new DataColumn(MassText, typeof(SqlDouble)),
                    new DataColumn(VolumeText, typeof(SqlDouble)),
                    new DataColumn(CapacityText, typeof(SqlDouble)),
                    new DataColumn(PortionSizeText, typeof(SqlInt32)),
                    new DataColumn(RaceIDText, typeof(SqlByte)),
                    new DataColumn(BasePriceText, typeof(SqlMoney)),
                    new DataColumn(PublishedText, typeof(SqlBoolean)),
                    new DataColumn(MarketGroupIDText, typeof(SqlInt32)),
                    new DataColumn(ChanceOfDuplicatingText, typeof(SqlDouble)),
                    new DataColumn(FactionIDText, typeof(SqlInt32)),
                    new DataColumn(GraphicIDText, typeof(SqlInt32)),
                    new DataColumn(IconIDText, typeof(SqlInt32)),
                    new DataColumn(RadiusText, typeof(SqlDouble)),
                    new DataColumn(SoundIDText, typeof(SqlInt32)),
                });
                return invTypesTable;
            }
        }

        /// <summary>
        /// Imports the translations static data.
        /// </summary>
        /// <param name="addTraitsRecords">if set to <c>true</c> [add traits records].</param>
        private static void ImportTranslationsStaticData(bool addTraitsRecords)
        {
            Translations.InsertTranslationsStaticData(s_sqlConnectionProvider, new TranslationsParameters
            {
                TableName = InvTypesTableName,
                SourceTable = "inventory.typesTx",
                ColumnName = DescriptionText,
                MasterID = TypeIDText,
                TcGroupID = TranslationTypesGroupID,
                TcID = TranslationTypesDescriptionID
            });

            Translations.InsertTranslationsStaticData(s_sqlConnectionProvider, new TranslationsParameters
            {
                TableName = InvTypesTableName,
                SourceTable = "inventory.typesTx",
                ColumnName = TypeNameText,
                MasterID = TypeIDText,
                TcGroupID = TranslationTypesGroupID,
                TcID = TranslationTypesTypeNameID
            });

            if (!addTraitsRecords)
                return;

            Translations.InsertTranslationsStaticData(s_sqlConnectionProvider, new TranslationsParameters
            {
                TableName = DgmTraitsTableName,
                SourceTable = "dogma.traitsTx",
                ColumnName = BonusTextText,
                MasterID = TraitIDText,
                TcGroupID = TranslationTraitsGroupID,
                TcID = TranslationTraitsBonusTextID
            });
        }
    }
}
