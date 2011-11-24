using System;
using System.Collections.Generic;

namespace EVEMon.Common.CustomEventArgs
{
    public sealed class MarketOrdersEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="endedOrders">The ended orders.</param>
        public MarketOrdersEventArgs(Character character, IEnumerable<MarketOrder> endedOrders)
        {
            Character = character;
            EndedOrders = endedOrders;
        }

        /// <summary>
        /// Gets or sets the character.
        /// </summary>
        /// <value>The character.</value>
        public Character Character { get; private set; }

        /// <summary>
        /// Gets or sets the ended orders.
        /// </summary>
        /// <value>The ended orders.</value>
        public IEnumerable<MarketOrder> EndedOrders { get; private set; }
    }
}
