using System;
using System.Collections.Generic;
using EVEMon.Common.Constants;
using EVEMon.Common.Interfaces;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Service;
using EVEMon.Common.Serialization.Esi;

namespace EVEMon.Common.Models
{
    public sealed class EveNotification : IEveMessage
    {
        private readonly CCPCharacter m_ccpCharacter;
        private readonly long m_senderID;

        private string m_senderName;
        private string m_title;

        #region Constructor

        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="ccpCharacter">The CCP character.</param>
        /// <param name="src">The source.</param>
        internal EveNotification(CCPCharacter ccpCharacter, EsiNotificationsListItem src)
        {
            m_ccpCharacter = ccpCharacter;

            NotificationID = src.NotificationID;
            TypeID = EveNotificationType.GetID(src.Type);
            m_senderID = src.SenderID;
            m_senderName = (m_senderID == 0L) ? "EVE System" : EveIDToName.GetIDToName(m_senderID);
            SentDate = src.SentDate;
            Recipient = new List<string> { ccpCharacter.Name };
            EVENotificationText = new EveNotificationText(this, new SerializableNotificationTextsListItem
            {
                NotificationID = 0,
                NotificationText = String.Empty,
            });
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets the CCP character.
        /// </summary>
        /// <value>
        /// The CCP character.
        /// </value>
        public CCPCharacter CCPCharacter => m_ccpCharacter;

        /// <summary>
        /// Gets the EVE notification ID.
        /// </summary>
        /// <value>The notification ID.</value>
        public long NotificationID { get; }

        /// <summary>
        /// Gets the EVE notification type.
        /// </summary>
        /// <value>The type.</value>
        public int TypeID { get; }

        /// <summary>
        /// Gets the EVE notification sender name. If the ID was zero, it was already
        /// prepopulated with "EVE System" so it will never be unknowntext.
        /// </summary>
        public string SenderName => (m_senderName == EveMonConstants.UnknownText) ?
            (m_senderName = EveIDToName.GetIDToName(m_senderID)) : m_senderName;

        /// <summary>
        /// Gets the sent date of the EVE notification.
        /// </summary>
        /// <value>The sent date.</value>
        public DateTime SentDate { get; }

        /// <summary>
        /// Gets the EVE notification recipient.
        /// </summary>
        /// <value>The recipient.</value>
        public IEnumerable<string> Recipient { get; }

        /// <summary>
        /// Gets the EVE notification text.
        /// </summary>
        /// <value>The EVE notification text.</value>
        public EveNotificationText EVENotificationText { get; private set; }

        /// <summary>
        /// Gets the EVE notification title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(m_title) && !m_title.Contains(EveMonConstants.UnknownText))
                    return m_title;

                string subjectLayout = EveNotificationType.GetSubjectLayout(TypeID);

                m_title = string.IsNullOrWhiteSpace(subjectLayout) ? EveMonConstants.UnknownText :
                    EVENotificationText.Parse(subjectLayout);

                m_title = m_title.Contains("{") || m_title == EVENotificationText.NotificationText ?
                    EveMonConstants.UnknownText : m_title;

                return m_title;
            }
        }

        /// <summary>
        /// Gets the EVE notification text.
        /// </summary>
        /// <value>The text.</value>
        public string Text => EVENotificationText.ParsedText;

        #endregion

    }
}
