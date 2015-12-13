using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Constants;
using EVEMon.Common.Data;
using EVEMon.Common.Helpers;
using EVEMon.Common.Interfaces;
using EVEMon.Common.Net;
using EVEMon.Common.Serialization.Osmium.Loadout;
using EVEMon.Common.Threading;

namespace EVEMon.Common.Loadouts.Osmium
{
    public sealed class OsmiumLoadoutsProvider : LoadoutsProvider
    {
        #region Fields

        private static bool s_queryFeedPending;
        private static bool s_queryPending;
        private Loadout s_selectedLoadout;

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public override string Name
        {
            get { return "Osmium"; }
        }

        /// <summary>
        /// Gets the topic URL.
        /// </summary>
        /// <value>
        /// The topic URL.
        /// </value>
        public override Uri TopicUrl
        {
            get
            {
                return new Uri(String.Format(CultureConstants.InvariantCulture, "{0}{1}", NetworkConstants.OsmiumBaseUrl,
                    String.Format(CultureConstants.InvariantCulture, NetworkConstants.OsmiumLoadoutTopic, s_selectedLoadout.ID)));
            }
        }

        #endregion


        #region Inherited Methods

        /// <summary>
        /// Gets the loadouts feed.
        /// </summary>
        /// <param name="ship">The ship.</param>
        public override void GetLoadoutsFeedAsync(Item ship)
        {
            // Quit if query is pending
            if (s_queryFeedPending)
                return;

            Uri url = new Uri(String.Format(CultureConstants.InvariantCulture, "{0}{1}", NetworkConstants.OsmiumBaseUrl,
                String.Format(CultureConstants.InvariantCulture, NetworkConstants.OsmiumLoadoutFeed, ship.Name)));

            Util.DownloadJsonAsync<List<SerializableOsmiumLoadoutFeed>>(url, OnLoadoutsFeedDownloaded, true);

            s_queryFeedPending = true;
        }

        /// <summary>
        /// Gets the loadout by type ID.
        /// </summary>
        /// <param name="id">The id.</param>
        public override void GetLoadoutByIDAsync(long id)
        {
            // Quit if query is pending
            if (s_queryPending)
                return;

            Uri url = new Uri(String.Format(CultureConstants.InvariantCulture, "{0}{1}", NetworkConstants.OsmiumBaseUrl,
                String.Format(CultureConstants.InvariantCulture, NetworkConstants.OsmiumLoadoutDetails, id)));

            HttpWebService.DownloadStringAsync(url, OnLoadoutDownloaded, null);

            s_queryPending = true;
        }

        /// <summary>
        /// Deserializes the loadouts feed.
        /// </summary>
        /// <param name="ship">The ship.</param>
        /// <param name="feed">The feed.</param>
        /// <returns></returns>
        public override ILoadoutInfo DeserializeLoadoutsFeed(Item ship, object feed)
        {
            if (feed == null)
                throw new ArgumentNullException("feed");

            List<SerializableOsmiumLoadoutFeed> loadoutFeed = feed as List<SerializableOsmiumLoadoutFeed>;

            return loadoutFeed == null
                ? new LoadoutInfo()
                : LoadoutHelper.DeserializeOsmiumJsonFeedFormat(ship, loadoutFeed);
        }

        /// <summary>
        /// Deserializes the loadout.
        /// </summary>
        /// <param name="loadout">The loadout.</param>
        /// <param name="feed">The feed.</param>
        public override void DeserializeLoadout(Loadout loadout, object feed)
        {
            if (loadout == null)
                throw new ArgumentNullException("loadout");

            if (feed == null)
                throw new ArgumentNullException("feed");

            s_selectedLoadout = loadout;

            loadout.Items = LoadoutHelper.DeserializeEFTFormat(feed as string).Loadouts.First().Items;
        }

        /// <summary>
        /// Occurs when we downloaded a loadouts feed from the provider.
        /// </summary>
        /// <param name="loadoutFeed">The loadout feed.</param>
        /// <param name="errorMessage">The error message.</param>
        private static void OnLoadoutsFeedDownloaded(object loadoutFeed, string errorMessage)
        {
            s_queryFeedPending = false;

            EveMonClient.OnLoadoutsFeedDownloaded(loadoutFeed, errorMessage);
        }

        /// <summary>
        /// Occurs when we downloaded a loadout from the provider.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="userstate">The userstate.</param>
        private static void OnLoadoutDownloaded(DownloadStringAsyncResult e, object userstate)
        {
            Dispatcher.Invoke(() =>
            {
                s_queryPending = false;

                EveMonClient.OnLoadoutDownloaded(e.Result, e.Error?.Message);
            });
        }

        #endregion
    }
}
