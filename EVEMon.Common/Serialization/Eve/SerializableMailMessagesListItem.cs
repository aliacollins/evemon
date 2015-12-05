using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using EVEMon.Common.Collections;
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableMailMessagesListItem
    {
        private readonly Collection<string> m_toCharacterIDs;
        private readonly Collection<string> m_toListID;

        public SerializableMailMessagesListItem()
        {
            m_toCharacterIDs = new Collection<string>();
            m_toListID = new Collection<string>();
        }

        [XmlAttribute("messageID")]
        public long MessageID { get; set; }

        [XmlAttribute("senderID")]
        public long SenderID { get; set; }

        [XmlAttribute("senderName")]
        public string SenderName { get; set; }

        [XmlAttribute("sentDate")]
        public string SentDateXml
        {
            get { return SentDate.DateTimeToTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    SentDate = value.TimeStringToDateTime();
            }
        }

        [XmlAttribute("title")]
        public string TitleXml
        {
            get { return Title; }
            set { Title = value == null ? String.Empty : value.HtmlDecode(); }
        }

        [XmlAttribute("toCorpOrAllianceID")]
        public string ToCorpOrAllianceID { get; set; }

        [XmlAttribute("toCharacterIDs")]
        public string ToCharacterIDsXml
        {
            get { return String.Join(",", m_toCharacterIDs); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    m_toCharacterIDs.AddRange(value.Split(','));
            }
        }

        [XmlAttribute("toListID")]
        public string ToListIDXml
        {
            get { return String.Join(",", m_toListID); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    m_toListID.AddRange(value.Split(','));
            }
        }

        [XmlIgnore]
        public string Title { get; set; }

        [XmlIgnore]
        public DateTime SentDate { get; set; }

        [XmlIgnore]
        public Collection<string> ToCharacterIDs
        {
            get { return m_toCharacterIDs; }
        }

        [XmlIgnore]
        public Collection<string> ToListID
        {
            get { return m_toListID; }
        }
    }
}