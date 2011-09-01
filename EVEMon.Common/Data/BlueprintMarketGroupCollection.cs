using System.Collections.Generic;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents a read-only collection of blueprint groups
    /// </summary>
    public sealed class BlueprintMarketGroupCollection : ReadonlyCollection<BlueprintMarketGroup>
    {
        #region Constructor

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="group">The blueprint market group.</param>
        /// <param name="src">The source.</param>
        internal BlueprintMarketGroupCollection(BlueprintMarketGroup group, ICollection<SerializableBlueprintMarketGroup> src)
            : base(src == null ? 0 : src.Count)
        {
            if (src == null)
                return;

            foreach (SerializableBlueprintMarketGroup subGroup in src)
            {
                Items.Add(new BlueprintMarketGroup(group, subGroup));
            }
        }

        #endregion
    }
}