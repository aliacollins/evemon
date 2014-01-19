using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace EVEMon.YamlToSql.Tables
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

        /// <summary>
        /// Imports the certificates.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public static void ImportCertificates(SqlConnection connection)
        {
            DateTime startTime = DateTime.Now;
            Util.ResetCounters();

            var yamlFile = YamlFilesConstants.certificates;
            var filePath = Util.CheckYamlFileExists(yamlFile);

            if (String.IsNullOrEmpty(filePath))
                return;


            var command = new SqlCommand { Connection = connection };

            CreateCertTables(connection, command);

            Console.WriteLine();
            Console.Write(@"Importing {0}... ", yamlFile);

            YamlMappingNode rNode = Util.ParseYamlFile(filePath);

            if (rNode == null)
            {
                Console.WriteLine(@"Unable to parse {0}.", yamlFile);
                return;
            }

            ImportCertificatesData(connection, command, rNode, startTime);

            Console.WriteLine();
        }

        /// <summary>
        /// Imports the certificates data.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="command">The command.</param>
        /// <param name="rNode">The r node.</param>
        /// <param name="startTime">The start time.</param>
        private static void ImportCertificatesData(SqlConnection connection, SqlCommand command, YamlMappingNode rNode,
            DateTime startTime)
        {
            // certificates.yaml
            const string DescriptionText = "description";
            const string GroupIDText = "groupID";
            const string NameText = "name";
            const string RecommendedForText = "recommendedFor";
            const string SkillTypesText = "skillTypes";

            // crtCertificates
            const string CertificateIDText = "certificateID";
            const string CategoryIDText = "categoryID";

            // crtClasses
            int classId = 0;
            const string ClassIDText = "classID";
            const string ClassNameText = "className";

            // crtRecommendations
            int recommendationId = 0;
            const string RecommendationIDText = "recommendationID";
            const string ShipTypeIDText = "shipTypeID";

            // crtRecommendations
            int relationshipId = 0;
            const string RelationshipIDText = "relationshipID";
            const string ParentTypeIDText = "parentTypeID";
            const string ParentLevelText = "parentLevel";
            const string ChildIDText = "childID";
            const string GradeText = "grade";

            using (var tx = connection.BeginTransaction())
            {
                command.Transaction = tx;
                try
                {
                    foreach (KeyValuePair<YamlNode, YamlNode> pair in rNode.Children)
                    {
                        Util.UpdatePercentDone(rNode.Count());

                        string certificateId = ((YamlScalarNode)pair.Key).Value;
                        string groupID = "null";
                        string description = String.Format("'{0}'", String.Empty);
                        Dictionary<string, string> parameters;

                        YamlMappingNode cNode = rNode.Children[pair.Key] as YamlMappingNode;

                        if (cNode == null)
                            continue;

                        YamlNode descriptionNode = new YamlScalarNode(DescriptionText);
                        if (cNode.Children.ContainsKey(descriptionNode))
                        {
                            description = String.Format("'{0}'",
                                ((YamlScalarNode)cNode.Children[descriptionNode]).Value.Replace("'", "''"));
                        }

                        YamlNode groupIDNode = new YamlScalarNode(GroupIDText);
                        if (cNode.Children.ContainsKey(groupIDNode))
                            groupID = ((YamlScalarNode)cNode.Children[groupIDNode]).Value;

                        YamlNode nameNode = new YamlScalarNode(NameText);
                        if (cNode.Children.ContainsKey(nameNode))
                        {
                            string name = String.Format("'{0}'",
                                ((YamlScalarNode)cNode.Children[nameNode]).Value.Replace("'", "''"));

                            classId++;
                            parameters = new Dictionary<string, string>();
                            parameters[ClassIDText] = classId.ToString(CultureInfo.InvariantCulture);
                            parameters[ClassNameText] = name;
                            parameters[DescriptionText] = name;

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
                                recommendationId++;
                                parameters = new Dictionary<string, string>();
                                parameters[RecommendationIDText] = recommendationId.ToString(CultureInfo.InvariantCulture);
                                parameters[ShipTypeIDText] = ((YamlScalarNode)recommendation).Value;
                                parameters[CertificateIDText] = certificateId;

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
                                    relationshipId++;
                                    parameters = new Dictionary<string, string>();
                                    parameters[RelationshipIDText] = relationshipId.ToString(CultureInfo.InvariantCulture);
                                    parameters[ParentTypeIDText] = ((YamlScalarNode)skillType.Key).Value;
                                    parameters[ParentLevelText] = ((YamlScalarNode)grade.Value).Value;
                                    parameters[ChildIDText] = certificateId;
                                    parameters[GradeText] =
                                        ((int)Enum.Parse(typeof(CertificateGrade), ((YamlScalarNode)grade.Key).Value, true))
                                            .ToString(CultureInfo.InvariantCulture);

                                    command.CommandText = Database.SqlInsertCommandText(CrtRelationshipsTableName, parameters);
                                    command.ExecuteNonQuery();
                                }
                            }
                        }

                        parameters = new Dictionary<string, string>();
                        parameters[CertificateIDText] = certificateId;
                        parameters[CategoryIDText] = groupID;
                        parameters[ClassIDText] = classId.ToString(CultureInfo.InvariantCulture);
                        parameters[DescriptionText] = description;

                        command.CommandText = Database.SqlInsertCommandText(CrtCertificateTableName, parameters);
                        command.ExecuteNonQuery();
                    }

                    tx.Commit();
                    Util.DisplayEndTime(startTime);
                }
                catch (SqlException e)
                {
                    tx.Rollback();
                    Console.WriteLine();
                    Console.WriteLine(@"Unable to execute SQL command: {0}", command.CommandText);
                    Console.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// Creates the cert tables.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="command">The command.</param>
        private static void CreateCertTables(SqlConnection connection, SqlCommand command)
        {
            DataTable dataTable = connection.GetSchema("columns");

            if (dataTable.Select(String.Format("TABLE_NAME = '{0}'", CrtClassesTableName)).Length == 0)
                Database.CreateTable(command, CrtClassesTableName);
            else
            {
                Database.DropTable(command, CrtClassesTableName);
                Database.CreateTable(command, CrtClassesTableName);
            }

            if (dataTable.Select(String.Format("TABLE_NAME = '{0}'", CrtCertificateTableName)).Length == 0)
                Database.CreateTable(command, CrtCertificateTableName);
            else
            {
                Database.DropTable(command, CrtCertificateTableName);
                Database.CreateTable(command, CrtCertificateTableName);
            }

            if (dataTable.Select(String.Format("TABLE_NAME = '{0}'", CrtRecommendationsTableName)).Length == 0)
                Database.CreateTable(command, CrtRecommendationsTableName);
            else
            {
                Database.DropTable(command, CrtRecommendationsTableName);
                Database.CreateTable(command, CrtRecommendationsTableName);
            }

            if (dataTable.Select(String.Format("TABLE_NAME = '{0}'", CrtRelationshipsTableName)).Length == 0)
                Database.CreateTable(command, CrtRelationshipsTableName);
            else
            {
                Database.DropTable(command, CrtRelationshipsTableName);
                Database.CreateTable(command, CrtRelationshipsTableName);
            }
        }
    }
}