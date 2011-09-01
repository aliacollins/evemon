using System;

namespace EVEMon.Common.Notifications
{
    public sealed class NewEveMailMessageNotificationEventArgs : NotificationEventArgs
    {
        private int m_newMailMessagesCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewEveMailMessageNotificationEventArgs"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="newMessages">The new messages.</param>
        public NewEveMailMessageNotificationEventArgs(Object sender, int newMessages)
            : base(NotificationCategory.NewEveMailMessage, sender)
        {
            m_newMailMessagesCount = newMessages;
            UpdateDescription();
        }

        /// <summary>
        /// Gets true if the notification has details.
        /// </summary>
        public override bool HasDetails
        {
            get { return false; }
        }

        /// <summary>
        /// Adds the number of new mail messages from the given notification to this notification.
        /// </summary>
        /// <param name="other"></param>
        public override void Append(NotificationEventArgs other)
        {
            m_newMailMessagesCount += ((NewEveMailMessageNotificationEventArgs)other).m_newMailMessagesCount;
            UpdateDescription();
        }

        /// <summary>
        /// Updates the description.
        /// </summary>
        private void UpdateDescription()
        {
            Description = String.Format(CultureConstants.DefaultCulture, "{0} new EVE mail message{1}.",
                                        m_newMailMessagesCount, (m_newMailMessagesCount > 1 ? "s" : String.Empty));
        }
    }
}