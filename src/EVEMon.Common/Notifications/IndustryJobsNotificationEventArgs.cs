using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using EVEMon.Common.Constants;
using EVEMon.Common.Models;

namespace EVEMon.Common.Notifications
{
    /// <summary>
    /// Provides notification services for IndustryJobs.
    /// </summary>
    public sealed class IndustryJobsNotificationEventArgs : NotificationEventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="jobs">The jobs.</param>
        public IndustryJobsNotificationEventArgs(Object sender, IEnumerable<IndustryJob> jobs)
            : base(sender, NotificationCategory.IndustryJobsCompletion)
        {
            if (jobs == null)
                throw new ArgumentNullException("jobs");

            Jobs = new Collection<IndustryJob>();
            foreach (IndustryJob job in jobs)
            {
                Jobs.Add(job);
            }
            UpdateDescription();
        }

        /// <summary>
        /// Gets the associated API result.
        /// </summary>
        public Collection<IndustryJob> Jobs { get; }

        /// <summary>
        /// Gets true if the notification has details.
        /// </summary>
        public override bool HasDetails => true;

        /// <summary>
        /// Enqueue the jobs from the given notification at the end of this notification.
        /// </summary>
        /// <param name="other"></param>
        public override void Append(NotificationEventArgs other)
        {
            foreach (IndustryJob job in ((IndustryJobsNotificationEventArgs)other).Jobs)
            {
                Jobs.Add(job);
            }

            UpdateDescription();
        }

        /// <summary>
        /// Updates the description.
        /// </summary>
        private void UpdateDescription()
        {
            Description = String.Format(CultureConstants.DefaultCulture, "{0} industry job{1} completed.", Jobs.Count,
                                        Jobs.Count > 1 ? "s" : String.Empty);
        }
    }
}