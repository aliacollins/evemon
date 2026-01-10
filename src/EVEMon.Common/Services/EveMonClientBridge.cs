using System;
using System.Linq;
using EVEMon.Common.Abstractions.Events;
using EVEMon.Common.CustomEventArgs;

namespace EVEMon.Common.Services
{
    /// <summary>
    /// Bridges legacy EveMonClient static events to the new EventBroker system.
    /// This enables the Strangler Fig migration pattern - old UI code continues
    /// working with legacy events while new code can use the EventBroker.
    /// </summary>
    public static class EveMonClientBridge
    {
        private static IEventBroker _eventBroker;
        private static bool _initialized;

        /// <summary>
        /// Initializes the bridge, connecting legacy events to the EventBroker.
        /// Call this after EveMonClient.Initialize() but before UI loads.
        /// </summary>
        /// <param name="eventBroker">The event broker to publish events to.</param>
        public static void Initialize(IEventBroker eventBroker)
        {
            if (_initialized)
                return;

            _eventBroker = eventBroker ?? throw new ArgumentNullException(nameof(eventBroker));

            // Bridge timer events
            BridgeTimerEvents();

            // Bridge settings events
            BridgeSettingsEvents();

            // Bridge character events
            BridgeCharacterEvents();

            // Bridge ESI/API events
            BridgeESIEvents();

            // Bridge server events
            BridgeServerEvents();

            // Bridge notification events
            BridgeNotificationEvents();

            // Bridge plan events
            BridgePlanEvents();

            // Bridge global data events
            BridgeGlobalDataEvents();

            // Bridge update events
            BridgeUpdateEvents();

            _initialized = true;
        }

        #region Timer Events

        private static void BridgeTimerEvents()
        {
            EveMonClient.SecondTick += (s, e) =>
                _eventBroker.Publish(new SecondTickEvent());

            EveMonClient.FiveSecondTick += (s, e) =>
                _eventBroker.Publish(new FiveSecondTickEvent());

            EveMonClient.ThirtySecondTick += (s, e) =>
                _eventBroker.Publish(new ThirtySecondTickEvent());
        }

        #endregion

        #region Settings Events

        private static void BridgeSettingsEvents()
        {
            EveMonClient.SettingsChanged += (s, e) =>
                _eventBroker.Publish(new SettingsChangedEvent());

            EveMonClient.SchedulerChanged += (s, e) =>
                _eventBroker.Publish(new SchedulerChangedEvent());
        }

        #endregion

        #region Character Events

        private static void BridgeCharacterEvents()
        {
            // Core character events
            EveMonClient.CharacterUpdated += (s, e) =>
                _eventBroker.Publish(new CharacterUpdatedEvent(e.Character));

            EveMonClient.CharacterInfoUpdated += (s, e) =>
                _eventBroker.Publish(new CharacterInfoUpdatedEvent(e.Character));

            EveMonClient.CharacterPortraitUpdated += (s, e) =>
                _eventBroker.Publish(new CharacterPortraitUpdatedEvent(e.Character));

            EveMonClient.CharacterLabelChanged += (s, e) =>
                _eventBroker.Publish(new CharacterLabelChangedEvent(e.Character, e.AllLabels.ToList().AsReadOnly()));

            EveMonClient.CharacterSkillQueueUpdated += (s, e) =>
                _eventBroker.Publish(new CharacterSkillQueueUpdatedEvent(e.Character));

            // Batched events
            EveMonClient.CharactersBatchUpdated += (s, e) =>
                _eventBroker.Publish(new CharactersBatchUpdatedEvent(e.Characters.ToList().AsReadOnly()));

            EveMonClient.SkillQueuesBatchUpdated += (s, e) =>
                _eventBroker.Publish(new SkillQueuesBatchUpdatedEvent(e.Characters.ToList().AsReadOnly()));

            EveMonClient.QueuedSkillsCompleted += (s, e) =>
                _eventBroker.Publish(new QueuedSkillsCompletedEvent(e.Character, e.CompletedSkills));

            // Collection events
            EveMonClient.CharacterCollectionChanged += (s, e) =>
                _eventBroker.Publish(new CharacterCollectionChangedEvent());

            EveMonClient.MonitoredCharacterCollectionChanged += (s, e) =>
                _eventBroker.Publish(new MonitoredCharacterCollectionChangedEvent());

            EveMonClient.CharacterImplantSetCollectionChanged += (s, e) =>
                _eventBroker.Publish(new CharacterImplantSetCollectionChangedEvent(e.Character));

            EveMonClient.CharacterPlanCollectionChanged += (s, e) =>
                _eventBroker.Publish(new CharacterPlanCollectionChangedEvent(e.Character));

            // Standings and faction
            EveMonClient.CharacterStandingsUpdated += (s, e) =>
                _eventBroker.Publish(new CharacterStandingsUpdatedEvent(e.Character));

            EveMonClient.CharacterFactionalWarfareStatsUpdated += (s, e) =>
                _eventBroker.Publish(new CharacterFactionalWarfareStatsUpdatedEvent(e.Character));

            // Assets
            EveMonClient.CharacterAssetsUpdated += (s, e) =>
                _eventBroker.Publish(new CharacterAssetsUpdatedEvent(e.Character));

            // Market orders
            EveMonClient.MarketOrdersUpdated += (s, e) =>
                _eventBroker.Publish(new MarketOrdersUpdatedEvent(e.Character));

            EveMonClient.CharacterMarketOrdersUpdated += (s, e) =>
                _eventBroker.Publish(new CharacterMarketOrdersUpdatedEvent(e.Character, e.EndedOrders));

            EveMonClient.CorporationMarketOrdersUpdated += (s, e) =>
                _eventBroker.Publish(new CorporationMarketOrdersUpdatedEvent(e.Character, e.EndedOrders));

            // Contracts
            EveMonClient.ContractsUpdated += (s, e) =>
                _eventBroker.Publish(new ContractsUpdatedEvent(e.Character));

            EveMonClient.CharacterContractsUpdated += (s, e) =>
                _eventBroker.Publish(new CharacterContractsUpdatedEvent(e.Character, e.EndedContracts));

            EveMonClient.CorporationContractsUpdated += (s, e) =>
                _eventBroker.Publish(new CorporationContractsUpdatedEvent(e.Character, e.EndedContracts));

            EveMonClient.CharacterContractBidsDownloaded += (s, e) =>
                _eventBroker.Publish(new CharacterContractBidsDownloadedEvent(e.Character));

            EveMonClient.CorporationContractBidsDownloaded += (s, e) =>
                _eventBroker.Publish(new CorporationContractBidsDownloadedEvent(e.Character));

            EveMonClient.CharacterContractItemsDownloaded += (s, e) =>
                _eventBroker.Publish(new CharacterContractItemsDownloadedEvent(e.Character));

            EveMonClient.CorporationContractItemsDownloaded += (s, e) =>
                _eventBroker.Publish(new CorporationContractItemsDownloadedEvent(e.Character));

            // Wallet
            EveMonClient.CharacterWalletJournalUpdated += (s, e) =>
                _eventBroker.Publish(new CharacterWalletJournalUpdatedEvent(e.Character));

            EveMonClient.CharacterWalletTransactionsUpdated += (s, e) =>
                _eventBroker.Publish(new CharacterWalletTransactionsUpdatedEvent(e.Character));

            // Industry
            EveMonClient.IndustryJobsUpdated += (s, e) =>
                _eventBroker.Publish(new IndustryJobsUpdatedEvent(e.Character));

            EveMonClient.CharacterIndustryJobsUpdated += (s, e) =>
                _eventBroker.Publish(new CharacterIndustryJobsUpdatedEvent(e.Character));

            EveMonClient.CorporationIndustryJobsUpdated += (s, e) =>
                _eventBroker.Publish(new CorporationIndustryJobsUpdatedEvent(e.Character));

            EveMonClient.CharacterIndustryJobsCompleted += (s, e) =>
                _eventBroker.Publish(new CharacterIndustryJobsCompletedEvent(e.Character, e.CompletedJobs));

            EveMonClient.CorporationIndustryJobsCompleted += (s, e) =>
                _eventBroker.Publish(new CorporationIndustryJobsCompletedEvent(e.Character, e.CompletedJobs));

            // Planetary
            EveMonClient.CharacterPlaneteryPinsCompleted += (s, e) =>
                _eventBroker.Publish(new CharacterPlanetaryPinsCompletedEvent(e.Character, e.CompletedPins));

            EveMonClient.CharacterPlanetaryColoniesUpdated += (s, e) =>
                _eventBroker.Publish(new CharacterPlanetaryColoniesUpdatedEvent(e.Character));

            EveMonClient.CharacterPlanetaryLayoutUpdated += (s, e) =>
                _eventBroker.Publish(new CharacterPlanetaryLayoutUpdatedEvent(e.Character));

            // Research
            EveMonClient.CharacterResearchPointsUpdated += (s, e) =>
                _eventBroker.Publish(new CharacterResearchPointsUpdatedEvent(e.Character));

            // Mail
            EveMonClient.CharacterEVEMailMessagesUpdated += (s, e) =>
                _eventBroker.Publish(new CharacterEVEMailMessagesUpdatedEvent(e.Character));

            EveMonClient.CharacterEVEMailingListsUpdated += (s, e) =>
                _eventBroker.Publish(new CharacterEVEMailingListsUpdatedEvent(e.Character));

            EveMonClient.CharacterEVEMailBodyDownloaded += (s, e) =>
                _eventBroker.Publish(new CharacterEVEMailBodyDownloadedEvent(e.Character));

            // Notifications
            EveMonClient.CharacterEVENotificationsUpdated += (s, e) =>
                _eventBroker.Publish(new CharacterEVENotificationsUpdatedEvent(e.Character));

            // Contacts
            EveMonClient.CharacterContactsUpdated += (s, e) =>
                _eventBroker.Publish(new CharacterContactsUpdatedEvent(e.Character));

            // Medals
            EveMonClient.CharacterMedalsUpdated += (s, e) =>
                _eventBroker.Publish(new CharacterMedalsUpdatedEvent(e.Character));

            EveMonClient.CorporationMedalsUpdated += (s, e) =>
                _eventBroker.Publish(new CorporationMedalsUpdatedEvent(e.Character));

            // Calendar
            EveMonClient.CharacterUpcomingCalendarEventsUpdated += (s, e) =>
                _eventBroker.Publish(new CharacterUpcomingCalendarEventsUpdatedEvent(e.Character));

            EveMonClient.CharacterCalendarEventAttendeesDownloaded += (s, e) =>
                _eventBroker.Publish(new CharacterCalendarEventAttendeesDownloadedEvent(e.Character));

            // Kill log
            EveMonClient.CharacterKillLogUpdated += (s, e) =>
                _eventBroker.Publish(new CharacterKillLogUpdatedEvent(e.Character));

            // Loyalty points
            EveMonClient.CharacterLoyaltyPointsUpdated += (s, e) =>
                _eventBroker.Publish(new CharacterLoyaltyPointsUpdatedEvent(e.Character));
        }

        #endregion

        #region ESI Events

        private static void BridgeESIEvents()
        {
            EveMonClient.ESIKeyCollectionChanged += (s, e) =>
                _eventBroker.Publish(new ESIKeyCollectionChangedEvent());

            EveMonClient.ESIKeyMonitoredChanged += (s, e) =>
                _eventBroker.Publish(new ESIKeyMonitoredChangedEvent());

            EveMonClient.ESIKeyInfoUpdated += (s, e) =>
                _eventBroker.Publish(new ESIKeyInfoUpdatedEvent(null)); // Legacy event doesn't pass the key

            EveMonClient.AccountStatusUpdated += (s, e) =>
                _eventBroker.Publish(new AccountStatusUpdatedEvent(null)); // Legacy event doesn't pass the key

            EveMonClient.CharacterListUpdated += (s, e) =>
                _eventBroker.Publish(new CharacterListUpdatedEvent(e.ESIKey));
        }

        #endregion

        #region Server Events

        private static void BridgeServerEvents()
        {
            EveMonClient.ServerStatusUpdated += (s, e) =>
                _eventBroker.Publish(new ServerStatusUpdatedEvent(e.Server, e.PreviousStatus, e.Status));

            EveMonClient.ConquerableStationListUpdated += (s, e) =>
                _eventBroker.Publish(new ConquerableStationListUpdatedEvent());

            EveMonClient.EveFactionalWarfareStatsUpdated += (s, e) =>
                _eventBroker.Publish(new EveFactionalWarfareStatsUpdatedEvent());
        }

        #endregion

        #region Notification Events

        private static void BridgeNotificationEvents()
        {
            EveMonClient.NotificationSent += (s, e) =>
                _eventBroker.Publish(new NotificationSentEvent(e));

            EveMonClient.NotificationInvalidated += (s, e) =>
                _eventBroker.Publish(new NotificationInvalidatedEvent(e));
        }

        #endregion

        #region Plan Events

        private static void BridgePlanEvents()
        {
            EveMonClient.PlanChanged += (s, e) =>
                _eventBroker.Publish(new PlanChangedEvent(e.Plan));

            EveMonClient.PlanNameChanged += (s, e) =>
                _eventBroker.Publish(new PlanNameChangedEvent(e.Plan));
        }

        #endregion

        #region Global Data Events

        private static void BridgeGlobalDataEvents()
        {
            EveMonClient.EveIDToNameUpdated += (s, e) =>
                _eventBroker.Publish(new EveIDToNameUpdatedEvent());

            EveMonClient.RefTypesUpdated += (s, e) =>
                _eventBroker.Publish(new RefTypesUpdatedEvent());

            EveMonClient.NotificationRefTypesUpdated += (s, e) =>
                _eventBroker.Publish(new NotificationRefTypesUpdatedEvent());

            EveMonClient.EveFlagsUpdated += (s, e) =>
                _eventBroker.Publish(new EveFlagsUpdatedEvent());

            EveMonClient.ItemPricesUpdated += (s, e) =>
                _eventBroker.Publish(new ItemPricesUpdatedEvent());
        }

        #endregion

        #region Update Events

        private static void BridgeUpdateEvents()
        {
            EveMonClient.UpdateAvailable += (s, e) =>
                _eventBroker.Publish(new UpdateAvailableEvent(
                    e.ForumUrl,
                    e.InstallerUrl,
                    e.UpdateMessage,
                    e.CurrentVersion,
                    e.NewestVersion,
                    e.MD5Sum,
                    e.CanAutoInstall,
                    e.AutoInstallArguments));

            EveMonClient.DataUpdateAvailable += (s, e) =>
                _eventBroker.Publish(new DataUpdateAvailableEvent(e.ChangedFiles.Cast<object>().ToList().AsReadOnly()));

            EveMonClient.LoadoutFeedUpdated += (s, e) =>
                _eventBroker.Publish(new LoadoutFeedUpdatedEvent(e.LoadoutFeed, e.Error?.Message));

            EveMonClient.LoadoutUpdated += (s, e) =>
                _eventBroker.Publish(new LoadoutUpdatedEvent(e.Loadout, e.Error?.Message));
        }

        #endregion
    }
}
