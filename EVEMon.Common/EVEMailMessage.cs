using System;
using System.Collections.Generic;
using System.Linq;

using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class EveMailMessage : IEveMessage
    {
        private CCPCharacter m_ccpCharacter;


        #region Constructor

        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="src"></param>
        internal EveMailMessage(CCPCharacter ccpCharacter, SerializableMailMessagesListItem src)
        {
            m_ccpCharacter = ccpCharacter;
            State = (src.SenderID != ccpCharacter.CharacterID ?
                        EVEMailState.Inbox : EVEMailState.SentItem);
            MessageID = src.MessageID;
            Sender = src.ToListID.Any(x => x == src.SenderID.ToString()) ?
                        GetMailingListIDToName(src.SenderID.ToString()) : EveIDtoName.GetIDToName(src.SenderID.ToString());
            SentDate = src.SentDate;
            Title = src.Title;
            ToCorpOrAlliance = EveIDtoName.GetIDToName(src.ToCorpOrAllianceID);
            ToCharacters = GetIDsToNames(src.ToCharacterIDs);
            ToMailingLists = GetMailingListIDsToNames(src.ToListID);
            Recipient = GetRecipient();
        }

        #endregion


        #region Properties

        public EVEMailState State { get; private set; }

        public long MessageID { get; private set; }

        public string Sender { get; private set; }

        public DateTime SentDate { get; private set; }

        public string Title { get; private set; }

        public string ToCorpOrAlliance { get; private set; }

        public List<string> ToCharacters { get; private set; }

        public List<string> ToMailingLists { get; private set; }

        public List<string> Recipient { get; private set; }

        public EveMailBody EVEMailBody { get; private set; }

        public bool MailBodyDownloaded { get; private set; }

        public string Text { get { return EVEMailBody.BodyText; } }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Gets the names of the character IDs.
        /// </summary>
        /// <param name="src">A list of character IDs.</param>
        /// <returns>A list of character names</returns>
        private List<string> GetIDsToNames(List<string> src)
        {
            // If there are no IDs to query return a list with an empty entry
            if (src.Count == 0)
            {
                src.Add(String.Empty);
                return src;
            }

            List<string> listOfNames = new List<string>();
            List<string> listOfIDsToQuery = new List<string>();

            foreach (var id in src)
            {
                if (id == m_ccpCharacter.CharacterID.ToString())
                {
                    listOfNames.Add(m_ccpCharacter.Name);
                }
                else
                {
                    listOfIDsToQuery.Add(id);
                }
            }

            // We have IDs to query
            if (listOfIDsToQuery.Count > 0)
                listOfNames.AddRange(EveIDtoName.GetIDsToNames(listOfIDsToQuery));

            return listOfNames;
        }

        private string GetMailingListIDToName(string mailingListID)
        {
            // If there is no ID to query return an empty string
            if (String.IsNullOrEmpty(mailingListID))
                return String.Empty;

            // If it's a zero ID return "Unknown"
            if (mailingListID == "0")
                return "Unknown";

            List<string> list = new List<string>();
            list.Add(mailingListID);
            List<string> name = GetMailingListIDsToNames(list);
            return name[0];
        }

        private List<string> GetMailingListIDsToNames(List<string> mailingListIDs)
        {
            // If there are no IDs to query return a list with an empty entry
            if (mailingListIDs.Count == 0)
            {
                mailingListIDs.Add(String.Empty);
                return mailingListIDs;
            }

            List<string> listOfNames = new List<string>();

            foreach (var list in m_ccpCharacter.EVEMailingLists)
            {
                var name = mailingListIDs.FirstOrDefault(x => x == list.ID.ToString());
                if (name != null)
                {
                    listOfNames.Add(list.Name);
                }
                else
                {
                    listOfNames.Add("Unknown");
                }
            }

            // In case the list returned from the API is empty, add an "Unknown" entry
            if (listOfNames.Count == 0)
                listOfNames.Add("Unknown");

            return listOfNames;
        }

        /// <summary>
        /// Gets the recipient.
        /// </summary>
        /// <returns></returns>
        private List<string> GetRecipient()
        {
            Recipient = new List<string>();

            if (!String.IsNullOrEmpty(ToCharacters[0]))
            {
                Recipient.AddRange(ToCharacters);
                return Recipient;
            }

            if (!String.IsNullOrEmpty(ToCorpOrAlliance))
            {
                Recipient.Add(ToCorpOrAlliance);
                return Recipient;
            }

            if (!String.IsNullOrEmpty(ToMailingLists[0]))
            {
                Recipient.AddRange(ToMailingLists);
                return Recipient;
            }

            return Recipient;
        }

        #endregion


        #region Querying

        /// <summary>
        /// Gets the EVE mail body.
        /// </summary>
        public void GetMailBody()
        {
            var result = EveClient.APIProviders.CurrentProvider.QueryMailBody(
                                                                    m_ccpCharacter.Identity.Account.UserID,
                                                                    m_ccpCharacter.Identity.Account.APIKey,
                                                                    m_ccpCharacter.CharacterID,
                                                                    MessageID);

            OnEVEMailBodyDownloaded(result);
        }

        /// <summary>
        /// Processes the queried EVE mail message mail body.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnEVEMailBodyDownloaded(APIResult<SerializableAPIMailBodies> result)
        {
            // Notify an error occured
            if (m_ccpCharacter.ShouldNotifyError(result, APIMethods.MailBodies))
                EveClient.Notifications.NotifyEVEMailBodiesError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // Import the data
            EVEMailBody = new EveMailBody(result.Result.Bodies[0]);
            MailBodyDownloaded = true;
        }

        #endregion
    }
}
