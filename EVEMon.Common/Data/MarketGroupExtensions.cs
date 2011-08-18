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
            while (marketGroup != null)
            {
                if (group != null && group.Any(groupID => marketGroup.ID == groupID))
                    return true;

                marketGroup = marketGroup.ParentGroup;
            }

            return false;
        }
    }
}
