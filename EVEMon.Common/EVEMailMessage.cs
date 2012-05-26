using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class EveMailMessage : IEveMessage
    {
        private readonly SerializableMailMessagesListItem m_source;
        private readonly CCPCharacter m_ccpCharacter;

        private IEnumerable<string> m_mailingLists;
        private IEnumerable<string> m_toCharacters;
        private string m_toCorpOrAlliance;
        private string m_sender; 
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
            m_source = src;

            State = (src.SenderID != ccpCharacter.CharacterID
                         ? EVEMailState.Inbox
                         : EVEMailState.SentItem);
            MessageID = src.MessageID;
            SentDate = src.SentDate;
            Title = src.Title.HtmlDecode();
            m_sender = GetSender();
            m_toCharacters = GetIDsToNames(src.ToCharacterIDs);
            m_mailingLists = GetMailingListIDsToNames(src.ToListID);
            m_toCorpOrAlliance = GetCorpOrAlliance(src.ToCorpOrAllianceID);
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
        public string Sender
        {
            get
            {
                return m_sender == "Unknown"
                           ? m_sender = GetSender()
                           : m_sender;
            }
        }

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
        public string ToCorpOrAlliance
        {
            get
            {
                return m_toCorpOrAlliance == "Unknown"
                           ? m_toCorpOrAlliance = GetCorpOrAlliance(m_source.ToCorpOrAllianceID)
                           : m_toCorpOrAlliance;
            }
        }

        /// <summary>
        /// Gets or sets the EVE mail recipient(s) (characters).
        /// </summary>
        public IEnumerable<string> ToCharacters
        {
            get
            {
                return m_toCharacters.Contains("Unknown")
                           ? m_toCharacters = GetIDsToNames(m_source.ToCharacterIDs)
                           : m_toCharacters;
            }
        }

        /// <summary>
        /// Gets or sets the EVE mail recipient (mailing lists).
        /// </summary>
        public IEnumerable<string> ToMailingLists
        {
            get
            {
                return m_mailingLists.Contains("Unknown")
                           ? m_mailingLists = GetMailingListIDsToNames(m_source.ToListID)
                           : m_mailingLists;
            }
        }

        /// <summary>
        /// Gets the recipient.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> Recipient
        {
            get
            {
                return !String.IsNullOrEmpty(ToCharacters.FirstOrDefault())
                           ? ToCharacters
                           : !String.IsNullOrEmpty(ToCorpOrAlliance)
                                 ? new List<string> { ToCorpOrAlliance }
                                 : !String.IsNullOrEmpty(ToMailingLists.FirstOrDefault())
                                       ? ToMailingLists
                                       : new EmptyEnumerable<string>();
            }
        }

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
        /// Gets the sender.
        /// </summary>
        /// <returns></returns>
        private string GetSender()
        {
            return m_source.ToListID.Any(x => x == m_source.SenderID.ToString(CultureConstants.InvariantCulture))
                       ? GetMailingListIDToName(m_source.SenderID.ToString(CultureConstants.InvariantCulture))
                       : GetIDToName(m_source.SenderID.ToString(CultureConstants.InvariantCulture));
        }

        /// <summary>
        /// Gets the sender.
        /// </summary>
        /// <param name="toCorpOrAlliance">The source.</param>
        /// <returns></returns>
        private string GetCorpOrAlliance(string toCorpOrAlliance)
        {
            return toCorpOrAlliance == m_ccpCharacter.Corporation.Name
                       ? m_ccpCharacter.Corporation.Name
                       : toCorpOrAlliance == m_ccpCharacter.AllianceName
                             ? m_ccpCharacter.AllianceName
                             : EveIDToName.GetIDToName(toCorpOrAlliance);
        }

        /// <summary>
        /// Gets the name of the ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        private string GetIDToName(string id)
        {
            // If it's a zero ID return "(None)"
            return id == "0" ? "(None)" : GetIDsToNames(new Collection<string> { id }).First();
        }

        /// <summary>
        /// Gets the names of the IDs.
        /// </summary>
        /// <param name="src">A list of IDs.</param>
        /// <returns>A list of names</returns>
        private IEnumerable<string> GetIDsToNames(ICollection<string> src)
        {
            // If there are no IDs to query return a list with an empty entry
            if (!src.Any())
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
            if (listOfIDsToQuery.Any())
                listOfNames.AddRange(EveIDToName.GetIDsToNames(listOfIDsToQuery));

            return listOfNames;
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
            return GetMailingListIDsToNames(list).First();
        }

        /// <summary>
        /// Gets the mailing list IDs to names.
        /// </summary>
        /// <param name="mailingListIDs">The mailing list IDs.</param>
        /// <returns></returns>
        private IEnumerable<string> GetMailingListIDsToNames(ICollection<string> mailingListIDs)
        {
            // If there are no IDs to query return a list with an empty entry
            if (!mailingListIDs.Any())
            {
                mailingListIDs.Add(String.Empty);
                return mailingListIDs;
            }

            List<string> listOfNames = mailingListIDs.Select(listID => m_ccpCharacter.EVEMailingLists.FirstOrDefault(
                x => x.ID.ToString(CultureConstants.InvariantCulture) == listID)).Select(
                    mailingList => mailingList != null ? mailingList.Name : "Unknown").ToList();

            // In case the list returned from the API is empty, add an "Unknown" entry
            if (!listOfNames.Any())
                listOfNames.Add("Unknown");

            return listOfNames;
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