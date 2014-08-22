using System.ComponentModel;
using EVEMon.Common.Attributes;

namespace EVEMon.Common.SettingsObjects
{
    public enum PlanetaryColoniesColumn
    {
        None = -1,

        [Header("Planet Type")]
        [Description("Planet Type")]
        PlanetTypeName = 0,

        [Header("Planet")]
        [Description("Planet Name")]
        PlanetName = 1,

        [Header("Location")]
        [Description("Location (Full)")]
        Location = 2,

        [Header("Region")]
        [Description("Location (Region)")]
        Region = 3,

        [Header("System")]
        [Description("Location (Solar System)")]
        SolarSystem = 4,

        [Header("Installations")]
        [Description("Number of Installations")]
        Installations = 5,

        [Header("Upgrade Level")]
        [Description("Upgrade Level")]
        UpgradeLevel = 6,

        [Header("Last Change")]
        [Description("Last Change")]
        LastUpdate = 7,

        //[Header("State")]
        //[Description("Installation State")]
        //State = 0,

        //[Header("TTC")]
        //[Description("Time To Completion (TTC)")]
        //TTC = 1,

        //[Header("Name")]
        //[Description("Installation Name")]
        //PinName = 2,

        //[Header("Install Date")]
        //[Description("Installed Time")]
        //InstallTime = 6,

        //[Header("End Date")]
        //[Description("Estimated End Time")]
        //EndTime = 7,
    }
}
