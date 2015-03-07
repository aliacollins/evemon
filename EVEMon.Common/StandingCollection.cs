using System.Collections.Generic;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class StandingCollection : ReadonlyCollection<Standing>
    {
        private readonly CCPCharacter m_character;

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="character">The character.</param>
        internal StandingCollection(CCPCharacter character)
        {
            m_character = character;
        }

        /// <summary>
        /// Imports an enumeration of API objects.
        /// </summary>
        /// <param name="src">The enumeration of serializable standings from the API.</param>
        internal void Import(IEnumerable<SerializableStandingsListItem> src)
        {
            Items.Clear();

            // Import the standings from the API
            foreach (SerializableStandingsListItem srcStanding in src)
            {
                Items.Add(new Standing(m_character, srcStanding));
            }
        }
    }
}