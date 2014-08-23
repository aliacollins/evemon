using System.Collections.Generic;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class PlanetaryColoniesCollection : ReadonlyCollection<PlanetaryColony>
    {
        private readonly CCPCharacter m_ccpCharacter;

        #region Constructor

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="ccpCharacter">The CCP character.</param>
        internal PlanetaryColoniesCollection(CCPCharacter ccpCharacter)
        {
            m_ccpCharacter = ccpCharacter;
        }

        #endregion

        #region Importation

        /// <summary>
        /// Imports an enumeration of API objects.
        /// </summary>
        /// <param name="src">The enumeration of serializable kill log from the API.</param>
        internal void Import(IEnumerable<SerializablePlanetaryColony> src)
        {
            Items.Clear();

            // Import the palnetary colony from the API
            foreach (SerializablePlanetaryColony srcColony in src)
            {
                Items.Add(new PlanetaryColony(m_ccpCharacter, srcColony));
            }
        }

        #endregion
    }
}
