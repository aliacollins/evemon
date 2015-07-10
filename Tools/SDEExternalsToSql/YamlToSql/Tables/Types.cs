using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

        private const string EnglishLanguageIDText = "en";

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
            DateTime startTime = DateTime.Now;
            Util.ResetCounters();

            var yamlFile = YamlFilesConstants.typeIDs;
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

            Database.CreateTable(InvTypesTableName);
            Database.CreateTable(DgmMasteriesTableName);
            Database.CreateTable(DgmTypeMasteriesTableName);
            Database.CreateTable(DgmTraitsTableName);
            Database.CreateTable(DgmTypeTraitsTableName);
            Database.CreateColumns(InvTypesTableName, new Dictionary<string, string>
            {
                { FactionIDText, "int" },
                { GraphicIDText, "int" },
                { IconIDText, "int" },
                { RadiusText, "float" },
                { SoundIDText, "int" },
            });

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
            int masteryId = 0;
            int traitId = 0;
            Dictionary<int, Dictionary<string, string>> masteriesDict = new Dictionary<int, Dictionary<string, string>>();
            Dictionary<int, string> traitsDict = new Dictionary<int, string>();

            using (SqlTransaction tx = Database.SqlConnection.BeginTransaction())
            {
                IDbCommand command = new SqlCommand { Connection = Database.SqlConnection, Transaction = tx };
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
                                        var bonusTextNodes = bonusNode.Children[new YamlScalarNode(BonusTextText)] as YamlMappingNode;

                                        traitId++;
                                        parameters = new Dictionary<string, string>();
                                        parameters[TraitIDText] = traitId.ToString(CultureInfo.InvariantCulture);
                                        parameters[BonusTextText] = String.Format("N'{0}'", (bonusTextNodes == null
                                            ? bonusNode.Children[new YamlScalarNode(BonusTextText)].ToString()
                                            : bonusTextNodes.Children[new YamlScalarNode(EnglishLanguageIDText)].ToString())
                                            .Replace("'", Database.StringEmpty));
                                        parameters[UnitIDText] = bonusNode.Children.Keys.Any(key => key.ToString() == UnitIDText)
                                            ? bonusNode.Children[new YamlScalarNode(UnitIDText)].ToString()
                                            : Database.Null;

                                        Dictionary<string, string> pars = new Dictionary<string, string>();
                                        pars[TypeIDText] = pair.Key.ToString();
                                        pars[ParentTypeIDText] = trait.Key.ToString();

                                        String value = parameters[BonusTextText];

                                        if (traitsDict.ContainsValue(value))
                                        {
                                            pars[TraitIDText] =
                                                traitsDict.First(x => x.Value == value).Key.ToString(CultureInfo.InvariantCulture);
                                            pars[BonusText] = bonusNode.Children.Keys.Any(key => key.ToString() == BonusText)
                                                ? bonusNode.Children[new YamlScalarNode(BonusText)].ToString()
                                                : Database.Null;

                                            command.CommandText = Database.SqlInsertCommandText(DgmTypeTraitsTableName, pars);
                                            command.ExecuteNonQuery();

                                            continue;
                                        }

                                        traitsDict[traitId] = value;

                                        pars[TraitIDText] = traitId.ToString(CultureInfo.InvariantCulture);
                                        pars[BonusText] = bonusNode.Children.Keys.Any(key => key.ToString() == BonusText)
                                            ? bonusNode.Children[new YamlScalarNode(BonusText)].ToString()
                                            : Database.Null;

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

                        parameters[FactionIDText] = cNode.Children.Keys.Any(key => key.ToString() == FactionIDText)
                            ? cNode.Children[new YamlScalarNode(FactionIDText)].ToString()
                            : Database.Null;
                        parameters[GraphicIDText] = cNode.Children.Keys.Any(key => key.ToString() == GraphicIDText)
                            ? cNode.Children[new YamlScalarNode(GraphicIDText)].ToString()
                            : Database.Null;
                        parameters[IconIDText] = cNode.Children.Keys.Any(key => key.ToString() == IconIDText)
                            ? cNode.Children[new YamlScalarNode(IconIDText)].ToString()
                            : Database.Null;
                        parameters[RadiusText] = cNode.Children.Keys.Any(key => key.ToString() == RadiusText)
                            ? cNode.Children[new YamlScalarNode(RadiusText)].ToString()
                            : Database.Null;
                        parameters[SoundIDText] = cNode.Children.Keys.Any(key => key.ToString() == SoundIDText)
                            ? cNode.Children[new YamlScalarNode(SoundIDText)].ToString()
                            : Database.Null;

                        command.CommandText = Database.SqlUpdateCommandText(InvTypesTableName, parameters);
                        if (command.ExecuteNonQuery() != 0)
                            continue;

                        InsertRow(parameters, cNode, pair, command);
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
            parameters[GroupIDText] = cNode.Children.Keys.Any(key => key.ToString() == GroupIDText)
                ? cNode.Children[new YamlScalarNode(GroupIDText)].ToString()
                : Database.Null;
            parameters[TypeNameText] = cNode.Children.Keys.Any(key => key.ToString() == NameText)
                ? String.Format("N'{0}'", (typeNameNodes == null
                    ? cNode.Children[new YamlScalarNode(NameText)].ToString().Replace("'", Database.StringEmpty)
                    : typeNameNodes.Children.Keys.Any(key => key.ToString() == EnglishLanguageIDText)
                        ? typeNameNodes.Children[new YamlScalarNode(EnglishLanguageIDText)].ToString()
                            .Replace("'", Database.StringEmpty)
                        : String.Empty))
                : Database.StringEmpty;
            parameters[DescriptionText] = cNode.Children.Keys.Any(key => key.ToString() == DescriptionText)
                ? String.Format("N'{0}'", (descriptionNodes == null
                    ? cNode.Children[new YamlScalarNode(DescriptionText)].ToString().Replace("'", Database.StringEmpty)
                    : descriptionNodes.Children.Keys.Any(key => key.ToString() == EnglishLanguageIDText)
                        ? descriptionNodes.Children[new YamlScalarNode(EnglishLanguageIDText)].ToString()
                            .Replace("'", Database.StringEmpty)
                        : String.Empty))
                : Database.StringEmpty;
            parameters[MassText] = cNode.Children.Keys.Any(key => key.ToString() == MassText)
                ? cNode.Children[new YamlScalarNode(MassText)].ToString()
                : "0";
            parameters[VolumeText] = cNode.Children.Keys.Any(key => key.ToString() == VolumeText)
                ? cNode.Children[new YamlScalarNode(VolumeText)].ToString()
                : "0";
            parameters[CapacityText] = cNode.Children.Keys.Any(key => key.ToString() == CapacityText)
                ? cNode.Children[new YamlScalarNode(CapacityText)].ToString()
                : "0";
            parameters[PortionSizeText] = cNode.Children.Keys.Any(key => key.ToString() == PortionSizeText)
                ? cNode.Children[new YamlScalarNode(PortionSizeText)].ToString()
                : Database.Null;
            parameters[RaceIDText] = cNode.Children.Keys.Any(key => key.ToString() == RaceIDText)
                ? cNode.Children[new YamlScalarNode(RaceIDText)].ToString()
                : Database.Null;
            parameters[BasePriceText] = cNode.Children.Keys.Any(key => key.ToString() == BasePriceText)
                ? cNode.Children[new YamlScalarNode(BasePriceText)].ToString()
                : "0.00";
            parameters[PublishedText] = cNode.Children.Keys.Any(key => key.ToString() == PublishedText)
                ? Convert.ToByte(Convert.ToBoolean(cNode.Children[new YamlScalarNode(PublishedText)].ToString()))
                    .ToString()
                : "0";
            parameters[MarketGroupIDText] = cNode.Children.Keys.Any(key => key.ToString() == MarketGroupIDText)
                ? cNode.Children[new YamlScalarNode(MarketGroupIDText)].ToString()
                : Database.Null;
            parameters[ChanceOfDuplicatingText] = cNode.Children.Keys.Any(key => key.ToString() == ChanceOfDuplicatingText)
                ? cNode.Children[new YamlScalarNode(ChanceOfDuplicatingText)].ToString()
                : "0";

            command.CommandText = Database.SqlInsertCommandText(InvTypesTableName, parameters);
            command.ExecuteNonQuery();
        }
    }
}
