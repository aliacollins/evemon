using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace EVEMon.SDEToSQL.YamlToSQL.Tables
{
    /// <summary>
    /// Represents a certificate grade.
    /// </summary>
    enum CertificateGrade
    {
        None = 0,
        Basic = 1,
        Standard = 2,
        Improved = 3,
        Advanced = 4,
        Elite = 5
    }

    internal class Certificates
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
            if (Program.IsClosing)
                return;

            Stopwatch stopwatch = Stopwatch.StartNew();
            Util.ResetCounters();

            string yamlFile = YamlFilesConstants.certificates;
            string filePath = Util.CheckYamlFileExists(yamlFile);

            if (String.IsNullOrEmpty(filePath))
                return;

            string text = String.Format("Parsing {0}... ", yamlFile);
            Console.Write(text);
            YamlMappingNode rNode = Util.ParseYamlFile(filePath);

            if (rNode == null)
            {
                Console.WriteLine(@"Unable to parse {0}.", yamlFile);
                return;
            }

            Console.SetCursorPosition(Console.CursorLeft - text.Length, Console.CursorTop);

            Console.Write(@"Importing {0}... ", yamlFile);

            Database.DropAndCreateTable(CrtClassesTableName);
            Database.DropAndCreateTable(CrtCertificateTableName);
            Database.DropAndCreateTable(CrtRecommendationsTableName);
            Database.DropAndCreateTable(CrtRelationshipsTableName);

            ImportDataBulk(rNode);

            Util.DisplayEndTime(stopwatch);

            Console.WriteLine();
        }

        /// <summary>
        /// Imports the data bulk.
        /// </summary>
        /// <param name="rNode">The r node.</param>
        private static void ImportDataBulk(YamlMappingNode rNode)
        {
            Util.UpdatePercentDone(0);

            DataTable crtCertificatesTable = GetCrtCertificatesDataTable();
            DataTable crtClassesTable = GetCrtClassesDataTable();
            DataTable crtRecommendationsTable = GetCrtRecommendationsDataTable();
            DataTable crtRelationshipsTable = GetCrtRelationshipsDataTable();

            int classId = 0;
            int total = rNode.Count();
            total = (int)Math.Ceiling(total + (total * 0.01));

            foreach (KeyValuePair<YamlNode, YamlNode> pair in rNode.Children)
            {
                Util.UpdatePercentDone(total);

                YamlMappingNode cNode = pair.Value as YamlMappingNode;

                if (cNode == null)
                    continue;

                ImportClasses(cNode, crtClassesTable, ref classId);

                ImportRecommendations(cNode, crtRecommendationsTable, pair);

                ImportRelationships(cNode, crtRelationshipsTable, pair);

                DataRow row = crtCertificatesTable.NewRow();
                row[CertificateIDText] = SqlInt32.Parse(pair.Key.ToString());
                row[GroupIDText] = cNode.Children.GetSqlTypeOrDefault<SqlInt16>(GroupIDText);
                row[ClassIDText] = classId;
                row[DescriptionText] = cNode.Children.GetSqlTypeOrDefault<SqlString>(DescriptionText, defaultValue: "");

                crtCertificatesTable.Rows.Add(row);
            }

            Database.ImportDataBulk(CrtClassesTableName, crtClassesTable);
            Database.ImportDataBulk(CrtCertificateTableName, crtCertificatesTable);
            Database.ImportDataBulk(CrtRecommendationsTableName, crtRecommendationsTable);
            Database.ImportDataBulk(CrtRelationshipsTableName, crtRelationshipsTable);

            Util.UpdatePercentDone(crtCertificatesTable.Rows.Count);
        }

        /// <summary>
        /// Imports the relationships.
        /// </summary>
        /// <param name="cNode">The c node.</param>
        /// <param name="crtRelationshipsTable">The CRT relationships table.</param>
        /// <param name="pair">The pair.</param>
        private static void ImportRelationships(YamlMappingNode cNode, DataTable crtRelationshipsTable,
            KeyValuePair<YamlNode, YamlNode> pair)
        {
            YamlNode skillTypesNode = new YamlScalarNode(SkillTypesText);
            if (cNode.Children.ContainsKey(skillTypesNode))
            {
                YamlMappingNode stNode = cNode.Children[skillTypesNode] as YamlMappingNode;

                if (stNode == null)
                    return;

                foreach (KeyValuePair<YamlNode, YamlNode> skillType in stNode)
                {
                    YamlMappingNode grNode = skillType.Value as YamlMappingNode;

                    if (grNode == null)
                        continue;

                    foreach (KeyValuePair<YamlNode, YamlNode> grade in grNode)
                    {
                        DataRow row = crtRelationshipsTable.NewRow();
                        row[ParentTypeIDText] = SqlInt32.Parse(skillType.Key.ToString());
                        row[ParentLevelText] = SqlByte.Parse(grade.Value.ToString());
                        row[ChildIDText] = SqlInt32.Parse(pair.Key.ToString());
                        row[GradeText] = SqlByte.Parse(
                            ((byte)(int)Enum.Parse(typeof(CertificateGrade), grade.Key.ToString(), true))
                                .ToString());

                        crtRelationshipsTable.Rows.Add(row);
                    }
                }
            }
        }

        /// <summary>
        /// Imports the recommendations.
        /// </summary>
        /// <param name="cNode">The c node.</param>
        /// <param name="crtRecommendationsTable">The CRT recommendations table.</param>
        /// <param name="pair">The pair.</param>
        private static void ImportRecommendations(YamlMappingNode cNode, DataTable crtRecommendationsTable,
            KeyValuePair<YamlNode, YamlNode> pair)
        {
            YamlNode recommendedForNode = new YamlScalarNode(RecommendedForText);
            if (cNode.Children.ContainsKey(recommendedForNode))
            {
                YamlSequenceNode recNode = cNode.Children[recommendedForNode] as YamlSequenceNode;

                if (recNode == null)
                    return;

                foreach (YamlNode recommendation in recNode.Distinct())
                {
                    DataRow row = crtRecommendationsTable.NewRow();
                    row[ShipTypeIDText] = SqlInt32.Parse(recommendation.ToString());
                    row[CertificateIDText] = SqlInt32.Parse(pair.Key.ToString());

                    crtRecommendationsTable.Rows.Add(row);
                }
            }
        }

        /// <summary>
        /// Imports the classes.
        /// </summary>
        /// <param name="cNode">The c node.</param>
        /// <param name="crtClassesTable">The CRT classes table.</param>
        /// <param name="classId">The class identifier.</param>
        private static void ImportClasses(YamlMappingNode cNode, DataTable crtClassesTable, ref int classId)
        {
            YamlNode nameNode = new YamlScalarNode(NameText);
            if (cNode.Children.ContainsKey(nameNode))
            {
                classId++;
                DataRow row = crtClassesTable.NewRow();
                row[ClassIDText] = classId;
                row[DescriptionText] = cNode.Children.GetSqlTypeOrDefault<SqlString>(NameText, defaultValue: "");
                row[ClassNameText] = cNode.Children.GetSqlTypeOrDefault<SqlString>(NameText, defaultValue: "");

                crtClassesTable.Rows.Add(row);
            }
        }

        /// <summary>
        /// Gets the data table for the crtRelationships table.
        /// </summary>
        /// <returns></returns>
        private static DataTable GetCrtRelationshipsDataTable()
        {
            DataTable crtRelationshipsTable = new DataTable();
            crtRelationshipsTable.Columns.AddRange(
                new[]
                {
                    new DataColumn("relationshipID", typeof(SqlInt32)),
                    new DataColumn("parentID", typeof(SqlInt32)),
                    new DataColumn(ParentTypeIDText, typeof(SqlInt32)),
                    new DataColumn(ParentLevelText, typeof(SqlByte)),
                    new DataColumn(ChildIDText, typeof(SqlInt32)),
                    new DataColumn(GradeText, typeof(SqlByte)),
                });
            return crtRelationshipsTable;
        }

        /// <summary>
        /// Gets the data table for the crtRecommendations table.
        /// </summary>
        /// <returns></returns>
        private static DataTable GetCrtRecommendationsDataTable()
        {
            DataTable crtRecommendationsTable = new DataTable();
            crtRecommendationsTable.Columns.AddRange(
                new[]
                {
                    new DataColumn("recommendationID", typeof(SqlInt32)),
                    new DataColumn(ShipTypeIDText, typeof(SqlInt32)),
                    new DataColumn(CertificateIDText, typeof(SqlInt32)),
                });
            return crtRecommendationsTable;
        }

        /// <summary>
        /// Gets the data table for the crtClasses table.
        /// </summary>
        /// <returns></returns>
        private static DataTable GetCrtClassesDataTable()
        {
            DataTable crtClassesTable = new DataTable();
            crtClassesTable.Columns.AddRange(
                new[]
                {
                    new DataColumn(ClassIDText, typeof(SqlInt32)),
                    new DataColumn(DescriptionText, typeof(SqlString)),
                    new DataColumn(ClassNameText, typeof(SqlString)),
                });
            return crtClassesTable;
        }

        /// <summary>
        /// Gets the data table for the crtCertificates table.
        /// </summary>
        /// <returns></returns>
        private static DataTable GetCrtCertificatesDataTable()
        {
            DataTable crtCertificatesTable = new DataTable();
            crtCertificatesTable.Columns.AddRange(
                new[]
                {
                    new DataColumn(CertificateIDText, typeof(SqlInt32)),
                    new DataColumn(GroupIDText, typeof(SqlInt16)),
                    new DataColumn(ClassIDText, typeof(SqlInt32)),

                    // Not used columns
                    new DataColumn(GradeText, typeof(SqlByte)),
                    new DataColumn("corpID", typeof(SqlInt32)),
                    new DataColumn("iconID", typeof(SqlInt32)),
                    new DataColumn(DescriptionText, typeof(SqlString)),
                });
            return crtCertificatesTable;
        }
    }
}