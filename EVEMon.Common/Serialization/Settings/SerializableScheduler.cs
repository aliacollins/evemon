using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Settings
{
    public sealed class SerializableScheduler
    {
        private readonly Collection<SerializableScheduleEntry> m_entries;

        public SerializableScheduler()
        {
            m_entries = new Collection<SerializableScheduleEntry>();
        }

        [XmlArray("entries")]
        [XmlArrayItem("simple", typeof(SerializableScheduleEntry))]
        [XmlArrayItem("recurring", typeof(SerializableRecurringScheduleEntry))]
        public Collection<SerializableScheduleEntry> Entries
        {
            get { return m_entries; }
        }
    }
}