using System;
using EVEMon.Common.Data;
using EVEMon.Common.Helpers;
using EVEMon.Common.Interfaces;

namespace EVEMon.Common.Loadouts.Osmium
{
    public sealed class OsmiumLoadoutsProvider : LoadoutsProvider
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public override string Name { get; }

        /// <summary>
        /// Gets the topic URL.
        /// </summary>
        /// <value>
        /// The topic URL.
        /// </value>
        public override string TopicUrl { get; }

        /// <summary>
        /// Gets the loadouts feed.
        /// </summary>
        /// <param name="ship">The ship.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void GetLoadoutsFeedAsync(Item ship)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the loadout by type ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void GetLoadoutByIDAsync(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deserializes the loadout.
        /// </summary>
        /// <param name="loadout">The loadout.</param>
        /// <param name="feed">The feed.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void DeserializeLoadout(Loadout loadout, object feed)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deserializes the loadouts feed.
        /// </summary>
        /// <param name="ship">The ship.</param>
        /// <param name="feed">The feed.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override ILoadoutInfo DeserializeLoadoutsFeed(Item ship, object feed)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Occurs when we downloaded a loadouts feed from the provider.
        /// </summary>
        /// <param name="feed">The feed.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private static void OnLoadoutsFeedDownloaded(object feed, string errorMessage)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Occurs when we downloaded a loadout from the provider.
        /// </summary>
        /// <param name="loadout">The loadout.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private static void OnLoadoutDownloaded(object loadout, string errorMessage)
        {
            throw new NotImplementedException();
        }
    }
}
