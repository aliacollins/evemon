using System;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Models;

namespace EVEMon.Common.CustomEventArgs
{
    /// <summary>
    /// Represents an argument for server changes
    /// </summary>
    public sealed class EveServerEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="previousStatus"></param>
        /// <param name="status"></param>
        public EveServerEventArgs(EveServer server, ServerStatus previousStatus, ServerStatus status)
        {
            Server = server;
            Status = status;
            PreviousStatus = previousStatus;
        }

        /// <summary>
        /// Gets the updated server
        /// </summary>
        public EveServer Server { get; }

        /// <summary>
        /// Gets the current status
        /// </summary>
        public ServerStatus Status { get; }

        /// <summary>
        /// Gets the previous status
        /// </summary>
        public ServerStatus PreviousStatus { get; }
    }
}