using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections;
using System.IO;
using System.Reflection;
using EVEMon.Common.Serialization.Datafiles;
using EVEMon.Common.Collections;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents a readonly skill group definition. Characters have their own implementations, <see cref="Skillgroup"/>
    /// </summary>
    public sealed class StaticSkillGroup : ReadonlyKeyedCollection<string, StaticSkill>
    {
        private readonly int m_ID;
        private readonly string m_name;

        /// <summary>
        /// Deserialization constructor from datafiles.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="skillArrayIndex"></param>
        internal StaticSkillGroup(SerializableSkillGroup src, ref int skillArrayIndex)
        {
            m_ID = src.ID;
            m_name = src.Name;
            foreach (var srcSkill in src.Skills)
            {
                m_items[srcSkill.Name] = new StaticSkill(this, srcSkill, skillArrayIndex);
                skillArrayIndex++;
            }
        }

        /// <summary>
        /// Gets the group's identifier.
        /// </summary>
        public int ID
        {
            get { return m_ID; }
        }

        /// <summary>
        /// Gets the group's name.
        /// </summary>
        public string Name
        {
            get { return m_name; }
        }

        /// <summary>
        /// Gets true if the group is the "learning" group
        /// </summary>
        public bool IsLearningGroup
        {
            get { return m_name == "Learning"; }
        }

        /// <summary>
        /// Gets a skill from this group by its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public StaticSkill this[string name]
        {
            get { return GetByKey(name); }
        }

        /// <summary>
        /// Checks whether this group contains the specified skill
        /// </summary>
        /// <param name="skillName"></param>
        /// <returns></returns>
        public bool Contains(string skillName)
        {
            return m_items.ContainsKey(skillName);
        }

        #region IEnumerable<StaticSkill> Members
        public IEnumerator<StaticSkill> GetEnumerator()
        {
            return m_items.Values.GetEnumerator();
        }

        #endregion
    }
}
