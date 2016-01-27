using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EVEMon.Common.Constants;
using EVEMon.Common.Net;
using EVEMon.Common.Serialization.EveMarketData.MarketPricer;
using EVEMon.Common.Service;

namespace EVEMon.Common.MarketPricer.EveMarketdata
{
    public sealed class EMDItemPricer : ItemPricer
    {
        #region Fields

        private const string Filename = "emd_item_prices";

        private static bool s_queryPending;

        #endregion


        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name => "Eve-MarketData";

        /// <summary>
        /// Gets a value indicating whether this <see cref="ItemPricer" /> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        protected override bool Enabled => true;

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


        #region Importation - Exportation Methods

        /// <summary>
        /// Ensures the list has been imported.
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

            // Update the file if we don't have it or the data have expired
            if (!File.Exists(file) || (Loaded && CachedUntil < DateTime.UtcNow))
            {
                Task.Run(GetPricesAsync);
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
            SerializableEMDItemPrices result = Util.DeserializeXmlFromFile<SerializableEMDItemPrices>(file);

            // In case the file is an old one, we try to get a fresh copy
            if (result == null || CachedUntil < DateTime.UtcNow)
            {
                Task.Run(GetPricesAsync);
                return;
            }

            PriceByItemID.Clear();

            // Import the data
            Import(result.Result.ItemPrices);
        }

        /// <summary>
        /// Import the query result list.
        /// </summary>
        /// <param name="itemPrices">The item prices.</param>
        private static void Import(IEnumerable<SerializableEMDItemPriceListItem> itemPrices)
        {
            EveMonClient.Trace("begin");

            foreach (IGrouping<int, SerializableEMDItemPriceListItem> item in itemPrices.GroupBy(item => item.ID))
            {
                double buyPrice = item.First(x => x.BuySell == "b").Price;
                double sellPrice = item.First(x => x.BuySell == "s").Price;

                if (Math.Abs(buyPrice) <= Double.Epsilon)
                    PriceByItemID[item.Key] = sellPrice;
                else if (Math.Abs(sellPrice) <= Double.Epsilon)
                    PriceByItemID[item.Key] = buyPrice;
                else
                    PriceByItemID[item.Key] = (buyPrice + sellPrice) / 2;
            }

            Loaded = true;

            EveMonClient.Trace("done");
        }

        /// <summary>
        /// Downloads the item prices list.
        /// </summary>
        protected override async Task GetPricesAsync()
        {
            // Quit if query is pending
            if (s_queryPending)
                return;

            s_queryPending = true;

            PriceByItemID.Clear();

            EveMonClient.Trace("begin");

            var url = new Uri(
                String.Format(CultureConstants.InvariantCulture, "{0}{1}", NetworkConstants.EVEMarketDataBaseUrl,
                    NetworkConstants.EVEMarketDataAPIItemPrices));

            DownloadAsyncResult<SerializableEMDItemPrices> result =
                await Util.DownloadXmlAsync<SerializableEMDItemPrices>(url, acceptEncoded: true);
            OnPricesDownloaded(result);
        }

        /// <summary>
        /// Called when data downloaded.
        /// </summary>
        /// <param name="result">The result.</param>
        private static void OnPricesDownloaded(DownloadAsyncResult<SerializableEMDItemPrices> result)
        {
            // Reset query pending flag
            s_queryPending = false;

            if (result == null || result.Error != null || result.Result.Result == null || !result.Result.Result.ItemPrices.Any())
            {
                if (result?.Result == null)
                    EveMonClient.Trace("no result");
                else if (result.Error != null)
                    EveMonClient.Trace(result.Error.Message);
                else if (result.Result.Result == null || !result.Result.Result.ItemPrices.Any())
                    EveMonClient.Trace("empty result");
                else
                    EveMonClient.Trace("failed");

                EveMonClient.OnPricesDownloaded(null, String.Empty);

                return;
            }

            EveMonClient.Trace("done");

            Import(result.Result.Result.ItemPrices);

            // Reset query pending flag
            s_queryPending = false;

            // Save the file in cache
            Save(Filename, Util.SerializeToXmlDocument(result.Result));

            CachedUntil = DateTime.UtcNow.AddDays(1);

            EveMonClient.OnPricesDownloaded(null, String.Empty);
        }

        #endregion
    }
}
