using System;

namespace EVEMon.Common.Data
{
    public sealed class NPCCorporation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NPCCorporation"/> class.
        /// </summary>
        /// <param name="station">The station.</param>
        public NPCCorporation(Station station)
        {
            if (station == null)
                throw new ArgumentNullException("station");

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
        public string Name { get; set; }
    }
}