using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using EVEMon.Common.Data;
using EVEMon.Common.Serialization;
using EVEMon.Common.Serialization.Settings;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents a character from CCP, with additional capacities for training and such
    /// </summary>
    public sealed class CCPCharacter : Character
    {
        private readonly SkillQueue m_queue;
        private readonly CharacterQueryMonitor<SerializableSkillQueue> m_skillQueueMonitor;
        private readonly CharacterQueryMonitor<SerializableAPICharacter> m_charSheetMonitor;
        private readonly CharacterQueryMonitor<SerializableAPIOrderList> m_charMarketOrdersMonitor;
        private readonly CharacterQueryMonitor<SerializableAPIOrderList> m_corpMarketOrdersMonitor;
        private readonly CharacterQueryMonitor<SerializableAPIJobList> m_charIndustryJobsMonitor;
        private readonly CharacterQueryMonitor<SerializableAPIJobList> m_corpIndustryJobsMonitor;
        private readonly MarketOrderCollection m_marketOrders;
        private readonly IndustryJobCollection m_industryJobs;
        private readonly QueryMonitorCollection m_monitors;

        private List<SerializableAPIOrder> m_orders = new List<SerializableAPIOrder>();
        private List<SerializableAPIJob> m_jobs = new List<SerializableAPIJob>();
        private APIMethods m_errorNotifiedMethod;

        private bool m_charOrdersUpdated;
        private bool m_corpOrdersUpdated;
        private bool m_charOrdersAdded;
        private bool m_corpOrdersAdded;

        private bool m_charJobsUpdated;
        private bool m_corpJobsUpdated;
        private bool m_charJobsAdded;
        private bool m_corpJobsAdded;

        /// <summary>
        /// Base constructor.
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="guid"></param>
        private CCPCharacter(CharacterIdentity identity, Guid guid)
            : base(identity, guid)
        {
            m_queue = new SkillQueue(this);
            m_marketOrders = new MarketOrderCollection(this);
            m_industryJobs = new IndustryJobCollection(this);
            m_monitors = new QueryMonitorCollection();

            // Initializes the query monitors 
            m_charSheetMonitor = new CharacterQueryMonitor<SerializableAPICharacter>(this, APIMethods.CharacterSheet);
            m_charSheetMonitor.Updated += new QueryCallback<SerializableAPICharacter>(OnCharacterSheetUpdated);
            m_monitors.Add(m_charSheetMonitor);

            m_skillQueueMonitor = new CharacterQueryMonitor<SerializableSkillQueue>(this, APIMethods.SkillQueue);
            m_skillQueueMonitor.Updated += new QueryCallback<SerializableSkillQueue>(OnSkillQueueUpdated);
            m_monitors.Add(m_skillQueueMonitor);

            m_charMarketOrdersMonitor = new CharacterQueryMonitor<SerializableAPIOrderList>(this, APIMethods.MarketOrders);
            m_charMarketOrdersMonitor.Updated += new QueryCallback<SerializableAPIOrderList>(OnCharacterMarketOrdersUpdated);
            m_monitors.Add(m_charMarketOrdersMonitor);

            m_corpMarketOrdersMonitor = new CharacterQueryMonitor<SerializableAPIOrderList>(this, APIMethods.CorporationMarketOrders);
            m_corpMarketOrdersMonitor.Updated += new QueryCallback<SerializableAPIOrderList>(OnCorporationMarketOrdersUpdated);
            m_monitors.Add(m_corpMarketOrdersMonitor);

            m_charIndustryJobsMonitor = new CharacterQueryMonitor<SerializableAPIJobList>(this, APIMethods.IndustryJobs);
            m_charIndustryJobsMonitor.Updated += new QueryCallback<SerializableAPIJobList>(OnCharacterJobsUpdated);
            m_monitors.Add(m_charIndustryJobsMonitor);

            m_corpIndustryJobsMonitor = new CharacterQueryMonitor<SerializableAPIJobList>(this, APIMethods.CorporationIndustryJobs);
            m_corpIndustryJobsMonitor.Updated += new QueryCallback<SerializableAPIJobList>(OnCorporationJobsUpdated);
            m_monitors.Add(m_corpIndustryJobsMonitor);
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
        }

        /// <summary>
        /// Gets an adorned name, with (file), (url) or (cached) labels.
        /// </summary>
        public override string AdornedName
        {
            get 
            {
                if (m_charSheetMonitor.LastResult != null && m_charSheetMonitor.LastResult.HasError)
                    return String.Format("{0} (cached)", m_name);

                return m_name; 
            }
        }

        /// <summary>
        /// Gets the skill queue for this character
        /// </summary>
        public SkillQueue SkillQueue
        {
            get { return m_queue; }
        }

        /// <summary>
        /// Gets the collection of market orders.
        /// </summary>
        public MarketOrderCollection MarketOrders
        {
            get { return m_marketOrders; }
        }

        /// <summary>
        /// Gets the collection of industry jobs.
        /// </summary>
        public IndustryJobCollection IndustryJobs
        {
            get { return m_industryJobs; }
        }

        /// <summary>
        /// Gets true when the character is currently actively training, false otherwise
        /// </summary>
        public override bool IsTraining
        {
            get { return m_queue.IsTraining; }
        }

        /// <summary>
        /// Gets the skill currently in training, even when it is paused.
        /// </summary>
        public override QueuedSkill CurrentlyTrainingSkill
        {
            get { return m_queue.CurrentlyTraining; }
        }

        /// <summary>
        /// Gets true when the character is in a NPC corporation, false otherwise.
        /// </summary>
        public bool IsInNPCCorporation
        {
            get { return StaticGeography.AllStations.Any(x => x.CorporationID == this.CorporationID); }
        }

        /// <summary>
        /// Gets true when character has insufficient balance to complete its buy orders.
        /// </summary>
        public bool HasSufficientBalance
        {
            get
            {
                var activeBuyOrdersIssuedForCharacter = m_marketOrders.Where(x => (x.State == OrderState.Active || x.State == OrderState.Modified) 
                    && x is BuyOrder && x.IssuedFor == IssuedFor.Character);
                decimal additionalToCover = activeBuyOrdersIssuedForCharacter.Sum(x => x.TotalPrice) - activeBuyOrdersIssuedForCharacter.Sum(x => ((BuyOrder)x).Escrow);
                
                return m_balance >= additionalToCover;
            }
        }

        /// <summary>
        /// Create a serializable character sheet for this character
        /// </summary>
        /// <returns></returns>
        public override SerializableSettingsCharacter Export()
        {
            var serial = new SerializableCCPCharacter();
            Export(serial);

            // Skill queue
            serial.SkillQueue = m_queue.Export();
            
            // Market orders
            serial.MarketOrders = m_marketOrders.Export();

            // Industry jobs
            serial.IndustryJobs = m_industryJobs.Export();

            // Last API updates
            foreach (var monitor in m_monitors)
            {
                var update = new SerializableAPIUpdate { Method = monitor.Method, Time = monitor.LastUpdate };
                serial.LastUpdates.Add(update);
            }
            
            return serial;
        }

        /// <summary>
        /// Imports data from a serialization object
        /// </summary>
        /// <param name="serial"></param>
        public void Import(SerializableCCPCharacter serial)
        {
            Import((SerializableSettingsCharacter)serial);

            // Skill queue
            m_queue.Import(serial.SkillQueue);
            m_queue.UpdateOnTimerTick();
            
            // Market orders
            m_marketOrders.Import(serial.MarketOrders);
            
            // Industry jobs
            m_industryJobs.Import(serial.IndustryJobs);

            // Last API updates
            foreach(var lastUpdate in serial.LastUpdates)
            {
                var monitor = m_monitors[lastUpdate.Method] as IQueryMonitorEx;
                if (monitor != null)
                    monitor.Reset(lastUpdate.Time);
            }

            // Fire the global event
            EveClient.OnCharacterChanged(this);
        }

        #region Querying
        /// <summary>
        /// Gets the query monitors enumeration.
        /// </summary>
        public QueryMonitorCollection QueryMonitors
        {
            get { return m_monitors; }
        }

        /// <summary>
        /// Updates the character on a timer tick.
        /// </summary>
        internal override void UpdateOnOneSecondTick()
        {
            if (!this.Monitored)
                return;

            m_monitors.UpdateOnOneSecondTick();
            m_queue.UpdateOnTimerTick();
            m_industryJobs.UpdateOnTimerTick();
        }

        /// <summary>
        /// Processed the queried skill queue information.
        /// </summary>
        /// <param name="result"></param>
        private void OnCharacterSheetUpdated(APIResult<SerializableAPICharacter> result)
        {
            // Notify an error occured
            if (ShouldNotifyError(result, APIMethods.CharacterSheet))
                EveClient.Notifications.NotifyCharacterSheetError(this, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Imports the data
            this.Import(result);

            // Check the character has a sufficient clone or send a notification
            if (m_cloneSkillPoints < this.SkillPoints)
            {
                EveClient.Notifications.NotifyInsufficientClone(this);
            }
            else
            {
                EveClient.Notifications.InvalidateInsufficientClone(this);
            }
        }

        /// <summary>
        /// Processes the queried skill queue information.
        /// </summary>
        /// <param name="result"></param>
        private void OnSkillQueueUpdated(APIResult<SerializableSkillQueue> result)
        {
            // Notify an error occured
            if (ShouldNotifyError(result, APIMethods.SkillQueue))
                EveClient.Notifications.NotifySkillQueueError(this, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            m_queue.Import(result.Result.Queue);

            // Check the account is in training
            var account = m_identity.Account;
            account.CharacterInTraining();

            // Check the character has room in skill queue
            if (IsTraining && (SkillQueue.EndTime < DateTime.UtcNow.AddHours(24)))
            {
                EveClient.Notifications.NotifySkillQueueRoomAvailable(this);
            }
            else
            {
                EveClient.Notifications.InvalidateSkillQueueRoomAvailability(this);
            }
        }

        /// <summary>
        /// Processes the queried character's personal market orders.
        /// </summary>
        /// <param name="result"></param>
        /// <remarks>This method is sensitive to which market orders gets queried first</remarks>
        private void OnCharacterMarketOrdersUpdated(APIResult<SerializableAPIOrderList> result)
        {
            m_charOrdersUpdated = true;

            // Notify an error occured
            if (ShouldNotifyError(result, APIMethods.MarketOrders))
                EveClient.Notifications.NotifyCharacterMarketOrdersError(this, result);

            // Add orders to list
            m_charOrdersAdded = AddOrders(result, m_corpOrdersAdded, IssuedFor.Character);

            // Import the data if all queried and there are orders to import 
            if (m_corpOrdersUpdated && m_orders.Count != 0)
                Import(m_orders);
        }

        /// <summary>
        /// Processes the queried character's corporation market orders.
        /// </summary>
        /// <param name="result"></param>
        /// <remarks>This method is sensitive to which market orders gets queried first</remarks>
        private void OnCorporationMarketOrdersUpdated(APIResult<SerializableAPIOrderList> result)
        {
            m_corpOrdersUpdated = true;

            // Character is not in NPC corporation
            if (!IsInNPCCorporation)
            {
                // We don't want to be notified about corp roles error
                if (result.CCPError != null && !result.CCPError.IsOrdersRelatedCorpRolesError)
                {
                    // Notify an error occured
                    if (ShouldNotifyError(result, APIMethods.CorporationMarketOrders))
                        EveClient.Notifications.NotifyCorporationMarketOrdersError(this, result);
                }

                // Add orders to list
                m_corpOrdersAdded = AddOrders(result, m_charOrdersAdded, IssuedFor.Corporation);
            }

            // Import the data if all queried and there are orders to import
            if (m_charOrdersUpdated && m_orders.Count != 0)
                Import(m_orders);
        }

        /// <summary>
        /// Processes the queried character's personal industry jobs.
        /// </summary>
        /// <param name="result"></param>
        /// <remarks>This method is sensitive to which "issued for" jobs gets queried first</remarks>
        private void OnCharacterJobsUpdated(APIResult<SerializableAPIJobList> result)
        {
            m_charJobsUpdated = true;

            // Notify an error occured
            if (ShouldNotifyError(result, APIMethods.IndustryJobs))
                EveClient.Notifications.NotifyCharacterIndustryJobsError(this, result);

            // Add jobs to list
            m_charJobsAdded = AddJobs(result, m_corpJobsAdded, IssuedFor.Character);

            // Import the data if all queried and there are jobs to import 
            if (m_corpJobsUpdated && m_jobs.Count != 0)
                Import(m_jobs);
        }

        /// <summary>
        /// Processes the queried character's corporation industry jobs.
        /// </summary>
        /// <param name="result"></param>
        /// <remarks>This method is sensitive to which industry jobs gets queried first</remarks>
        private void OnCorporationJobsUpdated(APIResult<SerializableAPIJobList> result)
        {
            m_corpJobsUpdated = true;

            // Character is not in NPC corporation
            if (!IsInNPCCorporation)
            {
                // We don't want to be notified about corp roles error
                if (result.CCPError != null && !result.CCPError.IsJobsRelatedCorpRolesError)
                {
                    // Notify an error occured
                    if (ShouldNotifyError(result, APIMethods.CorporationMarketOrders))
                        EveClient.Notifications.NotifyCorporationIndustryJobsError(this, result);
                }

                // Add jobs to list
                m_corpJobsAdded = AddJobs(result, m_charJobsAdded, IssuedFor.Corporation);
            }

            // Import the data if all queried and there are jobs to import
            if (m_charJobsUpdated && m_jobs.Count != 0)
                Import(m_jobs);
        }

        /// <summary>
        /// Checks whether we should notify an error.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool ShouldNotifyError(IAPIResult result, APIMethods method)
        {
            // Notify an error occured
            if (result.HasError)
            {
                if (m_errorNotifiedMethod != APIMethods.None)
                    return false;

                m_errorNotifiedMethod = method;
                return true;
            }

            // Removes the previous error notification.
            if (m_errorNotifiedMethod == method)
            {
                EveClient.Notifications.InvalidateCharacterAPIError(this);
                m_errorNotifiedMethod = APIMethods.None;
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
        private bool AddOrders(APIResult<SerializableAPIOrderList> result, bool ordersAdded, IssuedFor issuedFor)
        {           
            // Add orders if there isn't an error
            if (result.HasError)
                return false;

            // Check to see if other market
            // orders have been added before
            if (!ordersAdded)
                m_orders.Clear();

            // Add orders in list
            result.Result.Orders.ForEach(x => x.IssuedFor = issuedFor);
            m_orders.AddRange(result.Result.Orders);

            return true;
        }

        /// <summary>
        /// Import the orders from both market orders querying.
        /// </summary>
        /// <param name="orders"></param>
        private void Import(List<SerializableAPIOrder> orders)
        {
            // Exclude orders that wheren't issued by this character
            var characterOrders = orders.Where(x => x.OwnerID == m_characterID);

            var endedOrders = new List<MarketOrder>();
            m_marketOrders.Import(characterOrders, endedOrders);

            NotifyOnEndedOrders(endedOrders);

            // Check the character has sufficient balance
            // for its buying orders or send a notification
            if (!HasSufficientBalance)
            {
                EveClient.Notifications.NotifyInsufficientBalance(this);
            }
            else
            {
                EveClient.Notifications.InvalidateInsufficientBalance(this);
            }

            // Reset flags
            m_charOrdersUpdated = false;
            m_corpOrdersUpdated = false;
            m_charOrdersAdded = false;
            m_corpOrdersAdded = false;
        }

        /// <summary>
        /// Notify the user which orders has ended.
        /// </summary>
        /// <param name="endedOrders"></param>
        private void NotifyOnEndedOrders(List<MarketOrder> endedOrders)
        {
            // Sends a notification
            if (endedOrders.Count != 0)
                EveClient.Notifications.NotifyMarkerOrdersEnding(this, endedOrders);

            // Fires the event regarding market orders update.
            EveClient.OnCharacterMarketOrdersChanged(this);
        }

        /// <summary>
        /// Add the queried jobs to a list.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="ordersAdded"></param>
        /// <param name="issuedFor"></param>
        /// <returns>True if jobs get added, false otherwise</returns>
        private bool AddJobs(APIResult<SerializableAPIJobList> result, bool jobsAdded, IssuedFor issuedFor)
        {
            // Add orders if there isn't an error
            if (result.HasError)
                return false;

            // Check to see if other market
            // orders have been added before
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
        /// <param name="orders"></param>
        private void Import(List<SerializableAPIJob> jobs)
        {
            // Exclude jobs that wheren't issued by this character
            var characterJobs = jobs.Where(x => x.InstallerID == m_characterID);

            m_industryJobs.Import(characterJobs);
            
            // Fires the event regarding industry jobs update.
            EveClient.OnCharacterIndustryJobsChanged(this);

            // Reset flags
            m_charJobsUpdated = false;
            m_corpJobsUpdated = false;
            m_charJobsAdded = false;
            m_corpJobsAdded = false;
        }
        
        #endregion
    }
}
