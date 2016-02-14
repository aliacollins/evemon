using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EVEMon.Common.Enumerations.CCPAPI;

namespace EVEMon.Common.Models
{
    /// <summary>
    /// Represents a character's identity, defined by its sole ID. 
    /// Character identities to ensure 
    /// </summary>
    public sealed class CharacterIdentity
    {
        private readonly Collection<APIKey> m_apiKeys;

        /// <summary>
        /// Constructor from an id and a name.
        /// </summary>
        /// <param name="id">The id for this identity</param>
        /// <param name="name">The name.</param>
        /// <param name="corpId">The corp id.</param>
        /// <param name="corpName">Name of the corp.</param>
        /// <param name="allianceId"></param>
        /// <param name="allianceName"></param>
        /// <param name="factionId"></param>
        /// <param name="factionName"></param>
        internal CharacterIdentity(long id, string name, long corpId, string corpName, long allianceId, string allianceName,
            int factionId, string factionName)
        {
            CharacterID = id;
            CharacterName = name;
            CorporationID = corpId;
            CorporationName = corpName;
            AllianceID = allianceId;
            AllianceName = allianceName;
            FactionID = factionId;
            FactionName = factionName;

            m_apiKeys = new Collection<APIKey>();
        }

        /// <summary>
        /// Gets the character ID.
        /// </summary>
        /// <value>The character ID.</value>
        public long CharacterID { get; private set; }

        /// <summary>
        /// Gets the character's name.
        /// </summary>
        /// <value>The name of the character.</value>
        public string CharacterName { get; internal set; }

        /// <summary>
        /// Gets or sets the corporation ID.
        /// </summary>
        /// <value>The corporation ID.</value>
        public long CorporationID { get; set; }

        /// <summary>
        /// Gets or sets the name of the corporation.
        /// </summary>
        /// <value>The name of the corporation.</value>
        public string CorporationName { get; set; }

        /// <summary>
        /// Gets or sets the alliance identifier.
        /// </summary>
        /// <value>
        /// The alliance identifier.
        /// </value>
        public long AllianceID { get; set; }

        /// <summary>
        /// Gets or sets the name of the alliance.
        /// </summary>
        /// <value>
        /// The name of the alliance.
        /// </value>
        public string AllianceName { get; set; }

        /// <summary>
        /// Gets or sets the faction identifier.
        /// </summary>
        /// <value>
        /// The faction identifier.
        /// </value>
        public int FactionID { get; set; }

        /// <summary>
        /// Gets or sets the name of the faction.
        /// </summary>
        /// <value>
        /// The name of the faction.
        /// </value>
        public string FactionName { get; set; }

        /// <summary>
        /// Gets the API keys this identity is associated with.
        /// </summary>
        public Collection<APIKey> APIKeys => m_apiKeys;

        /// <summary>
        /// Gets the character type API keys.
        /// </summary>
        /// <value>The character type API keys.</value>
        public IEnumerable<APIKey> CharacterTypeAPIKeys => APIKeys.Where(apikey => apikey.IsCharacterOrAccountType);

        /// <summary>
        /// Gets the corporation type API keys.
        /// </summary>
        /// <value>The corporation type API keys.</value>
        public IEnumerable<APIKey> CorporationTypeAPIKeys => APIKeys.Where(apikey => apikey.IsCorporationType);

        /// <summary>
        /// Gets a value indicating whether this instance can query character related info.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can query character related info; otherwise, <c>false</c>.
        /// </value>
        public bool CanQueryCharacterInfo => CharacterTypeAPIKeys.Any(apiKey => apiKey.Monitored);


        /// <summary>
        /// Gets a value indicating whether this instance can query corporation related info.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can query corporation related info; otherwise, <c>false</c>.
        /// </value>
        public bool CanQueryCorporationInfo => CorporationTypeAPIKeys.Any(apiKey => apiKey.Monitored);

        /// <summary>
        /// Gets the CCP character representing this identity, or null when there is none.
        /// </summary>
        public CCPCharacter CCPCharacter
            => EveMonClient.Characters.OfType<CCPCharacter>()
                .FirstOrDefault(character => character.CharacterID == CharacterID);

        /// <summary>
        /// Finds the API key with access to the specified API method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The API key with access to the specified method or null if non found.</returns>
        public APIKey FindAPIKeyWithAccess(CCPAPICharacterMethods method)
            => CharacterTypeAPIKeys
                .FirstOrDefault(apiKey => apiKey.Monitored && (int)method == (apiKey.AccessMask & (int)method));

        /// <summary>
        /// Finds the API key with access to the specified API method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The API key with access to the specified method or null if non found.</returns>
        public APIKey FindAPIKeyWithAccess(CCPAPICorporationMethods method)
            => CorporationTypeAPIKeys
                .FirstOrDefault(apiKey => apiKey.Monitored && (int)method == (apiKey.AccessMask & (int)method));
    }
}