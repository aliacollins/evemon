using System;
using System.Collections.Generic;
using System.Text;
using EVEMon.Common.Threading;
using EVEMon.Common.Serialization;
using EVEMon.Common.Net;
using System.Xml.Serialization;
using System.Xml;
using EVEMon.Common.Attributes;
using EVEMon.Common.Serialization.Settings;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.API;


namespace EVEMon.Common
{
    /// <summary>
    /// Represents the characters list
    /// </summary>
    [EnforceUIThreadAffinity]
    public sealed class GlobalCharacterIdentityCollection : ReadonlyKeyedCollection<Int64, CharacterIdentity>
    {
        /// <summary>
        /// 
        /// </summary>
        internal GlobalCharacterIdentityCollection()
        {
        }

        /// <summary>
        /// Gets the character identity with the given id, or null if none was created so far.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CharacterIdentity this[long id]
        {
            get { return GetByKey(id); }
        }

        /// <summary>
        /// Creates and stores a new character identity.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        internal CharacterIdentity Add(Int64 id, string name)
        {
            if (m_items.ContainsKey(id))
            {
                throw new ArgumentException("An identity with the same ID already exists.");
            }

            var identity = new CharacterIdentity(id, name);
            m_items[id] = identity;
            return identity;
        }

    }
}
