using System;
using System.Linq;
using EVEMon.Common.Constants;
using EVEMon.Common.Data;
using EVEMon.Common.Helpers;
using EVEMon.Common.Interfaces;
using EVEMon.Common.Serialization.BattleClinic.Loadout;

namespace EVEMon.Common.Loadouts.BattleClinic
{
    public sealed class BCLoadoutsProvider : LoadoutsProvider
    {
        #region Fields

        private static bool s_queryFeedPending;
        private static bool s_queryPending;

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
            get { return "BattleClinic"; }
        }

        /// <summary>
        /// Gets the topic URL.
        /// </summary>
        /// <value>
        /// The topic URL.
        /// </value>
        public override string TopicUrl
        {
            get { return NetworkConstants.BattleClinicLoadoutTopic; }
        }


        #endregion


        #region Inherited Methods

        /// <summary>
        /// Gets the loadouts feed asynchronous.
        /// </summary>
        /// <param name="ship">The ship.</param>
        public override void GetLoadoutsFeedAsync(Item ship)
        {
            // Quit if query is pending
            if (s_queryFeedPending)
                return;

            Uri url = new Uri(String.Format(CultureConstants.InvariantCulture, "{0}{1}", NetworkConstants.BattleClinicEVEBase,
                String.Format(CultureConstants.InvariantCulture, NetworkConstants.BattleClinicLoadoutsFeed, ship.ID)));

            Util.DownloadXmlAsync<SerializableLoadoutFeed>(url, OnLoadoutsFeedDownloaded, true);

            s_queryFeedPending = true;
        }

        /// <summary>
        /// Gets the loadout by identifier asynchronous.
        /// </summary>
        /// <param name="id">The id.</param>
        public override void GetLoadoutByIDAsync(int id)
        {
            // Quit if query is pending
            if (s_queryPending)
                return;

            Uri url = new Uri(String.Format(CultureConstants.InvariantCulture, "{0}{1}", NetworkConstants.BattleClinicEVEBase,
                String.Format(CultureConstants.InvariantCulture, NetworkConstants.BattleClinicLoadoutDetails, id)));

            Util.DownloadXmlAsync<SerializableLoadoutFeed>(url, OnLoadoutDownloaded, true);

            s_queryPending = true;
        }

        /// <summary>
        /// Deserializes the loadouts feed.
        /// </summary>
        /// <param name="ship">The ship.</param>
        /// <param name="feed">The feed.</param>
        public override ILoadoutInfo DeserializeLoadoutsFeed(Item ship, object feed)
        {
            SerializableLoadoutFeed loadoutFeed = feed as SerializableLoadoutFeed;

            if (loadoutFeed == null)
                return new LoadoutInfo();

            return LoadoutHelper.DeserializeBCXMLFeedFormat(ship, loadoutFeed);
        }

        /// <summary>
        /// Deserializes the loadout.
        /// </summary>
        /// <param name="loadout">The loadout.</param>
        /// <param name="feed">The feed.</param>
        public override void DeserializeLoadout(Loadout loadout, object feed)
        {
            SerializableLoadoutFeed loadoutFeed = feed as SerializableLoadoutFeed;

            if (loadoutFeed == null)
                return;

            LoadoutHelper.DeserializeBCXMLFormat(loadout, loadoutFeed.Race.Loadouts.First().Slots);
        }

        /// <summary>
        /// Called when we downloaded a loadouts feed from BattleClinic.
        /// </summary>
        /// <param name="loadoutFeed"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private static void OnLoadoutsFeedDownloaded(object loadoutFeed, string errorMessage)
        {
            s_queryFeedPending = false;

            EveMonClient.OnLoadoutsFeedDownloaded(loadoutFeed, errorMessage);
        }

        /// <summary>
        /// Called when we downloaded a loadout from BattleClinic
        /// </summary>
        /// <param name="loadout"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private static void OnLoadoutDownloaded(object loadout, string errorMessage)
        {
            s_queryPending = false;

            EveMonClient.OnLoadoutDownloaded(loadout, errorMessage);
        }

        #endregion
    }
}
