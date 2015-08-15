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
                        parameters[SkinMaterialIDText] = pair.Key.ToString();
                        parameters[MaterialText] = cNode.Children.GetTextOrDefaultString(MaterialText,
                            defaultValue: Database.StringEmpty, isUnicode: true);
                        parameters[DisplayNameIDText] = cNode.Children.GetTextOrDefaultString(DisplayNameIDText);
                        parameters[ColorHullText] = cNode.Children.GetTextOrDefaultString(ColorHullText);
                        parameters[ColorWindowText] = cNode.Children.GetTextOrDefaultString(ColorWindowText);
                        parameters[ColorPrimaryText] = cNode.Children.GetTextOrDefaultString(ColorPrimaryText);
                        parameters[ColorSecondaryText] = cNode.Children.GetTextOrDefaultString(ColorSecondaryText);

                        command.CommandText = Database.SqlInsertCommandText(SknMaterialsTableName, parameters);
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
