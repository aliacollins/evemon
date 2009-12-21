using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using EVEMon.Common.Serialization;
using System.Xml;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.Serialization.Exportation;
using System.Windows.Forms;
using EVEMon.Common.Data;

namespace EVEMon.Common
{
    /// <summary>
    /// Provides exportation under different formats for characters.
    /// </summary>
    public static class CharacterExporter
    {
        private const string Separator = "=======================================================================";
        private const string SubSeparator = "-----------------------------------------------------------------------";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        public static string ExportAsText(Character character)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("BASIC INFO");
            builder.AppendLine(Separator);
            builder.AppendLine(String.Format("     Name: {0}", character.Name));
            builder.AppendLine(String.Format("   Gender: {0}", character.Gender));
            builder.AppendLine(String.Format("     Race: {0}", character.Race));
            builder.AppendLine(String.Format("Bloodline: {0}", character.Bloodline));
            builder.AppendLine(String.Format("  Balance: {0} ISK", character.Balance.ToString("#,##0.00")));
            builder.AppendLine();
            builder.AppendLine(String.Format("Intelligence: {0}", character.Intelligence.ToString("#0.00").PadLeft(5)));
            builder.AppendLine(String.Format("    Charisma: {0}", character.Charisma.ToString("#0.00").PadLeft(5)));
            builder.AppendLine(String.Format("  Perception: {0}", character.Perception.ToString("#0.00").PadLeft(5)));
            builder.AppendLine(String.Format("      Memory: {0}", character.Memory.ToString("#0.00").PadLeft(5)));
            builder.AppendLine(String.Format("   Willpower: {0}", character.Willpower.ToString("#0.00").PadLeft(5)));
            builder.AppendLine();

            // Implants
            var implants = character.CurrentImplants.Where(x => x != Implant.None && (int)x.Slot < 5);
            if (implants.Count() > 0)
            {
                builder.AppendLine("IMPLANTS");
                builder.AppendLine(Separator);
                foreach (var implant in implants)
                {
                    builder.AppendLine(String.Format("+{0} {1} : {2}", implant.Bonus, implant.Slot.ToString().PadRight(13), implant.Name));
                }
                builder.AppendLine();
            }

            // Skill groups
            builder.AppendLine("SKILLS");
            builder.AppendLine(Separator);
            foreach (var skillGroup in character.SkillGroups)
            {
                builder.AppendLine(String.Format("{0}, {1} Skill{2}, {3} Points", 
                             skillGroup.Name, skillGroup.Count, skillGroup.Count > 1 ? "s" : "",
                             skillGroup.TotalSP.ToString("#,##0")));

                // Skills
                foreach (var skill in skillGroup)
                {
                    string skillDesc = skill.ToString() + " (" + skill.Rank.ToString() + ")";
                    builder.AppendLine(String.Format(": {0} {1}/{2} Points", 
                        skillDesc.PadRight(40), skill.SkillPoints.ToString("#,##0"),
                        skill.StaticData.GetPointsRequiredForLevel(5).ToString("#,##0")));

                    // If the skill is in training...
                    if (skill.IsTraining)
                    {
                        DateTime adjustedEndTime = character.CurrentlyTrainingSkill.EndTime.ToLocalTime();
                        builder.AppendLine(String.Format(":  (Currently training to level {0}, completes {1})",
                                     Skill.GetRomanForInt(character.CurrentlyTrainingSkill.Level), adjustedEndTime));
                    }
                }

                builder.AppendLine(SubSeparator);
            }

            return builder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
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
        /// 
        /// </summary>
        /// <param name="character"></param>
        public static string ExportAsHTML(Character character)
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
            foreach(var implant in character.CurrentImplants.Where(x => x != Implant.None && (int)x.Slot < 5))
            {
                serial.AttributeEnhancers.Add(new OutputAttributeEnhancer { Attribute = implant.Slot, Bonus = implant.Bonus, Name = implant.Name });
            }

            // Certificates
            foreach (var certClass in character.CertificateClasses)
            {
                var cert = certClass.HighestClaimedGrade;
                if (cert == null) continue;

                serial.Certificates.Add(new OutputCertificate { Name = certClass.Name, Grade = cert.Grade.ToString() });
            }

            // Skills (grouped by skill groups)
            foreach (var skillGroup in character.SkillGroups)
            {
                var outGroup = new OutputSkillGroup { Name = skillGroup.Name, SkillsCount = skillGroup.Count, TotalSP = skillGroup.TotalSP };

                foreach (var skill in skillGroup.Where(x => x.IsKnown))
                {
                    outGroup.Skills.Add(new OutputSkill
                    {
                        Name = skill.Name,
                        Rank = skill.Rank,
                        Level = skill.Level,
                        SkillPoints = skill.SkillPoints,
                        RomanLevel = Skill.GetRomanForInt(skill.Level),
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
            //System.IO.File.WriteAllText("c:\\truc.xml", Util.GetXMLStringRepresentation(doc));

            var xslt = Util.LoadXSLT(Properties.Resources.XmlToHtmlXslt);
            var htmlDoc = Util.Transform(doc, xslt);

            // Returns the string representation of the generated doc.
            return Util.GetXMLStringRepresentation(htmlDoc);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        public static string ExportAsEVEMonXML(Character character)
        {
            var serial = character.Export();
            var doc = Util.SerializeToXmlDocument(serial.GetType(), serial);
            return Util.GetXMLStringRepresentation(doc);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        public static string ExportAsBBCode(Character character)
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine("[b]" + character.Name + "[/b]");
            result.AppendLine("");
            result.AppendLine("[b]Attributes[/b]");
            result.AppendLine("Intelligence: " + character.Intelligence.EffectiveValue.ToString("#0.00").PadLeft(5));
            result.AppendLine("Perception: " + character.Perception.EffectiveValue.ToString("#0.00").PadLeft(5));
            result.AppendLine("Charisma: " + character.Charisma.EffectiveValue.ToString("#0.00").PadLeft(5));
            result.AppendLine("Willpower: " + character.Willpower.EffectiveValue.ToString("#0.00").PadLeft(5));
            result.AppendLine("Memory: " + character.Memory.EffectiveValue.ToString("#0.00").PadLeft(5));

            foreach (var skillGroup in character.SkillGroups)
            {
                result.AppendLine("");
                result.AppendLine("[b]" + skillGroup.Name + "[/b]");

                foreach (var skill in skillGroup)
                {
                    string url = String.Format("[img]{0}{1}.gif[/img]", NetworkConstants.MyEVELevelImage, skill.Level);
                    result.AppendLine(String.Format("{0} {1}", url, skill.Name));
                }

                result.AppendLine("Total Skillpoints in Group: " + skillGroup.TotalSP.ToString("#,##0"));
            }

            result.AppendLine("");
            result.AppendLine("Total Skillpoints: " + character.SkillPoints.ToString("#,##0"));
            result.AppendLine("Total Number of Skills: " + character.KnownSkillCount.ToString());
            result.AppendLine("");
            result.AppendLine("Skills at Level 1: " + character.GetSkillCountAtLevel(1).ToString());
            result.AppendLine("Skills at Level 2: " + character.GetSkillCountAtLevel(2).ToString());
            result.AppendLine("Skills at Level 3: " + character.GetSkillCountAtLevel(3).ToString());
            result.AppendLine("Skills at Level 4: " + character.GetSkillCountAtLevel(4).ToString());
            result.AppendLine("Skills at Level 5: " + character.GetSkillCountAtLevel(5).ToString());

            return result.ToString();
        }
    }

    /// <summary>
    /// The available formats for a character exportation.
    /// </summary>
    public enum CharacterSaveFormat
    {
        None = 0,
        Text = 1,
        HTML = 2,
        CCPXML = 3,
        EVEMonXML = 4,
        PNG = 5,
    }
}
