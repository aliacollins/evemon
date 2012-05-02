using System;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class WalletJournal
    {
        private readonly long m_taxReceiverID;
        private readonly int m_refTypeID;
        private string m_taxReceiver;
        private string m_refType;


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="WalletJournal"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        public WalletJournal(SerializableWalletJournalListItem src)
        {
            ID = src.ID;
            Date = src.Date;
            Amount = src.Amount;
            Balance = src.Balance;
            Reason = ParseReason(src.Reason);
            Issuer = src.OwnerName1;
            Recipient = src.OwnerName2;
            TaxAmount = src.TaxAmount;
            m_refTypeID = src.RefTypeID;
            m_taxReceiverID = src.TaxReceiverID;

            m_refType = EveRefType.GetRefTypeIDToName(src.RefTypeID);
            m_taxReceiver = GetTaxReceiver();
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets the ID.
        /// </summary>
        public long ID { get; private set; }

        /// <summary>
        /// Gets the date.
        /// </summary>
        public DateTime Date { get; private set; }

        /// <summary>
        /// Gets the amount.
        /// </summary>
        public decimal Amount { get; private set; }

        /// <summary>
        /// Gets the balance.
        /// </summary>
        public decimal Balance { get; private set; }

        /// <summary>
        /// Gets the reason.
        /// </summary>
        public string Reason { get; private set; }

        /// <summary>
        /// Gets the issuer.
        /// </summary>
        public string Issuer { get; private set; }

        /// <summary>
        /// Gets the recipient.
        /// </summary>
        public string Recipient { get; private set; }

        /// <summary>
        /// Gets the tax amount.
        /// </summary>
        public decimal TaxAmount { get; private set; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        public string Type
        {
            get
            {
                return m_refType == "Unknown"
                           ? m_refType = EveRefType.GetRefTypeIDToName(m_refTypeID)
                           : m_refType;
            }
        }

        /// <summary>
        /// Gets the tax receiver.
        /// </summary>
        public string TaxReceiver
        {
            get
            {
                return m_taxReceiver == "Unknown"
                           ? m_taxReceiver = GetTaxReceiver()
                           : m_taxReceiver;
            }
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Gets the tax receiver.
        /// </summary>
        /// <returns></returns>
        private string GetTaxReceiver()
        {
            return m_taxReceiverID == 0 ? String.Empty : EveIDToName.GetIDToName(m_taxReceiverID);
        }

        /// <summary>
        /// Parses the reason text.
        /// </summary>
        /// <param name="reasonText">The reason text.</param>
        /// <returns></returns>
        private string ParseReason(string reasonText)
        {
            // If RefType is of type "Bounty Prizes" return a generic message,
            // otherwise clean the header of a player entered text if it exists
            return m_refTypeID == 85 ? "Killing NPC entities" : reasonText.Replace("DESC: ", String.Empty);
        }

        #endregion
    }
}