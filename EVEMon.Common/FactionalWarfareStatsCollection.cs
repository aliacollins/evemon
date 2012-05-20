using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class FactionalWarfareStatsCollection : ReadonlyCollection<FactionalWarfareStats>
    {
        private readonly CCPCharacter m_character;


        #region Constructor

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="character">The character.</param>
        internal FactionalWarfareStatsCollection(CCPCharacter character)
        {
            m_character = character;
        }

        #endregion


        #region Importation

        /// <summary>
        /// Imports an API object.
        /// </summary>
        /// <param name="src">The serializable factional warfare stats from the API.</param>
        internal void Import(SerializableAPIFactionalWarfareStats src)
        {
            Items.Clear();

            Items.Add(new FactionalWarfareStats(src));
        }

        #endregion
    }
}
