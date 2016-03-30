using System;
using System.Collections.Generic;
using EVEMon.Common.Threading;

namespace EVEMon.Common.MarketPricer
{
    /// <summary>
    /// Redundant class. Could be usefull if and when we implement a regions market monitor.
    /// </summary>
    public static class Pricer
    {
        /// <summary>
        /// Asynchronously queries the market orders
        /// </summary>
        /// <param name="itemIDs">The IDs of the types to query, see the items datafile.</param>
        /// <param name="regions">The IDs of the regions to query.</param>
        /// <param name="callback">A callback invoked on the UI thread.</param>
        public static void QueryRegionalMarketOrdersAsync(IEnumerable<int> itemIDs, IEnumerable<int> regions,
                                                          Action<PricerMarketOrdersQueryResult> callback)
        {
            Dispatcher.Schedule(TimeSpan.FromSeconds(1), () => callback(new PricerMarketOrdersQueryResult()));
        }
    }
}