using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Net;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class CharacterDataQuerying
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
        private readonly List<IQueryMonitorEx> m_characterQueryMonitors;
        private readonly List<IQueryMonitor> m_basicFeaturesMonitors;
        private readonly CCPCharacter m_ccpCharacter;

        private bool m_charMarketOrdersQueried;
        private bool m_charIndustryJobsQueried;

        #endregion


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterDataQuerying"/> class.
        /// </summary>
        /// <param name="ccpCharacter">The CCP character.</param>
        public CharacterDataQuerying(CCPCharacter ccpCharacter)
        {
            m_ccpCharacter = ccpCharacter;
            m_characterQueryMonitors = new List<IQueryMonitorEx>();

            // Initializes the query monitors 
            m_charSheetMonitor =
                new CharacterQueryMonitor<SerializableAPICharacterSheet>(ccpCharacter, APICharacterMethods.CharacterSheet,
                                                                         OnCharacterSheetUpdated);
            m_characterQueryMonitors.Add(m_charSheetMonitor);

            m_skillQueueMonitor =
                new CharacterQueryMonitor<SerializableAPISkillQueue>(ccpCharacter, APICharacterMethods.SkillQueue,
                                                                     OnSkillQueueUpdated);
            m_characterQueryMonitors.Add(m_skillQueueMonitor);

            m_charStandingsMonitor =
                new CharacterQueryMonitor<SerializableAPIStandings>(ccpCharacter, APICharacterMethods.Standings,
                                                                    OnStandingsUpdated) { QueryOnStartup = true };
            m_characterQueryMonitors.Add(m_charStandingsMonitor);

            m_charMarketOrdersMonitor =
                new CharacterQueryMonitor<SerializableAPIMarketOrders>(ccpCharacter, APICharacterMethods.MarketOrders,
                                                                       OnCharacterMarketOrdersUpdated);
            m_characterQueryMonitors.Add(m_charMarketOrdersMonitor);

            m_charIndustryJobsMonitor =
                new CharacterQueryMonitor<SerializableAPIIndustryJobs>(ccpCharacter, APICharacterMethods.IndustryJobs,
                                                                       OnCharacterIndustryJobsUpdated);
            m_characterQueryMonitors.Add(m_charIndustryJobsMonitor);

            m_charResearchPointsMonitor =
                new CharacterQueryMonitor<SerializableAPIResearch>(ccpCharacter, APICharacterMethods.ResearchPoints,
                                                                   OnCharacterResearchPointsUpdated) { QueryOnStartup = true };
            m_characterQueryMonitors.Add(m_charResearchPointsMonitor);

            m_charEVEMailMessagesMonitor =
                new CharacterQueryMonitor<SerializableAPIMailMessages>(ccpCharacter, APICharacterMethods.MailMessages,
                                                                       OnCharacterEVEMailMessagesUpdated)
                    { QueryOnStartup = true };
            m_characterQueryMonitors.Add(m_charEVEMailMessagesMonitor);

            m_charEVENotificationsMonitor =
                new CharacterQueryMonitor<SerializableAPINotifications>(ccpCharacter, APICharacterMethods.Notifications,
                                                                        OnCharacterEVENotificationsUpdated)
                    { QueryOnStartup = true };
            m_characterQueryMonitors.Add(m_charEVENotificationsMonitor);

            m_basicFeaturesMonitors = m_characterQueryMonitors.Cast<IQueryMonitor>().Select(
                monitor => new { monitor, method = (APICharacterMethods)monitor.Method }).Where(
                    monitor =>
                    (int)monitor.method == ((int)monitor.method & (int)APIMethodsExtensions.BasicCharacterFeatures)).Select(
                        basicFeature => basicFeature.monitor).ToList();

            m_characterQueryMonitors.ForEach(monitor => ccpCharacter.QueryMonitors.Add(monitor));

            EveMonClient.TimerTick += EveMonClient_TimerTick;
            EveMonClient.CharacterListUpdated += EveMonClient_CharacterListUpdated;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets the character sheet monitor.
        /// </summary>
        /// <value>The character sheet monitor.</value>
        public CharacterQueryMonitor<SerializableAPICharacterSheet> CharacterSheetMonitor
        {
            get { return m_charSheetMonitor; }
        }

        /// <summary>
        /// Gets the skill queue monitor.
        /// </summary>
        /// <value>The skill queue monitor.</value>
        public CharacterQueryMonitor<SerializableAPISkillQueue> SkillQueueMonitor
        {
            get { return m_skillQueueMonitor; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [character market orders queried].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [character market orders queried]; otherwise, <c>false</c>.
        /// </value>
        internal bool CharacterMarketOrdersQueried
        {
            get
            {
                // If character can not query character related data
                // or character market orders monitor is not enabled
                // we switch the flag
                IQueryMonitor charMarketOrdersMonitor =
                    m_ccpCharacter.QueryMonitors[APICharacterMethods.MarketOrders.ToString()];
                return m_charMarketOrdersQueried |= charMarketOrdersMonitor == null || !charMarketOrdersMonitor.Enabled;
            }
            set { m_charMarketOrdersQueried = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [character industry jobs queried].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [character industry jobs queried]; otherwise, <c>false</c>.
        /// </value>
        internal bool CharacterIndustryJobsQueried
        {
            get
            {
                // If character can not query character related data
                // or character industry jobs monitor is not enabled
                // we switch the flag
                IQueryMonitor charIndustryJobsMonitor =
                    m_ccpCharacter.QueryMonitors[APICharacterMethods.IndustryJobs.ToString()];
                return m_charIndustryJobsQueried |= charIndustryJobsMonitor == null || !charIndustryJobsMonitor.Enabled;
            }
            set { m_charIndustryJobsQueried = value; }
        }

        #endregion


        #region Querying

        /// <summary>
        /// Queries the character's mailing lists.
        /// </summary>
        private void QueryCharacterMailingLists()
        {
            // Quits if no network
            if (!NetworkMonitor.IsNetworkAvailable)
                return;

            // Quits if access denied
            APIKey apiKey = m_ccpCharacter.Identity.FindAPIKeyWithAccess(APICharacterMethods.MailingLists);
            if (apiKey == null)
                return;

            EveMonClient.APIProviders.CurrentProvider.QueryMethodAsync<SerializableAPIMailingLists>(
                APICharacterMethods.MailingLists,
                apiKey.ID,
                apiKey.VerificationCode,
                m_ccpCharacter.CharacterID,
                OnCharacterMailingListsUpdated);
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
            APIKey apiKey = m_ccpCharacter.Identity.FindAPIKeyWithAccess(APICharacterMethods.CharacterInfo);
            if (apiKey == null)
                return;

            EveMonClient.APIProviders.CurrentProvider.QueryMethodAsync<SerializableAPICharacterInfo>(
                APICharacterMethods.CharacterInfo,
                apiKey.ID,
                apiKey.VerificationCode,
                m_ccpCharacter.CharacterID,
                OnCharacterInfoUpdated);
        }

        /// <summary>
        /// Processed the queried skill queue information.
        /// </summary>
        /// <param name="result"></param>
        private void OnCharacterSheetUpdated(APIResult<SerializableAPICharacterSheet> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null)
                return;

            // Notify an error occurred
            if (m_ccpCharacter.ShouldNotifyError(result, APICharacterMethods.CharacterSheet))
                EveMonClient.Notifications.NotifyCharacterSheetError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Query the Character's info
            QueryCharacterInfo();

            // Imports the data
            m_ccpCharacter.Import(result);

            // Check the character has a sufficient clone or send a notification
            if (m_ccpCharacter.Monitored && (m_ccpCharacter.CloneSkillPoints < m_ccpCharacter.SkillPoints))
                EveMonClient.Notifications.NotifyInsufficientClone(m_ccpCharacter);
            else
                EveMonClient.Notifications.InvalidateInsufficientClone(m_ccpCharacter);

            // Check for claimable certificates
            IEnumerable<Certificate> claimableCertificates = m_ccpCharacter.Certificates.Where(x => x.CanBeClaimed);
            if (m_ccpCharacter.Monitored && claimableCertificates.Any())
                EveMonClient.Notifications.NotifyClaimableCertificate(m_ccpCharacter, claimableCertificates);
            else
                EveMonClient.Notifications.InvalidateClaimableCertificate(m_ccpCharacter);
        }

        /// <summary>
        /// Processes the queried character's info.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnCharacterInfoUpdated(APIResult<SerializableAPICharacterInfo> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null)
                return;

            // Notify an error occured
            if (m_ccpCharacter.ShouldNotifyError(result, APICharacterMethods.CharacterInfo))
                EveMonClient.Notifications.NotifyCharacterInfoError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            m_ccpCharacter.Import(result.Result);
        }

        /// <summary>
        /// Processes the queried skill queue information.
        /// </summary>
        /// <param name="result"></param>
        private void OnSkillQueueUpdated(APIResult<SerializableAPISkillQueue> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null)
                return;

            // Notify an error occurred
            if (m_ccpCharacter.ShouldNotifyError(result, APICharacterMethods.SkillQueue))
                EveMonClient.Notifications.NotifySkillQueueError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            m_ccpCharacter.SkillQueue.Import(result.Result.Queue);

            // Check the account has a character in training (if API key of type "Account")
            APIKey apikey = m_ccpCharacter.Identity.FindAPIKeyWithAccess(APICharacterMethods.CharacterSkillInTraining);
            if (apikey != null)
                apikey.CharacterInTraining();

            // Check the character has room in skill queue
            if (m_ccpCharacter.IsTraining && (m_ccpCharacter.SkillQueue.EndTime < DateTime.UtcNow.AddHours(24)))
            {
                EveMonClient.Notifications.NotifySkillQueueRoomAvailable(m_ccpCharacter);
                return;
            }

            EveMonClient.Notifications.InvalidateSkillQueueRoomAvailability(m_ccpCharacter);
        }

        /// <summary>
        /// Processes the queried skill queue information.
        /// </summary>
        /// <param name="result"></param>
        private void OnStandingsUpdated(APIResult<SerializableAPIStandings> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null)
                return;

            // Notify an error occurred
            if (m_ccpCharacter.ShouldNotifyError(result, APICharacterMethods.Standings))
                EveMonClient.Notifications.NotifyCharacterStandingsError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            m_ccpCharacter.Standings.Import(result.Result.CharacterNPCStandings.All);
        }

        /// <summary>
        /// Processes the queried character's personal market orders.
        /// </summary>
        /// <param name="result"></param>
        /// <remarks>This method is sensitive to which "issued for" jobs gets queried first</remarks>
        private void OnCharacterMarketOrdersUpdated(APIResult<SerializableAPIMarketOrders> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null)
                return;

            CharacterMarketOrdersQueried = true;

            // Notify an error occurred
            if (m_ccpCharacter.ShouldNotifyError(result, APICharacterMethods.MarketOrders))
                EveMonClient.Notifications.NotifyCharacterMarketOrdersError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            result.Result.Orders.ToList().ForEach(order => order.IssuedFor = IssuedFor.Character);

            // Import the data
            List<MarketOrder> endedOrders = new List<MarketOrder>();
            m_ccpCharacter.CharacterMarketOrders.Import(result.Result.Orders, endedOrders);

            // Fires the event regarding character market orders update
            EveMonClient.OnCharacterMarketOrdersUpdated(m_ccpCharacter, endedOrders);
        }

        /// <summary>
        /// Processes the queried character's personal industry jobs.
        /// </summary>
        /// <param name="result"></param>
        /// <remarks>This method is sensitive to which "issued for" jobs gets queried first</remarks>
        private void OnCharacterIndustryJobsUpdated(APIResult<SerializableAPIIndustryJobs> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null)
                return;

            CharacterIndustryJobsQueried = true;

            // Notify an error occurred
            if (m_ccpCharacter.ShouldNotifyError(result, APICharacterMethods.IndustryJobs))
                EveMonClient.Notifications.NotifyCharacterIndustryJobsError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            result.Result.Jobs.ToList().ForEach(x => x.IssuedFor = IssuedFor.Character);

            // Import the data
            m_ccpCharacter.CharacterIndustryJobs.Import(result.Result.Jobs);

            // Fires the event regarding character industry jobs update
            EveMonClient.OnCharacterIndustryJobsUpdated(m_ccpCharacter);
        }

        /// <summary>
        /// Processes the queried character's research points.
        /// </summary>
        /// <param name="result"></param>
        private void OnCharacterResearchPointsUpdated(APIResult<SerializableAPIResearch> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null)
                return;

            // Notify an error occured
            if (m_ccpCharacter.ShouldNotifyError(result, APICharacterMethods.ResearchPoints))
                EveMonClient.Notifications.NotifyResearchPointsError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            m_ccpCharacter.ResearchPoints.Import(result.Result.ResearchPoints);
        }

        /// <summary>
        /// Processes the queried character's EVE mail messages.
        /// </summary>
        /// <param name="result"></param>
        private void OnCharacterEVEMailMessagesUpdated(APIResult<SerializableAPIMailMessages> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null)
                return;

            // Notify an error occured
            if (m_ccpCharacter.ShouldNotifyError(result, APICharacterMethods.MailMessages))
                EveMonClient.Notifications.NotifyEVEMailMessagesError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Each time we import a new batch of EVE mail messages,
            // query the mailing lists so that we are always up to date
            QueryCharacterMailingLists();

            // Import the data
            m_ccpCharacter.EVEMailMessages.Import(result.Result.Messages);

            // Notify on new messages
            if (m_ccpCharacter.EVEMailMessages.NewMessages != 0)
                EveMonClient.Notifications.NotifyNewEVEMailMessages(m_ccpCharacter, m_ccpCharacter.EVEMailMessages.NewMessages);
        }

        /// <summary>
        /// Processes the queried character's EVE mailing lists.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnCharacterMailingListsUpdated(APIResult<SerializableAPIMailingLists> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null)
                return;

            // Notify an error occured
            if (m_ccpCharacter.ShouldNotifyError(result, APICharacterMethods.MailingLists))
                EveMonClient.Notifications.NotifyMailingListsError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            m_ccpCharacter.EVEMailingLists.Import(result.Result.MailingLists);
        }

        /// <summary>
        /// Processes the queried character's EVE notifications.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnCharacterEVENotificationsUpdated(APIResult<SerializableAPINotifications> result)
        {
            // Character may have been deleted or set to not be monitored since we queried
            if (m_ccpCharacter == null)
                return;

            // Notify an error occured
            if (m_ccpCharacter.ShouldNotifyError(result, APICharacterMethods.Notifications))
                EveMonClient.Notifications.NotifyEVENotificationsError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            m_ccpCharacter.EVENotifications.Import(result.Result.Notifications);

            // Notify on new notifications
            if (m_ccpCharacter.EVENotifications.NewNotifications != 0)
                EveMonClient.Notifications.NotifyNewEVENotifications(m_ccpCharacter,
                                                                     m_ccpCharacter.EVENotifications.NewNotifications);
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
            m_basicFeaturesMonitors.ForEach(monitor => monitor.Enabled = m_ccpCharacter.Monitored &&
                                                                         m_ccpCharacter.QueryMonitors.Contains(monitor));
        }

        /// <summary>
        /// Handles the CharacterListUpdated event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EVEMon.Common.CustomEventArgs.APIKeyInfoChangedEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_CharacterListUpdated(object sender, APIKeyInfoChangedEventArgs e)
        {
            if (!m_ccpCharacter.Identity.APIKeys.Contains(e.APIKey))
                return;

            if (e.APIKey.Type == APIKeyType.Corporation &&
                m_ccpCharacter.Identity.APIKeys.All(apiKey => apiKey.Type == APIKeyType.Corporation) &&
                m_characterQueryMonitors.Exists(monitor => m_ccpCharacter.QueryMonitors.Contains(monitor)))
            {
                m_characterQueryMonitors.ForEach(monitor => m_ccpCharacter.QueryMonitors.Remove(monitor));
                return;
            }

            if ((e.APIKey.Type != APIKeyType.Account && e.APIKey.Type != APIKeyType.Character) ||
                m_characterQueryMonitors.Exists(monitor => m_ccpCharacter.QueryMonitors.Contains(monitor)))
                return;

            m_characterQueryMonitors.ForEach(monitor => m_ccpCharacter.QueryMonitors.Add(monitor));
        }

        #endregion
    }
}