using System;

namespace EVEMon.Common
{
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
}