using System;
using System.Collections.Generic;

namespace EVEMon.MarketUnifiedUploader
{
    internal class PriceHistoryEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PriceHistoryEntry"/> class.
        /// </summary>
        /// <param name="entry">The entry.</param>
        public PriceHistoryEntry(IDictionary<object, object> entry)
        {
            HistoryDate = DateTime.FromFileTimeUtc(Convert.ToInt64(entry["historyDate"]));
            LowPrice = Convert.ToDecimal(entry["lowPrice"]);
            HighPrice = Convert.ToDecimal(entry["highPrice"]);
            AveragePrice = Convert.ToDecimal(entry["avgPrice"]);
            Quantity = Convert.ToInt64(entry["volume"]);
            Orders = Convert.ToInt32(entry["orders"]);
        }

        /// <summary>
        /// Gets the history date.
        /// </summary>
        internal DateTime HistoryDate { get; private set; }

        /// <summary>
        /// Gets the low price.
        /// </summary>
        internal decimal LowPrice { get; private set; }

        /// <summary>
        /// Gets the high price.
        /// </summary>
        internal decimal HighPrice { get; private set; }

        /// <summary>
        /// Gets the average price.
        /// </summary>
        internal decimal AveragePrice { get; private set; }

        /// <summary>
        /// Gets the quantity.
        /// </summary>
        internal long Quantity { get; private set; }

        /// <summary>
        /// Gets the orders.
        /// </summary>
        internal int Orders { get; private set; }

    }
}