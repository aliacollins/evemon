using System;
using System.Collections.Generic;
using System.Text;
using EVEMon.Common.Attributes;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents a plan's invalid entry.
    /// </summary>
    [EnforceUIThreadAffinity]
    public sealed class InvalidPlanEntry
    {
        /// <summary>
        /// Name of the skill that can not be identified
        /// </summary>
        public string SkillName
        {
            get;
            set;
        }

        /// <summary>
        /// Planned level
        /// </summary>
        public int PlannedLevel
        {
            get;
            set;
        }
    }
}
