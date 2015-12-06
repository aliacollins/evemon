using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Eve
{
    /// <summary>
    /// A collection of wallets balances.
    /// </summary>
    public sealed class SerializableAPIAccountBalance
    {
        private readonly Collection<SerializableAccountBalanceListItem> m_accounts;

        public SerializableAPIAccountBalance()
        {
            m_accounts = new Collection<SerializableAccountBalanceListItem>();
        }

        /// <summary>
        /// Gets the list of balance accounts for every account on this API key (one for character balance, one per division for corporations).
        /// </summary>
        [XmlArray("accounts")]
        [XmlArrayItem("account")]
        public Collection<SerializableAccountBalanceListItem> Accounts
        {
            get { return m_accounts; }
        }
    }
}