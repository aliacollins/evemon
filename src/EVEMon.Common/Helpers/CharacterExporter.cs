using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using EVEMon.Common.Collections;
using EVEMon.Common.Constants;
using EVEMon.Common.Data;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Extensions;
using EVEMon.Common.Models;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Serialization.Exportation;
using EVEMon.Common.Serialization.Settings;
using EVEMon.Common.Service;

namespace EVEMon.Common.Helpers
{
    /// <summary>
    /// Provides exportation under different formats for characters.
    /// </summary>
    public static class CharacterExporter
    {
        /// <summary>
        /// Creates a TXT format file for character exportation.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="plan"></param>
        private static string ExportAsText(Character character, Plan plan)
        {
            const string Separator = "=======================================================================";
            const string SubSeparator = "-----------------------------------------------------------------------";

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("BASIC INFO");
            builder.AppendLine(Separator);
            builder.AppendFormat(CultureConstants.InvariantCulture, "     Name: {0}{1}", character.Name,
                                 Environment.NewLine);
            builder.AppendFormat(CultureConstants.InvariantCulture, "  Balance: {0:N2} ISK{1}",
                                 character.Balance, Environment.NewLine);
            builder.AppendFormat(CultureConstants.InvariantCulture, " Birthday: {0} UTC{1}", character.Birthday,
                                 Environment.NewLine);
            builder.AppendFormat(CultureConstants.InvariantCulture, "   Gender: {0}{1}", character.Gender,
                                 Environment.NewLine);
            builder.AppendFormat(CultureConstants.InvariantCulture, "     Race: {0}{1}", character.Race,
                                 Environment.NewLine);
            builder.AppendFormat(CultureConstants.InvariantCulture, "Bloodline: {0}{1}", character.Bloodline,
                                 Environment.NewLine);
            builder.AppendFormat(CultureConstants.InvariantCulture, " Ancestry: {0}{1}", character.Ancestry,
                                 Environment.NewLine);
            builder.AppendLine();
            builder.AppendFormat(CultureConstants.InvariantCulture, "Intelligence: {0}{1}",
                                 character.Intelligence.EffectiveValue, Environment.NewLine);
            builder.AppendFormat(CultureConstants.InvariantCulture, "    Charisma: {0}{1}",
                                 character.Charisma.EffectiveValue, Environment.NewLine);
            builder.AppendFormat(CultureConstants.InvariantCulture, "  Perception: {0}{1}",
                                 character.Perception.EffectiveValue, Environment.NewLine);
            builder.AppendFormat(CultureConstants.InvariantCulture, "      Memory: {0}{1}",
                                 character.Memory.EffectiveValue, Environment.NewLine);
            builder.AppendFormat(CultureConstants.InvariantCulture, "   Willpower: {0}{1}",
                                 character.Willpower.EffectiveValue, Environment.NewLine);
            builder.AppendLine();

            // Implants
            IEnumerable<Implant> implants = character.CurrentImplants.Where(x => x != Implant.None && (int)x.Slot < 5);
            if (implants.Any())
            {
                builder.AppendLine("IMPLANTS");
                builder.AppendLine(Separator);
                foreach (Implant implant in implants)
                {
                    builder.AppendFormat(CultureConstants.InvariantCulture, "+{0} {1} : {2}{3}", implant.Bonus,
                                         implant.Slot.ToString().PadRight(13), implant.Name, Environment.NewLine);
                }
                builder.AppendLine();
            }

            // Skill groups
            builder.AppendLine("SKILLS");
            builder.AppendLine(Separator);
            foreach (SkillGroup skillGroup in character.SkillGroups)
            {
                AddSkillGroups(character, plan, builder, skillGroup);

                builder.AppendLine(SubSeparator);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Adds the skill groups.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="plan">The plan.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="skillGroup">The skill group.</param>
        private static void AddSkillGroups(Character character, Plan plan, StringBuilder builder, SkillGroup skillGroup)
        {
            int count =
                skillGroup.Where(x => x.IsKnown || (plan != null && plan.IsPlanned(x))).Select(
                    x => GetMergedSkill(plan, x)).Count();
            Int64 skillGroupTotalSP =
                skillGroup.Where(x => x.IsKnown || (plan != null && plan.IsPlanned(x))).Select(
                    x => GetMergedSkill(plan, x)).Sum(x => x.Skillpoints);

            // Skill Group
            builder.AppendFormat(CultureConstants.InvariantCulture, "{0}, {1} Skill{2}, {3:N0} Points{4}",
                                 skillGroup.Name, count, count > 1 ? "s" : String.Empty,
                                 skillGroupTotalSP, Environment.NewLine);

            // Skills
            foreach (Skill skill in skillGroup.Where(x => x.IsKnown || (plan != null && plan.IsPlanned(x))))
            {
                AddSkills(character, plan, builder, skill);
            }
        }

        /// <summary>
        /// Adds the skills.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="plan">The plan.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="skill">The skill.</param>
        private static void AddSkills(Character character, Plan plan, StringBuilder builder, Skill skill)
        {
            SerializableCharacterSkill mergedSkill = GetMergedSkill(plan, skill);

            string skillDesc = String.Format(CultureConstants.InvariantCulture, "{0} ({1})", skill, skill.Rank);
            builder.AppendFormat(CultureConstants.InvariantCulture, "  {0} L{1} {2:N0}/{3:N0} Points{4}",
                                 skillDesc.PadRight(45), mergedSkill.Level.ToString(CultureConstants.InvariantCulture).PadRight(5),
                                 mergedSkill.Skillpoints,
                                 skill.StaticData.GetPointsRequiredForLevel(5),
                                 Environment.NewLine);

            // If the skill is in training...
            if (!skill.IsTraining)
                return;

            DateTime adjustedEndTime = character.CurrentlyTrainingSkill.EndTime.ToLocalTime();
            builder.AppendFormat(CultureConstants.InvariantCulture,
                                 ":  (Currently training to level {0}, completes {1}){2}",
                                 Skill.GetRomanFromInt(character.CurrentlyTrainingSkill.Level), adjustedEndTime,
                                 Environment.NewLine);
        }

        /// <summary>
        /// Creates a CHR format file for character exportation.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="plan"></param>
        private static string ExportAsEFTCHR(Character character, Plan plan)
        {
            StringBuilder builder = new StringBuilder();

            foreach (SerializableCharacterSkill skill in character.Skills.Where(
                x => x.IsPublic && x.Group.ID != DBConstants.CorporationManagementSkillsGroupID
                     && x.Group.ID != DBConstants.SocialSkillsGroupID
                     && x.Group.ID != DBConstants.TradeSkillsGroupID).Select(x => GetMergedSkill(plan, x)))
            {
                builder.AppendFormat(CultureConstants.InvariantCulture, "{0}={1}{2}", skill.Name, skill.Level,
                                     Environment.NewLine);
            }

            APIKey apiKey = character.Identity.FindAPIKeyWithAccess(CCPAPICharacterMethods.CharacterSheet);
            if (apiKey == null)
                return builder.ToString();

            builder.AppendFormat(CultureConstants.InvariantCulture, "KeyID={0}{1}", apiKey.ID,
                Environment.NewLine);
            builder.AppendFormat(CultureConstants.InvariantCulture, "VCode={0}{1}", apiKey.VerificationCode,
                Environment.NewLine);

            builder.AppendFormat(CultureConstants.InvariantCulture, "CharID={0}", character.CharacterID);

            return builder.ToString();
        }

        /// <summary>
        /// Creates a HTML format file for character exportation.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="plan"></param>
        private static string ExportAsHTML(Character character, Plan plan)
        {
            // Retrieves a XML representation of this character
            OutputCharacter serial = new OutputCharacter
                                         {
                                             Name = character.Name,
                                             Balance = character.Balance.ToNumericString(2, CultureConstants.InvariantCulture),
                                             Birthday = character.Birthday.ToString(CultureConstants.InvariantCulture),
                                             CorporationName = character.CorporationName,
                                             CharacterID = character.CharacterID,
                                             BloodLine = character.Bloodline,
                                             Ancestry = character.Ancestry,
                                             Gender = character.Gender,
                                             Race = character.Race,
                                             Intelligence = character.Intelligence.EffectiveValue,
                                             Perception = character.Perception.EffectiveValue,
                                             Willpower = character.Willpower.EffectiveValue,
                                             Charisma = character.Charisma.EffectiveValue,
                                             Memory = character.Memory.EffectiveValue
                                         };

            CompleteSerialization(character, plan, serial);

            // Serializes to XML and apply a XSLT to generate the HTML doc
            XmlDocument doc = (XmlDocument)Util.SerializeToXmlDocument(serial);

            XslCompiledTransform xslt = Util.LoadXslt(Properties.Resources.XmlToHtmlXslt);
            XmlDocument htmlDoc = (XmlDocument)Util.Transform(doc, xslt);

            // Returns the string representation of the generated doc
            return Util.GetXmlStringRepresentation(htmlDoc);
        }

        /// <summary>
        /// Completes the serialization.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="plan">The plan.</param>
        /// <param name="serial">The serial.</param>
        private static void CompleteSerialization(Character character, Plan plan, OutputCharacter serial)
        {
            // Attributes enhancers
            foreach (Implant implant in character.CurrentImplants.Where(x => x != Implant.None && (int)x.Slot < 5))
            {
                serial.AttributeEnhancers.Add(new OutputAttributeEnhancer
                                                  { Attribute = implant.Slot, Bonus = implant.Bonus, Name = implant.Name });
            }

            // Skills (grouped by skill groups)
            foreach (SkillGroup skillGroup in character.SkillGroups)
            {
                OutputSkillGroup outGroup = AddSkillGroup(plan, skillGroup);

                foreach (Skill skill in skillGroup.Where(x => x.IsKnown || (plan != null && plan.IsPlanned(x))))
                {
                    AddSkill(plan, outGroup, skill);
                }

                if (outGroup.Skills.Count != 0)
                    serial.SkillGroups.Add(outGroup);
            }
        }

        /// <summary>
        /// Adds the skill group.
        /// </summary>
        /// <param name="plan">The plan.</param>
        /// <param name="skillGroup">The skill group.</param>
        /// <returns></returns>
        private static OutputSkillGroup AddSkillGroup(Plan plan, SkillGroup skillGroup)
        {
            int count = skillGroup.Where(x => x.IsKnown || (plan != null && plan.IsPlanned(x))).Select(
                x => GetMergedSkill(plan, x)).Count();
            Int64 skillGroupTotalSP = skillGroup.Where(x => x.IsKnown || (plan != null && plan.IsPlanned(x))).Select(
                x => GetMergedSkill(plan, x)).Sum(x => x.Skillpoints);

            OutputSkillGroup outGroup = new OutputSkillGroup
                                            {
                                                Name = skillGroup.Name,
                                                SkillsCount = count,
                                                TotalSP = skillGroupTotalSP.ToString("N0", CultureConstants.InvariantCulture)
                                            };
            return outGroup;
        }

        /// <summary>
        /// Adds the skill.
        /// </summary>
        /// <param name="plan">The plan.</param>
        /// <param name="outGroup">The out group.</param>
        /// <param name="skill">The skill.</param>
        private static void AddSkill(Plan plan, OutputSkillGroup outGroup, Skill skill)
        {
            SerializableCharacterSkill mergedSkill = GetMergedSkill(plan, skill);

            outGroup.Skills.Add(new OutputSkill
                                    {
                                        Name = mergedSkill.Name,
                                        Rank = skill.Rank,
                                        Level = mergedSkill.Level,
                                        SkillPoints = mergedSkill.Skillpoints.ToString("N0", CultureConstants.InvariantCulture),
                                        RomanLevel = Skill.GetRomanFromInt(mergedSkill.Level),
                                        MaxSkillPoints =
                                            skill.StaticData.GetPointsRequiredForLevel(5).ToString("N0",
                                                                                                   CultureConstants.InvariantCulture)
                                    });
        }

        /// <summary>
        /// Creates a XML format file for character exportation.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="plan"></param>
        private static string ExportAsEVEMonXML(Character character, Plan plan)
        {
            SerializableSettingsCharacter serial = character.Export();

            if (plan != null)
            {
                serial.Skills.Clear();
                serial.Skills.AddRange(character.Skills.Where(skill => skill.IsKnown || plan.IsPlanned(skill))
                    .Select(skill => GetMergedSkill(plan, skill)));
            }

            XmlDocument doc = (XmlDocument)Util.SerializeToXmlDocument(serial);
            return doc != null ? Util.GetXmlStringRepresentation(doc) : null;
        }

        /// <summary>
        /// Creates a XML format file for character exportation.
        /// </summary>
        /// <param name="character"></param>
        private static string ExportAsCCPXML(Character character)
        {
            // Try to use the last XML character sheet downloaded from CCP
            XmlDocument doc = (XmlDocument)LocalXmlCache.GetCharacterXml(character);
            return doc != null ? Util.GetXmlStringRepresentation(doc) : null;
        }

        /// <summary>
        /// Creates a BBCode format file for character exportation.
        /// </summary>
        /// <param name="character"></param>
        public static string ExportAsBBCode(Character character)
        {
            if (character == null)
                throw new ArgumentNullException("character");

            StringBuilder result = new StringBuilder();

            result.AppendFormat(CultureConstants.InvariantCulture, "[b]{0}[/b]{1}", character.Name, Environment.NewLine);
            result.AppendLine();
            result.AppendLine("[b]Attributes[/b]");
            result.AppendLine("[table]");
            result.AppendFormat(CultureConstants.InvariantCulture, "[tr][td]Intelligence:[/td][td]{0}[/td][/tr]{1}",
                                character.Intelligence.EffectiveValue.ToString(CultureConstants.InvariantCulture).PadLeft(5),
                                Environment.NewLine);
            result.AppendFormat(CultureConstants.InvariantCulture, "[tr][td]Perception:[/td][td]{0}[/td][/tr]{1}",
                                character.Perception.EffectiveValue.ToString(CultureConstants.InvariantCulture).PadLeft(5),
                                Environment.NewLine);
            result.AppendFormat(CultureConstants.InvariantCulture, "[tr][td]Charisma:[/td][td]{0}[/td][/tr]{1}",
                                character.Charisma.EffectiveValue.ToString(CultureConstants.InvariantCulture).PadLeft(5),
                                Environment.NewLine);
            result.AppendFormat(CultureConstants.InvariantCulture, "[tr][td]Willpower:[/td][td]{0}[/td][/tr]{1}",
                                character.Willpower.EffectiveValue.ToString(CultureConstants.InvariantCulture).PadLeft(5),
                                Environment.NewLine);
            result.AppendFormat(CultureConstants.InvariantCulture, "[tr][td]Memory:[/td][td]{0}[/td][/tr]{1}",
                                character.Memory.EffectiveValue.ToString(CultureConstants.InvariantCulture).PadLeft(5),
                                Environment.NewLine);
            result.AppendLine("[/table]");

            foreach (SkillGroup skillGroup in character.SkillGroups)
            {
                bool skillGroupAppended = false;
                foreach (Skill skill in skillGroup.Where(skill => skill.Level > 0))
                {
                    if (!skillGroupAppended)
                    {
                        result.AppendLine();
                        result.AppendFormat(CultureConstants.InvariantCulture, "[b]{0}[/b]{1}", skillGroup.Name,
                                            Environment.NewLine);

                        skillGroupAppended = true;
                    }

                    result.AppendFormat(CultureConstants.InvariantCulture, "[img]{0}{1}{2}.gif[/img] {3}{4}",
                                        NetworkConstants.EVECommunityBase, NetworkConstants.MyEVELevelImage, skill.Level, skill.Name, Environment.NewLine);
                }

                if (skillGroupAppended)
                {
                    result.AppendFormat(CultureConstants.InvariantCulture, "Total Skillpoints in Group: {0}{1}",
                                        skillGroup.TotalSP.ToString("N0", CultureConstants.InvariantCulture), Environment.NewLine);
                }
            }

            result.AppendLine();
            result.AppendFormat(CultureConstants.InvariantCulture, "Total Skillpoints: {0}{1}",
                                character.SkillPoints.ToString("N0", CultureConstants.InvariantCulture), Environment.NewLine);
            result.AppendFormat(CultureConstants.InvariantCulture, "Total Number of Skills: {0}{1}",
                                character.KnownSkillCount.ToString(CultureConstants.InvariantCulture).PadLeft(5),
                                Environment.NewLine);
            result.AppendLine();
            result.AppendFormat(CultureConstants.InvariantCulture, "Skills at Level 0: {0}{1}",
                                character.GetSkillCountAtLevel(0).ToString(CultureConstants.InvariantCulture).PadLeft(5),
                                Environment.NewLine);
            result.AppendFormat(CultureConstants.InvariantCulture, "Skills at Level 1: {0}{1}",
                                character.GetSkillCountAtLevel(1).ToString(CultureConstants.InvariantCulture).PadLeft(5),
                                Environment.NewLine);
            result.AppendFormat(CultureConstants.InvariantCulture, "Skills at Level 2: {0}{1}",
                                character.GetSkillCountAtLevel(2).ToString(CultureConstants.InvariantCulture).PadLeft(5),
                                Environment.NewLine);
            result.AppendFormat(CultureConstants.InvariantCulture, "Skills at Level 3: {0}{1}",
                                character.GetSkillCountAtLevel(3).ToString(CultureConstants.InvariantCulture).PadLeft(5),
                                Environment.NewLine);
            result.AppendFormat(CultureConstants.InvariantCulture, "Skills at Level 4: {0}{1}",
                                character.GetSkillCountAtLevel(4).ToString(CultureConstants.InvariantCulture).PadLeft(5),
                                Environment.NewLine);
            result.AppendFormat(CultureConstants.InvariantCulture, "Skills at Level 5: {0}{1}",
                                character.GetSkillCountAtLevel(5).ToString(CultureConstants.InvariantCulture).PadLeft(5),
                                Environment.NewLine);

            return result.ToString();
        }

        /// <summary>
        /// Gets the skill properties of a merged skill with a plan entry, if one is provided.
        /// If no plan is provided, the skill properties are returned unmodified.
        /// </summary>
        /// <param name="plan"></param>
        /// <param name="skill"></param>
        /// <returns>The skill properties after the merge</returns>
        private static SerializableCharacterSkill GetMergedSkill(Plan plan, Skill skill)
        {
            SerializableCharacterSkill mergedSkill = new SerializableCharacterSkill
                                                         {
                                                             ID = skill.ID,
                                                             Name = skill.Name,
                                                             IsKnown = skill.IsKnown,
                                                             OwnsBook = skill.IsOwned,
                                                             Level = skill.Level,
                                                             Skillpoints = skill.SkillPoints
                                                         };


            plan?.Merge(mergedSkill);

            return mergedSkill;
        }

        /// <summary>
        /// Creates formatted string for character exportation.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="character"></param>
        /// <param name="plan"></param>
        /// <exception cref="NotImplementedException"></exception>
        /// <returns></returns>
        public static string Export(CharacterSaveFormat format, Character character, Plan plan = null)
        {
            switch (format)
            {
                case CharacterSaveFormat.Text:
                    return ExportAsText(character, plan);
                case CharacterSaveFormat.EFTCHR:
                    return ExportAsEFTCHR(character, plan);
                case CharacterSaveFormat.EVEMonXML:
                    return ExportAsEVEMonXML(character, plan);
                case CharacterSaveFormat.HTML:
                    return ExportAsHTML(character, plan);
                case CharacterSaveFormat.CCPXML:
                    return ExportAsCCPXML(character);
                default:
                    throw new NotImplementedException();
            }
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