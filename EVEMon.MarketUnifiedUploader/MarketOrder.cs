using System;
using System.Collections.Generic;
using EVEMon.Common;

namespace EVEMon.MarketUnifiedUploader
{
    internal sealed class MarketOrder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarketOrder"/> class.
        /// </summary>
        /// <param name="order">The order.</param>
        public MarketOrder(IDictionary<object, object> order)
        {
            Price = Convert.ToDecimal(order["price"], CultureConstants.InvariantCulture);
            VolumeRemaining = Convert.ToDouble(order["volRemaining"], CultureConstants.InvariantCulture);
            OrderID = Convert.ToInt64(order["orderID"], CultureConstants.InvariantCulture);
            IssueDate = DateTime.FromFileTimeUtc(Convert.ToInt64(order["issueDate"], CultureConstants.InvariantCulture));
            TypeID = Convert.ToInt32(order["typeID"], CultureConstants.InvariantCulture);
            VolumeEntered = Convert.ToInt32(order["volEntered"], CultureConstants.InvariantCulture);
            MinVolume = Convert.ToInt32(order["minVolume"], CultureConstants.InvariantCulture);
            StationID = Convert.ToInt32(order["stationID"], CultureConstants.InvariantCulture);
            RegionID = Convert.ToInt32(order["regionID"], CultureConstants.InvariantCulture);
            SolarSystemID = Convert.ToInt32(order["solarSystemID"], CultureConstants.InvariantCulture);
            Jumps = Convert.ToInt32(order["jumps"], CultureConstants.InvariantCulture);
            Range = Convert.ToInt16(order["range"], CultureConstants.InvariantCulture);
            Duration = Convert.ToInt16(order["duration"], CultureConstants.InvariantCulture);
            Bid = Convert.ToBoolean(order["bid"], CultureConstants.InvariantCulture);
        }

        /// <summary>
        /// Gets the price.
        /// </summary>
        internal decimal Price { get; private set; }

        /// <summary>
        /// Gets the volume remaining.
        /// </summary>
        internal double VolumeRemaining { get; private set; }

        /// <summary>
        /// Gets the order ID.
        /// </summary>
        internal long OrderID { get; private set; }

        /// <summary>
        /// Gets the issue date.
        /// </summary>
        internal DateTime IssueDate { get; private set; }

        /// <summary>
        /// Gets the type ID.
        /// </summary>
        internal int TypeID { get; private set; }

        /// <summary>
        /// Gets the volume entered.
        /// </summary>
        internal int VolumeEntered { get; private set; }

        /// <summary>
        /// Gets the min volume.
        /// </summary>
        internal int MinVolume { get; private set; }

        /// <summary>
        /// Gets the station ID.
        /// </summary>
        internal int StationID { get; private set; }

        /// <summary>
        /// Gets the region ID.
        /// </summary>
        internal int RegionID { get; private set; }

        /// <summary>
        /// Gets the solar system ID.
        /// </summary>
        internal int SolarSystemID { get; private set; }

        /// <summary>
        /// Gets the jumps.
        /// </summary>
        internal int Jumps { get; private set; }

        /// <summary>
        /// Gets the range.
        /// </summary>
        internal short Range { get; private set; }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        internal short Duration { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="MarketOrder"/> is bid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if bid; otherwise, <c>false</c>.
        /// </value>
        internal bool Bid { get; private set; }
    }
}