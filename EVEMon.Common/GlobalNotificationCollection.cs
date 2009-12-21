using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.Notifications;

namespace EVEMon.Common
{
    /// <summary>
    /// The collection used by <see cref="EveClient.Notifications"/>
    /// </summary>
    public sealed class GlobalNotificationCollection : ReadonlyCollection<Notification>
    {
        /// <summary>
        /// Constructor, used by <see cref="EveClient"/> only.
        /// </summary>
        internal GlobalNotificationCollection()
        {
        }

        /// <summary>
        /// Adds a notification to this collection.
        /// </summary>
        /// <param name="notification"></param>
        public void Notify(Notification notification)
        {
            // 
            switch (notification.Behaviour)
            {
                case NotificationBehaviour.Cohabitate:
                    m_items.Add(notification);
                    break;

                case NotificationBehaviour.Overwrite:
                    // Replace the previous notifications with the same invalidation key
                    InvalidateCore(notification.InvalidationKey);
                    m_items.Add(notification);
                    break;

                case NotificationBehaviour.Merge:
                    // Merge the notifications with the same key
                    var key = notification.InvalidationKey;
                    foreach (var other in m_items.Where(x => x.InvalidationKey == key))
                    {
                        notification.Append(other);
                    }

                    // Replace the previous notifications with the same invalidation key
                    InvalidateCore(key);
                    m_items.Add(notification);
                    break;
            }

            EveClient.OnNotificationSent(notification);
        }

        /// <summary>
        /// Invalidates the notifications with the given key and notify an event.
        /// </summary>
        /// <param name="notification"></param>
        public void Remove(Notification notification)
        {
            m_items.Remove(notification);
        }

        /// <summary>
        /// Invalidates the notifications with the given key and notify an event.
        /// </summary>
        /// <param name="args"></param>
        public void Invalidate(NotificationInvalidationEventArgs args)
        {
            if (InvalidateCore(args.Key)) EveClient.OnNotificationInvalidated(args);
        }

        /// <summary>
        /// Invalidates the notifications with the given key.
        /// </summary>
        /// <param name="key"></param>
        private bool InvalidateCore(long key)
        {
            int index = 0;
            bool foundAny = false;

            // Removes all the notifications with the given key.
            while (index < m_items.Count)
            {
                if (m_items[index].InvalidationKey != key) index++;
                else
                {
                    m_items.RemoveAt(index);
                    foundAny = true;
                }
            }

            // Did we remove anything
            return foundAny;
        }

        #region Server status error
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
        /// <param name="character"></param>
        /// <param name="result"></param>
        internal void NotifyServerStatusError(APIResult<SerializableServerStatus> result)
        {
            var notification = new APIErrorNotification(null, result);
            notification.Description = "An error occured while querying the server status.";
            notification.Behaviour = NotificationBehaviour.Overwrite;
            notification.Priority = NotificationPriority.Error;
            Notify(notification);
        }
        #endregion


        #region Account API errors
        /// <summary>
        /// Invalidates the notification for an account's error.
        /// </summary>
        /// <param name="account"></param>
        internal void InvalidateAccountError(Account account)
        {
            Invalidate(new NotificationInvalidationEventArgs(account, NotificationCategory.QueryingError));
        }

        /// <summary>
        /// Notifies an account's characters list querying error.
        /// </summary>
        /// <param name="account"></param>
        /// <param name="result"></param>
        internal void NotifyCharacterListError(Account account, APIResult<SerializableCharacterList> result)
        {
            var notification = new APIErrorNotification(account, result);
            notification.Description = "An error occured while querying the character list for account " + account.ToString() + ".";
            notification.Behaviour = NotificationBehaviour.Overwrite;
            notification.Priority = NotificationPriority.Error;
            Notify(notification);
        }

        /// <summary>
        /// Notifies an account's characters list querying error.
        /// </summary>
        /// <param name="account"></param>
        /// <param name="result"></param>
        internal void NotifyKeyLevelError(Account account, APIResult<SerializableAccountBalanceList> result)
        {
            var notification = new APIErrorNotification(account, result);
            notification.Description = "An error occured while checking the API key level for account " + account.ToString() + ".";
            notification.Behaviour = NotificationBehaviour.Overwrite;
            notification.Priority = NotificationPriority.Error;
            Notify(notification);
        }
        #endregion


        #region Character API errors
        /// <summary>
        /// Invalidates the notification for a character's API error.
        /// </summary>
        internal void InvalidateCharacterAPIError(CCPCharacter character)
        {
            Invalidate(new NotificationInvalidationEventArgs(character, NotificationCategory.QueryingError));
        }

        /// <summary>
        /// Notifies a character sheet querying error.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="result"></param>
        internal void NotifyCharacterSheetError(CCPCharacter character, APIResult<SerializableAPICharacter> result)
        {
            var notification = new APIErrorNotification(character, result);
            notification.Description = "An error occured while querying the character sheet.";
            notification.Behaviour = NotificationBehaviour.Overwrite;
            notification.Priority = NotificationPriority.Error;
            Notify(notification);
        }

        /// <summary>
        /// Notifies a skill queue querying error.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="result"></param>
        internal void NotifySkillQueueError(CCPCharacter character, APIResult<SerializableSkillQueue> result)
        {
            var notification = new APIErrorNotification(character, result);
            notification.Description = "An error occured while querying the skill queue.";
            notification.Behaviour = NotificationBehaviour.Overwrite;
            notification.Priority = NotificationPriority.Error;
            Notify(notification);
        }

        /// <summary>
        /// Notifies a market orders querying error.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="result"></param>
        internal void NotifyMarketOrdersError(CCPCharacter character, APIResult<SerializableAPIOrderList> result)
        {
            var notification = new APIErrorNotification(character, result);
            notification.Description = "An error occured while querying the market orders.";
            notification.Behaviour = NotificationBehaviour.Overwrite;
            notification.Priority = NotificationPriority.Error;
            Notify(notification);
        }
        #endregion


        #region Account not in training
        /// <summary>
        /// Invalidates the notification for an account's characters list querying error.
        /// </summary>
        internal void InvalidateAccountNotInTraining(Account account)
        {
            Invalidate(new NotificationInvalidationEventArgs(account, NotificationCategory.AccountNotInTraining));
        }

        /// <summary>
        /// Notifies an account has no character training.
        /// </summary>
        /// <param name="account"></param>
        internal void NotifyAccountNotInTraining(Account account)
        {
            var notification = new Notification(NotificationCategory.AccountNotInTraining, account);
            notification.Description = "This account has no characters in training: " + account.ToString() + ".";
            notification.Behaviour = NotificationBehaviour.Overwrite;
            notification.Priority = NotificationPriority.Warning;
            Notify(notification);
        }
        #endregion


        #region Insufficient clone
        /// <summary>
        /// Invalidates the notification for an insufficient clone.
        /// </summary>
        internal void InvalidateInsufficientClone(CCPCharacter character)
        {
            Invalidate(new NotificationInvalidationEventArgs(character, NotificationCategory.InsufficientClone));
        }

        /// <summary>
        /// Notifies an account has an insufficient clone.
        /// </summary>
        /// <param name="account"></param>
        internal void NotifyInsufficientClone(CCPCharacter character)
        {
            var notification = new Notification(NotificationCategory.InsufficientClone, character);
            notification.Description = "This character has an insufficient clone.";
            notification.Behaviour = NotificationBehaviour.Overwrite;
            notification.Priority = NotificationPriority.Warning;
            Notify(notification);
        }
        #endregion


        #region Skill completion
        /// <summary>
        /// Invalidates the notification for a skill completion.
        /// </summary>
        internal void InvalidateSkillCompletion(CCPCharacter character)
        {
            Invalidate(new NotificationInvalidationEventArgs(character, NotificationCategory.SkillCompletion));
        }

        /// <summary>
        /// Notifies a character finished training a skill.
        /// </summary>
        /// <param name="account"></param>
        internal void NotifySkillCompletion(CCPCharacter character, QueuedSkill skill)
        {
            var notification = new Notification(NotificationCategory.SkillCompletion, character);
            notification.Description = String.Format("{0} {1} completed.", skill.Skill.Name, Skill.GetRomanForInt(skill.Level));
            notification.Behaviour = NotificationBehaviour.Cohabitate;
            notification.Priority = NotificationPriority.Information;
            Notify(notification);
        }
        #endregion


        #region Server status
        /// <summary>
        /// Invalidates the notification for an skill completion.
        /// </summary>
        internal void InvalidateServerStatusChange()
        {
            Invalidate(new NotificationInvalidationEventArgs(null, NotificationCategory.ServerStatusChange));
        }

        /// <summary>
        /// Notifies a character finished training a skill.
        /// </summary>
        /// <param name="account"></param>
        internal void NotifyServerStatusChange(ServerStatus status)
        {
            string text = "";
            switch(status)
            {
                default:
                    return;
                case ServerStatus.Offline:
                    text = "Tranquility is offline.";
                    break;
                case ServerStatus.Online:
                    text = "Tranquility is online.";
                    break;
            }

            var notification = new Notification(NotificationCategory.ServerStatusChange, null);
            notification.Description = text;
            notification.Behaviour = NotificationBehaviour.Overwrite;
            notification.Priority = NotificationPriority.Information;
            Notify(notification);
        }
        #endregion


        #region IGB Service Initilization Exception
        /// <summary>
        /// Invalidates the notification for an socket error on starting IGB service
        /// </summary>
        internal void InvalidateIgbServiceException()
        {
            Invalidate(new NotificationInvalidationEventArgs(null, NotificationCategory.IgbServiceException));
        }

        /// <summary>
        /// Notification for a socket error on starting IGB service
        /// </summary>
        /// <param name="account"></param>
        internal void NotifyIgbServiceException(int port)
        {
            var notification = new Notification(NotificationCategory.IgbServiceException, null);
            notification.Description = String.Format("Failed to start the IGB server on port {0}.", port);
            notification.Behaviour = NotificationBehaviour.Overwrite;
            notification.Priority = NotificationPriority.Error;
            Notify(notification);
        }
        #endregion 
   
    
        #region Market orders expiration
        /// <summary>
        /// Notify some market orders expired or have been fulfilled.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="expiredOrders"></param>
        internal void NotifyMarkerOrdersEnding(Character character, List<MarketOrder> expiredOrders)
        {
            var notification = new MarketOrdersNotification(character, expiredOrders);
            notification.Behaviour = NotificationBehaviour.Merge;
            notification.Priority = NotificationPriority.Information;
            Notify(notification);
        }
        #endregion


        #region Skill queue room available
        /// <summary>
        /// Invalidates the notification for skill queue availability.
        /// </summary>
        internal void InvalidateSkillQueueRoomAvailability(CCPCharacter character)
        {
            Invalidate(new NotificationInvalidationEventArgs(character, NotificationCategory.SkillQueueRoomAvailable));
        }

        /// <summary>
        /// Notify when we have room to queue more skills.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="expiredOrders"></param>
        internal void NotifySkillQueueRoomAvailable(Character character)
        {
            var notification = new Notification(NotificationCategory.SkillQueueRoomAvailable, character);
            notification.Description = "This character has free room in the skill queue.";
            notification.Behaviour = NotificationBehaviour.Overwrite;
            notification.Priority = NotificationPriority.Warning;
            Notify(notification);
        }
        #endregion
    }
}
