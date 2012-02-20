using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents a character from CCP, with additional capacities for training and such.
    /// </summary>
    public sealed class CCPCharacter : Character
    {
        private readonly List<MarketOrder> m_endedOrdersForCharacter;
        private readonly List<MarketOrder> m_endedOrdersForCorporation;
        private readonly List<Contract> m_endedContractsForCharacter;
        private readonly List<Contract> m_endedContractsForCorporation;
        private readonly List<IndustryJob> m_jobsCompletedForCharacter;

        private CharacterDataQuerying m_characterDataQuerying;
        private CorporationDataQuerying m_corporationDataQuerying;

        private Enum m_errorNotifiedMethod = APIMethodsExtensions.None;


        #region Constructors

        /// <summary>
        /// Base constructor.
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="guid"></param>
        private CCPCharacter(CharacterIdentity identity, Guid guid)
            : base(identity, guid)
        {
            QueryMonitors = new QueryMonitorCollection();
            SkillQueue = new SkillQueue(this);
            Standings = new StandingCollection(this);
            CharacterMarketOrders = new MarketOrderCollection(this);
            CorporationMarketOrders = new MarketOrderCollection(this);
            CharacterContracts = new ContractCollection(this);
            CorporationContracts = new ContractCollection(this);
            CharacterContractBids = new ContractBidsCollection(this);
            CorporationContractBids = new ContractBidsCollection(this);
            CharacterIndustryJobs = new IndustryJobCollection(this);
            CorporationIndustryJobs = new IndustryJobCollection(this);
            ResearchPoints = new ResearchPointCollection(this);
            EVEMailMessages = new EveMailMessageCollection(this);
            EVEMailingLists = new EveMailingListCollection(this);
            EVENotifications = new EveNotificationCollection(this);

            m_endedOrdersForCharacter = new List<MarketOrder>();
            m_endedOrdersForCorporation = new List<MarketOrder>();

            m_endedContractsForCharacter = new List<Contract>();
            m_endedContractsForCorporation = new List<Contract>();

            m_jobsCompletedForCharacter = new List<IndustryJob>();

            EveMonClient.CharacterMarketOrdersUpdated += EveMonClient_CharacterMarketOrdersUpdated;
            EveMonClient.CorporationMarketOrdersUpdated += EveMonClient_CorporationMarketOrdersUpdated;
            EveMonClient.CharacterContractsUpdated += EveMonClient_CharacterContractsUpdated;
            EveMonClient.CorporationContractsUpdated += EveMonClient_CorporationContractsUpdated;
            EveMonClient.CharacterIndustryJobsUpdated += EveMonClient_CharacterIndustryJobsUpdated;
            EveMonClient.CorporationIndustryJobsUpdated += EveMonClient_CorporationIndustryJobsUpdated;
            EveMonClient.CharacterIndustryJobsCompleted += EveMonClient_CharacterIndustryJobsCompleted;
            EveMonClient.CorporationIndustryJobsCompleted += EveMonClient_CorporationIndustryJobsCompleted;
            EveMonClient.TimerTick += EveMonClient_TimerTick;
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="identity">The identity for this character</param>
        /// <param name="serial">A deserialization object for characters</param>
        internal CCPCharacter(CharacterIdentity identity, SerializableCCPCharacter serial)
            : this(identity, serial.Guid)
        {
            Import(serial);
            LastAPIUpdates = serial.LastUpdates;
        }

        /// <summary>
        /// Constructor for a new CCP character on a new identity.
        /// </summary>
        /// <param name="identity"></param>
        internal CCPCharacter(CharacterIdentity identity)
            : this(identity, Guid.NewGuid())
        {
            ForceUpdateBasicFeatures = true;
            LastAPIUpdates = new EmptyEnumerable<SerializableAPIUpdate>();
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets an adorned name, with (file), (url) or (cached) labels.
        /// </summary>
        public override string AdornedName
        {
            get
            {
                return (!Identity.APIKeys.Any() || Identity.APIKeys.All(apiKey => !apiKey.Monitored) ||
                        (m_characterDataQuerying != null && m_characterDataQuerying.CharacterSheetMonitor.HasError))
                           ? String.Format(CultureConstants.DefaultCulture, "{0} (cached)", Name)
                           : Name;
            }
        }

        /// <summary>
        /// Gets the skill queue for this character.
        /// </summary>
        public SkillQueue SkillQueue { get; private set; }

        /// <summary>
        /// Gets the standings for this character.
        /// </summary>
        public StandingCollection Standings { get; private set; }

        /// <summary>
        /// Gets the collection of market orders.
        /// </summary>
        public IEnumerable<MarketOrder> MarketOrders
        {
            get { return CharacterMarketOrders.Concat(CorporationMarketOrders.Where(order => order.OwnerID == CharacterID)); }
        }

        /// <summary>
        /// Gets or sets the character market orders.
        /// </summary>
        /// <value>The character market orders.</value>
        public MarketOrderCollection CharacterMarketOrders { get; private set; }

        /// <summary>
        /// Gets or sets the corporation market orders.
        /// </summary>
        /// <value>The corporation market orders.</value>
        public MarketOrderCollection CorporationMarketOrders { get; private set; }

        /// <summary>
        /// Gets the collection of contracts.
        /// </summary>
        /// <remarks>Excludes contracts that appear in both collections</remarks>
        public IEnumerable<Contract> Contracts
        {
            get
            {
                return CharacterContracts.Where(charContract => CorporationContracts.All(
                    corpContract => corpContract.ID != charContract.ID)).Concat(CorporationContracts.Where(
                        contract => contract.IssuerID == CharacterID));
            }
        }

        /// <summary>
        /// Gets or sets the character contracts.
        /// </summary>
        /// <value>The character contracts.</value>
        public ContractCollection CharacterContracts { get; private set; }

        /// <summary>
        /// Gets or sets the corporation contracts.
        /// </summary>
        /// <value>The character contracts.</value>
        public ContractCollection CorporationContracts { get; private set; }

        /// <summary>
        /// Gets or sets the character contract bids.
        /// </summary>
        /// <value>The character contract bids.</value>
        public ContractBidsCollection CharacterContractBids { get; private set; }

        /// <summary>
        /// Gets or sets the corporation contract bids.
        /// </summary>
        /// <value>The character contract bids.</value>
        public ContractBidsCollection CorporationContractBids { get; private set; }

        /// <summary>
        /// Gets the collection of industry jobs.
        /// </summary>
        public IEnumerable<IndustryJob> IndustryJobs
        {
            get { return CharacterIndustryJobs.Concat(CorporationIndustryJobs.Where(job => job.InstallerID == CharacterID)); }
        }

        /// <summary>
        /// Gets or sets the character industry jobs.
        /// </summary>
        /// <value>The character industry jobs.</value>
        public IndustryJobCollection CharacterIndustryJobs { get; set; }

        /// <summary>
        /// Gets or sets the corporation industry jobs.
        /// </summary>
        /// <value>The corporation industry jobs.</value>
        public IndustryJobCollection CorporationIndustryJobs { get; set; }

        /// <summary>
        /// Gets the collection of research points.
        /// </summary>
        public ResearchPointCollection ResearchPoints { get; private set; }

        /// <summary>
        /// Gets the collection of EVE mail messages.
        /// </summary>
        public EveMailMessageCollection EVEMailMessages { get; private set; }

        /// <summary>
        /// Gets the collection of EVE mail messages.
        /// </summary>
        public EveMailingListCollection EVEMailingLists { get; private set; }

        /// <summary>
        /// Gets the collection of EVE notifications.
        /// </summary>
        public EveNotificationCollection EVENotifications { get; private set; }

        /// <summary>
        /// Gets the query monitors enumeration.
        /// </summary>
        public QueryMonitorCollection QueryMonitors { get; private set; }

        /// <summary>
        /// Gets the query monitors last API updates enumeration.
        /// </summary>
        public IEnumerable<SerializableAPIUpdate> LastAPIUpdates { get; private set; }

        /// <summary>
        /// Gets true when the character is currently actively training, false otherwise.
        /// </summary>
        public override bool IsTraining
        {
            get { return SkillQueue.IsTraining; }
        }

        /// <summary>
        /// Gets the skill currently in training, even when it is paused.
        /// </summary>
        public override QueuedSkill CurrentlyTrainingSkill
        {
            get { return SkillQueue.CurrentlyTraining; }
        }

        /// <summary>
        /// Gets true when character has insufficient balance to complete its buy orders.
        /// </summary>
        public bool HasSufficientBalance
        {
            get
            {
                IEnumerable<BuyOrder> activeBuyOrders = MarketOrders.OfType<BuyOrder>().Where(
                    order => (order.State == OrderState.Active || order.State == OrderState.Modified) &&
                             order.IssuedFor == IssuedFor.Character);

                decimal additionalToCover = activeBuyOrders.Sum(x => x.TotalPrice) - activeBuyOrders.Sum(order => order.Escrow);

                return Balance >= additionalToCover;
            }

        }

        /// <summary>
        /// Gets true when a new character is created.
        /// </summary>
        public bool ForceUpdateBasicFeatures { get; private set; }

        #endregion


        #region Importing & Exporting

        /// <summary>
        /// Create a serializable character sheet for this character.
        /// </summary>
        /// <returns></returns>
        public override SerializableSettingsCharacter Export()
        {
            SerializableCCPCharacter serial = new SerializableCCPCharacter();
            Export(serial);

            // Skill queue
            serial.SkillQueue.AddRange(SkillQueue.Export());

            // Market orders
            serial.MarketOrders.AddRange(MarketOrdersExport());

            // Contracts
            serial.Contracts.AddRange(ContractsExport());

            // ContractBids
            serial.ContractBids.AddRange(CharacterContractBids.Export());

            // Industry jobs
            serial.IndustryJobs.AddRange(IndustryJobsExport());

            // Eve mail messages IDs
            serial.EveMailMessagesIDs = EVEMailMessages.Export();

            // Eve notifications IDs
            serial.EveNotificationsIDs = EVENotifications.Export();

            // Last API updates
            if (QueryMonitors.Any())
            {
                LastAPIUpdates = QueryMonitors.Select(
                monitor => new SerializableAPIUpdate
                               {
                                   Method = monitor.Method.ToString(),
                                   Time = monitor.LastUpdate
                               });
            }

            serial.LastUpdates.AddRange(LastAPIUpdates);

            return serial;
        }

        /// <summary>
        /// Exports the market orders.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<SerializableOrderBase> MarketOrdersExport()
        {
            return CharacterMarketOrders.Export().Concat(CorporationMarketOrders.ExportOnlyIssuedByCharacter());
        }

        /// <summary>
        /// Exports the contracts.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Excludes contracts that appear in both collections</remarks>
        private IEnumerable<SerializableContract> ContractsExport()
        {
            IEnumerable<SerializableContract> corporationContractsExport = CorporationContracts.ExportOnlyIssuedByCharacter();
            return CharacterContracts.Export().Where(charContract => corporationContractsExport.All(
                corpContract => corpContract.ContractID != charContract.ContractID)).Concat(corporationContractsExport);
        }

        /// <summary>
        /// Exports the industry jobs.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<SerializableJob> IndustryJobsExport()
        {
            return CharacterIndustryJobs.Export().Concat(CorporationIndustryJobs.ExportOnlyIssuedByCharacter());
        }

        /// <summary>
        /// Imports data from a serialization object.
        /// </summary>
        /// <param name="serial"></param>
        private void Import(SerializableCCPCharacter serial)
        {
            Import((SerializableSettingsCharacter)serial);

            // Skill queue
            SkillQueue.Import(serial.SkillQueue);

            // Market orders
            MarketOrdersImport(serial.MarketOrders);

            // Contracts
            ContractsImport(serial.Contracts);

            // Contract Bids
            CharacterContractBids.Import(serial.ContractBids);

            // Industry jobs
            IndustryJobsImport(serial.IndustryJobs);

            // EVE mail messages IDs
            EVEMailMessages.Import(serial.EveMailMessagesIDs);

            // EVE notifications IDs
            EVENotifications.Import(serial.EveNotificationsIDs);

            // Fire the global event
            EveMonClient.OnCharacterUpdated(this);
        }

        /// <summary>
        /// Imports the market orders.
        /// </summary>
        /// <param name="marketOrders">The market orders.</param>
        private void MarketOrdersImport(IEnumerable<SerializableOrderBase> marketOrders)
        {
            CharacterMarketOrders.Import(marketOrders.Where(job => job.IssuedFor == IssuedFor.Character));
            CorporationMarketOrders.Import(marketOrders.Where(job => job.IssuedFor == IssuedFor.Corporation));
        }

        /// <summary>
        /// Imports the contracts.
        /// </summary>
        /// <param name="contracts">The contracts.</param>
        private void ContractsImport(IEnumerable<SerializableContract> contracts)
        {
            CharacterContracts.Import(contracts.Where(contract => contract.IssuedFor == IssuedFor.Character));
            CorporationContracts.Import(contracts.Where(contract => contract.IssuedFor == IssuedFor.Corporation));
        }

        /// <summary>
        /// Imports the industry jobs.
        /// </summary>
        /// <param name="industryJobs">The industry jobs.</param>
        private void IndustryJobsImport(IEnumerable<SerializableJob> industryJobs)
        {
            CharacterIndustryJobs.Import(industryJobs.Where(job => job.IssuedFor == IssuedFor.Character));
            CorporationIndustryJobs.Import(industryJobs.Where(job => job.IssuedFor == IssuedFor.Corporation));
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Checks whether we should notify an error.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        internal bool ShouldNotifyError(IAPIResult result, Enum method)
        {
            // Checks if EVE database is out of service
            if (result.EVEDatabaseError)
                return false;

            // We don't want to be notified about corp roles error
            if (result.CCPError != null && result.CCPError.IsCorpRolesError)
                return false;

            // Notify an error occurred
            if (result.HasError)
            {
                if (!m_errorNotifiedMethod.Equals(APIMethodsExtensions.None))
                    return false;

                m_errorNotifiedMethod = method;
                return true;
            }

            // Removes the previous error notification
            if (m_errorNotifiedMethod.Equals(method))
            {
                EveMonClient.Notifications.InvalidateCharacterAPIError(this);
                m_errorNotifiedMethod = APIMethodsExtensions.None;
            }
            return false;
        }

        /// <summary>
        /// Notifies for market orders related events.
        /// </summary>
        private void NotifyForMarketOrdersRelatedEvents()
        {
            // Notify for ended orders
            NotifyEndedOrders();

            // Notify for insufficient balance
            NotifyInsufficientBalance();

            // Reset helper lists
            m_endedOrdersForCharacter.Clear();
            m_endedOrdersForCorporation.Clear();

            // Fires the event regarding market orders update
            EveMonClient.OnMarketOrdersUpdated(this);
        }

        /// <summary>
        /// Notifies for ended orders.
        /// </summary>
        private void NotifyEndedOrders()
        {
            // Notify ended orders issued by the character
            if (m_endedOrdersForCharacter.Any())
                EveMonClient.Notifications.NotifyCharacterMarkerOrdersEnded(this, m_endedOrdersForCharacter);

            // Uncomment upon implementing an exclusive corporation monitor
            // Notify ended orders issued for the corporation
            //if (m_endedOrdersForCorporation.Any())
            //    EveMonClient.Notifications.NotifyCorporationMarketOrdersEnded(Corporation, m_endedOrdersForCorporation);
        }

        /// <summary>
        /// Notifies for contracts related events.
        /// </summary>
        private void NotifyForContractsRelatedEvents()
        {
            // Notify for ended contracts
            NotifyEndedContracts();

            // Notify for assigned contracts
            NotifyAssignedContracts();

            // Reset helper lists
            // Note: Special condition logic is applied due to the fact that CCP
            // includes coproration related contracts in character API feed
            if (m_characterDataQuerying != null && m_corporationDataQuerying != null &&
                m_corporationDataQuerying.CorporationContractsQueried)
            {
                m_endedContractsForCharacter.Clear();
            }

            if (m_corporationDataQuerying != null && m_characterDataQuerying != null &&
                m_characterDataQuerying.CharacterContractsQueried)
            {
                m_endedContractsForCorporation.Clear();
            }

            // Fires the event regarding contracts update
            EveMonClient.OnContractsUpdated(this);
        }

        /// <summary>
        /// Notifies for ended contracts.
        /// </summary>
        private void NotifyEndedContracts()
        {
            // Notify ended contracts issued by the character
            if (m_endedContractsForCharacter.Any(x => !x.NotificationSend))
            {
                EveMonClient.Notifications.NotifyCharacterContractsEnded(this, m_endedContractsForCharacter);
                m_endedContractsForCharacter.ForEach(x => x.NotificationSend = true);
            }

            // Uncomment upon implementing an exclusive corporation monitor
            // Notify ended contracts issued for the corporation
            //if (m_endedContractsForCorporation.All(x => x.NotificationSend))
            //    return;

            //EveMonClient.Notifications.NotifyCorporationContractsEnded(Corporation, m_endedContractsForCorporation);
            //m_endedContractsForCorporation.ForEach(x => x.NotificationSend = true);
        }

        /// <summary>
        /// Notifies for assigned contracts.
        /// </summary>
        private void NotifyAssignedContracts()
        {
            if (Contracts.Any(contract => contract.State == ContractState.Assigned))
            {
                int assignedContracts = Contracts.Count(contracts => contracts.State == ContractState.Assigned);
                EveMonClient.Notifications.NotifyCharacterContractsAssigned(this, assignedContracts);
                return;
            }

            EveMonClient.Notifications.InvalidateCharacterContractsAssigned(this);
        }

        /// <summary>
        /// Notifies for insufficient balance.
        /// </summary>
        private void NotifyInsufficientBalance()
        {
            // Check the character has sufficient balance
            // for its buying orders and send a notification if not
            if (!HasSufficientBalance)
            {
                EveMonClient.Notifications.NotifyInsufficientBalance(this);
                return;
            }

            EveMonClient.Notifications.InvalidateInsufficientBalance(this);
        }

        /// <summary>
        /// Notifies for industry jobs related events.
        /// </summary>
        private void NotifyForIndustryJobsRelatedEvents()
        {
            // Fires the event regarding industry jobs update
            EveMonClient.OnIndustryJobsUpdated(this);
        }

        /// <summary>
        /// Resets the last API updates.
        /// </summary>
        /// <param name="lastUpdates">The last updates.</param>
        private void ResetLastAPIUpdates(IEnumerable<SerializableAPIUpdate> lastUpdates)
        {
            foreach (SerializableAPIUpdate lastUpdate in lastUpdates)
            {
                Enum method = APIMethods.Methods.FirstOrDefault(apiMethod => apiMethod.ToString() == lastUpdate.Method);
                if (method == null)
                    continue;

                IQueryMonitorEx monitor = QueryMonitors[method] as IQueryMonitorEx;
                if (monitor != null)
                    monitor.Reset(lastUpdate.Time);
            }
        }

        #endregion


        #region Global Events

        /// <summary>
        /// Handles the TimerTick event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EveMonClient_TimerTick(object sender, EventArgs e)
        {
            if (EveMonClient.APIKeys.Any(apiKey => !apiKey.IsProcessed))
                return;

            if (!Identity.APIKeys.Any() || Identity.APIKeys.Any(apiKey => apiKey.Type == APIKeyType.Unknown))
                return;

            if (m_characterDataQuerying != null & m_corporationDataQuerying != null)
                return;

            if (m_characterDataQuerying == null && Identity.APIKeys.Any(apiKey => apiKey.IsCharacterOrAccountType))
            {
                m_characterDataQuerying = new CharacterDataQuerying(this);
                ResetLastAPIUpdates(LastAPIUpdates.Where(lastUpdate => Enum.IsDefined(typeof(APICharacterMethods), lastUpdate.Method)));
            }

            if (m_corporationDataQuerying == null && Identity.APIKeys.Any(apiKey => apiKey.IsCorporationType))
            {
                m_corporationDataQuerying = new CorporationDataQuerying(this);
                ResetLastAPIUpdates(LastAPIUpdates.Where(lastUpdate => Enum.IsDefined(typeof(APICorporationMethods), lastUpdate.Method)));
            }
        }

        /// <summary>
        /// Handles the CharacterMarketOrdersUpdated event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EVEMon.Common.CustomEventArgs.MarketOrdersEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_CharacterMarketOrdersUpdated(object sender, MarketOrdersEventArgs e)
        {
            if (e.Character != this)
                return;

            m_endedOrdersForCharacter.AddRange(e.EndedOrders);

            if (Identity.CanQueryCorporationInfo && m_corporationDataQuerying != null &&
                !m_corporationDataQuerying.CorporationMarketOrdersQueried)
            {
                return;
            }

            NotifyForMarketOrdersRelatedEvents();
        }

        /// <summary>
        /// Handles the CorporationMarketOrdersUpdated event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EVEMon.Common.CustomEventArgs.MarketOrdersEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_CorporationMarketOrdersUpdated(object sender, MarketOrdersEventArgs e)
        {
            if (e.Character != this)
                return;

            m_endedOrdersForCorporation.AddRange(e.EndedOrders);
            m_endedOrdersForCharacter.AddRange(e.EndedOrders.Where(order => order.OwnerID == CharacterID));

            if (Identity.CanQueryCharacterInfo && m_characterDataQuerying != null &&
                !m_characterDataQuerying.CharacterMarketOrdersQueried)
            {
                return;
            }

            NotifyForMarketOrdersRelatedEvents();
        }

        /// <summary>
        /// Handles the CharacterContractsUpdated event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EVEMon.Common.CustomEventArgs.ContractsEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_CharacterContractsUpdated(object sender, ContractsEventArgs e)
        {
            if (e.Character != this)
                return;

            m_endedContractsForCharacter.AddRange(e.EndedContracts.Where(
                charEndedContract => m_endedContractsForCorporation.All(
                    corpEndedContract => corpEndedContract.ID != charEndedContract.ID)));

            if (Identity.CanQueryCorporationInfo && m_corporationDataQuerying != null &&
                !m_corporationDataQuerying.CorporationContractsQueried)
            {
                return;
            }

            NotifyForContractsRelatedEvents();
        }

        /// <summary>
        /// Handles the CorporationContractsUpdated event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EVEMon.Common.CustomEventArgs.ContractsEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_CorporationContractsUpdated(object sender, ContractsEventArgs e)
        {
            if (e.Character != this)
                return;

            m_endedContractsForCorporation.AddRange(e.EndedContracts);
            m_endedContractsForCharacter.AddRange(e.EndedContracts.Where(contract => contract.IssuerID == CharacterID).Where(
                corpEndedContract => m_endedContractsForCharacter.All(
                    charEndedContract => charEndedContract.ID != corpEndedContract.ID)));

            if (Identity.CanQueryCharacterInfo && m_characterDataQuerying != null &&
                !m_characterDataQuerying.CharacterContractsQueried)
            {
                return;
            }

            NotifyForContractsRelatedEvents();
        }

        /// <summary>
        /// Handles the CharacterIndustryJobsUpdated event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EVEMon.Common.CustomEventArgs.CharacterChangedEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_CharacterIndustryJobsUpdated(object sender, CharacterChangedEventArgs e)
        {
            if (e.Character != this)
                return;

            if (Identity.CanQueryCorporationInfo && m_corporationDataQuerying != null &&
                !m_corporationDataQuerying.CorporationIndustryJobsQueried)
            {
                return;
            }

            NotifyForIndustryJobsRelatedEvents();
        }

        /// <summary>
        /// Handles the CorporationIndustryJobsUpdated event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EVEMon.Common.CustomEventArgs.CharacterChangedEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_CorporationIndustryJobsUpdated(object sender, CharacterChangedEventArgs e)
        {
            if (e.Character != this)
                return;

            if (Identity.CanQueryCharacterInfo && m_characterDataQuerying != null &&
                !m_characterDataQuerying.CharacterIndustryJobsQueried)
            {
                return;
            }

            NotifyForIndustryJobsRelatedEvents();
        }

        /// <summary>
        /// Handles the CharacterIndustryJobsCompleted event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EVEMon.Common.CustomEventArgs.IndustryJobsEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_CharacterIndustryJobsCompleted(object sender, IndustryJobsEventArgs e)
        {
            if (e.Character != this)
                return;

            // Add the completed jobs to a list
            m_jobsCompletedForCharacter.AddRange(e.CompletedJobs);

            // If character has completed corporation issued jobs, we wait till we gather those too
            if (CorporationIndustryJobs.Any(job => job.ActiveJobState == ActiveJobState.Ready && !job.NotificationSend))
                return;

            // Notify completed jobs issued by the character
            EveMonClient.Notifications.NotifyCharacterIndustryJobCompletion(this, m_jobsCompletedForCharacter);

            // Now that we have send the notification clear the list
            m_jobsCompletedForCharacter.Clear();
        }

        /// <summary>
        /// Handles the CorporationIndustryJobsCompleted event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EVEMon.Common.CustomEventArgs.IndustryJobsEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_CorporationIndustryJobsCompleted(object sender, IndustryJobsEventArgs e)
        {
            if (e.Character != this)
                return;

            // Uncomment upon implementing an exclusive corporation monitor
            // Notify completed jobs issued for the corporation
            //EveMonClient.Notifications.NotifyCorporationIndustryJobCompletion(Corporation, e.CompletedJobs);
        }

        #endregion
    }
}