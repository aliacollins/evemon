using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Settings;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.Serialization;

namespace EVEMon.Common
{
    /// <summary>
    /// A collection of market orders.
    /// </summary>
    public sealed class MarketOrderCollection : ReadonlyCollection<MarketOrder>
    {
        private readonly Character m_character;

        /// <summary>
        /// Internal constructor.
        /// </summary>
        internal MarketOrderCollection(Character character)
        {
            m_character = character;
        }

        /// <summary>
        /// Imports an enumeration of serialization objects.
        /// </summary>
        /// <param name="src"></param>
        internal void Import(IEnumerable<SerializableOrderBase> src)
        {
            m_items.Clear();
            foreach (var srcOrder in src)
            {
                if (srcOrder is SerializableBuyOrder)
                {
                    m_items.Add(new BuyOrder((SerializableBuyOrder)srcOrder));
                }
                else
                {
                    m_items.Add(new SellOrder((SerializableSellOrder)srcOrder));
                }
            }
        }

        /// <summary>
        /// Imports an enumeration of API objects.
        /// </summary>
        /// <param name="src"></param>
        /// <returns>The list of expired orders.</returns>
        internal void Import(IEnumerable<SerializableAPIOrder> src, List<MarketOrder> endedOrders)
        {
            // Mark the ignored orders for deletion 
            // If they are found again on the API feed, they won't be deleted and left as ignored
            foreach (var order in m_items)
            {
                order.MarkedForDeletion = order.Ignored;
            }

            // Import the orders from the API
            List<MarketOrder> newOrders = new List<MarketOrder>();
            foreach (var srcOrder in src)
            {
                // Skip expired tests
                var limit = srcOrder.Issued.AddDays(srcOrder.Duration + MarketOrder.MaxExpirationDays);
                if (limit < DateTime.UtcNow)
                    continue;

                // First check whether it is an existing order
                // If it is, update it and remove the deletion candidate flag
                if (m_items.Any(x => x.TryImport(srcOrder, endedOrders)))
                    continue;

                // It's a new order, let's add it
                if (srcOrder.IsBuyOrder != 0)
                {
                    var order = new BuyOrder(srcOrder);
                    if (order.Item != null)
                        newOrders.Add(order);
                }
                else
                {
                    var order = new SellOrder(srcOrder);
                    if (order.Item != null)
                        newOrders.Add(order);
                }
            }

            // Add the items that are no longer marked for deletion
            newOrders.AddRange(m_items.Where(x => !x.MarkedForDeletion));

            // Replace the old list with the new one
            m_items.Clear();
            m_items.AddRange(newOrders);
        }

        /// <summary>
        /// Exports the orders to a serialization object for the settings file.
        /// </summary>
        /// <returns></returns>
        internal List<SerializableOrderBase> Export()
        {
            List<SerializableOrderBase> serial = new List<SerializableOrderBase>(m_items.Count);

            foreach (var order in m_items)
            {
                serial.Add(order.Export());
            }

            return serial;
        }
    }
}
