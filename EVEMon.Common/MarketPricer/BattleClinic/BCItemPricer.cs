using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using EVEMon.Common.Serialization.BattleClinic.MarketPricer;

namespace EVEMon.Common.MarketPricer.BattleClinic
{
    public sealed class BCItemPricer : ItemPricer
    {
        /// <summary>
        /// Occurs when BattleClinic item prices updated.
        /// </summary>
        public override event EventHandler ItemPricesUpdated;


        #region Fields

        private static readonly Dictionary<int, double> s_priceByItemID = new Dictionary<int, double>();

        private const string Filename = "bc_item_prices";

        private static bool s_queryPending;
        private static bool s_loaded;

        private static DateTime s_cachedUntil;

        #endregion


        public override string Name
        {
            get { return "BattleClinic"; }
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
            s_priceByItemID.TryGetValue(id, out result);
            return result;
        }


        #region Importation - Exportation Methods

        /// <summary>
        /// Ensures the list has been imported.
        /// </summary>
        private void EnsureImportation()
        {
            string file = LocalXmlCache.GetFile(Filename).FullName;

            // Update the file if we don't have it or the data have expired
            if (!File.Exists(file) || (s_loaded && s_cachedUntil < DateTime.UtcNow))
            {
                UpdateFile();
                return;
            }

            // Exit if we have already imported the list
            if (s_loaded)
                return;
            
            SerializableBCItemPrices result = null;

            // Deserialize the xml file
            if (s_cachedUntil == DateTime.MinValue)
                result = Util.DeserializeXmlFromFile<SerializableBCItemPrices>(file);

            s_cachedUntil = result != null ? result.CachedUntil.ToUniversalTime() : s_cachedUntil;

            // In case the file has an error or it's an old one, we try to get a fresh copy
            if (result == null || s_cachedUntil < DateTime.UtcNow)
            {
                UpdateFile();
                return;
            }

            // Import the data
            Import(result.ItemPrices);
        }

        /// <summary>
        /// Import the query result list.
        /// </summary>
        private static void Import(IEnumerable<SerializableBCItemPrice> itemPrices)
        {
            EveMonClient.Trace("BCItemPricer.Import - begin");

            s_priceByItemID.Clear();
            foreach (SerializableBCItemPrice item in itemPrices)
            {
                s_priceByItemID[item.ID] = item.Price;
            }

            s_loaded = true;

            // Reset query pending flag
            s_queryPending = false;

            EveMonClient.Trace("BCItemPricer.Import - done");
        }

        /// <summary>
        /// Downloads the item prices list.
        /// </summary>
        private void UpdateFile()
        {
            // Quit if query is pending
            if (s_queryPending)
                return;

            EveMonClient.Trace("BCItemPricer.UpdateFile - begin");

            var url = new Uri(
                String.Format(CultureConstants.InvariantCulture, "{0}{1}", NetworkConstants.BattleClinicEVEBase,
                    NetworkConstants.BattleClinicItemPrices));

            Util.DownloadXmlAsync<SerializableBCItemPrices>(url, OnDownloaded, true);

            s_queryPending = true;
        }

        /// <summary>
        /// Called when data downloaded.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="errormessage">The errormessage.</param>
        private void OnDownloaded(SerializableBCItemPrices result, string errormessage)
        {
            if (!String.IsNullOrEmpty(errormessage))
            {
                // Reset query pending flag
                s_queryPending = false;

                EveMonClient.Trace(errormessage);
                return;
            }

            // Save the file in cache
            Save(Filename, Util.SerializeToXmlDocument(result));

            EveMonClient.Trace("BCItemPricer.UpdateFile - done");

            s_cachedUntil = result.CachedUntil.ToUniversalTime();
            Import(result.ItemPrices);

            if (ItemPricesUpdated != null)
                ItemPricesUpdated(null, EventArgs.Empty);
        }

        #endregion
    }
}
