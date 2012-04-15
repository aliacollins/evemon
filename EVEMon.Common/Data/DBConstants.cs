using System.Collections.ObjectModel;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Provides constants about the CCP databases.
    /// </summary>
    public static class DBConstants
    {
        #region Collections

        // Group of PropertyIDs
        public static readonly ReadOnlyCollection<int> RequiredSkillPropertyIDs =
            new ReadOnlyCollection<int>(new[]
                                            {
                                                RequiredSkill1PropertyID, RequiredSkill2PropertyID,
                                                RequiredSkill3PropertyID, RequiredSkill4PropertyID,
                                                RequiredSkill5PropertyID, RequiredSkill6PropertyID
                                            });

        public static readonly ReadOnlyCollection<int> RequiredSkillLevelPropertyIDs =
            new ReadOnlyCollection<int>(new[]
                                            {
                                                RequiredSkill1LevelPropertyID, RequiredSkill2LevelPropertyID,
                                                RequiredSkill3LevelPropertyID, RequiredSkill4LevelPropertyID,
                                                RequiredSkill5LevelPropertyID, RequiredSkill6LevelPropertyID
                                            });

        public static readonly ReadOnlyCollection<int> AlwaysVisibleForShipPropertyIDs =
            new ReadOnlyCollection<int>(new[]
                                            {
                                                CPUOutputPropertyID, PGOutputPropertyID, UpgradeCapacityPropertyID,
                                                HiSlotsPropertyID, MedSlotsPropertyID, LowSlotsPropertyID,
                                                DroneCapacityPropertyID, DroneBandwidthPropertyID, CargoCapacityPropertyID,
                                                MassPropertyID, VolumePropertyID, CapacitorCapacityPropertyID,
                                                CapacitorRechargeRatePropertyID, MaxTargetRangePropertyID,
                                                ScanResolutionPropertyID, SignatureRadiusPropertyID, MaxVelocityPropertyID,
                                                ShipWarpSpeedPropertyID, StructureHitpointsPropertyID, ShieldHitpointsPropertyID,
                                                ArmorHitpointsPropertyID, ShieldRechargeRatePropertyID,
                                                ShieldEMResistancePropertyID, ShieldExplosiveResistancePropertyID,
                                                ShieldKineticResistancePropertyID, ShieldThermalResistancePropertyID,
                                                ArmorEMResistancePropertyID, ArmorExplosiveResistancePropertyID,
                                                ArmorKineticResistancePropertyID, ArmorThermalResistancePropertyID
                                            });

        public static readonly ReadOnlyCollection<int> HideIfDefaultPropertyIDs =
            new ReadOnlyCollection<int>(new[]
                                            {
                                                LauncherSlotsLeftPropertyID, TurretSlotsLeftPropertyID,
                                                TurretHardPointModifierPropertyID, LauncherHardPointModifierPropertyID,
                                                HiSlotModifierPropertyID, MedSlotModifierPropertyID,
                                                LowSlotModifierPropertyID, ScanRadarStrengthPropertyID,
                                                ScanLadarStrengthPropertyID, ScanMagnetometricStrengthPropertyID,
                                                ScanGravimetricStrengthPropertyID, HullEMResistancePropertyID,
                                                HullExplosiveResistancePropertyID, HullKineticResistancePropertyID,
                                                HullThermalResistancePropertyID, EmDamagePropertyID, ExplosiveDamagePropertyID,
                                                KineticDamagePropertyID, ThermalDamagePropertyID,
                                                CharismaModifierPropertyID, IntelligenceModifierPropertyID,
                                                MemoryModifierPropertyID, PerceptionModifierPropertyID,
                                                WillpowerModifierPropertyID, MetaLevelPropertyID
                                            });

        public static readonly ReadOnlyCollection<int> LauncherGroupPropertyIDs =
            new ReadOnlyCollection<int>(new[]
                                            {
                                                LauncherGroupPropertyID, LauncherGroup2PropertyID, LauncherGroup3PropertyID
                                            });

        public static readonly ReadOnlyCollection<int> ChargeGroupPropertyIDs =
            new ReadOnlyCollection<int>(new[]
                                            {
                                                ChargeGroup1PropertyID, ChargeGroup2PropertyID, ChargeGroup3PropertyID,
                                                ChargeGroup4PropertyID, ChargeGroup5PropertyID
                                            });

        public static readonly ReadOnlyCollection<int> CanFitShipGroupPropertyIDs =
            new ReadOnlyCollection<int>(new[]
                                            {
                                                CanFitShipGroup1PropertyID, CanFitShipGroup2PropertyID,
                                                CanFitShipGroup3PropertyID, CanFitShipGroup4PropertyID
                                            });

        public static readonly ReadOnlyCollection<int> ModuleShipGroupPropertyIDs =
            new ReadOnlyCollection<int>(new[]
                                            {
                                                ModuleShipGroup1PropertyID, ModuleShipGroup2PropertyID,
                                                ModuleShipGroup3PropertyID
                                            });

        public static readonly ReadOnlyCollection<int> ReactionGroupPropertyIDs =
            new ReadOnlyCollection<int>(new[] { ReactionGroup1PropertyID, ReactionGroup2PropertyID });

        public static readonly ReadOnlyCollection<int> IndustryModifyingPropertyIDs =
            new ReadOnlyCollection<int>(new[]
                                            {
                                                ManufacturingTimeBonusPropertyID, ManufactureCostBonusPropertyID,
                                                CopySpeedBonusPropertyID, BlueprintManufactureTimeBonusPropertyID,
                                                MineralNeedResearchBonusPropertyID
                                            });

        public static readonly ReadOnlyCollection<int> SpecialisationAsteroidGroupPropertyIDs =
            new ReadOnlyCollection<int>(new[] { SpecialisationAsteroidGroupPropertyID });

        public static readonly ReadOnlyCollection<int> PosCargobayAcceptGroupPropertyIDs =
            new ReadOnlyCollection<int>(new[] { PosCargobayAcceptGroupPropertyID });

        // Group of MarketGroupIDs
        public static readonly ReadOnlyCollection<int> StrategicComponentsMarketGroupIDs =
            new ReadOnlyCollection<int>(new[] { SubsystemsMarketGroupID, StrategicCruisersMarketGroupID });

        public static readonly ReadOnlyCollection<int> SmallToXLargeShipsMarketGroupIDs =
            new ReadOnlyCollection<int>(new[]
                                            {
                                                StandardFrigatesMarketGroupID, StandardCruisersMarketGroupID,
                                                StandardBattleshipsMarketGroupID, StandardIndustrialShipsMarketGroupID,
                                                ShuttlessMarketGroupID, StandardDestroyersMarketGroupID,
                                                StandardBattlecruisersMarketGroupID, MiningBargesMarketGroupID,
                                                DreadnoughtsMarketGroupID, FreightersMarketGroupID, CarriersMarketGroupID,
                                                FightersMarketGroupID, CapitalIndustrialShipsMarketGroupID,
                                                FighterBombersMarketGroupID
                                            });

        public static readonly ReadOnlyCollection<int> CapitalShipsMarketGroupIDs =
            new ReadOnlyCollection<int>(new[]
                                            {
                                                DreadnoughtsMarketGroupID, FreightersMarketGroupID,
                                                TitansMarketGroupID, CarriersMarketGroupID
                                            });

        public static readonly ReadOnlyCollection<int> AdvancedSmallToLargeShipsMarketGroupIDs =
            new ReadOnlyCollection<int>(new[]
                                            {
                                                InterceptorsMarketGroupID, CovertOpsMarketGroupID, AssaultShipsMarketGroupID,
                                                LogisticsMarketGroupID, HeavyAssaultShipsMarketGroupID,
                                                TransportShipsMarketGroupID, CommandShipsMarketGroupID, InterdictorsMarketGroupID
                                                ,
                                                ReconShipsMarketGroupID, ExhumersMarketGroupID,
                                                ElectronicAttackFrigatesMarketGroupID, HeavyInterdictorsMarketGroupID,
                                                BlackOpsMarketGroupID, MaraudersMarketGroupID, JumpFreightersMarketGroupID
                                            });

        // Group of Implants IDs
        public static readonly ReadOnlyCollection<int> ManufacturingModifyingImplantIDs =
            new ReadOnlyCollection<int>(new[]
                                            {
                                                ZainouBeancounterF40ID, ZainouBeancounterF50ID, ZainouBeancounterF60ID
                                            });

        public static readonly ReadOnlyCollection<int> ResearchMaterialTimeModifyingImplantIDs =
            new ReadOnlyCollection<int>(new[]
                                            {
                                                ZainouBeancounterJ40ID, ZainouBeancounterJ50ID, ZainouBeancounterJ60ID
                                            });

        public static readonly ReadOnlyCollection<int> ResearchCopyTimeModifyingImplantIDs =
            new ReadOnlyCollection<int>(new[]
                                            {
                                                ZainouBeancounterK40ID, ZainouBeancounterK50ID, ZainouBeancounterK60ID
                                            });

        public static readonly ReadOnlyCollection<int> ResearchProductivityTimeModifyingImplantIDs =
            new ReadOnlyCollection<int>(new[]
                                            {
                                                ZainouBeancounterI40ID, ZainouBeancounterI50ID, ZainouBeancounterI60ID
                                            });

        public static readonly ReadOnlyCollection<int> MaterialQuantityModifyingImplantIDs =
            new ReadOnlyCollection<int>(new[]
                                            {
                                                ZainouBeancounterG40ID, ZainouBeancounterG50ID, ZainouBeancounterG60ID
                                            });

        #endregion


        #region Effect IDs

        public const int LowSlotEffectID = 11;
        public const int HiSlotEffectID = 12;
        public const int MedSlotEffectID = 13;

        #endregion


        #region Attribute Category IDs

        public const int FittingAtributeCategoryID = 1;
        public const int StructureAtributeCategoryID = 4;
        public const int MiscellaneousAttributeCategoryID = 7;
        public const int NULLAtributeCategoryID = 9;

        #endregion


        #region Attribute types IDs (Properties)

        public const int MassPropertyID = 4;
        public const int CapacitorNeedPropertyID = 6;
        public const int StructureHitpointsPropertyID = 9;
        public const int PGOutputPropertyID = 11;
        public const int LowSlotsPropertyID = 12;
        public const int MedSlotsPropertyID = 13;
        public const int HiSlotsPropertyID = 14;
        public const int PGNeedPropertyID = 30;
        public const int MaxVelocityPropertyID = 37;
        public const int CargoCapacityPropertyID = 38;
        public const int CPUOutputPropertyID = 48;
        public const int CPUNeedPropertyID = 50;
        public const int CapacitorRechargeRatePropertyID = 55;
        public const int ShieldBonusPropertyID = 68;
        public const int AgilityPropertyID = 70;
        public const int DurationPropertyID = 73;
        public const int MaxTargetRangePropertyID = 76;
        public const int ShieldTransferRangePropertyID = 87;
        public const int LauncherSlotsLeftPropertyID = 101;
        public const int TurretSlotsLeftPropertyID = 102;
        public const int EmDamagePropertyID = 114;
        public const int ExplosiveDamagePropertyID = 116;
        public const int KineticDamagePropertyID = 117;
        public const int ThermalDamagePropertyID = 118;
        public const int LauncherGroupPropertyID = 137;
        public const int CapacitorRechargeRateMultiplierPropertyID = 144;
        public const int VolumePropertyID = 161;
        public const int CharismaPropertyID = 164;
        public const int IntelligencePropertyID = 165;
        public const int MemoryPropertyID = 166;
        public const int PerceptionPropertyID = 167;
        public const int WillpowerPropertyID = 168;
        public const int CharismaModifierPropertyID = 175;
        public const int IntelligenceModifierPropertyID = 176;
        public const int MemoryModifierPropertyID = 177;
        public const int PerceptionModifierPropertyID = 178;
        public const int WillpowerModifierPropertyID = 179;
        public const int PrimaryAttributePropertyID = 180;
        public const int SecondaryAttributePropertyID = 181;
        public const int RequiredSkill1PropertyID = 182;
        public const int RequiredSkill2PropertyID = 183;
        public const int RequiredSkill3PropertyID = 184;
        public const int ScanRadarStrengthPropertyID = 208;
        public const int ScanLadarStrengthPropertyID = 209;
        public const int ScanMagnetometricStrengthPropertyID = 210;
        public const int ScanGravimetricStrengthPropertyID = 211;
        public const int ShieldHitpointsPropertyID = 263;
        public const int ArmorHitpointsPropertyID = 265;
        public const int ArmorEMResistancePropertyID = 267;
        public const int ArmorExplosiveResistancePropertyID = 268;
        public const int ArmorKineticResistancePropertyID = 269;
        public const int ArmorThermalResistancePropertyID = 270;
        public const int ShieldEMResistancePropertyID = 271;
        public const int ShieldExplosiveResistancePropertyID = 272;
        public const int ShieldKineticResistancePropertyID = 273;
        public const int ShieldThermalResistancePropertyID = 274;
        public const int SkillTimeConstantPropertyID = 275;
        public const int RequiredSkill1LevelPropertyID = 277;
        public const int RequiredSkill2LevelPropertyID = 278;
        public const int RequiredSkill3LevelPropertyID = 279;
        public const int DroneCapacityPropertyID = 283;
        public const int ImplantSlotPropertyID = 331;
        public const int TechLevelPropertyID = 422;
        public const int CPUOutputBonusPropertyID = 424;
        public const int ManufacturingTimeBonusPropertyID = 440;
        public const int ManufactureCostBonusPropertyID = 451;
        public const int CopySpeedBonusPropertyID = 452;
        public const int BlueprintManufactureTimeBonusPropertyID = 453;
        public const int MineralNeedResearchBonusPropertyID = 468;
        public const int ShieldRechargeRatePropertyID = 479;
        public const int CapacitorCapacityPropertyID = 482;
        public const int SignatureRadiusPropertyID = 552;
        public const int AnchoringDelayPropertyID = 556;
        public const int CloakingTargetingDelayPropertyID = 560;
        public const int ScanResolutionPropertyID = 564;
        public const int WarpSpeedMultiplierPropertyID = 600;
        public const int LauncherGroup2PropertyID = 602;
        public const int LauncherGroup3PropertyID = 603;
        public const int ChargeGroup1PropertyID = 604;
        public const int ChargeGroup2PropertyID = 605;
        public const int ChargeGroup3PropertyID = 606;
        public const int ChargeGroup4PropertyID = 609;
        public const int ChargeGroup5PropertyID = 610;
        public const int MetaLevelPropertyID = 633;
        public const int ModuleShipGroup2PropertyID = 666;
        public const int ModuleShipGroup3PropertyID = 667;
        public const int ModuleShipGroup1PropertyID = 668;
        public const int ModuleReactivationDelayPropertyID = 669;
        public const int UnanchoringDelayPropertyID = 676;
        public const int OnliningDelayPropertyID = 677;
        public const int IceHarvestCycleBonusPropertyID = 780;
        public const int SpecialisationAsteroidGroupPropertyID = 781;
        public const int ReprocessingSkillPropertyID = 790;
        public const int ShipBonusPirateFactionPropertyID = 793;
        public const int ReactionGroup1PropertyID = 842;
        public const int ReactionGroup2PropertyID = 843;
        public const int ShipMaintenanceBayCapacityPropertyID = 908;
        public const int HullEMResistancePropertyID = 974;
        public const int HullExplosiveResistancePropertyID = 975;
        public const int HullKineticResistancePropertyID = 976;
        public const int HullThermalResistancePropertyID = 977;
        public const int CanNotBeTrainedOnTrialPropertyID = 1047;
        public const int CPUPenaltyPercentPropertyID = 1082;
        public const int UpgradeCapacityPropertyID = 1132;
        public const int RigSlotsPropertyID = 1137;
        public const int UpgradeCostPropertyID = 1153;
        public const int DroneBandwidthPropertyID = 1271;
        public const int DroneBandwidthUsedPropertyID = 1272;
        public const int ShipWarpSpeedPropertyID = 1281;
        public const int RequiredSkill4PropertyID = 1285;
        public const int RequiredSkill4LevelPropertyID = 1286;
        public const int RequiredSkill5LevelPropertyID = 1287;
        public const int RequiredSkill6LevelPropertyID = 1288;
        public const int RequiredSkill5PropertyID = 1289;
        public const int RequiredSkill6PropertyID = 1290;
        public const int CanFitShipGroup1PropertyID = 1298;
        public const int CanFitShipGroup2PropertyID = 1299;
        public const int CanFitShipGroup3PropertyID = 1300;
        public const int CanFitShipGroup4PropertyID = 1301;
        public const int PosCargobayAcceptGroupPropertyID = 1352;
        public const int MaxSubSystemsPropertyID = 1367;
        public const int TurretHardPointModifierPropertyID = 1368;
        public const int LauncherHardPointModifierPropertyID = 1369;
        public const int HiSlotModifierPropertyID = 1374;
        public const int MedSlotModifierPropertyID = 1375;
        public const int LowSlotModifierPropertyID = 1376;
        public const int FitsToShipTypePropertyID = 1380;
        public const int AITargetSwitchTimerPropertyID = 1416;
        public const int RigSizePropertyID = 1547;
        public const int MetaGroupPropertyID = 1692;

        #endregion


        #region EVE Unit IDs (Properties UnitID's)

        public const int MassUnitID = 2;
        public const int MillsecondsUnitID = 101;
        public const int AbsolutePercentUnitID = 127;
        public const int InverseAbsolutePercentUnitID = 108;
        public const int ModifierPercentUnitID = 109;
        public const int InversedModifierPercentUnitID = 111;
        public const int GroupIDUnitID = 115;
        public const int TypeUnitID = 116;
        public const int SizeclassUnitID = 117;

        #endregion


        #region Certificate Classes IDs

        public const int IndustrialHarvestingID = 104;
        public const int AutomatedMiningID = 106;
        public const int ProductionInternID = 111;

        #endregion


        #region Certificate Grade IDs

        public const int BasicID = 1;
        public const int StandardID = 2;
        public const int ImprovedID = 3;
        public const int EliteID = 5;

        #endregion


        #region Category IDs

        public const int BlueprintCategoryID = 9;
        public const int ImplantCategoryID = 20;
        public const int SkillCategoryID = 16;

        #endregion


        #region Group IDs

        public const int FrigateGroupID = 25;
        public const int CruiserGroupID = 26;
        public const int BattleshipGroupID = 27;
        public const int IndustrialGroupID = 28;
        public const int TitanGroupID = 30;
        public const int RookieShipGroupID = 237;
        public const int TradeSkillsGroupID = 274;
        public const int SocialSkillsGroupID = 278;
        public const int AssaultShipGroupID = 324;
        public const int HeavyAssaultShipGroupID = 358;
        public const int TransportShipGroupID = 380;
        public const int EliteBattleshipGroupID = 381;
        public const int BattlecruiserGroupID = 419;
        public const int DestroyerGroupID = 420;
        public const int MiningBargeGroupID = 463;
        public const int DreadnoughtGroupID = 485;
        public const int FakeSkillsGroupID = 505;
        public const int FreighterGroupID = 513;
        public const int CommandShipGroupID = 540;
        public const int InterdictorGroupID = 541;
        public const int ExhumerGroupID = 543;
        public const int CarrierGroupID = 547;
        public const int SupercarrierGroupID = 659;
        public const int CyberLearningImplantsGroupID = 745;
        public const int CovertOpsGroupID = 830;
        public const int InterceptorGroupID = 831;
        public const int LogisticsGroupID = 832;
        public const int ForceReconShipGroupID = 833;
        public const int StealthBomberGroupID = 834;
        public const int CapitalIndustrialShipGroupID = 883;
        public const int ElectronicAttackShipGroupID = 893;
        public const int HeavyInterdictorGroupID = 894;
        public const int BlackOpsGroupID = 898;
        public const int MarauderGroupID = 900;
        public const int JumpFreighterGroupID = 902;
        public const int CombatReconShipGroupID = 906;
        public const int IndustrialCommandShipGroupID = 941;
        public const int StrategicCruiserGroupID = 963;
        public const int CorporationManagementSkillsGroupID = 266;

        #endregion


        #region Market group IDs

        public const int BlueprintsMarketGroupID = 2;
        public const int ShipsMarketGroupID = 4;
        public const int StandardFrigatesMarketGroupID = 5;
        public const int StandardCruisersMarketGroupID = 6;
        public const int StandardBattleshipsMarketGroupID = 7;
        public const int StandardIndustrialShipsMarketGroupID = 8;
        public const int ShipEquipmentsMarketGroupID = 9;
        public const int AmmosAndChargesMarketGroupID = 11;
        public const int ImplantsAndBoostersMarketGroupID = 24;
        public const int ImplantsMarketGroupID = 27;
        public const int SkillsMarketGroupID = 150;
        public const int DronesMarketGroupID = 157;
        public const int ShuttlessMarketGroupID = 391;
        public const int InterceptorsMarketGroupID = 399;
        public const int CovertOpsMarketGroupID = 420;
        public const int AssaultShipsMarketGroupID = 432;
        public const int LogisticsMarketGroupID = 437;
        public const int HeavyAssaultShipsMarketGroupID = 448;
        public const int StandardDestroyersMarketGroupID = 464;
        public const int StandardBattlecruisersMarketGroupID = 469;
        public const int ComponentsMarketGroupID = 475;
        public const int StarbaseStructuresMarketGroupID = 477;
        public const int MiningBargesMarketGroupID = 494;
        public const int OREMiningBargesMarketGroupID = 495;
        public const int SkillHardwiringImplantsMarketGroupID = 531;
        public const int AttributeEnhancersImplantsMarketGroupID = 532;
        public const int TransportShipsMarketGroupID = 629;
        public const int DreadnoughtsMarketGroupID = 761;
        public const int FreightersMarketGroupID = 766;
        public const int TitansMarketGroupID = 812;
        public const int CarriersMarketGroupID = 817;
        public const int CommandShipsMarketGroupID = 822;
        public const int InterdictorsMarketGroupID = 823;
        public const int ReconShipsMarketGroupID = 824;
        public const int FightersMarketGroupID = 840;
        public const int ExhumersMarketGroupID = 874;
        public const int OREExhumersMarketGroupID = 875;
        public const int ShipModificationsMarketGroupID = 955;
        public const int CapitalIndustrialShipsMarketGroupID = 1047;
        public const int ORECapitalIndustrialsMarketGroupID = 1048;
        public const int ElectronicAttackFrigatesMarketGroupID = 1065;
        public const int HeavyInterdictorsMarketGroupID = 1070;
        public const int BlackOpsMarketGroupID = 1075;
        public const int MaraudersMarketGroupID = 1080;
        public const int JumpFreightersMarketGroupID = 1089;
        public const int SubsystemsMarketGroupID = 1112;
        public const int StrategicCruisersMarketGroupID = 1138;
        public const int FighterBombersMarketGroupID = 1310;
        public const int OREIndustrialsMarketGroupID = 1390;

        #endregion


        #region Custom market group IDs

        public const int RootNonMarketGroupID = 11000;

        public const int UniqueDesignsRootNonMarketGroupID = 10000;
        public const int UniqueDesignBattleshipsNonMarketGroupID = 10200;
        public const int UniqueDesignShuttlesNonMarketGroupID = 10900;
        public const int RookieShipRootGroupID = 11100;
        public const int RookieShipAmarrGroupID = 11140;
        public const int RookieShipCaldariGroupID = 11110;
        public const int RookieShipGallenteGroupID = 11180;
        public const int RookieShipMinmatarGroupID = 11120;

        public const int BlueprintRootNonMarketGroupID = 21000;
        public const int BlueprintTechINonMarketGroupID = 21001;
        public const int BlueprintTechIINonMarketGroupID = 21002;
        public const int BlueprintStorylineNonMarketGroupID = 21003;
        public const int BlueprintFactionNonMarketGroupID = 21004;
        public const int BlueprintOfficerNonMarketGroupID = 21005;
        public const int BlueprintTechIIINonMarketGroupID = 21014;

        #endregion


        #region MetaGroup IDs

        public const int TechIMetaGroupID = 1;
        public const int TechIIMetaGroupID = 2;
        public const int StorylineMetaGroupID = 3;
        public const int FactionMetaGroupID = 4;
        public const int OfficerMetaGroupID = 5;
        public const int DeadspaceMetaGroupID = 6;
        public const int TechIIIMetaGroupID = 14;

        #endregion


        #region TechLevels

        public const int TechLevelII = 2;
        public const int TechLevelIII = 3;

        #endregion


        #region Icon IDs

        public const int UnknownIconID = 0;
        public const int UnknownShipIconID = 1443;
        public const int UnknownBlueprintBackdropIconID = 2703;

        #endregion


        #region Types IDs

        public const int CorporationID = 2;
        public const int TemperatePlanetID = 11;
        public const int IcePlanetID = 12;
        public const int GasPlanetID = 13;
        public const int ReaperID = 588;
        public const int ImpairorID = 596;
        public const int IbisID = 601;
        public const int VelatorID = 606;
        public const int CapsuleID = 670;
        public const int CharacterAmarrID = 1373;
        public const int CharacterVherokiorID = 1386;
        public const int OceanicPlanetID = 2014;
        public const int LavaPlanetID = 2015;
        public const int BarrenPlanetID = 2016;
        public const int StormPlanetID = 2017;
        public const int PlasmaPlanetID = 2063;
        public const int AdrestiaBlueprintID = 2837;
        public const int GunnerySkillID = 3300;
        public const int SmallHybridTurretSkillID = 3301;
        public const int SmallProjectileTurretSkillID = 3302;
        public const int SmallEnergyTurretSkillID = 3303;
        public const int SpaceshipCommandSkillID = 3327;
        public const int GallenteFrigateSkillID = 3328;
        public const int MinmatarFrigateSkillID = 3329;
        public const int CaldariFrigateSkillID = 3330;
        public const int AmarrFrigateSkillID = 3331;
        public const int DiplomacySkillID = 3357;
        public const int ConnectionsSkillID = 3359;
        public const int IndustrySkillID = 3380;
        public const int MiningSkillID = 3386;
        public const int MassProductionSkillID = 3387;
        public const int ProductionEfficiencySkillID = 3388;
        public const int MechanicSkillID = 3392;
        public const int ScienceSkillID = 3402;
        public const int ResearchSkillID = 3403;
        public const int LaboratoryOperationSkillID = 3406;
        public const int MetallurgySkillID = 3409;
        public const int EngineeringSkillID = 3413;
        public const int ShieldOperationSkillID = 3416;
        public const int ElectronicsSkillID = 3426;
        public const int TradeSkillID = 3443;
        public const int RetailSkillID = 3444;
        public const int BrokerRelationsSkillID = 3446;
        public const int VisibilitySkillID = 3447;
        public const int NavigationSkillID = 3449;
        public const int EchelonBlueprintID = 3533;
        public const int CivilianGatlingPulseLaserID = 3634;
        public const int CivilianGatlingAutocannonID = 3636;
        public const int CivilianGatlingRailgunID = 3638;
        public const int CivilianLightElectronBlasterID = 3640;
        public const int ScrapMetalProcessingSkillID = 12196;
        public const int MegathronFederateIssueID = 13202;
        public const int AllianceID = 16159;
        public const int ProcurementSkillID = 16594;
        public const int DaytradingSkillID = 16595;
        public const int WholesaleSkillID = 16596;
        public const int MarketingSkillID = 16598;
        public const int AccountingSkillID = 16622;
        public const int CaldariNavyHookbillBlueprintID = 17620;
        public const int ImperialNavySlicerBlueprintID = 17704;
        public const int PhantasmBlueprintID = 17719;
        public const int CynabalBlueprintID = 17721;
        public const int NightmareBlueprintID = 17737;
        public const int MacharielBlueprintID = 17739;
        public const int RepublicFleetFiretailBlueprintID = 17813;
        public const int FederationNavyCometBlueprintID = 17842;
        public const int AshimmuBlueprintID = 17923;
        public const int SuccubusBlueprintID = 17925;
        public const int CruorBlueprintID = 17927;
        public const int DaredevilBlueprintID = 17929;
        public const int DramielBlueprintID = 17933;
        public const int TycconSkillID = 18580;
        public const int GorusShuttleID = 21097;
        public const int GorusShuttleBlueprintID = 21098;
        public const int GuristasShuttleID = 21628;
        public const int GuristasShuttleBlueprintID = 21629;
        public const int GallenteMiningLaserBlueprintID = 21842;
        public const int WildMinerIBlueprintID = 22924;
        public const int InfomorphPsychologySkillID = 24242;
        public const int SupplyChainManagementSkillID = 24268;
        public const int ScientificNetworkingSkillID = 24270;
        public const int AdvancedLaboratoryOperationSkillID = 24624;
        public const int AdvancedMassProductionSkillID = 24625;
        public const int RavenStateIssueID = 26840;
        public const int TempestTribalIssueID = 26842;
        public const int CivilianDataInterfaceID = 27026;
        public const int ZainouBeancounterF50ID = 27167;
        public const int ZainouBeancounterG50ID = 27168;
        public const int ZainouBeancounterF40ID = 27170;
        public const int ZainouBeancounterF60ID = 27171;
        public const int ZainouBeancounterG60ID = 27172;
        public const int ZainouBeancounterG40ID = 27173;
        public const int ZainouBeancounterJ50ID = 27176;
        public const int ZainouBeancounterI50ID = 27177;
        public const int ZainouBeancounterK50ID = 27178;
        public const int ZainouBeancounterI60ID = 27179;
        public const int ZainouBeancounterI40ID = 27180;
        public const int ZainouBeancounterJ60ID = 27181;
        public const int ZainouBeancounterJ40ID = 27182;
        public const int ZainouBeancounterK60ID = 27184;
        public const int ZainouBeancounterK40ID = 27185;
        public const int TenguBlueprintID = 29985;
        public const int LegionBlueprintID = 29987;
        public const int ProteusBlueprintID = 29989;
        public const int LokiBlueprintID = 29991;
        public const int LegionElectronicsEnergyParasiticComplexBlueprintID = 30037;
        public const int TenguElectronicsObfuscationManifoldBlueprintID = 30047;
        public const int ProteusElectronicsFrictionExtensionProcessorBlueprintID = 30057;
        public const int LokiElectronicsImmobilityDriversBlueprintID = 30067;
        public const int LegionPropulsionChassisOptimizationBlueprintID = 30077;
        public const int TenguPropulsionIntercalatedNanofibersBlueprintID = 30087;
        public const int ProteusPropulsionWakeLimiterBlueprintID = 30097;
        public const int LokiPropulsionChassisOptimizationBlueprintID = 30107;
        public const int TenguEngineeringPowerCoreMultiplierBlueprintID = 30140;
        public const int ProteusEngineeringPowerCoreMultiplierBlueprintID = 30150;
        public const int LokiEngineeringPowerCoreMultiplierBlueprintID = 30160;
        public const int LegionEngineeringPowerCoreMultiplierBlueprintID = 30170;
        public const int LegionDefensiveAdaptiveAugmenterBlueprintID = 30227;
        public const int TenguDefensiveAdaptiveShieldingBlueprintID = 30232;
        public const int ProteusDefensiveAdaptiveAugmenterBlueprintID = 30237;
        public const int LokiDefensiveAdaptiveShieldingBlueprintID = 30242;
        public const int LegionOffensiveDroneSynthesisProjectorBlueprintID = 30392;
        public const int TenguOffensiveAcceleratedEjectionBayBlueprintID = 30397;
        public const int ProteusOffensiveDissonicEncodingPlatformBlueprintID = 30402;
        public const int LokiOffensiveTurretConcurrenceRegistryBlueprintID = 30407;
        public const int InterbusShuttleID = 30842;
        public const int InterbusShuttleBlueprintID = 30843;
        public const int ShatteredPlanetID = 30889;
        public const int FrekiBlueprintID = 32208;
        public const int MimirBlueprintID = 32210;

        #endregion
    }
}