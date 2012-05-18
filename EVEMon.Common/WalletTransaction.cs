using System;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class WalletTransaction
    {
        private int m_stationID;


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="WalletTransaction"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        public WalletTransaction(SerializableWalletTransactionsListItem src)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            ID = src.ID;
            JournalID = src.JournalTransactionID;
            Date = src.TransactionDate;
            ItemName = src.TypeName;
            Quantity = src.Quantity;
            Price = src.Price;
            ClientName = src.ClientName;
            TransactionType = src.TransactionType == "buy" ? TransactionType.Buy : TransactionType.Sell;
            TransactionFor = src.TransactionFor == "personal" ? IssuedFor.Character : IssuedFor.Corporation;
            m_stationID = src.StationID;
            UpdateStation();

            Credit = GetCredit();
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets the ID.
        /// </summary>
        public long ID { get; private set; }

        /// <summary>
        /// Gets the journal ID.
        /// </summary>
        public long JournalID { get; private set; }

        /// <summary>
        /// Gets the date.
        /// </summary>
        public DateTime Date { get; private set; }

        /// <summary>
        /// Gets the name of the item.
        /// </summary>
        /// <value>
        /// The name of the item.
        /// </value>
        public string ItemName { get; private set; }

        /// <summary>
        /// Gets the quantity.
        /// </summary>
        public long Quantity { get; private set; }

        /// <summary>
        /// Gets the price.
        /// </summary>
        public decimal Price { get; private set; }

        /// <summary>
        /// Gets the name of the client.
        /// </summary>
        /// <value>
        /// The name of the client.
        /// </value>
        public string ClientName { get; private set; }

        /// <summary>
        /// Gets the station.
        /// </summary>
        public Station Station { get; private set; }

        /// <summary>
        /// Gets the type of the transaction.
        /// </summary>
        /// <value>
        /// The type of the transaction.
        /// </value>
        public TransactionType TransactionType { get; private set; }

        /// <summary>
        /// Gets the transaction for.
        /// </summary>
        public IssuedFor TransactionFor { get; private set; }

        /// <summary>
        /// Gets the credit.
        /// </summary>
        public decimal Credit { get; private set; }

        #endregion


        #region Helper Methods

        private decimal GetCredit()
        {

            decimal credit = Quantity * Price;
            return TransactionType == TransactionType.Buy ? -credit : credit;
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Updates the station.
        /// </summary>
        public void UpdateStation()
        {
            Station = Station.GetByID(m_stationID);
        }

        #endregion
    }
}