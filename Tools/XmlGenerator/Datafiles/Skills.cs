using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EVEMon.Common;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.Datafiles;
using EVEMon.XmlGenerator.StaticData;

namespace EVEMon.XmlGenerator.Datafiles
{
    public static class Skills
    {
        private const int SkillGenTotal = 426;

        private static DateTime s_startTime;

        /// <summary>
        /// Generate the skills datafile.
        /// </summary>
        internal static void GenerateDatafile()
        {
            s_startTime = DateTime.Now;
            Util.ResetCounters();

            Console.WriteLine();
            Console.Write("Generating skills datafile... ");

            // Export skill groups
            List<SerializableSkillGroup> listOfSkillGroups = new List<SerializableSkillGroup>();

            foreach (InvGroup group in Database.InvGroupTable.Where(
                x => x.CategoryID == DBConstants.SkillCategoryID && x.ID != DBConstants.FakeSkillsGroupID).OrderBy(x => x.Name))
            {
                SerializableSkillGroup skillGroup = new SerializableSkillGroup
                                                        {
                                                            ID = group.ID,
                                                            Name = group.Name,
                                                        };

                // Add skills in skill group
                skillGroup.Skills.AddRange(ExportSkillsInGroup(group).OrderBy(x => x.Name));

                // Add skill group
                listOfSkillGroups.Add(skillGroup);
            }

            Console.WriteLine(String.Format(CultureConstants.DefaultCulture, " in {0}",
                                            DateTime.Now.Subtract(s_startTime)).TrimEnd('0'));

            // Serialize
            SkillsDatafile datafile = new SkillsDatafile();
            datafile.SkillGroups.AddRange(listOfSkillGroups);

            Util.SerializeXML(datafile, DatafileConstants.SkillsDatafile);
        }

        /// <summary>
        /// Exports the skills in the skill group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <returns></returns>
        private static IEnumerable<SerializableSkill> ExportSkillsInGroup(IHasID group)
        {
            List<SerializableSkill> listOfSkillsInGroup = new List<SerializableSkill>();

            foreach (InvType skill in Database.InvTypeTable.Where(x => x.GroupID == group.ID))
            {
                Util.UpdatePercentDone(SkillGenTotal);

                SerializableSkill singleSkill = new SerializableSkill
                                                    {
                                                        ID = skill.ID,
                                                        Name = skill.Name,
                                                        Description = skill.Description ?? String.Empty,
                                                        Public = skill.Published,
                                                        Cost = (long)skill.BasePrice,
                                                    };

                // Export skill atributes
                Dictionary<int, int> skillAttributes = Database.DgmTypeAttributesTable.Where(
                    x => x.ItemID == skill.ID).ToDictionary(
                        attribute => attribute.AttributeID, attribute => attribute.GetIntValue);

                singleSkill.Rank = skillAttributes.ContainsKey(DBConstants.SkillTimeConstantPropertyID) &&
                                   skillAttributes[DBConstants.SkillTimeConstantPropertyID] > 0
                                       ? skillAttributes[DBConstants.SkillTimeConstantPropertyID]
                                       : 1;

                singleSkill.PrimaryAttribute = skillAttributes.ContainsKey(DBConstants.PrimaryAttributePropertyID)
                                                   ? IntToEveAttribute(skillAttributes[DBConstants.PrimaryAttributePropertyID])
                                                   : EveAttribute.None;
                singleSkill.SecondaryAttribute = skillAttributes.ContainsKey(DBConstants.SecondaryAttributePropertyID)
                                                     ? IntToEveAttribute(
                                                         skillAttributes[DBConstants.SecondaryAttributePropertyID])
                                                     : EveAttribute.None;
                singleSkill.CanTrainOnTrial = !skillAttributes.ContainsKey(DBConstants.CanNotBeTrainedOnTrialPropertyID) ||
                                              skillAttributes[DBConstants.CanNotBeTrainedOnTrialPropertyID] == 0;

                // Export prerequesities
                List<SerializableSkillPrerequisite> listOfPrerequisites = new List<SerializableSkillPrerequisite>();

                for (int i = 0; i < DBConstants.RequiredSkillPropertyIDs.Count; i++)
                {
                    if (!skillAttributes.ContainsKey(DBConstants.RequiredSkillPropertyIDs[i]) ||
                        !skillAttributes.ContainsKey(DBConstants.RequiredSkillLevelPropertyIDs[i]))
                        continue;

                    InvType prereqSkill = Database.InvTypeTable.First(
                        x => x.ID == skillAttributes[DBConstants.RequiredSkillPropertyIDs[i]]);

                    if (prereqSkill == null)
                        continue;

                    SerializableSkillPrerequisite preReq = new SerializableSkillPrerequisite
                                                               {
                                                                   ID = prereqSkill.ID,
                                                                   Level =
                                                                       skillAttributes[
                                                                           DBConstants.RequiredSkillLevelPropertyIDs[i]],
                                                                   Name = prereqSkill.Name
                                                               };

                    // Add prerequisites
                    listOfPrerequisites.Add(preReq);
                }

                // Add prerequesites to skill
                singleSkill.SkillPrerequisites.AddRange(listOfPrerequisites);

                // Add skill
                listOfSkillsInGroup.Add(singleSkill);
            }
            return listOfSkillsInGroup;
        }

        /// <summary>
        /// Gets the Eve attribute.
        /// </summary>        
        private static EveAttribute IntToEveAttribute(int attributeValue)
        {
            switch (attributeValue)
            {
                case DBConstants.CharismaPropertyID:
                    return EveAttribute.Charisma;
                case DBConstants.IntelligencePropertyID:
                    return EveAttribute.Intelligence;
                case DBConstants.MemoryPropertyID:
                    return EveAttribute.Memory;
                case DBConstants.PerceptionPropertyID:
                    return EveAttribute.Perception;
                case DBConstants.WillpowerPropertyID:
                    return EveAttribute.Willpower;
                default:
                    return EveAttribute.None;
            }
        }
    }
}
