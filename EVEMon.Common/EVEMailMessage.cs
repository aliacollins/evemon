using System;
using System.Collections.Generic;

using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class EveMailMessage
    {
        #region Constructor

        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="src"></param>
        internal EveMailMessage(Character character, SerializableMailMessagesListItem src)
        {
            Character = character;
            State = (src.SenderID != character.CharacterID ?
                        EVEMailState.Inbox : EVEMailState.SentItem);
            MessageID = src.MessageID;
            Sender = GetIDToName(src.SenderID.ToString());
            SentDate = src.SentDate;
            Title = src.Title;
            ToCorpOrAlliance = GetIDToName(src.ToCorpOrAllianceID);
            ToCharacters = GetIDToName(src.ToCharacterIDs);
            ToListID = GetIDToName(src.ToListID);
        }

        #endregion


        #region Properties

        private Character Character { get; set; }

        public EVEMailState State { get; set; }

        public long MessageID { get; set; }

        public string Sender { get; set; }

        public DateTime SentDate { get; set; }

        public string Title { get; set; }

        public string ToCorpOrAlliance { get; set; }

        public List<string> ToCharacters { get; set; }

        public List<string> ToListID { get; set; }

        public EveMailBody EVEMailBody { get; set; }
        
        public bool MailBodyDownloaded { get; set; }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Gets the name of the character ID.
        /// </summary>
        /// <param name="senderId">The sender id.</param>
        /// <returns></returns>
        private string GetIDToName(string senderId)
        {
            if (String.IsNullOrEmpty(senderId))
                return String.Empty;

            if (senderId == "0")
                return "Unknown";

            List<string> list = new List<string>();
            list.Add(senderId);

            List<string> name = GetIDToName(list);
            return (name.Count == 0 ? "Unknown" : name[0]);
        }

        /// <summary>
        /// Gets the names of the character IDs.
        /// </summary>
        /// <param name="src">A list of character IDs.</param>
        /// <returns>A list of character names</returns>
        private List<string> GetIDToName(List<string> src)
        {
            if (src.Count == 0)
            {
                src.Add(String.Empty);
                return src;
            }

            List<string> listOfNames = new List<string>();
            List<string> listOfIDsToGet = new List<string>();

            foreach (var id in src)
            {
                if (id == Character.CharacterID.ToString())
                {
                    listOfNames.Add(Character.Name);
                }
                else
                {
                    listOfIDsToGet.Add(id);
                }
            }

            if (listOfIDsToGet.Count > 0)
                listOfNames.AddRange(EveIDtoName.GetIDsToNames(listOfIDsToGet));
            
            return listOfNames;
        }

        /// <summary>
        /// Gets the EVE mail body.
        /// </summary>
        public void GetMailBody()
        {
            var result = EveClient.APIProviders.CurrentProvider.QueryMailBody(
                                                                    Character.Identity.Account.UserID,
                                                                    Character.Identity.Account.APIKey,
                                                                    Character.CharacterID,
                                                                    MessageID);

            OnEVEMailBodyDownloaded(result);
        }

        /// <summary>
        /// Called when the EVE mail body has been downloaded.
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnEVEMailBodyDownloaded(APIResult<SerializableAPIMailBodies> result)
        {
            CCPCharacter ccpCharacter = Character as CCPCharacter;
            if (ccpCharacter == null)
                return;

            // Notify an error occured
            if (ccpCharacter.ShouldNotifyError(result, APIMethods.MailBodies))
                EveClient.Notifications.NotifyEVEMailBodiesError(ccpCharacter, result);

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
