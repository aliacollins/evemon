using System;

namespace EVEMon.Common
{
    public sealed class PlanChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="plan"></param>
        public PlanChangedEventArgs(Plan plan)
        {
            Plan = plan;
        }

        /// <summary>
        /// Gets the plan related to this event.
        /// </summary>
        public Plan Plan { get; private set; }
    }
}
