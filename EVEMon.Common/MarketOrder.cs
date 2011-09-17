using System;
using System.Collections.Generic;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common
{
    /// <summary>
    /// A base class for market orders.
    /// </summary>
    public abstract class MarketOrder
    {
        /// <summary>
        /// The maximum number of days after expiration. Beyond this limit, we do not import orders anymore.
        /// </summary>
        public const int MaxExpirationDays = 7;

        private OrderState m_state;

        private readonly long m_itemID;


        #region Constructors

        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="src"></param>
        protected MarketOrder(SerializableOrderListItem src)
        {
            m_state = GetState(src);
            ID = src.OrderID;
            m_itemID = src.ItemID;
            Item = StaticItems.GetItemByID(src.ItemID);
            Station = GetStationByID(src.StationID);
            UnitaryPrice = src.UnitaryPrice;
            InitialVolume = src.InitialVolume;
            RemainingVolume = src.RemainingVolume;
            LastStateChange = DateTime.UtcNow;
            MinVolume = src.MinVolume;
            Duration = src.Duration;
            Issued = src.Issued;
            IssuedFor = src.IssuedFor;
        }

        /// <summary>
        /// Constructor from an object deserialized from the settings file.
        /// </summary>
        /// <param name="src"></param>
        protected MarketOrder(SerializableOrderBase src)
        {
            Ignored = src.Ignored;
            ID = src.OrderID;
            m_state = src.State;
            m_itemID = GetItemID(src);
            Item = GetItem(src);
            Station = GetStationByID(src.StationID);
            UnitaryPrice = src.UnitaryPrice;
            InitialVolume = src.InitialVolume;
            RemainingVolume = src.RemainingVolume;
            LastStateChange = src.LastStateChange;
            MinVolume = src.MinVolume;
            Duration = src.Duration;
            Issued = src.Issued;
            IssuedFor = (src.IssuedFor == IssuedFor.None ? IssuedFor.Character : src.IssuedFor);
        }

        #endregion


        #region Importation, Exportation

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
            src.Ignored = Ignored;
            src.OrderID = ID;
            src.State = m_state;
            src.ItemID = m_itemID;
            src.Item = (Item != null ? Item.Name : "Unknown Item");
            src.StationID = Station.ID;
            src.UnitaryPrice = UnitaryPrice;
            src.InitialVolume = InitialVolume;
            src.RemainingVolume = RemainingVolume;
            src.LastStateChange = LastStateChange;
            src.MinVolume = MinVolume;
            src.Duration = Duration;
            src.Issued = Issued;
            src.IssuedFor = IssuedFor;
        }

        /// <summary>
        /// Try to update this order with a serialization object from the API.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="endedOrders"></param>
        /// <returns></returns>
        internal bool TryImport(SerializableOrderListItem src, List<MarketOrder> endedOrders)
        {
            // Note that, before a match is found, all orders have been marked for deletion : m_markedForDeletion == true

            // Checks whether ID is the same (IDs can be recycled ?)
            if (!MatchesWith(src))
                return false;

            // Prevent deletion
            MarkedForDeletion = false;

            // Update infos (if ID is the same it may have been modified either by the market 
            // or by the user [modify order] so we update the orders info that are changeable)
            if (IsModified(src))
            {
                // If it's a buying order, escrow may have changed
                if (src.IsBuyOrder != 0)
                    ((BuyOrder)this).Escrow = src.Escrow;

                UnitaryPrice = src.UnitaryPrice;
                RemainingVolume = src.RemainingVolume;
                Issued = src.Issued;
                m_state = OrderState.Modified;
            }
            else if (m_state == OrderState.Modified)
                m_state = OrderState.Active;

            // Update state
            OrderState state = GetState(src);
            if (m_state != OrderState.Modified && state != m_state) // it has either expired or fulfilled
            {
                m_state = state;
                LastStateChange = DateTime.UtcNow;

                // Should we notify it to the user ?
                if ((state == OrderState.Expired || state == OrderState.Fulfilled) && !Ignored)
                    endedOrders.Add(this);
            }

            return true;
        }

        #endregion


        #region Properties

        /// <summary>
        /// When true, the order will be deleted unless it was found on the API feed.
        /// </summary>
        internal bool MarkedForDeletion { get; set; }

        /// <summary>
        /// Gets or sets whether an expired order has been deleted by the user.
        /// </summary>
        public bool Ignored { get; set; }

        /// <summary>
        /// Gets the order state.
        /// </summary>
        public OrderState State
        {
            get
            {
                if (IsExpired && (m_state == OrderState.Active || m_state == OrderState.Modified))
                    return OrderState.Expired;

                return m_state;
            }
        }

        /// <summary>
        /// Gets the order ID.
        /// </summary>
        public long ID { get; private set; }

        /// <summary>
        /// Gets the item.
        /// </summary>
        public Item Item { get; private set; }

        /// <summary>
        /// Gets the station where this order is located.
        /// </summary>
        public Station Station { get; private set; }

        /// <summary>
        /// Gets the intial volume.
        /// </summary>
        public int InitialVolume { get; private set; }

        /// <summary>
        /// Gets the remaining volume.
        /// </summary>
        public int RemainingVolume { get; private set; }

        /// <summary>
        /// Gets the minimum sell/buy threshold volume.
        /// </summary>
        public int MinVolume { get; private set; }

        /// <summary>
        /// Gets the duration, in days, of the order.
        /// </summary>
        public int Duration { get; private set; }

        /// <summary>
        /// Gets the unitary price.
        /// </summary>
        public decimal UnitaryPrice { get; private set; }

        /// <summary>
        /// Gets the total price.
        /// </summary>
        public decimal TotalPrice
        {
            get { return UnitaryPrice * RemainingVolume; }
        }

        /// <summary>
        /// Gets the time (UTC) this order was expired.
        /// </summary>
        public DateTime Issued { get; private set; }

        /// <summary>
        /// Gets for which the order was issued.
        /// </summary>
        public IssuedFor IssuedFor { get; private set; }

        /// <summary>
        /// Gets the estimated expiration time.
        /// </summary>
        public DateTime Expiration
        {
            get { return Issued.AddDays(Duration); }
        }

        /// <summary>
        /// Gets the last state change.
        /// </summary>
        public DateTime LastStateChange { get; private set; }

        /// <summary>
        /// Gets true if order naturally expired because of its duration.
        /// </summary>
        public bool IsExpired
        {
            get { return Issued.AddDays(Duration) < DateTime.UtcNow; }
        }

        /// <summary>
        /// Gets true if the order is not fulfilled, canceled, expired, etc.
        /// </summary>
        public bool IsAvailable
        {
            get { return (m_state == OrderState.Active || m_state == OrderState.Modified) && !IsExpired; }
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Gets an items ID either by source or by name.
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        private static long GetItemID(SerializableOrderBase src)
        {
            // Try get item ID by source
            long itemID = src.ItemID;

            // We failed? Try get item ID by name
            if (itemID == 0)
            {
                Item item = StaticItems.GetItemByName(src.Item);
                itemID = (item == null ? 0 : item.ID);
            }

            return itemID;
        }

        /// <summary>
        /// Gets an item by its ID or its name.
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        private static Item GetItem(SerializableOrderBase src)
        {
            // Try get item by its ID, if we faile try get it by its name
            Item item = StaticItems.GetItemByID(src.ItemID) ?? StaticItems.GetItemByName(src.Item);
            return item;
        }

        /// <summary>
        /// Gets the station of an order.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static Station GetStationByID(long id)
        {
            // Look for the station in datafile, if we fail then it may be a conquerable outpost station
            Station station = StaticGeography.GetStationByID(id) ?? ConquerableStation.GetStationByID(id);

            // We failed again ? It's not in any data we can access
            // We set it to a fixed one and notify about it in the trace file
            if (station == null)
            {
                station = StaticGeography.GetStationByID(60013747);
                EveMonClient.Trace("Could not find station id {0}", id);
                EveMonClient.Trace("Setting to {0}", station.Name);
            }

            return station;
        }

        /// <summary>
        /// Gets the state of an order.
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        private static OrderState GetState(SerializableOrderListItem src)
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
                    return (src.RemainingVolume == 0 ? OrderState.Fulfilled : OrderState.Expired);

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Checks whether the given API object matches with this order.
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        private bool MatchesWith(SerializableOrderListItem src)
        {
            return src.OrderID == ID;
        }

        /// <summary>
        /// Checks whether the given API object matches with this order.
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        private bool IsModified(SerializableOrderListItem src)
        {
            return src.RemainingVolume != 0
                   && ((src.UnitaryPrice != UnitaryPrice && src.Issued != Issued)
                       || src.RemainingVolume != RemainingVolume);
        }

        /// <summary>
        /// Formats the given price to a readable string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string Format(decimal value, AbbreviationFormat format)
        {
            decimal abs = Math.Abs(value);
            if (format == AbbreviationFormat.AbbreviationWords)
            {
                if (abs >= 1E9M)
                    return Format("Billions", value / 1E9M);
                if (abs >= 1E6M)
                    return Format("Millions", value / 1E6M);

                return abs >= 1E3M ? Format("Thousands", value / 1E3M) : Format("", value);
            }

            if (abs >= 1E9M)
                return Format("B", value / 1E9M);
            if (abs >= 1E6M)
                return Format("M", value / 1E6M);

            return abs >= 1E3M ? Format("K", value / 1E3M) : Format("", value);
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

            decimal abs = Math.Abs(value);
            if (abs < 1.0M)
                return (((int)value * 100) / 100.0M).ToString("0.##") + suffix;
            if (abs < 10.0M)
                return (((int)value * 1000) / 1000.0M).ToString("#.##") + suffix;
            if (abs < 100.0M)
                return (((int)value * 1000) / 1000.0M).ToString("##.#") + suffix;

            return (((int)value * 1000) / 1000.0M).ToString("###") + suffix;
        }

        #endregion
    }
}