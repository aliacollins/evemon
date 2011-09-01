using System.Collections.Generic;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents a read-only collection of items.
    /// </summary>
    public sealed class MarketGroupCollection : ReadonlyCollection<MarketGroup>
    {
        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="src">The SRC.</param>
        internal MarketGroupCollection(MarketGroup group, ICollection<SerializableMarketGroup> src)
            : base(src == null ? 0 : src.Count)
        {
            if (src == null)
                return;

            foreach (SerializableMarketGroup subCat in src)
            {
                Items.Add(new MarketGroup(group, subCat));
            }
        }
    }
}