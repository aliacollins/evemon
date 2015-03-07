using System;
using System.Xml.Serialization;
using EVEMon.Common.Scheduling;

namespace EVEMon.Common.Serialization.Settings
{
    public sealed class SerializableRecurringScheduleEntry : SerializableScheduleEntry
    {
        [XmlElement("frequency")]
        public RecurringFrequency Frequency { get; set; }

        [XmlElement("weeksPeriod")]
        public int WeeksPeriod { get; set; }

        [XmlElement("dayOfWeek")]
        public DayOfWeek DayOfWeek { get; set; }

        [XmlElement("dayOfMonth")]
        public int DayOfMonth { get; set; }

        [XmlElement("overflowResolution")]
        public MonthlyOverflowResolution OverflowResolution { get; set; }

        [XmlElement("startTimeInSeconds")]
        public int StartTimeInSeconds { get; set; }

        [XmlElement("endTimeInSeconds")]
        public int EndTimeInSeconds { get; set; }
    }
}