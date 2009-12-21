using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
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
        private readonly CharacterQueryMonitor<SerializableAPIOrderList> m_marketOrdersMonitor;
        private readonly MarketOrderCollection m_marketOrders;
        private readonly QueryMonitorCollection m_monitors;

        private APIMethods m_errorNotifiedMethod;

        /// <summary>
        /// Base constructor
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="guid"></param>
        private CCPCharacter(CharacterIdentity identity, Guid guid)
            : base(identity, guid)
        {
            m_queue = new SkillQueue(this);
            m_marketOrders = new MarketOrderCollection(this);
            m_monitors = new QueryMonitorCollection();

            // Initializes the query monitors 
            m_charSheetMonitor = new CharacterQueryMonitor<SerializableAPICharacter>(this, APIMethods.CharacterSheet);
            m_charSheetMonitor.Updated += new QueryCallback<SerializableAPICharacter>(OnCharacterSheetUpdated);
            m_monitors.Add(m_charSheetMonitor);

            m_skillQueueMonitor = new CharacterQueryMonitor<SerializableSkillQueue>(this, APIMethods.SkillQueue);
            m_skillQueueMonitor.Updated += new QueryCallback<SerializableSkillQueue>(OnSkillQueueUpdated);
            m_monitors.Add(m_skillQueueMonitor);

            m_marketOrdersMonitor = new CharacterQueryMonitor<SerializableAPIOrderList>(this, APIMethods.MarketOrders);
            m_marketOrdersMonitor.Updated += new QueryCallback<SerializableAPIOrderList>(OnMarketOrdersUpdated);
            m_monitors.Add(m_marketOrdersMonitor);
        }

        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="identity">The identitiy for this character</param>
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
                if (m_charSheetMonitor.LastResult != null && m_charSheetMonitor.LastResult.HasError) return m_name + " (cached)";
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
        /// Create a serializable character sheet for this character
        /// </summary>
        /// <returns></returns>
        public override SerializableSettingsCharacter Export()
        {
            var serial = new SerializableCCPCharacter();
            Export(serial);

            // Last API updates
            foreach (var monitor in m_monitors)
            {
                var update = new SerializableAPIUpdate { Method = monitor.Method, Time = monitor.LastUpdate };
                serial.LastUpdates.Add(update);
            }

            // Skill queue
            serial.MarketOrders = m_marketOrders.Export();
            serial.SkillQueue = m_queue.Export();
            return serial;
        }

        /// <summary>
        /// Imports data from a serialization object
        /// </summary>
        /// <param name="serial"></param>
        public void Import(SerializableCCPCharacter serial)
        {
            Import((SerializableSettingsCharacter)serial);

            // Training queue
            m_marketOrders.Import(serial.MarketOrders);
            m_queue.Import(serial.SkillQueue);
            m_queue.UpdateOnTimerTick();

            // Last API updates
            foreach(var lastUpdate in serial.LastUpdates)
            {
                var monitor = m_monitors[lastUpdate.Method] as IQueryMonitorEx;
                if (monitor != null) monitor.Reset(lastUpdate.Time);
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
        /// Updates the character on a timer tick
        /// </summary>
        internal override void UpdateOnOneSecondTick()
        {
            if (!this.Monitored) return;
            m_monitors.UpdateOnOneSecondTick();
            m_queue.UpdateOnTimerTick();
        }

        /// <summary>
        /// Processed the queried skill queue information.
        /// </summary>
        /// <param name="result"></param>
        private void OnCharacterSheetUpdated(APIResult<SerializableAPICharacter> result)
        {
            // Notify an error occured
            if (ShouldNotifyError(result, APIMethods.CharacterSheet))
            {
                EveClient.Notifications.NotifyCharacterSheetError(this, result);
            }

            // Quits if there is an error
            if (result.HasError) return;

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
            {
                EveClient.Notifications.NotifySkillQueueError(this, result);
            }

            // Quits if there is an error
            if (result.HasError) return;

            // Import the data
            m_queue.Import(result.Result.Queue);

            // Check the account is in training or send a notification
            var account = m_identity.Account;
            if (account.TrainingCharacter == null && !account.CharacterIdentities.IsEmpty())
            {
                EveClient.Notifications.NotifyAccountNotInTraining(account);
            }
            else
            {
                EveClient.Notifications.InvalidateAccountNotInTraining(account);
            }

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
        /// Processes the queried market orders.
        /// </summary>
        /// <param name="result"></param>
        private void OnMarketOrdersUpdated(APIResult<SerializableAPIOrderList> result)
        {
            // Notify an error occured
            if (ShouldNotifyError(result, APIMethods.MarketOrders))
            {
                EveClient.Notifications.NotifyMarketOrdersError(this, result);
            }

            // Quits if there is an error
            if (result.HasError) return;

            // Import the data
            var endedOrders = new List<MarketOrder>();
            m_marketOrders.Import(result.Result.Orders, endedOrders);

            // Sends a notification
            if (endedOrders.Count != 0)
            {
                EveClient.Notifications.NotifyMarkerOrdersEnding(this, endedOrders);
            }

            // Fires the event regarding market orders update.
            EveClient.OnCharacterMarketOrdersChanged(this);
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
                if (m_errorNotifiedMethod != APIMethods.None) return false;

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
        #endregion
    }
}
