using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace EVEMon.SDEExternalsToSql.YamlToSql.Tables
{
    internal static class SkinLicenses
    {
        private const string SknLicensesTableName = "sknLicenses";

        private const string LicenseTypeIDText = "licenseTypeID";
        private const string SkinIDText = "skinID";
        private const string DurationText = "duration";

        /// <summary>
        /// Imports the skin licenses.
        /// </summary>
        internal static void Import()
        {
            DateTime startTime = DateTime.Now;
            Util.ResetCounters();

            var yamlFile = YamlFilesConstants.skinLicenses;
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

            Database.CreateTable(SknLicensesTableName);

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
                        parameters[LicenseTypeIDText] = pair.Key.ToString();
                        parameters[SkinIDText] = cNode.Children.Keys.Any(key => key.ToString() == SkinIDText)
                            ? cNode.Children[new YamlScalarNode(SkinIDText)].ToString()
                            : Database.Null;
                        parameters[DurationText] = cNode.Children.Keys.Any(key => key.ToString() == DurationText)
                            ? cNode.Children[new YamlScalarNode(DurationText)].ToString()
                            : "-1";

                        command.CommandText = Database.SqlInsertCommandText(SknLicensesTableName, parameters);
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
