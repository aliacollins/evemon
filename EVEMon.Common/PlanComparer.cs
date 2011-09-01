using System;
using System.Collections.Generic;

namespace EVEMon.Common
{
    /// <summary>
    /// Implements a plan comparer.
    /// </summary>
    public sealed class PlanComparer : Comparer<Plan>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sort"></param>
        public PlanComparer(PlanSort sort)
        {
            Sort = sort;
        }

        /// <summary>
        /// Sort order (ascending, descending).
        /// </summary>
        public SortOrder Order { get; set; }

        /// <summary>
        /// Sort criteria.
        /// </summary>
        public PlanSort Sort { get; set; }

        /// <summary>
        /// Comparison function.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override int Compare(Plan x, Plan y)
        {
            // Swap variables for descending order
            if (Order == SortOrder.Descending)
            {
                Plan tmp = y;
                y = x;
                x = tmp;
            }

            // Compare plans
            switch (Sort)
            {
                case PlanSort.Name:
                    if (x != null && y != null)
                        return String.Compare(x.Name, y.Name);
                    break;
                case PlanSort.Time:
                    if (x != null && y != null)
                    {
                        TimeSpan xtime = x.GetTotalTime(null, true);
                        TimeSpan ytime = y.GetTotalTime(null, true);
                        return TimeSpan.Compare(xtime, ytime);
                    }
                    break;
                case PlanSort.SkillsCount:
                    if (x != null && y != null)
                        return x.UniqueSkillsCount - y.UniqueSkillsCount;
                    break;
                default:
                    throw new NotImplementedException();
            }
            return 0;
        }
    }
}