using System;
using EVEMon.Common.Attributes;
using EVEMon.Common.Collections;

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
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        /// <param name="corpId">The corp id.</param>
        /// <param name="corpName">Name of the corp.</param>
        /// <returns></returns>
        internal CharacterIdentity Add(long id, string name, long corpId, string corpName)
        {
            if (Items.ContainsKey(id))
                throw new ArgumentException("An identity with the same ID already exists.");

            CharacterIdentity identity = new CharacterIdentity(id, name, corpId, corpName);
            Items[id] = identity;
            return identity;
        }
    }
}