using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using EVEMon.Common.Serialization.EveAddicts.MarketPricer;

namespace EVEMon.Common.MarketPricer.EveAddicts
{
    public sealed class EAItemPricer : ItemPricer
    {
        /// <summary>
        /// Occurs when EVE Addicts item prices updated.
        /// </summary>
        public override event EventHandler ItemPricesUpdated;


        #region Fields

        private static readonly Dictionary<int, double> s_priceByItemID = new Dictionary<int, double>();

        private static bool s_queryPending;
        private static bool s_loaded;

        private static DateTime s_cachedUntil;

        #endregion


        public override string Name
        {
            get { return "EveAddicts"; }
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
            string file = LocalXmlCache.GetFile("ea_item_prices").FullName;

            // Update the file if we don't have it or the data have expired
            if (!File.Exists(file) || (s_loaded && s_cachedUntil < DateTime.UtcNow))
            {
                UpdateFile();
                return;
            }

            // Exit if we have already imported the list
            if (s_loaded)
                return;

            s_cachedUntil = File.GetLastWriteTimeUtc(file).AddDays(1);

            // In case the file has an error or it's an old one, we try to get a fresh copy
            if (s_cachedUntil < DateTime.UtcNow)
            {
                UpdateFile();
                return;
            }

            // Deserialize the xml file
            SerializableEAItemPrices result = Util.DeserializeXmlFromFile<SerializableEAItemPrices>(file);

            // Import the data
            Import(result.ItemPrices);
        }

        /// <summary>
        /// Import the query result list.
        /// </summary>
        private static void Import(IEnumerable<SerializableEAItemPrice> itemPrices)
        {
            EveMonClient.Trace("EAItemPricer.Import - begin");

            s_priceByItemID.Clear();
            foreach (SerializableEAItemPrice item in itemPrices)
            {
                s_priceByItemID[item.ID] = item.Price;
            }

            s_loaded = true;

            // Reset query pending flag
            s_queryPending = false;

            EveMonClient.Trace("EAItemPricer.Import - done");
        }

        /// <summary>
        /// Downloads the item prices list.
        /// </summary>
        private void UpdateFile()
        {
            // Quit if query is pending
            if (s_queryPending)
                return;

            EveMonClient.Trace("EAItemPricer.UpdateFile - begin");

            var url = new Uri(
                String.Format(CultureConstants.InvariantCulture, "{0}{1}", NetworkConstants.EVEAddictsBaseUrl,
                    NetworkConstants.EVEAddictsAPIItemPrices));

            Util.DownloadXmlAsync<SerializableEAItemPrices>(url, OnDownloaded, true);

            s_queryPending = true;
        }

        /// <summary>
        /// Called when data downloaded.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="errormessage">The errormessage.</param>
        private void OnDownloaded(SerializableEAItemPrices result, string errormessage)
        {
            if (!String.IsNullOrEmpty(errormessage))
            {
                // Reset query pending flag
                s_queryPending = false;

                EveMonClient.Trace(errormessage);
                return;
            }

            // Save the file in cache
            Save(result);

            EveMonClient.Trace("EAItemPricer.UpdateFile - done");

            s_cachedUntil = DateTime.UtcNow.AddDays(1);

            Import(result.ItemPrices);

            if (ItemPricesUpdated != null)
                ItemPricesUpdated(null, EventArgs.Empty);
        }

        /// <summary>
        /// Saves the specified result.
        /// </summary>
        /// <param name="result">The result.</param>
        private static void Save(SerializableEAItemPrices result)
        {
            EveMonClient.EnsureCacheDirInit();
            FileHelper.OverwriteOrWarnTheUser(LocalXmlCache.GetFile("ea_item_prices").FullName,
                fs =>
                {
                    XmlSerializer xs = new XmlSerializer(typeof(SerializableEAItemPrices));
                    xs.Serialize(fs, result);
                    fs.Flush();
                    return true;
                });
        }

        #endregion
    }
}
