using System;
using System.Collections.Generic;

namespace EVEMon.Common.Notifications
{
    public sealed class MarketOrdersNotificationEventArgs : NotificationEventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="orders">The orders.</param>
        public MarketOrdersNotificationEventArgs(Object sender, IEnumerable<MarketOrder> orders)
            : base(NotificationCategory.MarketOrdersEnding, sender)
        {
            Orders = new List<MarketOrder>(orders);
            UpdateDescription();
        }

        /// <summary>
        /// Gets the associated API result.
        /// </summary>
        public List<MarketOrder> Orders { get; private set; }

        /// <summary>
        /// Gets true if the notification has details.
        /// </summary>
        public override bool HasDetails
        {
            get { return true; }
        }

        /// <summary>
        /// Enqueue the orders from the given notification at the end of this notification.
        /// </summary>
        /// <param name="other"></param>
        public override void Append(NotificationEventArgs other)
        {
            Orders.AddRange(((MarketOrdersNotificationEventArgs)other).Orders);
            UpdateDescription();
        }

        /// <summary>
        /// Updates the description.
        /// </summary>
        private void UpdateDescription()
        {
            Description = String.Format(CultureConstants.DefaultCulture, "{0} market order{1} expired or fulfilled.",
                                        Orders.Count, (Orders.Count > 1 ? "s" : String.Empty));
        }
    }
}