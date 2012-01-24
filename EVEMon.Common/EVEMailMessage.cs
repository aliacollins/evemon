using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class EveMailMessage : IEveMessage
    {
        private readonly CCPCharacter m_ccpCharacter;
        private bool m_queryPending;


        #region Constructor

        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="ccpCharacter"></param>
        /// <param name="src"></param>
        internal EveMailMessage(CCPCharacter ccpCharacter, SerializableMailMessagesListItem src)
        {
            m_ccpCharacter = ccpCharacter;
            State = (src.SenderID != ccpCharacter.CharacterID
                         ? EVEMailState.Inbox
                         : EVEMailState.SentItem);
            MessageID = src.MessageID;
            Sender = src.ToListID.Any(x => x == src.SenderID.ToString(CultureConstants.InvariantCulture))
                         ? GetMailingListIDToName(src.SenderID.ToString(CultureConstants.InvariantCulture))
                         : EveIDToName.GetIDToName(src.SenderID.ToString(CultureConstants.InvariantCulture));
            SentDate = src.SentDate;
            Title = src.Title.HtmlDecode();
            ToCorpOrAlliance = EveIDToName.GetIDToName(src.ToCorpOrAllianceID);
            ToCharacters = GetIDsToNames(src.ToCharacterIDs);
            ToMailingLists = GetMailingListIDsToNames(src.ToListID);
            Recipient = GetRecipient();
            EVEMailBody = new EveMailBody(new SerializableMailBodiesListItem
                                              {
                                                  MessageID = 0,
                                                  MessageText = String.Empty
                                              });
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the EVE mail state.
        /// </summary>
        /// <value>The state.</value>
        public EVEMailState State { get; private set; }

        /// <summary>
        /// Gets or sets the EVE mail message ID.
        /// </summary>
        /// <value>The message ID.</value>
        public long MessageID { get; private set; }

        /// <summary>
        /// Gets or sets the EVE mail sender name.
        /// </summary>
        /// <value>The sender.</value>
        public string Sender { get; private set; }

        /// <summary>
        /// Gets or sets the sent date of the EVE mail.
        /// </summary>
        /// <value>The sent date.</value>
        public DateTime SentDate { get; private set; }

        /// <summary>
        /// Gets or sets the EVE mail title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; private set; }

        /// <summary>
        /// Gets or sets the EVE mail recipient (corp or alliance).
        /// </summary>
        /// <value>To corp or alliance.</value>
        public string ToCorpOrAlliance { get; private set; }

        /// <summary>
        /// Gets or sets the EVE mail recipient(s) (characters).
        /// </summary>
        /// <value>To characters.</value>
        public Collection<string> ToCharacters { get; private set; }

        /// <summary>
        /// Gets or sets the EVE mail recipient (mailing lists).
        /// </summary>
        /// <value>To mailing lists.</value>
        public Collection<string> ToMailingLists { get; private set; }

        /// <summary>
        /// Gets or sets the recipients.
        /// </summary>
        /// <value>The recipient.</value>
        public IEnumerable<string> Recipient { get; private set; }

        /// <summary>
        /// Gets or sets the EVE mail body.
        /// </summary>
        /// <value>The EVE mail body.</value>
        public EveMailBody EVEMailBody { get; private set; }

        /// <summary>
        /// Gets the EVE mail body text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get { return EVEMailBody.BodyText.Normalize(); }
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Gets the names of the character IDs.
        /// </summary>
        /// <param name="src">A list of character IDs.</param>
        /// <returns>A list of character names</returns>
        private Collection<string> GetIDsToNames(Collection<string> src)
        {
            // If there are no IDs to query return a list with an empty entry
            if (src.Count == 0)
            {
                src.Add(String.Empty);
                return src;
            }

            List<string> listOfNames = new List<string>();
            List<string> listOfIDsToQuery = new List<string>();

            foreach (string id in src)
            {
                if (id == m_ccpCharacter.CharacterID.ToString(CultureConstants.InvariantCulture))
                    listOfNames.Add(m_ccpCharacter.Name);
                else
                    listOfIDsToQuery.Add(id);
            }

            // We have IDs to query
            if (listOfIDsToQuery.Count > 0)
                listOfNames.AddRange(EveIDToName.GetIDsToNames(listOfIDsToQuery));

            return new Collection<string>(listOfNames);
        }

        /// <summary>
        /// Gets the name of the mailing list ID.
        /// </summary>
        /// <param name="mailingListID">The mailing list ID.</param>
        /// <returns></returns>
        private string GetMailingListIDToName(string mailingListID)
        {
            // If there is no ID to query return an empty string
            if (String.IsNullOrEmpty(mailingListID))
                return String.Empty;

            // If it's a zero ID return "Unknown"
            if (mailingListID == "0")
                return "Unknown";

            Collection<string> list = new Collection<string> { mailingListID };
            Collection<string> name = GetMailingListIDsToNames(list);
            return name[0];
        }

        /// <summary>
        /// Gets the mailing list IDs to names.
        /// </summary>
        /// <param name="mailingListIDs">The mailing list IDs.</param>
        /// <returns></returns>
        private Collection<string> GetMailingListIDsToNames(Collection<string> mailingListIDs)
        {
            // If there are no IDs to query return a list with an empty entry
            if (mailingListIDs.Count == 0)
            {
                mailingListIDs.Add(String.Empty);
                return mailingListIDs;
            }

            List<string> listOfNames = mailingListIDs.Select(listID => m_ccpCharacter.EVEMailingLists.FirstOrDefault(
                x => x.ID.ToString(CultureConstants.InvariantCulture) == listID)).Select(
                    mailingList => mailingList != null ? mailingList.Name : "Unknown").ToList();

            // In case the list returned from the API is empty, add an "Unknown" entry
            if (listOfNames.Count == 0)
                listOfNames.Add("Unknown");

            return new Collection<string>(listOfNames);
        }

        /// <summary>
        /// Gets the recipient.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<string> GetRecipient()
        {
            Recipient = new List<string>();

            if (!String.IsNullOrEmpty(ToCharacters[0]))
            {
                Recipient = ToCharacters.ToList();
                return Recipient;
            }

            if (!String.IsNullOrEmpty(ToCorpOrAlliance))
            {
                Recipient = new List<string> { ToCorpOrAlliance };
                return Recipient;
            }

            if (!String.IsNullOrEmpty(ToMailingLists[0]))
            {
                Recipient = ToMailingLists.ToList();
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
            // Exit if we are already trying to download the mail message body text
            if (m_queryPending)
                return;

            m_queryPending = true;

            // Quits if access denied
            APIKey apiKey = m_ccpCharacter.Identity.FindAPIKeyWithAccess(APICharacterMethods.MailBodies);
            if (apiKey == null)
                return;

            EveMonClient.APIProviders.CurrentProvider.QueryMethodAsync<SerializableAPIMailBodies>(
                APICharacterMethods.MailBodies,
                apiKey.ID,
                apiKey.VerificationCode,
                m_ccpCharacter.CharacterID,
                MessageID,
                OnEVEMailBodyDownloaded);
        }

        /// <summary>
        /// Processes the queried EVE mail message mail body.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnEVEMailBodyDownloaded(APIResult<SerializableAPIMailBodies> result)
        {
            m_queryPending = false;

            // Notify an error occured
            if (m_ccpCharacter.ShouldNotifyError(result, APICharacterMethods.MailBodies))
                EveMonClient.Notifications.NotifyEVEMailBodiesError(m_ccpCharacter, result);

            // Quits if there is an error
            if (result.HasError)
                return;

            // If there is an error response on missing IDs inform the user
            if (!String.IsNullOrEmpty(result.Result.MissingMessageIDs))
            {
                result.Result.Bodies.Add(
                    new SerializableMailBodiesListItem
                        {
                            MessageID = long.Parse(result.Result.MissingMessageIDs, CultureConstants.InvariantCulture),
                            MessageText = "The text for this message was reported missing."
                        });
            }

            // Quit if for any reason there is no text
            if (!result.Result.Bodies.Any())
                return;

            // Import the data
            EVEMailBody = new EveMailBody(result.Result.Bodies.First());

            EveMonClient.OnCharacterEVEMailBodyDownloaded(m_ccpCharacter);
        }

        #endregion
    }
}