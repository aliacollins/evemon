using System;
using System.Collections.Generic;
using System.Text;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.Settings;
using EVEMon.Common.Serialization.API;
using System.Globalization;
using EVEMon.Common.Attributes;

namespace EVEMon.Common
{
    #region MarketOrder
    /// <summary>
    /// A base class for market orders.
    /// </summary>
    public abstract class MarketOrder
    {
        /// <summary>
        /// The maximum number of days after expiration. Beyond this limit, we do not import orders anymore.
        /// </summary>
        public const int MaxExpirationDays = 7;

        protected bool m_ignored;
        protected bool m_markedForDeletion;
        protected DateTime m_lastStateChange;
        protected OrderState m_state;
        protected DateTime m_issued;

        protected readonly int m_orderID;
        protected readonly Item m_item;
        protected readonly Station m_station;
        protected readonly int m_initialVolume;
        protected readonly int m_minVolume;
        protected readonly int m_duration;

        protected int m_remainingVolume;
        protected decimal m_unitaryPrice;


        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="src"></param>
        protected MarketOrder(SerializableAPIOrder src)
        {
            m_state = GetState(src);
            m_orderID = src.OrderID;
            m_item = StaticItems.GetItem(src.ItemID);
            m_station = StaticGeography.GetStation(src.StationID);
            m_unitaryPrice = src.UnitaryPrice;
            m_initialVolume = src.InitialVolume;
            m_remainingVolume = src.RemainingVolume;
            m_lastStateChange = DateTime.UtcNow;
            m_minVolume = src.MinVolume;
            m_duration = src.Duration;
            m_issued = src.Issued;
        }

        /// <summary>
        /// Constructor from an object deserialized from the settings file.
        /// </summary>
        /// <param name="src"></param>
        protected MarketOrder(SerializableOrderBase src)
        {
            m_ignored = src.Ignored;
            m_orderID = src.OrderID;
            m_state = src.State;
            m_item = StaticItems.FindItem(src.Item);
            m_station = StaticGeography.GetStation(src.StationID);
            m_unitaryPrice = src.UnitaryPrice;
            m_initialVolume = src.InitialVolume;
            m_remainingVolume = src.RemainingVolume;
            m_lastStateChange = src.LastStateChange;
            m_minVolume = src.MinVolume;
            m_duration = src.Duration;
            m_issued = src.Issued;
        }

        /// <summary>
        /// Exports the given object to a serialization object.
        /// </summary>
        /// <returns></returns>
        public abstract SerializableOrderBase Export();

        /// <summary>
        /// Fetches the data to the given source.
        /// </summary>
        /// <param name="src"></param>
        protected void Export(SerializableOrderBase src)
        {
            src.Ignored = m_ignored;
            src.OrderID = m_orderID;
            src.State = m_state;
            src.Item = m_item.Name;
            src.StationID = m_station.ID;
            src.UnitaryPrice = m_unitaryPrice;
            src.InitialVolume = m_initialVolume;
            src.RemainingVolume = m_remainingVolume;
            src.LastStateChange = m_lastStateChange;
            src.MinVolume = m_minVolume;
            src.Duration = m_duration;
            src.Issued = m_issued;
        }

        /// <summary>
        /// Try to update this order with a serialization object from the API.
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        internal bool TryImport(SerializableAPIOrder src, List<MarketOrder> endedOrders)
        {
            // Note that, before a match is found, all ignored orders have been marked for deletion : m_markedForDeletion == true

            // Checks whether ID is the same (IDs can be recycled ?)
            if (this.MatchesWith(src))
            {
                // Update infos (if ID is the same it may have been modified either by the market 
                // or by the user [modify order] so we update the orders info that are changeable)
                if (this.IsModified(src))
                {
                    m_unitaryPrice = src.UnitaryPrice;
                    m_remainingVolume = src.RemainingVolume;
                    m_issued = src.Issued;
                    m_state = OrderState.Modified;
                }
                else if (m_state == OrderState.Modified) m_state = OrderState.Active;

                // Canceled orders are left as marked for deletion.
                OrderState state = GetState(src);
                if (state == OrderState.Canceled || (state == OrderState.Expired && Expiration > DateTime.UtcNow))
                {
                    m_state = OrderState.Canceled;
                    m_markedForDeletion = true;
                    return true;
                }

                // Prevent deletion
                m_markedForDeletion = false;

                // Update state
                if (m_state != OrderState.Modified && state != m_state) // it has either expired or fulfilled
                {
                    m_state = state;
                    m_lastStateChange = DateTime.UtcNow;

                    // Should we notify it to the user ?
                    if (state == OrderState.Expired || state == OrderState.Fulfilled)
                    {
                        if (!m_ignored) endedOrders.Add(this);
                    }
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the state of an order.
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        private OrderState GetState(SerializableAPIOrder src)
        {
            switch ((CCPOrderState)src.State)
            {
                case CCPOrderState.Closed:
                case CCPOrderState.Canceled:
                case CCPOrderState.CharacterDeleted:
                    return OrderState.Canceled;

                case CCPOrderState.Pending:
                case CCPOrderState.Opened:
                    return OrderState.Active;

                case CCPOrderState.ExpiredOrFulfilled:
                    if (src.RemainingVolume == 0) return OrderState.Fulfilled;
                    else return OrderState.Expired;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// When true, the order will be deleted unless it was found on the API feed.
        /// </summary>
        internal bool MarkedForDeletion
        {
            get { return m_markedForDeletion; }
            set { m_markedForDeletion = value; }
        }

        /// <summary>
        /// Gets or sets whether an expired order has been deleted by the user.
        /// </summary>
        public bool Ignored
        {
            get { return m_ignored; }
            set { m_ignored = value; }
        }

        /// <summary>
        /// Gets the order state.
        /// </summary>
        public OrderState State
        {
            get 
            {
                if (IsExpired && m_state == OrderState.Active) return OrderState.Expired;
                return m_state; 
            }
        }

        /// <summary>
        /// Gets the order ID.
        /// </summary>
        public int ID
        {
            get { return m_orderID; }
        }

        /// <summary>
        /// Gets the item. May be null for some items like trade goods and such. In such a case, use <see cref="ItemName"/>.
        /// </summary>
        public Item Item
        {
            get { return m_item; }
        }

        /// <summary>
        /// Gets the station where this order is located.
        /// </summary>
        public Station Station
        {
            get { return m_station; }
        }

        /// <summary>
        /// Gets the intial volume.
        /// </summary>
        public int InitialVolume
        {
            get { return m_initialVolume; }
        }

        /// <summary>
        /// Gets the remaining volume.
        /// </summary>
        public int RemainingVolume
        {
            get { return m_remainingVolume; }
        }

        /// <summary>
        /// Gets the minimum sell/buy threshold volume.
        /// </summary>
        public int MinVolume
        {
            get { return m_minVolume; }
        }

        /// <summary>
        /// Gets the duration, in days, of the order.
        /// </summary>
        public int Duration
        {
            get { return m_duration; }
        }

        /// <summary>
        /// Gets the unitary price.
        /// </summary>
        public decimal UnitaryPrice
        {
            get { return m_unitaryPrice; }
        }

        /// <summary>
        /// Gets the total price.
        /// </summary>
        public decimal TotalPrice
        {
            get { return m_unitaryPrice * m_remainingVolume; }
        }

        /// <summary>
        /// Gets the time (UTC) this order was expired.
        /// </summary>
        public DateTime Issued
        {
            get { return m_issued; }
        }

        /// <summary>
        /// Gets the estimated expiration time.
        /// </summary>
        public DateTime Expiration
        {
            get { return m_issued.AddDays(m_duration); }
        }

        /// <summary>
        /// Gets the last state change.
        /// </summary>
        public DateTime LastStateChange
        {
            get { return m_lastStateChange; }
        }

        /// <summary>
        /// Gets true if order naturally expired because of its duration.
        /// </summary>
        public bool IsExpired
        {
            get { return m_issued.AddDays(m_duration) < DateTime.UtcNow; }
        }

        /// <summary>
        /// Gets true if the order is not fulfilled, canceled, expired, etc.
        /// </summary>
        public bool IsAvailable
        {
            get { return (m_state == OrderState.Active || m_state == OrderState.Modified) && !IsExpired; }
        }

        /// <summary>
        /// Checks whether the given API object matches with this order.
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        internal bool MatchesWith(SerializableAPIOrder src)
        {
            return src.OrderID == m_orderID;
        }

        /// <summary>
        /// Checks whether the given API object matches with this order.
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        internal bool IsModified (SerializableAPIOrder src)
        {
            return (src.UnitaryPrice != m_unitaryPrice && src.Issued != m_issued)
                || (src.RemainingVolume != m_remainingVolume && src.RemainingVolume != 0);
        }

        /// <summary>
        /// Formats the given price to a readable string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Format(decimal value, AbbreviationFormat format)
        {
            decimal abs = Math.Abs(value);
            var culture = CultureInfo.InvariantCulture;
            if (format == AbbreviationFormat.AbbreviationWords)
            {
                if (abs >= 1E9M) return Format("Billions", value / 1E9M);
                if (abs >= 1E6M) return Format("Millions", value / 1E6M);
                if (abs >= 1E3M) return Format("Thousands", value / 1E3M);
                return Format("", value);
            }
            else
            {
                if (abs >= 1E9M) return Format("B", value / 1E9M);
                if (abs >= 1E6M) return Format("M", value / 1E6M);
                if (abs >= 1E3M) return Format("K", value / 1E3M);
                return Format("", value);
            }
        }

        /// <summary>
        /// Formats the given value and suffix the way we want.
        /// </summary>
        /// <param name="suffix"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string Format(string suffix, decimal value)
        {
            // Explanations : 999.99 was displayed as 1000 because only three digits were required.
            // So we do the truncation at hand for the number of digits we exactly request.

            var abs = Math.Abs(value);
            if (abs < 1.0M) return (((int)value * 100) / 100.0M).ToString("0.##") + suffix;
            if (abs < 10.0M) return (((int)value * 1000) / 1000.0M).ToString("#.##") + suffix;
            if (abs < 100.0M) return (((int)value * 1000)/1000.0M).ToString("##.#") + suffix;
            return (((int)value * 1000) / 1000.0M).ToString("###") + suffix;
        }
    }
    #endregion


    public enum AbbreviationFormat
    {
        AbbreviationWords,
        AbbreviationSymbols
    }

    #region BuyOrder
    /// <summary>
    /// This class represents a buy order.
    /// </summary>
    public sealed class BuyOrder : MarketOrder
    {
        private readonly decimal m_escrow;
        private readonly int m_range;

        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="src"></param>
        internal BuyOrder(SerializableAPIOrder src)
            : base(src)
        {
            m_escrow = src.Escrow;
            m_range = src.Range;
        }

        /// <summary>
        /// Constructor from an object deserialized from the settings file.
        /// </summary>
        /// <param name="src"></param>
        internal BuyOrder(SerializableBuyOrder src)
            : base(src)
        {
            m_escrow = src.Escrow;
            m_range = src.Range;
        }

        /// <summary>
        /// Gets the amount currently invested in escrow.
        /// </summary>
        public decimal Escrow
        {
            get { return m_escrow; }
        }

        /// <summary>
        /// Gets the range of this order.
        /// </summary>
        public int Range
        {
            get { return m_range; }
        }

        /// <summary>
        /// Gets the description of the range.
        /// </summary>
        public string RangeDescription
        {
            get
            {
                if (m_range == -1) return "Station";
                if (m_range == 0) return "Solar System";
                if (m_range == 1) return m_range.ToString() + " jump";
                if (m_range == EveConstants.RegionRange) return "Region";
                return m_range.ToString() + " jumps";
            }
        }

        /// <summary>
        /// Exports the given object to a serialization object.
        /// </summary>
        /// <returns></returns>
        public override SerializableOrderBase Export()
        {
            var serial = new SerializableBuyOrder();
            serial.Escrow = m_escrow;
            serial.Range = m_range;
            this.Export(serial);
            return serial;
        }
    }
    #endregion


    #region SellOrder
    /// <summary>
    /// This class represents a sell order.
    /// </summary>
    public sealed class SellOrder : MarketOrder
    {
        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="src"></param>
        internal SellOrder(SerializableAPIOrder src)
            : base(src)
        {
        }

        /// <summary>
        /// Constructor from an object deserialized from the settings file.
        /// </summary>
        /// <param name="src"></param>
        internal SellOrder(SerializableSellOrder src)
            : base(src)
        {
        }

        /// <summary>
        /// Exports the given object to a serialization object.
        /// </summary>
        /// <returns></returns>
        public override SerializableOrderBase Export()
        {
            var serial = new SerializableSellOrder();
            this.Export(serial);
            return serial;
        }
    }
    #endregion


    #region OrderState
    /// <summary>
    /// The status of a market order.
    /// </summary>
    /// <remarks>The integer value determines the sort order.</remarks>
    public enum OrderState
    {
        [Header("Active orders")]
        Active = 0,
        [Header("Canceled orders")]
        Canceled = 1,
        [Header("Expired orders")]
        Expired = 2,
        [Header("Fulfilled orders")]
        Fulfilled = 3,
        [Header("Modified orders")]
        Modified = 4
    }
    #endregion
}
