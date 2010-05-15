using System.Collections.Generic;

using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents the list of all static skills
    /// </summary>
    public static class StaticSkills
    {
        private static int m_arrayIndicesCount;
        private static StaticSkill[] m_skills;
        private static readonly Dictionary<int, StaticSkill> m_skillsById = new Dictionary<int, StaticSkill>();
        private static readonly Dictionary<string, StaticSkill> m_skillsByName = new Dictionary<string, StaticSkill>();
        private static readonly Dictionary<int, StaticSkillGroup> m_allGroupsById = new Dictionary<int, StaticSkillGroup>();

        #region Public Properties

        /// <summary>
        /// Gets the total number of zero-based indices given to skills (for optimization purposes, it allows the use of arrays for computations)
        /// </summary>
        public static int ArrayIndicesCount
        {
            get { return m_arrayIndicesCount; }
        }

        /// <summary>
        /// Gets the list of groups
        /// </summary>
        public static IEnumerable<StaticSkillGroup> AllGroups
        {
            get
            {
                foreach (var group in m_allGroupsById.Values)
                {
                    yield return group;
                }
            }
        }

        /// <summary>
        /// Gets the list of groups
        /// </summary>
        public static IEnumerable<StaticSkill> AllSkills
        {
            get
            {
                foreach (var group in m_allGroupsById.Values)
                {
                    foreach (var skill in group)
                    {
                        yield return skill;
                    }
                }
            }
        }
        
        /// <summary>
        /// Gets the "learning" skill
        /// </summary>
        public static StaticSkill LearningSkill
        {
            get { return m_skillsById[DBConstants.LearningSkillID]; }
        }

        /// <summary>
        /// Gets the "learning" skill group
        /// </summary>
        public static StaticSkillGroup LearningSkillGroup
        {
            get { return m_allGroupsById[DBConstants.LearningSkillsGroupID]; }
        }

        #endregion


        #region Public Finders

        /// <summary>
        /// Gets a skill by its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static StaticSkill GetSkillByName(string name)
        {
            StaticSkill skill = null;
            m_skillsByName.TryGetValue(name, out skill);
            return skill;
        }

        /// <summary>
        /// Gets a skill by its identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static StaticSkill GetSkillById(int id)
        {
            StaticSkill skill = null;
            m_skillsById.TryGetValue(id, out skill);
            return skill;
        }

        /// <summary>
        /// Gets a skill by its array index
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static StaticSkill GetSkillByArrayIndex(int index)
        {
            return m_skills[index];
        }

        /// <summary>
        /// Gets the low-tier learning skill for the given attribute.
        /// </summary>
        /// <param name="attrib"></param>
        /// <returns></returns>
        public static StaticSkill GetLowerAttributeLearningSkill(EveAttribute attrib)
        {
            switch (attrib)
            {
                case EveAttribute.Charisma:
                    return m_skillsById[DBConstants.EmpathySkillID];
                case EveAttribute.Intelligence:
                    return m_skillsById[DBConstants.AnalyticalMindSkillID];
                case EveAttribute.Memory:
                    return m_skillsById[DBConstants.InstantRecallSkillID];
                case EveAttribute.Perception:
                    return m_skillsById[DBConstants.SpatialAwarenessSkillID];
                case EveAttribute.Willpower:
                    return m_skillsById[DBConstants.IronWillSkillID];
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the low-tier learning skill for the given attribute.
        /// </summary>
        /// <param name="attrib"></param>
        /// <returns></returns>
        public static StaticSkill GetUpperAttributeLearningSkill(EveAttribute attrib)
        {
            switch (attrib)
            {
                case EveAttribute.Charisma:
                    return m_skillsById[DBConstants.PresenceSkillID];
                case EveAttribute.Intelligence:
                    return m_skillsById[DBConstants.LogicSkillID];
                case EveAttribute.Memory:
                    return m_skillsById[DBConstants.EideticMemorySkillID];
                case EveAttribute.Perception:
                    return m_skillsById[DBConstants.ClaritySkillID];
                case EveAttribute.Willpower:
                    return m_skillsById[DBConstants.FocusSkillID];
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets a group by its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static StaticSkillGroup GetGroupByID(int groupId)
        {
            StaticSkillGroup group = null;
            m_allGroupsById.TryGetValue(groupId, out group);
            return group;
        }

        #endregion


        #region Initializers

        /// <summary>
        /// Initialize static skills
        /// </summary>
        internal static void Load()
        {
            SkillsDatafile datafile = Util.DeserializeDatafile<SkillsDatafile>(DatafileConstants.SkillsDatafile);

            // Fetch deserialized data
            m_arrayIndicesCount = 0;
            var prereqs = new List<SerializableSkillPrerequisite[]>();
            foreach (var srcGroup in datafile.Groups)
            {
                var group = new StaticSkillGroup(srcGroup, ref m_arrayIndicesCount);
                m_allGroupsById[group.ID] = group;

                // Store skills
                foreach (var skill in group)
                {
                    m_skillsById[skill.ID] = skill;
                    m_skillsByName[skill.Name] = skill;
                }

                // Store prereqs
                foreach (var serialSkill in srcGroup.Skills)
                {
                    prereqs.Add(serialSkill.Prereqs);
                }
            }

            // Complete initialization
            m_skills = new StaticSkill[m_arrayIndicesCount];
            foreach (var ss in m_skillsById.Values)
            {
                ss.CompleteInitialization(prereqs[ss.ArrayIndex]);
                m_skills[ss.ArrayIndex] = ss;
            }
        }

        #endregion

    }
}
