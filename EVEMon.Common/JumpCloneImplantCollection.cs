using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class JumpCloneImplantCollection : ReadonlyCollection<JumpCloneImplant>
    {
        private readonly Character m_character;


        #region Constructor

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="character">The character.</param>
        internal JumpCloneImplantCollection(Character character)
        {
            m_character = character;
        }

        #endregion


        #region Import & Export Methods

        /// <summary>
        /// Imports an enumeration of API objects.
        /// </summary>
        /// <param name="src">The enumeration of serializable objects from the API.</param>
        internal void Import(IEnumerable<SerializableCharacterJumpCloneImplant> src)
        {
            Items.Clear();

            // Import the jump clone implants from the API
            foreach (SerializableCharacterJumpCloneImplant srcJumpCloneImplant in src)
            {
                Items.Add(new JumpCloneImplant(srcJumpCloneImplant));
            }
        }

        /// <summary>
        /// Imports an enumeration of serialization objects.
        /// </summary>
        /// <param name="src">The enumeration of serializable objects from the settings file.</param>
        //internal void Import(IEnumerable<SerializableJumpCloneImplant> src)
        //{
        //    Items.Clear();

        //    foreach (SerializableJumpCloneImplant srcJumpCloneImplant in src)
        //    {
        //        Items.Add(new JumpCloneImplant(srcJumpClone));
        //    }
        //}

        /// <summary>
        /// Exports the serialization object for the settings file.
        /// </summary>
        /// <returns>List of serializable jump clone implants.</returns>
        internal IEnumerable<SerializableCharacterJumpCloneImplant> Export()
        {
            return Items.Select(jumpCloneImplant => jumpCloneImplant.Export());
        }

        #endregion

    }
}
