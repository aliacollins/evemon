using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
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

            Util.DownloadXmlAsync<SerializableBCLoadoutFeed>(url, OnLoadoutsFeedDownloaded, true);

            s_queryFeedPending = true;
        }

        /// <summary>
        /// Gets the loadout by identifier asynchronous.
        /// </summary>
        /// <param name="id">The id.</param>
        public override void GetLoadoutByIDAsync(long id)
        {
            // Quit if query is pending
            if (s_queryPending)
                return;

            Uri url = new Uri(String.Format(CultureConstants.InvariantCulture, "{0}{1}", NetworkConstants.BattleClinicEVEBase,
                String.Format(CultureConstants.InvariantCulture, NetworkConstants.BattleClinicLoadoutDetails, id)));

            Util.DownloadXmlAsync<SerializableBCLoadoutFeed>(url, OnLoadoutDownloaded, true);

            s_queryPending = true;
        }

        /// <summary>
        /// Deserializes the loadouts feed.
        /// </summary>
        /// <param name="ship">The ship.</param>
        /// <param name="feed">The feed.</param>
        public override ILoadoutInfo DeserializeLoadoutsFeed(Item ship, object feed)
        {
            if (feed == null)
                throw new ArgumentNullException("feed");

            SerializableBCLoadoutFeed loadoutFeed = feed as SerializableBCLoadoutFeed;

            return loadoutFeed == null
                ? new LoadoutInfo()
                : DeserializeBCXMLFeedFormat(ship, loadoutFeed);
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

            SerializableBCLoadoutFeed loadoutFeed = feed as SerializableBCLoadoutFeed;

            if (loadoutFeed == null)
                return;

            DeserializeBCXMLLoadoutFormat(loadout, loadoutFeed.Race.Loadouts.First().Slots);
        }

        /// <summary>
        /// Called when we downloaded a loadouts feed from BattleClinic.
        /// </summary>
        /// <param name="loadoutFeed">The loadout feed.</param>
        /// <param name="errorMessage">The error message.</param>
        private static void OnLoadoutsFeedDownloaded(object loadoutFeed, string errorMessage)
        {
            s_queryFeedPending = false;

            EveMonClient.OnLoadoutsFeedDownloaded(loadoutFeed, errorMessage);
        }

        /// <summary>
        /// Called when we downloaded a loadout from BattleClinic
        /// </summary>
        /// <param name="loadout">The loadout.</param>
        /// <param name="errorMessage">The error message.</param>
        private static void OnLoadoutDownloaded(object loadout, string errorMessage)
        {
            s_queryPending = false;

            EveMonClient.OnLoadoutDownloaded(loadout, errorMessage);
        }

        /// <summary>
        /// Deserializes the BattleClinic XML feed format.
        /// </summary>
        /// <param name="ship">The ship.</param>
        /// <param name="feed">The feed.</param>
        /// <returns></returns>
        private static ILoadoutInfo DeserializeBCXMLFeedFormat(Item ship, SerializableBCLoadoutFeed feed)
        {
            ILoadoutInfo loadoutInfo = new LoadoutInfo
            {
                Ship = ship
            };

            loadoutInfo.Loadouts
                .AddRange(feed.Race.Loadouts
                    .Select(serialLoadout =>
                        new Loadout
                        {
                            ID = serialLoadout.ID,
                            Name = serialLoadout.Name,
                            Description = String.Empty,
                            Author = serialLoadout.Author,
                            Rating = serialLoadout.Rating,
                            SubmissionDate = serialLoadout.SubmissionDate,
                            TopicUrl = new Uri(
                                String.Format(CultureConstants.InvariantCulture,
                                    NetworkConstants.BattleClinicLoadoutTopic, serialLoadout.TopicID)),
                            Items = Enumerable.Empty<Item>()
                        }));

            return loadoutInfo;
        }

        /// <summary>
        /// Deserializes the BattleClinic XML loadout format.
        /// </summary>
        /// <param name="loadout">The loadout.</param>
        /// <param name="slots">The slots.</param>
        private static void DeserializeBCXMLLoadoutFormat(Loadout loadout, IEnumerable<SerializableBCLoadoutSlot> slots)
        {
            var listOfItems = new List<Item>();

            foreach (IGrouping<string, SerializableBCLoadoutSlot> slotType in slots.GroupBy(x => x.SlotType))
            {
                listOfItems.AddRange(slotType.Where(slot => slot.ItemID != 0)
                    .Select(slot => StaticItems.GetItemByID(slot.ItemID))
                    .Where(item => item != null));
            }

            loadout.Items = listOfItems;
        }

        #endregion
    }
}
