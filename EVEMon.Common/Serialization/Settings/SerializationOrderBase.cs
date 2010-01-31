using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using EVEMon.Common.Attributes;

namespace EVEMon.Common.Serialization.Settings
{
    #region SerializableOrderBase
    /// <summary>
    /// Represents a market sell order.
    /// </summary>
    public class SerializableOrderBase
    {
        /// <summary>
        /// True if the user choose to delete this order.
        /// </summary>
        [XmlAttribute("ignored")]
        public bool Ignored
        {
            get;
            set;
        }

        /// <summary>
        /// Unique order ID for this order. Note that these are not guaranteed to be unique forever, they can recycle. 
        /// But they are unique for the purpose of one data pull. 
        /// </summary>
        [XmlAttribute("orderID")]
        public int OrderID
        {
            get;
            set;
        }

        [XmlAttribute("itemID")]
        public int ItemID
        {
            get;
            set;
        }

        [XmlAttribute("item")]
        public string Item
        {
            get;
            set;
        }

        [XmlAttribute("stationID")]
        public int StationID
        {
            get;
            set;
        }

        [XmlAttribute("volEntered")]
        public int InitialVolume
        {
            get;
            set;
        }

        [XmlAttribute("volRemaining")]
        public int RemainingVolume
        {
            get;
            set;
        }

        /// <summary>
        /// The minimum volume a buyer can buy.
        /// </summary>
        [XmlAttribute("minVolume")]
        public int MinVolume
        {
            get;
            set;
        }

        /// <summary>
        /// The state of the order.
        /// </summary>
        [XmlAttribute("orderState")]
        public OrderState State
        {
            get;
            set;
        }

        /// <summary>
        /// How many days this order is good for. Expiration is issued + duration in days. 
        /// </summary>
        [XmlAttribute("duration")]
        public int Duration
        {
            get;
            set;
        }

        /// <summary>
        /// The cost per unit for this order.
        /// </summary>
        [XmlAttribute("price")]
        public decimal UnitaryPrice
        {
            get;
            set;
        }

        /// <summary>
        /// The time this order was issued.
        /// </summary>
        [XmlAttribute("issued")]
        public DateTime Issued
        {
            get;
            set;
        }

        /// <summary>
        /// The time this order was issued.
        /// </summary>
        [XmlAttribute("lastStateChange")]
        public DateTime LastStateChange
        {
            get;
            set;
        }
    }
    #endregion


    #region SerializableSellOrder
    /// <summary>
    /// Represents a sell order.
    /// </summary>
    public sealed class SerializableSellOrder : SerializableOrderBase
    {
        public SerializableSellOrder Clone()
        {
            return (SerializableSellOrder)MemberwiseClone();
        }
    }
    #endregion


    #region SerializableBuyOrder
    /// <summary>
    /// Represents a buy order.
    /// </summary>
    public sealed class SerializableBuyOrder : SerializableOrderBase
    {
        /// <summary>
        /// How much ISK is in escrow.
        /// </summary>
        [XmlAttribute("escrow")]
        public decimal Escrow
        {
            get;
            set;
        }

        /// <summary>
        /// -1 = station, 0 = solar system, 1 = 1 jump, 2 = 2 jumps, ..., 32767 = region.
        /// </summary>
        [XmlAttribute("range")]
        public int Range
        {
            get;
            set;
        }

        public SerializableBuyOrder Clone()
        {
            return (SerializableBuyOrder)MemberwiseClone();
        }
    }
    #endregion
}
