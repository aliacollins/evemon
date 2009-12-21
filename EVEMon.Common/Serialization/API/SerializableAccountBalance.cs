using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    /// <summary>
    /// A collection of wallets balances.
    /// </summary>
    public sealed class SerializableAccountBalanceList
    {
        /// <summary>
        /// Gets the list of balance accounts for every account on this account (one for character balance, one per division for corporations).
        /// </summary>
        [XmlArray("accounts")]
        [XmlArrayItem("account")]
        public SerializableAccountBalance[] Accounts
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Represents the balance of a wallet
    /// </summary>
    public sealed class SerializableAccountBalance
    {
        [XmlAttribute("accountID")]
        public long AccountID
        {
            get;
            set;
        }

        [XmlAttribute("accountKey")]
        public long AccountKey
        {
            get;
            set;
        }

        [XmlAttribute("balance")]
        public decimal Balance
        {
            get;
            set;
        }
    }
}
