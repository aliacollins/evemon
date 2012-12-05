using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.BattleClinic.MarketPrices
{
    public static class BCItemPrices
    {
        /// <summary>
        /// Occurs when BattleClinic item prices updated.
        /// </summary>
        public static event EventHandler BCItemPricesUpdated;


        #region Fields

        private static readonly Dictionary<int, double> s_priceByItemID = new Dictionary<int, double>();
        private static readonly string s_file = LocalXmlCache.GetFile("xml_item_prices").FullName;

        private static bool s_loaded;
        private static bool s_queryPending;

        private static DateTime s_cachedUntil;

        #endregion


        #region Public Finders

        /// <summary>
        /// Gets the price by type ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static double GetPriceByTypeID(int id)
        {
            // Ensure list importation
            EnsureImportation();

            double result;
            s_priceByItemID.TryGetValue(id, out result);
            return result;
        }

        #endregion


        #region Importation - Exportation Methods

        /// <summary>
        /// Ensures the list has been imported.
        /// </summary>
        private static void EnsureImportation()
        {
            // Update the file if we don't have it or the data have expired
            if (!File.Exists(s_file) || (s_loaded && s_cachedUntil < DateTime.UtcNow))
            {
                UpdateFile();
                return;
            }

            // Exit if we have already imported the list
            if (s_loaded)
                return;

            // Deserialize the xml file
            SerializableBCItemPrices result = Util.DeserializeXmlFromFile<SerializableBCItemPrices>(s_file);

            // In case the file has an error or it's an old one, we try to get a fresh copy
            if (result == null || result.CachedUntil.ToUniversalTime() < DateTime.UtcNow)
            {
                UpdateFile();
                return;
            }

            // Import the data
            s_cachedUntil = result.CachedUntil.ToUniversalTime();
            Import(result.ItemPrices);
        }

        /// <summary>
        /// Import the query result list.
        /// </summary>
        private static void Import(IEnumerable<SerializableBCItemPrice> itemPrices)
        {
            EveMonClient.Trace("BCMarketPrices.Import - begin");

            s_priceByItemID.Clear();
            foreach (SerializableBCItemPrice item in itemPrices)
            {
                s_priceByItemID[item.ID] = item.Price;
            }

            s_loaded = true;
            EveMonClient.Trace("BCMarketPrices.Import - done");
        }

        /// <summary>
        /// Downloads the item prices list.
        /// </summary>
        private static void UpdateFile()
        {
            // Quit if query is pending
            if (s_queryPending)
                return;

            Util.DownloadXmlAsync<SerializableBCItemPrices>(new Uri(NetworkConstants.BattleclinicItemPrices), OnDownloaded, true);

            s_queryPending = true;
        }

        /// <summary>
        /// Called when data downloaded.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="errormessage">The errormessage.</param>
        private static void OnDownloaded(SerializableBCItemPrices result, string errormessage)
        {
            if (!String.IsNullOrEmpty(errormessage))
            {
                EveMonClient.Trace(errormessage);
                return;
            }

            // Save the file in cache
            Save(result);

            s_cachedUntil = result.CachedUntil.ToUniversalTime();
            Import(result.ItemPrices);

            if (BCItemPricesUpdated != null)
                BCItemPricesUpdated(null, EventArgs.Empty);
        }

        /// <summary>
        /// Saves the specified result.
        /// </summary>
        /// <param name="result">The result.</param>
        private static void Save(SerializableBCItemPrices result)
        {
            EveMonClient.EnsureCacheDirInit();
            FileHelper.OverwriteOrWarnTheUser(s_file,
                                              fs =>
                                                  {
                                                      XmlSerializer xs = new XmlSerializer(typeof(SerializableBCItemPrices));
                                                      xs.Serialize(fs, result);
                                                      fs.Flush();
                                                      return true;
                                                  });
        }

        #endregion
    }
}