using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.Threading;

namespace EVEMon.Common
{
    /// <summary>
    /// Redundant class. Could be usefull if and when we implement a regions market monitor.
    /// </summary>
    public sealed class PricerMaketOrdersQueryResult
    {
        public string ErrorMessage { get; set; }

        public static  IEnumerable<MarketOrder> Orders
        {
            get
            {
                return StaticGeography.AllStations.First(x => x.Name == "Jita").SolarSystem.Select(
                    station => new SellOrder(new SerializableOrderListItem
                                                 {
                                                     Range = 0,
                                                     MinVolume = 1,
                                                     ItemID = 35,
                                                     StationID = station.ID,
                                                     RemainingVolume = 5000,
                                                     UnitaryPrice = 10.0M
                                                 }));
            }
        }
    }

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
                                                          Action<PricerMaketOrdersQueryResult> callback)
        {
            Dispatcher.Schedule(TimeSpan.FromSeconds(1.0), () => callback(new PricerMaketOrdersQueryResult()));
        }
    }
}