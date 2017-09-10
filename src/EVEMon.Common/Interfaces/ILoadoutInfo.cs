using System.Collections.ObjectModel;
using EVEMon.Common.Data;
using EVEMon.Common.Helpers;

namespace EVEMon.Common.Interfaces
{
    public interface ILoadoutInfo
    {
        /// <summary>
        /// Gets or sets the ship of the loadout.
        /// </summary>
        /// <value>
        /// The ship.
        /// </value>
        Item Ship { get; set; }

        /// <summary>
        /// Gets or sets the loadouts.
        /// </summary>
        /// <value>
        /// The loadouts.
        /// </value>
        Collection<Loadout> Loadouts { get; set; }
    }
}