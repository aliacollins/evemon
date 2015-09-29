using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using EVEMon.Common;
using EVEMon.Common.Constants;

namespace EVEMon.MarketUnifiedUploader
{
    internal class EveCentralFormat
    {
        private readonly NameValueCollection m_data = new NameValueCollection();
        private const string EVEMonVersion = "1.7.0.beta";

        /// <summary>
        /// Prevents a default instance of the <see cref="EveCentralFormat"/> class from being created.
        /// </summary>
        /// <param name="input">The input.</param>
        private EveCentralFormat(KeyValuePair<object, object> input)
        {
            Format(input);
        }

        /// <summary>
        /// Gets the market object.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static string GetMarketObject(KeyValuePair<object, object> input)
        {
            EveCentralFormat marketFormat = new EveCentralFormat(input);
            return String.Join("&", Array.ConvertAll(marketFormat.m_data.AllKeys,
                                                     key => String.Format(CultureConstants.InvariantCulture,
                                                                          "{0}={1}", key, marketFormat.m_data[key])));
        }

        /// <summary>
        /// Formats the specified result.
        /// </summary>
        /// <param name="result">The result.</param>
        private void Format(KeyValuePair<object, object> result)
        {
            List<object> key = ((List<object>)((Tuple<object>)result.Key).Item1);

            if ((string)key[0] != "marketProxy")
                return;

            List<object> version = (List<object>)((Dictionary<object, object>)result.Value)["version"];
            string generatedAt = DateTime.FromFileTimeUtc((long)version[0]).ToUniversalDateTimeString();
            string regionID = key[2].ToString();
            string typeID = key[3].ToString();
            List<object> value = (List<object>)((Dictionary<object, object>)result.Value)["lret"];
            string rows;
            string resultType;

            switch ((string)key[1])
            {
                case "GetOrders":
                    rows = GenerateOrdersFormat(value);
                    resultType = "orders";
                    break;
                    //case "GetNewPriceHistory":
                    //case "GetOldPriceHistory":
                    //    rows = GenerateHistoryFormat(value);
                    //    resultType = "history";
                    //    break;
                default:
                    return;
            }

            // The actual market format
            m_data.Add("typeid", typeID);
            m_data.Add("typename", resultType);
            m_data.Add("region", regionID);
            m_data.Add("cache", true.ToString());
            m_data.Add("timestamp", generatedAt);
            m_data.Add("userid", "0");
            m_data.Add("data", rows);
        }

        /// <summary>
        /// Generates the history format.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static string GenerateHistoryFormat(IEnumerable<object> value)
        {
            List<PriceHistoryEntry> priceHistoryEntries = value.Cast<Dictionary<object, object>>().Select(
                entry => new PriceHistoryEntry(entry)).ToList();

            StringBuilder sb = new StringBuilder();
            sb.Append("historyDate,lowPrice,highPrice,avgPrice,volume,orders,source");

            const string RowFormat = "{0},{1},{2},{3},{4},{5},{6}{7}";
            priceHistoryEntries.ForEach(entry =>
                                        sb.Append(String.Format(CultureInfo.InvariantCulture, RowFormat,
                                                                entry.HistoryDate.ToUniversalDateTimeString(),
                                                                entry.LowPrice,
                                                                entry.HighPrice,
                                                                entry.AveragePrice,
                                                                entry.Quantity,
                                                                entry.Orders,
                                                                "EVEMon.MarketUnifiedUploader:" + EVEMonVersion,
                                                                Environment.NewLine)));
            return sb.ToString();
        }

        /// <summary>
        /// Generates the orders format.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static string GenerateOrdersFormat(IEnumerable<object> value)
        {
            List<MarketOrder> orders = value.Cast<List<object>>().SelectMany(
                obj => obj.Cast<Dictionary<object, object>>(),
                (obj, order) => new MarketOrder(order)).OrderBy(order => order.Bid).ToList();

            StringBuilder sb = new StringBuilder();
            const string RowFormat = "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}{15}";

            sb.Append("price,volRemaining,typeID,range,orderID,volEntered,minVolume,bid,issued," +
                      "duration,stationID,regionID,solarSystemID,jumps,source");

            orders.ForEach(order => sb.Append(String.Format(CultureInfo.InvariantCulture, RowFormat,
                                                            order.Price,
                                                            order.VolumeRemaining,
                                                            order.TypeID,
                                                            order.Range,
                                                            order.OrderID,
                                                            order.VolumeEntered,
                                                            order.MinVolume,
                                                            order.Bid,
                                                            order.IssueDate.ToUniversalDateTimeString(),
                                                            order.Duration,
                                                            order.StationID,
                                                            order.RegionID,
                                                            order.SolarSystemID,
                                                            order.Jumps,
                                                            "EVEMon.MarketUnifiedUploader:" + EVEMonVersion,
                                                            Environment.NewLine)));

            return sb.ToString();
        }
    }
}
