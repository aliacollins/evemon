using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace EVEMon.Common.Schedule
{
    public class ScheduleDateTimeRange
    {
        private DateTime m_from;
        private DateTime m_to;

        public ScheduleDateTimeRange(DateTime start, DateTime end)
        {
            m_from = start;
            m_to = end;
        }

        public DateTime From
        {
            get { return m_from; }
        }

        public DateTime To
        {
            get { return m_to; }
        }
    }

    [Flags]
    public enum ScheduleEntryOptions
    {
        None = 0,
        Blocking = 1, // Blocks skill training starting
        Quiet = 2, // Silences alerts
        EVETime = 4  // Uses EVETime
    }

    public abstract class ScheduleEntry
    {
        public abstract bool Contains(DateTime checkDateTime);
        public abstract bool Expired { get; }
        public abstract IEnumerable<ScheduleDateTimeRange> GetRangesInPeriod(DateTime fromDt, DateTime toDt);
        public abstract ScheduleEntryOptions ScheduleEntryOptions { set; get; }
        public abstract string Title { set; get; }
        public abstract bool Silent(DateTime timeToTest);
        public abstract bool Blocking(DateTime timeToTest);
        public abstract bool IsToday(DateTime timeToTest);
    }

    public class ScheduleEntryComparisonTitle : Comparer<ScheduleEntry>
    {
        public override int Compare(ScheduleEntry e1, ScheduleEntry e2)
        {
            return e1.Title.CompareTo(e2.Title);
        }
    }

    public class SimpleScheduleEntry : ScheduleEntry
    {
        private DateTime m_start = DateTime.MinValue;
        private DateTime m_end = DateTime.MinValue;
        private ScheduleEntryOptions m_seo = ScheduleEntryOptions.None;
        private string m_title = String.Empty;

        public DateTime StartDateTime
        {
            get { return m_start; }
            set { m_start = value; }
        }

        public DateTime EndDateTime
        {
            get { return m_end; }
            set { m_end = value; }
        }

        #region ScheduleEntry Members
        public override bool Contains(DateTime checkDateTime)
        {
            return (checkDateTime >= m_start && checkDateTime < m_end);
        }

        [XmlIgnore]
        public override bool Expired
        {
            get { return (DateTime.Now > m_end); }
        }

        public override IEnumerable<ScheduleDateTimeRange> GetRangesInPeriod(DateTime fromDt, DateTime toDt)
        {
            List<ScheduleDateTimeRange> result = new List<ScheduleDateTimeRange>();
            if ((m_start < fromDt && m_end > fromDt) ||
                (m_start >= fromDt && m_start <= toDt))
            {
                result.Add(new ScheduleDateTimeRange(m_start, m_end));
            }
            return result;
        }

        public override ScheduleEntryOptions ScheduleEntryOptions
        {
            get { return m_seo; }
            set { m_seo = value; }
        }

        public override string Title
        {
            get { return m_title; }
            set { m_title = value; }
        }

        private bool Clash(DateTime timeToTest)
        {
            DateTime testtime;
            if ((m_seo & ScheduleEntryOptions.EVETime) != 0)
                testtime = timeToTest.ToUniversalTime();
            else
                testtime = timeToTest;
            if (m_start <= testtime && testtime <= m_end)
            {
                return true;
            }
            return false;
        }

        public override bool Silent(DateTime timeToTest)
        {
            if ((m_seo & ScheduleEntryOptions.Quiet) != 0)
            {
                return Clash(timeToTest);
            }
            return false;
        }

        public override bool Blocking(DateTime timeToTest)
        {
            if ((m_seo & ScheduleEntryOptions.Blocking) != 0)
            {
                return Clash(timeToTest);
            }
            return false;
        }
        public override bool IsToday(DateTime timeToTest)
        {
            if (m_start.DayOfYear <= timeToTest.DayOfYear && m_end.DayOfYear >= timeToTest.DayOfYear &&
                m_start.Year <= timeToTest.Year && m_end.Year >= timeToTest.Year
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }

    public enum RecurFrequency
    {
        Daily,
        Weekdays,
        Weekends,
        Weekly,
        Monthly
    }

    public enum MonthlyOverflowResolution
    {
        ClipBack, // Apr 31 becomes Apr 30
        Drop, // Apr 31 is ignored
        OverlapForward // Apr 31 becomes May 1
    }

    public class RecurringScheduleEntry : ScheduleEntry
    {
        private string m_title = String.Empty;
        private ScheduleEntryOptions m_seo = ScheduleEntryOptions.None;
        private DateTime m_recurStart = DateTime.MinValue;

        public DateTime RecurStart
        {
            get { return m_recurStart; }
            set { m_recurStart = value; }
        }

        private DateTime m_recurEnd = DateTime.MaxValue;

        public DateTime RecurEnd
        {
            get { return m_recurEnd; }
            set { m_recurEnd = value; }
        }

        private RecurFrequency m_freq = RecurFrequency.Daily;

        public RecurFrequency RecurFrequency
        {
            get { return m_freq; }
            set { m_freq = value; }
        }

        private int m_recurWeeklyRepeat = 1;

        public int nWeekly
        {
            get { return m_recurWeeklyRepeat; }
            set { m_recurWeeklyRepeat = value; }
        }

        private DayOfWeek m_recurDayOfWeek = DayOfWeek.Monday;

        public DayOfWeek RecurDayOfWeek
        {
            get { return m_recurDayOfWeek; }
            set { m_recurDayOfWeek = value; }
        }

        private int m_recurDayOfMonth = 1;

        public int RecurDayOfMonth
        {
            get { return m_recurDayOfMonth; }
            set { m_recurDayOfMonth = value; }
        }

        private MonthlyOverflowResolution m_overflowResolution = MonthlyOverflowResolution.ClipBack;

        public MonthlyOverflowResolution OverflowResolution
        {
            get { return m_overflowResolution; }
            set { m_overflowResolution = value; }
        }

        private int m_startSecond;

        public int StartSecond
        {
            get { return m_startSecond; }
            set { m_startSecond = value; }
        }

        private int m_endSecond;

        public int EndSecond
        {
            get { return m_endSecond; }
            set { m_endSecond = value; }
        }

        public const int SECONDS_PER_DAY = 60*60*24;

        #region ScheduleEntry Members
        public override bool Contains(DateTime checkDateTime)
        {
            IEnumerable<ScheduleDateTimeRange> ranges = GetRangesInPeriod(checkDateTime, checkDateTime);
            foreach (ScheduleDateTimeRange sdtr in ranges)
            {
                if (checkDateTime >= sdtr.From && checkDateTime < sdtr.To)
                {
                    return true;
                }
            }
            return false;
        }

        [XmlIgnore]
        public override bool Expired
        {
            get { return DateTime.Now > m_recurEnd; }
        }

        public override IEnumerable<ScheduleDateTimeRange> GetRangesInPeriod(DateTime fromDt, DateTime toDt)
        {
            List<ScheduleDateTimeRange> result = new List<ScheduleDateTimeRange>();
            DateTime startDt = fromDt -
                               TimeSpan.FromMilliseconds(fromDt.Millisecond) -
                               TimeSpan.FromSeconds(fromDt.Second) -
                               TimeSpan.FromMinutes(fromDt.Minute) -
                               TimeSpan.FromHours(fromDt.Hour);
            DateTime endDt = toDt -
                             TimeSpan.FromMilliseconds(toDt.Millisecond) -
                             TimeSpan.FromSeconds(toDt.Second) -
                             TimeSpan.FromMinutes(toDt.Minute) -
                             TimeSpan.FromHours(toDt.Hour) + TimeSpan.FromDays(1);
            if (m_endSecond > SECONDS_PER_DAY)
            {
                startDt -= TimeSpan.FromDays(1);
            }
            DateTime wrkDt = startDt;
            while (wrkDt < endDt)
            {
                GetRangeForDay(wrkDt, result);
                wrkDt += TimeSpan.FromDays(1);
            }
            return result;
        }

        private void GetRangeForDay(DateTime wrkDt, List<ScheduleDateTimeRange> result)
        {
            bool appliesToday = false;
            switch (m_freq)
            {
                default:
                case RecurFrequency.Daily:
                    appliesToday = true;
                    break;
                case RecurFrequency.Weekdays:
                    if (wrkDt.DayOfWeek >= DayOfWeek.Monday && wrkDt.DayOfWeek <= DayOfWeek.Friday)
                    {
                        appliesToday = true;
                    }
                    else
                    {
                        appliesToday = false;
                    }
                    break;
                case RecurFrequency.Weekends:
                    if (wrkDt.DayOfWeek == DayOfWeek.Saturday || wrkDt.DayOfWeek == DayOfWeek.Sunday)
                    {
                        appliesToday = true;
                    }
                    else
                    {
                        appliesToday = false;
                    }
                    break;
                case RecurFrequency.Weekly:
                    DateTime FirstInstance = m_recurStart.AddDays((m_recurDayOfWeek - m_recurStart.DayOfWeek + 7) % 7);
                    if (wrkDt.DayOfWeek == m_recurDayOfWeek && (wrkDt.Subtract(FirstInstance).Days % (7 * m_recurWeeklyRepeat)) == 0)
                    {
                        appliesToday = true;
                    }
                    else
                    {
                        appliesToday = false;
                    }
                    break;
                case RecurFrequency.Monthly:
                    if (wrkDt.Day == m_recurDayOfMonth)
                    {
                        appliesToday = true;
                    }
                    else if (wrkDt.Day <= 3 && m_recurDayOfMonth >= 29)
                    {
                        appliesToday = IsOverflowDate(wrkDt, m_recurDayOfMonth);
                    }
                    else if (wrkDt.Day >= 28 && m_recurDayOfMonth >= 29)
                    {
                        appliesToday = IsOverflowDate(wrkDt, m_recurDayOfMonth);
                    }
                    else
                    {
                        appliesToday = false;
                    }
                    break;
            }
            if (!appliesToday)
            {
                return;
            }
            ScheduleDateTimeRange sdtr = new ScheduleDateTimeRange(
                wrkDt + TimeSpan.FromSeconds(m_startSecond),
                wrkDt + TimeSpan.FromSeconds(m_endSecond));
            result.Add(sdtr);
        }

        private bool IsOverflowDate(DateTime wrkDt, int m_recurDayOfMonth)
        {
            switch (m_overflowResolution)
            {
                default:
                case MonthlyOverflowResolution.Drop:
                    return false;
                case MonthlyOverflowResolution.OverlapForward:
                    DateTime lastDayOfPreviousMonthDt = wrkDt - TimeSpan.FromDays(wrkDt.Day);
                    int lastDayOfPreviousMonth = lastDayOfPreviousMonthDt.Day;
                    int dayOfThisMonth = wrkDt.Day;
                    if (m_recurDayOfMonth - lastDayOfPreviousMonth == dayOfThisMonth)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case MonthlyOverflowResolution.ClipBack:
                    DateTime searchForward = wrkDt + TimeSpan.FromDays(1);
                    if (wrkDt.Month == searchForward.Month)
                    {
                        return false;
                    }
                    else if (m_recurDayOfMonth > wrkDt.Day)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
            }
        }

        public override ScheduleEntryOptions ScheduleEntryOptions
        {
            get { return m_seo; }
            set { m_seo = value; }
        }

        public override string Title
        {
            get { return m_title; }
            set { m_title = value; }
        }

        private bool Clash(DateTime timeToTest)
        {
            DateTime testtime;
            if ((m_seo & ScheduleEntryOptions.EVETime) != 0)
                testtime = timeToTest.ToUniversalTime();
            else
                testtime = timeToTest;
            if (m_recurStart <= testtime && testtime <= m_recurEnd)
            {
                switch (m_freq)
                {
                    default:
                    case RecurFrequency.Daily:
                        if ((testtime.Date.Add(TimeSpan.FromSeconds(m_startSecond)) <= testtime && testtime <= testtime.Date.Add(TimeSpan.FromSeconds(m_endSecond))) ||
                            (testtime.AddDays(-1).Date.Add(TimeSpan.FromSeconds(m_startSecond)) <= testtime && testtime <= testtime.AddDays(-1).Date.Add(TimeSpan.FromSeconds(m_endSecond))))
                        {
                            return true;
                        }
                        break;
                    case RecurFrequency.Weekdays:
                        if (((testtime.DayOfWeek != DayOfWeek.Saturday && testtime.DayOfWeek != DayOfWeek.Sunday) && testtime.Date.Add(TimeSpan.FromSeconds(m_startSecond)) <= testtime && testtime <= testtime.Date.Add(TimeSpan.FromSeconds(m_endSecond))) ||
                            (((testtime.DayOfWeek == DayOfWeek.Saturday) && testtime.AddDays(-1).Date.Add(TimeSpan.FromSeconds(m_startSecond)) <= testtime && testtime <= testtime.AddDays(-1).Date.Add(TimeSpan.FromSeconds(m_endSecond)))))
                        {
                            return true;
                        }
                        break;
                    case RecurFrequency.Weekends:
                        if (((testtime.DayOfWeek == DayOfWeek.Saturday || testtime.DayOfWeek == DayOfWeek.Sunday) && testtime.Date.Add(TimeSpan.FromSeconds(m_startSecond)) <= testtime && testtime <= testtime.Date.Add(TimeSpan.FromSeconds(m_endSecond))) ||
                            ((testtime.DayOfWeek == DayOfWeek.Monday && testtime.AddDays(-1).Date.Add(TimeSpan.FromSeconds(m_startSecond)) <= testtime && testtime <= testtime.AddDays(-1).Date.Add(TimeSpan.FromSeconds(m_endSecond)))))
                        {
                            return true;
                        }
                        break;
                    case RecurFrequency.Weekly:
                        DateTime FirstInstance = m_recurStart.AddDays((m_recurDayOfWeek - m_recurStart.DayOfWeek + 7) % 7);
                        if ((testtime.DayOfWeek == m_recurDayOfWeek && (testtime.Subtract(FirstInstance).Days % (7 * m_recurWeeklyRepeat) == 0) && testtime.Date.Add(TimeSpan.FromSeconds(m_startSecond)) <= testtime && testtime <= testtime.Date.Add(TimeSpan.FromSeconds(m_endSecond))) ||
                            ((testtime.AddDays(-1).DayOfWeek == m_recurDayOfWeek && (testtime.AddDays(-1).Subtract(FirstInstance).Days % (7 * m_recurWeeklyRepeat) == 0) && testtime.AddDays(-1).Date.Add(TimeSpan.FromSeconds(m_startSecond)) <= testtime && testtime <= testtime.AddDays(-1).Date.Add(TimeSpan.FromSeconds(m_endSecond)))))
                        {
                            return true;
                        }
                        break;
                    case RecurFrequency.Monthly:
                        int dayofmonthdif = 0; // initialised to 0 so it doesn't trigger unexpectedly
                        switch (m_overflowResolution)
                        {
                            case MonthlyOverflowResolution.ClipBack:
                                if (m_recurDayOfMonth > DateTime.DaysInMonth(testtime.Year, testtime.Month))
                                {
                                    dayofmonthdif = DateTime.DaysInMonth(testtime.Year, testtime.Month);
                                }
                                break;
                            case MonthlyOverflowResolution.Drop:
                                dayofmonthdif = 0;
                                break;
                            case MonthlyOverflowResolution.OverlapForward:
                                if (m_recurDayOfMonth > DateTime.DaysInMonth(testtime.AddMonths(-1).Year, testtime.AddMonths(-1).Month))
                                {
                                    dayofmonthdif = m_recurDayOfMonth - DateTime.DaysInMonth(testtime.AddMonths(-1).Year, testtime.AddMonths(-1).Month);
                                }
                                break;
                        }
                        if (((testtime.Day == m_recurDayOfMonth || testtime.Day == dayofmonthdif) && testtime.Date.Add(TimeSpan.FromSeconds(m_startSecond)) <= testtime && testtime <= testtime.Date.Add(TimeSpan.FromSeconds(m_endSecond))) ||
                            (((testtime.AddDays(-1).Day == m_recurDayOfMonth || testtime.AddDays(-1).Day == dayofmonthdif) && testtime.AddDays(-1).Date.Add(TimeSpan.FromSeconds(m_startSecond)) <= testtime && testtime <= testtime.AddDays(-1).Date.Add(TimeSpan.FromSeconds(m_endSecond)))))
                        {
                            return true;
                        }
                        break;
                }
            }
            return false;
        }

        public override bool Silent(DateTime timeToTest)
        {
            if ((m_seo & ScheduleEntryOptions.Quiet) != 0)
            {
                return Clash(timeToTest);
            }
            return false;
        }

        public override bool Blocking(DateTime timeToTest)
        {
            if ((m_seo & ScheduleEntryOptions.Blocking) != 0)
            {
                return Clash(timeToTest);
            }
            return false;
        }

        public override bool IsToday(DateTime timeToTest)
        {
            List<ScheduleDateTimeRange> ranges = new List<ScheduleDateTimeRange>();
            this.GetRangeForDay(timeToTest, ranges);

            if (ranges.Count == 0)
            {
                return false;
            }

            // Check if this date is within the reoccurance time span
            if (m_recurStart.DayOfYear <= timeToTest.DayOfYear && m_recurEnd.DayOfYear >= timeToTest.DayOfYear &&
                m_recurStart.Year <= timeToTest.Year && m_recurEnd.Year >= timeToTest.Year
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}
