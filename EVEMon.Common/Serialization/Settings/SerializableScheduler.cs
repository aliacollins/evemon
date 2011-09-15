using System.Collections.Generic;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Settings
{
    public sealed class SerializableScheduler
    {
        public SerializableScheduler()
        {
            Entries = new List<SerializableScheduleEntry>();
        }

        [XmlArray("entries")]
        [XmlArrayItem("simple", typeof(SerializableScheduleEntry))]
        [XmlArrayItem("recurring", typeof(SerializableRecurringScheduleEntry))]
        public List<SerializableScheduleEntry> Entries { get; set; }
    }
}