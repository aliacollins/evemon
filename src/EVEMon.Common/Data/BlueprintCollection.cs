using System.Collections.Generic;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Represents a read-only collection of blueprints.
    /// </summary>
    public sealed class BlueprintCollection : ReadonlyCollection<Blueprint>
    {
        #region Constructor

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="group">Blueprint Market Group that the blueprint will contain.</param>
        /// <param name="src">One or more serializable blueprints.</param>
        internal BlueprintCollection(BlueprintMarketGroup group, ICollection<SerializableBlueprint> src)
            : base(src?.Count ?? 0)
        {
            if (src == null)
                return;

            foreach (SerializableBlueprint blueprint in src)
            {
                Items.Add(new Blueprint(group, blueprint));
            }
        }

        #endregion
    }
}