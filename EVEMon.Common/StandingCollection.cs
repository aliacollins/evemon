using System.Collections.Generic;
using System.Linq;
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
            Items.Clear();

            // Import the standings from the API
            foreach (SerializableStandingsListItem srcStanding in src)
            {
                Items.Add(new Standing(m_character, srcStanding));
            }

            // Fires the event regarding standings update
            EveMonClient.OnCharacterStandingsUpdated(m_character);
        }

        /// <summary>
        /// Imports an enumeration of serialization objects.
        /// </summary>
        /// <param name="src">The enumeration of serializable standings from the API.</param>
        internal void Import(IEnumerable<SerializableStanding> src)
        {
            Items.Clear();

            foreach (SerializableStanding srcStanding in src)
            {
                Items.Add(new Standing(m_character, srcStanding));
            }
        }

        /// <summary>
        /// Exports the standings to a serialization object for the settings file.
        /// </summary>
        /// <returns>List of serializable research points.</returns>
        internal List<SerializableStanding> Export()
        {
            List<SerializableStanding> serial = new List<SerializableStanding>(Items.Count);
            serial.AddRange(Items.Select(standing => standing.Export()));

            return serial;
        }
    }
}