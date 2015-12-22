using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Data;
using EVEMon.Common.Models;
using EVEMon.Common.Serialization.Eve;

namespace EVEMon.Common.MarketPricer
{
    /// <summary>
    /// Redundant class. Could be usefull if and when we implement a regions market monitor.
    /// </summary>
    public sealed class PricerMarketOrdersQueryResult
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
}