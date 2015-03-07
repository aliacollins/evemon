using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EVEMon.Common.CustomEventArgs
{
    public sealed class QueuedSkillsEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="queuedSkills">The queued skills.</param>
        public QueuedSkillsEventArgs(Character character, IEnumerable<QueuedSkill> queuedSkills)
        {
            Character = character;
            CompletedSkills = new List<QueuedSkill>(queuedSkills).AsReadOnly();
        }

        /// <summary>
        /// Gets or sets the character related to this event.
        /// </summary>
        public Character Character { get; private set; }

        /// <summary>
        /// Gets or sets the queued skills related to this event.
        /// </summary>
        public ReadOnlyCollection<QueuedSkill> CompletedSkills { get; private set; }
    }
}