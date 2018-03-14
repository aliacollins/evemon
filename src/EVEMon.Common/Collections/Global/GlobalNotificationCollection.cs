using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Models;
using EVEMon.Common.Notifications;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Serialization.Esi;

namespace EVEMon.Common.Collections.Global
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
        /// Clears the collection.
        /// </summary>
        public void Clear()
        {
            Items.Clear();
        }

        /// <summary>
        /// Adds a notification to this collection.
        /// </summary>
        /// <param name="notification"></param>
        /// <exception cref="System.ArgumentNullException">notification</exception>
        public void Notify(NotificationEventArgs notification)
        {
            notification.ThrowIfNull(nameof(notification));

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
        /// <param name="e"></param>
        public void Invalidate(NotificationInvalidationEventArgs e)
        {
            if (InvalidateCore(e.Key))
                EveMonClient.OnNotificationInvalidated(e);
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
        internal void NotifyConquerableStationListError(CCPAPIResult<SerializableAPIConquerableStationList> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(null, result)
                {
                    Description = "An error occurred while querying the conquerable station list.",
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = NotificationPriority.Error
                };
            Notify(notification);
        }

        /// <summary>
        /// Notifies an EVE factional warfare stats querying error.
        /// </summary>
        /// <param name="result">The result.</param>
        internal void NotifyEveFactionalWarfareStatsError(CCPAPIResult<SerializableAPIEveFactionalWarfareStats> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(null, result)
                {
                    Description = "An error occurred while querying the EVE factional warfare statistics.",
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = NotificationPriority.Error
                };
            Notify(notification);
        }

        /// <summary>
        /// Notifies a character Id to name querying error.
        /// </summary>
        /// <param name="result">The result.</param>
        internal void NotifyCharacterNameError(CCPAPIResult<SerializableAPICharacterName> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(null, result)
                {
                    Description = "An error occurred while querying the ID to Name conversion.",
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = NotificationPriority.Error
                };
            Notify(notification);
        }

        /// <summary>
        /// Notifies a refTypes querying error.
        /// </summary>
        /// <param name="result">The result.</param>
        internal void NotifyRefTypesError(CCPAPIResult<SerializableAPIRefTypes> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(null, result)
                {
                    Description = "An error occurred while querying the RefTypes list.",
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = NotificationPriority.Error
                };
            Notify(notification);
        }

        /// <summary>
        /// Notifies EVE Backend Database is temporarily disabled.
        /// </summary>
        /// <param name="result">The result.</param>
        internal void NotifyEVEDatabaseError(IAPIResult result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(null, result)
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
        /// Notifies a server status querying error.
        /// </summary>
        /// <param name="result">The result.</param>
        internal void NotifyServerStatusError(CCPAPIResult<SerializableAPIServerStatus> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(null, result)
                {
                    Description = "An error occurred while querying the server status.",
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = NotificationPriority.Error
                };
            Notify(notification);
        }

        #endregion


        #region API key info errors

        /// <summary>
        /// Invalidates the notification for an API key's info error.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        internal void InvalidateAPIKeyInfoError(APIKey apiKey)
        {
            Invalidate(new NotificationInvalidationEventArgs(apiKey, NotificationCategory.QueryingError));
        }

        /// <summary>
        /// Notifies an API key's characters list querying error.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        /// <param name="result">The result.</param>
        internal void NotifyCharacterListError(APIKey apiKey, CCPAPIResult<SerializableAPIKeyInfo> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(apiKey, result)
                {
                    Description = $"An error occurred while querying the character list for API key {apiKey}.",
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = NotificationPriority.Error
                };
            Notify(notification);
        }

        #endregion


        #region Account status errors

        /// <summary>
        /// Invalidates the notification for an account status error.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        internal void InvalidateAccountStatusError(APIKey apiKey)
        {
            Invalidate(new NotificationInvalidationEventArgs(apiKey, NotificationCategory.QueryingError));
        }

        /// <summary>
        /// Notifies an account status querying error.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        /// <param name="result">The result.</param>
        internal void NotifyAccountStatusError(APIKey apiKey, CCPAPIResult<SerializableAPIAccountStatus> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(apiKey, result)
                {
                    Description = $"An error occurred while querying the account status for API key {apiKey}.",
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
        internal void NotifySkillInTrainingError(CCPCharacter character, CCPAPIResult<SerializableAPISkillInTraining> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(character, result)
                {
                    Description = "An error occurred while querying the skill in training.",
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
        internal void NotifyCharacterSheetError(CCPCharacter character, CCPAPIResult<SerializableAPICharacterSheet> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(character, result)
                {
                    Description = "An error occurred while querying the character sheet.",
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
        internal void NotifyCharacterInfoError(CCPCharacter character, CCPAPIResult<SerializableAPICharacterInfo> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(character, result)
                {
                    Description = "An error occurred while querying the character info.",
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
        internal void NotifySkillQueueError(CCPCharacter character, CCPAPIResult<SerializableAPISkillQueue> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(character, result)
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
        internal void NotifyCharacterStandingsError(CCPCharacter character, CCPAPIResult<SerializableAPIStandings> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(character, result)
                {
                    Description = "An error occurred while querying the personal standings.",
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = NotificationPriority.Error
                };
            Notify(notification);
        }

        /// <summary>
        /// Notifies a factional warfare stats querying error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyCharacterFactionalWarfareStatsError(CCPCharacter character,
            CCPAPIResult<SerializableAPIFactionalWarfareStats> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(character, result)
                {
                    Description = "An error occurred while querying the personal factional warfare stats.",
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = NotificationPriority.Error
                };
            Notify(notification);
        }

        /// <summary>
        /// Notifies an assets querying error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyCharacterAssetsError(CCPCharacter character, CCPAPIResult<SerializableAPIAssetList> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(character, result)
                {
                    Description = "An error occurred while querying the personal assets list.",
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = NotificationPriority.Error
                };
            Notify(notification);
        }

        /// <summary>
        /// Notifies the character market orders querying error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyCharacterMarketOrdersError(CCPCharacter character, CCPAPIResult<SerializableAPIMarketOrders> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(character, result)
                {
                    Description = "An error occurred while querying the personal market orders.",
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = NotificationPriority.Error
                };
            Notify(notification);
        }

        /// <summary>
        /// Notifies a corporation market orders querying error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyCorporationMarketOrdersError(CCPCharacter character, CCPAPIResult<SerializableAPIMarketOrders> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(character, result)
                {
                    Description = "An error occurred while querying the corporation market orders.",
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = NotificationPriority.Error
                };
            Notify(notification);
        }

        /// <summary>
        /// Notifies the character contracts querying error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyCharacterContractsError(CCPCharacter character, CCPAPIResult<SerializableAPIContracts> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(character, result)
                {
                    Description = "An error occurred while querying the personal contracts.",
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = NotificationPriority.Error
                };
            Notify(notification);
        }

        /// <summary>
        /// Notifies the corporation contracts querying error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyCorporationContractsError(CCPCharacter character, CCPAPIResult<SerializableAPIContracts> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(character, result)
                {
                    Description = "An error occurred while querying the corporation contracts.",
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = NotificationPriority.Error
                };
            Notify(notification);
        }

        /// <summary>
        /// Notifies a contract items querying error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyContractItemsError(CCPCharacter character, CCPAPIResult<SerializableAPIContractItems> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(character, result)
                {
                    Description = "An error occurred while querying a contract's items.",
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = NotificationPriority.Error
                };
            Notify(notification);
        }

        /// <summary>
        /// Notifies a character contract bids querying error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyCharacterContractBidsError(CCPCharacter character, CCPAPIResult<SerializableAPIContractBids> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(character, result)
                {
                    Description = "An error occurred while querying the personal contract bids.",
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = NotificationPriority.Error
                };
            Notify(notification);
        }

        /// <summary>
        /// Notifies a corporation contract bids querying error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyCorporationContractBidsError(CCPCharacter character, CCPAPIResult<SerializableAPIContractBids> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(character, result)
                {
                    Description = "An error occurred while querying the corporation contract bids.",
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = NotificationPriority.Error
                };
            Notify(notification);
        }

        /// <summary>
        /// Notifies a wallet journal querying error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyCharacterWalletJournalError(CCPCharacter character, CCPAPIResult<SerializableAPIWalletJournal> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(character, result)
                {
                    Description = "An error occurred while querying the personal wallet journal.",
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = NotificationPriority.Error
                };
            Notify(notification);
        }

        /// <summary>
        /// Notifies a wallet transactions querying error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyCharacterWalletTransactionsError(CCPCharacter character,
            CCPAPIResult<SerializableAPIWalletTransactions> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(character, result)
                {
                    Description = "An error occurred while querying the personal wallet transactions.",
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = NotificationPriority.Error
                };
            Notify(notification);
        }

        /// <summary>
        /// Notifies a character industry jobs querying error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyCharacterIndustryJobsError(CCPCharacter character, CCPAPIResult<SerializableAPIIndustryJobs> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(character, result)
                {
                    Description = "An error occurred while querying the personal industry jobs.",
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = NotificationPriority.Error
                };
            Notify(notification);
        }

        /// <summary>
        /// Notifies a corporation industry jobs querying error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyCorporationIndustryJobsError(CCPCharacter character, CCPAPIResult<SerializableAPIIndustryJobs> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(character, result)
                {
                    Description = "An error occurred while querying the corporation industry jobs.",
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
        internal void NotifyResearchPointsError(CCPCharacter character, CCPAPIResult<SerializableAPIResearch> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(character, result)
                {
                    Description = "An error occured while querying the research points.",
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
        internal void NotifyEVEMailMessagesError(CCPCharacter character, CCPAPIResult<SerializableAPIMailMessages> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(character, result)
                {
                    Description = "An error occured while querying the EVE mail messages.",
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
        internal void NotifyEVEMailBodiesError(CCPCharacter character, CCPAPIResult<SerializableAPIMailBodies> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(character, result)
                {
                    Description = "An error occured while querying the EVE mail message body.",
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
        internal void NotifyMailingListsError(CCPCharacter character, CCPAPIResult<SerializableAPIMailingLists> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(character, result)
                {
                    Description = "An error occured while querying the mailing lists.",
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
        internal void NotifyEVENotificationsError(CCPCharacter character, CCPAPIResult<SerializableAPINotifications> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(character, result)
                {
                    Description = "An error occured while querying the EVE notifications.",
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
        internal void NotifyEVENotificationTextsError(CCPCharacter character, CCPAPIResult<SerializableAPINotificationTexts> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(character, result)
                {
                    Description = "An error occured while querying the EVE notification text.",
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = NotificationPriority.Error
                };
            Notify(notification);
        }

        /// <summary>
        /// Notifies a contact list query error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyCharacterContactsError(CCPCharacter character, CCPAPIResult<SerializableAPIContactList> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(character, result)
                {
                    Description = "An error occured while querying the personal contacts list.",
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = NotificationPriority.Error
                };
            Notify(notification);
        }

        /// <summary>
        /// Notifies a medals query error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyCharacterMedalsError(CCPCharacter character, CCPAPIResult<SerializableAPIMedals> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(character, result)
                {
                    Description = "An error occured while querying the personal medals.",
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = NotificationPriority.Error
                };
            Notify(notification);
        }

        /// <summary>
        /// Notifies a corporation medals querying error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyCorporationMedalsError(CCPCharacter character, CCPAPIResult<SerializableAPIMedals> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(character, result)
                {
                    Description = "An error occurred while querying the corporation medals.",
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = NotificationPriority.Error
                };
            Notify(notification);
        }

        /// <summary>
        /// Notifies an upcoming calendar event details query error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyCharacterUpcomingCalendarEventDetailsError(CCPCharacter character,
            CCPAPIResult<EsiAPICalendarEvent> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(character, result)
                {
                    Description = "An error occured while querying the upcoming calendar event details.",
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = NotificationPriority.Error
                };
            Notify(notification);
        }

        /// <summary>
        /// Notifies an upcoming calendar events query error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyCharacterUpcomingCalendarEventsError(CCPCharacter character,
            CCPAPIResult<SerializableAPIUpcomingCalendarEvents> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(character, result)
                {
                    Description = "An error occured while querying the upcoming calendar events.",
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = NotificationPriority.Error
                };
            Notify(notification);
        }

        /// <summary>
        /// Notifies a calendar event attendees query error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyCharacterCalendarEventAttendeesError(CCPCharacter character,
            CCPAPIResult<SerializableAPICalendarEventAttendees> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(character, result)
                {
                    Description = "An error occured while querying the personal calendar event attendees.",
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = NotificationPriority.Error
                };
            Notify(notification);
        }

        /// <summary>
        /// Notifies a kill log query error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyCharacterKillLogError(CCPCharacter character, CCPAPIResult<SerializableAPIKillLog> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(character, result)
                {
                    Description = "An error occured while querying the personal kill log.",
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = NotificationPriority.Error
                };
            Notify(notification);
        }

        /// <summary>
        /// Notifies a planetary colonies query error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyCharacterPlanetaryColoniesError(CCPCharacter character,
            CCPAPIResult<SerializableAPIPlanetaryColonies> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(character, result)
                {
                    Description = "An error occured while querying the planetary colonies.",
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = NotificationPriority.Error
                };
            Notify(notification);
        }

        /// <summary>
        /// Notifies a planetary pins query error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyCharacterPlanetaryPinsError(CCPCharacter character,
            CCPAPIResult<SerializableAPIPlanetaryPins> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(character, result)
                {
                    Description = "An error occured while querying the planetary pins.",
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = NotificationPriority.Error
                };
            Notify(notification);
        }

        /// <summary>
        /// Notifies a planetary routes query error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyCharacterPlanetaryRoutesError(CCPCharacter character,
            CCPAPIResult<SerializableAPIPlanetaryRoutes> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(character, result)
                {
                    Description = "An error occured while querying the planetary routes.",
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = NotificationPriority.Error
                };
            Notify(notification);
        }

        /// <summary>
        /// Notifies a planetary links query error.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="result">The result.</param>
        internal void NotifyCharacterPlanetaryLinksError(CCPCharacter character,
            CCPAPIResult<SerializableAPIPlanetaryLinks> result)
        {
            APIErrorNotificationEventArgs notification =
                new APIErrorNotificationEventArgs(character, result)
                {
                    Description = "An error occured while querying the planetary links.",
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = NotificationPriority.Error
                };
            Notify(notification);
        }

        #endregion


        #region API key expiration

        /// <summary>
        /// Invalidates the notification for an API key expiration.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        internal void InvalidateAPIKeyExpiration(APIKey apiKey)
        {
            Invalidate(new NotificationInvalidationEventArgs(apiKey, NotificationCategory.APIKeyExpiration));
        }

        /// <summary>
        /// Notifies an API key is to expire within a week.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        /// <param name="expireDate">The expire date.</param>
        /// <param name="priority">The priority.</param>
        internal void NotifyAPIKeyExpiration(APIKey apiKey, DateTime expireDate, NotificationPriority priority)
        {
            NotificationEventArgs notification =
                new NotificationEventArgs(apiKey, NotificationCategory.APIKeyExpiration)
                {
                    Description =
                        $"This API key expires in {expireDate.ToRemainingTimeShortDescription(DateTimeKind.Utc)} ({apiKey}).",
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = priority
                };
            Notify(notification);
        }

        #endregion


        #region Account expiration

        /// <summary>
        /// Invalidates the notification for an account expiration.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        internal void InvalidateAccountExpiration(APIKey apiKey)
        {
            Invalidate(new NotificationInvalidationEventArgs(apiKey, NotificationCategory.AccountExpiration));
        }

        /// <summary>
        /// Notifies an account is to expire within a week.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        /// <param name="expireDate">The expire date.</param>
        /// <param name="priority">The priority.</param>
        internal void NotifyAccountExpiration(APIKey apiKey, DateTime expireDate, NotificationPriority priority)
        {
            NotificationEventArgs notification =
                new NotificationEventArgs(apiKey, NotificationCategory.AccountExpiration)
                {
                    Description =
                        $"This account expires in {expireDate.ToRemainingTimeShortDescription(DateTimeKind.Utc)}: {apiKey}.",
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
        /// <param name="apiKey">The API key.</param>
        internal void InvalidateAccountNotInTraining(APIKey apiKey)
        {
            Invalidate(new NotificationInvalidationEventArgs(apiKey, NotificationCategory.AccountNotInTraining));
        }

        /// <summary>
        /// Notifies an account has no character training.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        internal void NotifyAccountNotInTraining(APIKey apiKey)
        {
            NotificationEventArgs notification =
                new NotificationEventArgs(apiKey, NotificationCategory.AccountNotInTraining)
                {
                    Description = $"This account has no characters in training: {apiKey}.",
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = NotificationPriority.Warning
                };
            Notify(notification);
        }

        #endregion


        #region Insufficient balance

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
            NotificationEventArgs notification =
                new NotificationEventArgs(character, NotificationCategory.InsufficientBalance)
                {
                    Description = "This character has insufficient balance to fulfill its buying orders.",
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
            SkillCompletionNotificationEventArgs notification =
                new SkillCompletionNotificationEventArgs(character, skillsCompleted)
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
        internal void InvalidateSkillQueueLessThanADay(CCPCharacter character)
        {
            Invalidate(new NotificationInvalidationEventArgs(character, NotificationCategory.SkillQueueRoomAvailable));
        }

        /// <summary>
        /// Notify when we have room to queue more skills.
        /// </summary>
        /// <param name="character">The character.</param>
        internal void NotifySkillQueueLessThanADay(CCPCharacter character)
        {
            NotificationEventArgs notification =
                new NotificationEventArgs(character, NotificationCategory.SkillQueueRoomAvailable)
                {
                    Description = "This character has less than a day training.",
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = NotificationPriority.Warning
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
                    text = $"{serverName} is offline.";
                    break;
                case ServerStatus.Online:
                    text = $"{serverName} is online.";
                    break;
                case ServerStatus.CheckDisabled:
                case ServerStatus.Unknown:
                    break;
                default:
                    throw new NotImplementedException();
            }

            if (String.IsNullOrEmpty(text))
                return;

            NotificationEventArgs notification =
                new NotificationEventArgs(null, NotificationCategory.ServerStatusChange)
                {
                    Description = text,
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = NotificationPriority.Information
                };
            Notify(notification);
        }

        #endregion


        #region Market orders expiration

        /// <summary>
        /// Notify some character market orders have been expired or fulfilled.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="expiredOrders">The expired orders.</param>
        internal void NotifyCharacterMarkerOrdersEnded(Character character, IEnumerable<MarketOrder> expiredOrders)
        {
            MarketOrdersNotificationEventArgs notification =
                new MarketOrdersNotificationEventArgs(character, expiredOrders)
                {
                    Behaviour = NotificationBehaviour.Merge,
                    Priority = NotificationPriority.Information
                };
            Notify(notification);
        }

        /// <summary>
        /// Notify some corporation market orders have been expired or fulfilled.
        /// </summary>
        /// <param name="corporation">The corporation.</param>
        /// <param name="expiredOrders">The expired orders.</param>
        internal void NotifyCorporationMarketOrdersEnded(Corporation corporation, IEnumerable<MarketOrder> expiredOrders)
        {
            MarketOrdersNotificationEventArgs notification =
                new MarketOrdersNotificationEventArgs(corporation, expiredOrders)
                {
                    Behaviour = NotificationBehaviour.Merge,
                    Priority = NotificationPriority.Information
                };
            Notify(notification);
        }

        #endregion


        #region Contracts expiration

        /// <summary>
        /// Notify some character contracts have been expired or fulfilled.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="endedContracts">The ended contracts.</param>
        public void NotifyCharacterContractsEnded(Character character, IEnumerable<Contract> endedContracts)
        {
            ContractsNotificationEventArgs notification =
                new ContractsNotificationEventArgs(character, endedContracts)
                {
                    Behaviour = NotificationBehaviour.Merge,
                    Priority = NotificationPriority.Information
                };
            Notify(notification);
        }

        /// <summary>
        /// Notify some corporation contracts have been expired or fulfilled.
        /// </summary>
        /// <param name="corporation">The corporation.</param>
        /// <param name="endedContracts">The ended contracts.</param>
        public void NotifyCorporationContractsEnded(Corporation corporation, IEnumerable<Contract> endedContracts)
        {
            ContractsNotificationEventArgs notification =
                new ContractsNotificationEventArgs(corporation, endedContracts)
                {
                    Behaviour = NotificationBehaviour.Merge,
                    Priority = NotificationPriority.Information
                };
            Notify(notification);
        }

        #endregion


        #region Contracts assigned

        /// <summary>
        /// Invalidates the notification for assigned contracts.
        /// </summary>
        /// <param name="character">The character.</param>
        internal void InvalidateCharacterContractsAssigned(CCPCharacter character)
        {
            Invalidate(new NotificationInvalidationEventArgs(character, NotificationCategory.ContractsAssigned));
        }

        /// <summary>
        /// Notifies for character assigned contracts.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="assignedContracts">The assigned contracts.</param>
        public void NotifyCharacterContractsAssigned(Character character, int assignedContracts)
        {
            NotificationEventArgs notification =
                new NotificationEventArgs(character, NotificationCategory.ContractsAssigned)
                {
                    Description = $"{assignedContracts} assigned contract{(assignedContracts > 1 ? "s" : String.Empty)}.",
                    Behaviour = NotificationBehaviour.Overwrite,
                    Priority = NotificationPriority.Information
                };
            Notify(notification);
        }

        #endregion


        #region Industry jobs completion

        /// <summary>
        /// Notify some character industry jobs have ended.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="jobsCompleted">The completed jobs.</param>
        internal void NotifyCharacterIndustryJobCompletion(Character character, IEnumerable<IndustryJob> jobsCompleted)
        {
            IndustryJobsNotificationEventArgs notification =
                new IndustryJobsNotificationEventArgs(character, jobsCompleted)
                {
                    Behaviour = NotificationBehaviour.Merge,
                    Priority = NotificationPriority.Information
                };
            Notify(notification);
        }

        /// <summary>
        /// Notify some corporation industry jobs have ended.
        /// </summary>
        /// <param name="corporation">The corporation.</param>
        /// <param name="jobsCompleted">The completed jobs.</param>
        internal void NotifyCorporationIndustryJobCompletion(Corporation corporation, IEnumerable<IndustryJob> jobsCompleted)
        {
            IndustryJobsNotificationEventArgs notification =
                new IndustryJobsNotificationEventArgs(corporation, jobsCompleted)
                {
                    Behaviour = NotificationBehaviour.Merge,
                    Priority = NotificationPriority.Information
                };
            Notify(notification);
        }

        #endregion


        #region Planetary Pins expiration

        /// <summary>
        /// Invalidates the notification for completed planetary pins.
        /// </summary>
        /// <param name="character">The character.</param>
        internal void InvalidateCharacterPlanetaryPinCompleted(CCPCharacter character)
        {
            Invalidate(new NotificationInvalidationEventArgs(character, NotificationCategory.PlanetaryPinsCompleted));
        }

        /// <summary>
        /// Notify some character planetary pins have ended.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="pinsCompleted">The completed pins.</param>
        internal void NotifyCharacterPlanetaryPinCompleted(Character character, IEnumerable<PlanetaryPin> pinsCompleted)
        {
            PlanetaryPinsNotificationEventArgs notification =
                new PlanetaryPinsNotificationEventArgs(character, pinsCompleted)
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
            EveMailMessageNotificationEventArgs notification =
                new EveMailMessageNotificationEventArgs(character, newMessages)
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
            EveNotificationEventArgs notification =
                new EveNotificationEventArgs(character, newNotifications)
                {
                    Behaviour = NotificationBehaviour.Merge,
                    Priority = NotificationPriority.Information
                };
            Notify(notification);
        }

        #endregion

    }
}
