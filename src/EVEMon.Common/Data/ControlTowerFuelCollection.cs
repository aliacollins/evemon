using System.Collections.Generic;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    public sealed class ControlTowerFuelCollection : ReadonlyCollection<SerializableControlTowerFuel>
    {     
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlTowerFuelCollection"/> class.
        /// </summary>
        /// <param name="controlTowerFuelInfo">The controlTowerFuelInfo.</param>
        internal ControlTowerFuelCollection(ICollection<SerializableControlTowerFuel> controlTowerFuelInfo)
            : base(controlTowerFuelInfo?.Count ?? 0)
        {
            if (controlTowerFuelInfo == null)
                return;

            foreach (SerializableControlTowerFuel reaction in controlTowerFuelInfo)
            {
                Items.Add(reaction);
            }
        }
    }
}