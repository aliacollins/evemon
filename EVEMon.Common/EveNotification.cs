using System;
using System.Collections.Generic;
using System.Linq;

using EVEMon.Common.Data;
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
            Type = EveNotificationType.GetType(src.TypeID);
            Sender = GetIDToName(src.SenderID);
            SentDate = src.SentDate;
            Recipient = GetRecipient();
        }

        #endregion


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
        /// Gets the name of the ID.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        private string GetIDToName(long ID)
        {
            // Look into EVEMon's data file if it's an NPC corporation or agent
            foreach (var station in StaticGeography.AllStations)
            {
                if (station.CorporationID == ID && station.CorporationName != null)
                    return station.CorporationName;

                if (station.Agents.Any(x => x.ID == ID))
                    return station.Agents.First(x => x.ID == ID).Name;
            }

            // Lookup if it's a players null sec corporation
            // (while we have the data we can avoid unnecessary queries to the API)
            Station conqStation = ConquerableStation.AllStations.FirstOrDefault(x => x.CorporationID == ID);
            if (conqStation != null)
                return conqStation.CorporationName;

            // Didn't found any ? Query the API
            return EveIDtoName.GetIDToName(ID.ToString());
        }

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

        #endregion


        #region Querying

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
