using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class EveNotificationCollection : ReadonlyCollection<EveNotification>
    {
        private readonly CCPCharacter m_ccpCharacter;


        #region Constructor

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="ccpCharacter">The CCP character.</param>
        internal EveNotificationCollection(CCPCharacter ccpCharacter)
        {
            m_ccpCharacter = ccpCharacter;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the number of new notifications.
        /// </summary>
        /// <value>The new notifications.</value>
        public int NewNotifications { get; private set; }

        #endregion


        #region Importation & Exportation

        /// <summary>
        /// Imports the eve notifications IDs from a serializable object.
        /// </summary>
        /// <param name="eveNotificationsIDs">The eve notifications IDs.</param>
        internal void Import(string eveNotificationsIDs)
        {
            if (String.IsNullOrEmpty(eveNotificationsIDs))
                return;

            Items.Clear();
            List<string> ids = eveNotificationsIDs.Split(',').ToList();
            foreach (long id in ids.Select(long.Parse))
            {
                Items.Add(new EveNotification(m_ccpCharacter,
                                              new SerializableNotificationsListItem
                                                  {
                                                      NotificationID = id
                                                  }));
            }
        }

        /// <summary>
        /// Imports an enumeration of API objects.
        /// </summary>
        /// <param name="src">The enumeration of serializable notifications from the API.</param>
        internal void Import(IEnumerable<SerializableNotificationsListItem> src)
        {
            NewNotifications = 0;

            List<EveNotification> newNotifications = new List<EveNotification>();

            // Import the notifications from the API
            foreach (SerializableNotificationsListItem srcEVENotification in src)
            {
                // If it's a new notification increase the counter
                EveNotification notification = Items.FirstOrDefault(x => x.NotificationID == srcEVENotification.NotificationID);
                if (notification == null)
                    NewNotifications++;

                newNotifications.Add(new EveNotification(m_ccpCharacter, srcEVENotification));
            }

            Items.Clear();
            Items.AddRange(newNotifications);

            // Fires the event regarding EVE mail messages update
            EveMonClient.OnCharacterEVENotificationsUpdated(m_ccpCharacter);
        }

        /// <summary>
        /// Exports the eve notifications IDs to a serializable object.
        /// </summary>
        /// <returns></returns>
        internal String Export()
        {
            return String.Join(",", Items.Select(notification => notification.NotificationID.ToString(CultureConstants.InvariantCulture)));
        }

        #endregion
    }
}