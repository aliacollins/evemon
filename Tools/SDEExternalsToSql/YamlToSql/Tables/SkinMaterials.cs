using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace EVEMon.SDEExternalsToSql.YamlToSql.Tables
{
    internal static class SkinMaterials
    {
        private const string SknMaterialsTableName = "sknMaterials";

        private const string SkinMaterialIDText = "skinMaterialID";
        private const string MaterialText = "material";
        private const string DisplayNameIDText = "displayNameID";
        private const string ColorHullText = "colorHull";
        private const string ColorWindowText = "colorWindow";
        private const string ColorPrimaryText = "colorPrimary";
        private const string ColorSecondaryText = "colorSecondary";

        /// <summary>
        /// Imports the skin materials.
        /// </summary>
        internal static void Import()
        {
            DateTime startTime = DateTime.Now;
            Util.ResetCounters();

            var yamlFile = YamlFilesConstants.skinMaterials;
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

            Database.CreateTable(SknMaterialsTableName);

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
                        parameters[SkinMaterialIDText] = pair.Key.ToString();
                        parameters[MaterialText] = cNode.Children.Keys.Any(key => key.ToString() == MaterialText)
                            ? String.Format("N'{0}'",
                                cNode.Children[new YamlScalarNode(MaterialText)].ToString()
                                    .Replace("'", Database.StringEmpty))
                            : Database.StringEmpty;
                        parameters[DisplayNameIDText] = cNode.Children.Keys.Any(key => key.ToString() == DisplayNameIDText)
                            ? cNode.Children[new YamlScalarNode(DisplayNameIDText)].ToString()
                            : Database.Null;
                        parameters[ColorHullText] = cNode.Children.Keys.Any(key => key.ToString() == ColorHullText)
                            ? String.Format("'{0}'",
                                cNode.Children[new YamlScalarNode(ColorHullText)].ToString()
                                    .Replace("'", Database.StringEmpty))
                            : Database.Null;
                        parameters[ColorWindowText] = cNode.Children.Keys.Any(key => key.ToString() == ColorWindowText)
                            ? String.Format("'{0}'",
                                cNode.Children[new YamlScalarNode(ColorWindowText)].ToString()
                                    .Replace("'", Database.StringEmpty))
                            : Database.Null;
                        parameters[ColorPrimaryText] = cNode.Children.Keys.Any(key => key.ToString() == ColorPrimaryText)
                            ? String.Format("'{0}'",
                                cNode.Children[new YamlScalarNode(ColorPrimaryText)].ToString()
                                    .Replace("'", Database.StringEmpty))
                            : Database.Null;
                        parameters[ColorSecondaryText] = cNode.Children.Keys.Any(key => key.ToString() == ColorSecondaryText)
                            ? String.Format("'{0}'",
                                cNode.Children[new YamlScalarNode(ColorSecondaryText)].ToString()
                                    .Replace("'", Database.StringEmpty))
                            : Database.Null;

                        command.CommandText = Database.SqlInsertCommandText(SknMaterialsTableName, parameters);
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
