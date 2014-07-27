using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace EVEMon.SDEExternalsToSql.YamlToSql.Tables
{
    /// <summary>
    /// Represents a certificate grade.
    /// </summary>
    internal enum CertificateGrade
    {
        None = 0,
        Basic = 1,
        Standard = 2,
        Improved = 3,
        Advanced = 4,
        Elite = 5
    }

    internal static class Certificates
    {
        private const string CrtClassesTableName = "crtClasses";
        private const string CrtCertificateTableName = "crtCertificates";
        private const string CrtRecommendationsTableName = "crtRecommendations";
        private const string CrtRelationshipsTableName = "crtRelationships";

        // certificates.yaml
        private const string DescriptionText = "description";
        private const string GroupIDText = "groupID";
        private const string NameText = "name";
        private const string RecommendedForText = "recommendedFor";
        private const string SkillTypesText = "skillTypes";

        // crtCertificates
        private const string CertificateIDText = "certificateID";

        // crtClasses
        private const string ClassIDText = "classID";
        private const string ClassNameText = "className";

        // crtRecommendations
        private const string ShipTypeIDText = "shipTypeID";

        // crtRecommendations
        private const string ParentTypeIDText = "parentTypeID";
        private const string ParentLevelText = "parentLevel";
        private const string ChildIDText = "childID";
        private const string GradeText = "grade";

        /// <summary>
        /// Imports the certificates.
        /// </summary>
        public static void Import()
        {
            DateTime startTime = DateTime.Now;
            Util.ResetCounters();

            var yamlFile = YamlFilesConstants.certificates;
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

            Database.CreateTable(CrtClassesTableName);
            Database.CreateTable(CrtCertificateTableName);
            Database.CreateTable(CrtRecommendationsTableName);
            Database.CreateTable(CrtRelationshipsTableName);

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
            int classId = 0;

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

                        YamlNode nameNode = new YamlScalarNode(NameText);
                        if (cNode.Children.ContainsKey(nameNode))
                        {
                            classId++;
                            parameters = new Dictionary<string, string>();
                            parameters[ClassIDText] = classId.ToString(CultureInfo.InvariantCulture);
                            parameters[ClassNameText] = cNode.Children.Keys.Any(key => key.ToString() == NameText)
                            ? String.Format("'{0}'",
                                cNode.Children[new YamlScalarNode(NameText)].ToString().Replace("'", Database.StringEmpty))
                            : Database.StringEmpty;
                            parameters[DescriptionText] = cNode.Children.Keys.Any(key => key.ToString() == NameText)
                            ? String.Format("'{0}'",
                                cNode.Children[new YamlScalarNode(NameText)].ToString().Replace("'", Database.StringEmpty))
                            : Database.StringEmpty;

                            command.CommandText = Database.SqlInsertCommandText(CrtClassesTableName, parameters);
                            command.ExecuteNonQuery();
                        }

                        YamlNode recommendedForNode = new YamlScalarNode(RecommendedForText);
                        if (cNode.Children.ContainsKey(recommendedForNode))
                        {
                            YamlSequenceNode recNode = cNode.Children[recommendedForNode] as YamlSequenceNode;

                            if (recNode == null)
                                continue;

                            foreach (YamlNode recommendation in recNode)
                            {
                                parameters = new Dictionary<string, string>();
                                parameters[ShipTypeIDText] = recommendation.ToString();
                                parameters[CertificateIDText] = pair.Key.ToString();

                                command.CommandText = Database.SqlInsertCommandText(CrtRecommendationsTableName, parameters);
                                command.ExecuteNonQuery();
                            }
                        }

                        YamlNode skillTypesNode = new YamlScalarNode(SkillTypesText);
                        if (cNode.Children.ContainsKey(skillTypesNode))
                        {
                            YamlMappingNode stNode = cNode.Children[skillTypesNode] as YamlMappingNode;

                            if (stNode == null)
                                continue;

                            foreach (KeyValuePair<YamlNode, YamlNode> skillType in stNode)
                            {
                                YamlMappingNode grNode = skillType.Value as YamlMappingNode;

                                if (grNode == null)
                                    continue;

                                foreach (KeyValuePair<YamlNode, YamlNode> grade in grNode)
                                {
                                    parameters = new Dictionary<string, string>();
                                    parameters[ParentTypeIDText] = skillType.Key.ToString();
                                    parameters[ParentLevelText] = grade.Value.ToString();
                                    parameters[ChildIDText] = pair.Key.ToString();
                                    parameters[GradeText] =
                                        ((int)Enum.Parse(typeof(CertificateGrade), grade.Key.ToString(), true))
                                            .ToString(CultureInfo.InvariantCulture);

                                    command.CommandText = Database.SqlInsertCommandText(CrtRelationshipsTableName, parameters);
                                    command.ExecuteNonQuery();
                                }
                            }
                        }

                        parameters = new Dictionary<string, string>();
                        parameters[CertificateIDText] = pair.Key.ToString();
                        parameters[GroupIDText] = cNode.Children.Keys.Any(key => key.ToString() == GroupIDText)
                            ? cNode.Children[new YamlScalarNode(GroupIDText)].ToString()
                            : Database.Null;
                        parameters[ClassIDText] = classId.ToString(CultureInfo.InvariantCulture);
                        parameters[DescriptionText] = cNode.Children.Keys.Any(key => key.ToString() == DescriptionText)
                            ? String.Format("'{0}'",
                                cNode.Children[new YamlScalarNode(DescriptionText)].ToString().Replace("'", Database.StringEmpty))
                            : Database.StringEmpty;

                        command.CommandText = Database.SqlInsertCommandText(CrtCertificateTableName, parameters);
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