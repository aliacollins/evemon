using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace EVEMon.SDEExternalsToSql.YamlToSql.Tables
{
    internal enum Activity
    {
        None = 0,

        [Description("manufacturing")]
        Manufacturing = 1,

        [Description("research_technology")]
        ResearchingTechnology = 2,

        [Description("research_time")]
        ResearchingTimeEfficiency = 3,

        [Description("research_material")]
        ResearchingMaterialEfficiency = 4,

        [Description("copying")]
        Copying = 5,

        [Description("duplicating")]
        Duplicating = 6,

        [Description("reverse_engineering")]
        ReverseEngineering = 7,

        [Description("invention")]
        Invention = 8
    }

    internal static class Blueprints
    {
        private const string InvBlueprintTypesTableName = "invBlueprintTypes";
        private const string RamTypeRequirementsTableName = "ramTypeRequirements";

        // blueprints.yaml
        private const string ActivitiesText = "activities";
        private const string BlueprintTypeIDText = "blueprintTypeID";
        private const string MaxProductionLimitText = "maxProductionLimit";
        private const string MaterialsText = "materials";
        private const string ProductsText = "products";
        private const string SkillsText = "skills";
        private const string TimeText = "time";
        private const string QuantityText = "quantity";
        private const string ProbabilityText = "probability";
        private const string RaceIDText = "raceID";
        private const string LevelText = "level";
        private const string ConsumeText = "consume";

        // invBlueprintTypes
        private const string IbtBlueprintTypeIDText = "blueprintTypeID";
        private const string ProductTypeIDText = "productTypeID";
        private const string ProductionTimeText = "productionTime";
        private const string ResearchProductivityTimeText = "researchProductivityTime";
        private const string ResearchMaterialTimeText = "researchMaterialTime";
        private const string ResearchCopyTimeText = "researchCopyTime";
        private const string ResearchTechTimeText = "researchTechTime";
        private const string DuplicatingTimeText = "duplicatingTime";
        private const string ReverseEngineeringTimeText = "reverseEngineeringTime";
        private const string InventionTimeText = "inventionTime";
        private const string IbtMaxProductionLimitText = "maxProductionLimit";

        // ramTypeRequirements
        private const string TypeIDText = "typeID";
        private const string ActivityIDText = "activityID";
        private const string RequiredTypeIDText = "requiredTypeID";
        private const string RtrQuantityText = "quantity";
        private const string RtrLevelText = "level";
        private const string RtrRaceIDText = "raceID";
        private const string RtrProbabilityText = "probability";
        private const string RtrConsumeText = "consume";


        public static void Import()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            Util.ResetCounters();

            var yamlFile = YamlFilesConstants.blueprints;
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

            Database.CreateTable(InvBlueprintTypesTableName);
            Database.CreateTable(RamTypeRequirementsTableName);

            ImportData(rNode);

            Util.DisplayEndTime(stopwatch);

            Console.WriteLine();
        }

        private static void ImportData(YamlMappingNode rNode)
        {
            using (IDbCommand command = new SqlCommand(
                String.Empty,
                Database.SqlConnection,
                Database.SqlConnection.BeginTransaction()))
            {
                try
                {
                    YamlNode manActivity = new YamlScalarNode(Activity.Manufacturing.GetDescription());
                    YamlNode rteActivity =
                        new YamlScalarNode(Activity.ResearchingTechnology.GetDescription());
                    YamlNode rtpActivity =
                        new YamlScalarNode(Activity.ResearchingTimeEfficiency.GetDescription());
                    YamlNode rmpActivity =
                        new YamlScalarNode(Activity.ResearchingMaterialEfficiency.GetDescription());
                    YamlNode copActivity = new YamlScalarNode(Activity.Copying.GetDescription());
                    YamlNode dupActivity = new YamlScalarNode(Activity.Duplicating.GetDescription());
                    YamlNode renActivity =
                        new YamlScalarNode(Activity.ReverseEngineering.GetDescription());
                    YamlNode invActivity = new YamlScalarNode(Activity.Invention.GetDescription());

                    foreach (KeyValuePair<YamlNode, YamlNode> pair in rNode.Children)
                    {
                        Util.UpdatePercentDone(rNode.Count());

                        YamlMappingNode cNode = pair.Value as YamlMappingNode;

                        if (cNode == null)
                            continue;

                        String productTypeIDText = Database.DbNull;
                        String productionTimeText = Database.DbNull;
                        String researchTechTimeText = Database.DbNull;
                        String researchProductivityTimeText = Database.DbNull;
                        String researchMaterialTimeText = Database.DbNull;
                        String researchCopyTimeText = Database.DbNull;
                        String duplicatingTimeText = Database.DbNull;
                        String reverseEngeneeringTimeText = Database.DbNull;
                        String inventionTimeText = Database.DbNull;

                        String blueprintTypeIDText = pair.Key.ToString();
                        YamlNode blueprintTypeIDNode = cNode.Children[new YamlScalarNode(BlueprintTypeIDText)];

                        if (blueprintTypeIDText != blueprintTypeIDNode.ToString())
                            throw new Exception(String.Format("Key [{0}] differs from {1}", blueprintTypeIDText, BlueprintTypeIDText));

                        YamlNode activitiesNode = new YamlScalarNode(ActivitiesText);
                        if (cNode.Children.ContainsKey(activitiesNode))
                        {
                            YamlMappingNode activityNode = cNode.Children[activitiesNode] as YamlMappingNode;

                            if (activityNode == null)
                                continue;

                            if (activityNode.Children.ContainsKey(manActivity))
                            {
                                YamlMappingNode actNode = (YamlMappingNode)activityNode.Children[manActivity];

                                if (actNode.Children.Keys.Any(key => key.ToString() == ProductsText))
                                {
                                    productTypeIDText =
                                        ((YamlMappingNode)
                                            ((YamlSequenceNode)actNode.Children[new YamlScalarNode(ProductsText)]).Children
                                                .First()).Children[new YamlScalarNode(TypeIDText)].ToString();
                                }
                            }

                            productionTimeText = activityNode.Children.ContainsKey(manActivity)
                                ? ((YamlMappingNode)activityNode.Children[manActivity]).Children.GetTextOrDefaultString(TimeText)
                                : Database.DbNull;

                            researchTechTimeText = activityNode.Children.ContainsKey(rteActivity)
                                ? ((YamlMappingNode)activityNode.Children[rteActivity]).Children.GetTextOrDefaultString(TimeText)
                                : Database.DbNull;

                            researchProductivityTimeText = activityNode.Children.ContainsKey(rtpActivity)
                                ? ((YamlMappingNode)activityNode.Children[rtpActivity]).Children.GetTextOrDefaultString(TimeText)
                                : Database.DbNull;

                            researchMaterialTimeText = activityNode.Children.ContainsKey(rmpActivity)
                                ? ((YamlMappingNode)activityNode.Children[rmpActivity]).Children.GetTextOrDefaultString(TimeText)
                                : Database.DbNull;

                            researchCopyTimeText = activityNode.Children.ContainsKey(copActivity)
                                ? ((YamlMappingNode)activityNode.Children[copActivity]).Children.GetTextOrDefaultString(TimeText)
                                : Database.DbNull;

                            duplicatingTimeText = activityNode.Children.ContainsKey(dupActivity)
                                ? ((YamlMappingNode)activityNode.Children[dupActivity]).Children.GetTextOrDefaultString(TimeText)
                                : Database.DbNull;

                            reverseEngeneeringTimeText = activityNode.Children.ContainsKey(renActivity)
                                ? ((YamlMappingNode)activityNode.Children[renActivity]).Children.GetTextOrDefaultString(TimeText)
                                : Database.DbNull;

                            inventionTimeText = activityNode.Children.ContainsKey(invActivity)
                                ? ((YamlMappingNode)activityNode.Children[invActivity]).Children.GetTextOrDefaultString(TimeText)
                                : Database.DbNull;

                            foreach (KeyValuePair<YamlNode, YamlNode> activity in activityNode)
                            {
                                if (!activity.Key.Equals(manActivity))
                                    ImportProducts(command, activity, blueprintTypeIDText);

                                ImportMaterials(command, activity, blueprintTypeIDText);
                                ImportSkills(command, activity, blueprintTypeIDText);
                            }
                        }

                        Dictionary<string, string> parameters = new Dictionary<string, string>();
                        parameters[IbtBlueprintTypeIDText] = blueprintTypeIDText;
                        parameters[ProductTypeIDText] = productTypeIDText;
                        parameters[ProductionTimeText] = productionTimeText;
                        parameters[ResearchProductivityTimeText] = researchProductivityTimeText;
                        parameters[ResearchMaterialTimeText] = researchMaterialTimeText;
                        parameters[ResearchCopyTimeText] = researchCopyTimeText;
                        parameters[ResearchTechTimeText] = researchTechTimeText;
                        parameters[DuplicatingTimeText] = duplicatingTimeText;
                        parameters[ReverseEngineeringTimeText] = reverseEngeneeringTimeText;
                        parameters[InventionTimeText] = inventionTimeText;
                        parameters[IbtMaxProductionLimitText] = cNode.Children.GetTextOrDefaultString(MaxProductionLimitText);

                        command.CommandText = Database.SqlInsertCommandText(InvBlueprintTypesTableName, parameters);
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

        private static void ImportProducts(IDbCommand command, KeyValuePair<YamlNode, YamlNode> activity,
            String blueprintTypeIDText)
        {
            if (String.IsNullOrWhiteSpace(blueprintTypeIDText) || blueprintTypeIDText == Database.DbNull)
                return;

            YamlMappingNode actNode = activity.Value as YamlMappingNode;

            if (actNode == null)
                return;

            YamlNode productsNode = new YamlScalarNode(ProductsText);
            if (!actNode.Children.ContainsKey(productsNode))
                return;

            YamlSequenceNode prodsNode = actNode.Children[productsNode] as YamlSequenceNode;

            if (prodsNode == null)
                return;

            foreach (YamlNode product in prodsNode.Distinct())
            {
                YamlMappingNode prodNode = product as YamlMappingNode;

                if (prodNode == null)
                    continue;

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters[TypeIDText] = blueprintTypeIDText;
                parameters[ActivityIDText] = activity.Key.GetValueOrDefaultString<Activity>();
                parameters[RequiredTypeIDText] = prodNode.Children[new YamlScalarNode(TypeIDText)].ToString();
                parameters[RtrQuantityText] = prodNode.Children.GetTextOrDefaultString(QuantityText);
                parameters[RtrProbabilityText] = prodNode.Children.GetTextOrDefaultString(ProbabilityText);
                parameters[RtrRaceIDText] = prodNode.Children.GetTextOrDefaultString(RaceIDText);

                command.CommandText = Database.SqlInsertCommandText(RamTypeRequirementsTableName, parameters);
                command.ExecuteNonQuery();
            }
        }

        private static void ImportMaterials(IDbCommand command, KeyValuePair<YamlNode, YamlNode> activity,
            String productTypeIDText)
        {
            if (String.IsNullOrWhiteSpace(productTypeIDText) || productTypeIDText == Database.DbNull)
                return;

            YamlMappingNode actNode = activity.Value as YamlMappingNode;

            if (actNode == null)
                return;

            YamlNode materialsNode = new YamlScalarNode(MaterialsText);
            if (!actNode.Children.ContainsKey(materialsNode))
                return;

            YamlSequenceNode matsNode = actNode.Children[materialsNode] as YamlSequenceNode;

            if (matsNode == null)
                return;

            foreach (YamlNode material in matsNode.Distinct())
            {
                YamlMappingNode matNode = material as YamlMappingNode;

                if (matNode == null)
                    continue;

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters[TypeIDText] = productTypeIDText;
                parameters[ActivityIDText] = activity.Key.GetValueOrDefaultString<Activity>();
                parameters[RequiredTypeIDText] = matNode.Children[new YamlScalarNode(TypeIDText)].ToString();
                parameters[RtrQuantityText] = matNode.Children.GetTextOrDefaultString(QuantityText);
                parameters[RtrConsumeText] = matNode.Children.GetTextOrDefaultString(ConsumeText);

                command.CommandText = Database.SqlInsertCommandText(RamTypeRequirementsTableName, parameters);
                command.ExecuteNonQuery();
            }
        }

        private static void ImportSkills(IDbCommand command, KeyValuePair<YamlNode, YamlNode> activity, String productTypeIDText)
        {
            if (String.IsNullOrWhiteSpace(productTypeIDText) || productTypeIDText == Database.DbNull)
                return;

            YamlMappingNode actNode = activity.Value as YamlMappingNode;

            if (actNode == null)
                return;

            YamlNode skillsNode = new YamlScalarNode(SkillsText);
            if (!actNode.Children.ContainsKey(skillsNode))
                return;

            YamlSequenceNode sksNode = actNode.Children[skillsNode] as YamlSequenceNode;

            if (sksNode == null)
                return;

            foreach (YamlNode skill in sksNode.Distinct())
            {
                YamlMappingNode skillNode = skill as YamlMappingNode;

                if (skillNode == null)
                    continue;

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters[TypeIDText] = productTypeIDText;
                parameters[ActivityIDText] = activity.Key.GetValueOrDefaultString<Activity>();
                parameters[RequiredTypeIDText] = skillNode.Children[new YamlScalarNode(TypeIDText)].ToString();
                parameters[RtrLevelText] = skillNode.Children.GetTextOrDefaultString(LevelText);

                command.CommandText = Database.SqlInsertCommandText(RamTypeRequirementsTableName, parameters);
                command.ExecuteNonQuery();
            }
        }
    }
}
