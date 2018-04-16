using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Helpers;
using EVEMon.Common.Interfaces;
using EVEMon.Common.Models;
using EVEMon.Common.Net;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Service;
using EVEMon.Common.Serialization.Esi;

namespace EVEMon.Common.QueryMonitor
{
    internal sealed class CharacterDataQuerying
    {
        #region Fields

        private readonly CharacterQueryMonitor<EsiAPICharacterSheet> m_charSheetMonitor;
        private readonly CharacterQueryMonitor<EsiAPIMarketOrders> m_charMarketOrdersMonitor;
        private readonly CharacterQueryMonitor<EsiAPIContracts> m_charContractsMonitor;
        private readonly CharacterQueryMonitor<EsiAPIIndustryJobs> m_charIndustryJobsMonitor;
        private readonly List<IQueryMonitorEx> m_characterQueryMonitors;
        private readonly List<IQueryMonitorEx> m_basicFeaturesMonitors;
        private readonly CCPCharacter m_ccpCharacter;

        #endregion


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterDataQuerying"/> class.
        /// </summary>
        /// <param name="ccpCharacter">The CCP character.</param>
        internal CharacterDataQuerying(CCPCharacter ccpCharacter)
        {
            // Initializes the query monitors 
            m_ccpCharacter = ccpCharacter;
            m_characterQueryMonitors = new List<IQueryMonitorEx>();

            m_charSheetMonitor = new CharacterQueryMonitor<EsiAPICharacterSheet>(ccpCharacter,
                ESIAPICharacterMethods.CharacterSheet, OnCharacterSheetUpdated);
            IQueryMonitorEx skillQueueMonitor = new CharacterQueryMonitor<EsiAPISkillQueue>(ccpCharacter,
                ESIAPICharacterMethods.SkillQueue, OnSkillQueueUpdated);
            IQueryMonitorEx charStandingsMonitor = new CharacterQueryMonitor<EsiAPIStandings>(ccpCharacter,
                ESIAPICharacterMethods.Standings, OnStandingsUpdated)
            { QueryOnStartup = true };
            IQueryMonitorEx charContactsMonitor = new CharacterQueryMonitor<EsiAPIContactsList>(ccpCharacter,
                ESIAPICharacterMethods.ContactList, OnContactsUpdated)
            { QueryOnStartup = true };
            IQueryMonitorEx charFacWarStatsMonitor = new CharacterQueryMonitor<EsiAPIFactionalWarfareStats>(ccpCharacter,
                ESIAPICharacterMethods.FactionalWarfareStats, OnFactionalWarfareStatsUpdated)
            { QueryOnStartup = true };
            IQueryMonitorEx charMedalsMonitor = new CharacterQueryMonitor<EsiAPIMedals>(ccpCharacter,
                ESIAPICharacterMethods.Medals, OnMedalsUpdated)
            { QueryOnStartup = true };
            IQueryMonitorEx charKillLogMonitor = new CharacterQueryMonitor<EsiAPIKillLog>(ccpCharacter,
                ESIAPICharacterMethods.KillLog, OnKillLogUpdated)
            { QueryOnStartup = true };
            IQueryMonitorEx charAssetsMonitor = new CharacterQueryMonitor<EsiAPIAssetList>(ccpCharacter,
                ESIAPICharacterMethods.AssetList, OnAssetsUpdated)
            { QueryOnStartup = true };
            m_charMarketOrdersMonitor = new CharacterQueryMonitor<EsiAPIMarketOrders>(ccpCharacter,
                ESIAPICharacterMethods.MarketOrders, OnMarketOrdersUpdated)
            { QueryOnStartup = true };
            m_charContractsMonitor = new CharacterQueryMonitor<EsiAPIContracts>(ccpCharacter,
                ESIAPICharacterMethods.Contracts, OnContractsUpdated)
            { QueryOnStartup = true };
            IQueryMonitorEx charWalletJournalMonitor = new CharacterQueryMonitor<EsiAPIWalletJournal>(ccpCharacter,
                ESIAPICharacterMethods.WalletJournal, OnWalletJournalUpdated)
            { QueryOnStartup = true };
            IQueryMonitorEx charWalletTransactionsMonitor = new CharacterQueryMonitor<EsiAPIWalletTransactions>(ccpCharacter,
                ESIAPICharacterMethods.WalletTransactions, OnWalletTransactionsUpdated)
            { QueryOnStartup = true };
            m_charIndustryJobsMonitor = new CharacterQueryMonitor<EsiAPIIndustryJobs>(ccpCharacter,
                ESIAPICharacterMethods.IndustryJobs, OnIndustryJobsUpdated)
            { QueryOnStartup = true };
            IQueryMonitorEx charResearchPointsMonitor = new CharacterQueryMonitor<EsiAPIResearchPoints>(ccpCharacter,
                ESIAPICharacterMethods.ResearchPoints, OnResearchPointsUpdated)
            { QueryOnStartup = true };
            IQueryMonitorEx charEVEMailMessagesMonitor = new CharacterQueryMonitor<EsiAPIMailMessages>(ccpCharacter,
                ESIAPICharacterMethods.MailMessages, OnEVEMailMessagesUpdated)
            { QueryOnStartup = true };
            IQueryMonitorEx charEVENotificationsMonitor = new CharacterQueryMonitor<EsiAPINotifications>(ccpCharacter,
                ESIAPICharacterMethods.Notifications, OnEVENotificationsUpdated)
            { QueryOnStartup = true };
            IQueryMonitorEx charUpcomingCalendarEventsMonitor = new CharacterQueryMonitor<EsiAPICalendarEvents>(ccpCharacter,
                ESIAPICharacterMethods.UpcomingCalendarEvents, OnUpcomingCalendarEventsUpdated)
            { QueryOnStartup = true };
            IQueryMonitorEx charPlanetaryColoniesMonitor = new CharacterQueryMonitor<EsiAPIPlanetaryColoniesList>(ccpCharacter,
                ESIAPICharacterMethods.PlanetaryColonies, OnPlanetaryColoniesUpdated)
            { QueryOnStartup = true };

            // Add the monitors in an order as they will appear in the throbber menu
            m_characterQueryMonitors.AddRange(new[]
            {
                m_charSheetMonitor,
                skillQueueMonitor,
                charStandingsMonitor,
                charContactsMonitor,
                charFacWarStatsMonitor,
                charMedalsMonitor,
                charKillLogMonitor,
                charAssetsMonitor,
                m_charMarketOrdersMonitor,
                m_charContractsMonitor,
                charWalletJournalMonitor,
                charWalletTransactionsMonitor,
                m_charIndustryJobsMonitor,
                charPlanetaryColoniesMonitor,
                charResearchPointsMonitor,
                charEVEMailMessagesMonitor,
                charEVENotificationsMonitor,
                charUpcomingCalendarEventsMonitor
            });

            m_characterQueryMonitors.ForEach(monitor => ccpCharacter.QueryMonitors.Add(monitor));

            m_basicFeaturesMonitors = m_characterQueryMonitors
                .Select(monitor =>
                    new
                    {
                        monitor,
                        method = (ESIAPICharacterMethods)monitor.Method
                    })
                .Where(monitor =>
                    (long)monitor.method == ((long)monitor.method & (long)CCPAPIMethodsEnum.BasicCharacterFeatures))
                .Select(basicFeature => basicFeature.monitor)
                .ToList();

            if (ccpCharacter.ForceUpdateBasicFeatures)
                m_basicFeaturesMonitors.ForEach(monitor => monitor.ForceUpdate(true));

            EveMonClient.TimerTick += EveMonClient_TimerTick;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets the character sheet monitor.
        /// </summary>
        /// <value>The character sheet monitor.</value>
        internal CharacterQueryMonitor<EsiAPICharacterSheet> CharacterSheetMonitor => m_charSheetMonitor;

        /// <summary>
        /// Gets or sets a value indicating whether the character market orders have been queried.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the character market orders have been queried; otherwise, <c>false</c>.
        /// </value>
        internal bool CharacterMarketOrdersQueried => !m_charMarketOrdersMonitor.IsUpdating;

        /// <summary>
        /// Gets or sets a value indicating whether the character contracts have been queried.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the character contracts have been queried; otherwise, <c>false</c>.
        /// </value>
        internal bool CharacterContractsQueried => !m_charContractsMonitor.IsUpdating;

        /// <summary>
        /// Gets or sets a value indicating whether the character industry jobs have been queried.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the character industry jobs have been queried; otherwise, <c>false</c>.
        /// </value>
        internal bool CharacterIndustryJobsQueried => !m_charIndustryJobsMonitor.IsUpdating;

        #endregion


        #region Dispose

        /// <summary>
        /// Called when the object gets disposed.
        /// </summary>
        internal void Dispose()
        {
            EveMonClient.TimerTick -= EveMonClient_TimerTick;

            // Unsubscribe events in monitors
            foreach (IQueryMonitorEx monitor in m_characterQueryMonitors)
            {
                monitor.Dispose();
            }
        }

        #endregion


        #region Querying

        /// <summary>
        /// Queries the character's data.
        /// </summary>
        private void QueryCharacterData<T>(ESIAPICharacterMethods targetMethod,
            APIProvider.ESIRequestCallback<T> onUpdated) where T : class
        {
            // Quits if no network
            if (!NetworkMonitor.IsNetworkAvailable)
                return;

            // Quits if access denied
            ESIKey apiKey = m_ccpCharacter.Identity.FindAPIKeyWithAccess(targetMethod);
            if (apiKey == null)
                return;

            EveMonClient.APIProviders.CurrentProvider.QueryEsiAsync<T>(
                targetMethod, apiKey.AccessToken, m_ccpCharacter.CharacterID, onUpdated);
        }
        
        /// <summary>
        /// Queries the character's contract bids.
        /// </summary>
        private void QueryCharacterContractBids(long forContract)
        {
            // Quits if no network
            if (!NetworkMonitor.IsNetworkAvailable)
                return;

            // Quits if access denied
            ESIKey apiKey = m_ccpCharacter.Identity.FindAPIKeyWithAccess(ESIAPICharacterMethods.Contracts);
            if (apiKey == null)
                return;

            EveMonClient.APIProviders.CurrentProvider.QueryEsiAsync<EsiAPIContractBids>(
                ESIAPICharacterMethods.ContractBids, apiKey.AccessToken,
                m_ccpCharacter.CharacterID, forContract, OnContractBidsUpdated, forContract);
        }
        
        /// <summary>
        /// Processed the queried character's character sheet information.
        /// </summary>
        /// <param name="result"></param>
        private void OnCharacterSheetUpdated(EsiResult<EsiAPICharacterSheet> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occurred
            if (m_ccpCharacter.ShouldNotifyError(result, ESIAPICharacterMethods.CharacterSheet))
                EveMonClient.Notifications.NotifyCharacterSheetError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Query the Character's data
            QueryCharacterData<EsiAPILocation>(ESIAPICharacterMethods.Location,
                OnCharacterLocationUpdated);
            QueryCharacterData<EsiAPIClones>(ESIAPICharacterMethods.Clones,
                OnCharacterClonesUpdated);
            QueryCharacterData<EsiAPIJumpFatigue>(ESIAPICharacterMethods.JumpFatigue,
                OnCharacterFatigueUpdated);
            QueryCharacterData<EsiAPIAttributes>(ESIAPICharacterMethods.Attributes,
                OnCharacterAttributesUpdated);
            QueryCharacterData<EsiAPIShip>(ESIAPICharacterMethods.Ship,
                OnCharacterShipUpdated);
            QueryCharacterData<EsiAPISkills>(ESIAPICharacterMethods.Skills,
                OnCharacterSkillsUpdated);
            QueryCharacterData<List<int>>(ESIAPICharacterMethods.Implants,
                OnCharacterImplantsUpdated);

            // Imports the data
            m_ccpCharacter.Import(result);

            // Notify for insufficient balance
            m_ccpCharacter.NotifyInsufficientBalance();

            // Save the file to our cache
            LocalXmlCache.SaveAsync(result.Result.Name, Util.SerializeToXmlDocument(result.Result)).ConfigureAwait(false);
        }

        /// <summary>
        /// Processes the queried character's location.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnCharacterLocationUpdated(EsiResult<EsiAPILocation> result, object ignore)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occured
            if (m_ccpCharacter.ShouldNotifyError(result, ESIAPICharacterMethods.Location))
                EveMonClient.Notifications.NotifyCharacterLocationError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            m_ccpCharacter.Import(result);
        }

        /// <summary>
        /// Processes the queried character's ship.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnCharacterShipUpdated(EsiResult<EsiAPIShip> result, object ignore)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occured
            if (m_ccpCharacter.ShouldNotifyError(result, ESIAPICharacterMethods.Ship))
                EveMonClient.Notifications.NotifyCharacterShipError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            m_ccpCharacter.Import(result);
        }

        /// <summary>
        /// Processes the queried character's clones.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnCharacterClonesUpdated(EsiResult<EsiAPIClones> result, object ignore)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occured
            if (m_ccpCharacter.ShouldNotifyError(result, ESIAPICharacterMethods.Clones))
                EveMonClient.Notifications.NotifyCharacterClonesError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            m_ccpCharacter.Import(result);
        }

        /// <summary>
        /// Processes the queried character's jump fatigue.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnCharacterFatigueUpdated(EsiResult<EsiAPIJumpFatigue> result, object ignore)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occured
            if (m_ccpCharacter.ShouldNotifyError(result, ESIAPICharacterMethods.JumpFatigue))
                EveMonClient.Notifications.NotifyCharacterFatigueError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            m_ccpCharacter.Import(result);
        }

        /// <summary>
        /// Processes the queried character's attributes.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnCharacterAttributesUpdated(EsiResult<EsiAPIAttributes> result, object ignore)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occured
            if (m_ccpCharacter.ShouldNotifyError(result, ESIAPICharacterMethods.Attributes))
                EveMonClient.Notifications.NotifyCharacterAttributesError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            m_ccpCharacter.Import(result);
        }

        /// <summary>
        /// Processes the queried character's implants.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnCharacterImplantsUpdated(EsiResult<List<int>> result, object ignore)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occured
            if (m_ccpCharacter.ShouldNotifyError(result, ESIAPICharacterMethods.Implants))
                EveMonClient.Notifications.NotifyCharacterImplantsError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            m_ccpCharacter.Import(result);
        }

        /// <summary>
        /// Processes the queried character's skills.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnCharacterSkillsUpdated(EsiResult<EsiAPISkills> result, object ignore)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occured
            if (m_ccpCharacter.ShouldNotifyError(result, ESIAPICharacterMethods.Skills))
                EveMonClient.Notifications.NotifyCharacterSkillsError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            m_ccpCharacter.Import(result);
        }

        /// <summary>
        /// Processes the queried character's skill queue information.
        /// </summary>
        /// <param name="result"></param>
        private void OnSkillQueueUpdated(EsiResult<EsiAPISkillQueue> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occurred
            if (m_ccpCharacter.ShouldNotifyError(result, ESIAPICharacterMethods.SkillQueue))
                EveMonClient.Notifications.NotifySkillQueueError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            m_ccpCharacter.SkillQueue.Import(result.Result.ToXMLItem().Queue);
            
            // Check the character has less than a day of training in skill queue
            if (m_ccpCharacter.IsTraining && m_ccpCharacter.SkillQueue.LessThanWarningThreshold)
            {
                EveMonClient.Notifications.NotifySkillQueueLessThanADay(m_ccpCharacter);
                return;
            }

            EveMonClient.Notifications.InvalidateSkillQueueLessThanADay(m_ccpCharacter);
        }

        /// <summary>
        /// Processes the queried character's standings information.
        /// </summary>
        /// <param name="result"></param>
        private void OnStandingsUpdated(EsiResult<EsiAPIStandings> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occurred
            if (m_ccpCharacter.ShouldNotifyError(result, ESIAPICharacterMethods.Standings))
                EveMonClient.Notifications.NotifyCharacterStandingsError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            m_ccpCharacter.Standings.Import(result.Result.ToXMLItem().CharacterNPCStandings.All);

            // Fires the event regarding standings update
            EveMonClient.OnCharacterStandingsUpdated(m_ccpCharacter);
        }

        /// <summary>
        /// Processes the queried character's factional warfare statistic information.
        /// </summary>
        /// <param name="result"></param>
        private void OnFactionalWarfareStatsUpdated(EsiResult<EsiAPIFactionalWarfareStats> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occurred
            if (m_ccpCharacter.ShouldNotifyError(result, ESIAPICharacterMethods.FactionalWarfareStats))
                EveMonClient.Notifications.NotifyCharacterFactionalWarfareStatsError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
            {
                // Update the enlisted in factional warfare flag
                m_ccpCharacter.IsFactionalWarfareNotEnlisted = true;

                // Fires the event regarding factional warfare stats update
                EveMonClient.OnCharacterFactionalWarfareStatsUpdated(m_ccpCharacter);
                return;
            }

            // Update the enlisted in factional warfare flag
            m_ccpCharacter.IsFactionalWarfareNotEnlisted = false;

            // Import the data
            m_ccpCharacter.FactionalWarfareStats = new FactionalWarfareStats(result.Result.ToXMLItem());

            // Fires the event regarding factional warfare stats update
            EveMonClient.OnCharacterFactionalWarfareStatsUpdated(m_ccpCharacter);
        }

        /// <summary>
        /// Processes the queried character's assets information.
        /// </summary>
        /// <param name="result"></param>
        private void OnAssetsUpdated(EsiResult<EsiAPIAssetList> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occurred
            if (m_ccpCharacter.ShouldNotifyError(result, ESIAPICharacterMethods.AssetList))
                EveMonClient.Notifications.NotifyCharacterAssetsError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            TaskHelper.RunCPUBoundTaskAsync(() => m_ccpCharacter.Assets.Import(result.Result.ToXMLItem().Assets))
                .ContinueWith(_ =>
                {
                    // Fires the event regarding assets update
                    EveMonClient.OnCharacterAssetsUpdated(m_ccpCharacter);

                }, EveMonClient.CurrentSynchronizationContext);
        }

        /// <summary>
        /// Processes the queried character's market orders.
        /// </summary>
        /// <param name="result"></param>
        /// <remarks>This method is sensitive to which "issued for" orders gets queried first</remarks>
        private void OnMarketOrdersUpdated(EsiResult<EsiAPIMarketOrders> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occurred
            if (m_ccpCharacter.ShouldNotifyError(result, ESIAPICharacterMethods.MarketOrders))
                EveMonClient.Notifications.NotifyCharacterMarketOrdersError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            var orders = result.Result.ToXMLItem(m_ccpCharacter.CharacterID).Orders;
            foreach (var order in orders)
                order.IssuedFor = IssuedFor.Character;

            // Import the data
            var endedOrders = new List<MarketOrder>();
            m_ccpCharacter.CharacterMarketOrders.Import(orders, endedOrders);

            // Fires the event regarding character market orders update
            EveMonClient.OnCharacterMarketOrdersUpdated(m_ccpCharacter, endedOrders);
        }

        /// <summary>
        /// Processes the queried character's contracts.
        /// </summary>
        /// <param name="result"></param>
        /// <remarks>This method is sensitive to which "issued for" contracts gets queried first</remarks>
        private void OnContractsUpdated(EsiResult<EsiAPIContracts> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occurred
            if (m_ccpCharacter.ShouldNotifyError(result, ESIAPICharacterMethods.Contracts))
                EveMonClient.Notifications.NotifyCharacterContractsError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            var contracts = result.Result.ToXMLItem().Contracts;
            foreach (var contract in contracts)
            {
                contract.APIMethod = ESIAPICharacterMethods.Contracts;
                contract.IssuedFor = IssuedFor.Character;

                // Fetch contract bids for auctions only
                if (contract.Type.Equals("auction", StringComparison.InvariantCultureIgnoreCase))
                    QueryCharacterContractBids(contract.ContractID);
            }

            // Import the data
            List<Contract> endedContracts = new List<Contract>();
            m_ccpCharacter.CharacterContracts.Import(contracts, endedContracts);

            // Fires the event regarding character contracts update
            EveMonClient.OnCharacterContractsUpdated(m_ccpCharacter, endedContracts);
        }

        /// <summary>
        /// Processes the queried character's contract bids.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnContractBidsUpdated(EsiResult<EsiAPIContractBids> result, object forContract)
        {
            long contractID = (forContract as long?) ?? 0L;

            // Character may have been deleted or set to not be monitored since we queried
            if (contractID == 0L || m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occured
            if (m_ccpCharacter.ShouldNotifyError(result, ESIAPICharacterMethods.ContractBids))
                EveMonClient.Notifications.NotifyCharacterContractBidsError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            m_ccpCharacter.CharacterContractBids.Import(result.Result.ToXMLItem(contractID).
                ContractBids);

            // Fires the event regarding character contract bids update
            EveMonClient.OnCharacterContractBidsUpdated(m_ccpCharacter);
        }

        /// <summary>
        /// Processes the queried character's wallet journal information.
        /// </summary>
        /// <param name="result"></param>
        private void OnWalletJournalUpdated(EsiResult<EsiAPIWalletJournal> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occurred
            if (m_ccpCharacter.ShouldNotifyError(result, ESIAPICharacterMethods.WalletJournal))
                EveMonClient.Notifications.NotifyCharacterWalletJournalError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            m_ccpCharacter.WalletJournal.Import(result.Result.ToXMLItem().WalletJournal);

            // Fires the event regarding wallet journal update
            EveMonClient.OnCharacterWalletJournalUpdated(m_ccpCharacter);
        }

        /// <summary>
        /// Processes the queried character's wallet transactions information.
        /// </summary>
        /// <param name="result"></param>
        private void OnWalletTransactionsUpdated(EsiResult<EsiAPIWalletTransactions> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occurred
            if (m_ccpCharacter.ShouldNotifyError(result, ESIAPICharacterMethods.WalletTransactions))
                EveMonClient.Notifications.NotifyCharacterWalletTransactionsError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            m_ccpCharacter.WalletTransactions.Import(result.Result.ToXMLItem().WalletTransactions);

            // Fires the event regarding wallet transactions update
            EveMonClient.OnCharacterWalletTransactionsUpdated(m_ccpCharacter);
        }

        /// <summary>
        /// Processes the queried character's personal industry jobs.
        /// </summary>
        /// <param name="result"></param>
        /// <remarks>This method is sensitive to which "issued for" jobs gets queried first</remarks>
        private void OnIndustryJobsUpdated(EsiResult<EsiAPIIndustryJobs> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occurred
            if (m_ccpCharacter.ShouldNotifyError(result, ESIAPICharacterMethods.IndustryJobs))
                EveMonClient.Notifications.NotifyCharacterIndustryJobsError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            var jobs = result.Result.ToXMLItem().Jobs;
            foreach (var job in jobs)
                job.IssuedFor = IssuedFor.Character;

            // Import the data
            m_ccpCharacter.CharacterIndustryJobs.Import(jobs);

            // Fires the event regarding character industry jobs update
            EveMonClient.OnCharacterIndustryJobsUpdated(m_ccpCharacter);
        }

        /// <summary>
        /// Processes the queried character's research points.
        /// </summary>
        /// <param name="result"></param>
        private void OnResearchPointsUpdated(EsiResult<EsiAPIResearchPoints> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occured
            if (m_ccpCharacter.ShouldNotifyError(result, ESIAPICharacterMethods.ResearchPoints))
                EveMonClient.Notifications.NotifyResearchPointsError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            m_ccpCharacter.ResearchPoints.Import(result.Result.ToXMLItem().ResearchPoints);

            // Fires the event regarding research points update
            EveMonClient.OnCharacterResearchPointsUpdated(m_ccpCharacter);
        }

        /// <summary>
        /// Processes the queried character's EVE mail messages.
        /// </summary>
        /// <param name="result"></param>
        private void OnEVEMailMessagesUpdated(EsiResult<EsiAPIMailMessages> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occured
            if (m_ccpCharacter.ShouldNotifyError(result, ESIAPICharacterMethods.MailMessages))
                EveMonClient.Notifications.NotifyEVEMailMessagesError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Each time we import a new batch of EVE mail messages,
            // query the mailing lists so that we are always up to date
            QueryCharacterData<EsiAPIMailingLists>(ESIAPICharacterMethods.MailingLists,
                OnMailingListsUpdated);

            // Import the data
            m_ccpCharacter.EVEMailMessages.Import(result.Result.ToXMLItem().Messages);

            // Notify on new messages
            if (m_ccpCharacter.EVEMailMessages.NewMessages != 0)
                EveMonClient.Notifications.NotifyNewEVEMailMessages(m_ccpCharacter, m_ccpCharacter.EVEMailMessages.NewMessages);
        }

        /// <summary>
        /// Processes the queried character's EVE mailing lists.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnMailingListsUpdated(EsiResult<EsiAPIMailingLists> result, object ignore)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occured
            if (m_ccpCharacter.ShouldNotifyError(result, ESIAPICharacterMethods.MailingLists))
                EveMonClient.Notifications.NotifyMailingListsError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            m_ccpCharacter.EVEMailingLists.Import(result.Result.ToXMLItem().MailingLists);
        }

        /// <summary>
        /// Processes the queried character's EVE notifications.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnEVENotificationsUpdated(EsiResult<EsiAPINotifications> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occured
            if (m_ccpCharacter.ShouldNotifyError(result, ESIAPICharacterMethods.Notifications))
                EveMonClient.Notifications.NotifyEVENotificationsError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            m_ccpCharacter.EVENotifications.Import(result.Result);

            // Notify on new notifications
            if (m_ccpCharacter.EVENotifications.NewNotifications != 0)
                EveMonClient.Notifications.NotifyNewEVENotifications(m_ccpCharacter,
                                                                     m_ccpCharacter.EVENotifications.NewNotifications);
        }

        /// <summary>
        /// Processes the queried character's contact list.
        /// </summary>
        /// <param name="result"></param>
        private void OnContactsUpdated(EsiResult<EsiAPIContactsList> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occurred
            if (m_ccpCharacter.ShouldNotifyError(result, ESIAPICharacterMethods.ContactList))
                EveMonClient.Notifications.NotifyCharacterContactsError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            // TODO Corp and alliance contacts
            m_ccpCharacter.Contacts.Import(result.Result.ToXMLItem().AllContacts);

            // Fires the event regarding contacts update
            EveMonClient.OnCharacterContactsUpdated(m_ccpCharacter);
        }

        /// <summary>
        /// Processes the queried character's medals.
        /// </summary>
        /// <param name="result"></param>
        private void OnMedalsUpdated(EsiResult<EsiAPIMedals> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occurred
            if (m_ccpCharacter.ShouldNotifyError(result, ESIAPICharacterMethods.Medals))
                EveMonClient.Notifications.NotifyCharacterMedalsError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            m_ccpCharacter.CharacterMedals.Import(result.Result.ToXMLItem().CorporationMedals);

            // Fires the event regarding medals update
            EveMonClient.OnCharacterMedalsUpdated(m_ccpCharacter);
        }

        /// <summary>
        /// Processes the queried character's kill log.
        /// </summary>
        /// <param name="result"></param>
        private void OnKillLogUpdated(EsiResult<EsiAPIKillLog> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occurred
            if (m_ccpCharacter.ShouldNotifyError(result, ESIAPICharacterMethods.KillLog))
                EveMonClient.Notifications.NotifyCharacterKillLogError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            var kills = result.Result;
            // TODO m_ccpCharacter.KillLog.Import(kills.Kills);

            // Fires the event regarding kill log update
            EveMonClient.OnCharacterKillLogUpdated(m_ccpCharacter);

            // Save the file to our cache
            string filename = $"{m_ccpCharacter.Name}-{ESIAPICharacterMethods.KillLog}";
            LocalXmlCache.SaveAsync(filename, Util.SerializeToXmlDocument(kills)).ConfigureAwait(false);
        }

        /// <summary>
        /// Processes the queried character's upcoming calendar events.
        /// </summary>
        /// <param name="result"></param>
        private void OnUpcomingCalendarEventsUpdated(EsiResult<EsiAPICalendarEvents> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occurred
            if (m_ccpCharacter.ShouldNotifyError(result, ESIAPICharacterMethods.UpcomingCalendarEvents))
                EveMonClient.Notifications.NotifyCharacterUpcomingCalendarEventsError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            // TODO m_ccpCharacter.UpcomingCalendarEvents.Import(result.Result);

            // Fires the event regarding upcoming calendar events update
            EveMonClient.OnCharacterUpcomingCalendarEventsUpdated(m_ccpCharacter);
        }

        /// <summary>
        /// Processes the queried character's palnetary colonies.
        /// </summary>
        /// <param name="result"></param>
        private void OnPlanetaryColoniesUpdated(EsiResult<EsiAPIPlanetaryColoniesList> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null || !m_ccpCharacter.Monitored)
                return;

            // Notify an error occurred
            if (m_ccpCharacter.ShouldNotifyError(result, ESIAPICharacterMethods.PlanetaryColonies))
                EveMonClient.Notifications.NotifyCharacterPlanetaryColoniesError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Invalidate previous notifications
            EveMonClient.Notifications.InvalidateCharacterPlanetaryPinCompleted(m_ccpCharacter);

            // Import the data
            m_ccpCharacter.PlanetaryColonies.Import(result.Result.ToXMLItem().Colonies);

            // Fires the event regarding planetary colonies update
            EveMonClient.OnCharacterPlanetaryColoniesUpdated(m_ccpCharacter);
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// Handles the TimerTick event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EveMonClient_TimerTick(object sender, EventArgs e)
        {
            // If character is monitored enable the basic feature monitoring
            m_basicFeaturesMonitors.ForEach(monitor => monitor.Enabled = m_ccpCharacter.Monitored);
        }

        #endregion
    }
}
