using System;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class WalletJournal
    {
        private readonly long m_taxReceiverID;
        private string m_taxReceiver;


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="WalletJournal"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        public WalletJournal(SerializableWalletJournalListItem src)
        {
            ID = src.ID;
            Type = EveRefType.GetRefTypeIDToName(src.RefTypeID);
            Date = src.Date;
            Amount = src.Amount;
            Balance = src.Balance;
            Description = String.Empty;
            Issuer = src.OwnerName1;
            Recipient = src.OwnerName2;
            m_taxReceiverID = src.TaxReceiverID;
            m_taxReceiver = GetTaxReceiver();
            TaxAmount = src.TaxAmount;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets the ID.
        /// </summary>
        public long ID { get; private set; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        public string Type { get; private set; }

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
        /// Gets the description.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the issuer.
        /// </summary>
        public string Issuer { get; private set; }

        /// <summary>
        /// Gets the recipient.
        /// </summary>
        public string Recipient { get; private set; }

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

        /// <summary>
        /// Gets the tax amount.
        /// </summary>
        public decimal TaxAmount { get; private set; }

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

        #endregion
    }
}