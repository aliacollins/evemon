using System;
using System.Collections.Generic;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common.Scheduling
{
    /// <summary>
    /// Represents a schedule entry which occurs once only.
    /// </summary>
    public class SimpleScheduleEntry : ScheduleEntry
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SimpleScheduleEntry()
        {
        }

        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="serial"></param>
        internal SimpleScheduleEntry(SerializableScheduleEntry serial)
        {
            StartDate = serial.StartDate;
            EndDate = serial.EndDate;
            Title = serial.Title;
            Options = serial.Options;
        }

        /// <summary>
        /// Checks whether the given time is contained within this entry
        /// </summary>
        /// <param name="checkDateTime"></param>
        /// <returns></returns>
        public override bool Contains(DateTime checkDateTime)
        {
            return (checkDateTime >= StartDate && checkDateTime < EndDate);
        }

        /// <summary>
        /// Gets an enumeration of ranges for this entry within the given interval. It will return at most one range.
        /// </summary>
        /// <param name="fromDt"></param>
        /// <param name="toDt"></param>
        /// <returns></returns>
        public override IEnumerable<ScheduleDateTimeRange> GetRangesInPeriod(DateTime fromDt, DateTime toDt)
        {
            if ((StartDate < fromDt && EndDate > fromDt) || (StartDate >= fromDt && StartDate <= toDt))
                yield return new ScheduleDateTimeRange(StartDate, EndDate);
        }

        /// <summary>
        /// Checks whether the given time intersects with this entry.
        /// </summary>
        /// <param name="timeToTest"></param>
        /// <returns></returns>
        protected override bool Clash(DateTime timeToTest)
        {
            DateTime testtime = (Options & ScheduleEntryOptions.EVETime) != 0 ? timeToTest.ToUniversalTime() : timeToTest;
            return StartDate <= testtime && testtime <= EndDate;
        }

        /// <summary>
        /// Checks whether this entry occurs on the given day.
        /// </summary>
        /// <param name="timeToTest"></param>
        /// <returns></returns>
        public override bool IsToday(DateTime timeToTest)
        {
            return StartDate.DayOfYear <= timeToTest.DayOfYear && EndDate.DayOfYear >= timeToTest.DayOfYear &&
                   StartDate.Year <= timeToTest.Year && EndDate.Year >= timeToTest.Year;
        }

        /// <summary>
        /// Exports the data to a serialization object.
        /// </summary>
        /// <returns></returns>
        internal override SerializableScheduleEntry Export()
        {
            return new SerializableScheduleEntry
                       {
                           StartDate = StartDate,
                           EndDate = EndDate,
                           Title = Title,
                           Options = Options
                       };
        }
    }
}