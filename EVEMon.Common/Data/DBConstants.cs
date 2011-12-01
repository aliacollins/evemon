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
        public static readonly ReadOnlyCollection<long> RequiredSkillPropertyIDs =
            new ReadOnlyCollection<long>(new[]
                                            {
                                                RequiredSkill1PropertyID, RequiredSkill2PropertyID,
                                                RequiredSkill3PropertyID, RequiredSkill4PropertyID,
                                                RequiredSkill5PropertyID, RequiredSkill6PropertyID
                                            });

        public static readonly ReadOnlyCollection<long> RequiredSkillLevelPropertyIDs =
            new ReadOnlyCollection<long>(new[]
                                            {
                                                RequiredSkill1LevelPropertyID, RequiredSkill2LevelPropertyID,
                                                RequiredSkill3LevelPropertyID, RequiredSkill4LevelPropertyID,
                                                RequiredSkill5LevelPropertyID, RequiredSkill6LevelPropertyID
                                            });

        public static readonly ReadOnlyCollection<long> AlwaysVisibleForShipPropertyIDs =
            new ReadOnlyCollection<long>(new[]
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

        public static readonly ReadOnlyCollection<long> HideIfDefaultPropertyIDs =
            new ReadOnlyCollection<long>(new[]
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

        public static readonly ReadOnlyCollection<long> LauncherGroupPropertyIDs =
            new ReadOnlyCollection<long>(new[]
                                            {
                                                LauncherGroupPropertyID, LauncherGroup2PropertyID, LauncherGroup3PropertyID
                                            });

        public static readonly ReadOnlyCollection<long> ChargeGroupPropertyIDs =
            new ReadOnlyCollection<long>(new[]
                                            {
                                                ChargeGroup1PropertyID, ChargeGroup2PropertyID, ChargeGroup3PropertyID,
                                                ChargeGroup4PropertyID, ChargeGroup5PropertyID
                                            });

        public static readonly ReadOnlyCollection<long> CanFitShipGroupPropertyIDs =
            new ReadOnlyCollection<long>(new[]
                                            {
                                                CanFitShipGroup1PropertyID, CanFitShipGroup2PropertyID,
                                                CanFitShipGroup3PropertyID, CanFitShipGroup4PropertyID
                                            });

        public static readonly ReadOnlyCollection<long> ModuleShipGroupPropertyIDs =
            new ReadOnlyCollection<long>(new[]
                                            {
                                                ModuleShipGroup1PropertyID, ModuleShipGroup2PropertyID, ModuleShipGroup3PropertyID
                                            });

        public static readonly ReadOnlyCollection<long> ReactionGroupPropertyIDs =
            new ReadOnlyCollection<long>(new[] { ReactionGroup1PropertyID, ReactionGroup2PropertyID });

        public static readonly ReadOnlyCollection<long> IndustryModifyingPropertyIDs =
            new ReadOnlyCollection<long>(new[]
                                             {
                                                 ManufacturingTimeBonusPropertyID, ManufactureCostBonusPropertyID,
                                                 CopySpeedBonusPropertyID, BlueprintManufactureTimeBonusPropertyID,
                                                 MineralNeedResearchBonusPropertyID
                                             });

        public static readonly ReadOnlyCollection<long> SpecialisationAsteroidGroupPropertyIDs =
            new ReadOnlyCollection<long>(new[] { SpecialisationAsteroidGroupPropertyID });

        public static readonly ReadOnlyCollection<long> PosCargobayAcceptGroupPropertyIDs =
            new ReadOnlyCollection<long>(new[] { PosCargobayAcceptGroupPropertyID });

        // Group of MarketGroupIDs
        public static readonly ReadOnlyCollection<long> StategicComponentsMarketGroupIDs =
            new ReadOnlyCollection<long>(new[] { SubsystemsMarketGroupID, StrategicCruisersMarketGroupID });

        public static readonly ReadOnlyCollection<long> SmallToXLargeShipsMarketGroupIDs =
            new ReadOnlyCollection<long>(new[]
                                            {
                                                StandardFrigatesMarketGroupID, StandardCruisersMarketGroupID,
                                                StandardBattleshipsMarketGroupID, StandardIndustrialShipsMarketGroupID,
                                                ShuttlessMarketGroupID, StandardDestroyersMarketGroupID,
                                                StandardBattlecruisersMarketGroupID, MiningBargesMarketGroupID,
                                                DreadnoughtsMarketGroupID, FreightersMarketGroupID, CarriersMarketGroupID,
                                                FightersMarketGroupID, CapitalIndustrialShipsMarketGroupID,
                                                FighterBombersMarketGroupID
                                            });

        public static readonly ReadOnlyCollection<long> CapitalShipsMarketGroupIDs =
            new ReadOnlyCollection<long>(new[]
                                            {
                                                DreadnoughtsMarketGroupID, FreightersMarketGroupID,
                                                TitansMarketGroupID, CarriersMarketGroupID
                                            });

        public static readonly ReadOnlyCollection<long> AdvancedSmallToLargeShipsMarketGroupIDs =
            new ReadOnlyCollection<long>(new[]
                                            {
                                                InterceptorsMarketGroupID, CovertOpsMarketGroupID, AssaultShipsMarketGroupID,
                                                LogisticsMarketGroupID, HeavyAssaultShipsMarketGroupID,
                                                TransportShipsMarketGroupID, CommandShipsMarketGroupID, InterdictorsMarketGroupID,
                                                ReconShipsMarketGroupID, ExhumersMarketGroupID,
                                                ElectronicAttackFrigatesMarketGroupID, HeavyInterdictorsMarketGroupID,
                                                BlackOpsMarketGroupID, MaraudersMarketGroupID, JumpFreightersMarketGroupID
                                            });

        #endregion


        #region Effect IDs

        public const long LowSlotEffectID = 11;
        public const long HiSlotEffectID = 12;
        public const long MedSlotEffectID = 13;

        #endregion


        #region Attribute Category IDs

        public const long FittingAtributeCategoryID = 1;
        public const long StructureAtributeCategoryID = 4;
        public const long MiscellaneousAttributeCategoryID = 7;
        public const long NULLAtributeCategoryID = 9;

        #endregion


        #region Attribute types IDs (Properties)

        public const long MassPropertyID = 4;
        public const long CapacitorNeedPropertyID = 6;
        public const long StructureHitpointsPropertyID = 9;
        public const long PGOutputPropertyID = 11;
        public const long LowSlotsPropertyID = 12;
        public const long MedSlotsPropertyID = 13;
        public const long HiSlotsPropertyID = 14;
        public const long PGNeedPropertyID = 30;
        public const long MaxVelocityPropertyID = 37;
        public const long CargoCapacityPropertyID = 38;
        public const long CPUOutputPropertyID = 48;
        public const long CPUNeedPropertyID = 50;
        public const long CapacitorRechargeRatePropertyID = 55;
        public const long ShieldBonusPropertyID = 68;
        public const long AgilityPropertyID = 70;
        public const long DurationPropertyID = 73;
        public const long MaxTargetRangePropertyID = 76;
        public const long ShieldTransferRangePropertyID = 87;
        public const long LauncherSlotsLeftPropertyID = 101;
        public const long TurretSlotsLeftPropertyID = 102;
        public const long EmDamagePropertyID = 114;
        public const long ExplosiveDamagePropertyID = 116;
        public const long KineticDamagePropertyID = 117;
        public const long ThermalDamagePropertyID = 118;
        public const long LauncherGroupPropertyID = 137;
        public const long CapacitorRechargeRateMultiplierPropertyID = 144;
        public const long VolumePropertyID = 161;
        public const long CharismaPropertyID = 164;
        public const long IntelligencePropertyID = 165;
        public const long MemoryPropertyID = 166;
        public const long PerceptionPropertyID = 167;
        public const long WillpowerPropertyID = 168;
        public const long CharismaModifierPropertyID = 175;
        public const long IntelligenceModifierPropertyID = 176;
        public const long MemoryModifierPropertyID = 177;
        public const long PerceptionModifierPropertyID = 178;
        public const long WillpowerModifierPropertyID = 179;
        public const long PrimaryAttributePropertyID = 180;
        public const long SecondaryAttributePropertyID = 181;
        public const long RequiredSkill1PropertyID = 182;
        public const long RequiredSkill2PropertyID = 183;
        public const long RequiredSkill3PropertyID = 184;
        public const long ScanRadarStrengthPropertyID = 208;
        public const long ScanLadarStrengthPropertyID = 209;
        public const long ScanMagnetometricStrengthPropertyID = 210;
        public const long ScanGravimetricStrengthPropertyID = 211;
        public const long ShieldHitpointsPropertyID = 263;
        public const long ArmorHitpointsPropertyID = 265;
        public const long ArmorEMResistancePropertyID = 267;
        public const long ArmorExplosiveResistancePropertyID = 268;
        public const long ArmorKineticResistancePropertyID = 269;
        public const long ArmorThermalResistancePropertyID = 270;
        public const long ShieldEMResistancePropertyID = 271;
        public const long ShieldExplosiveResistancePropertyID = 272;
        public const long ShieldKineticResistancePropertyID = 273;
        public const long ShieldThermalResistancePropertyID = 274;
        public const long SkillTimeConstantPropertyID = 275;
        public const long RequiredSkill1LevelPropertyID = 277;
        public const long RequiredSkill2LevelPropertyID = 278;
        public const long RequiredSkill3LevelPropertyID = 279;
        public const long DroneCapacityPropertyID = 283;
        public const long ImplantSlotPropertyID = 331;
        public const long TechLevelPropertyID = 422;
        public const long CPUOutputBonusPropertyID = 424;
        public const long ManufacturingTimeBonusPropertyID = 440;
        public const long ManufactureCostBonusPropertyID = 451;
        public const long CopySpeedBonusPropertyID = 452;
        public const long BlueprintManufactureTimeBonusPropertyID = 453;
        public const long MineralNeedResearchBonusPropertyID = 468;
        public const long ShieldRechargeRatePropertyID = 479;
        public const long CapacitorCapacityPropertyID = 482;
        public const long SignatureRadiusPropertyID = 552;
        public const long AnchoringDelayPropertyID = 556;
        public const long CloakingTargetingDelayPropertyID = 560;
        public const long ScanResolutionPropertyID = 564;
        public const long WarpSpeedMultiplierPropertyID = 600;
        public const long LauncherGroup2PropertyID = 602;
        public const long LauncherGroup3PropertyID = 603;
        public const long ChargeGroup1PropertyID = 604;
        public const long ChargeGroup2PropertyID = 605;
        public const long ChargeGroup3PropertyID = 606;
        public const long ChargeGroup4PropertyID = 609;
        public const long ChargeGroup5PropertyID = 610;
        public const long MetaLevelPropertyID = 633;
        public const long ModuleShipGroup2PropertyID = 666;
        public const long ModuleShipGroup3PropertyID = 667;
        public const long ModuleShipGroup1PropertyID = 668;
        public const long ModuleReactivationDelayPropertyID = 669;
        public const long UnanchoringDelayPropertyID = 676;
        public const long OnliningDelayPropertyID = 677;
        public const long IceHarvestCycleBonusPropertyID = 780;
        public const long SpecialisationAsteroidGroupPropertyID = 781;
        public const long ReprocessingSkillPropertyID = 790;
        public const long ShipBonusPirateFactionPropertyID = 793;
        public const long ReactionGroup1PropertyID = 842;
        public const long ReactionGroup2PropertyID = 843;
        public const long ShipMaintenanceBayCapacityPropertyID = 908;
        public const long HullEMResistancePropertyID = 974;
        public const long HullExplosiveResistancePropertyID = 975;
        public const long HullKineticResistancePropertyID = 976;
        public const long HullThermalResistancePropertyID = 977;
        public const long CanNotBeTrainedOnTrialPropertyID = 1047;
        public const long CPUPenaltyPercentPropertyID = 1082;
        public const long UpgradeCapacityPropertyID = 1132;
        public const long RigSlotsPropertyID = 1137;
        public const long UpgradeCostPropertyID = 1153;
        public const long DroneBandwidthPropertyID = 1271;
        public const long DroneBandwidthUsedPropertyID = 1272;
        public const long ShipWarpSpeedPropertyID = 1281;
        public const long RequiredSkill4PropertyID = 1285;
        public const long RequiredSkill4LevelPropertyID = 1286;
        public const long RequiredSkill5LevelPropertyID = 1287;
        public const long RequiredSkill6LevelPropertyID = 1288;
        public const long RequiredSkill5PropertyID = 1289;
        public const long RequiredSkill6PropertyID = 1290;
        public const long CanFitShipGroup1PropertyID = 1298;
        public const long CanFitShipGroup2PropertyID = 1299;
        public const long CanFitShipGroup3PropertyID = 1300;
        public const long CanFitShipGroup4PropertyID = 1301;
        public const long PosCargobayAcceptGroupPropertyID = 1352;
        public const long MaxSubSystemsPropertyID = 1367;
        public const long TurretHardPointModifierPropertyID = 1368;
        public const long LauncherHardPointModifierPropertyID = 1369;
        public const long HiSlotModifierPropertyID = 1374;
        public const long MedSlotModifierPropertyID = 1375;
        public const long LowSlotModifierPropertyID = 1376;
        public const long FitsToShipTypePropertyID = 1380;
        public const long AITargetSwitchTimerPropertyID = 1416;
        public const long RigSizePropertyID = 1547;
        public const long MetaGroupPropertyID = 1692;

        #endregion


        #region EVE Unit IDs (Properties UnitID's)

        public const long MassUnitID = 2;
        public const long MillsecondsUnitID = 101;
        public const long AbsolutePercentUnitID = 127;
        public const long InverseAbsolutePercentUnitID = 108;
        public const long ModifierPercentUnitID = 109;
        public const long InversedModifierPercentUnitID = 111;
        public const long GroupIDUnitID = 115;
        public const long TypeUnitID = 116;
        public const long SizeclassUnitID = 117;

        #endregion


        #region Certificate Classes IDs

        public const long IndustrialHarvestingID = 104;
        public const long AutomatedMiningID = 106;
        public const long ProductionInternID = 111;

        #endregion


        #region Certificate Grade IDs

        public const long BasicID = 1;
        public const long StandardID = 2;
        public const long ImprovedID = 3;
        public const long EliteID = 5;

        #endregion


        #region Category IDs

        public const long BlueprintCategoryID = 9;
        public const long ImplantCategoryID = 20;
        public const long SkillCategoryID = 16;

        #endregion


        #region Group IDs

        public const long PlanetGroupID = 7;
        public const long FrigateGroupID = 25;
        public const long CruiserGroupID = 26;
        public const long BattleshipGroupID = 27;
        public const long IndustrialGroupID = 28;
        public const long CapsuleGroupID = 29;
        public const long TitanGroupID = 30;
        public const long ShuttleGroupID = 31;
        public const long RookieShipGroupID = 237;
        public const long TradeSkillsGroupID = 274;
        public const long SocialSkillsGroupID = 278;
        public const long AssaultShipGroupID = 324;
        public const long HeavyAssaultShipGroupID = 358;
        public const long TransportShipGroupID = 380;
        public const long EliteBattleshipGroupID = 381;
        public const long BattlecruiserGroupID = 419;
        public const long DestroyerGroupID = 420;
        public const long MiningBargeGroupID = 463;
        public const long DreadnoughtGroupID = 485;
        public const long FakeSkillsGroupID = 505;
        public const long FreighterGroupID = 513;
        public const long CommandShipGroupID = 540;
        public const long InterdictorGroupID = 541;
        public const long ExhumerGroupID = 543;
        public const long CarrierGroupID = 547;
        public const long SupercarrierGroupID = 659;
        public const long CyberLearningImplantsGroupID = 745;
        public const long CovertOpsGroupID = 830;
        public const long InterceptorGroupID = 831;
        public const long LogisticsGroupID = 832;
        public const long ForceReconShipGroupID = 833;
        public const long StealthBomberGroupID = 834;
        public const long CapitalIndustrialShipGroupID = 883;
        public const long ElectronicAttackShipGroupID = 893;
        public const long HeavyInterdictorGroupID = 894;
        public const long BlackOpsGroupID = 898;
        public const long MarauderGroupID = 900;
        public const long JumpFreighterGroupID = 902;
        public const long CombatReconShipGroupID = 906;
        public const long IndustrialCommandShipGroupID = 941;
        public const long StrategicCruiserGroupID = 963;
        public const long SubsystemsGroupID = 989;
        public const long PrototypeExplorationShipGroupID = 1022;
        public const long CorporationManagementSkillsGroupID = 266;

        #endregion


        #region Market group IDs

        public const long BlueprintsMarketGroupID = 2;
        public const long ShipsMarketGroupID = 4;
        public const long StandardFrigatesMarketGroupID = 5;
        public const long StandardCruisersMarketGroupID = 6;
        public const long StandardBattleshipsMarketGroupID = 7;
        public const long StandardIndustrialShipsMarketGroupID = 8;
        public const long ShipEquipmentsMarketGroupID = 9;
        public const long AmmosAndChargesMarketGroupID = 11;
        public const long ImplantsAndBoostersMarketGroupID = 24;
        public const long ImplantsMarketGroupID = 27;
        public const long SkillsMarketGroupID = 150;
        public const long DronesMarketGroupID = 157;
        public const long ShipsBlueprintsMarketGroupID = 204;
        public const long ShuttlessMarketGroupID = 391;
        public const long InterceptorsMarketGroupID = 399;
        public const long CovertOpsMarketGroupID = 420;
        public const long AssaultShipsMarketGroupID = 432;
        public const long LogisticsMarketGroupID = 437;
        public const long HeavyAssaultShipsMarketGroupID = 448;
        public const long StandardDestroyersMarketGroupID = 464;
        public const long StandardBattlecruisersMarketGroupID = 469;
        public const long ComponentsMarketGroupID = 475;
        public const long StarbaseStructuresMarketGroupID = 477;
        public const long MiningBargesMarketGroupID = 494;
        public const long OREMiningBargesMarketGroupID = 495;
        public const long SkillHardwiringImplantsMarketGroupID = 531;
        public const long AttributeEnhancersImplantsMarketGroupID = 532;
        public const long TransportShipsMarketGroupID = 629;
        public const long DreadnoughtsMarketGroupID = 761;
        public const long FreightersMarketGroupID = 766;
        public const long TitansMarketGroupID = 812;
        public const long CarriersMarketGroupID = 817;
        public const long CommandShipsMarketGroupID = 822;
        public const long InterdictorsMarketGroupID = 823;
        public const long ReconShipsMarketGroupID = 824;
        public const long FightersMarketGroupID = 840;
        public const long ExhumersMarketGroupID = 874;
        public const long OREExhumersMarketGroupID = 875;
        public const long ShipModificationsMarketGroupID = 955;
        public const long CapitalIndustrialShipsMarketGroupID = 1047;
        public const long ORECapitalIndustrialsMarketGroupID = 1048;
        public const long ElectronicAttackFrigatesMarketGroupID = 1065;
        public const long HeavyInterdictorsMarketGroupID = 1070;
        public const long BlackOpsMarketGroupID = 1075;
        public const long MaraudersMarketGroupID = 1080;
        public const long JumpFreightersMarketGroupID = 1089;
        public const long SubsystemsMarketGroupID = 1112;
        public const long StrategicCruisersMarketGroupID = 1138;
        public const long FighterBombersMarketGroupID = 1310;
        public const long OREIndustrialsMarketGroupID = 1390;

        #endregion


        #region Custom market group IDs

        public const long RootNonMarketGroupID = 11000;

        public const long UniqueDesignsRootNonMarketGroupID = 10000;
        public const long UniqueDesignBattleshipsNonMarketGroupID = 10200;
        public const long UniqueDesignShuttlesNonMarketGroupID = 10900;
        public const long RookieShipRootGroupID = 11100;
        public const long RookieShipAmarrGroupID = 11140;
        public const long RookieShipCaldariGroupID = 11110;
        public const long RookieShipGallenteGroupID = 11180;
        public const long RookieShipMinmatarGroupID = 11120;

        public const long BlueprintRootNonMarketGroupID = 21000;
        public const long BlueprintTechINonMarketGroupID = 21001;
        public const long BlueprintTechIINonMarketGroupID = 21002;
        public const long BlueprintStorylineNonMarketGroupID = 21003;
        public const long BlueprintFactionNonMarketGroupID = 21004;
        public const long BlueprintOfficerNonMarketGroupID = 21005;
        public const long BlueprintTechIIINonMarketGroupID = 21014;

        #endregion


        #region MetaGroup IDs

        public const long TechIMetaGroupID = 1;
        public const long TechIIMetaGroupID = 2;
        public const long StorylineMetaGroupID = 3;
        public const long FactionMetaGroupID = 4;
        public const long OfficerMetaGroupID = 5;
        public const long DeadspaceMetaGroupID = 6;
        public const long TechIIIMetaGroupID = 14;

        #endregion


        #region TechLevels

        public const long TechLevelI = 1;
        public const long TechLevelII = 2;
        public const long TechLevelIII = 3;

        #endregion


        #region Icon IDs

        public const long UnknownShipIconID = 1443;
        public const long UnknownBlueprintBackdropIconID = 2703;

        #endregion


        #region Types IDs

        public const long CorporationID = 2;
        public const long TemperatePlanetID = 11;
        public const long IcePlanetID = 12;
        public const long GasPlanetID = 13;
        public const long ReaperID = 588;
        public const long ImpairorID = 596;
        public const long IbisID = 601;
        public const long VelatorID = 606;
        public const long CapsuleID = 670;
        public const long CharacterAmarrID = 1373;
        public const long CharacterVherokiorID = 1386;
        public const long OceanicPlanetID = 2014;
        public const long LavaPlanetID = 2015;
        public const long BarrenPlanetID = 2016;
        public const long StormPlanetID = 2017;
        public const long PlasmaPlanetID = 2063;
        public const long AdrestiaBlueprintID = 2837;
        public const long GunnerySkillID = 3300;
        public const long SmallHybridTurretSkillID = 3301;
        public const long SmallProjectileTurretSkillID = 3302;
        public const long SmallEnergyTurretSkillID = 3303;
        public const long SpaceshipCommandSkillID = 3327;
        public const long GallenteFrigateSkillID = 3328;
        public const long MinmatarFrigateSkillID = 3329;
        public const long CaldariFrigateSkillID = 3330;
        public const long AmarrFrigateSkillID = 3331;
        public const long DiplomacySkillID = 3357;
        public const long ConnectionsSkillID = 3359;
        public const long IndustrySkillID = 3380;
        public const long MiningSkillID = 3386;
        public const long MassProductionSkillID = 3387;
        public const long ProductionEfficiencySkillID = 3388;
        public const long MechanicSkillID = 3392;
        public const long ScienceSkillID = 3402;
        public const long ResearchSkillID = 3403;
        public const long LaboratoryOperationSkillID = 3406;
        public const long MetallurgySkillID = 3409;
        public const long EngineeringSkillID = 3413;
        public const long ShieldOperationSkillID = 3416;
        public const long ElectronicsSkillID = 3426;
        public const long TradeSkillID = 3443;
        public const long RetailSkillID = 3444;
        public const long BrokerRelationsSkillID = 3446;
        public const long VisibilitySkillID = 3447;
        public const long NavigationSkillID = 3449;
        public const long EchelonBlueprintID = 3533;
        public const long GallenteAdministrativeOutpostPlatformID = 10257;
        public const long MinmatarServiceOutpostPlatformID = 10258;
        public const long AmarrFactoryOutpostPlatformID = 10260;
        public const long ScrapMetalProcessingSkillID = 12196;
        public const long MegathronFederateIssueID = 13202;
        public const long AllianceID = 16159;
        public const long ProcurementSkillID = 16594;
        public const long DaytradingSkillID = 16595;
        public const long WholesaleSkillID = 16596;
        public const long MarketingSkillID = 16598;
        public const long AccountingSkillID = 16622;
        public const long CaldariNavyHookbillBlueprintID = 17620;
        public const long ImperialNavySlicerBlueprintID = 17704;
        public const long PhantasmBlueprintID = 17719;
        public const long CynabalBlueprintID = 17721;
        public const long NightmareBlueprintID = 17737;
        public const long MacharielBlueprintID = 17739;
        public const long GoldSculptureID = 17761;
        public const long RepublicFleetFiretailBlueprintID = 17813;
        public const long FederationNavyCometBlueprintID = 17842;
        public const long AshimmuBlueprintID = 17923;
        public const long SuccubusBlueprintID = 17925;
        public const long CruorBlueprintID = 17927;
        public const long DaredevilBlueprintID = 17929;
        public const long DramielBlueprintID = 17933;
        public const long TycconSkillID = 18580;
        public const long CaldariResearchOutpostPlatformID = 19758;
        public const long CrudeSculptureID = 21054;
        public const long GorusShuttleID = 21097;
        public const long GorusShuttleBlueprintID = 21098;
        public const long ProcessInterruptiveWarpDisruptorID = 21510;
        public const long GuristasShuttleID = 21628;
        public const long GuristasShuttleBlueprintID = 21629;
        public const long EliteDroneAIID = 21815;
        public const long GallenteMiningLaserBlueprintID = 21842;
        public const long DaemonCodebreakerIID = 22325;
        public const long CodexCodebreakerIID = 22327;
        public const long AlphaCodebreakerIID = 22329;
        public const long LibramCodebreakerIID = 22331;
        public const long TalocanDataAnalyzerIID = 22333;
        public const long SleeperDataAnalyzerIID = 22335;
        public const long TerranDataAnalyzerIID = 22337;
        public const long TetrimonDataAnalyzerIID = 22339;
        public const long WarpDisruptProbeBlueprintID = 22779;
        public const long WildMinerIID = 22923;
        public const long StandardDecodingDeviceID = 23882;
        public const long MethrosEnhancedDecodingDeviceID = 23883;
        public const long WildMinerIBlueprintID = 22924;
        public const long InfomorphPsychologySkillID = 24242;
        public const long SupplyChainManagementSkillID = 24268;
        public const long ScientificNetworkingSkillID = 24270;
        public const long EncodingMatrixComponentID = 24289;
        public const long AdvancedLaboratoryOperationSkillID = 24624;
        public const long AdvancedMassProductionSkillID = 24625;
        public const long RavenStateIssueID = 26840;
        public const long TempestTribalIssueID = 26842;
        public const long ChalcopyriteID = 27029;
        public const long ClayPigeonID = 27038;
        public const long SynthSoothSayerBoosterBlueprintID = 28685;
        public const long AmberMykoserocinID = 28694;
        public const long ModifiedAugumeneAntidoteID = 29202;
        public const long MinmatarDNAID = 29203;
        public const long BasicRoboticsID = 29226;
        public const long TenguBlueprintID = 29985;
        public const long LegionBlueprintID = 29987;
        public const long ProteusBlueprintID = 29989;
        public const long LokiBlueprintID = 29991;
        public const long LegionElectronicsEnergyParasiticComplexBlueprintID = 30037;
        public const long TenguElectronicsObfuscationManifoldBlueprintID = 30047;
        public const long ProteusElectronicsFrictionExtensionProcessorBlueprintID = 30057;
        public const long LokiElectronicsImmobilityDriversBlueprintID = 30067;
        public const long LegionPropulsionChassisOptimizationBlueprintID = 30077;
        public const long SmallEWDroneRangeAugmentorIIBlueprintID = 32078;
        public const long TenguPropulsionIntercalatedNanofibersBlueprintID = 30087;
        public const long ProteusPropulsionWakeLimiterBlueprintID = 30097;
        public const long LokiPropulsionChassisOptimizationBlueprintID = 30107;
        public const long TenguEngineeringPowerCoreMultiplierBlueprintID = 30140;
        public const long ProteusEngineeringPowerCoreMultiplierBlueprintID = 30150;
        public const long LokiEngineeringPowerCoreMultiplierBlueprintID = 30160;
        public const long LegionEngineeringPowerCoreMultiplierBlueprintID = 30170;
        public const long LegionDefensiveAdaptiveAugmenterBlueprintID = 30227;
        public const long TenguDefensiveAdaptiveShieldingBlueprintID = 30232;
        public const long ProteusDefensiveAdaptiveAugmenterBlueprintID = 30237;
        public const long CivilianStasisWebifierID = 30328;
        public const long LokiDefensiveAdaptiveShieldingBlueprintID = 30242;
        public const long CivilianHeatDissipationFieldID = 30342;
        public const long LegionOffensiveDroneSynthesisProjectorBlueprintID = 30392;
        public const long TenguOffensiveAcceleratedEjectionBayBlueprintID = 30397;
        public const long ProteusOffensiveDissonicEncodingPlatformBlueprintID = 30402;
        public const long LokiOffensiveTurretConcurrenceRegistryBlueprintID = 30407;
        public const long CivilianPhotonScatteringFieldID = 30420;
        public const long CivilianExplosionDampeningFieldID = 30422;
        public const long CivilianBallisticDeflectionFieldID = 30424;
        public const long CivilianDamageControlID = 30839;
        public const long InterbusShuttleID = 30842;
        public const long InterbusShuttleBlueprintID = 30843;
        public const long ShatteredPlanetID = 30889;
        public const long SmallEWDroneRangeAugmentorIIID = 32077;
        public const long FrekiBlueprintID = 32208;
        public const long MimirBlueprintID = 32210;

        #endregion
    }
}