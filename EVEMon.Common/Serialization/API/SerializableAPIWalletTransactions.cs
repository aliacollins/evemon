using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableAPIWalletTransactions
    {
        private readonly Collection<SerializableWalletTransactionsListItem> m_walletTransactions;

        public SerializableAPIWalletTransactions()
        {
            m_walletTransactions = new Collection<SerializableWalletTransactionsListItem>();
        }

        [XmlArray("transactions")]
        [XmlArrayItem("transaction")]
        public Collection<SerializableWalletTransactionsListItem> WalletTransactions
        {
            get { return m_walletTransactions; }
        }

    }
}
