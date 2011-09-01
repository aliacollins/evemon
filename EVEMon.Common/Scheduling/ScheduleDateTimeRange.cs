using System;

namespace EVEMon.Common.Scheduling
{
    public class ScheduleDateTimeRange
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleDateTimeRange"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public ScheduleDateTimeRange(DateTime start, DateTime end)
        {
            From = start;
            To = end;
        }

        /// <summary>
        /// Gets or sets from.
        /// </summary>
        /// <value>From.</value>
        public DateTime From { get; private set; }

        /// <summary>
        /// Gets or sets to.
        /// </summary>
        /// <value>To.</value>
        public DateTime To { get; private set; }
    }
}