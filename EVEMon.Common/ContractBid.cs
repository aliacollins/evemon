using System;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common
{
    public sealed class ContractBid
    {
        private readonly CCPCharacter m_character;

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
            Bidder = EveIDToName.GetIDToName(src.BidderID);
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
            Bidder = src.Bidder;
            BidDate = src.BidDate;
            Amount = src.Amount;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public long ID { get; private set; }

        /// <summary>
        /// Gets or sets the contract ID.
        /// </summary>
        /// <value>The contract ID.</value>
        public long ContractID { get; private set; }

        /// <summary>
        /// Gets or sets the bidder.
        /// </summary>
        /// <value>The bidder.</value>
        public string Bidder { get; private set; }

        /// <summary>
        /// Gets or sets the bid date.
        /// </summary>
        /// <value>The bid date.</value>
        public DateTime BidDate { get; private set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        /// <value>The amount.</value>
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