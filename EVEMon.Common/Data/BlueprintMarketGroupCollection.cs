using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents a read-only collection of blueprint groups
    /// </summary>
    public sealed class BlueprintMarketGroupCollection : ReadonlyCollection<BlueprintMarketGroup>
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="src"></param>
        internal BlueprintMarketGroupCollection(BlueprintMarketGroup group, SerializableBlueprintGroup[] src)
            : base(src == null ? 0 : src.Length)
        {
            if (src == null)
                return;

            foreach (SerializableBlueprintGroup subGroup in src)
            {
                m_items.Add(new BlueprintMarketGroup(group, subGroup));
            }
        }
    }
}
