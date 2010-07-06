using System;

namespace EVEMon.Common
{
    public interface IGroupingListView<T>
    {
        /// <summary>
        /// Gets or sets the grouping of a listview.
        /// </summary>
        T Grouping { get; set; }
    }
}
