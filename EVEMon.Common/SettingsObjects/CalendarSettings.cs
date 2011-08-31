using System;
using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    public sealed class CalendarSettings
    {
        public CalendarSettings()
        {
            GoogleURL = NetworkConstants.GoogleCalendarURL;
            GoogleReminder = GoogleCalendarReminder.Email;
            RemindingInterval = 10;

            EarlyReminding = DateTime.Now.Date.AddHours(8);
            LateReminding = DateTime.Now.Date.AddHours(20);
            LastQueuedSkillOnly = true;
        }

        [XmlAttribute("enabled")]
        public bool Enabled { get; set; }

        [XmlElement("provider")]
        public CalendarProvider Provider { get; set; }

        [XmlElement("useReminding")]
        public bool UseReminding { get; set; }

        /// <summary>
        /// Interval between remindings, in minutes
        /// </summary>
        [XmlElement("remindingInterval")]
        public int RemindingInterval { get; set; }

        [XmlElement("remindingRange")]
        public bool UseRemindingRange { get; set; }

        [XmlElement("earlyReminding")]
        public DateTime EarlyReminding { get; set; }

        [XmlElement("lateReminding")]
        public DateTime LateReminding { get; set; }

        [XmlElement("googleEmail")]
        public string GoogleEmail { get; set; }

        [XmlElement("googlePassword")]
        public string GooglePassword { get; set; }

        [XmlElement("googleUrl")]
        public string GoogleURL { get; set; }

        [XmlElement("googleReminder")]
        public GoogleCalendarReminder GoogleReminder { get; set; }

        [XmlElement("lastQueuedSkillOnly")]
        public bool LastQueuedSkillOnly { get; set; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal CalendarSettings Clone()
        {
            return (CalendarSettings)MemberwiseClone();
        }
    }
}
