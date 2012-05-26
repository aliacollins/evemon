using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EVEMon.MarketUnifiedUploader
{
    internal class UnifiedFormat
    {
        private const string UnifiedFormatVersion = "0.1";
        private readonly AssemblyName m_assembly = Assembly.GetExecutingAssembly().GetName();
        private readonly Dictionary<string, object> m_data = new Dictionary<string, object>();
        private readonly string[] m_orderColumns = new[]
                                                       {
                                                           "price", "volRemaining", "range", "orderID", "volEntered", "minVolume",
                                                           "bid", "issueDate", "duration", "stationID", "solarSystemID"
                                                       };

        private readonly string[] m_historyColumns = new[]
                                                         {
                                                             "date", "orders", "quantity", "low", "high", "average"
                                                         };

        /// <summary>
        /// Initializes a new instance of the <see cref="UnifiedFormat"/> class.
        /// </summary>
        /// <param name="input">The input.</param>
        private UnifiedFormat(KeyValuePair<object, object> input)
        {
            Format(input);
        }

        /// <summary>
        /// Gets the JSON object.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static Dictionary<string, object> GetJSONObject(KeyValuePair<object, object> input)
        {
            UnifiedFormat unifiedFormat = new UnifiedFormat(input);
            return unifiedFormat.m_data;
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
                                                           { "name", m_assembly.Name },
                                                           { "version", m_assembly.Version.ToString() }
                                                       };

            // Columns and Rows info
            List<object> value = (List<object>)((Dictionary<object, object>)result.Value)["lret"];
            ArrayList columns = new ArrayList();
            ArrayList rows = new ArrayList();
            string resultType;
            switch ((string)key[1])
            {
                case "GetOrders":
                    columns.AddRange(m_orderColumns);
                    GenerateOrdersFormat(value, rows);
                    resultType = "orders";
                    break;
                case "GetNewPriceHistory":
                case "GetOldPriceHistory":
                    columns.AddRange(m_historyColumns);
                    GenerateHistoryFormat(value, rows);
                    resultType = "history";
                    break;
                default:
                    return;
            }

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
            m_data.Add("resultType", resultType);
            m_data.Add("version", UnifiedFormatVersion);
            m_data.Add("uploadKeys", uploadKeys);
            m_data.Add("generator", generator);
            m_data.Add("currentTime", DateTime.UtcNow.ToIsoDateTimeUTCString());
            m_data.Add("columns", columns);
            m_data.Add("rowsets", rowsets);
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
