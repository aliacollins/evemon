using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EVEMon.MarketUnifiedUploader
{
    internal static class UnifiedFormat
    {
        private const string UnifiedFormatVersion = "0.1";
        private static readonly AssemblyName s_assembly = Assembly.GetExecutingAssembly().GetName();
        private static readonly Dictionary<string, object> s_data = new Dictionary<string, object>();
        private static readonly string[] s_orderColumns = new[]
                                                       {
                                                           "price", "volRemaining", "range", "orderID", "volEntered", "minVolume",
                                                           "bid", "issueDate", "duration", "stationID", "solarSystemID"
                                                       };

        private static readonly string[] s_historyColumns = new[]
                                                         {
                                                             "date", "orders", "quantity", "low", "high", "average"
                                                         };

        /// <summary>
        /// Gets the JSON object.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static Dictionary<string, object> GetJSONObject(KeyValuePair<object, object> input)
        {
            return Format(input);
        }

        /// <summary>
        /// Formats the specified result.
        /// </summary>
        /// <param name="result">The result.</param>
        private static Dictionary<string, object> Format(KeyValuePair<object, object> result)
        {
            s_data.Clear();

            List<object> key = ((List<object>)((Tuple<object>)result.Key).Item1);

            if ((string)key[0] != "marketProxy")
                return s_data;

            List<object> version = (List<object>)((Dictionary<object, object>)result.Value)["version"];
            string generatedAt = DateTime.FromFileTimeUtc((long)version[0]).ToIsoDateTimeUTCString();
            string regionID = key[2].ToString();
            string typeID = key[3].ToString();

            // UploadKeys info
            ArrayList uploadKeys = new ArrayList();
            uploadKeys.AddRange(Uploader.EndPoints.Where(endPoint => endPoint.Enabled &&
                                                                     endPoint.NextUploadTimeUtc < DateTime.UtcNow)
                                    .Select(endpoint => new Dictionary<string, object>
                                                            {
                                                                { "name", endpoint.Name },
                                                                { "key", endpoint.UploadKey }
                                                            }).ToArray());

            // Generator info
            Dictionary<string, object> generator = new Dictionary<string, object>
                                                       {
                                                           { "name", s_assembly.Name },
                                                           { "version", s_assembly.Version.ToString() }
                                                       };

            // Columns and Rows info
            List<object> value = (List<object>)((Dictionary<object, object>)result.Value)["lret"];
            ArrayList columns = new ArrayList();
            ArrayList rows = new ArrayList();
            string resultType;
            switch ((string)key[1])
            {
                case "GetOrders":
                    columns.AddRange(s_orderColumns);
                    GenerateOrdersFormat(value, rows);
                    resultType = "orders";
                    break;
                case "GetNewPriceHistory":
                case "GetOldPriceHistory":
                    columns.AddRange(s_historyColumns);
                    GenerateHistoryFormat(value, rows);
                    resultType = "history";
                    break;
                default:
                    return s_data;
            }

            // No data to upload
            if (rows.Count == 0)
                return s_data;

            // Rowsets
            ArrayList rowsets = new ArrayList
                                    {
                                        new Dictionary<string, object>
                                            {
                                                { "generatedAt", generatedAt },
                                                { "regionID", regionID },
                                                { "typeID", typeID },
                                                { "rows", rows }
                                            }
                                    };

            // The actual unified format
            s_data.Add("resultType", resultType);
            s_data.Add("version", UnifiedFormatVersion);
            s_data.Add("uploadKeys", uploadKeys);
            s_data.Add("generator", generator);
            s_data.Add("currentTime", DateTime.UtcNow.ToIsoDateTimeUTCString());
            s_data.Add("columns", columns);
            s_data.Add("rowsets", rowsets);

            return s_data;
        }

        /// <summary>
        /// Generates the history format.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="rows">The rows.</param>
        private static void GenerateHistoryFormat(IEnumerable<object> value, ArrayList rows)
        {
            List<PriceHistoryEntry> priceHistoryEntries = value.Cast<Dictionary<object, object>>().Select(
                entry => new PriceHistoryEntry(entry)).ToList();

            rows.AddRange(priceHistoryEntries
                              .Select(entry => new ArrayList
                                                   {
                                                       entry.HistoryDate.ToIsoDateTimeUTCString(),
                                                       entry.Orders,
                                                       entry.Quantity,
                                                       entry.LowPrice,
                                                       entry.HighPrice,
                                                       entry.AveragePrice
                                                   }).ToArray());
        }

        /// <summary>
        /// Generates the orders format.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="rows">The rows.</param>
        private static void GenerateOrdersFormat(IEnumerable<object> value, ArrayList rows)
        {
            // Rows
            List<MarketOrder> orders = value.Cast<List<object>>().SelectMany(
                obj => obj.Cast<Dictionary<object, object>>(),
                (obj, order) => new MarketOrder(order)).OrderBy(order => order.Bid).ToList();

            rows.AddRange(orders.Select(order => new ArrayList
                                                     {
                                                         order.Price,
                                                         order.VolumeRemaining,
                                                         order.Range,
                                                         order.OrderID,
                                                         order.VolumeEntered,
                                                         order.MinVolume,
                                                         order.Bid,
                                                         order.IssueDate.ToIsoDateTimeUTCString(),
                                                         order.Duration,
                                                         order.StationID,
                                                         order.SolarSystemID
                                                     }).ToArray());
        }

    }
}
