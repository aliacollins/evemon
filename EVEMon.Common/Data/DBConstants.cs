
namespace EVEMon.Common.Data
{
    /// <summary>
    /// Provides constants about the CCP databases.
    /// </summary>
    public static class DBConstants
    {

        #region Collections

        // Group of PropertyIDs
        public static readonly int[] RequiredSkillPropertyIDs = new int[] 
            {
                RequiredSkill1PropertyID, RequiredSkill2PropertyID, RequiredSkill3PropertyID,
                RequiredSkill4PropertyID, RequiredSkill5PropertyID, RequiredSkill6PropertyID
            };

        public static readonly int[] RequiredSkillLevelPropertyIDs = new int[]
            {
                RequiredSkill1LevelPropertyID, RequiredSkill2LevelPropertyID, RequiredSkill3LevelPropertyID,
                RequiredSkill4LevelPropertyID, RequiredSkill5LevelPropertyID, RequiredSkill6LevelPropertyID 
            };

        public static readonly int[] AlwaysVisibleForShipPropertyIDs = new int[]
            { 
                CPUOutputPropertyID, PGOutputPropertyID, UpgradeCapacityPropertyID, HiSlotsPropertyID,
                MedSlotsPropertyID,LowSlotsPropertyID, DroneCapacityPropertyID, DroneBandwidthPropertyID,
                CargoCapacityPropertyID, MassPropertyID,VolumePropertyID, CapacitorCapacityPropertyID,
                CapacitorRechargeRatePropertyID, MaxTargetRangePropertyID, ScanResolutionPropertyID,
                SignatureRadiusPropertyID, MaxVelocityPropertyID, ShipWarpSpeedPropertyID, StructureHitpointsPropertyID,
                ShieldHitpointsPropertyID, ArmorHitpointsPropertyID, ShieldRechargeRatePropertyID,
                ShieldEMResistancePropertyID, ShieldExplosiveResistancePropertyID, ShieldKineticResistancePropertyID,
                ShieldThermalResistancePropertyID, ArmorEMResistancePropertyID, ArmorExplosiveResistancePropertyID,
                ArmorKineticResistancePropertyID, ArmorThermalResistancePropertyID
            };

        public static readonly int[] HideIfDefaultPropertyIDs = new int[]
            { 
                LauncherSlotsLeftPropertyID, TurretSlotsLeftPropertyID, TurretHardPointModifierPropertyID, 
                LauncherHardPointModifierPropertyID, HiSlotModifierPropertyID, MedSlotModifierPropertyID,
                LowSlotModifierPropertyID, ScanRadarStrengthPropertyID, ScanLadarStrengthPropertyID,
                ScanMagnetometricStrengthPropertyID, ScanGravimetricStrengthPropertyID, HullEMResistancePropertyID,
                HullExplosiveResistancePropertyID, HullKineticResistancePropertyID, HullThermalResistancePropertyID,
                EmDamagePropertyID, ExplosiveDamagePropertyID, KineticDamagePropertyID, ThermalDamagePropertyID, 
                CharismaModifierPropertyID, IntelligenceModifierPropertyID, MemoryModifierPropertyID, 
                PerceptionModifierPropertyID, WillpowerModifierPropertyID, MetaLevelPropertyID
            };

        public static readonly int[] LauncherGroupPropertyIDs = new int[]
            { 
                LauncherGroupPropertyID, LauncherGroup2PropertyID, LauncherGroup3PropertyID
            };

        public static readonly int[] ChargeGroupPropertyIDs = new int[]
            { 
                ChargeGroup1PropertyID, ChargeGroup2PropertyID, ChargeGroup3PropertyID,
                ChargeGroup4PropertyID, ChargeGroup5PropertyID
            };

        public static readonly int[] CanFitShipGroupPropertyIDs = new int[] 
            {
                CanFitShipGroup1PropertyID, CanFitShipGroup2PropertyID,
                CanFitShipGroup3PropertyID, CanFitShipGroup4PropertyID
            };

        public static readonly int[] ModuleShipGroupPropertyIDs = new int[]
            {
                ModuleShipGroup1PropertyID, ModuleShipGroup2PropertyID, ModuleShipGroup3PropertyID
            };

        public static readonly int[] ReactionGroupPropertyIDs = new int[]
            {
                ReactionGroup1PropertyID, ReactionGroup2PropertyID
            };

        public static readonly int[] IndustryModifyingPropertyIDs = new int[]
            { 
                ManufacturingTimeBonusPropertyID, ManufactureCostBonusPropertyID, CopySpeedBonusPropertyID,
                BlueprintManufactureTimeBonusPropertyID, MineralNeedResearchBonusPropertyID
            };

        public static readonly int[] SpecialisationAsteroidGroupPropertyIDs = new int[] 
            { SpecialisationAsteroidGroupPropertyID };

        public static readonly int[] PosCargobayAcceptGroupPropertyIDs = new int[]
            { PosCargobayAcceptGroupPropertyID };

        // Group of MarketGroupIDs
        public static readonly int[] StategicComponentsMarketGroupIDs = new int[]
            {
                SubsystemsMarketGroupID, StrategicCruisersMarketGroupID
            };

        public static readonly int[] SmallToXLargeShipsMarketGroupIDs = new int[]
            {
                StandardFrigatesMarketGroupID, StandardCruisersMarketGroupID, StandardBattleshipsMarketGroupID,
                StandardIndustrialShipsMarketGroupID, ShuttlessMarketGroupID, StandardDestroyersMarketGroupID,
                StandardBattlecruisersMarketGroupID, MiningBargesMarketGroupID, DreadnoughtsMarketGroupID,
                FreightersMarketGroupID, CarriersMarketGroupID, FightersMarketGroupID,
                CapitalIndustrialShipsMarketGroupID, FighterBombersMarketGroupID
            };

        public static readonly int[] CapitalShipsMarketGroupIDs = new int[] 
            {
                DreadnoughtsMarketGroupID, FreightersMarketGroupID, TitansMarketGroupID, CarriersMarketGroupID
            };

        public static readonly int[] AdvancedSmallToLargeShipsMarketGroupIDs = new int[]
            { 
                InterceptorsMarketGroupID, CovertOpsMarketGroupID, AssaultShipsMarketGroupID, LogisticsMarketGroupID,
                HeavyAssaultShipsMarketGroupID, TransportShipsMarketGroupID, CommandShipsMarketGroupID,
                InterdictorsMarketGroupID, ReconShipsMarketGroupID, ExhumersMarketGroupID,
                ElectronicAttackFrigatesMarketGroupID, HeavyInterdictorsMarketGroupID, BlackOpsMarketGroupID,
                MaraudersMarketGroupID, JumpFreightersMarketGroupID 
            };


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

        public const int PlanetGroupID = 7;
        public const int FrigateGroupID = 25;
        public const int CruiserGroupID = 26;
        public const int BattleshipGroupID = 27;
        public const int IndustrialGroupID = 28;
        public const int CapsuleGroupID = 29;
        public const int TitanGroupID = 30;
        public const int ShuttleGroupID = 31;
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
        public const int SubsystemsGroupID = 989;
        public const int PrototypeExplorationShipGroupID = 1022;
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
        public const int ShipsBlueprintsMarketGroupID = 204;
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

        public const int TechLevelI = 1;
        public const int TechLevelII = 2;
        public const int TechLevelIII = 3;

        #endregion


        #region Icon IDs

        public const int UnknownShipIconID = 1443;
        public const int UnknownBlueprintBackdropIconID = 2703;

        #endregion


        #region Types IDs

        public const int TemperatePlanetID = 11;
        public const int IcePlanetID = 12;
        public const int GasPlanetID = 13;
        public const int ReaperID = 588;
        public const int ImpairorID = 596;
        public const int IbisID = 601;
        public const int VelatorID = 606;
        public const int CapsuleID = 670;
        public const int OceanicPlanetID = 2014;
        public const int LavaPlanetID = 2015;
        public const int BarrenPlanetID = 2016;
        public const int StormPlanetID = 2017;
        public const int PlasmaPlanetID = 2063;
        public const int AdrestiaBlueprintID = 2837;
        public const int LaboratoryOperationSkillID = 3406;
        public const int VisibilitySkillID = 3447;
        public const int DiplomacySkillID = 3357;
        public const int ConnectionsSkillID = 3359;
        public const int IndustrySkillID = 3380;
        public const int MassProductionSkillID = 3387;
        public const int ProductionEfficiencySkillID = 3388;
        public const int ScienceSkillID = 3402;
        public const int ResearchSkillID = 3403;
        public const int MetallurgySkillID = 3409;
        public const int TradeSkillID = 3443;
        public const int RetailSkillID = 3444;
        public const int BrokerRelationsSkillID = 3446;
        public const int EchelonBlueprintID = 3533;
        public const int GallenteAdministrativeOutpostPlatformID = 10257;
        public const int MinmatarServiceOutpostPlatformID = 10258;
        public const int AmarrFactoryOutpostPlatformID = 10260;
        public const int ScrapMetalProcessingSkillID = 12196;
        public const int MegathronFederateIssueID = 13202;
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
        public const int GoldSculptureID = 17761;
        public const int RepublicFleetFiretailBlueprintID = 17813;
        public const int FederationNavyCometBlueprintID = 17842;
        public const int AshimmuBlueprintID = 17923;
        public const int SuccubusBlueprintID = 17925;
        public const int CruorBlueprintID = 17927;
        public const int DaredevilBlueprintID = 17929;
        public const int DramielBlueprintID = 17933;
        public const int TycconSkillID = 18580;
        public const int CaldariResearchOutpostPlatformID = 19758;
        public const int CrudeSculptureID = 21054;
        public const int GorusShuttleID = 21097;
        public const int GorusShuttleBlueprintID = 21098;
        public const int ProcessInterruptiveWarpDisruptorID = 21510;
        public const int GuristasShuttleID = 21628;
        public const int GuristasShuttleBlueprintID = 21629;
        public const int EliteDroneAIID = 21815;
        public const int GallenteMiningLaserBlueprintID = 21842;
        public const int DaemonCodebreakerIID = 22325;
        public const int CodexCodebreakerIID = 22327;
        public const int AlphaCodebreakerIID = 22329;
        public const int LibramCodebreakerIID = 22331;
        public const int TalocanDataAnalyzerIID = 22333;
        public const int SleeperDataAnalyzerIID = 22335;
        public const int TerranDataAnalyzerIID = 22337;
        public const int TetrimonDataAnalyzerIID = 22339;
        public const int WarpDisruptProbeBlueprintID = 22779;
        public const int WildMinerIID = 22923;
        public const int StandardDecodingDeviceID = 23882;
        public const int MethrosEnhancedDecodingDeviceID = 23883;
        public const int WildMinerIBlueprintID = 22924;
        public const int SupplyChainManagementSkillID = 24268;
        public const int ScientificNetworkingSkillID = 24270;
        public const int EncodingMatrixComponentID = 24289;
        public const int AdvancedLaboratoryOperationSkillID = 24624;
        public const int AdvancedMassProductionSkillID = 24625;
        public const int RavenStateIssueID = 26840;
        public const int TempestTribalIssueID = 26842;
        public const int ChalcopyriteID = 27029;
        public const int ClayPigeonID = 27038;
        public const int SynthSoothSayerBoosterBlueprintID = 28685;
        public const int AmberMykoserocinID = 28694;
        public const int ModifiedAugumeneAntidoteID = 29202;
        public const int MinmatarDNAID = 29203;
        public const int BasicRoboticsID = 29226;
        public const int TenguBlueprintID = 29985;
        public const int LegionBlueprintID = 29987;
        public const int ProteusBlueprintID = 29989;
        public const int LokiBlueprintID = 29991;
        public const int LegionElectronicsEnergyParasiticComplexBlueprintID = 30037;
        public const int TenguElectronicsObfuscationManifoldBlueprintID = 30047;
        public const int ProteusElectronicsFrictionExtensionProcessorBlueprintID = 30057;
        public const int LokiElectronicsImmobilityDriversBlueprintID = 30067;
        public const int LegionPropulsionChassisOptimizationBlueprintID = 30077;
        public const int SmallEWDroneRangeAugmentorIIBlueprintID = 32078;
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
        public const int CivilianStasisWebifierID = 30328;
        public const int LokiDefensiveAdaptiveShieldingBlueprintID = 30242;
        public const int CivilianHeatDissipationFieldID = 30342;
        public const int LegionOffensiveDroneSynthesisProjectorBlueprintID = 30392;
        public const int TenguOffensiveAcceleratedEjectionBayBlueprintID = 30397;
        public const int ProteusOffensiveDissonicEncodingPlatformBlueprintID = 30402;
        public const int LokiOffensiveTurretConcurrenceRegistryBlueprintID = 30407;
        public const int CivilianPhotonScatteringFieldID = 30420;
        public const int CivilianExplosionDampeningFieldID = 30422;
        public const int CivilianBallisticDeflectionFieldID = 30424;
        public const int CivilianDamageControlID = 30839;
        public const int InterbusShuttleID = 30842;
        public const int InterbusShuttleBlueprintID = 30843;
        public const int ShatteredPlanetID = 30889;
        public const int SmallEWDroneRangeAugmentorIIID = 32077;
        public const int FrekiBlueprintID = 32208;
        public const int MimirBlueprintID = 32210;

        #endregion
    }
}
