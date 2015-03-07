using System;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common
{
    public sealed class ContractBid
    {
        private readonly long m_bidderId;
        private string m_bidder;


        #region Constructor

        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="src">The source.</param>
        internal ContractBid(SerializableContractBidsListItem src)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            ID = src.ID;
            ContractID = src.ContractID;
            m_bidderId = src.BidderID;
            m_bidder = EveIDToName.GetIDToName(src.BidderID);
            BidDate = src.DateBid;
            Amount = src.Amount;
        }

        /// <summary>
        /// Constructor from an object deserialized from the settings file.
        /// </summary>
        /// <param name="src">The source.</param>
        internal ContractBid(SerializableContractBid src)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            ID = src.BidID;
            ContractID = src.ContractID;
            m_bidder = src.Bidder;
            BidDate = src.BidDate;
            Amount = src.Amount;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets the ID.
        /// </summary>
        public long ID { get; private set; }

        /// <summary>
        /// Gets the contract ID.
        /// </summary>
        public long ContractID { get; private set; }

        /// <summary>
        /// Gets the bidder.
        /// </summary>
        public string Bidder
        {
            get
            {
                return m_bidder == "Unknown"
                           ? m_bidder = EveIDToName.GetIDToName(m_bidderId)
                           : m_bidder;
            }
        }

        /// <summary>
        /// Gets the bid date.
        /// </summary>
        public DateTime BidDate { get; private set; }

        /// <summary>
        /// Gets the amount.
        /// </summary>
        public decimal Amount { get; private set; }

        #endregion


        #region Exportation

        /// <summary>
        /// Exports the given object to a serialization object.
        /// </summary>
        /// <returns></returns>
        internal SerializableContractBid Export()
        {
            return new SerializableContractBid
                       {
                           BidID = ID,
                           ContractID = ContractID,
                           Bidder = Bidder,
                           BidDate = BidDate,
                           Amount = Amount
                       };
        }

        #endregion
    }
}