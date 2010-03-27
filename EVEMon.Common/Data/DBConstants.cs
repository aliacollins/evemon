using System;
using System.Collections.Generic;
using System.Text;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Provides constants about the CCP databases.
    /// </summary>
    public static class DBConstants
    {
        public static readonly int[] RequiredSkillLevelPropertyIDs = new int[] { 277, 278, 279, 1286, 1287, 1288 };
        public static readonly int[] RequiredSkillPropertyIDs = new int[] { 182, 183, 184, 1285, 1289, 1290 };
        public static readonly int[] LauncherGroupIDs = new int[] { 137, 602, 603 };
        public static readonly int[] ChargeGroupIDs = new int[] { 604, 605, 606, 609, 610 };
        public static readonly int[] CanFitShipGroupIDs = new int[] { 1298, 1299, 1300, 1301 };
        public static readonly int[] ModuleShipGroupIDs = new int[] { 666, 667, 668 };
        public static readonly int[] SpecialisationAsteroidGroupIDs = new int[] { 781 };
        public static readonly int[] ReactionGroupIDs = new int[] { 842, 843 };
        public static readonly int[] PosCargobayAcceptGroupIDs = new int[] { 1352 };

        public const int SubsystemsCategoryID = 989;
        public const int MetaLevelPropertyID = 633;
        public const int CargoCapacityPropertyID = 38;
        public const int VolumePropertyID = 161;
        public const int MassPropertyID = 4;

        public const int LowSlotEffectID = 11;
        public const int MedSlotEffectID = 13;
        public const int HiSlotEffectID = 12;

        public const int ImplantSlotPropertyID = 331;
        public const int CharismaModifierPropertyID = 175;
        public const int IntelligenceModifierPropertyID = 176;
        public const int MemoryModifierPropertyID = 177;
        public const int PerceptionModifierPropertyID = 178;
        public const int WillpowerModifierPropertyID = 179;

        public const int CorporationManagementSkillsGroupID = 266;
        public const int LearningSkillsGroupID = 267;
        public const int SocialSkillsGroupID = 278;
        public const int TradeSkillsGroupID = 274;

        public const int SkillGroupID = 150;
        public const int ShipsGroupID = 4;
        public const int DronesGroupID = 157;
        public const int ShipEquipmentGroupID = 9;
        public const int AmmosAndChargesGroupID = 11;
        public const int ImplantsAndBoostersGroupID = 24;
        public const int StarbaseStructuresGroupID = 477;
        public const int ShipModificationsGroupID = 955;

        public const int MiningBargesGroupID = 495;
        public const int ExhumersGroupID = 875;
        public const int CapitalIndustrialsGroupID = 1048;

        public const int ShipBonusPirateFactionPropertyID = 793;

        public const int WarpSpeedMultiplierPropertyID = 600;
        public const int ShipWarpSpeedPropertyID = 1281;

        public const int CPUOutputPropertyID = 48;
        public const int PGOutputPropertyID = 11;

        public const int CPUNeedPropertyID = 50;
        public const int PGNeedPropertyID = 30;

        public const int ReprocessingSkillPropertyID = 790;
        public const int ScrapMetalProcessingSkillID = 12196;
    }
}
