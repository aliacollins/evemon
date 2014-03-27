using System;
using System.Collections.Generic;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class EveNotification : IEveMessage
    {
        private readonly CCPCharacter m_ccpCharacter;
        private readonly int m_senderID;

        private string m_sender;
        private bool m_queryPending;


        #region Constructor

        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="ccpCharacter">The CCP character.</param>
        /// <param name="src">The source.</param>
        internal EveNotification(CCPCharacter ccpCharacter, SerializableNotificationsListItem src)
        {
            m_ccpCharacter = ccpCharacter;

            NotificationID = src.NotificationID;
            Type = EveNotificationType.GetType(src.TypeID);
            m_senderID = src.SenderID;
            m_sender = GetIDToName();
            SentDate = src.SentDate;
            Recipient = new List<string> { ccpCharacter.Name };
            EVENotificationText = new EveNotificationText(new SerializableNotificationTextsListItem
                                                              {
                                                                  NotificationID = 0,
                                                                  NotificationText = String.Empty
                                                              });
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the EVE notification ID.
        /// </summary>
        /// <value>The notification ID.</value>
        public long NotificationID { get; private set; }

        /// <summary>
        /// Gets or sets the EVE notification type.
        /// </summary>
        /// <value>The type.</value>
        public string Type { get; private set; }

        /// <summary>
        /// Gets or sets the EVE notification sender name.
        /// </summary>
        public string SenderName
        {
            get
            {
                return m_sender == "Unknown"
                           ? m_sender = GetIDToName()
                           : m_sender;
            }
        }

        /// <summary>
        /// Gets or sets the sent date of the EVE notification.
        /// </summary>
        /// <value>The sent date.</value>
        public DateTime SentDate { get; private set; }

        /// <summary>
        /// Gets or sets the EVE notification recipient.
        /// </summary>
        /// <value>The recipient.</value>
        public IEnumerable<string> Recipient { get; private set; }

        /// <summary>
        /// Gets or sets the EVE notification text.
        /// </summary>
        /// <value>The EVE notification text.</value>
        public EveNotificationText EVENotificationText { get; private set; }

        /// <summary>
        /// Gets the EVE notification title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get { return Type; }
        }

        /// <summary>
        /// Gets the EVE notification text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get { return EVENotificationText.NotificationText.Normalize(); }
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Gets the name of the ID.
        /// </summary>
        /// <returns></returns>
        private string GetIDToName()
        {
            if (m_senderID == 0)
                return "(None)";

            // Look into EVEMon's data file if it's an NPC corporation or a players null sec corporation
            Station station = Station.GetByID(m_senderID);
            if (station != null)
                return station.CorporationName;

            // Look into EVEMon's data file if it's an agent
            // In case we didn't found any, query the API
            Agent agent = StaticGeography.GetAgentByID(m_senderID);
            return agent != null ? agent.Name : EveIDToName.GetIDToName(m_senderID);
        }

        #endregion


        #region Querying

        /// <summary>
        /// Gets the EVE notification.
        /// </summary>
        public void GetNotificationText()
        {
            // Exit if we are already trying to download the mail message body text
            if (m_queryPending)
                return;

            m_queryPending = true;

            // Quits if access denied
            APIKey apiKey = m_ccpCharacter.Identity.FindAPIKeyWithAccess(APICharacterMethods.MailingLists);
            if (apiKey == null)
                return;

            EveMonClient.APIProviders.CurrentProvider.QueryMethodAsync<SerializableAPINotificationTexts>(
                APICharacterMethods.NotificationTexts,
                apiKey.ID,
                apiKey.VerificationCode,
                m_ccpCharacter.CharacterID,
                NotificationID,
                OnEVENotificationTextDownloaded);
        }

        /// <summary>
        /// Processes the queried EVE notification text.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnEVENotificationTextDownloaded(APIResult<SerializableAPINotificationTexts> result)
        {
            m_queryPending = false;

            // Notify an error occured
            if (m_ccpCharacter.ShouldNotifyError(result, APICharacterMethods.NotificationTexts))
                EveMonClient.Notifications.NotifyEVENotificationTextsError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // If there is an error response on missing IDs inform the user
            if (!String.IsNullOrEmpty(result.Result.MissingMessageIDs))
            {
                result.Result.Texts.Add(
                    new SerializableNotificationTextsListItem
                    {
                        NotificationID = long.Parse(result.Result.MissingMessageIDs, CultureConstants.InvariantCulture),
                        NotificationText = "The text for this notification was reported missing."
                    });
            }

            // Quit if for any reason there is no text
            if (result.Result.Texts.Count == 0)
                return;

            // Import the data
            EVENotificationText = new EveNotificationText(result.Result.Texts[0]);

            EveMonClient.OnCharacterEVENotificationTextDownloaded(m_ccpCharacter);
        }

        #endregion
    }
}