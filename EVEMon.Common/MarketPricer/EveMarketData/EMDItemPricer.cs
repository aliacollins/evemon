using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using EVEMon.Common.Constants;
using EVEMon.Common.Helpers;
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


        public override string Name
        {
            get { return "Eve-MarketData"; }
        }

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

            // In case the file is an old one, we try to get a fresh copy
            if (CachedUntil < DateTime.UtcNow)
            {
                GetPricesAsync();
                return;
            }

            // Deserialize the xml file
            SerializableEMDItemPrices result = Util.DeserializeXmlFromFile<SerializableEMDItemPrices>(file);

            if (result == null)
                return;

            // Import the data
            Import(result.Result.ItemPrices);
        }

        /// <summary>
        /// Import the query result list.
        /// </summary>
        /// <param name="itemPrices">The item prices.</param>
        private void Import(IEnumerable<SerializableEMDItemPriceListItem> itemPrices)
        {
            EveMonClient.Trace("{0}.Import - begin", GetType().Name);

            PriceByItemID.Clear();
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

            EveMonClient.Trace("{0}.Import - done", GetType().Name);
        }

        /// <summary>
        /// Downloads the item prices list.
        /// </summary>
        protected override void GetPricesAsync()
        {
            // Quit if query is pending
            if (s_queryPending)
                return;

            s_queryPending = true;

            EveMonClient.Trace("{0}.GetPricesAsync - begin", GetType().Name);

            var url = new Uri(
                String.Format(CultureConstants.InvariantCulture, "{0}{1}", NetworkConstants.EVEMarketDataBaseUrl,
                    NetworkConstants.EVEMarketDataAPIItemPrices));

            Util.DownloadXmlAsync<SerializableEMDItemPrices>(url, OnPricesDownloaded, true);
        }

        /// <summary>
        /// Called when data downloaded.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="errormessage">The errormessage.</param>
        private void OnPricesDownloaded(SerializableEMDItemPrices result, string errormessage)
        {
            if (result == null)
                return;

            if (!String.IsNullOrEmpty(errormessage))
            {
                // Reset query pending flag
                s_queryPending = false;

                EveMonClient.Trace(errormessage);
                return;
            }

            EveMonClient.Trace("{0}.GetPricesAsync - done", GetType().Name);

            Import(result.Result.ItemPrices);

            // Reset query pending flag
            s_queryPending = false;

            // Save the file in cache
            Save(Filename, Util.SerializeToXmlDocument(result));
            
            CachedUntil = DateTime.UtcNow.AddDays(1);

            EveMonClient.OnPricesDownloaded(null, String.Empty);
        }

        #endregion
    }
}
