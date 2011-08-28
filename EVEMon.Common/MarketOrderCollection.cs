using System;
using System.Collections.Generic;
using System.Linq;

using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common
{
    /// <summary>
    /// A collection of market orders.
    /// </summary>
    public sealed class MarketOrderCollection : ReadonlyCollection<MarketOrder>
    {
        private readonly CCPCharacter m_character;

        /// <summary>
        /// Internal constructor.
        /// </summary>
        internal MarketOrderCollection(CCPCharacter character)
        {
            m_character = character;
        }

        /// <summary>
        /// Imports an enumeration of serialization objects.
        /// </summary>
        /// <param name="src"></param>
        internal void Import(IEnumerable<SerializableOrderBase> src)
        {
            Items.Clear();
            foreach (SerializableOrderBase srcOrder in src)
            {
                if (srcOrder is SerializableBuyOrder)
                {
                    Items.Add(new BuyOrder((SerializableBuyOrder)srcOrder));
                }
                else
                {
                    Items.Add(new SellOrder((SerializableSellOrder)srcOrder));
                }
            }
        }

        /// <summary>
        /// Imports an enumeration of API objects.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="endedOrders"></param>
        /// <returns>The list of expired orders.</returns>
        internal void Import(IEnumerable<SerializableOrderListItem> src, List<MarketOrder> endedOrders)
        {
            // Mark all orders for deletion 
            // If they are found again on the API feed, they won't be deleted
            // and those set as ignored will be left as ignored
            foreach (MarketOrder order in Items)
            {
                order.MarkedForDeletion = true;
            }

            // Import the orders from the API
            List<MarketOrder> newOrders = new List<MarketOrder>();
            foreach (SerializableOrderListItem srcOrder in
                src.Select(srcOrder => new
                                           {
                                               srcOrder,
                                               limit = srcOrder.Issued.AddDays(srcOrder.Duration + MarketOrder.MaxExpirationDays)
                                           }).Where(
                                               order => order.limit >= DateTime.UtcNow).Where(
                                                   order => !Items.Any(x => x.TryImport(order.srcOrder, endedOrders))).Select(
                                                       order => order.srcOrder))
            {
                // It's a new order, let's add it
                if (srcOrder.IsBuyOrder != 0)
                {
                    BuyOrder order = new BuyOrder(srcOrder);
                    if (order.Item != null)
                        newOrders.Add(order);
                }
                else
                {
                    SellOrder order = new SellOrder(srcOrder);
                    if (order.Item != null)
                        newOrders.Add(order);
                }
            }

            // Add the items that are no longer marked for deletion
            newOrders.AddRange(Items.Where(x => !x.MarkedForDeletion));

            // Add the items that are no longer present in the API
            // (a.k.a. Canceled, Expired, Fulfilled)
            endedOrders.AddRange(Items.Except(newOrders)); // This code line is to remain till CCP fixes the market orders API

            // Replace the old list with the new one
            Items.Clear();
            Items.AddRange(newOrders);

            // Fires the event regarding market orders update
            EveMonClient.OnCharacterMarketOrdersUpdated(m_character);
        }

        /// <summary>
        /// Exports the orders to a serialization object for the settings file.
        /// </summary>
        /// <returns></returns>
        internal List<SerializableOrderBase> Export()
        {
            List<SerializableOrderBase> serial = new List<SerializableOrderBase>(Items.Count);
            serial.AddRange(Items.Select(order => order.Export()));
            return serial;
        }
    }
}
