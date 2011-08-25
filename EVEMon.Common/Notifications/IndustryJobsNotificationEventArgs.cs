using System;
using System.Collections.Generic;


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
            : base(NotificationCategory.IndustryJobsCompletion, sender)
        {
            Jobs = new List<IndustryJob>(jobs);
            UpdateDescription();
        }

        /// <summary>
        /// Gets the associated API result.
        /// </summary>
        public List<IndustryJob> Jobs { get; private set; }

        /// <summary>
        /// Gets true if the notification has details.
        /// </summary>
        public override bool HasDetails
        {
            get { return true; }
        }

        /// <summary>
        /// Enqueue the jobs from the given notification at the end of this notification.
        /// </summary>
        /// <param name="other"></param>
        public override void Append(NotificationEventArgs other)
        {
            Jobs.AddRange(((IndustryJobsNotificationEventArgs) other).Jobs);
            UpdateDescription();
        }

        /// <summary>
        /// Updates the description.
        /// </summary>
        private void UpdateDescription()
        {
            Description = String.Format(CultureConstants.DefaultCulture, "{0} industry job{1} completed.", Jobs.Count,
                                        (Jobs.Count > 1 ? "s" : String.Empty));
        }
    }
}
