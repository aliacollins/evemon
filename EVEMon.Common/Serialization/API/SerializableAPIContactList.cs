using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    /// <summary>
    /// Represents a serializable version of contact list. Used for querying CCP.
    /// </summary>
    public sealed class SerializableAPIContactList
    {
        private readonly Collection<SerializableContactListItem> m_contacts;
        private readonly Collection<SerializableContactListItem> m_corporateContacts;
        private readonly Collection<SerializableContactListItem> m_allianceContacts;

        public SerializableAPIContactList()
        {
            m_contacts = new Collection<SerializableContactListItem>();
            m_corporateContacts = new Collection<SerializableContactListItem>();
            m_allianceContacts = new Collection<SerializableContactListItem>();
        }

        [XmlArray("contacts")]
        [XmlArrayItem("contact")]
        public Collection<SerializableContactListItem> Contacts
        {
            get
            {
                foreach (SerializableContactListItem contact in m_contacts)
                {
                    contact.Group = ContactGroup.Contact;
                }
                return m_contacts;
            }
        }

        [XmlArray("corporateContacts")]
        [XmlArrayItem("corporateContact")]
        public Collection<SerializableContactListItem> CorporateContacts
        {
            get
            {
                foreach (SerializableContactListItem corporateContact in m_corporateContacts)
                {
                    corporateContact.Group = ContactGroup.Corporate;
                }
                return m_corporateContacts;
            }
        }

        [XmlArray("allianceContacts")]
        [XmlArrayItem("allianceContact")]
        public Collection<SerializableContactListItem> AllianceContacts
        {
            get
            {
                foreach (SerializableContactListItem allianceContact in m_allianceContacts)
                {
                    allianceContact.Group = ContactGroup.Alliance;
                }
                return m_allianceContacts;
            }
        }

        [XmlIgnore]
        public IEnumerable<SerializableContactListItem> AllContacts
        {
            get
            {
                List<SerializableContactListItem> contacts = new List<SerializableContactListItem>();
                contacts.AddRange(Contacts);
                contacts.AddRange(CorporateContacts);
                contacts.AddRange(AllianceContacts);
                return contacts;
            }
        }
    }
}
