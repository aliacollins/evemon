using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EVEMon.Common.Collections;
using EVEMon.Common.Constants;
using EVEMon.Common.Data;
using EVEMon.Common.Net;
using EVEMon.Common.Serialization.EveCentral.MarketPricer;
using EVEMon.Common.Service;

namespace EVEMon.Common.MarketPricer.EveCentral
{
    public sealed class ECItemPricer : ItemPricer
    {
        #region Fields

        private const string Filename = "ec_item_prices";

        private static Queue<int> s_queue;
        private static List<int> s_queryMonitorList;
        private static int s_queryCounter;
        private static bool s_queryPending;
        private static int s_queryStep = 100;

        #endregion


        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name => "EVE-Central";

        /// <summary>
        /// Gets a value indicating whether this <see cref="ItemPricer" /> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        protected override bool Enabled => false;

        /// <summary>
        /// Gets the price by type ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public override double GetPriceByTypeID(int id)
        {
            // Ensure list importation
            EnsureImportation();

            double result;
            PriceByItemID.TryGetValue(id, out result);
            return result;
        }

        /// <summary>
        /// Ensures the importation.
        /// </summary>
        private void EnsureImportation()
        {
            // Quit if query is pending
            if (s_queryPending)
                return;

            // Check the selected provider
            if (!String.IsNullOrWhiteSpace(SelectedProviderName))
            {
                if (SelectedProviderName != Name)
                {
                    Loaded = false;
                    CachedUntil = DateTime.MinValue;
                    SelectedProviderName = Name;
                }
            }
            else
                SelectedProviderName = Name;

            string file = LocalXmlCache.GetFile(Filename).FullName;

            if (!File.Exists(file) || (Loaded && CachedUntil < DateTime.UtcNow))
            {
                GetPricesAsync();
                return;
            }

            // Exit if we have already imported the list
            if (Loaded)
                return;

            LoadFromFile(file);
        }

        /// <summary>
        /// Loads from file.
        /// </summary>
        /// <param name="file">The file.</param>
        private void LoadFromFile(string file)
        {
            CachedUntil = File.GetLastWriteTimeUtc(file).AddDays(1);

            // Deserialize the xml file
            SerializableECItemPrices result = Util.DeserializeXmlFromFile<SerializableECItemPrices>(file);

            // In case the file is an old one, we try to get a fresh copy
            if (result == null || CachedUntil < DateTime.UtcNow)
            {
                GetPricesAsync();
                return;
            }

            PriceByItemID.Clear();

            // Import the data
            Import(result.ItemPrices);
        }

        /// <summary>
        /// Imports the specified item prices.
        /// </summary>
        /// <param name="itemPrices">The item prices.</param>
        private static void Import(IEnumerable<SerializableECItemPriceListItem> itemPrices)
        {
            EveMonClient.Trace("begin");

            foreach (SerializableECItemPriceListItem item in itemPrices)
            {
                PriceByItemID[item.ID] = item.Prices.Average;
            }

            if (((s_queue == null) || (s_queue.Count == 0)) && !s_queryPending)
                Loaded = true;

            EveMonClient.Trace("done");
        }

        /// <summary>
        /// Gets the prices asynchronous.
        /// </summary>
        /// Gets the item prices list.
        protected override void GetPricesAsync()
        {
            // Quit if query is pending
            if (s_queryPending)
                return;

            s_queryPending = true;

            PriceByItemID.Clear();

            IOrderedEnumerable<int> marketItems = StaticItems.AllItems
                .Where(item =>
                    !item.MarketGroup.BelongsIn(DBConstants.RootNonMarketGroupID) &&
                    !item.MarketGroup.BelongsIn(DBConstants.BlueprintRootNonMarketGroupID) &&
                    !item.MarketGroup.BelongsIn(DBConstants.UniqueDesignsRootNonMarketGroupID))
                .Select(item => item.ID)
                .OrderBy(id => id);

            s_queue = new Queue<int>(marketItems);
            s_queryMonitorList = marketItems.ToList();

            EveMonClient.Trace("begin");

            QueryIDs();
        }

        /// <summary>
        /// Queries the ids.
        /// </summary>
        /// <returns></returns>
        private async void QueryIDs()
        {
            var idsToQuery = new List<int>();
            var url = new Uri($"{NetworkConstants.EVECentralBaseUrl}{NetworkConstants.EVECentralAPIItemPrices}");

            while (s_queue.Count > 0)
            {
                idsToQuery.Clear();
                for (int i = 0; i < s_queryStep; i++)
                {
                    idsToQuery.Add(s_queue.Dequeue());

                    if (s_queue.Count == 0)
                        break;
                }

                s_queryCounter++;

                DownloadAsyncResult<SerializableECItemPrices> result =
                    await Util.DownloadXmlAsync<SerializableECItemPrices>(url,
                        postData: GetPostData(idsToQuery), acceptEncoded: true);
                OnPricesDownloaded(result);
            }
        }

        /// <summary>
        /// Gets the post data.
        /// </summary>
        /// <param name="idsToQuery">The ids to query.</param>
        /// <returns></returns>
        private static string GetPostData(IReadOnlyCollection<int> idsToQuery)
        {
            StringBuilder sb = new StringBuilder();

            foreach (int i in idsToQuery)
            {
                sb.AppendFormat("typeid={0}", i);

                if (idsToQuery.Last() != i)
                    sb.Append("&");
            }

            SolarSystem jitaSolarSystem = StaticGeography.GetSolarSystemByName("Jita");

            if (jitaSolarSystem != null)
                sb.AppendFormat("&usesystem={0}", jitaSolarSystem.ID);

            return sb.ToString();
        }

        /// <summary>
        /// Called when prices downloaded.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnPricesDownloaded(DownloadAsyncResult<SerializableECItemPrices> result)
        {
            if (CheckQueryStatus(result))
                return;

            if (EveMonClient.IsDebugBuild)
                EveMonClient.Trace($"Remaining ids: {String.Join(", ", s_queryMonitorList)}", printMethod: false);

            EveMonClient.Trace("done");

            // Reset query pending flag
            s_queryPending = false;

            // Save the file in cache
            Save(Filename, Util.SerializeToXmlDocument(Export()));

            CachedUntil = DateTime.UtcNow.AddDays(1);

            EveMonClient.OnPricesDownloaded(null, String.Empty);
        }

        /// <summary>
        /// Checks the query status.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        private bool CheckQueryStatus(DownloadAsyncResult<SerializableECItemPrices> result)
        {
            s_queryCounter--;

            if (result == null || result.Error != null)
            {
                if (result?.Error != null)
                    EveMonClient.Trace(result.Error.Message);

                // Reset query pending flag
                if (s_queryCounter == 0)
                {
                    s_queryPending = false;
                    EveMonClient.OnPricesDownloaded(null, String.Empty);
                }
                else
                    return true;

                if (result == null)
                    return false;
            }

            // When the query succeeds import the data and remove the ids from the monitoring list
            if (result.Result != null)
            {
                foreach (SerializableECItemPriceListItem item in result.Result.ItemPrices)
                {
                    s_queryMonitorList.Remove(item.ID);
                }

                Import(result.Result.ItemPrices);
            }

            if (s_queryCounter != 0)
                return true;

            // if there are ids still to query repeat the query on a lower query step
            if (!s_queryMonitorList.Any())
                return false;

            // If the query step is greater than one at a time repeat the querying
            if (s_queryStep <= 1)
                return false;

            s_queryStep = s_queryStep / 2;
            s_queue = new Queue<int>(s_queryMonitorList);

            QueryIDs();

            return true;
        }

        /// <summary>
        /// Exports the cache list to a serializable object.
        /// </summary>
        /// <returns></returns>
        private static SerializableECItemPrices Export()
        {
            IEnumerable<SerializableECItemPriceListItem> entitiesList = PriceByItemID
                .OrderBy(x => x.Key)
                .Select(
                    item =>
                        new SerializableECItemPriceListItem
                        {
                            ID = item.Key,
                            Prices = new SerializableECItemPriceItem { Average = item.Value }
                        });

            SerializableECItemPrices serial = new SerializableECItemPrices();
            serial.ItemPrices.AddRange(entitiesList);

            return serial;
        }
    }
}
