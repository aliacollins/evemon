using System.ComponentModel;
using EVEMon.Common.Attributes;

namespace EVEMon.Common.SettingsObjects
{
    public enum AssetColumn
    {
        None = -1,

        [Header("Item")]
        [Description("Item (Name)")]
        ItemName = 0,

        [Header("Quantity")]
        [Description("Quantity")]
        Quantity = 1,

        [Header("Volume (m³)")]
        [Description("Volume")]
        Volume = 2,

        [Header("Blueprint type")]
        [Description("Blueprint Type (Original or Copy)")]
        BlueprintType = 3,

        [Header("Group")]
        [Description("Item Group")]
        Group = 4,

        [Header("Category")]
        [Description("Item Category")]
        Category = 5,

        [Header("Container")]
        [Description("Container (that contains the item)")]
        Container = 6,

        [Header("Flag")]
        [Description("Item Flag")]
        Flag = 7,

        [Header("Location")]
        [Description("Location")]
        Location = 8,

        [Header("Region")]
        [Description("Location (Region)")]
        Region = 9,

        [Header("System")]
        [Description("Location (Solar System)")]
        SolarSystem = 10,

        [Header("Path")]
        [Description("Location (Path)")]
        FullLocation = 11,

        [Header("Jumps")]
        [Description("Jumps (away from last known location)")]
        Jumps = 12
    }
}