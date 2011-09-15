using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Net;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents a character from CCP, with additional capacities for training and such
    /// </summary>
    public sealed class CCPCharacter : Character
    {
        #region Fields

        private readonly CharacterQueryMonitor<SerializableAPISkillQueue> m_skillQueueMonitor;
        private readonly CharacterQueryMonitor<SerializableAPICharacterSheet> m_charSheetMonitor;
        private readonly CharacterQueryMonitor<SerializableAPIStandings> m_charStandingsMonitor;
        private readonly CharacterQueryMonitor<SerializableAPIResearch> m_charResearchPointsMonitor;
        private readonly CharacterQueryMonitor<SerializableAPIMarketOrders> m_charMarketOrdersMonitor;
        private readonly CharacterQueryMonitor<SerializableAPIIndustryJobs> m_charIndustryJobsMonitor;
        private readonly CharacterQueryMonitor<SerializableAPIMailMessages> m_charEVEMailMessagesMonitor;
        private readonly CharacterQueryMonitor<SerializableAPINotifications> m_charEVENotificationsMonitor;

        private APIMethods m_errorNotifiedMethod;

        #endregion


        #region Constructors

        /// <summary>
        /// Base constructor.
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="guid"></param>
        private CCPCharacter(CharacterIdentity identity, Guid guid)
            : base(identity, guid)
        {
            SkillQueue = new SkillQueue(this);
            Standings = new StandingCollection(this);
            MarketOrders = new MarketOrderCollection(this);
            IndustryJobs = new IndustryJobCollection(this);
            ResearchPoints = new ResearchPointCollection(this);
            EVEMailMessages = new EveMailMessagesCollection(this);
            EVEMailingLists = new EveMailingListsCollection(this);
            EVENotifications = new EveNotificationsCollection(this);
            QueryMonitors = new QueryMonitorCollection();

            // Initializes the query monitors 
            m_charSheetMonitor = new CharacterQueryMonitor<SerializableAPICharacterSheet>(this, APIMethods.CharacterSheet);
            m_charSheetMonitor.Updated += OnCharacterSheetUpdated;
            QueryMonitors.Add(m_charSheetMonitor);

            m_skillQueueMonitor = new CharacterQueryMonitor<SerializableAPISkillQueue>(this, APIMethods.SkillQueue);
            m_skillQueueMonitor.Updated += OnSkillQueueUpdated;
            QueryMonitors.Add(m_skillQueueMonitor);

            m_charStandingsMonitor = new CharacterQueryMonitor<SerializableAPIStandings>(this, APIMethods.Standings);
            m_charStandingsMonitor.Updated += OnStandingsUpdated;
            QueryMonitors.Add(m_charStandingsMonitor);

            m_charMarketOrdersMonitor = new CharacterQueryMonitor<SerializableAPIMarketOrders>(this, APIMethods.MarketOrders);
            m_charMarketOrdersMonitor.Updated += OnCharacterMarketOrdersUpdated;
            QueryMonitors.Add(m_charMarketOrdersMonitor);

            m_charIndustryJobsMonitor = new CharacterQueryMonitor<SerializableAPIIndustryJobs>(this, APIMethods.IndustryJobs);
            m_charIndustryJobsMonitor.Updated += OnCharacterJobsUpdated;
            QueryMonitors.Add(m_charIndustryJobsMonitor);

            m_charResearchPointsMonitor = new CharacterQueryMonitor<SerializableAPIResearch>(this, APIMethods.ResearchPoints)
                                              { QueryOnStartup = true };
            m_charResearchPointsMonitor.Updated += OnCharacterResearchPointsUpdated;
            QueryMonitors.Add(m_charResearchPointsMonitor);

            m_charEVEMailMessagesMonitor = new CharacterQueryMonitor<SerializableAPIMailMessages>(this, APIMethods.MailMessages)
                                               { QueryOnStartup = true };
            m_charEVEMailMessagesMonitor.Updated += OnCharacterEVEMailMessagesUpdated;
            QueryMonitors.Add(m_charEVEMailMessagesMonitor);

            m_charEVENotificationsMonitor = new CharacterQueryMonitor<SerializableAPINotifications>(this, APIMethods.Notifications)
                                                { QueryOnStartup = true };
            m_charEVENotificationsMonitor.Updated += OnCharacterEVENotificationsUpdated;
            QueryMonitors.Add(m_charEVENotificationsMonitor);

            // We disable all advanced features monitors here
            // Their enabling is determined in each 'CharacterMonitor'
            foreach (IQueryMonitor monitor in QueryMonitors)
            {
                monitor.Enabled = monitor.Method != (monitor.Method & APIMethods.AdvancedFeatures);
            }
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
            m_charSheetMonitor.ForceUpdate(true);
            m_skillQueueMonitor.ForceUpdate(true);
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
                if ((Identity.APIKey == null)
                    || (m_charSheetMonitor.LastResult != null && m_charSheetMonitor.LastResult.HasError))
                    return String.Format("{0} (cached)", Name);

                return Name;
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
        public MarketOrderCollection MarketOrders { get; private set; }

        /// <summary>
        /// Gets the collection of industry jobs.
        /// </summary>
        public IndustryJobCollection IndustryJobs { get; private set; }

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
                IEnumerable<MarketOrder> activeBuyOrdersIssuedForCharacter = MarketOrders
                    .Where(x => (x.State == OrderState.Active || x.State == OrderState.Modified)
                                && x is BuyOrder && x.IssuedFor == IssuedFor.Character);

                decimal activeTotal = activeBuyOrdersIssuedForCharacter.Sum(x => x.TotalPrice);
                decimal activeEscrow = activeBuyOrdersIssuedForCharacter.Sum(x => ((BuyOrder)x).Escrow);
                decimal additionalToCover = activeTotal - activeEscrow;

                return Balance >= additionalToCover;
            }
        }

        /// <summary>
        /// Gets the query monitors enumeration.
        /// </summary>
        public QueryMonitorCollection QueryMonitors { get; private set; }

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
            serial.MarketOrders = MarketOrders.Export();

            // Industry jobs
            serial.IndustryJobs = IndustryJobs.Export();

            // Eve mail messages IDs
            serial.EveMailMessagesIDs = EVEMailMessages.Export();

            // Eve notifications IDs
            serial.EveNotificationsIDs = EVENotifications.Export();

            // Last API updates
            foreach (SerializableAPIUpdate update in QueryMonitors.Select(
                monitor => new SerializableAPIUpdate
                               {
                                   Method = monitor.Method,
                                   Time = monitor.LastUpdate
                               }))
            {
                serial.LastUpdates.Add(update);
            }

            return serial;
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
            MarketOrders.Import(serial.MarketOrders);

            // Industry jobs
            IndustryJobs.Import(serial.IndustryJobs);

            // EVE mail messages IDs
            EVEMailMessages.Import(serial.EveMailMessagesIDs);

            // EVE notifications IDs
            EVENotifications.Import(serial.EveNotificationsIDs);

            // Last API updates
            foreach (SerializableAPIUpdate lastUpdate in serial.LastUpdates)
            {
                IQueryMonitorEx monitor = QueryMonitors[lastUpdate.Method] as IQueryMonitorEx;
                if (monitor != null)
                    monitor.Reset(lastUpdate.Time);
            }

            // Fire the global event
            EveMonClient.OnCharacterUpdated(this);
        }

        #endregion


        #region Querying

        /// <summary>
        /// Updates the character on a timer tick.
        /// </summary>
        internal void UpdateOnOneSecondTick()
        {
            if (!Monitored)
                return;

            QueryMonitors.UpdateOnOneSecondTick();
            SkillQueue.UpdateOnTimerTick();

            // If industry jobs monitoring is enabled, update job timers
            if (m_charIndustryJobsMonitor.Enabled && m_charIndustryJobsMonitor.HasAccess)
                IndustryJobs.UpdateOnTimerTick();
        }

        /// <summary>
        /// Queries the character's mailing list.
        /// </summary>
        private void QueryCharacterMailingList()
        {
            // Quits if no network
            if (!NetworkMonitor.IsNetworkAvailable)
                return;

            // Quits if access denied
            if ((int)APIMethods.MailingLists != (Identity.APIKey.AccessMask & (int)APIMethods.MailingLists))
                return;

            EveMonClient.APIProviders.CurrentProvider.QueryMethodAsync<SerializableAPIMailingLists>(
                APIMethods.MailingLists,
                Identity.APIKey.ID,
                Identity.APIKey.VerificationCode,
                CharacterID,
                OnCharacterMailingListUpdated);
        }

        /// <summary>
        /// Queries the character's info.
        /// </summary>
        private void QueryCharacterInfo()
        {
            // Quits if no network
            if (!NetworkMonitor.IsNetworkAvailable)
                return;

            // Quits if access denied
            if ((int)APIMethods.CharacterInfo != (Identity.APIKey.AccessMask & (int)APIMethods.CharacterInfo))
                return;

            EveMonClient.APIProviders.CurrentProvider.QueryMethodAsync<SerializableAPICharacterInfo>(
                APIMethods.CharacterInfo,
                Identity.APIKey.ID,
                Identity.APIKey.VerificationCode,
                CharacterID,
                OnCharacterInfoUpdated);
        }

        /// <summary>
        /// Processed the queried skill queue information.
        /// </summary>
        /// <param name="result"></param>
        private void OnCharacterSheetUpdated(APIResult<SerializableAPICharacterSheet> result)
        {
            // Notify an error occurred
            if (ShouldNotifyError(result, APIMethods.CharacterSheet))
                EveMonClient.Notifications.NotifyCharacterSheetError(this, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Query the Character's info
            QueryCharacterInfo();

            // Imports the data
            Import(result);

            // Check the character has a sufficient clone or send a notification
            if (Monitored && (CloneSkillPoints < SkillPoints))
                EveMonClient.Notifications.NotifyInsufficientClone(this);
            else
                EveMonClient.Notifications.InvalidateInsufficientClone(this);

            // Check for claimable certificates
            List<Certificate> claimableCertifitates = new List<Certificate>();
            claimableCertifitates.AddRange(Certificates.Where(x => x.CanBeClaimed));
            if (Monitored && claimableCertifitates.Count > 0)
                EveMonClient.Notifications.NotifyClaimableCertificate(this, claimableCertifitates);
            else
                EveMonClient.Notifications.InvalidateClaimableCertificate(this);
        }

        /// <summary>
        /// Processes the queried character's info.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnCharacterInfoUpdated(APIResult<SerializableAPICharacterInfo> result)
        {
            // Notify an error occured
            if (ShouldNotifyError(result, APIMethods.CharacterInfo))
                EveMonClient.Notifications.NotifyCharacterInfoError(this, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            Import(result.Result);
        }

        /// <summary>
        /// Processes the queried skill queue information.
        /// </summary>
        /// <param name="result"></param>
        private void OnSkillQueueUpdated(APIResult<SerializableAPISkillQueue> result)
        {
            // Notify an error occurred
            if (ShouldNotifyError(result, APIMethods.SkillQueue))
                EveMonClient.Notifications.NotifySkillQueueError(this, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            SkillQueue.Import(result.Result.Queue);

            // Check the account has a character in training
            Identity.APIKey.CharacterInTraining();

            // Check the character has room in skill queue
            if (IsTraining && (SkillQueue.EndTime < DateTime.UtcNow.AddHours(24)))
            {
                EveMonClient.Notifications.NotifySkillQueueRoomAvailable(this);
                return;
            }

            EveMonClient.Notifications.InvalidateSkillQueueRoomAvailability(this);
        }

        /// <summary>
        /// Processes the queried skill queue information.
        /// </summary>
        /// <param name="result"></param>
        private void OnStandingsUpdated(APIResult<SerializableAPIStandings> result)
        {
            // Notify an error occurred
            if (ShouldNotifyError(result, APIMethods.Standings))
                EveMonClient.Notifications.NotifyCharacterStandingsError(this, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            Standings.Import(result.Result.CharacterNPCStandings.All);
        }

        /// <summary>
        /// Processes the queried character's personal market orders.
        /// </summary>
        /// <param name="result"></param>
        /// <remarks>This method is sensitive to which market orders gets queried first</remarks>
        private void OnCharacterMarketOrdersUpdated(APIResult<SerializableAPIMarketOrders> result)
        {
            // Notify an error occurred
            if (ShouldNotifyError(result, APIMethods.MarketOrders))
                EveMonClient.Notifications.NotifyCharacterMarketOrdersError(this, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            List<MarketOrder> endedOrders = new List<MarketOrder>();
            result.Result.Orders.ForEach(x => x.IssuedFor = IssuedFor.Character);
            MarketOrders.Import(result.Result.Orders, endedOrders);

            // Sends a notification
            if (endedOrders.Count != 0)
                EveMonClient.Notifications.NotifyMarkerOrdersEnding(this, endedOrders);

            // Check the character has sufficient balance
            // for its buying orders or send a notification
            if (!HasSufficientBalance)
            {
                EveMonClient.Notifications.NotifyInsufficientBalance(this);
                return;
            }

            EveMonClient.Notifications.InvalidateInsufficientBalance(this);
        }

        /// <summary>
        /// Processes the queried character's personal industry jobs.
        /// </summary>
        /// <param name="result"></param>
        /// <remarks>This method is sensitive to which "issued for" jobs gets queried first</remarks>
        private void OnCharacterJobsUpdated(APIResult<SerializableAPIIndustryJobs> result)
        {
            // Notify an error occurred
            if (ShouldNotifyError(result, APIMethods.IndustryJobs))
                EveMonClient.Notifications.NotifyCharacterIndustryJobsError(this, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            result.Result.Jobs.ForEach(x => x.IssuedFor = IssuedFor.Character);
            IndustryJobs.Import(result.Result.Jobs);
        }

        /// <summary>
        /// Processes the queried character's research points.
        /// </summary>
        /// <param name="result"></param>
        private void OnCharacterResearchPointsUpdated(APIResult<SerializableAPIResearch> result)
        {
            // Notify an error occured
            if (ShouldNotifyError(result, APIMethods.ResearchPoints))
                EveMonClient.Notifications.NotifyResearchPointsError(this, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            ResearchPoints.Import(result.Result.ResearchPoints);
        }

        /// <summary>
        /// Processes the queried character's EVE mail messages.
        /// </summary>
        /// <param name="result"></param>
        private void OnCharacterEVEMailMessagesUpdated(APIResult<SerializableAPIMailMessages> result)
        {
            // Notify an error occured
            if (ShouldNotifyError(result, APIMethods.MailMessages))
                EveMonClient.Notifications.NotifyEVEMailMessagesError(this, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Each time we import a new batch of EVE mail messages,
            // query the mailing lists so that we are always up to date
            QueryCharacterMailingList();

            // Import the data
            EVEMailMessages.Import(result.Result.Messages);

            // Notify on new messages
            if (EVEMailMessages.NewMessages != 0)
                EveMonClient.Notifications.NotifyNewEVEMailMessages(this, EVEMailMessages.NewMessages);
        }

        /// <summary>
        /// Processes the queried character's EVE mailing lists.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnCharacterMailingListUpdated(APIResult<SerializableAPIMailingLists> result)
        {
            // Notify an error occured
            if (ShouldNotifyError(result, APIMethods.MailingLists))
                EveMonClient.Notifications.NotifyMailingListsError(this, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            EVEMailingLists.Import(result.Result.MailingLists);
        }

        /// <summary>
        /// Processes the queried character's EVE notifications.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnCharacterEVENotificationsUpdated(APIResult<SerializableAPINotifications> result)
        {
            // Notify an error occured
            if (ShouldNotifyError(result, APIMethods.Notifications))
                EveMonClient.Notifications.NotifyEVENotificationsError(this, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            EVENotifications.Import(result.Result.Notifications);

            // Notify on new messages
            if (EVENotifications.NewNotifications != 0)
                EveMonClient.Notifications.NotifyNewEVENotifications(this, EVENotifications.NewNotifications);
        }

        #endregion


        # region Helper Methods

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
        internal bool ShouldNotifyError(IAPIResult result, APIMethods method)
        {
            // Notify an error occurred
            if (result.HasError)
            {
                // Checks if EVE Backend Database is temporarily disabled
                if (result.EVEBackendDatabaseDisabled)
                    return false;

                if (m_errorNotifiedMethod != APIMethods.None)
                    return false;

                m_errorNotifiedMethod = method;
                return true;
            }

            // Removes the previous error notification
            if (m_errorNotifiedMethod == method)
            {
                EveMonClient.Notifications.InvalidateCharacterAPIError(this);
                m_errorNotifiedMethod = APIMethods.None;
            }
            return false;
        }

        #endregion
    }
}