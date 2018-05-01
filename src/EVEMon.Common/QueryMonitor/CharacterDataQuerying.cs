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
            var notifiers = EveMonClient.Notifications;
            m_ccpCharacter = ccpCharacter;
            m_characterQueryMonitors = new List<IQueryMonitorEx>();

            // Add the monitors in an order as they will appear in the throbber menu
            m_charSheetMonitor = new CharacterQueryMonitor<EsiAPICharacterSheet>(ccpCharacter,
                ESIAPICharacterMethods.CharacterSheet, OnCharacterSheetUpdated,
                notifiers.NotifyCharacterSheetError);
            m_characterQueryMonitors.Add(m_charSheetMonitor);
            m_characterQueryMonitors.Add(new CharacterQueryMonitor<EsiAPISkillQueue>(
                ccpCharacter, ESIAPICharacterMethods.SkillQueue, OnSkillQueueUpdated,
                notifiers.NotifySkillQueueError));
            m_characterQueryMonitors.Add(new CharacterQueryMonitor<EsiAPIStandings>(
                ccpCharacter, ESIAPICharacterMethods.Standings, OnStandingsUpdated,
                notifiers.NotifyCharacterStandingsError) { QueryOnStartup = true });
            m_characterQueryMonitors.Add(new CharacterQueryMonitor<EsiAPIContactsList>(
                ccpCharacter, ESIAPICharacterMethods.ContactList, OnContactsUpdated,
                notifiers.NotifyCharacterContactsError) { QueryOnStartup = true });
            m_characterQueryMonitors.Add(new CharacterQueryMonitor<EsiAPIFactionalWarfareStats>(
                ccpCharacter, ESIAPICharacterMethods.FactionalWarfareStats,
                OnFactionalWarfareStatsUpdated, notifiers.
                NotifyCharacterFactionalWarfareStatsError) { QueryOnStartup = true });
            m_characterQueryMonitors.Add(new CharacterQueryMonitor<EsiAPIMedals>(ccpCharacter,
                ESIAPICharacterMethods.Medals, OnMedalsUpdated,
                notifiers.NotifyCharacterMedalsError) { QueryOnStartup = true });
            m_characterQueryMonitors.Add(new CharacterQueryMonitor<EsiAPIKillLog>(ccpCharacter,
                ESIAPICharacterMethods.KillLog, OnKillLogUpdated,
                notifiers.NotifyCharacterKillLogError) { QueryOnStartup = true });
            m_characterQueryMonitors.Add(new CharacterQueryMonitor<EsiAPIAssetList>(ccpCharacter,
                ESIAPICharacterMethods.AssetList, OnAssetsUpdated,
                notifiers.NotifyCharacterAssetsError) { QueryOnStartup = true });
            m_charMarketOrdersMonitor = new CharacterQueryMonitor<EsiAPIMarketOrders>(
                ccpCharacter, ESIAPICharacterMethods.MarketOrders, OnMarketOrdersUpdated,
                notifiers.NotifyCharacterMarketOrdersError) { QueryOnStartup = true };
            m_characterQueryMonitors.Add(m_charMarketOrdersMonitor);
            m_charContractsMonitor = new CharacterQueryMonitor<EsiAPIContracts>(ccpCharacter,
                ESIAPICharacterMethods.Contracts, OnContractsUpdated,
                notifiers.NotifyCharacterContractsError) { QueryOnStartup = true };
            m_characterQueryMonitors.Add(m_charContractsMonitor);
            m_characterQueryMonitors.Add(new CharacterQueryMonitor<EsiAPIWalletJournal>(
                ccpCharacter, ESIAPICharacterMethods.WalletJournal, OnWalletJournalUpdated,
                notifiers.NotifyCharacterWalletJournalError) { QueryOnStartup = true });
            m_characterQueryMonitors.Add(new CharacterQueryMonitor<EsiAPIWalletTransactions>(
                ccpCharacter, ESIAPICharacterMethods.WalletTransactions,
                OnWalletTransactionsUpdated, notifiers.NotifyCharacterWalletTransactionsError)
            { QueryOnStartup = true });
            m_charIndustryJobsMonitor = new CharacterQueryMonitor<EsiAPIIndustryJobs>(
                ccpCharacter, ESIAPICharacterMethods.IndustryJobs, OnIndustryJobsUpdated,
                notifiers.NotifyCharacterIndustryJobsError) { QueryOnStartup = true };
            m_characterQueryMonitors.Add(m_charIndustryJobsMonitor);
            m_characterQueryMonitors.Add(new CharacterQueryMonitor<EsiAPIResearchPoints>(
                ccpCharacter, ESIAPICharacterMethods.ResearchPoints, OnResearchPointsUpdated,
                notifiers.NotifyCharacterResearchPointsError) { QueryOnStartup = true });
            m_characterQueryMonitors.Add(new CharacterQueryMonitor<EsiAPIMailMessages>(
                ccpCharacter, ESIAPICharacterMethods.MailMessages, OnEVEMailMessagesUpdated,
                notifiers.NotifyEVEMailMessagesError) { QueryOnStartup = true });
            m_characterQueryMonitors.Add(new CharacterQueryMonitor<EsiAPINotifications>(
                ccpCharacter, ESIAPICharacterMethods.Notifications, OnEVENotificationsUpdated,
                notifiers.NotifyEVENotificationsError) { QueryOnStartup = true });
            m_characterQueryMonitors.Add(new CharacterQueryMonitor<EsiAPICalendarEvents>(
                ccpCharacter, ESIAPICharacterMethods.UpcomingCalendarEvents,
                OnUpcomingCalendarEventsUpdated, notifiers.
                NotifyCharacterUpcomingCalendarEventsError) { QueryOnStartup = true });
            m_characterQueryMonitors.Add(new CharacterQueryMonitor<EsiAPIPlanetaryColoniesList>(
                ccpCharacter, ESIAPICharacterMethods.PlanetaryColonies,
                OnPlanetaryColoniesUpdated, notifiers.
                NotifyCharacterPlanetaryColoniesError) { QueryOnStartup = true });
            m_characterQueryMonitors.ForEach(monitor => ccpCharacter.QueryMonitors.Add(monitor));

            // Enumerate the basic feature monitors into a separate list
            m_basicFeaturesMonitors = new List<IQueryMonitorEx>(m_characterQueryMonitors.Count);
            long basicFeatures = (long)CCPAPIMethodsEnum.BasicCharacterFeatures;
            foreach (var monitor in m_characterQueryMonitors)
            {
                long method = (long)(ESIAPICharacterMethods)monitor.Method;
                if (method == (method & basicFeatures))
                {
                    m_basicFeaturesMonitors.Add(monitor);
                    // If force update is selected, update basic features only
                    if (ccpCharacter.ForceUpdateBasicFeatures)
                        monitor.ForceUpdate(true);
                }
            }

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
            foreach (var monitor in m_characterQueryMonitors)
                monitor.Dispose();
        }

        #endregion


        #region Querying

        /// <summary>
        /// Queries the character's data. Used generically across multiple methods.
        /// </summary>
        /// <param name="targetMethod">The ESI method to use.</param>
        /// <param name="onError">The callback if an error occurs.</param>
        /// <param name="onSuccess">The callback if the request is successful.</param>
        private void QueryCharacterData<T>(ESIAPICharacterMethods targetMethod,
            CharacterQueryMonitor<T>.NotifyErrorCallback onError, Action<T> onSuccess)
            where T : class
        {
            ESIKey apiKey = m_ccpCharacter.Identity.FindAPIKeyWithAccess(targetMethod);

            // Network available, has access
            if (NetworkMonitor.IsNetworkAvailable && apiKey != null)
                EveMonClient.APIProviders.CurrentProvider.QueryEsiAsync<T>(targetMethod,
                    apiKey.AccessToken, m_ccpCharacter.CharacterID, (result, ignore) =>
                    {
                        var target = m_ccpCharacter;

                        // Character may have been deleted or set to not be monitored
                        if (target != null && target.Monitored)
                        {
                            // Notify an error occured
                            if (target.ShouldNotifyError(result, targetMethod))
                                onError.Invoke(target, result);
                            if (!result.HasError)
                                onSuccess.Invoke(result.Result);
                        }
                    });
        }
        
        /// <summary>
        /// Queries the character's contract bids.
        /// </summary>
        /// <param name="forContract">The contract ID to query.</param>
        private void QueryCharacterContractBids(long forContract)
        {
            ESIKey apiKey = m_ccpCharacter.Identity.FindAPIKeyWithAccess(
                ESIAPICharacterMethods.ContractBids);

            // Network available, has access
            if (NetworkMonitor.IsNetworkAvailable && apiKey != null)
                EveMonClient.APIProviders.CurrentProvider.QueryEsiAsync<EsiAPIContractBids>(
                    ESIAPICharacterMethods.ContractBids, apiKey.AccessToken,
                    m_ccpCharacter.CharacterID, forContract, OnContractBidsUpdated, forContract);
        }

        /// <summary>
        /// Processes the queried character's character sheet information.
        /// </summary>
        /// <param name="result"></param>
        private void OnCharacterSheetUpdated(EsiAPICharacterSheet result)
        {
            var target = m_ccpCharacter;

            // Character may have been deleted since we queried
            if (target != null)
            {
                var notifiers = EveMonClient.Notifications;

                // Query remaining character data
                QueryCharacterData<EsiAPILocation>(ESIAPICharacterMethods.Location,
                    notifiers.NotifyCharacterLocationError, target.Import);
                QueryCharacterData<EsiAPIClones>(ESIAPICharacterMethods.Clones,
                    notifiers.NotifyCharacterClonesError, target.Import);
                QueryCharacterData<EsiAPIJumpFatigue>(ESIAPICharacterMethods.JumpFatigue,
                    notifiers.NotifyCharacterFatigueError, target.Import);
                QueryCharacterData<EsiAPIAttributes>(ESIAPICharacterMethods.Attributes,
                    notifiers.NotifyCharacterAttributesError, target.Import);
                QueryCharacterData<EsiAPIShip>(ESIAPICharacterMethods.Ship,
                    notifiers.NotifyCharacterShipError, target.Import);
                QueryCharacterData<EsiAPISkills>(ESIAPICharacterMethods.Skills,
                    notifiers.NotifyCharacterSkillsError, target.Import);
                QueryCharacterData<List<int>>(ESIAPICharacterMethods.Implants,
                    notifiers.NotifyCharacterImplantsError, target.Import);
                QueryCharacterData<EsiAPIEmploymentHistory>(ESIAPICharacterMethods.EmploymentHistory,
                    notifiers.NotifyCharacterEmploymentError, target.Import);
                QueryCharacterData<string>(ESIAPICharacterMethods.AccountBalance,
                    notifiers.NotifyCharacterBalanceError, target.Import);

                target.Import(result);
                // Notify for insufficient balance
                target.NotifyInsufficientBalance();
                // Save results to XML cache
                var doc = Util.SerializeToXmlDocument(result);
                LocalXmlCache.SaveAsync(result.Name, doc).ConfigureAwait(false);
            }
        }
        
        /// <summary>
        /// Processes the queried character's skill queue information.
        /// </summary>
        /// <param name="result"></param>
        private void OnSkillQueueUpdated(EsiAPISkillQueue result)
        {
            var target = m_ccpCharacter;

            // Character may have been deleted since we queried
            if (target != null)
            {
                target.SkillQueue.Import(result.ToXMLItem().Queue);

                // Check the character has less than a day of training in skill queue
                if (target.IsTraining && target.SkillQueue.LessThanWarningThreshold)
                    EveMonClient.Notifications.NotifySkillQueueLessThanADay(target);
                else
                    EveMonClient.Notifications.InvalidateSkillQueueLessThanADay(target);
            }
        }

        /// <summary>
        /// Processes the queried character's standings information.
        /// </summary>
        /// <param name="result"></param>
        private void OnStandingsUpdated(EsiAPIStandings result)
        {
            var target = m_ccpCharacter;

            // Character may have been deleted since we queried
            if (target != null)
            {
                // Import the data
                target.Standings.Import(result.ToXMLItem().CharacterNPCStandings.All);

                // Fires the event regarding standings update
                EveMonClient.OnCharacterStandingsUpdated(target);
            }
        }

        /// <summary>
        /// Processes the queried character's factional warfare statistic information.
        /// </summary>
        /// <param name="result"></param>
        private void OnFactionalWarfareStatsUpdated(EsiAPIFactionalWarfareStats result)
        {
            var target = m_ccpCharacter;
            int factionID = result.FactionID;

            // Character may have been deleted since we queried
            if (target != null)
            {
                // Update the enlisted in factional warfare flag
                if (factionID != 0)
                {
                    target.IsFactionalWarfareNotEnlisted = false;
                    target.FactionalWarfareStats = new FactionalWarfareStats(result.ToXMLItem());
                }
                else
                    target.IsFactionalWarfareNotEnlisted = true;

                // Fires the event regarding factional warfare stats update
                EveMonClient.OnCharacterFactionalWarfareStatsUpdated(target);
            }
        }

        /// <summary>
        /// Processes the queried character's assets information.
        /// </summary>
        /// <param name="result"></param>
        private void OnAssetsUpdated(EsiAPIAssetList result)
        {
            var target = m_ccpCharacter;

            // Character may have been deleted since we queried
            if (target != null)
                TaskHelper.RunCPUBoundTaskAsync(() => target.Assets.Import(result.ToXMLItem().
                    Assets)).ContinueWith(_ =>
                    {
                        EveMonClient.OnCharacterAssetsUpdated(target);
                    }, EveMonClient.CurrentSynchronizationContext);
        }

        /// <summary>
        /// Processes the queried character's market orders.
        /// </summary>
        /// <param name="result"></param>
        /// <remarks>This method is sensitive to which "issued for" orders gets queried first</remarks>
        private void OnMarketOrdersUpdated(EsiAPIMarketOrders result)
        {
            var target = m_ccpCharacter;

            // Character may have been deleted since we queried
            if (target != null)
            {
                var orders = result.ToXMLItem(target.CharacterID).Orders;
                foreach (var order in orders)
                    order.IssuedFor = IssuedFor.Character;

                var endedOrders = new List<MarketOrder>();
                target.CharacterMarketOrders.Import(orders, endedOrders);

                // Fires the event regarding character market orders update
                EveMonClient.OnCharacterMarketOrdersUpdated(target, endedOrders);
            }
        }

        /// <summary>
        /// Processes the queried character's contracts.
        /// </summary>
        /// <param name="result"></param>
        /// <remarks>This method is sensitive to which "issued for" contracts gets queried first</remarks>
        private void OnContractsUpdated(EsiAPIContracts result)
        {
            var target = m_ccpCharacter;

            // Character may have been deleted since we queried
            if (target != null)
            {
                var contracts = result.ToXMLItem().Contracts;
                foreach (var contract in contracts)
                {
                    contract.APIMethod = ESIAPICharacterMethods.Contracts;
                    contract.IssuedFor = IssuedFor.Character;

                    // Fetch contract bids for auctions only
                    if (contract.Type.Equals("auction", StringComparison.InvariantCultureIgnoreCase))
                        QueryCharacterContractBids(contract.ContractID);
                }

                // Import the data
                var endedContracts = new List<Contract>();
                target.CharacterContracts.Import(contracts, endedContracts);

                // Fires the event regarding character contracts update
                EveMonClient.OnCharacterContractsUpdated(target, endedContracts);
            }
        }

        /// <summary>
        /// Processes the queried character's contract bids.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="forContract">The contract ID for which these bids were fetched.</param>
        private void OnContractBidsUpdated(EsiResult<EsiAPIContractBids> result, object forContract)
        {
            long contractID = (forContract as long?) ?? 0L;
            var target = m_ccpCharacter;

            // Character may have been deleted or set to not be monitored since we queried
            if (contractID != 0L && target != null && target.Monitored)
            {
                if (target.ShouldNotifyError(result, ESIAPICharacterMethods.ContractBids))
                    EveMonClient.Notifications.NotifyCharacterContractBidsError(target, result);
                if (!result.HasError)
                {
                    target.CharacterContractBids.Import(result.Result.ToXMLItem(contractID).
                        ContractBids);
                    EveMonClient.OnCharacterContractBidsUpdated(m_ccpCharacter);
                }
            }
        }

        /// <summary>
        /// Processes the queried character's wallet journal information.
        /// </summary>
        /// <param name="result"></param>
        private void OnWalletJournalUpdated(EsiAPIWalletJournal result)
        {
            var target = m_ccpCharacter;

            // Character may have been deleted since we queried
            if (target != null)
            {
                target.WalletJournal.Import(result.ToXMLItem().WalletJournal);
                EveMonClient.OnCharacterWalletJournalUpdated(target);
            }
        }

        /// <summary>
        /// Processes the queried character's wallet transactions information.
        /// </summary>
        /// <param name="result"></param>
        private void OnWalletTransactionsUpdated(EsiAPIWalletTransactions result)
        {
            var target = m_ccpCharacter;

            // Character may have been deleted since we queried
            if (target != null)
            {
                target.WalletTransactions.Import(result.ToXMLItem().WalletTransactions);
                EveMonClient.OnCharacterWalletTransactionsUpdated(target);
            }
        }

        /// <summary>
        /// Processes the queried character's personal industry jobs.
        /// </summary>
        /// <param name="result"></param>
        /// <remarks>This method is sensitive to which "issued for" jobs gets queried first</remarks>
        private void OnIndustryJobsUpdated(EsiAPIIndustryJobs result)
        {
            var target = m_ccpCharacter;

            // Character may have been deleted since we queried
            if (target != null)
            {
                var jobs = result.ToXMLItem().Jobs;
                foreach (var job in jobs)
                    job.IssuedFor = IssuedFor.Character;
                target.CharacterIndustryJobs.Import(jobs);
                EveMonClient.OnCharacterIndustryJobsUpdated(target);
            }
        }

        /// <summary>
        /// Processes the queried character's research points.
        /// </summary>
        /// <param name="result"></param>
        private void OnResearchPointsUpdated(EsiAPIResearchPoints result)
        {
            var target = m_ccpCharacter;

            // Character may have been deleted since we queried
            if (target != null)
            {
                target.ResearchPoints.Import(result.ToXMLItem().ResearchPoints);
                EveMonClient.OnCharacterResearchPointsUpdated(target);
            }
        }

        /// <summary>
        /// Processes the queried character's EVE mail messages.
        /// </summary>
        /// <param name="result"></param>
        private void OnEVEMailMessagesUpdated(EsiAPIMailMessages result)
        {
            var target = m_ccpCharacter;

            // Character may have been deleted since we queried
            if (target != null)
            {
                // Each time we import a new batch of EVE mail messages,
                // query the mailing lists so that we are always up to date
                QueryCharacterData<EsiAPIMailingLists>(ESIAPICharacterMethods.MailingLists,
                    EveMonClient.Notifications.NotifyMailingListsError, (lists) =>
                    target.EVEMailingLists.Import(lists.ToXMLItem().MailingLists));
                target.EVEMailMessages.Import(result.ToXMLItem().Messages);

                // Notify on new messages
                int newMessages = target.EVEMailMessages.NewMessages;
                if (newMessages != 0)
                    EveMonClient.Notifications.NotifyNewEVEMailMessages(target, newMessages);
            }
        }
        
        /// <summary>
        /// Processes the queried character's EVE notifications.
        /// </summary>
        /// <param name="result"></param>
        private void OnEVENotificationsUpdated(EsiAPINotifications result)
        {
            var target = m_ccpCharacter;

            // Character may have been deleted since we queried
            if (target != null)
            {
                target.EVENotifications.Import(result);

                // Notify on new notifications
                int newNotify = target.EVENotifications.NewNotifications;
                if (newNotify != 0)
                    EveMonClient.Notifications.NotifyNewEVENotifications(target, newNotify);
            }
        }

        /// <summary>
        /// Processes the queried character's contact list.
        /// </summary>
        /// <param name="result"></param>
        private void OnContactsUpdated(EsiAPIContactsList result)
        {
            var target = m_ccpCharacter;

            // Character may have been deleted since we queried
            if (target != null)
            {
                // TODO Corp and alliance contacts
                target.Contacts.Import(result.ToXMLItem().AllContacts);
                EveMonClient.OnCharacterContactsUpdated(target);
            }
        }

        /// <summary>
        /// Processes the queried character's medals.
        /// </summary>
        /// <param name="result"></param>
        private void OnMedalsUpdated(EsiAPIMedals result)
        {
            var target = m_ccpCharacter;

            // Character may have been deleted since we queried
            if (target != null)
            {
                target.CharacterMedals.Import(result.ToXMLItem().CorporationMedals);
                EveMonClient.OnCharacterMedalsUpdated(target);
            }
        }

        /// <summary>
        /// Processes the queried character's kill log.
        /// </summary>
        /// <param name="result"></param>
        private void OnKillLogUpdated(EsiAPIKillLog result)
        {
            var target = m_ccpCharacter;

            // Character may have been deleted since we queried
            if (target != null)
            {
                // TODO target.KillLog.Import(result.Kills);
                EveMonClient.OnCharacterKillLogUpdated(m_ccpCharacter);
                // Save the file to the cache
                string filename = $"{target.Name}-{ESIAPICharacterMethods.KillLog}";
                LocalXmlCache.SaveAsync(filename, Util.SerializeToXmlDocument(result)).
                    ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Processes the queried character's upcoming calendar events.
        /// </summary>
        /// <param name="result"></param>
        private void OnUpcomingCalendarEventsUpdated(EsiAPICalendarEvents result)
        {
            var target = m_ccpCharacter;

            // Character may have been deleted since we queried
            if (target != null)
            {
                // TODO target.UpcomingCalendarEvents.Import(result);
                EveMonClient.OnCharacterUpcomingCalendarEventsUpdated(target);
            }
        }

        /// <summary>
        /// Processes the queried character's planetary colonies.
        /// </summary>
        /// <param name="result"></param>
        private void OnPlanetaryColoniesUpdated(EsiAPIPlanetaryColoniesList result)
        {
            var target = m_ccpCharacter;

            // Character may have been deleted since we queried
            if (target != null)
            {
                // Invalidate previous notifications
                EveMonClient.Notifications.InvalidateCharacterPlanetaryPinCompleted(target);

                target.PlanetaryColonies.Import(result.ToXMLItem().Colonies);
                EveMonClient.OnCharacterPlanetaryColoniesUpdated(target);
            }
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
            foreach (var monitor in m_basicFeaturesMonitors)
                monitor.Enabled = m_ccpCharacter.Monitored;
        }

        #endregion

    }
}
