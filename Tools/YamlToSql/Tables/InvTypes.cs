using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace EVEMon.YamlToSql.Tables
{
    internal static class InvTypes
    {
        private const string InvTypesTableName = "invTypes";
        private const string DgmMasteriesTableName = "dgmMasteries";
        private const string DgmTypeMasteriesTableName = "dgmTypeMasteries";
        private const string DgmTraitsTableName = "dgmTraits";
        private const string DgmTypeTraitsTableName = "dgmTypeTraits";

        private const string FactionIDText = "factionID";
        private const string GraphicIDText = "graphicID";
        private const string IconIDText = "iconID";
        private const string RadiusText = "radius";
        private const string SoundIDText = "soundID";
        private const string MasteriesText = "masteries";
        private const string TraitsText = "traits";

        // Masteries
        private const string TypeIDText = "typeID";
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

        private const string NullText = "Null";

        /// <summary>
        /// Imports the type ids.
        /// </summary>
        internal static void ImportTypeIds(SqlConnection connection)
        {
            DateTime startTime = DateTime.Now;
            Util.ResetCounters();

            var yamlFile = YamlFilesConstants.typeIDs;
            var filePath = Util.CheckYamlFileExists(yamlFile);

            if (String.IsNullOrEmpty(filePath))
                return;

            CreateInvTypesColumns(connection);
            CreateDgmMasteriesTable(connection);
            CreateDgmTypeMasteriesTable(connection);
            CreateDgmTraitsTable(connection);
            CreateDgmTypeTraitsTable(connection);

            Console.WriteLine();
            Console.Write(@"Importing {0}... ", yamlFile);

            YamlMappingNode rNode = Util.ParseYamlFile(filePath);

            if (rNode == null)
            {
                Console.WriteLine(@"Unable to parse {0}.", yamlFile);
                return;
            }

            ImportInvTypesData(connection, rNode);

            Util.DisplayEndTime(startTime);

            Console.WriteLine();
        }

        /// <summary>
        /// Creates the DGM type traits table.
        /// </summary>
        /// <param name="connection">The connection.</param>
        private static void CreateDgmTypeTraitsTable(SqlConnection connection)
        {
            var command = new SqlCommand { Connection = connection };
            DataTable dataTable = connection.GetSchema("columns");

            if (dataTable.Select(String.Format("TABLE_NAME = '{0}'", DgmTypeTraitsTableName)).Length == 0)
                Database.CreateTable(command, DgmTypeTraitsTableName);
            else
            {
                Database.DropTable(command, DgmTypeTraitsTableName);
                Database.CreateTable(command, DgmTypeTraitsTableName);
            }
        }

        /// <summary>
        /// Creates the DGM traits table.
        /// </summary>
        /// <param name="connection">The connection.</param>
        private static void CreateDgmTraitsTable(SqlConnection connection)
        {
            var command = new SqlCommand { Connection = connection };
            DataTable dataTable = connection.GetSchema("columns");

            if (dataTable.Select(String.Format("TABLE_NAME = '{0}'", DgmTraitsTableName)).Length == 0)
                Database.CreateTable(command, DgmTraitsTableName);
            else
            {
                Database.DropTable(command, DgmTraitsTableName);
                Database.CreateTable(command, DgmTraitsTableName);
            }
        }

        /// <summary>
        /// Creates the DGM type masteries table.
        /// </summary>
        /// <param name="connection">The connection.</param>
        private static void CreateDgmTypeMasteriesTable(SqlConnection connection)
        {
            var command = new SqlCommand { Connection = connection };
            DataTable dataTable = connection.GetSchema("columns");

            if (dataTable.Select(String.Format("TABLE_NAME = '{0}'", DgmTypeMasteriesTableName)).Length == 0)
                Database.CreateTable(command, DgmTypeMasteriesTableName);
            else
            {
                Database.DropTable(command, DgmTypeMasteriesTableName);
                Database.CreateTable(command, DgmTypeMasteriesTableName);
            }
        }

        /// <summary>
        /// Creates the DGM masteries table.
        /// </summary>
        /// <param name="connection">The connection.</param>
        private static void CreateDgmMasteriesTable(SqlConnection connection)
        {
            var command = new SqlCommand { Connection = connection };
            DataTable dataTable = connection.GetSchema("columns");

            if (dataTable.Select(String.Format("TABLE_NAME = '{0}'", DgmMasteriesTableName)).Length == 0)
                Database.CreateTable(command, DgmMasteriesTableName);
            else
            {
                Database.DropTable(command, DgmMasteriesTableName);
                Database.CreateTable(command, DgmMasteriesTableName);
            }
        }

        /// <summary>
        /// Creates the inv types columns.
        /// </summary>
        /// <param name="connection">The connection.</param>
        private static void CreateInvTypesColumns(SqlConnection connection)
        {
            var command = new SqlCommand { Connection = connection };
            DataTable dataTable = connection.GetSchema("columns");

            if (dataTable.Select(String.Format("TABLE_NAME = '{0}'", InvTypesTableName)).Length == 0)
            {
                Console.WriteLine(@"Can't find table '{0}'.", InvTypesTableName);
                Console.ReadLine();
                Environment.Exit(-1);
            }

            Database.CreateColumn(dataTable, command, InvTypesTableName, FactionIDText, "int");
            Database.CreateColumn(dataTable, command, InvTypesTableName, GraphicIDText, "int");
            Database.CreateColumn(dataTable, command, InvTypesTableName, IconIDText, "int");
            Database.CreateColumn(dataTable, command, InvTypesTableName, RadiusText, "float");
            Database.CreateColumn(dataTable, command, InvTypesTableName, SoundIDText, "int");
        }

        /// <summary>
        /// Imports the inv types data.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="rNode">The r node.</param>
        private static void ImportInvTypesData(SqlConnection connection,YamlMappingNode rNode)
        {
            var command = new SqlCommand { Connection = connection };

            int masteryId = 0;
            int traitId = 0;
            Dictionary<int, Dictionary<string, string>> masteriesDict = new Dictionary<int, Dictionary<string, string>>();
            Dictionary<int, string> traitsDict = new Dictionary<int, string>();

            using (var tx = connection.BeginTransaction())
            {
                command.Transaction = tx;

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

                                foreach (YamlNode certificate in certNode)
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
                                        param[MasteryIDText] = masteriesDict.First(x => value.Any(y => x.Value.ContainsKey(y.Key) && x.Value.ContainsValue(y.Value))).Key.ToString(CultureInfo.InvariantCulture);

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
                                        traitId++;
                                        parameters = new Dictionary<string, string>();
                                        parameters[TraitIDText] = traitId.ToString(CultureInfo.InvariantCulture);
                                        parameters[BonusTextText] = String.Format("'{0}'",
                                            bonusNode.Children[new YamlScalarNode(BonusTextText)]
                                                .ToString()
                                                .Replace("'", "''"));
                                        parameters[UnitIDText] = bonusNode.Children.Keys.Any(key => key.ToString() == UnitIDText)
                                            ? bonusNode.Children[new YamlScalarNode(UnitIDText)].ToString()
                                            : NullText;
                                        
                                        var pars = new Dictionary<string, string>();
                                        pars[TypeIDText] = pair.Key.ToString();
                                        pars[ParentTypeIDText] = trait.Key.ToString();

                                        var value = parameters[BonusTextText];

                                        if (traitsDict.ContainsValue(value))
                                        {
                                            pars[TraitIDText] = traitsDict.First(x => x.Value == value).Key.ToString(CultureInfo.InvariantCulture);
                                            pars[BonusText] = bonusNode.Children.Keys.Any(key => key.ToString() == BonusText)
                                            ? bonusNode.Children[new YamlScalarNode(BonusText)].ToString()
                                            : NullText;

                                            command.CommandText = Database.SqlInsertCommandText(DgmTypeTraitsTableName, pars);
                                            command.ExecuteNonQuery();

                                            continue;
                                        }

                                        traitsDict[traitId] = value;

                                        pars[TraitIDText] = traitId.ToString(CultureInfo.InvariantCulture);
                                        pars[BonusText] = bonusNode.Children.Keys.Any(key => key.ToString() == BonusText)
                                            ? bonusNode.Children[new YamlScalarNode(BonusText)].ToString()
                                            : NullText;

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
                            : NullText;
                        parameters[GraphicIDText] = cNode.Children.Keys.Any(key => key.ToString() == GraphicIDText)
                            ? cNode.Children[new YamlScalarNode(GraphicIDText)].ToString()
                            : NullText;
                        parameters[IconIDText] = cNode.Children.Keys.Any(key => key.ToString() == IconIDText)
                            ? cNode.Children[new YamlScalarNode(IconIDText)].ToString()
                            : NullText;
                        parameters[RadiusText] = cNode.Children.Keys.Any(key => key.ToString() == RadiusText)
                            ? cNode.Children[new YamlScalarNode(RadiusText)].ToString()
                            : NullText;
                        parameters[SoundIDText] = cNode.Children.Keys.Any(key => key.ToString() == SoundIDText)
                            ? cNode.Children[new YamlScalarNode(SoundIDText)].ToString()
                            : NullText;

                        command.CommandText = Database.SqlUpdateCommandText(InvTypesTableName, parameters);
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
                    Console.ReadLine();
                    Environment.Exit(-1);
                }
            }
        }
    }
}
