using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents a character from CCP, with additional capacities for training and such.
    /// </summary>
    public sealed class CCPCharacter : Character
    {
        private readonly List<SerializableOrderListItem> m_orders = new List<SerializableOrderListItem>();
        private readonly List<SerializableJobListItem> m_jobs = new List<SerializableJobListItem>();
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
            MarketOrders = new MarketOrderCollection(this);
            IndustryJobs = new IndustryJobCollection(this);
            ResearchPoints = new ResearchPointCollection(this);
            EVEMailMessages = new EveMailMessagesCollection(this);
            EVEMailingLists = new EveMailingListsCollection(this);
            EVENotifications = new EveNotificationsCollection(this);

            CharacterDataQuerying = new CharacterDataQuerying(this);
            CorporationDataQuerying = new CorporationDataQuerying(this);
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
            CharacterDataQuerying.CharacterSheetMonitor.ForceUpdate(true);
            CharacterDataQuerying.SkillQueueMonitor.ForceUpdate(true);
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
                        (CharacterDataQuerying.CharacterSheetMonitor.LastResult != null &&
                         CharacterDataQuerying.CharacterSheetMonitor.LastResult.HasError))
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
        /// Gets the query monitors enumeration.
        /// </summary>
        public QueryMonitorCollection QueryMonitors { get; private set; }

        /// <summary>
        /// Gets the character data querying.
        /// </summary>
        /// <value>The character data querying.</value>
        public CharacterDataQuerying CharacterDataQuerying { get; private set; }

        /// <summary>
        /// Gets the corporation data querying.
        /// </summary>
        /// <value>The corporation data querying.</value>
        public CorporationDataQuerying CorporationDataQuerying { get; private set; }

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
                                   Method = monitor.Method.ToString(),
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
                Enum method = APIMethods.Methods.First(x => x.ToString() == lastUpdate.Method);
                IQueryMonitorEx monitor = QueryMonitors[method] as IQueryMonitorEx;
                if (monitor != null)
                    monitor.Reset(lastUpdate.Time);
            }

            // Fire the global event
            EveMonClient.OnCharacterUpdated(this);
        }

        #endregion


        # region Querying Helper Methods

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
                //EveMonClient.Notifications.InvalidateCharacterAPIError(CCPCharacter);
                m_errorNotifiedMethod = APIMethodsExtensions.None;
            }
            return false;
        }

        /// <summary>
        /// Add the queried orders to a list.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="ordersAdded"></param>
        /// <param name="issuedFor"></param>
        /// <returns>True if orders get added, false otherwise</returns>
        internal bool AddOrders(APIResult<SerializableAPIMarketOrders> result, bool ordersAdded, IssuedFor issuedFor)
        {
            // Add orders if there isn't an error
            if (result.HasError)
                return false;

            // Check to see if other market
            // orders have been added before
            if (!ordersAdded)
                m_orders.Clear();

            // Add orders in list
            result.Result.Orders.ForEach(order => order.IssuedFor = issuedFor);
            m_orders.AddRange(result.Result.Orders);

            return true;
        }

        /// <summary>
        /// Import the orders from both market orders querying.
        /// </summary>
        internal void ImportOrders()
        {
            // Exclude orders that wheren't issued by this character
            // (Delete this line upon implementing an exclusive corporation market monitor)
            IEnumerable<SerializableOrderListItem> characterOrders = m_orders.Where(x => x.OwnerID == CharacterID);

            List<MarketOrder> endedOrders = new List<MarketOrder>();
            MarketOrders.Import(characterOrders, endedOrders);

            // Notify for ended orders
            if (endedOrders.Count() != 0)
                EveMonClient.Notifications.NotifyMarkerOrdersEnded(this, endedOrders);

            // Reset flags
            CharacterDataQuerying.CharOrdersUpdated = false;
            CharacterDataQuerying.CharOrdersAdded = false;
            CorporationDataQuerying.CorpJobsUpdated = false;
            CorporationDataQuerying.CorpJobsAdded = false;

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
        /// Add the queried jobs to a list.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="jobsAdded"></param>
        /// <param name="issuedFor"></param>
        /// <returns>True if jobs get added, false otherwise</returns>
        internal bool AddJobs(APIResult<SerializableAPIIndustryJobs> result, bool jobsAdded, IssuedFor issuedFor)
        {
            // Add orders if there isn't an error
            if (result.HasError)
                return false;

            // Check to see if other jobs
            // have been added before
            if (!jobsAdded)
                m_jobs.Clear();

            // Add jobs in list
            result.Result.Jobs.ForEach(x => x.IssuedFor = issuedFor);
            m_jobs.AddRange(result.Result.Jobs);

            return true;
        }

        /// <summary>
        /// Import the jobs from both industry jobs querying.
        /// </summary>
        internal void ImportJobs()
        {
            // Exclude jobs that wheren't issued by this character
            // (Delete this line upon implementing an exclusive corporation jobs monitor)
            IEnumerable<SerializableJobListItem> characterJobs = m_jobs.Where(x => x.InstallerID == CharacterID);

            IndustryJobs.Import(characterJobs);

            // Fires the event regarding industry jobs update
            EveMonClient.OnIndustryJobsUpdated(this);

            // Reset flags
            CharacterDataQuerying.CharJobsUpdated = false;
            CharacterDataQuerying.CharJobsAdded = false;
            CorporationDataQuerying.CorpOrdersUpdated = false;
            CorporationDataQuerying.CorpOrdersAdded = false;
        }

        #endregion

    }
}
