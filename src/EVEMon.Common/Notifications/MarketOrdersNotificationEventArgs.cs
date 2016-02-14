using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using EVEMon.Common.Constants;
using EVEMon.Common.Models;

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
            : base(sender, NotificationCategory.MarketOrdersEnding)
        {
            if (orders == null)
                throw new ArgumentNullException("orders");

            Orders = new Collection<MarketOrder>();
            foreach (MarketOrder order in orders)
            {
                Orders.Add(order);
            }
            UpdateDescription();
        }

        /// <summary>
        /// Gets the associated API result.
        /// </summary>
        public Collection<MarketOrder> Orders { get; private set; }

        /// <summary>
        /// Gets true if the notification has details.
        /// </summary>
        public override bool HasDetails => true;

        /// <summary>
        /// Enqueue the orders from the given notification at the end of this notification.
        /// </summary>
        /// <param name="other"></param>
        public override void Append(NotificationEventArgs other)
        {
            foreach (MarketOrder order in ((MarketOrdersNotificationEventArgs)other).Orders)
            {
                Orders.Add((order));
            }

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