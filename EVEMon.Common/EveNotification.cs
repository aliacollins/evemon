using System;
using System.Collections.Generic;
using System.Web;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class EveNotification : IEveMessage
    {
        private readonly CCPCharacter m_ccpCharacter;
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
            Sender = GetIDToName(src.SenderID);
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
        /// <value>The sender.</value>
        public string Sender { get; private set; }

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
            get { return HttpUtility.HtmlDecode(EVENotificationText.NotificationText.Normalize()); }
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Gets the name of the ID.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <returns></returns>
        private static string GetIDToName(long id)
        {
            if (id == 0)
                return "Unknown";

            // Look into EVEMon's data file if it's an NPC corporation or a players null sec corporation
            Station station = StaticGeography.GetStationByID(id) ?? ConquerableStation.GetStationByID(id);
            if (station != null)
                return station.CorporationName;

            // Look into EVEMon's data file if it's an agent
            // In case we didn't found any, query the API
            Agent agent = StaticGeography.GetAgentByID(id);
            return agent != null ? agent.Name : EveIDToName.GetIDToName(id);
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

            EveMonClient.APIProviders.CurrentProvider.QueryMethodAsync<SerializableAPINotificationTexts>(
                APIMethods.NotificationTexts,
                m_ccpCharacter.Identity.APIKey.ID,
                m_ccpCharacter.Identity.APIKey.VerificationCode,
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
            if (m_ccpCharacter.ShouldNotifyError(result, APIMethods.NotificationTexts))
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
                        NotificationID = long.Parse(result.Result.MissingMessageIDs),
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