using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using EVEMon.Common;

namespace EVEMon.MarketUnifiedUploader
{
    internal class MarketeerFormat
    {
        private readonly NameValueCollection m_data = new NameValueCollection();
        private const string EVEMonVersion = "1.7.0.beta";

        /// <summary>
        /// Prevents a default instance of the <see cref="MarketeerFormat"/> class from being created.
        /// </summary>
        /// <param name="input">The input.</param>
        private MarketeerFormat(KeyValuePair<object, object> input)
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
            MarketeerFormat marketFormat = new MarketeerFormat(input);
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
                case "GetNewPriceHistory":
                case "GetOldPriceHistory":
                    rows = GenerateHistoryFormat(value);
                    resultType = "history";
                    break;
                default:
                    return;
            }

            // The actual market format
            m_data.Add("upload_type", resultType);
            m_data.Add("region_id", regionID);
            m_data.Add("type_id", typeID);
            m_data.Add("log", rows);
            m_data.Add("upload_key", "EMDR");
            m_data.Add("developer_key", "EVEMon.MarketUnifiedUploader");
            m_data.Add("version", EVEMonVersion);
            m_data.Add("generated_at", generatedAt);
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

            if (priceHistoryEntries.Any())
            {
                const string RowFormat = "{0},{1},{2},{3},{4},{5}{6}";
                priceHistoryEntries.ForEach(entry =>
                                            sb.Append(String.Format(CultureInfo.InvariantCulture, RowFormat,
                                                                    entry.HistoryDate.ToUniversalDateTimeString(),
                                                                    entry.LowPrice,
                                                                    entry.HighPrice,
                                                                    entry.AveragePrice,
                                                                    entry.Quantity,
                                                                    entry.Orders,
                                                                    Environment.NewLine)));
            }
            else
                sb.Append("none");

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

            if (orders.Any())
            {
                const string RowFormat = "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}{11}";
                orders.ForEach(order => sb.Append(String.Format(CultureInfo.InvariantCulture, RowFormat,
                                                                order.OrderID,
                                                                order.Bid ? "b" : "s",
                                                                order.SolarSystemID,
                                                                order.StationID,
                                                                order.Price,
                                                                order.VolumeEntered,
                                                                order.VolumeRemaining,
                                                                order.MinVolume,
                                                                order.IssueDate.ToUniversalDateTimeString(),
                                                                order.Duration,
                                                                order.Range,
                                                                Environment.NewLine)));
            }
            else
                sb.Append("none");

            return sb.ToString();
        }
    }
}
