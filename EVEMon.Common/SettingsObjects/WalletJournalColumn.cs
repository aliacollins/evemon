using System.ComponentModel;
using EVEMon.Common.Attributes;

namespace EVEMon.Common.SettingsObjects
{
    public enum WalletJournalColumn
    {
        None = -1,

        [Header("Date")]
        [Description("Date")]
        Date = 0,

        [Header("Type")]
        [Description("Type (of transaction)")]
        Type = 1,

        [Header("Amount")]
        [Description("Amount")]
        Amount = 2,

        [Header("Balance")]
        [Description("Balance")]
        Balance = 3,

        [Header("Description")]
        [Description("Description")]
        Description = 4,

        [Header("From")]
        [Description("From (Issuer)")]
        Issuer = 5,

        [Header("To")]
        [Description("To (Recipient)")]
        Recipient = 6,

        [Header("Tax Receiver")]
        [Description("Tax Receiver")]
        TaxReceiver = 7,

        [Header("Tax Amount")]
        [Description("Tax Amount")]
        TaxAmount = 8,

        [Header("ID")]
        [Description("ID (of transaction)")]
        ID = 9
    }
}
