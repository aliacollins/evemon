using EVEMon.Common.Extensions;

namespace EVEMon.Common.Data
{
    public sealed class NPCCorporation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NPCCorporation"/> class.
        /// </summary>
        /// <param name="station">The station.</param>
        /// <exception cref="System.ArgumentNullException">station</exception>
        public NPCCorporation(Station station)
        {
            station.ThrowIfNull(nameof(station));

            ID = station.CorporationID;
            Name = station.CorporationName;
        }

        /// <summary>
        /// Gets or sets the corporation's ID.
        /// </summary>
        /// <value>The ID.</value>
        public long ID { get; set; }

        /// <summary>
        /// Gets or sets the corporation's name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }
    }
}