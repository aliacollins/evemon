using System.Collections.Generic;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class KillLogCollection : ReadonlyCollection<KillLog>
    {
        private readonly CCPCharacter m_ccpCharacter;

        #region Constructor

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="ccpCharacter">The CCP character.</param>
        internal KillLogCollection(CCPCharacter ccpCharacter)
        {
            m_ccpCharacter = ccpCharacter;
        }

        #endregion

        /// <summary>
        /// Imports an enumeration of API objects.
        /// </summary>
        /// <param name="src">The enumeration of serializable kill log from the API.</param>
        internal void Import(IEnumerable<SerializableKillLogListItem> src)
        {
            Items.Clear();

            // Import the kill log from the API
            foreach (SerializableKillLogListItem srcKillLog in src)
            {
                Items.Add(new KillLog(m_ccpCharacter, srcKillLog));
            }
        }
    }
}