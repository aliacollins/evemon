using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using EVEMon.Common.Attributes;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common.Scheduling
{
    /// <summary>
    /// Holds the scheduling entries
    /// </summary>
    [EnforceUIThreadAffinity]
    public static class Scheduler
    {
        private static readonly List<ScheduleEntry> m_schedule = new List<ScheduleEntry>();

        /// <summary>
        /// Add the given entry
        /// </summary>
        /// <param name="entry"></param>
        public static void Add(ScheduleEntry entry)
        {
            m_schedule.Add(entry);

            // Notify to subscribers
            EveClient.OnSchedulerChanged();
        }

        /// <summary>
        /// Add the given entry
        /// </summary>
        /// <param name="entry"></param>
        public static void Remove(ScheduleEntry entry)
        {
            m_schedule.Remove(entry);

            // Notify to subscribers
            EveClient.OnSchedulerChanged();
        }

        /// <summary>
        /// Gets the scheduled entries
        /// </summary>
        public static IEnumerable<ScheduleEntry> Entries
        {
            get
            {
                foreach (var entry in m_schedule)
                {
                    if (!entry.Expired)
                        yield return entry;
                }
            }
        }

        /// <summary>
        /// Checks whether EVEMon is currently running in silent mode (no tooltips nor sounds).
        /// </summary>
        /// <returns></returns>
        public static bool SilentMode
        {
            get
            {
                return m_schedule.Any(x => x.Silent(DateTime.Now));
            }
        }

        /// <summary>
        /// Checks whether a certain datetime will fall on a time where the user won't be able to log in.
        /// </summary>
        /// <remarks>Checks both scheduling entries and downtimes.</remarks>
        /// <param name="time"></param>
        /// <param name="blockingEntry"></param>
        /// <returns></returns>
        public static bool SkillIsBlockedAt(DateTime time, out string blockingEntry)
        {
            blockingEntry = String.Empty;

            // Checks whether it will be on downtime
            if (time.ToUniversalTime().Hour == EveConstants.DowntimeHour)
            {
                blockingEntry = "DOWNTIME";
                return true;
            }

            // Checks schedule entries to see if they overlap the imput time
            foreach(var entry in m_schedule)
            {
                if (entry.Blocking(time))
                {
                    blockingEntry = entry.Title;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Imports data from the given serialization object.
        /// </summary>
        /// <param name="serial"></param>
        internal static void Import(SerializableScheduler serial)
        {
            m_schedule.Clear();
            foreach (var serialEntry in serial.Entries)
            {
                if (serialEntry is SerializableSimpleScheduleEntry)
                {
                    m_schedule.Add(new SimpleScheduleEntry(serialEntry as SerializableSimpleScheduleEntry));
                }
                else
                {
                    m_schedule.Add(new RecurringScheduleEntry(serialEntry as SerializableRecurringScheduleEntry));
                }
            }

            // Notify to subscribers
            EveClient.OnSchedulerChanged();
        }

        /// <summary>
        /// Exports data to a serialization object.
        /// </summary>
        /// <returns></returns>
        internal static SerializableScheduler Export()
        {
            SerializableScheduler serial = new SerializableScheduler();
            foreach (var entry in m_schedule)
            {
                if (!entry.Expired)
                    serial.Entries.Add(entry.Export());
            }
            return serial;
        }

        /// <summary>
        /// Clears all the expired entries.
        /// </summary>
        public static void ClearExpired()
        {
            // Removed the expired entries
            int i = 0;
            while (i < m_schedule.Count)
            {
                var entry = m_schedule[i];
                if (entry.Expired)
                {
                    m_schedule.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            // Notify to subscribers
            EveClient.OnSchedulerChanged();
        }
    }
}
