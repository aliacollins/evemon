using System;
using System.Collections.Generic;
using System.Text;
using EVEMon.Common.Attributes;
using System.ComponentModel;

namespace EVEMon.Common.SettingsObjects
{
    public enum MarketOrderColumn
    {
        None = -1,

        [Header("Item")]
        [Description("Item")]
        Item = 0,

        [Header("Type")]
        [Description("Item Type")]
        ItemType = 1,

        [Header("Location")]
        [Description("Location (Full)")]
        Location = 2,

        [Header("Region")]
        [Description("Location (Region)")]
        Region = 3,

        [Header("System")]
        [Description("Location (Solar System)")]
        SolarSystem = 4,

        [Header("Station")]
        [Description("Location (Station)")]
        Station = 5,

        [Header("Unit Price")]
        [Description("Unit Price")]
        UnitaryPrice = 6,

        [Header("Total Price")]
        [Description("Total Price")]
        TotalPrice = 7,

        [Header("Issued")]
        [Description("Issue Date")]
        Issued = 8,

        [Header("Expires In")]
        [Description("Expires In")]
        Expiration = 9,

        [Header("Duration")]
        [Description("Duration")]
        Duration = 10,

        [Header("Quantity")]
        [Description("Quantity (Remaining / Initial)")]
        Volume = 11,

        [Header("Min")]
        [Description("Quantity (Minimum)")]
        MinimumVolume = 12,

        [Header("Rem")]
        [Description("Quantity (Remaining)")]
        RemainingVolume = 13,

        [Header("Initial")]
        [Description("Quantity (Initial)")]
        InitialVolume = 14,

        [Header("Last Change")]
        [Description("Last Order State Change")]
        LastStateChange = 15,

        [Header("Range")]
        [Description("Order Range")]
        OrderRange = 16,

        [Header("Escrow")]
        [Description("Escrow")]
        Escrow = 17

    }
}
