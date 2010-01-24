using System;
using System.Xml;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Collections.Generic;

using EVEMon.Common.Serialization;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.Serialization.Exportation;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common
{
    /// <summary>
    /// Provides exportation under different formats for characters.
    /// </summary>
    public static class CharacterExporter
    {
        private static SerializableCharacterSkill mergedSkill = new SerializableCharacterSkill();
        
        /// <summary>
        /// Creates a TXT format file for character exportation.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="plan"></param>
        public static string ExportAsText(Character character, Plan plan)
        {
            string Separator = "=======================================================================";
            string SubSeparator = "-----------------------------------------------------------------------";

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("BASIC INFO");
            builder.AppendLine(Separator);
            builder.AppendFormat(CultureInfo.CurrentCulture, "     Name: {0}{1}", character.Name, Environment.NewLine);
            builder.AppendFormat(CultureInfo.CurrentCulture, "   Gender: {0}{1}", character.Gender, Environment.NewLine);
            builder.AppendFormat(CultureInfo.CurrentCulture, "     Race: {0}{1}", character.Race, Environment.NewLine);
            builder.AppendFormat(CultureInfo.CurrentCulture, "Bloodline: {0}{1}", character.Bloodline, Environment.NewLine);
            builder.AppendFormat(CultureInfo.CurrentCulture, "  Balance: {0} ISK{1}", character.Balance.ToString("#,##0.00"), Environment.NewLine);
            builder.AppendLine();
            builder.AppendFormat(CultureInfo.CurrentCulture, "Intelligence: {0}{1}", character.Intelligence.EffectiveValue.ToString("#0.00").PadLeft(5), Environment.NewLine);
            builder.AppendFormat(CultureInfo.CurrentCulture, "    Charisma: {0}{1}", character.Charisma.EffectiveValue.ToString("#0.00").PadLeft(5), Environment.NewLine);
            builder.AppendFormat(CultureInfo.CurrentCulture, "  Perception: {0}{1}", character.Perception.EffectiveValue.ToString("#0.00").PadLeft(5), Environment.NewLine);
            builder.AppendFormat(CultureInfo.CurrentCulture, "      Memory: {0}{1}", character.Memory.EffectiveValue.ToString("#0.00").PadLeft(5), Environment.NewLine);
            builder.AppendFormat(CultureInfo.CurrentCulture, "   Willpower: {0}{1}", character.Willpower.EffectiveValue.ToString("#0.00").PadLeft(5), Environment.NewLine);
            builder.AppendLine();

            // Implants
            var implants = character.CurrentImplants.Where(x => x != Implant.None && (int)x.Slot < 5);
            if (implants.Count() > 0)
            {
                builder.AppendLine("IMPLANTS");
                builder.AppendLine(Separator);
                foreach (var implant in implants)
                {
                    builder.AppendFormat(CultureInfo.CurrentCulture, "+{0} {1} : {2}{3}", implant.Bonus, implant.Slot.ToString().PadRight(13), implant.Name, Environment.NewLine);
                }
                builder.AppendLine();
            }

            // Skill groups
            builder.AppendLine("SKILLS");
            builder.AppendLine(Separator);
            foreach (var skillGroup in character.SkillGroups)
            {
                int count = 0;
                foreach (var skill in skillGroup)
                {
                    if (skill.IsKnown)
                        count++;
                }

                builder.AppendFormat(CultureInfo.CurrentCulture, "{0}, {1} Skill{2}, {3} Points{4}",
                             skillGroup.Name, count, count > 1 ? "s" : "",
                             skillGroup.TotalSP.ToString("#,##0"), Environment.NewLine);

                // Skills
                foreach (var skill in skillGroup)
                {
                    int skillLevel;
                    int skillPoints;

                    if (plan != null)
                    {
                        GetMergedSkill(plan, skill);

                        if (!skill.IsKnown && mergedSkill.Level == 0)
                            continue;

                        skillLevel = mergedSkill.Level;
                        skillPoints = mergedSkill.Skillpoints;
                    }
                    else
                    {
                        if (!skill.IsKnown)
                            continue;

                        skillLevel = skill.Level;
                        skillPoints = skill.SkillPoints;
                    }

                    string skillDesc = skill.ToString() + " (" + skill.Rank.ToString() + ")";
                    builder.AppendFormat(CultureInfo.CurrentCulture, ": {0} L{1} {2}/{3} Points{4}",
                        skillDesc.PadRight(45), skillLevel.ToString().PadRight(5), skillPoints.ToString("#,##0"),
                        skill.StaticData.GetPointsRequiredForLevel(5).ToString("#,##0"), Environment.NewLine);

                    // If the skill is in training...
                    if (skill.IsTraining)
                    {
                        DateTime adjustedEndTime = character.CurrentlyTrainingSkill.EndTime.ToLocalTime();
                        builder.AppendFormat(CultureInfo.CurrentCulture, ":  (Currently training to level {0}, completes {1}){2}",
                                     Skill.GetRomanForInt(character.CurrentlyTrainingSkill.Level), adjustedEndTime, Environment.NewLine);
                    }
                }

                builder.AppendLine(SubSeparator);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Creates a CHR format file for character exportation.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="plan"></param>
        public static string ExportAsEFTCHR(Character character, Plan plan)
        {
            StringBuilder builder = new StringBuilder();

            foreach (var skill in character.Skills.Where(x => x.IsPublic && x.Group.ID != DBConstants.CorporationManagementSkillsGroupID
                                                                        && x.Group.ID != DBConstants.LearningSkillsGroupID
                                                                        && x.Group.ID != DBConstants.SocialSkillsGroupID
                                                                        && x.Group.ID != DBConstants.TradeSkillsGroupID))
            {
                int skillLevel;

                if (plan != null)
                {
                    GetMergedSkill(plan, skill);

                    skillLevel = mergedSkill.Level;
                }
                else
                {
                    skillLevel = skill.Level;
                }

                builder.AppendFormat(CultureInfo.CurrentCulture, "{0}={1}{2}", skill.Name, skillLevel, Environment.NewLine);
            }

            if (character.Identity.Account != null)
            {
                builder.AppendFormat(CultureInfo.CurrentCulture, "UserID={0}{1}", character.Identity.Account.UserID, Environment.NewLine);
                builder.AppendFormat(CultureInfo.CurrentCulture, "APIKey={0}{1}", character.Identity.Account.APIKey, Environment.NewLine);
                builder.AppendFormat(CultureInfo.CurrentCulture, "CharID={0}{1}", character.CharacterID, Environment.NewLine);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Creates a HTML format file for character exportation.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="plan"></param>
        public static string ExportAsHTML(Character character, Plan plan)
        {
            // Retrieves a XML representation of this character
            var serial = new OutputCharacter();
            serial.Name = character.Name;
            serial.Balance = character.Balance;
            serial.CorporationName = character.CorporationName;
            serial.CharacterID = character.CharacterID;
            serial.BloodLine = character.Bloodline;
            serial.Gender = character.Gender;
            serial.Race = character.Race;

            serial.Intelligence = character.Intelligence.EffectiveValue;
            serial.Perception = character.Perception.EffectiveValue;
            serial.Willpower = character.Willpower.EffectiveValue;
            serial.Charisma = character.Charisma.EffectiveValue;
            serial.Memory = character.Memory.EffectiveValue;

            // Attributes enhancers
            foreach (var implant in character.CurrentImplants.Where(x => x != Implant.None && (int)x.Slot < 5))
            {
                serial.AttributeEnhancers.Add(new OutputAttributeEnhancer { Attribute = implant.Slot, Bonus = implant.Bonus, Name = implant.Name });
            }

            // Certificates
            foreach (var certClass in character.CertificateClasses)
            {
                var cert = certClass.HighestClaimedGrade;
                if (cert == null)
                    continue;

                serial.Certificates.Add(new OutputCertificate { Name = certClass.Name, Grade = cert.Grade.ToString() });
            }

            // Skills (grouped by skill groups)
            foreach (var skillGroup in character.SkillGroups)
            {
                int count = 0;
                foreach (var skill in skillGroup)
                {
                    if (skill.IsKnown)
                        count++;
                }

                var outGroup = new OutputSkillGroup { Name = skillGroup.Name, SkillsCount = count, TotalSP = skillGroup.TotalSP };

                foreach (var skill in skillGroup)
                {
                    int skillLevel;
                    int skillPoints;

                    if (plan != null)
                    {
                        GetMergedSkill(plan, skill);

                        if (!skill.IsKnown && mergedSkill.Level == 0)
                            continue;

                        skillLevel = mergedSkill.Level;
                        skillPoints = mergedSkill.Skillpoints;
                    }
                    else
                    {
                        if (!skill.IsKnown)
                            continue;

                        skillLevel = skill.Level;
                        skillPoints = skill.SkillPoints;
                    }

                    outGroup.Skills.Add(new OutputSkill
                    {
                        Name = skill.Name,
                        Rank = skill.Rank,
                        Level = skillLevel,
                        SkillPoints = skillPoints,
                        RomanLevel = Skill.GetRomanForInt(skillLevel),
                        MaxSkillPoints = skill.StaticData.GetPointsRequiredForLevel(5)
                    });
                }

                if (outGroup.Skills.Count != 0)
                {
                    serial.SkillGroups.Add(outGroup);
                }
            }

            // Serializes to XML and apply a XSLT to generate the HTML doc.
            var doc = Util.SerializeToXmlDocument(typeof(OutputCharacter), serial);

            var xslt = Util.LoadXSLT(Properties.Resources.XmlToHtmlXslt);
            var htmlDoc = Util.Transform(doc, xslt);

            // Returns the string representation of the generated doc.
            return Util.GetXMLStringRepresentation(htmlDoc);
        }

        /// <summary>
        /// Creates a XML format file for character exportation.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="plan"></param>
        public static string ExportAsEVEMonXML(Character character, Plan plan)
        {
            var serial = character.Export();

            if (plan != null)
            {
                serial.Skills.Clear();

                foreach (var skill in character.Skills)
                {

                    GetMergedSkill(plan, skill);

                    if (!skill.IsKnown && mergedSkill.Level == 0)
                        continue;

                    serial.Skills.Add(new SerializableCharacterSkill()
                    {
                        ID = mergedSkill.ID,
                        IsKnown = mergedSkill.IsKnown,
                        Level = mergedSkill.Level,
                        Name = mergedSkill.Name,
                        OwnsBook = mergedSkill.OwnsBook,
                        Skillpoints = mergedSkill.Skillpoints,
                    });
                }
            }

            var doc = Util.SerializeToXmlDocument(serial.GetType(), serial);
            return Util.GetXMLStringRepresentation(doc);
        }

        /// <summary>
        /// Creates a XML format file for character exportation.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="plan"></param>
        public static string ExportAsCCPXML(Character character)
        {
            // Try to use the last XML character sheet downloaded from CCP
            var doc = LocalXmlCache.GetCharacterXml(character.Name);
            if (doc != null)
            {
                return Util.GetXMLStringRepresentation(doc);
            }

            // Displays an error
            return null;
        }

        /// <summary>
        /// Creates a BBCode format file for character exportation.
        /// </summary>
        /// <param name="character"></param>
        public static string ExportAsBBCode(Character character)
        {
            StringBuilder result = new StringBuilder();

            result.AppendFormat(CultureInfo.CurrentCulture, "[b]{0}[/b]{1}", character.Name, Environment.NewLine);
            result.AppendLine();
            result.AppendLine("[b]Attributes[/b]");
            result.AppendFormat(CultureInfo.CurrentCulture, "Intelligence: {0}{1}", character.Intelligence.EffectiveValue.ToString("#0.00").PadLeft(5), Environment.NewLine);
            result.AppendFormat(CultureInfo.CurrentCulture, "Perception:   {0}{1}", character.Perception.EffectiveValue.ToString("#0.00").PadLeft(5), Environment.NewLine);
            result.AppendFormat(CultureInfo.CurrentCulture, "Charisma:     {0}{1}", character.Charisma.EffectiveValue.ToString("#0.00").PadLeft(5), Environment.NewLine);
            result.AppendFormat(CultureInfo.CurrentCulture, "Willpower:    {0}{1}", character.Willpower.EffectiveValue.ToString("#0.00").PadLeft(5), Environment.NewLine);
            result.AppendFormat(CultureInfo.CurrentCulture, "Memory:       {0}{1}", character.Memory.EffectiveValue.ToString("#0.00").PadLeft(5), Environment.NewLine);

            foreach (var skillGroup in character.SkillGroups)
            {
                var skillGroupAppended = false;
                foreach (var skill in skillGroup)
                {
                    if (skill.Level > 0)
                    {
                        if (!skillGroupAppended)
                        {
                            result.AppendLine();
                            result.AppendFormat(CultureInfo.CurrentCulture, "[b]{0}[/b]{1}", skillGroup.Name, Environment.NewLine);

                            skillGroupAppended = true;
                        }

                        result.AppendFormat(CultureInfo.CurrentCulture, "[img]{0}{1}.gif[/img] {2}{3}", NetworkConstants.MyEVELevelImage, skill.Level, skill.Name, Environment.NewLine);
                    }
                }

                if (skillGroupAppended)
                {
                    result.AppendFormat(CultureInfo.CurrentCulture, "Total Skillpoints in Group: {0}{1}", skillGroup.TotalSP.ToString("#,##0"), Environment.NewLine);
                }
            }

            result.AppendLine();
            result.AppendFormat(CultureInfo.CurrentCulture, "Total Skillpoints: {0}{1}", character.SkillPoints.ToString("#,##0"), Environment.NewLine);
            result.AppendFormat(CultureInfo.CurrentCulture, "Total Number of Skills: {0}{1}", character.KnownSkillCount.ToString(), Environment.NewLine);
            result.AppendLine();
            result.AppendFormat(CultureInfo.CurrentCulture, "Skills at Level 1: {0}{1}", character.GetSkillCountAtLevel(1).ToString(), Environment.NewLine);
            result.AppendFormat(CultureInfo.CurrentCulture, "Skills at Level 2: {0}{1}", character.GetSkillCountAtLevel(2).ToString(), Environment.NewLine);
            result.AppendFormat(CultureInfo.CurrentCulture, "Skills at Level 3: {0}{1}", character.GetSkillCountAtLevel(3).ToString(), Environment.NewLine);
            result.AppendFormat(CultureInfo.CurrentCulture, "Skills at Level 4: {0}{1}", character.GetSkillCountAtLevel(4).ToString(), Environment.NewLine);
            result.AppendFormat(CultureInfo.CurrentCulture, "Skills at Level 5: {0}{1}", character.GetSkillCountAtLevel(5).ToString(), Environment.NewLine);

            return result.ToString();
        }

        /// <summary>
        /// Gets the skill properties of a merged skill with a plan entry
        /// </summary>
        /// <param name="plan"></param>
        /// <param name="skill"></param>
        /// <returns>The skill properties after the merge</returns>
        private static SerializableCharacterSkill GetMergedSkill(Plan plan, Skill skill)
        {
            SerializableCharacterSkill s_skill = new SerializableCharacterSkill();

            s_skill.ID = skill.ID;
            s_skill.Level = skill.Level;
            s_skill.Skillpoints = skill.SkillPoints;

            plan.Merge(s_skill);

            mergedSkill.ID = skill.ID;
            mergedSkill.Name = skill.Name;
            mergedSkill.IsKnown = skill.IsKnown;
            mergedSkill.OwnsBook = skill.IsOwned;
            mergedSkill.Level = s_skill.Level;
            mergedSkill.Skillpoints = s_skill.Skillpoints;

            return mergedSkill;
        }
    }

    /// <summary>
    /// The available formats for a character exportation.
    /// </summary>
    public enum CharacterSaveFormat
    {
        None = 0,
        Text = 1,
        EFTCHR = 2,
        HTML = 3,
        EVEMonXML = 4,
        CCPXML = 5,
        PNG = 6,
    }
}
