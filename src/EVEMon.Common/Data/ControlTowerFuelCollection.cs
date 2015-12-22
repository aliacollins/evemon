using System.Collections.Generic;
using System.Linq;
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
        internal ControlTowerFuelCollection(IEnumerable<SerializableControlTowerFuel> controlTowerFuelInfo)
            : base(controlTowerFuelInfo == null ? 0 : controlTowerFuelInfo.Count())
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