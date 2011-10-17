using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Net;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class CharacterDataQuerying : DataQuerying
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

        #endregion


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterDataQuerying"/> class.
        /// </summary>
        /// <param name="ccpCharacter">The CCP character.</param>
        public CharacterDataQuerying(CCPCharacter ccpCharacter)
            : base(ccpCharacter)
        {
            m_characterQueryMonitors = new List<IQueryMonitorEx>();

            // Initializes the query monitors 
            m_charSheetMonitor =
                new CharacterQueryMonitor<SerializableAPICharacterSheet>(ccpCharacter, APICharacterMethods.CharacterSheet);
            m_charSheetMonitor.Updated += OnCharacterSheetUpdated;
            m_characterQueryMonitors.Add(m_charSheetMonitor);

            m_skillQueueMonitor =
                new CharacterQueryMonitor<SerializableAPISkillQueue>(ccpCharacter, APICharacterMethods.SkillQueue);
            m_skillQueueMonitor.Updated += OnSkillQueueUpdated;
            m_characterQueryMonitors.Add(m_skillQueueMonitor);

            m_charStandingsMonitor =
                new CharacterQueryMonitor<SerializableAPIStandings>(ccpCharacter, APICharacterMethods.Standings);
            m_charStandingsMonitor.Updated += OnStandingsUpdated;
            m_characterQueryMonitors.Add(m_charStandingsMonitor);

            m_charMarketOrdersMonitor =
                new CharacterQueryMonitor<SerializableAPIMarketOrders>(ccpCharacter, APICharacterMethods.MarketOrders);
            m_charMarketOrdersMonitor.Updated += OnCharacterMarketOrdersUpdated;
            m_characterQueryMonitors.Add(m_charMarketOrdersMonitor);

            m_charIndustryJobsMonitor =
                new CharacterQueryMonitor<SerializableAPIIndustryJobs>(ccpCharacter, APICharacterMethods.IndustryJobs);
            m_charIndustryJobsMonitor.Updated += OnCharacterIndustryJobsUpdated;
            m_characterQueryMonitors.Add(m_charIndustryJobsMonitor);

            m_charResearchPointsMonitor =
                new CharacterQueryMonitor<SerializableAPIResearch>(ccpCharacter, APICharacterMethods.ResearchPoints)
                    { QueryOnStartup = true };
            m_charResearchPointsMonitor.Updated += OnCharacterResearchPointsUpdated;
            m_characterQueryMonitors.Add(m_charResearchPointsMonitor);

            m_charEVEMailMessagesMonitor =
                new CharacterQueryMonitor<SerializableAPIMailMessages>(ccpCharacter, APICharacterMethods.MailMessages)
                    { QueryOnStartup = true };
            m_charEVEMailMessagesMonitor.Updated += OnCharacterEVEMailMessagesUpdated;
            m_characterQueryMonitors.Add(m_charEVEMailMessagesMonitor);

            m_charEVENotificationsMonitor =
                new CharacterQueryMonitor<SerializableAPINotifications>(ccpCharacter, APICharacterMethods.Notifications)
                    { QueryOnStartup = true };
            m_charEVENotificationsMonitor.Updated += OnCharacterEVENotificationsUpdated;
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
            APIKey apiKey = CCPCharacter.Identity.FindAPIKeyWithAccess(APICharacterMethods.MailingLists);
            if (apiKey == null)
                return;

            EveMonClient.APIProviders.CurrentProvider.QueryMethodAsync<SerializableAPIMailingLists>(
                APICharacterMethods.MailingLists,
                apiKey.ID,
                apiKey.VerificationCode,
                CCPCharacter.CharacterID,
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
            APIKey apiKey = CCPCharacter.Identity.FindAPIKeyWithAccess(APICharacterMethods.CharacterInfo);
            if (apiKey == null)
                return;

            EveMonClient.APIProviders.CurrentProvider.QueryMethodAsync<SerializableAPICharacterInfo>(
                APICharacterMethods.CharacterInfo,
                apiKey.ID,
                apiKey.VerificationCode,
                CCPCharacter.CharacterID,
                OnCharacterInfoUpdated);
        }

        /// <summary>
        /// Processed the queried skill queue information.
        /// </summary>
        /// <param name="result"></param>
        private void OnCharacterSheetUpdated(APIResult<SerializableAPICharacterSheet> result)
        {
            // Notify an error occurred
            if (ShouldNotifyError(result, APICharacterMethods.CharacterSheet))
                EveMonClient.Notifications.NotifyCharacterSheetError(CCPCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Query the Character's info
            QueryCharacterInfo();

            // Imports the data
            CCPCharacter.Import(result);

            // Check the character has a sufficient clone or send a notification
            if (CCPCharacter.Monitored && (CCPCharacter.CloneSkillPoints < CCPCharacter.SkillPoints))
                EveMonClient.Notifications.NotifyInsufficientClone(CCPCharacter);
            else
                EveMonClient.Notifications.InvalidateInsufficientClone(CCPCharacter);

            // Check for claimable certificates
            IEnumerable<Certificate> claimableCertificates = CCPCharacter.Certificates.Where(x => x.CanBeClaimed);
            if (CCPCharacter.Monitored && claimableCertificates.Count() > 0)
                EveMonClient.Notifications.NotifyClaimableCertificate(CCPCharacter, claimableCertificates);
            else
                EveMonClient.Notifications.InvalidateClaimableCertificate(CCPCharacter);
        }

        /// <summary>
        /// Processes the queried character's info.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnCharacterInfoUpdated(APIResult<SerializableAPICharacterInfo> result)
        {
            // Notify an error occured
            if (ShouldNotifyError(result, APICharacterMethods.CharacterInfo))
                EveMonClient.Notifications.NotifyCharacterInfoError(CCPCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            CCPCharacter.Import(result.Result);
        }

        /// <summary>
        /// Processes the queried skill queue information.
        /// </summary>
        /// <param name="result"></param>
        private void OnSkillQueueUpdated(APIResult<SerializableAPISkillQueue> result)
        {
            // Notify an error occurred
            if (ShouldNotifyError(result, APICharacterMethods.SkillQueue))
                EveMonClient.Notifications.NotifySkillQueueError(CCPCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            CCPCharacter.SkillQueue.Import(result.Result.Queue);

            // Check the account has a character in training (if API key of type "Account")
            APIKey apikey = CCPCharacter.Identity.FindAPIKeyWithAccess(APICharacterMethods.CharacterSkillInTraining);
            if (apikey != null)
                apikey.CharacterInTraining();

            // Check the character has room in skill queue
            if (CCPCharacter.IsTraining && (CCPCharacter.SkillQueue.EndTime < DateTime.UtcNow.AddHours(24)))
            {
                EveMonClient.Notifications.NotifySkillQueueRoomAvailable(CCPCharacter);
                return;
            }

            EveMonClient.Notifications.InvalidateSkillQueueRoomAvailability(CCPCharacter);
        }

        /// <summary>
        /// Processes the queried skill queue information.
        /// </summary>
        /// <param name="result"></param>
        private void OnStandingsUpdated(APIResult<SerializableAPIStandings> result)
        {
            // Notify an error occurred
            if (ShouldNotifyError(result, APICharacterMethods.Standings))
                EveMonClient.Notifications.NotifyCharacterStandingsError(CCPCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            CCPCharacter.Standings.Import(result.Result.CharacterNPCStandings.All);
        }

        /// <summary>
        /// Processes the queried character's research points.
        /// </summary>
        /// <param name="result"></param>
        private void OnCharacterResearchPointsUpdated(APIResult<SerializableAPIResearch> result)
        {
            // Notify an error occured
            if (ShouldNotifyError(result, APICharacterMethods.ResearchPoints))
                EveMonClient.Notifications.NotifyResearchPointsError(CCPCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            CCPCharacter.ResearchPoints.Import(result.Result.ResearchPoints);
        }

        /// <summary>
        /// Processes the queried character's EVE mail messages.
        /// </summary>
        /// <param name="result"></param>
        private void OnCharacterEVEMailMessagesUpdated(APIResult<SerializableAPIMailMessages> result)
        {
            // Notify an error occured
            if (ShouldNotifyError(result, APICharacterMethods.MailMessages))
                EveMonClient.Notifications.NotifyEVEMailMessagesError(CCPCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Each time we import a new batch of EVE mail messages,
            // query the mailing lists so that we are always up to date
            QueryCharacterMailingLists();

            // Import the data
            CCPCharacter.EVEMailMessages.Import(result.Result.Messages);

            // Notify on new messages
            if (CCPCharacter.EVEMailMessages.NewMessages != 0)
                EveMonClient.Notifications.NotifyNewEVEMailMessages(CCPCharacter, CCPCharacter.EVEMailMessages.NewMessages);
        }

        /// <summary>
        /// Processes the queried character's EVE mailing lists.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnCharacterMailingListsUpdated(APIResult<SerializableAPIMailingLists> result)
        {
            // Notify an error occured
            if (ShouldNotifyError(result, APICharacterMethods.MailingLists))
                EveMonClient.Notifications.NotifyMailingListsError(CCPCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            CCPCharacter.EVEMailingLists.Import(result.Result.MailingLists);
        }

        /// <summary>
        /// Processes the queried character's EVE notifications.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnCharacterEVENotificationsUpdated(APIResult<SerializableAPINotifications> result)
        {
            // Notify an error occured
            if (ShouldNotifyError(result, APICharacterMethods.Notifications))
                EveMonClient.Notifications.NotifyEVENotificationsError(CCPCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            CCPCharacter.EVENotifications.Import(result.Result.Notifications);

            // Notify on new messages
            if (CCPCharacter.EVENotifications.NewNotifications != 0)
                EveMonClient.Notifications.NotifyNewEVENotifications(CCPCharacter,
                                                                     CCPCharacter.EVENotifications.NewNotifications);
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
            m_basicFeaturesMonitors.ForEach(monitor => monitor.Enabled = CCPCharacter.Monitored &&
                                                                         CCPCharacter.QueryMonitors.Contains(monitor));
        }

        /// <summary>
        /// Handles the CharacterListUpdated event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EVEMon.Common.CustomEventArgs.APIKeyInfoChangedEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_CharacterListUpdated(object sender, APIKeyInfoChangedEventArgs e)
        {
            if (!CCPCharacter.Identity.APIKeys.Contains(e.APIKey))
                return;

            if (e.APIKey.Type == APIKeyType.Corporation &&
                CCPCharacter.Identity.APIKeys.All(apiKey => apiKey.Type == APIKeyType.Corporation) &&
                m_characterQueryMonitors.Exists(monitor => CCPCharacter.QueryMonitors.Contains(monitor)))
            {
                m_characterQueryMonitors.ForEach(monitor => CCPCharacter.QueryMonitors.Remove(monitor));
                return;
            }

            if ((e.APIKey.Type != APIKeyType.Account && e.APIKey.Type != APIKeyType.Character) ||
                m_characterQueryMonitors.Exists(monitor => CCPCharacter.QueryMonitors.Contains(monitor)))
                return;

            m_characterQueryMonitors.ForEach(monitor => CCPCharacter.QueryMonitors.Add(monitor));
        }

        #endregion
    }
}