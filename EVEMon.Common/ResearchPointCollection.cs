using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common
{
    public sealed class ResearchPointCollection : ReadonlyCollection<ResearchPoint>
    {
        private readonly CCPCharacter m_character;

        /// <summary>
        /// Internal constructor.
        /// </summary>
        internal ResearchPointCollection(CCPCharacter character)
        {
            m_character = character;
        }

        /// <summary>
        /// Imports an enumeration of serialization objects.
        /// </summary>
        /// <param name="src"></param>
        internal void Import(IEnumerable<SerializableResearchPoint> src)
        {
            Items.Clear();

            foreach (SerializableResearchPoint srcResearchPoint in src)
            {
                Items.Add(new ResearchPoint(srcResearchPoint));
            }
        }

        /// <summary>
        /// Imports an enumeration of API objects.
        /// </summary>
        /// <param name="src">The enumeration of serializable research points from the API.</param>
        internal void Import(IEnumerable<SerializableResearchListItem> src)
        {
            Items.Clear();

            // Import the research points from the API
            foreach (SerializableResearchListItem srcResearchPoint in src)
            {
                Items.Add(new ResearchPoint(srcResearchPoint));
            }

            // Fires the event regarding research points update
            EveMonClient.OnCharacterResearchPointsUpdated(m_character);
        }

        /// <summary>
        /// Exports the research points to a serialization object for the settings file.
        /// </summary>
        /// <returns>List of serializable research points.</returns>
        internal List<SerializableResearchPoint> Export()
        {
            List<SerializableResearchPoint> serial = new List<SerializableResearchPoint>(Items.Count);
            serial.AddRange(Items.Select(researchPoint => researchPoint.Export()));
            return serial;
        }
    }
}