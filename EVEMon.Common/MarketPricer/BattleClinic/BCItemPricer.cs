using System;
using System.Collections.Generic;
using System.IO;
using EVEMon.Common.Constants;
using EVEMon.Common.Serialization.BattleClinic.MarketPricer;
using EVEMon.Common.Service;

namespace EVEMon.Common.MarketPricer.BattleClinic
{
    public sealed class BCItemPricer : ItemPricer
    {
        #region Fields

        private const string Filename = "bc_item_prices";

        private static bool s_queryPending;


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
            
            SerializableBCItemPrices result = null;

            // Deserialize the xml file
            if (CachedUntil == DateTime.MinValue)
                result = Util.DeserializeXmlFromFile<SerializableBCItemPrices>(file);

            CachedUntil = result != null ? result.CachedUntil.ToUniversalTime() : CachedUntil;

            // In case the file has an error or it's an old one, we try to get a fresh copy
            if (result == null || CachedUntil < DateTime.UtcNow)
            {
                GetPricesAsync();
                return;
            }

            // Import the data
            Import(result.ItemPrices);
        }

        /// <summary>
        /// Import the query result list.
        /// </summary>
        /// <param name="itemPrices">The item prices.</param>
        private static void Import(IEnumerable<SerializableBCItemPrice> itemPrices)
        {
            EveMonClient.Trace("BCItemPricer.Import - begin");

            PriceByItemID.Clear();
            foreach (SerializableBCItemPrice item in itemPrices)
            {
                PriceByItemID[item.ID] = item.Price;
            }

            Loaded = true;

            // Reset query pending flag
            s_queryPending = false;

            EveMonClient.Trace("BCItemPricer.Import - done");
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

            EveMonClient.Trace("BCItemPricer.GetPricesAsync - begin");

            var url = new Uri(
                String.Format(CultureConstants.InvariantCulture, "{0}{1}", NetworkConstants.BattleClinicEVEBase,
                    NetworkConstants.BattleClinicItemPrices));

            Util.DownloadXmlAsync<SerializableBCItemPrices>(url, OnPricesDownloaded, true);

            s_queryPending = true;
        }

        /// <summary>
        /// Called when prices downloaded.
        /// </summary>
        /// <param name="pricesFeed">The prices feed.</param>
        /// <param name="errormessage">The errormessage.</param>
        protected override void OnPricesDownloaded(object pricesFeed, string errormessage)
        {
            SerializableBCItemPrices result = pricesFeed as SerializableBCItemPrices;

            if (result == null)
                return;

            if (!String.IsNullOrEmpty(errormessage))
            {
                // Reset query pending flag
                s_queryPending = false;

                EveMonClient.Trace(errormessage);
                return;
            }

            // Save the file in cache
            Save(Filename, Util.SerializeToXmlDocument(result));

            EveMonClient.Trace("BCItemPricer.GetPricesAsync - done");

            CachedUntil = result.CachedUntil.ToUniversalTime();
            Import(result.ItemPrices);

            base.OnPricesDownloaded(result, errormessage);
        }

        #endregion
    }
}
