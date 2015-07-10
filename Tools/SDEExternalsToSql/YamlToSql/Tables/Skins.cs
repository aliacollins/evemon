using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace EVEMon.SDEExternalsToSql.YamlToSql.Tables
{
    internal static class Skins
    {
        private const string SknSkinsTableName = "sknSkins";

        private const string SkinIDText = "skinID";
        private const string InternalNameText = "internalName";
        private const string SkinMaterialIDText = "skinMaterialID";
        private const string TypesText = "types";
        private const string TypeIDText = "typeID";
        private const string AllowCCPDevsText = "allowCCPDevs";
        private const string VisibleSerenityText = "visibleSerenity";
        private const string VisibleTranquilityText = "visibleTranquility";

        /// <summary>
        /// Imports the skins.
        /// </summary>
        internal static void Import()
        {
            DateTime startTime = DateTime.Now;
            Util.ResetCounters();

            var yamlFile = YamlFilesConstants.skins;
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

            Database.CreateTable(SknSkinsTableName);

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

                        Dictionary<string, string> parameters = new Dictionary<string, string>();
                        parameters[SkinIDText] = pair.Key.ToString();
                        parameters[InternalNameText] = cNode.Children.Keys.Any(key => key.ToString() == InternalNameText)
                            ? String.Format("N'{0}'",
                                cNode.Children[new YamlScalarNode(InternalNameText)].ToString()
                                    .Replace("'", Database.StringEmpty))
                            : Database.StringEmpty;
                        parameters[SkinMaterialIDText] = cNode.Children.Keys.Any(key => key.ToString() == SkinMaterialIDText)
                            ? cNode.Children[new YamlScalarNode(SkinMaterialIDText)].ToString()
                            : Database.Null;
                        parameters[AllowCCPDevsText] = cNode.Children.Keys.Any(key => key.ToString() == AllowCCPDevsText)
                            ? Convert.ToByte(Convert.ToBoolean(cNode.Children[new YamlScalarNode(AllowCCPDevsText)].ToString()))
                                .ToString()
                            : "0";
                        parameters[VisibleSerenityText] = cNode.Children.Keys.Any(key => key.ToString() == VisibleSerenityText)
                            ? Convert.ToByte(
                                Convert.ToBoolean(cNode.Children[new YamlScalarNode(VisibleSerenityText)].ToString()))
                                .ToString()
                            : "0";
                        parameters[VisibleTranquilityText] =
                            cNode.Children.Keys.Any(key => key.ToString() == VisibleTranquilityText)
                                ? Convert.ToByte(
                                    Convert.ToBoolean(cNode.Children[new YamlScalarNode(VisibleTranquilityText)].ToString()))
                                    .ToString()
                                : "0";

                        YamlNode typesNode = new YamlScalarNode(TypesText);
                        if (cNode.Children.ContainsKey(typesNode))
                        {
                            YamlSequenceNode typeIDsNode = cNode.Children[typesNode] as YamlSequenceNode;

                            parameters[TypeIDText] = typeIDsNode != null
                                ? typeIDsNode.Count() == 1
                                    ? typeIDsNode.Children.First().ToString()
                                    : Database.Null
                                : Database.Null;
                        }

                        command.CommandText = Database.SqlInsertCommandText(SknSkinsTableName, parameters);
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
