using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Serialization.API;
using YamlDotNet.RepresentationModel;

namespace EVEMon.Common
{
    public sealed class EveNotificationText
    {
        private readonly EveNotification m_notification;
        private readonly EveNotificationTextParser m_parser;
        private string m_parsedText;

        /// <summary>
        /// Initializes a new instance of the <see cref="EveNotificationText" /> class.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <param name="src">The source.</param>
        /// <exception cref="System.ArgumentNullException">src</exception>
        public EveNotificationText(EveNotification notification, SerializableNotificationTextsListItem src)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            NotificationID = src.NotificationID;
            NotificationText = src.NotificationText;
            m_notification = notification;
            m_parser = EveNotificationTextParser.GetParser();
        }


        #region Public Properties

        /// <summary>
        /// Gets or sets the notification ID.
        /// </summary>
        /// <value>The notification ID.</value>
        public long NotificationID { get; private set; }

        /// <summary>
        /// Gets the notification text.
        /// </summary>
        /// <value>
        /// The notification text.
        /// </value>
        internal string NotificationText { get; private set; }

        /// <summary>
        /// Gets or sets the notification text.
        /// </summary>
        /// <value>The notification text.</value>
        internal string ParsedText
        {
            get
            {
                return
                    m_parsedText = String.IsNullOrWhiteSpace(m_parsedText) || m_parsedText.Contains(EVEMonConstants.UnknownText)
                        ? Parse(EveNotificationType.GetTextLayout(m_notification.TypeID))
                            .NewLinesToBreakLines()
                            .DecodeUnicodeCharacters()
                            .Normalize()
                        : m_parsedText;
            }
        }

        #endregion


        /// <summary>
        /// Parses the specified notification text.
        /// </summary>
        /// <param name="textLayout">The text layout.</param>
        /// <returns></returns>
        internal string Parse(string textLayout = null)
        {
            if (String.IsNullOrWhiteSpace(textLayout))
                return NotificationText;

            if (!textLayout.Contains("{") || NotificationID == 0)
                return textLayout;

            YamlMappingNode pairs = Util.ParseYaml(NotificationText);

            IDictionary<string, string> parsedDict = pairs.Children.ToDictionary(pair => pair.Key.ToString(),
                pair => pair.Value.ToString());
            
            foreach (KeyValuePair<YamlNode, YamlNode> pair in pairs)
            {
                m_parser.Parse(m_notification, pair, parsedDict);
            }

            string parsedText = parsedDict.Aggregate(textLayout,
                (current, pair) => current.Replace(string.Format("{{{0}}}", pair.Key), pair.Value.Trim('\'')));

            return parsedText.Contains("{") ? NotificationText : parsedText;
        }
    }
}