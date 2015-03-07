using System.Collections.Generic;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public class EveFactionWarfareStatsCollection : ReadonlyCollection<EveFactionWarfareStats>
    {
        /// <summary>
        /// Imports an enumeration of API objects.
        /// </summary>
        /// <param name="src">The source.</param>
        internal void Import(IEnumerable<SerializableEveFactionalWarfareStatsListItem> src)
        {
            Items.Clear();

            foreach (SerializableEveFactionalWarfareStatsListItem item in src)
            {
                Items.Add(new EveFactionWarfareStats(item));
            }
        }
    }
}