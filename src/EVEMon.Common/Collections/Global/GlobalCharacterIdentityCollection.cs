using System;
using EVEMon.Common.Attributes;
using EVEMon.Common.Models;

namespace EVEMon.Common.Collections.Global
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
        public CharacterIdentity this[long id] => GetByKey(id);

        /// <summary>
        /// Creates and stores a new character identity.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        /// <param name="corpId">The corp id.</param>
        /// <param name="corpName">Name of the corp.</param>
        /// <param name="allianceId">The alliance id.</param>
        /// <param name="allianceName">Name of the alliance.</param>
        /// <param name="factionId">The warfare faction id.</param>
        /// <param name="factionName">Name of the warfare faction.</param>
        /// <returns></returns>
        internal CharacterIdentity Add(long id, string name, long corpId = 0, string corpName = null,
            long allianceId = 0, string allianceName = null, int factionId = 0, string factionName = null)
        {
            if (Items.ContainsKey(id))
                throw new ArgumentException("An identity with the same ID already exists.");

            CharacterIdentity identity = new CharacterIdentity(id, name, corpId, corpName, allianceId, allianceName,
                factionId, factionName);
            Items[id] = identity;
            return identity;
        }
    }
}