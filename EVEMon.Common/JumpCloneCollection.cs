using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class JumpCloneCollection : ReadonlyCollection<JumpClone>
    {
        private readonly Character m_character;


        #region Constructor

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="character">The character.</param>
        internal JumpCloneCollection(Character character)
        {
            m_character = character;
        }

        #endregion


        #region Import & Export Methods

        /// <summary>
        /// Imports an enumeration of API objects.
        /// </summary>
        /// <param name="src">The enumeration of serializable objects from the API.</param>
        internal void Import(IEnumerable<SerializableCharacterJumpClone> src)
        {
            Items.Clear();

            // Import the jump clones from the API
            foreach (SerializableCharacterJumpClone srcJumpClone in src)
            {
                Items.Add(new JumpClone(srcJumpClone));
            }
        }

        /// <summary>
        /// Imports an enumeration of serialization objects.
        /// </summary>
        /// <param name="src">The enumeration of serializable objects from the settings file.</param>
        //internal void Import(IEnumerable<SerializableJumpClone> src)
        //{
        //    Items.Clear();

        //    foreach (SerializableCharacterJumpClone srcJumpClone in src)
        //    {
        //        Items.Add(new JumpClone(srcJumpClone));
        //    }
        //}

        /// <summary>
        /// Exports the serialization object for the settings file.
        /// </summary>
        /// <returns>List of serializable jump clones.</returns>
        internal IEnumerable<SerializableCharacterJumpClone> Export()
        {
            return Items.Select(jumpClone => jumpClone.Export());
        }

        #endregion
    }
}
