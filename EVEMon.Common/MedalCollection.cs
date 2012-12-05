using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class MedalCollection : ReadonlyCollection<Medal>
    {
        private readonly CCPCharacter m_character;

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="character">The character.</param>
        internal MedalCollection(CCPCharacter character)
        {
            m_character = character;
        }

        /// <summary>
        /// Imports an enumeration of API objects.
        /// </summary>
        /// <param name="src">The enumeration of serializable medals from the API.</param>
        internal void Import(IEnumerable<SerializableMedalsListItem> src)
        {
            Items.Clear();

            // Import the medals from the API
            foreach (SerializableMedalsListItem srcMedal in src)
            {
                Items.Add(new Medal(srcMedal));
            }

            // Assign the 'number of times awarded'
            foreach (Medal medal in Items.ToList())
            {
                medal.TimesAwarded = Items.Count(x => x.ID == medal.ID);
            }
        }
    }
}
