using System;

namespace EVEMon.Common.Notifications
{
    /// <summary>
    /// Represents an argument for a notification invalidation.
    /// </summary>
    [Serializable]
    public class NotificationEventArgs : EventArgs
    {
        private readonly NotificationCategory m_category;
        private readonly Object m_sender;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="category"></param>
        /// <param name="sender"></param>
        public NotificationEventArgs(NotificationCategory category, Object sender)
        {
            m_category = category;
            m_sender = sender;
        }

        /// <summary>
        /// Gets this category's notification.
        /// </summary>
        public NotificationCategory Category
        {
            get { return m_category; }
        }

        /// <summary>
        /// Gets the sender of this notification.
        /// </summary>
        public Object Sender
        {
            get { return m_sender; }
        }

        /// <summary>
        /// Gets the character who sent this notification, or null if the sender was not a character.
        /// </summary>
        public Character SenderCharacter
        {
            get { return m_sender as Character; }
        }

        /// <summary>
        /// Gets the account which sent this notification, or null if the sender was not an account.
        /// </summary>
        public Account SenderAccount
        {
            get { return m_sender as Account; }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the priority for this notification.
        /// </summary>
        public NotificationPriority Priority { get; set; }

        /// <summary>
        /// Gets or sets the behaviour of this notification regarding other notifications with the same validation key.
        /// </summary>
        public NotificationBehaviour Behaviour { get; set; }

        /// <summary>
        /// Gets true if the notification has details.
        /// </summary>
        public virtual bool HasDetails
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a key, identifying a category/sender pair, that will be used for invalidation.
        /// </summary>
        public long InvalidationKey
        {
            get { return GetKey(m_sender, m_category); }
        }

        /// <summary>
        /// Gets the key for the given sender and category.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public static long GetKey(Object sender, NotificationCategory category)
        {
            long left = ((long)category) << 32;
            int right = (sender == null ? 0 : sender.GetHashCode());
            return left | unchecked((uint)right);
        }

        /// <summary>
        /// Appends a given notification to this one.
        /// </summary>
        /// <param name="other"></param>
        public virtual void Append(NotificationEventArgs other)
        {
            // Must have to be implemented by inheritors.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the description for this 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Description;
        }
    }
}
