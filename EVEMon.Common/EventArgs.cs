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
        /// Constructor.
        /// </summary>
        /// <param name="character"></param>
        public CharacterChangedEventArgs(Character character)
        {
            m_character = character;
        }

        /// <summary>
        /// Gets the character related to this event.
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
        /// Constructor.
        /// </summary>
        /// <param name="identity"></param>
        public CharacterIdentityChangedEventArgs(CharacterIdentity identity)
        {
            m_identity = identity;
        }

        /// <summary>
        /// Gets the character identity related to this event.
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
        /// Constructor.
        /// </summary>
        /// <param name="plan"></param>
        public PlanChangedEventArgs(Plan plan)
        {
            m_plan = plan;
        }

        /// <summary>
        /// Gets the plan related to this event.
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
        /// Constructor.
        /// </summary>
        /// <param name="character"></param>
        public QueuedSkillsEventArgs(Character character, IEnumerable<QueuedSkill> queuedSkills)
        {
            m_character = character;
            m_queuedSkills = new List<QueuedSkill>(queuedSkills).AsReadOnly();
        }

        /// <summary>
        /// Gets the character related to this event.
        /// </summary>
        public Character Character
        {
            get { return m_character; }
        }

        /// <summary>
        /// Gets the queued skills related to this event.
        /// </summary>
        public ReadOnlyCollection<QueuedSkill> CompletedSkills
        {
            get { return m_queuedSkills; }
        }

    }

    #endregion


    #region IndustryJobsEventArgs

    public sealed class IndustryJobsEventArgs : EventArgs
    {
        private Character m_character;
        private ReadOnlyCollection<IndustryJob> m_industryJobs;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="character"></param>
        public IndustryJobsEventArgs(Character character, IEnumerable<IndustryJob> industryJobs)
        {
            m_character = character;
            m_industryJobs = new List<IndustryJob>(industryJobs).AsReadOnly();
        }

        /// <summary>
        /// Gets the character related to this event.
        /// </summary>
        public Character Character
        {
            get { return m_character; }
        }

        /// <summary>
        /// Gets the industry jobs related to this event.
        /// </summary>
        public ReadOnlyCollection<IndustryJob> CompletedJobs
        {
            get { return m_industryJobs; }
        }

    }

    #endregion


    #region EveServerEventArgs
    
    /// <summary>
    /// Represents an argument for server changes
    /// </summary>
    public sealed class EveServerEventArgs : EventArgs
    {
        private readonly EveServer m_server;
        private readonly ServerStatus m_status;
        private readonly ServerStatus m_previousStatus;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="server"></param>
        /// <param name="previousStatus"></param>
        /// <param name="status"></param>
        public EveServerEventArgs(EveServer server, ServerStatus previousStatus, ServerStatus status)
        {
            m_server = server;
            m_status = status;
            m_previousStatus = previousStatus;
        }

        /// <summary>
        /// Gets the updated server
        /// </summary>
        public EveServer Server
        {
            get { return m_server; }
        }

        /// <summary>
        /// Gets the current status
        /// </summary>
        public ServerStatus Status
        {
            get { return m_status; }
        }

        /// <summary>
        /// Gets the previous status
        /// </summary>
        public ServerStatus PreviousStatus
        {
            get { return m_previousStatus; }
        }
    }

    #endregion
}
