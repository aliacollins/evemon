using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Notifications;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    /// <summary>
    /// The collection used by <see cref="EveMonClient.Notifications"/>
    /// </summary>
    public sealed class GlobalNotificationCollection : ReadonlyCollection<NotificationEventArgs>
    {
        /// <summary>
        /// Constructor, used by <see cref="EveMonClient"/> only.
        /// </summary>
        internal GlobalNotificationCollection()
        {
        }

        /// <summary>
        /// Protected default constructor with an initial capacity.
        /// </summary>
        internal GlobalNotificationCollection(int capacity)
            : base(capacity)
        {
        }

        /// <summary>
        /// Adds a notification to this collection.
        /// </summary>
        /// <param name="notification"></param>
        public void Notify(NotificationEventArgs notification)
        {
            switch (notification.Behaviour)
            {
                case NotificationBehaviour.Cohabitate:
                    Items.Add(notification);
                    break;

                case NotificationBehaviour.Overwrite:
                    // Replace the previous notifications with the same invalidation key
                    InvalidateCore(notification.InvalidationKey);
                    Items.Add(notification);
                    break;

                case NotificationBehaviour.Merge:
                    // Merge the notifications with the same key
                    long key = notification.InvalidationKey;
                    foreach (NotificationEventArgs other in Items.Where(x => x.InvalidationKey == key))
                    {
                        notification.Append(other);
                    }

                    // Replace the previous notifications with the same invalidation key
                    InvalidateCore(key);
                    Items.Add(notification);
                    break;
            }

            EveMonClient.OnNotificationSent(notification);
        }

        /// <summary>
        /// Invalidates the notifications with the given key and notify an event.
        /// </summary>
        /// <param name="args"></param>
        public void Invalidate(NotificationInvalidationEventArgs args)
        {
            if (InvalidateCore(args.Key))
                EveMonClient.OnNotificationInvalidated(args);
        }

        /// <summary>
        /// Invalidates the notifications with the given key.
        /// </summary>
        /// <param name="key"></param>
        private bool InvalidateCore(long key)
        {
            int index = 0;
            bool foundAny = false;

            // Removes all the notifications with the given key
            while (index < Items.Count)
            {
                if (Items[index].InvalidationKey != key)
                    index++;
                else
                {
                    Items.RemoveAt(index);
                    foundAny = true;
                }
            }

            // Did we remove anything
            return foundAny;
        }


        #region API Server error

        /// <summary>
        /// Invalidates the notification for an API server querying error.
        /// </summary>
        internal void InvalidateAPIError()
        {
            Invalidate(new NotificationInvalidationEventArgs(null, NotificationCategory.QueryingError));
        }

        /// <summary>
        /// Notifies a conquerable station list querying error.
        /// </summary>
        /// <param name="result">The result.</param>
        internal void NotifyConquerableStationListError(APIResult<SerializableAPIConquerableStationList> result)
        {
            APIErrorNotificationEventArgs notification = new APIErrorNotificationEventArgs(null, result)
                                                             {
                                                                 Description =
                                                                     "An error occurred while querying the conquerable station list.",
                                                                 Behaviour = NotificationBehaviour.Overwrite,
                                                                 Priority = NotificationPriority.Error
                                                             };
            Notify(notification);
        }

        /// <summary>
        /// Notifies a character Id to name querying error.
        /// </summary>
        /// <param name="result">The result.</param>
        internal void NotifyCharacterNameError(APIResult<SerializableAPICharacterName> result)
        {
            APIErrorNotificationEventArgs notification = new APIErrorNotificationEventArgs(null, result)
                                                             {
                                                                 Description =
                                                                     "An error occurred while querying the ID to Name conversion.",
                                                                 Behaviour = NotificationBehaviour.Overwrite,
                                                                 Priority = NotificationPriority.Error
                                                             };
            Notify(notification);
        }

        /// <summary>
        /// Notifies EVE Backend Database is temporarily disabled.
        /// </summary>
        /// <param name="result">The result.</param>
        internal void NotifyEVEBackendDatabaseDisabled(IAPIResult result)
        {
            APIErrorNotificationEventArgs notification = new APIErrorNotificationEventArgs(null, result)
                                                             {
                                                                 Description = result.ErrorMessage,
                                                                 Behaviour = NotificationBehaviour.Overwrite,
                                                                 Priority = NotificationPriority.Error
                                                             };
            Notify(notification);
        }

        #endregion


        #region Server status API error

        /// <summary>
        /// Invalidates the notification for a server status querying error.
        /// </summary>
        internal void InvalidateServerStatusError()
        {
            Invalidate(new NotificationInvalidationEventArgs(null, NotificationCategory.QueryingError));
        }

        /// <summary>
        /// Notifies a server status querying error.
        /// </summary>
        /// <param name="result">The result.</param>
        internal void NotifyServerStatusError(APIResult<SerializableAPIServerStatus> result)
        {
            APIErrorNotificationEventArgs notification = new APIErrorNotificationEventArgs(null, result)
                                                             {
                                                                 Description =
                                                                     "An error occurred while querying the server status.",
                                                                 Behaviour = NotificationBehaviour.Overwrite,
                                                                 Priority = NotificationPriority.Error
                                                             };
            Notify(notification);
        }

        #endregion


        #region Account API errors

        /// <summary>
        /// Invalidates the notification for an account's error.
        /// </summary>
        /// <param name="account">The account.</param>
        internal void InvalidateAccountError(Account account)
        {
            Invalidate(new NotificationInvalidationEventArgs(account, NotificationCategory.QueryingError));
        }

        /// <summary>
        /// Notifies an account's characters list querying error.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="result">The result.</param>
        internal void NotifyCharacterListError(Account account, APIResult<SerializableAPICharacters> result)
        {
            APIErrorNotificationEventArgs notification = new APIErrorNotificationEventArgs(account, result)
                                                             {
                                                                 Description =
                                                                     String.Format(
                                                                         "An error occurred while querying the character list for account {0}.",
                                                                         account),
                                                                 Behaviour = NotificationBehaviour.Overwrite,
                                                                 Priority = NotificationPriority.Error
                                                             };
            Notify(notification);
        }

        /// <summary>
        /// Notifies an account's status querying error.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="result">The result.</param>
        internal void NotifyAccountStatusError(Account account, APIResult<SerializableAPIAccountStatus> result)
        {
            APIErrorNotificationEventArgs notification = new APIErrorNotificationEventArgs(account, result)
                                                             {
                                                                 Description =
                                                                     String.Format(
                                                                         "An error occurred while querying the account status for account {0}.",
                                                                         account),
                                                                 Behaviour = NotificationBehaviour.Overwrite,
                                                                 Priority = NotificationPriority.Error
                                                             };
            Notify(notification);
        }

        /// <summary>
        /// Notifies an API key level querying error.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="result">The result.</param>
        internal void NotifyKeyLevelError(Account account, APIResult<SerializableAPIAccountStatus> result)
        {
            APIErrorNotificationEventArgs notification = new APIErrorNotificationEventArgs(account, result)
                                                             {
                                                                 Description =
                                                                     String.Format(
                                                                         "An error occurred while checking the API key level for account {0}.",
                                                                         account),
                                                                 Behaviour = NotificationBehaviour.Overwrite,
                                                                 Priority = NotificationPriority.Error
                                                             };
            Notify(notification);
        }

        #endregion


        #region Character API errors

        /// <summary>
        /// Invalidates the notification for a character's API error.
        /// </summary>
        /// <param name="character">The character.</param>
        internal void InvalidateCharacterAPIError(CCPCharacter character)
        {
            Invalidate(new NotificationInvalidationEventArgs(character, NotificationCategory.QueryingError));
        }

        /// <summary>
        /// Notifies a character skill in training querying error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifySkillInTrainingError(CCPCharacter character, APIResult<SerializableAPISkillInTraining> result)
        {
            APIErrorNotificationEventArgs notification = new APIErrorNotificationEventArgs(character, result)
                                                             {
                                                                 Description =
                                                                     "An error occurred while querying the skill in training.",
                                                                 Behaviour = NotificationBehaviour.Overwrite,
                                                                 Priority = NotificationPriority.Error
                                                             };
            Notify(notification);
        }

        /// <summary>
        /// Notifies a character sheet querying error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyCharacterSheetError(CCPCharacter character, APIResult<SerializableAPICharacterSheet> result)
        {
            APIErrorNotificationEventArgs notification = new APIErrorNotificationEventArgs(character, result)
                                                             {
                                                                 Description =
                                                                     "An error occurred while querying the character sheet.",
                                                                 Behaviour = NotificationBehaviour.Overwrite,
                                                                 Priority = NotificationPriority.Error
                                                             };
            Notify(notification);
        }

        /// <summary>
        /// Notifies a character info querying error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyCharacterInfoError(CCPCharacter character, APIResult<SerializableAPICharacterInfo> result)
        {
            APIErrorNotificationEventArgs notification = new APIErrorNotificationEventArgs(character, result)
                                                             {
                                                                 Description =
                                                                     "An error occurred while querying the character info.",
                                                                 Behaviour = NotificationBehaviour.Overwrite,
                                                                 Priority = NotificationPriority.Error
                                                             };
            Notify(notification);
        }

        /// <summary>
        /// Notifies a skill queue querying error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifySkillQueueError(CCPCharacter character, APIResult<SerializableAPISkillQueue> result)
        {
            APIErrorNotificationEventArgs notification = new APIErrorNotificationEventArgs(character, result)
                                                             {
                                                                 Description = "An error occurred while querying the skill queue.",
                                                                 Behaviour = NotificationBehaviour.Overwrite,
                                                                 Priority = NotificationPriority.Error
                                                             };
            Notify(notification);
        }

        /// <summary>
        /// Notifies a standings querying error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyCharacterStandingsError(CCPCharacter character, APIResult<SerializableAPIStandings> result)
        {
            APIErrorNotificationEventArgs notification = new APIErrorNotificationEventArgs(character, result)
                                                             {
                                                                 Description =
                                                                     "An error occurred while querying the personal standings.",
                                                                 Behaviour = NotificationBehaviour.Overwrite,
                                                                 Priority = NotificationPriority.Error
                                                             };
            Notify(notification);
        }

        /// <summary>
        /// Notifies a market orders querying error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyCharacterMarketOrdersError(CCPCharacter character, APIResult<SerializableAPIMarketOrders> result)
        {
            APIErrorNotificationEventArgs notification = new APIErrorNotificationEventArgs(character, result)
                                                             {
                                                                 Description =
                                                                     "An error occurred while querying the personal market orders.",
                                                                 Behaviour = NotificationBehaviour.Overwrite,
                                                                 Priority = NotificationPriority.Error
                                                             };
            Notify(notification);
        }

        /// <summary>
        /// Notifies a market orders querying error.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="result"></param>
        internal void NotifyCorporationMarketOrdersError(CCPCharacter character, APIResult<SerializableAPIMarketOrders> result)
        {
            APIErrorNotificationEventArgs notification = new APIErrorNotificationEventArgs(character, result)
                                                             {
                                                                 Description =
                                                                     "An error occurred while querying the corporation market orders.",
                                                                 Behaviour = NotificationBehaviour.Overwrite,
                                                                 Priority = NotificationPriority.Error
                                                             };
            Notify(notification);
        }

        /// <summary>
        /// Notifies an industry jobs querying error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyCharacterIndustryJobsError(CCPCharacter character, APIResult<SerializableAPIIndustryJobs> result)
        {
            APIErrorNotificationEventArgs notification = new APIErrorNotificationEventArgs(character, result)
                                                             {
                                                                 Description =
                                                                     "An error occurred while querying the personal industry jobs.",
                                                                 Behaviour = NotificationBehaviour.Overwrite,
                                                                 Priority = NotificationPriority.Error
                                                             };
            Notify(notification);
        }

        /// <summary>
        /// Notifies an industry jobs querying error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyCorporationIndustryJobsError(CCPCharacter character, APIResult<SerializableAPIIndustryJobs> result)
        {
            APIErrorNotificationEventArgs notification = new APIErrorNotificationEventArgs(character, result)
                                                             {
                                                                 Description =
                                                                     "An error occurred while querying the corporation industry jobs.",
                                                                 Behaviour = NotificationBehaviour.Overwrite,
                                                                 Priority = NotificationPriority.Error
                                                             };
            Notify(notification);
        }

        /// <summary>
        /// Notifies a research querying error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyResearchPointsError(CCPCharacter character, APIResult<SerializableAPIResearch> result)
        {
            APIErrorNotificationEventArgs notification = new APIErrorNotificationEventArgs(character, result)
                                                             {
                                                                 Description =
                                                                     "An error occured while querying the research points.",
                                                                 Behaviour = NotificationBehaviour.Overwrite,
                                                                 Priority = NotificationPriority.Error
                                                             };
            Notify(notification);
        }

        /// <summary>
        /// Notifies a mail messages query error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyEVEMailMessagesError(CCPCharacter character, APIResult<SerializableAPIMailMessages> result)
        {
            APIErrorNotificationEventArgs notification = new APIErrorNotificationEventArgs(character, result)
                                                             {
                                                                 Description =
                                                                     "An error occured while querying the EVE mail messages.",
                                                                 Behaviour = NotificationBehaviour.Overwrite,
                                                                 Priority = NotificationPriority.Error
                                                             };
            Notify(notification);
        }

        /// <summary>
        /// Notifies a mail body query error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyEVEMailBodiesError(CCPCharacter character, APIResult<SerializableAPIMailBodies> result)
        {
            APIErrorNotificationEventArgs notification = new APIErrorNotificationEventArgs(character, result)
                                                             {
                                                                 Description =
                                                                     "An error occured while querying the EVE mail message body.",
                                                                 Behaviour = NotificationBehaviour.Overwrite,
                                                                 Priority = NotificationPriority.Error
                                                             };
            Notify(notification);
        }

        /// <summary>
        /// Notifies a mailing lists query error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyMailingListsError(CCPCharacter character, APIResult<SerializableAPIMailingLists> result)
        {
            APIErrorNotificationEventArgs notification = new APIErrorNotificationEventArgs(character, result)
                                                             {
                                                                 Description =
                                                                     "An error occured while querying the mailing lists.",
                                                                 Behaviour = NotificationBehaviour.Overwrite,
                                                                 Priority = NotificationPriority.Error
                                                             };
            Notify(notification);
        }

        /// <summary>
        /// Notifies a notifications query error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyEVENotificationsError(CCPCharacter character, APIResult<SerializableAPINotifications> result)
        {
            APIErrorNotificationEventArgs notification = new APIErrorNotificationEventArgs(character, result)
                                                             {
                                                                 Description =
                                                                     "An error occured while querying the EVE notifications.",
                                                                 Behaviour = NotificationBehaviour.Overwrite,
                                                                 Priority = NotificationPriority.Error
                                                             };
            Notify(notification);
        }

        /// <summary>
        /// Notifies a notification texts query error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyEVENotificationTextsError(CCPCharacter character, APIResult<SerializableAPINotificationTexts> result)
        {
            APIErrorNotificationEventArgs notification = new APIErrorNotificationEventArgs(character, result)
                                                             {
                                                                 Description =
                                                                     "An error occured while querying the EVE notification text.",
                                                                 Behaviour = NotificationBehaviour.Overwrite,
                                                                 Priority = NotificationPriority.Error
                                                             };
            Notify(notification);
        }

        #endregion


        #region Account Expiration

        /// <summary>
        /// Invalidates the notification for an account expiration.
        /// </summary>
        /// <param name="account">The account.</param>
        internal void InvalidateAccountExpiration(Account account)
        {
            Invalidate(new NotificationInvalidationEventArgs(account, NotificationCategory.AccountExpiration));
        }

        /// <summary>
        /// Notifies an account is to expire within a week.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="expireDate">The expire date.</param>
        /// <param name="priority">The priority.</param>
        internal void NotifyAccountExpiration(Account account, DateTime expireDate, NotificationPriority priority)
        {
            NotificationEventArgs notification = new NotificationEventArgs(NotificationCategory.AccountExpiration, account)
                                                     {
                                                         Description = String.Format("This account expires in {0}: {1}.",
                                                                                     expireDate.ToRemainingTimeShortDescription(
                                                                                         DateTimeKind.Utc), account),
                                                         Behaviour = NotificationBehaviour.Overwrite,
                                                         Priority = priority
                                                     };
            Notify(notification);
        }

        #endregion


        #region Account not in training

        /// <summary>
        /// Invalidates the notification for an account's characters list querying error.
        /// </summary>
        /// <param name="account">The account.</param>
        internal void InvalidateAccountNotInTraining(Account account)
        {
            Invalidate(new NotificationInvalidationEventArgs(account, NotificationCategory.AccountNotInTraining));
        }

        /// <summary>
        /// Notifies an account has no character training.
        /// </summary>
        /// <param name="account">The account.</param>
        internal void NotifyAccountNotInTraining(Account account)
        {
            NotificationEventArgs notification = new NotificationEventArgs(NotificationCategory.AccountNotInTraining, account)
                                                     {
                                                         Description =
                                                             String.Format("This account has no characters in training: {0}.",
                                                                           account),
                                                         Behaviour = NotificationBehaviour.Overwrite,
                                                         Priority = NotificationPriority.Warning
                                                     };
            Notify(notification);
        }

        #endregion


        #region Insufficient Balance

        /// <summary>
        /// Invalidates the notification for an insufficient balance.
        /// </summary>
        /// <param name="character">The character.</param>
        internal void InvalidateInsufficientBalance(CCPCharacter character)
        {
            Invalidate(new NotificationInvalidationEventArgs(character, NotificationCategory.InsufficientBalance));
        }

        /// <summary>
        /// Notifies an account has an insufficient balance.
        /// </summary>
        /// <param name="character">The character.</param>
        internal void NotifyInsufficientBalance(CCPCharacter character)
        {
            NotificationEventArgs notification = new NotificationEventArgs(NotificationCategory.InsufficientBalance, character)
                                                     {
                                                         Description =
                                                             "This character has insufficient balance to fulfill its buying orders.",
                                                         Behaviour = NotificationBehaviour.Overwrite,
                                                         Priority = NotificationPriority.Warning
                                                     };
            Notify(notification);
        }

        #endregion


        #region Insufficient clone

        /// <summary>
        /// Invalidates the notification for an insufficient clone.
        /// </summary>
        /// <param name="character">The character.</param>
        internal void InvalidateInsufficientClone(CCPCharacter character)
        {
            Invalidate(new NotificationInvalidationEventArgs(character, NotificationCategory.InsufficientClone));
        }

        /// <summary>
        /// Notifies an account has an insufficient clone.
        /// </summary>
        /// <param name="character">The character.</param>
        internal void NotifyInsufficientClone(CCPCharacter character)
        {
            NotificationEventArgs notification = new NotificationEventArgs(NotificationCategory.InsufficientClone, character)
                                                     {
                                                         Description = "This character has an insufficient clone.",
                                                         Behaviour = NotificationBehaviour.Overwrite,
                                                         Priority = NotificationPriority.Warning
                                                     };
            Notify(notification);
        }

        #endregion


        #region Skill completion

        /// <summary>
        /// Notifies a character finished training a skill.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="skillsCompleted">The completed skills.</param>
        internal void NotifySkillCompletion(CCPCharacter character, IEnumerable<QueuedSkill> skillsCompleted)
        {
            SkillCompletionNotificationEventArgs notification = new SkillCompletionNotificationEventArgs(character,
                                                                                                         skillsCompleted)
                                                                    {
                                                                        Behaviour = NotificationBehaviour.Merge,
                                                                        Priority = NotificationPriority.Information
                                                                    };
            Notify(notification);
        }

        #endregion


        #region Skill queue room available

        /// <summary>
        /// Invalidates the notification for skill queue availability.
        /// </summary>
        /// <param name="character">The character.</param>
        internal void InvalidateSkillQueueRoomAvailability(CCPCharacter character)
        {
            Invalidate(new NotificationInvalidationEventArgs(character, NotificationCategory.SkillQueueRoomAvailable));
        }

        /// <summary>
        /// Notify when we have room to queue more skills.
        /// </summary>
        /// <param name="character">The character.</param>
        internal void NotifySkillQueueRoomAvailable(CCPCharacter character)
        {
            NotificationEventArgs notification = new NotificationEventArgs(NotificationCategory.SkillQueueRoomAvailable, character)
                                                     {
                                                         Description = "This character has free room in the skill queue.",
                                                         Behaviour = NotificationBehaviour.Overwrite,
                                                         Priority = NotificationPriority.Warning
                                                     };
            Notify(notification);
        }

        #endregion


        #region Claimable certificate

        /// <summary>
        /// Invalidates the notification for claimable certificates.
        /// </summary>
        /// <param name="character">The character.</param>
        internal void InvalidateClaimableCertificate(CCPCharacter character)
        {
            Invalidate(new NotificationInvalidationEventArgs(character, NotificationCategory.ClaimableCertificate));
        }

        /// <summary>
        /// Notifies a character has a claimable certificate.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="claimableCertificates">The claimable certificates.</param>
        internal void NotifyClaimableCertificate(CCPCharacter character, IEnumerable<Certificate> claimableCertificates)
        {
            ClaimableCertificateNotificationEventArgs notification = new ClaimableCertificateNotificationEventArgs(character,
                                                                                                                   claimableCertificates)
                                                                         {
                                                                             Behaviour = NotificationBehaviour.Overwrite,
                                                                             Priority = NotificationPriority.Information
                                                                         };
            Notify(notification);
        }

        #endregion


        #region Server status

        /// <summary>
        /// Invalidates the notification for the server status.
        /// </summary>
        internal void InvalidateServerStatusChange()
        {
            Invalidate(new NotificationInvalidationEventArgs(null, NotificationCategory.ServerStatusChange));
        }

        /// <summary>
        /// Notifies about the server status.
        /// </summary>
        /// <param name="serverName">Name of the server.</param>
        /// <param name="status">The status.</param>
        internal void NotifyServerStatusChanged(string serverName, ServerStatus status)
        {
            string text = String.Empty;
            switch (status)
            {
                case ServerStatus.Offline:
                    text = String.Format("{0} is offline.", serverName);
                    break;
                case ServerStatus.Online:
                    text = String.Format("{0} is online.", serverName);
                    break;
                case ServerStatus.CheckDisabled:
                case ServerStatus.Unknown:
                    break;
                default:
                    throw new NotImplementedException();
            }

            if (String.IsNullOrEmpty(text))
                return;

            NotificationEventArgs notification = new NotificationEventArgs(NotificationCategory.ServerStatusChange, null)
                                                     {
                                                         Description = text,
                                                         Behaviour = NotificationBehaviour.Overwrite,
                                                         Priority = NotificationPriority.Information
                                                     };
            Notify(notification);
        }

        #endregion


        #region IGB Service Initilization Exception

        /// <summary>
        /// Invalidates the notification for an socket error on starting IGB service.
        /// </summary>
        internal void InvalidateIgbServiceException()
        {
            Invalidate(new NotificationInvalidationEventArgs(null, NotificationCategory.IgbServiceException));
        }

        /// <summary>
        /// Notifies for a socket error on starting IGB service.
        /// </summary>
        /// <param name="port">The port.</param>
        internal void NotifyIgbServiceException(int port)
        {
            NotificationEventArgs notification = new NotificationEventArgs(NotificationCategory.IgbServiceException, null)
                                                     {
                                                         Description =
                                                             String.Format(CultureConstants.DefaultCulture,
                                                                           "Failed to start the IGB server on port {0}.", port),
                                                         Behaviour = NotificationBehaviour.Overwrite,
                                                         Priority = NotificationPriority.Error
                                                     };
            Notify(notification);
        }

        #endregion


        #region Market orders expiration

        /// <summary>
        /// Notify some market orders expired or have been fulfilled.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="expiredOrders">The expired orders.</param>
        internal void NotifyMarkerOrdersEnding(Character character, IEnumerable<MarketOrder> expiredOrders)
        {
            MarketOrdersNotificationEventArgs notification = new MarketOrdersNotificationEventArgs(character, expiredOrders)
                                                                 {
                                                                     Behaviour = NotificationBehaviour.Merge,
                                                                     Priority = NotificationPriority.Information
                                                                 };
            Notify(notification);
        }

        #endregion


        #region Industry jobs completion

        /// <summary>
        /// Notify some industry jobs have ended.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="jobsCompleted">The completed jobs.</param>
        internal void NotifyIndustryJobCompletion(Character character, IEnumerable<IndustryJob> jobsCompleted)
        {
            IndustryJobsNotificationEventArgs notification = new IndustryJobsNotificationEventArgs(character, jobsCompleted)
                                                                 {
                                                                     Behaviour = NotificationBehaviour.Merge,
                                                                     Priority = NotificationPriority.Information
                                                                 };
            Notify(notification);
        }

        #endregion


        #region New EVE mail message

        /// <summary>
        /// Notify new EVE mail message is available.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="newMessages">The new messages.</param>
        internal void NotifyNewEVEMailMessages(Character character, int newMessages)
        {
            NewEveMailMessageNotificationEventArgs notification =
                new NewEveMailMessageNotificationEventArgs(character, newMessages)
                    {
                        Behaviour = NotificationBehaviour.Merge,
                        Priority = NotificationPriority.Information
                    };
            Notify(notification);
        }

        #endregion


        #region New EVE notification

        /// <summary>
        /// Notify new EVE notification is available.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="newNotifications">The new notifications.</param>
        internal void NotifyNewEVENotifications(Character character, int newNotifications)
        {
            NewEveNotificationEventArgsNotificationEventArgs notification =
                new NewEveNotificationEventArgsNotificationEventArgs(character, newNotifications)
                    {
                        Behaviour = NotificationBehaviour.Merge,
                        Priority = NotificationPriority.Information
                    };
            Notify(notification);
        }

        #endregion
    }
}