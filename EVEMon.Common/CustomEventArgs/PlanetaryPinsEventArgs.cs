using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EVEMon.Common.CustomEventArgs
{
    public sealed class PlanetaryPinsEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="planetaryPins">The planetary pins.</param>
        public PlanetaryPinsEventArgs(Character character, IEnumerable<PlanetaryPin> planetaryPins)
        {
            Character = character;
            CompletedPins = planetaryPins.ToList().AsReadOnly();
        }

        /// <summary>
        /// Gets or sets the character related to this event.
        /// </summary>
        public Character Character { get; private set; }

        /// <summary>
        /// Gets or sets the planetary pins related to this event.
        /// </summary>
        public ReadOnlyCollection<PlanetaryPin> CompletedPins { get; private set; }
    }
}
