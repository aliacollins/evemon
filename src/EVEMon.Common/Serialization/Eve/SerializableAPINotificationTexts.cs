using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Eve
{
    public sealed class SerializableAPINotificationTexts
    {
        private readonly Collection<SerializableNotificationTextsListItem> m_texts;

        public SerializableAPINotificationTexts()
        {
            m_texts = new Collection<SerializableNotificationTextsListItem>();
        }

        [XmlArray("notifications")]
        [XmlArrayItem("notification")]
        public Collection<SerializableNotificationTextsListItem> Texts => m_texts;

        [XmlElement("missingIDs")]
        public string MissingMessageIDs { get; set; }
    }
}