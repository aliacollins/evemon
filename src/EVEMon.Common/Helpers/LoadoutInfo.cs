using System.Collections.ObjectModel;
using EVEMon.Common.Data;
using EVEMon.Common.Interfaces;

namespace EVEMon.Common.Helpers
{
    public sealed class LoadoutInfo : ILoadoutInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoadoutInfo"/> class.
        /// </summary>
        public LoadoutInfo()
        {
            Ship = Item.UnknownItem;
            Loadouts = new Collection<Loadout>();
        }

        /// <summary>
        /// Gets or sets the ship of the loadout.
        /// </summary>
        /// <value>
        /// The ship.
        /// </value>
        public Item Ship { get; set; }

        /// <summary>
        /// Gets or sets the loadouts.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public Collection<Loadout> Loadouts { get; set; }
    }
}