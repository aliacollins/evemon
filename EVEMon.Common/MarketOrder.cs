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

        private readonly int m_itemID;


        #region Constructors

        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="src"></param>
        protected MarketOrder(SerializableOrderListItem src)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            m_state = GetState(src);
            OwnerID = src.OwnerID;
            ID = src.OrderID;
            m_itemID = src.ItemID;
            Item = StaticItems.GetItemByID(src.ItemID);
            Station = Station.GetByID(src.StationID);
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
            if (src == null)
                throw new ArgumentNullException("src");

            Ignored = src.Ignored;
            ID = src.OrderID;
            m_state = src.State;
            m_itemID = GetItemID(src);
            Item = GetItem(src);
            Station = Station.GetByID(src.StationID);
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
        /// Gets or sets the owner ID.
        /// </summary>
        /// <value>The owner ID.</value>
        public long OwnerID { get; set; }

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
            get { return Expiration < DateTime.UtcNow; }
        }

        /// <summary>
        /// Gets true if the order is not fulfilled, canceled, expired, etc.
        /// </summary>
        public bool IsAvailable
        {
            get { return (m_state == OrderState.Active || m_state == OrderState.Modified) && !IsExpired; }
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
        protected SerializableOrderBase Export(SerializableOrderBase src)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            src.Ignored = Ignored;
            src.OrderID = ID;
            src.State = m_state;
            src.ItemID = m_itemID;
            src.Item = (Item != null ? Item.Name : "Unknown Item");
            src.StationID = (Station != null ? Station.ID : 0);
            src.UnitaryPrice = UnitaryPrice;
            src.InitialVolume = InitialVolume;
            src.RemainingVolume = RemainingVolume;
            src.LastStateChange = LastStateChange;
            src.MinVolume = MinVolume;
            src.Duration = Duration;
            src.Issued = Issued;
            src.IssuedFor = IssuedFor;

            return src;
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
                LastStateChange = DateTime.UtcNow;
            }
            else if (m_state == OrderState.Modified)
            {
                m_state = OrderState.Active;
                LastStateChange = DateTime.UtcNow;
            }

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


        #region Helper Methods

        /// <summary>
        /// Gets an items ID either by source or by name.
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        private static int GetItemID(SerializableOrderBase src)
        {
            // Try get item ID by source
            int itemID = src.ItemID;

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
            // Try get item by its ID, if we fail try get it by its name
            return StaticItems.GetItemByID(src.ItemID) ?? StaticItems.GetItemByName(src.Item);
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

        #endregion
    }
}