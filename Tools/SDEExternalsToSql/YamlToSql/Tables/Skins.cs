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

                        Dictionary<string, string> parameters = new Dictionary<string, string>();
                        parameters[SkinIDText] = pair.Key.ToString();
                        parameters[InternalNameText] = cNode.Children.GetTextOrDefaultString(InternalNameText,
                            defaultValue: Database.StringEmpty, isUnicode: true);
                        parameters[SkinMaterialIDText] = cNode.Children.GetTextOrDefaultString(SkinMaterialIDText);
                        parameters[AllowCCPDevsText] = cNode.Children.GetTextOrDefaultString(AllowCCPDevsText, defaultValue: "0");
                        parameters[VisibleSerenityText] = cNode.Children.GetTextOrDefaultString(VisibleSerenityText,
                            defaultValue: "0");
                        parameters[VisibleTranquilityText] = cNode.Children.GetTextOrDefaultString(VisibleTranquilityText,
                            defaultValue: "0");

                        YamlNode typesNode = new YamlScalarNode(TypesText);
                        if (cNode.Children.ContainsKey(typesNode))
                        {
                            YamlSequenceNode typeIDsNode = cNode.Children[typesNode] as YamlSequenceNode;

                            parameters[TypeIDText] = typeIDsNode != null
                                ? typeIDsNode.Count() == 1
                                    ? typeIDsNode.Children.First().ToString()
                                    : Database.DbNull
                                : Database.DbNull;
                        }

                        command.CommandText = Database.SqlInsertCommandText(SknSkinsTableName, parameters);
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
