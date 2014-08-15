using System.ComponentModel;
using EVEMon.Common.Attributes;

namespace EVEMon.Common.SettingsObjects
{
    public enum WalletTransactionColumn
    {
        None = -1,

        [Header("Date")]
        [Description("Date")]
        Date = 0,

        [Header("Item")]
        [Description("Item (Name)")]
        ItemName = 1,

        [Header("Price")]
        [Description("Price (ISK)")]
        Price = 2,

        [Header("Quantity")]
        [Description("Quantity")]
        Quantity = 3,

        [Header("Credit")]
        [Description("Credit (ISK)")]
        Credit = 4,

        [Header("Client")]
        [Description("Client")]
        Client = 5,

        [Header("Location")]
        [Description("Location (Full)")]
        Location = 6,

        [Header("Region")]
        [Description("Location (Region)")]
        Region = 7,

        [Header("System")]
        [Description("Location (Solar System)")]
        SolarSystem = 8,

        [Header("Station")]
        [Description("Location (Station)")]
        Station = 9,

        [Header("Transaction For")]
        [Description("Transaction For")]
        TransactionFor = 10,
       
        [Header("Journal ID")]
        [Description("Journal ID (of transaction)")]
        JournalID = 11
}
}
