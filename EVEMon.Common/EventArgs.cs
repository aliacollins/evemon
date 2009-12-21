using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace EVEMon.Common
{
    #region CharacterChangedEventArgs
    public sealed class CharacterChangedEventArgs : EventArgs
    {
        private Character m_character;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="character"></param>
        public CharacterChangedEventArgs(Character character)
        {
            m_character = character;
        }

        /// <summary>
        /// Gets the character related to this event
        /// </summary>
        public Character Character
        {
            get { return m_character; }
        }
    }
    #endregion



    #region CharacterIdentityChangedEventArgs
    public sealed class CharacterIdentityChangedEventArgs : EventArgs
    {
        private CharacterIdentity m_identity;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="identity"></param>
        public CharacterIdentityChangedEventArgs(CharacterIdentity identity)
        {
            m_identity = identity;
        }

        /// <summary>
        /// Gets the character identity related to this event
        /// </summary>
        public CharacterIdentity CharacterIdentity
        {
            get { return m_identity; }
        }
    }
    #endregion


    #region PlanChangedEventArgs
    public sealed class PlanChangedEventArgs : EventArgs
    {
        private Plan m_plan;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="plan"></param>
        public PlanChangedEventArgs(Plan plan)
        {
            m_plan = plan;
        }

        /// <summary>
        /// Gets the plan related to this event
        /// </summary>
        public Plan Plan
        {
            get { return m_plan; }
        }
    }
    #endregion


    #region QueuedSkillsEventArgs
    public sealed class QueuedSkillsEventArgs : EventArgs
    {
        private Character m_character;
        private ReadOnlyCollection<QueuedSkill> m_queuedSkills;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="character"></param>
        public QueuedSkillsEventArgs(Character character, IEnumerable<QueuedSkill> queuedSkills)
        {
            m_character = character;
            m_queuedSkills = new List<QueuedSkill>(queuedSkills).AsReadOnly();
        }

        /// <summary>
        /// Gets the character related to this event
        /// </summary>
        public Character Character
        {
            get { return m_character; }
        }

        /// <summary>
        /// Gets the queued skills related to this event
        /// </summary>
        public ReadOnlyCollection<QueuedSkill> CompletedSkills
        {
            get { return m_queuedSkills; }
        }

    }
    #endregion

}
