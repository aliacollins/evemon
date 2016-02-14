using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Eve
{
    /// <summary>
    /// Represents a serializable version of a characters' eve mailing lists. Used for querying CCP.
    /// </summary>
    public sealed class SerializableAPIMailingLists
    {
        private readonly Collection<SerializableMailingListsListItem> m_mailingLists;

        public SerializableAPIMailingLists()
        {
            m_mailingLists = new Collection<SerializableMailingListsListItem>();
        }

        [XmlArray("mailingLists")]
        [XmlArrayItem("mailingList")]
        public Collection<SerializableMailingListsListItem> MailingLists => m_mailingLists;
    }
}