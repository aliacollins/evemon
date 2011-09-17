using System.Collections.Generic;

namespace EVEMon.Common.Scheduling
{
    public class ScheduleEntryTitleComparer : Comparer<ScheduleEntry>
    {
        public override int Compare(ScheduleEntry e1, ScheduleEntry e2)
        {
            if (e1 != null && e2 != null)
                return e1.Title.CompareTo(e2.Title);

            return 0;
        }
    }
}