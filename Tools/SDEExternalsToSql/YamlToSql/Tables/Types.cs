using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace EVEMon.SDEExternalsToSql.YamlToSql.Tables
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

        /// <summary>
        /// Imports the type ids.
        /// </summary>
        internal static void Import()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            Util.ResetCounters();

            var yamlFile = YamlFilesConstants.typeIDs;
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

            var columns = new Dictionary<string, string>
            {
                { FactionIDText, "int" },
                { GraphicIDText, "int" },
                { IconIDText, "int" },
                { RadiusText, "float" },
                { SoundIDText, "int" }
            };
            Database.CreateTableOrColumns(InvTypesTableName, columns);

            Database.CreateTable(DgmMasteriesTableName);
            Database.CreateTable(DgmTypeMasteriesTableName);
            Database.CreateTable(DgmTraitsTableName);
            Database.CreateTable(DgmTypeTraitsTableName);

            ImportData(rNode);

            Util.DisplayEndTime(stopwatch);

            Console.WriteLine();
        }

        /// <summary>
        /// Imports the data.
        /// </summary>
        /// <param name="rNode">The r node.</param>
        private static void ImportData(YamlMappingNode rNode)
        {
            int masteryId = 0;
            int traitId = 0;
            Dictionary<int, Dictionary<string, string>> masteriesDict = new Dictionary<int, Dictionary<string, string>>();
            Dictionary<int, string> traitsDict = new Dictionary<int, string>();

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

                        Dictionary<string, string> parameters;

                        YamlMappingNode cNode = rNode.Children[pair.Key] as YamlMappingNode;

                        if (cNode == null)
                            continue;

                        YamlNode masteriesNode = new YamlScalarNode(MasteriesText);
                        if (cNode.Children.ContainsKey(masteriesNode))
                        {
                            YamlMappingNode mastNode = cNode.Children[masteriesNode] as YamlMappingNode;

                            if (mastNode == null)
                                continue;

                            foreach (KeyValuePair<YamlNode, YamlNode> mastery in mastNode)
                            {
                                YamlSequenceNode certNode = mastNode.Children[mastery.Key] as YamlSequenceNode;

                                if (certNode == null)
                                    continue;

                                foreach (YamlNode certificate in certNode.Distinct())
                                {
                                    masteryId++;
                                    parameters = new Dictionary<string, string>();
                                    parameters[MasteryIDText] = masteryId.ToString(CultureInfo.InvariantCulture);
                                    parameters[GradeText] = mastery.Key.ToString();
                                    parameters[CertificateIDText] = certificate.ToString();

                                    var param = new Dictionary<string, string>();
                                    param[TypeIDText] = pair.Key.ToString();

                                    var value = new Dictionary<string, string>
                                    {
                                        {
                                            mastery.Key.ToString(), certificate.ToString()
                                        }
                                    };

                                    if (masteriesDict.Values.Any(
                                        x => x.Any(y => y.Key == mastery.Key.ToString()
                                                        && y.Value == certificate.ToString())))
                                    {
                                        param[MasteryIDText] =
                                            masteriesDict.First(
                                                x => value.Any(y => x.Value.ContainsKey(y.Key) && x.Value.ContainsValue(y.Value)))
                                                .Key.ToString(CultureInfo.InvariantCulture);

                                        command.CommandText = Database.SqlInsertCommandText(DgmTypeMasteriesTableName, param);
                                        command.ExecuteNonQuery();

                                        continue;
                                    }

                                    masteriesDict[masteryId] = value;

                                    param[MasteryIDText] = masteryId.ToString(CultureInfo.InvariantCulture);

                                    command.CommandText = Database.SqlInsertCommandText(DgmTypeMasteriesTableName, param);
                                    command.ExecuteNonQuery();

                                    command.CommandText = Database.SqlInsertCommandText(DgmMasteriesTableName, parameters);
                                    command.ExecuteNonQuery();
                                }
                            }

                            YamlNode traitsNode = new YamlScalarNode(TraitsText);
                            if (cNode.Children.ContainsKey(traitsNode))
                            {
                                YamlMappingNode traitNode = cNode.Children[traitsNode] as YamlMappingNode;

                                if (traitNode == null)
                                    continue;

                                foreach (KeyValuePair<YamlNode, YamlNode> trait in traitNode)
                                {
                                    YamlMappingNode bonusesNode = traitNode.Children[trait.Key] as YamlMappingNode;

                                    if (bonusesNode == null)
                                        continue;

                                    foreach (YamlMappingNode bonusNode in bonusesNode
                                        .Select(bonuses => bonusesNode.Children[bonuses.Key])
                                        .OfType<YamlMappingNode>())
                                    {
                                        var bonusTextNodes =
                                            bonusNode.Children[new YamlScalarNode(BonusTextText)] as YamlMappingNode;

                                        traitId++;
                                        parameters = new Dictionary<string, string>();
                                        parameters[TraitIDText] = traitId.ToString(CultureInfo.InvariantCulture);
                                        parameters[BonusTextText] = bonusTextNodes == null
                                            ? bonusNode.Children.GetTextOrDefaultString(BonusTextText, isUnicode: true)
                                            : bonusTextNodes.Children.GetTextOrDefaultString(Translations.EnglishLanguageIDText,
                                                isUnicode: true);
                                        parameters[UnitIDText] = bonusNode.Children.GetTextOrDefaultString(UnitIDText);

                                        Dictionary<string, string> pars = new Dictionary<string, string>();
                                        pars[TypeIDText] = pair.Key.ToString();
                                        pars[ParentTypeIDText] = trait.Key.ToString();

                                        String value = parameters[BonusTextText];

                                        if (traitsDict.ContainsValue(value))
                                        {
                                            pars[TraitIDText] =
                                                traitsDict.First(x => x.Value == value).Key.ToString(CultureInfo.InvariantCulture);
                                            pars[BonusText] = bonusNode.Children.GetTextOrDefaultString(BonusText);

                                            command.CommandText = Database.SqlInsertCommandText(DgmTypeTraitsTableName, pars);
                                            command.ExecuteNonQuery();

                                            continue;
                                        }

                                        traitsDict[traitId] = value;

                                        pars[TraitIDText] = traitId.ToString(CultureInfo.InvariantCulture);
                                        pars[BonusText] = bonusNode.Children.GetTextOrDefaultString(BonusText);

                                        command.CommandText = Database.SqlInsertCommandText(DgmTypeTraitsTableName, pars);
                                        command.ExecuteNonQuery();

                                        command.CommandText = Database.SqlInsertCommandText(DgmTraitsTableName, parameters);
                                        command.ExecuteNonQuery();
                                    }
                                }
                            }
                        }

                        parameters = new Dictionary<string, string>();
                        parameters["id"] = pair.Key.ToString();
                        parameters["columnFilter"] = TypeIDText;

                        parameters[FactionIDText] = cNode.Children.GetTextOrDefaultString(FactionIDText);
                        parameters[GraphicIDText] = cNode.Children.GetTextOrDefaultString(GraphicIDText);
                        parameters[IconIDText] = cNode.Children.GetTextOrDefaultString(IconIDText);
                        parameters[RadiusText] = cNode.Children.GetTextOrDefaultString(RadiusText);
                        parameters[SoundIDText] = cNode.Children.GetTextOrDefaultString(SoundIDText);

                        command.CommandText = Database.SqlUpdateCommandText(InvTypesTableName, parameters);
                        if (command.ExecuteNonQuery() != 0)
                            continue;

                        InsertRow(parameters, cNode, pair, command);
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
        /// Inserts a row.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="cNode">The c node.</param>
        /// <param name="pair">The pair.</param>
        /// <param name="command">The command.</param>
        private static void InsertRow(IDictionary<string, string> parameters, YamlMappingNode cNode,
            KeyValuePair<YamlNode, YamlNode> pair, IDbCommand command)
        {
            parameters.Remove("id");
            parameters.Remove("columnFilter");

            var typeNameNodes = cNode.Children.Keys.Any(key => key.ToString() == NameText)
                ? cNode.Children[new YamlScalarNode(NameText)] as YamlMappingNode
                : null;
            var descriptionNodes = cNode.Children.Keys.Any(key => key.ToString() == DescriptionText)
                ? cNode.Children[new YamlScalarNode(DescriptionText)] as YamlMappingNode
                : null;

            parameters[TypeIDText] = pair.Key.ToString();
            parameters[GroupIDText] = cNode.Children.GetTextOrDefaultString(GroupIDText);
            parameters[TypeNameText] = typeNameNodes == null
                ? cNode.Children.GetTextOrDefaultString(NameText, defaultValue: Database.StringEmpty, isUnicode: true)
                : typeNameNodes.Children.GetTextOrDefaultString(Translations.EnglishLanguageIDText, defaultValue: Database.StringEmpty,
                    isUnicode: true);
            parameters[DescriptionText] = descriptionNodes == null
                ? cNode.Children.GetTextOrDefaultString(DescriptionText, defaultValue: Database.StringEmpty, isUnicode: true)
                : descriptionNodes.Children.GetTextOrDefaultString(Translations.EnglishLanguageIDText, defaultValue: Database.StringEmpty,
                    isUnicode: true);
            parameters[MassText] = cNode.Children.GetTextOrDefaultString(MassText, defaultValue: "0");
            parameters[VolumeText] = cNode.Children.GetTextOrDefaultString(VolumeText, defaultValue: "0");
            parameters[CapacityText] = cNode.Children.GetTextOrDefaultString(CapacityText, defaultValue: "0");
            parameters[PortionSizeText] = cNode.Children.GetTextOrDefaultString(PortionSizeText);
            parameters[RaceIDText] = cNode.Children.GetTextOrDefaultString(RaceIDText);
            parameters[BasePriceText] = cNode.Children.GetTextOrDefaultString(BasePriceText, defaultValue: "0.00");
            parameters[PublishedText] = cNode.Children.GetTextOrDefaultString(PublishedText, defaultValue: "0");
            parameters[MarketGroupIDText] = cNode.Children.GetTextOrDefaultString(MarketGroupIDText);
            parameters[ChanceOfDuplicatingText] = cNode.Children.GetTextOrDefaultString(ChanceOfDuplicatingText, defaultValue: "0");

            if (typeNameNodes != null)
                Translations.InsertTranslations(command, Translations.TranslationTypesTypeNameID, pair.Key, typeNameNodes);

            if (descriptionNodes != null)
                Translations.InsertTranslations(command, Translations.TranslationTypesDescriptionID, pair.Key, descriptionNodes);

            command.CommandText = Database.SqlInsertCommandText(InvTypesTableName, parameters);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Imports the translations.
        /// </summary>
        private static void ImportTranslations()
        {
            const string TableText = "dbo." + InvTypesTableName;
            var baseParameters = new Dictionary<string, string>();
            baseParameters[Translations.TcGroupIDText] = Translations.TranslationTypesGroupID;

            var parameters = new Dictionary<string, string>(baseParameters);
            parameters[Translations.SourceTableText] = "inventory.typesTx".GetTextOrDefaultString();
            parameters[Translations.DestinationTableText] = TableText.GetTextOrDefaultString();
            parameters[Translations.TranslatedKeyText] = DescriptionText.GetTextOrDefaultString();
            parameters[Translations.TcIDText] = Translations.TranslationTypesDescriptionID;
            parameters["id"] = parameters[Translations.SourceTableText];
            parameters["id2"] = parameters[Translations.TranslatedKeyText];
            parameters["columnFilter"] = Translations.SourceTableText;
            parameters["columnFilter2"] = Translations.TranslatedKeyText;

            Translations.ImportData(Translations.TranslationTableName, parameters);

            parameters[Translations.TranslatedKeyText] = TypeNameText.GetTextOrDefaultString();
            parameters[Translations.TcIDText] = Translations.TranslationTypesTypeNameID;
            parameters["id"] = parameters[Translations.SourceTableText];
            parameters["id2"] = parameters[Translations.TranslatedKeyText];
            parameters["columnFilter"] = Translations.SourceTableText;
            parameters["columnFilter2"] = Translations.TranslatedKeyText;

            Translations.ImportData(Translations.TranslationTableName, parameters);

            parameters = new Dictionary<string, string>(baseParameters);
            parameters[Translations.TcIDText] = Translations.TranslationTypesTypeNameID;
            parameters[Translations.TableNameText] = TableText.GetTextOrDefaultString();
            parameters[Translations.ColumnNameText] = TypeNameText.GetTextOrDefaultString();
            parameters[Translations.MasterIDText] = TypeIDText.GetTextOrDefaultString();
            parameters["id"] = parameters[Translations.TcIDText];
            parameters["columnFilter"] = Translations.TcIDText;

            Translations.ImportData(Translations.TrnTranslationColumnsTableName, parameters);

            parameters[Translations.TcIDText] = Translations.TranslationTypesDescriptionID;
            parameters[Translations.ColumnNameText] = DescriptionText.GetTextOrDefaultString();
            parameters["id"] = parameters[Translations.TcIDText];
            parameters["columnFilter"] = Translations.TcIDText;

            Translations.ImportData(Translations.TrnTranslationColumnsTableName, parameters);
        }
    }
}
