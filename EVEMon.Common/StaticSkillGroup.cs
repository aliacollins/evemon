using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections;
using System.IO;
using System.Reflection;

namespace EVEMon.Common
{
    public class StaticSkillGroup : IEnumerable<StaticSkill>
    {
        private string m_name;
        private int m_ID;
        private Dictionary<string, StaticSkill> m_skills = new Dictionary<string, StaticSkill>();

        public StaticSkillGroup(string name, int id, IEnumerable<StaticSkill> skills)
        {
            m_name = name;
            m_ID = id;
            foreach (StaticSkill gs in skills)
            {
                m_skills[gs.Name] = gs;
                gs.SetSkillGroup(this);
            }
            m_allGroups.Add(this);
        }

        public int ID
        {
            get { return m_ID; }
        }

        public string Name
        {
            get { return m_name; }
        }

        public StaticSkill this[string name]
        {
            get
            {
                StaticSkill result;
                m_skills.TryGetValue(name, out result);
                return result;
            }
        }

        public int Count
        {
            get { return m_skills.Count; }
        }

        public bool Contains(string skillName)
        {
            return m_skills.ContainsKey(skillName);
        }

        public bool Contains(StaticSkill gs)
        {
            return m_skills.ContainsValue(gs);
        }

        #region IEnumerable<StaticSkill> Members
        public IEnumerator<StaticSkill> GetEnumerator()
        {
            foreach (StaticSkill gs in m_skills.Values)
            {
                yield return gs;
            }
        }
        #endregion

        #region IEnumerable Members
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        internal void InsertSkill(StaticSkill ss)
        {
            m_skills[ss.Name] = ss;
            ss.SetSkillGroup(this);
        }

        private static List<StaticSkillGroup> m_allGroups = new List<StaticSkillGroup>();
        public static List<StaticSkillGroup> AllStaticGroups
        {
            get { return m_allGroups; }
        }

        public static void ResetSkillGroups()
        {
            m_allGroups.Clear();
        }

    }
}
