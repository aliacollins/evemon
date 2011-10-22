using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly CharacterDataQuerying m_characterDataQuerying;
        private readonly CorporationDataQuerying m_corporationDataQuerying;

        private Enum m_errorNotifiedMethod;


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
            CharacterIndustryJobs = new IndustryJobCollection(this);
            CorporationIndustryJobs = new IndustryJobCollection(this);
            ResearchPoints = new ResearchPointCollection(this);
            EVEMailMessages = new EveMailMessagesCollection(this);
            EVEMailingLists = new EveMailingListsCollection(this);
            EVENotifications = new EveNotificationsCollection(this);

            m_characterDataQuerying = new CharacterDataQuerying(this);
            m_corporationDataQuerying = new CorporationDataQuerying(this);

            EveMonClient.CharacterMarketOrdersUpdated += EveMonClient_CharacterMarketOrdersUpdated;
            EveMonClient.CorporationMarketOrdersUpdated += EveMonClient_CorporationMarketOrdersUpdated;
            EveMonClient.CharacterIndustryJobsUpdated += EveMonClient_CharacterIndustryJobsUpdated;
            EveMonClient.CorporationIndustryJobsUpdated += EveMonClient_CorporationIndustryJobsUpdated;
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
        }

        /// <summary>
        /// Constructor for a new CCP character on a new identity.
        /// </summary>
        /// <param name="identity"></param>
        internal CCPCharacter(CharacterIdentity identity)
            : this(identity, Guid.NewGuid())
        {
            m_characterDataQuerying.CharacterSheetMonitor.ForceUpdate(true);
            m_characterDataQuerying.SkillQueueMonitor.ForceUpdate(true);
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
                return (Identity.APIKeys.IsEmpty() ||
                        (m_characterDataQuerying.CharacterSheetMonitor.LastResult != null &&
                         m_characterDataQuerying.CharacterSheetMonitor.LastResult.HasError))
                           ? String.Format("{0} (cached)", Name)
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
            get { return CharacterMarketOrders.Concat(CorporationMarketOrders); }
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
        /// Gets the collection of industry jobs.
        /// </summary>
        public IEnumerable<IndustryJob> IndustryJobs
        {
            get { return CharacterIndustryJobs.Concat(CorporationIndustryJobs); }
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
        public EveMailMessagesCollection EVEMailMessages { get; private set; }

        /// <summary>
        /// Gets the collection of EVE mail messages.
        /// </summary>
        public EveMailingListsCollection EVEMailingLists { get; private set; }

        /// <summary>
        /// Gets the collection of EVE notifications.
        /// </summary>
        public EveNotificationsCollection EVENotifications { get; private set; }

        /// <summary>
        /// Gets the query monitors enumeration.
        /// </summary>
        public QueryMonitorCollection QueryMonitors { get; private set; }

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
            serial.SkillQueue = SkillQueue.Export();

            // Standings
            serial.Standings = Standings.Export();

            // Market orders
            serial.MarketOrders = MarketOrdersExport();

            // Industry jobs
            serial.IndustryJobs = IndustryJobsExport();

            // Eve mail messages IDs
            serial.EveMailMessagesIDs = EVEMailMessages.Export();

            // Eve notifications IDs
            serial.EveNotificationsIDs = EVENotifications.Export();

            // Last API updates
            foreach (SerializableAPIUpdate update in QueryMonitors.Select(
                monitor => new SerializableAPIUpdate
                               {
                                   Method = monitor.Method.ToString(),
                                   Time = monitor.LastUpdate
                               }))
            {
                serial.LastUpdates.Add(update);
            }

            return serial;
        }

        /// <summary>
        /// Exports the market orders.
        /// </summary>
        /// <returns></returns>
        private List<SerializableOrderBase> MarketOrdersExport()
        {
            return CharacterMarketOrders.Export().Concat(CorporationMarketOrders.Export()).ToList();
        }

        /// <summary>
        /// Exports the industry jobs.
        /// </summary>
        /// <returns></returns>
        private List<SerializableJob> IndustryJobsExport()
        {
            return CharacterIndustryJobs.Export().Concat(CorporationIndustryJobs.Export()).ToList();
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

            // Standings
            Standings.Import(serial.Standings);

            // Market orders
            MarketOrdersImport(serial.MarketOrders);

            // Industry jobs
            IndustryJobsImport(serial.IndustryJobs);

            // EVE mail messages IDs
            EVEMailMessages.Import(serial.EveMailMessagesIDs);

            // EVE notifications IDs
            EVENotifications.Import(serial.EveNotificationsIDs);

            // Last API updates
            foreach (SerializableAPIUpdate lastUpdate in serial.LastUpdates)
            {
                Enum method = APIMethods.Methods.First(x => x.ToString() == lastUpdate.Method);
                IQueryMonitorEx monitor = QueryMonitors[method] as IQueryMonitorEx;
                if (monitor != null)
                    monitor.Reset(lastUpdate.Time);
            }

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
        /// Forces an update on the selected query monitor.
        /// </summary>
        /// <param name="queryMonitor">The query monitor.</param>
        public void ForceUpdate(IQueryMonitor queryMonitor)
        {
            IQueryMonitorEx monitor = QueryMonitors[queryMonitor.Method] as IQueryMonitorEx;
            if (monitor != null)
                monitor.ForceUpdate(false);
        }

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
            if (result.CCPError != null && !result.CCPError.IsCorpRolesError)
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
            if (m_errorNotifiedMethod == method)
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
            // If character can not query corporation related data or corporation market orders monitor is not enabled
            // we switch the flag to proceed with notifications
            IQueryMonitor corpMarketOrdersMonitor = QueryMonitors[APICorporationMethods.CorporationMarketOrders];
            m_corporationDataQuerying.CorporationMarketOrdersQueried |= corpMarketOrdersMonitor == null ||
                                                                        !corpMarketOrdersMonitor.Enabled;

            // Quit if not all market orders where queried
            if (!m_characterDataQuerying.CharacterMarketOrdersQueried || !m_corporationDataQuerying.CorporationMarketOrdersQueried)
                return;

            // Notify for ended orders
            NotifyEndedOrders();

            // Notify for insufficient balance
            NotifyInsufficientBalance();

            // Reset flags
            m_characterDataQuerying.CharacterMarketOrdersQueried = false;
            m_corporationDataQuerying.CorporationMarketOrdersQueried = false;

            // Fires the event regarding market orders update
            EveMonClient.OnMarketOrdersUpdated(this);
        }

        /// <summary>
        /// Notifies for ended orders.
        /// </summary>
        private void NotifyEndedOrders()
        {
            IEnumerable<MarketOrder> endedOrders =
                m_characterDataQuerying.EndedOrders.Concat(m_corporationDataQuerying.EndedOrders);

            // Notify for ended orders
            if (endedOrders.Count() != 0)
                EveMonClient.Notifications.NotifyMarkerOrdersEnded(this, endedOrders);
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
            // If character can not query corporation related data or corporation industry jobs monitor is not enabled
            // we switch the flag to proceed with notifications
            IQueryMonitor corpIndustryJobsMonitor = QueryMonitors[APICorporationMethods.CorporationIndustryJobs];
            m_corporationDataQuerying.CorporationMarketOrdersQueried |= corpIndustryJobsMonitor == null ||
                                                                        !corpIndustryJobsMonitor.Enabled;

            // Quit if not all industry jobs where queried
            if (!m_characterDataQuerying.CharacterIndustryJobsQueried || !m_corporationDataQuerying.CorporationIndustryJobsQueried)
                return;

            // Reset flags
            m_characterDataQuerying.CharacterIndustryJobsQueried = false;
            m_corporationDataQuerying.CorporationIndustryJobsQueried = false;

            // Fires the event regarding industry jobs  update
            EveMonClient.OnIndustryJobsUpdated(this);
        }

        #endregion


        #region Global Events

        /// <summary>
        /// Handles the CharacterMarketOrdersUpdated event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EVEMon.Common.CustomEventArgs.CharacterChangedEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_CharacterMarketOrdersUpdated(object sender, CharacterChangedEventArgs e)
        {
            if (e.Character != this)
                return;

            NotifyForMarketOrdersRelatedEvents();
        }

        /// <summary>
        /// Handles the CorporationMarketOrdersUpdated event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EVEMon.Common.CustomEventArgs.CharacterChangedEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_CorporationMarketOrdersUpdated(object sender, CharacterChangedEventArgs e)
        {
            if (e.Character != this)
                return;

            NotifyForMarketOrdersRelatedEvents();
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

            NotifyForIndustryJobsRelatedEvents();
        }

        #endregion
    }
}