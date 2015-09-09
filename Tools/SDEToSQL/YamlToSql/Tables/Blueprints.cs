using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace EVEMon.SDEToSQL.YamlToSQL.Tables
{
    enum Activity
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

    internal class Blueprints
    {
        private const string InvBlueprintTypesTableName = "invBlueprintTypes";
        private const string RamTypeRequirementsTableName = "ramTypeRequirements";

        private const string BlueprintTypeIDText = "blueprintTypeID";
        private const string MaxProductionLimitText = "maxProductionLimit";
        private const string QuantityText = "quantity";
        private const string LevelText = "level";
        private const string RaceIDText = "raceID";
        private const string ProbabilityText = "probability";
        private const string ConsumeText = "consume";

        // blueprints.yaml
        private const string ActivitiesText = "activities";
        private const string MaterialsText = "materials";
        private const string ProductsText = "products";
        private const string SkillsText = "skills";
        private const string TimeText = "time";

        // invBlueprintTypes
        private const string ParentBlueprintTypeIDText = "parentBlueprintTypeID";
        private const string ProductTypeIDText = "productTypeID";
        private const string ProductionTimeText = "productionTime";
        private const string TechLevelText = "techLevel";
        private const string ResearchProductivityTimeText = "researchProductivityTime";
        private const string ResearchMaterialTimeText = "researchMaterialTime";
        private const string ResearchCopyTimeText = "researchCopyTime";
        private const string ResearchTechTimeText = "researchTechTime";
        private const string DuplicatingTimeText = "duplicatingTime";
        private const string ReverseEngineeringTimeText = "reverseEngineeringTime";
        private const string InventionTimeText = "inventionTime";
        private const string ProductivityModifierText = "productivityModifier";
        private const string MaterialModifierText = "materialModifier";
        private const string WasteFactorText = "wasteFactor";

        // ramTypeRequirements
        private const string TypeIDText = "typeID";
        private const string ActivityIDText = "activityID";
        private const string RequiredTypeIDText = "requiredTypeID";
        private const string DamagePerJobText = "damagePerJob";
        private const string RecycleText = "recycle";


        public static void Import()
        {
            if (Program.IsClosing)
                return;

            Stopwatch stopwatch = Stopwatch.StartNew();
            Util.ResetCounters();

            string yamlFile = YamlFilesConstants.blueprints;
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

            Database.DropAndCreateTable(InvBlueprintTypesTableName);
            Database.DropAndCreateTable(RamTypeRequirementsTableName);

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

            DataTable invBlueprintTypes = GetInvBlueprintTypesDataTable();
            DataTable ramTypeRequirements = GetRamTypeRequirementsDataTable();

            int total = rNode.Count();
            total = (int)Math.Ceiling(total + (total * 0.01));

            YamlNode manActivity = new YamlScalarNode(Activity.Manufacturing.GetDescription());
            YamlNode rteActivity = new YamlScalarNode(Activity.ResearchingTechnology.GetDescription());
            YamlNode rtpActivity = new YamlScalarNode(Activity.ResearchingTimeEfficiency.GetDescription());
            YamlNode rmpActivity = new YamlScalarNode(Activity.ResearchingMaterialEfficiency.GetDescription());
            YamlNode copActivity = new YamlScalarNode(Activity.Copying.GetDescription());
            YamlNode dupActivity = new YamlScalarNode(Activity.Duplicating.GetDescription());
            YamlNode renActivity = new YamlScalarNode(Activity.ReverseEngineering.GetDescription());
            YamlNode invActivity = new YamlScalarNode(Activity.Invention.GetDescription());

            foreach (KeyValuePair<YamlNode, YamlNode> pair in rNode.Children)
            {
                Util.UpdatePercentDone(total);

                YamlMappingNode cNode = pair.Value as YamlMappingNode;

                if (cNode == null)
                    continue;

                SqlInt32 blueprintTypeIDText = SqlInt32.Parse(pair.Key.ToString());
                YamlNode blueprintTypeIDNode = cNode.Children[new YamlScalarNode(BlueprintTypeIDText)];

                if (blueprintTypeIDText.ToString() != blueprintTypeIDNode.ToString())
                    throw new Exception(String.Format("Key [{0}] differs from {1}", blueprintTypeIDText, BlueprintTypeIDText));

                SqlInt32 productTypeIDText = SqlInt32.Null;
                SqlInt32 productionTimeText = SqlInt32.Null;
                SqlInt32 researchTechTimeText = SqlInt32.Null;
                SqlInt32 researchProductivityTimeText = SqlInt32.Null;
                SqlInt32 researchMaterialTimeText = SqlInt32.Null;
                SqlInt32 researchCopyTimeText = SqlInt32.Null;
                SqlInt32 duplicatingTimeText = SqlInt32.Null;
                SqlInt32 reverseEngeneeringTimeText = SqlInt32.Null;
                SqlInt32 inventionTimeText = SqlInt32.Null;

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
                            productTypeIDText = SqlInt32.Parse(
                                ((YamlMappingNode)
                                    ((YamlSequenceNode)actNode.Children[new YamlScalarNode(ProductsText)]).Children
                                        .First()).Children[new YamlScalarNode(TypeIDText)].ToString());
                        }
                    }

                    productionTimeText = activityNode.Children.ContainsKey(manActivity)
                        ? ((YamlMappingNode)activityNode.Children[manActivity]).Children.GetSqlTypeOrDefault<SqlInt32>(TimeText)
                        : productionTimeText;

                    researchTechTimeText = activityNode.Children.ContainsKey(rteActivity)
                        ? ((YamlMappingNode)activityNode.Children[rteActivity]).Children.GetSqlTypeOrDefault<SqlInt32>(TimeText)
                        : researchTechTimeText;

                    researchProductivityTimeText = activityNode.Children.ContainsKey(rtpActivity)
                        ? ((YamlMappingNode)activityNode.Children[rtpActivity]).Children.GetSqlTypeOrDefault<SqlInt32>(TimeText)
                        : researchProductivityTimeText;

                    researchMaterialTimeText = activityNode.Children.ContainsKey(rmpActivity)
                        ? ((YamlMappingNode)activityNode.Children[rmpActivity]).Children.GetSqlTypeOrDefault<SqlInt32>(TimeText)
                        : researchMaterialTimeText;

                    researchCopyTimeText = activityNode.Children.ContainsKey(copActivity)
                        ? ((YamlMappingNode)activityNode.Children[copActivity]).Children.GetSqlTypeOrDefault<SqlInt32>(TimeText)
                        : researchCopyTimeText;

                    duplicatingTimeText = activityNode.Children.ContainsKey(dupActivity)
                        ? ((YamlMappingNode)activityNode.Children[dupActivity]).Children.GetSqlTypeOrDefault<SqlInt32>(TimeText)
                        : duplicatingTimeText;

                    reverseEngeneeringTimeText = activityNode.Children.ContainsKey(renActivity)
                        ? ((YamlMappingNode)activityNode.Children[renActivity]).Children.GetSqlTypeOrDefault<SqlInt32>(TimeText)
                        : reverseEngeneeringTimeText;

                    inventionTimeText = activityNode.Children.ContainsKey(invActivity)
                        ? ((YamlMappingNode)activityNode.Children[invActivity]).Children.GetSqlTypeOrDefault<SqlInt32>(TimeText)
                        : inventionTimeText;

                    foreach (KeyValuePair<YamlNode, YamlNode> activity in activityNode)
                    {
                        if (!activity.Key.Equals(manActivity))
                            ImportProducts(activity, blueprintTypeIDText, ramTypeRequirements);

                        ImportMaterials(activity, blueprintTypeIDText, ramTypeRequirements);
                        ImportSkills(activity, blueprintTypeIDText, ramTypeRequirements);
                    }
                }

                DataRow row = invBlueprintTypes.NewRow();
                row[BlueprintTypeIDText] = blueprintTypeIDText;
                row[ProductTypeIDText] = productTypeIDText;
                row[ProductionTimeText] = productionTimeText;
                row[ResearchProductivityTimeText] = researchProductivityTimeText;
                row[ResearchMaterialTimeText] = researchMaterialTimeText;
                row[ResearchCopyTimeText] = researchCopyTimeText;
                row[ResearchTechTimeText] = researchTechTimeText;
                row[DuplicatingTimeText] = duplicatingTimeText;
                row[ReverseEngineeringTimeText] = reverseEngeneeringTimeText;
                row[InventionTimeText] = inventionTimeText;
                row[MaxProductionLimitText] = cNode.Children.GetSqlTypeOrDefault<SqlInt32>(MaxProductionLimitText);

                invBlueprintTypes.Rows.Add(row);
            }

            Database.ImportDataBulk(InvBlueprintTypesTableName, invBlueprintTypes);
            Database.ImportDataBulk(RamTypeRequirementsTableName, ramTypeRequirements);

            Util.UpdatePercentDone(invBlueprintTypes.Rows.Count);
        }

        /// <summary>
        /// Imports the products.
        /// </summary>
        /// <param name="activity">The activity.</param>
        /// <param name="productTypeIDText">The product type identifier text.</param>
        /// <param name="ramTypeRequirements">The ram type requirements.</param>
        private static void ImportProducts(KeyValuePair<YamlNode, YamlNode> activity, SqlInt32 productTypeIDText,
            DataTable ramTypeRequirements)
        {
            if (productTypeIDText == SqlInt32.Null)
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

                DataRow row = ramTypeRequirements.NewRow();
                row[TypeIDText] = productTypeIDText;
                row[ActivityIDText] = activity.Key.GetSqlTypeOrDefaultOfEnum<SqlByte, Activity>();
                row[RequiredTypeIDText] = SqlInt32.Parse(prodNode.Children[new YamlScalarNode(TypeIDText)].ToString());
                row[QuantityText] = prodNode.Children.GetSqlTypeOrDefault<SqlInt32>(QuantityText);
                row[ProbabilityText] = prodNode.Children.GetSqlTypeOrDefault<SqlDouble>(ProbabilityText);
                row[RaceIDText] = prodNode.Children.GetSqlTypeOrDefault<SqlInt32>(RaceIDText);

                ramTypeRequirements.Rows.Add(row);
            }
        }

        /// <summary>
        /// Imports the materials.
        /// </summary>
        /// <param name="activity">The activity.</param>
        /// <param name="productTypeIDText">The product type identifier text.</param>
        /// <param name="ramTypeRequirements">The ram type requirements.</param>
        private static void ImportMaterials(KeyValuePair<YamlNode, YamlNode> activity, SqlInt32 productTypeIDText,
            DataTable ramTypeRequirements)
        {
            if (productTypeIDText == SqlInt32.Null)
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

                DataRow row = ramTypeRequirements.NewRow();
                row[TypeIDText] = productTypeIDText;
                row[ActivityIDText] = activity.Key.GetSqlTypeOrDefaultOfEnum<SqlByte, Activity>();
                row[RequiredTypeIDText] = SqlInt32.Parse(matNode.Children[new YamlScalarNode(TypeIDText)].ToString());
                row[QuantityText] = matNode.Children.GetSqlTypeOrDefault<SqlInt32>(QuantityText);
                row[ConsumeText] = matNode.Children.GetSqlTypeOrDefault<SqlBoolean>(ConsumeText);

                ramTypeRequirements.Rows.Add(row);
            }
        }

        /// <summary>
        /// Imports the skills.
        /// </summary>
        /// <param name="activity">The activity.</param>
        /// <param name="productTypeIDText">The product type identifier text.</param>
        /// <param name="ramTypeRequirements">The ram type requirements.</param>
        private static void ImportSkills(KeyValuePair<YamlNode, YamlNode> activity, SqlInt32 productTypeIDText,
            DataTable ramTypeRequirements)
        {
            if (productTypeIDText == SqlInt32.Null)
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

                DataRow row = ramTypeRequirements.NewRow();
                row[TypeIDText] = productTypeIDText;
                row[ActivityIDText] = activity.Key.GetSqlTypeOrDefaultOfEnum<SqlByte, Activity>();
                row[RequiredTypeIDText] = SqlInt32.Parse(skillNode.Children[new YamlScalarNode(TypeIDText)].ToString());
                row[LevelText] = skillNode.Children.GetSqlTypeOrDefault<SqlInt32>(LevelText);

                ramTypeRequirements.Rows.Add(row);
            }
        }

        /// <summary>
        /// Gets the data table for the ramTypeRequirements table.
        /// </summary>
        /// <returns></returns>
        private static DataTable GetRamTypeRequirementsDataTable()
        {
            DataTable ramTypeRequirements = new DataTable();
            ramTypeRequirements.Columns.AddRange(
                new[]
                {
                    new DataColumn(TypeIDText, typeof(SqlInt32)),
                    new DataColumn(ActivityIDText, typeof(SqlByte)),
                    new DataColumn(RequiredTypeIDText, typeof(SqlInt32)),
                    new DataColumn(QuantityText, typeof(SqlInt32)),
                    new DataColumn(LevelText, typeof(SqlInt32)),
                    new DataColumn(DamagePerJobText, typeof(SqlDouble)),
                    new DataColumn(RecycleText, typeof(SqlBoolean)),
                    new DataColumn(RaceIDText, typeof(SqlInt32)),
                    new DataColumn(ProbabilityText, typeof(SqlDouble)),
                    new DataColumn(ConsumeText, typeof(SqlBoolean)),
                });
            return ramTypeRequirements;
        }

        /// <summary>
        /// Gets the data table for the invBlueprintTypes table.
        /// </summary>
        /// <returns></returns>
        private static DataTable GetInvBlueprintTypesDataTable()
        {
            DataTable invBlueprintTypes = new DataTable();
            invBlueprintTypes.Columns.AddRange(
                new[]
                {
                    new DataColumn(BlueprintTypeIDText, typeof(SqlInt32)),
                    new DataColumn(ParentBlueprintTypeIDText, typeof(SqlInt32)),
                    new DataColumn(ProductTypeIDText, typeof(SqlInt32)),
                    new DataColumn(ProductionTimeText, typeof(SqlInt32)),
                    new DataColumn(TechLevelText, typeof(SqlInt16)),
                    new DataColumn(ResearchProductivityTimeText, typeof(SqlInt32)),
                    new DataColumn(ResearchMaterialTimeText, typeof(SqlInt32)),
                    new DataColumn(ResearchCopyTimeText, typeof(SqlInt32)),
                    new DataColumn(ResearchTechTimeText, typeof(SqlInt32)),
                    new DataColumn(DuplicatingTimeText, typeof(SqlInt32)),
                    new DataColumn(ReverseEngineeringTimeText, typeof(SqlInt32)),
                    new DataColumn(InventionTimeText, typeof(SqlInt32)),
                    new DataColumn(ProductivityModifierText, typeof(SqlInt32)),
                    new DataColumn(MaterialModifierText, typeof(SqlInt16)),
                    new DataColumn(WasteFactorText, typeof(SqlInt16)),
                    new DataColumn(MaxProductionLimitText, typeof(SqlInt32)),
                });
            return invBlueprintTypes;
        }
    }
}
