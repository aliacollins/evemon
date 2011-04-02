using System;
using System.Collections.Generic;
using System.Linq;

using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class EveNotification : IEveMessage
    {
        private CCPCharacter m_ccpCharacter;


        #region Constructor

        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="src"></param>
        internal EveNotification(CCPCharacter ccpCharacter, SerializableNotificationsListItem src)
        {
            m_ccpCharacter = ccpCharacter;
            NotificationID = src.NotificationID;
            Type = GetType(src.TypeID);
            Sender = GetIDToName(src.SenderID.ToString());
            SentDate = src.SentDate;
            Recipient = GetRecipient();
        }

        #endregion

        private string GetType(int p)
        {
            return p.ToString();
        }

        private string GetIDToName(string p)
        {
            return p;
        }


        #region Properties

        public long NotificationID { get; private set; }

        public string Type { get; private set; }

        public string Sender { get; private set; }

        public DateTime SentDate { get; private set; }

        public List<string> Recipient { get; private set; }

        public EveNotificationText EVENotificationText { get; private set; }

        public bool NotificationTextDownloaded { get; private set; }

        public string Title { get { return Type; } }

        public string Text { get { return EVENotificationText.NotificationText; } }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Gets the recipient.
        /// </summary>
        /// <returns></returns>
        private List<string> GetRecipient()
        {
            Recipient = new List<string>();
            Recipient.Add(m_ccpCharacter.Name);

            return Recipient;
        }

        /// <summary>
        /// Gets the EVE notification.
        /// </summary>
        public void GetNotificationText()
        {
            var result = EveClient.APIProviders.CurrentProvider.QueryNotificationText(
                                                                    m_ccpCharacter.Identity.Account.UserID,
                                                                    m_ccpCharacter.Identity.Account.APIKey,
                                                                    m_ccpCharacter.CharacterID,
                                                                    NotificationID);

            OnEVENotificationTextDownloaded(result);
        }

        /// <summary>
        /// Processes the queried EVE mail message mail body.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnEVENotificationTextDownloaded(APIResult<SerializableAPINotificationTexts> result)
        {
            // Notify an error occured
            if (m_ccpCharacter.ShouldNotifyError(result, APIMethods.NotificationTexts))
                EveClient.Notifications.NotifyEVENotificationTextsError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            EVENotificationText = new EveNotificationText(result.Result.Texts[0]);
            NotificationTextDownloaded = true;
        }

        #endregion
    }
}
