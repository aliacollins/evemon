using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Data;
using EVEMon.Common.Helpers;
using EVEMon.Common.Interfaces;

namespace EVEMon.Common.Loadouts
{
    public abstract class LoadoutsProvider
    {
        /// <summary>
        /// Occurs when the loadout feed updated.
        /// </summary>
        public event EventHandler<LoadoutFeedEventArgs> LoadoutFeedUpdated;

        /// <summary>
        /// Occurs when the loadout updated.
        /// </summary>
        public event EventHandler<LoadoutEventArgs> LoadoutUpdated;

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the topic URL.
        /// </summary>
        /// <value>
        /// The topic URL.
        /// </value>
        public abstract string TopicUrl { get; }

        /// <summary>
        /// Gets the providers.
        /// </summary>
        /// <value>
        /// The providers.
        /// </value>
        internal static IEnumerable<LoadoutsProvider> Providers
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetTypes()
                    .Where(type => typeof(LoadoutsProvider).IsAssignableFrom(type) && type.GetConstructor(Type.EmptyTypes) != null)
                    .Select(type => Activator.CreateInstance(type) as LoadoutsProvider)
                    .Where(provider => !String.IsNullOrWhiteSpace(provider.Name))
                    .OrderBy(provider => provider.Name);
            }
        }

        /// <summary>
        /// Gets the loadouts feed asynchronous.
        /// </summary>
        /// <param name="ship">The ship.</param>
        public abstract void GetLoadoutsFeedAsync(Item ship);

        /// <summary>
        /// Gets the loadout by identifier asynchronous.
        /// </summary>
        /// <param name="id">The id.</param>
        public abstract void GetLoadoutByIDAsync(int id);

        /// <summary>
        /// Deserializes the loadout.
        /// </summary>
        /// <param name="loadout">The loadout.</param>
        /// <param name="feed">The feed.</param>
        public abstract void DeserializeLoadout(Loadout loadout, object feed);

        /// <summary>
        /// Deserializes the loadouts feed.
        /// </summary>
        /// <param name="ship">The ship.</param>
        /// <param name="feed">The feed.</param>
        public abstract ILoadoutInfo DeserializeLoadoutsFeed(Item ship, object feed);

        /// <summary>
        /// Occurs when we downloaded a loadouts feed from the provider.
        /// </summary>
        /// <param name="loadoutFeed">The loadout feed.</param>
        /// <param name="errorMessage">The error message.</param>
        protected virtual void OnLoadoutsFeedDownloaded(object loadoutFeed, string errorMessage)
        {
            if (LoadoutFeedUpdated != null)
                LoadoutFeedUpdated(this, new LoadoutFeedEventArgs(loadoutFeed, errorMessage));
        }

        /// <summary>
        /// Occurs when we downloaded a loadout from the provider.
        /// </summary>
        /// <param name="loadout">The loadout.</param>
        /// <param name="errorMessage">The error message.</param>
        protected virtual void OnLoadoutDownloaded(object loadout, string errorMessage)
        {
            if (LoadoutUpdated != null)
                LoadoutUpdated(this, new LoadoutEventArgs(loadout, errorMessage));
        }
    }
}
