using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EVEMon.Common
{
    public sealed class IndustryJobsEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="character"></param>
        public IndustryJobsEventArgs(Character character, IEnumerable<IndustryJob> industryJobs)
        {
            Character = character;
            CompletedJobs = new List<IndustryJob>(industryJobs).AsReadOnly();
        }

        /// <summary>
        /// Gets the character related to this event.
        /// </summary>
        public Character Character { get; private set; }

        /// <summary>
        /// Gets the industry jobs related to this event.
        /// </summary>
        public ReadOnlyCollection<IndustryJob> CompletedJobs { get; private set; }
    }
}
