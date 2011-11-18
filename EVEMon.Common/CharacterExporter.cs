using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.Serialization.Exportation;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common
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
            builder.AppendFormat(CultureConstants.DefaultCulture, "     Name: {0}{1}", character.Name,
                                 Environment.NewLine);
            builder.AppendFormat(CultureConstants.DefaultCulture, "   Gender: {0}{1}", character.Gender,
                                 Environment.NewLine);
            builder.AppendFormat(CultureConstants.DefaultCulture, "     Race: {0}{1}", character.Race,
                                 Environment.NewLine);
            builder.AppendFormat(CultureConstants.DefaultCulture, "Bloodline: {0}{1}", character.Bloodline,
                                 Environment.NewLine);
            builder.AppendFormat(CultureConstants.DefaultCulture, "  Balance: {0:N2} ISK{1}",
                                 character.Balance, Environment.NewLine);
            builder.AppendLine();
            builder.AppendFormat(CultureConstants.DefaultCulture, "Intelligence: {0}{1}",
                                 character.Intelligence.EffectiveValue.ToString().PadLeft(5), Environment.NewLine);
            builder.AppendFormat(CultureConstants.DefaultCulture, "    Charisma: {0}{1}",
                                 character.Charisma.EffectiveValue.ToString().PadLeft(5), Environment.NewLine);
            builder.AppendFormat(CultureConstants.DefaultCulture, "  Perception: {0}{1}",
                                 character.Perception.EffectiveValue.ToString().PadLeft(5), Environment.NewLine);
            builder.AppendFormat(CultureConstants.DefaultCulture, "      Memory: {0}{1}",
                                 character.Memory.EffectiveValue.ToString().PadLeft(5), Environment.NewLine);
            builder.AppendFormat(CultureConstants.DefaultCulture, "   Willpower: {0}{1}",
                                 character.Willpower.EffectiveValue.ToString().PadLeft(5), Environment.NewLine);
            builder.AppendLine();

            // Implants
            IEnumerable<Implant> implants = character.CurrentImplants.Where(x => x != Implant.None && (int)x.Slot < 5);
            if (implants.Count() > 0)
            {
                builder.AppendLine("IMPLANTS");
                builder.AppendLine(Separator);
                foreach (Implant implant in implants)
                {
                    builder.AppendFormat(CultureConstants.DefaultCulture, "+{0} {1} : {2}{3}", implant.Bonus,
                                         implant.Slot.ToString().PadRight(13), implant.Name, Environment.NewLine);
                }
                builder.AppendLine();
            }

            // Skill groups
            builder.AppendLine("SKILLS");
            builder.AppendLine(Separator);
            foreach (SkillGroup skillGroup in character.SkillGroups)
            {
                int count =
                    skillGroup.Where(x => x.IsKnown || (plan != null && plan.IsPlanned(x))).Select(
                        x => GetMergedSkill(plan, x)).Count();
                int skillGroupTotalSP =
                    skillGroup.Where(x => x.IsKnown || (plan != null && plan.IsPlanned(x))).Select(
                        x => GetMergedSkill(plan, x)).Sum(x => x.Skillpoints);

                // Skill Group
                builder.AppendFormat(CultureConstants.DefaultCulture, "{0}, {1} Skill{2}, {3:N0} Points{4}",
                                     skillGroup.Name, count, count > 1 ? "s" : String.Empty,
                                     skillGroupTotalSP, Environment.NewLine);

                // Skills
                foreach (Skill skill in skillGroup.Where(x => x.IsKnown || (plan != null && plan.IsPlanned(x))))
                {
                    SerializableCharacterSkill mergedSkill = GetMergedSkill(plan, skill);

                    string skillDesc = String.Format(CultureConstants.DefaultCulture, "{0} ({1})", skill, skill.Rank);
                    builder.AppendFormat(CultureConstants.DefaultCulture, ": {0} L{1} {2:N0}/{3:N0} Points{4}",
                                         skillDesc.PadRight(45), mergedSkill.Level.ToString().PadRight(5),
                                         mergedSkill.Skillpoints,
                                         skill.StaticData.GetPointsRequiredForLevel(5),
                                         Environment.NewLine);

                    // If the skill is in training...
                    if (!skill.IsTraining)
                        continue;

                    DateTime adjustedEndTime = character.CurrentlyTrainingSkill.EndTime.ToLocalTime();
                    builder.AppendFormat(CultureConstants.DefaultCulture,
                                         ":  (Currently training to level {0}, completes {1}){2}",
                                         Skill.GetRomanFromInt(character.CurrentlyTrainingSkill.Level), adjustedEndTime,
                                         Environment.NewLine);
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
        private static string ExportAsEFTCHR(Character character, Plan plan)
        {
            StringBuilder builder = new StringBuilder();

            foreach (SerializableCharacterSkill skill in character.Skills.Where(
                x => (x.IsPublic && x.Group.ID != DBConstants.CorporationManagementSkillsGroupID
                      && x.Group.ID != DBConstants.SocialSkillsGroupID
                      && x.Group.ID != DBConstants.TradeSkillsGroupID)).Select(x => GetMergedSkill(plan, x)))
            {
                builder.AppendFormat(CultureConstants.DefaultCulture, "{0}={1}{2}", skill.Name, skill.Level,
                                     Environment.NewLine);
            }

            APIKey apiKey = character.Identity.FindAPIKeyWithAccess(APICharacterMethods.CharacterSheet);
            if (apiKey != null)
            {
                builder.AppendFormat(CultureConstants.DefaultCulture, "KeyID={0}{1}", apiKey.ID,
                                     Environment.NewLine);
                builder.AppendFormat(CultureConstants.DefaultCulture, "VCode={0}{1}", apiKey.VerificationCode,
                                     Environment.NewLine);

                builder.AppendFormat(CultureConstants.DefaultCulture, "CharID={0}{1}", character.CharacterID,
                                     Environment.NewLine);
            }
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
                                             Balance = character.Balance,
                                             CorporationName = character.CorporationName,
                                             CharacterID = character.CharacterID,
                                             BloodLine = character.Bloodline,
                                             Gender = character.Gender,
                                             Race = character.Race,
                                             Intelligence = character.Intelligence.EffectiveValue,
                                             Perception = character.Perception.EffectiveValue,
                                             Willpower = character.Willpower.EffectiveValue,
                                             Charisma = character.Charisma.EffectiveValue,
                                             Memory = character.Memory.EffectiveValue
                                         };

            // Attributes enhancers
            foreach (Implant implant in character.CurrentImplants.Where(x => x != Implant.None && (int)x.Slot < 5))
            {
                serial.AttributeEnhancers.Add(new OutputAttributeEnhancer
                                                  { Attribute = implant.Slot, Bonus = implant.Bonus, Name = implant.Name });
            }

            // Certificates
            foreach (CertificateClass certClass in character.CertificateClasses)
            {
                Certificate cert = certClass.HighestClaimedGrade;
                if (cert == null)
                    continue;

                serial.Certificates.Add(new OutputCertificate { Name = certClass.Name, Grade = cert.Grade.ToString() });
            }

            // Skills (grouped by skill groups)
            foreach (SkillGroup skillGroup in character.SkillGroups)
            {
                int count =
                    skillGroup.Where(x => x.IsKnown || (plan != null && plan.IsPlanned(x))).Select(
                        x => GetMergedSkill(plan, x)).Count();
                int skillGroupTotalSP =
                    skillGroup.Where(x => x.IsKnown || (plan != null && plan.IsPlanned(x))).Select(
                        x => GetMergedSkill(plan, x)).Sum(x => x.Skillpoints);

                OutputSkillGroup outGroup = new OutputSkillGroup
                                                {
                                                    Name = skillGroup.Name,
                                                    SkillsCount = count,
                                                    TotalSP = skillGroupTotalSP
                                                };

                foreach (Skill skill in skillGroup.Where(x => x.IsKnown || (plan != null && plan.IsPlanned(x))))
                {
                    SerializableCharacterSkill mergedSkill = GetMergedSkill(plan, skill);

                    outGroup.Skills.Add(new OutputSkill
                                            {
                                                Name = mergedSkill.Name,
                                                Rank = skill.Rank,
                                                Level = mergedSkill.Level,
                                                SkillPoints = mergedSkill.Skillpoints,
                                                RomanLevel = Skill.GetRomanFromInt(mergedSkill.Level),
                                                MaxSkillPoints = skill.StaticData.GetPointsRequiredForLevel(5)
                                            });
                }

                if (outGroup.Skills.Count != 0)
                    serial.SkillGroups.Add(outGroup);
            }

            // Serializes to XML and apply a XSLT to generate the HTML doc
            XmlDocument doc = (XmlDocument)Util.SerializeToXmlDocument(typeof(OutputCharacter), serial);

            XslCompiledTransform xslt = Util.LoadXSLT(Properties.Resources.XmlToHtmlXslt);
            XmlDocument htmlDoc = (XmlDocument)Util.Transform(doc, xslt);

            // Returns the string representation of the generated doc
            return Util.GetXMLStringRepresentation(htmlDoc);
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
                serial.Skills =
                    character.Skills.Where(x => x.IsKnown || (plan.IsPlanned(x))).Select(x => GetMergedSkill(plan, x)).
                        ToList();
            }

            XmlDocument doc = (XmlDocument)Util.SerializeToXmlDocument(serial.GetType(), serial);
            return Util.GetXMLStringRepresentation(doc);
        }

        /// <summary>
        /// Creates a XML format file for character exportation.
        /// </summary>
        /// <param name="character"></param>
        private static string ExportAsCCPXML(Character character)
        {
            // Try to use the last XML character sheet downloaded from CCP
            XmlDocument doc = (XmlDocument)LocalXmlCache.GetCharacterXml(character.Name);
            return doc != null ? Util.GetXMLStringRepresentation(doc) : null;
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

            result.AppendFormat(CultureConstants.DefaultCulture, "[b]{0}[/b]{1}", character.Name, Environment.NewLine);
            result.AppendLine();
            result.AppendLine("[b]Attributes[/b]");
            result.AppendLine("[table]");
            result.AppendFormat(CultureConstants.DefaultCulture, "[tr][td]Intelligence:[/td][td]{0}[/td][/tr]{1}",
                                character.Intelligence.EffectiveValue.ToString().PadLeft(5), Environment.NewLine);
            result.AppendFormat(CultureConstants.DefaultCulture, "[tr][td]Perception:[/td][td]{0}[/td][/tr]{1}",
                                character.Perception.EffectiveValue.ToString().PadLeft(5), Environment.NewLine);
            result.AppendFormat(CultureConstants.DefaultCulture, "[tr][td]Charisma:[/td][td]{0}[/td][/tr]{1}",
                                character.Charisma.EffectiveValue.ToString().PadLeft(5), Environment.NewLine);
            result.AppendFormat(CultureConstants.DefaultCulture, "[tr][td]Willpower:[/td][td]{0}[/td][/tr]{1}",
                                character.Willpower.EffectiveValue.ToString().PadLeft(5), Environment.NewLine);
            result.AppendFormat(CultureConstants.DefaultCulture, "[tr][td]Memory:[/td][td]{0}[/td][/tr]{1}",
                                character.Memory.EffectiveValue.ToString().PadLeft(5), Environment.NewLine);
            result.AppendLine("[/table]");

            foreach (SkillGroup skillGroup in character.SkillGroups)
            {
                bool skillGroupAppended = false;
                foreach (Skill skill in skillGroup.Where(skill => skill.Level > 0))
                {
                    if (!skillGroupAppended)
                    {
                        result.AppendLine();
                        result.AppendFormat(CultureConstants.DefaultCulture, "[b]{0}[/b]{1}", skillGroup.Name,
                                            Environment.NewLine);

                        skillGroupAppended = true;
                    }

                    result.AppendFormat(CultureConstants.DefaultCulture, "[img]{0}{1}.gif[/img] {2}{3}",
                                        NetworkConstants.MyEVELevelImage, skill.Level, skill.Name, Environment.NewLine);
                }

                if (skillGroupAppended)
                {
                    result.AppendFormat(CultureConstants.DefaultCulture, "Total Skillpoints in Group: {0}{1}",
                                        skillGroup.TotalSP.ToString("N0"), Environment.NewLine);
                }
            }

            result.AppendLine();
            result.AppendFormat(CultureConstants.DefaultCulture, "Total Skillpoints: {0}{1}",
                                character.SkillPoints.ToString("N0"), Environment.NewLine);
            result.AppendFormat(CultureConstants.DefaultCulture, "Total Number of Skills: {0}{1}",
                                character.KnownSkillCount.ToString().PadLeft(5), Environment.NewLine);
            result.AppendLine();
            result.AppendFormat(CultureConstants.DefaultCulture, "Skills at Level 0: {0}{1}",
                                character.GetSkillCountAtLevel(0).ToString().PadLeft(5), Environment.NewLine);
            result.AppendFormat(CultureConstants.DefaultCulture, "Skills at Level 1: {0}{1}",
                                character.GetSkillCountAtLevel(1).ToString().PadLeft(5), Environment.NewLine);
            result.AppendFormat(CultureConstants.DefaultCulture, "Skills at Level 2: {0}{1}",
                                character.GetSkillCountAtLevel(2).ToString().PadLeft(5), Environment.NewLine);
            result.AppendFormat(CultureConstants.DefaultCulture, "Skills at Level 3: {0}{1}",
                                character.GetSkillCountAtLevel(3).ToString().PadLeft(5), Environment.NewLine);
            result.AppendFormat(CultureConstants.DefaultCulture, "Skills at Level 4: {0}{1}",
                                character.GetSkillCountAtLevel(4).ToString().PadLeft(5), Environment.NewLine);
            result.AppendFormat(CultureConstants.DefaultCulture, "Skills at Level 5: {0}{1}",
                                character.GetSkillCountAtLevel(5).ToString().PadLeft(5), Environment.NewLine);

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


            if (plan != null)
                plan.Merge(mergedSkill);

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
        public static string Export(CharacterSaveFormat format, Character character, Plan plan)
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