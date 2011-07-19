using System.Collections.Generic;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common
{
    public sealed class StandingCollection : ReadonlyCollection<Standing>
    {
        private readonly CCPCharacter m_character;

        /// <summary>
        /// Internal constructor.
        /// </summary>
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
            m_items.Clear();

            // Import the standings from the API
            foreach (SerializableStandingsListItem srcStanding in src)
            {
                m_items.Add(new Standing(m_character, srcStanding));
            }

            // Fires the event regarding standings update
            EveClient.OnCharacterStandingsUpdated(m_character);
        }

        /// <summary>
        /// Imports an enumeration of serialization objects.
        /// </summary>
        /// <param name="src">The enumeration of serializable standings from the API.</param>
        internal void Import(IEnumerable<SerializableStanding> src)
        {
            m_items.Clear();

            foreach (SerializableStanding srcStanding in src)
            {
                m_items.Add(new Standing(m_character, srcStanding));
            }
        }

        /// <summary>
        /// Exports the standings to a serialization object for the settings file.
        /// </summary>
        /// <returns>List of serializable research points.</returns>
        internal List<SerializableStanding> Export()
        {
            List<SerializableStanding> serial = new List<SerializableStanding>(m_items.Count);

            foreach (Standing standing in m_items)
            {
                serial.Add(standing.Export());
            }

            return serial;
        }
    }
}
