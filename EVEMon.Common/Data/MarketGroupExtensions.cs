using System.Collections.Generic;
using System.Linq;

namespace EVEMon.Common.Data
{
    public static class MarketGroupExtensions
    {
        /// <summary>
        /// Gets true if the item's market group belongs to the questioned group. 
        /// </summary>
        public static bool BelongsIn(this MarketGroup marketGroup, IEnumerable<int> group)
        {
            int[] g = group.ToArray();
            while (marketGroup != null)
            {
                if (g.Any(x => x == marketGroup.ID))
                    return true;

                marketGroup = marketGroup.ParentGroup;
            }

            return false;
        }
    }
}