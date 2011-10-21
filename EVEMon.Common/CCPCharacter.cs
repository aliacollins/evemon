using System;
using System.Collections.Generic;
using System.Linq;
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

            m_characterDataQuerying = new CharacterDataQuerying(this);
            m_corporationDataQuerying = new CorporationDataQuerying(this);
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
        public DataQuerying CharacterDataQuerying
        {
            get { return m_characterDataQuerying; }
        }

        /// <summary>
        /// Gets the corporation data querying.
        /// </summary>
        /// <value>The corporation data querying.</value>
        public DataQuerying CorporationDataQuerying
        {
            get { return m_corporationDataQuerying; }
        }

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

    }
}