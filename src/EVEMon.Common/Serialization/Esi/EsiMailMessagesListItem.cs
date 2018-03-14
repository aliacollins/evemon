using EVEMon.Common.Serialization.Eve;
using System.Globalization;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiMailMessagesListItem : EsiMailBase
    {
        [DataMember(Name = "mail_id")]
        public long MessageID { get; set; }

        [DataMember(Name = "is_read", IsRequired = false)]
        public bool Read { get; set; }

        public SerializableMailMessagesListItem ToXMLItem()
        {
            var ret = new SerializableMailMessagesListItem()
            {
                MessageID = MessageID,
                SenderID = SenderID,
                SentDate = SentDate,
                Title = Title,
            };

            // Split up the recipients by type
            if (Recipients != null)
                foreach (var recipient in Recipients)
                {
                    string id = recipient.RecipientID.ToString(CultureInfo.InvariantCulture);
                    switch (recipient.RecipientType)
                    {
                    case "corporation":
                        // Corp
                        ret.ToCorpID = id;
                        break;
                    case "alliance":
                        // Alliance
                        ret.ToAllianceID = id;
                        break;
                    case "mailing_list":
                        // List
                        ret.ToListID.Add(id);
                        break;
                    case "character":
                        // Character
                        ret.ToCharacterIDs.Add(id);
                        break;
                    default:
                        // Ignore
                        break;
                    }
                }

            return ret;
        }
    }
}
