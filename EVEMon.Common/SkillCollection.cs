using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Attributes;
using EVEMon.Common.Collections;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents a collection of skills.
    /// </summary>
    [EnforceUIThreadAffinity]
    public sealed class SkillCollection : ReadonlyKeyedCollection<long, Skill>
    {
        private readonly Character m_character;
        private readonly Skill[] m_itemsArray = new Skill[StaticSkills.ArrayIndicesCount];
        private readonly Dictionary<string, Skill> m_itemsByName = new Dictionary<string, Skill>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="character"></param>
        internal SkillCollection(Character character)
        {
            m_character = character;
            foreach (var group in character.SkillGroups)
            {
                foreach (var skill in group)
                {
                    Items[skill.ID] = skill;
                    m_itemsByName[skill.Name] = skill;
                    m_itemsArray[skill.ArrayIndex] = skill;
                }
            }

            // Build prerequisites list
            foreach (var skill in m_itemsArray)
            {
                skill.CompleteInitialization(m_itemsArray);
            }
        }

        /// <summary>
        /// Gets the skill with the provided name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Skill this[string name]
        {
            get
            {
                Skill skill = null;
                m_itemsByName.TryGetValue(name, out skill);
                return skill;
            }
        }

        /// <summary>
        /// Gets the skill with the provided id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Skill this[long id]
        {
            get { return GetByKey(id); }
        }

        /// <summary>
        /// Gets the skill representing the given static skill.
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        public Skill this[StaticSkill skill]
        {
            get { return GetByArrayIndex(skill.ArrayIndex); }
        }

        /// <summary>
        /// Gets the skill with the provided array index (see <see cref="StaticSkill.ArrayIndex"/>).
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Skill GetByArrayIndex(int index)
        {
            return m_itemsArray[index];
        }

        /// <summary>
        /// Exports this collection to a serialization object.
        /// </summary>
        /// <returns></returns>
        internal List<SerializableCharacterSkill> Export()
        {
            List<SerializableCharacterSkill> skills = new List<SerializableCharacterSkill>();
            foreach (Skill skill in Items.Values.Where(x => x.IsKnown || x.IsOwned))
            {
                skills.Add(skill.Export());
            }

            return skills;
        }

        /// Imports data from a serialization object.
        internal void Import(List<SerializableCharacterSkill> skills, bool fromCCP)
        {
            // Skills : reset all > update all
            foreach (Skill skill in Items.Values)
            {
                skill.Reset(fromCCP);
            }

            foreach (SerializableCharacterSkill serialSkill in skills.Where(x => this[x.ID] != null))
            {
                // Take care of the new skills not in our datafiles yet
                // Update if it exists
                Items[serialSkill.ID].Import(serialSkill, fromCCP);
            }
        }
    }
}
