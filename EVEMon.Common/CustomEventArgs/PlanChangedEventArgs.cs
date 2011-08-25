using System;

namespace EVEMon.Common.CustomEventArgs
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
        /// Gets or sets the plan related to this event.
        /// </summary>
        public Plan Plan { get; private set; }
    }
}
