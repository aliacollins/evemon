using System.Collections.Generic;
using System.Linq;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents a character's identity, defined by its sole ID. 
    /// Character identities to ensure 
    /// </summary>
    public sealed class CharacterIdentity
    {
        private readonly List<APIKey> m_apiKeys;

        /// <summary>
        /// Constructor from an id and a name.
        /// </summary>
        /// <param name="id">The id for this identity</param>
        /// <param name="name">The name.</param>
        /// <param name="corpId">The corp id.</param>
        /// <param name="corpName">Name of the corp.</param>
        internal CharacterIdentity(long id, string name, long corpId, string corpName)
        {
            CharacterID = id;
            CharacterName = name;
            CorporationID = corpId;
            CorporationName = corpName;

            m_apiKeys = new List<APIKey>();
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
        /// Gets the API keys this identity is associated with.
        /// </summary>
        public List<APIKey> APIKeys
        {
            get { return m_apiKeys; }
        }

        /// <summary>
        /// Gets the character type API keys.
        /// </summary>
        /// <value>The character type API keys.</value>
        public IEnumerable<APIKey> CharacterTypeAPIKeys
        {
            get { return APIKeys.Where(apikey => apikey.Type == APIKeyType.Account || apikey.Type == APIKeyType.Character); }
        }

        /// <summary>
        /// Gets the corporation type API keys.
        /// </summary>
        /// <value>The corporation type API keys.</value>
        public IEnumerable<APIKey> CorporationTypeAPIKeys
        {
            get { return APIKeys.Where(apikey => apikey.Type == APIKeyType.Corporation); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can query character related info.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can query character related info; otherwise, <c>false</c>.
        /// </value>
        public bool CanQueryCharacterRelatedInfo
        {
            get { return !CharacterTypeAPIKeys.IsEmpty(); }
        }


        /// <summary>
        /// Gets a value indicating whether this instance can query corporation related info.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can query corporation related info; otherwise, <c>false</c>.
        /// </value>
        public bool CanQueryCorporationRelatedInfo
        {
            get { return !CorporationTypeAPIKeys.IsEmpty(); }
        }

        /// <summary>
        /// Gets the CCP character representing this identity, or null when there is none.
        /// </summary>
        public CCPCharacter CCPCharacter
        {
            get
            {
                return EveMonClient.Characters.OfType<CCPCharacter>().Where(
                    character => character.CharacterID == CharacterID).FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets the enumeration of the uri characters representing this identity.
        /// </summary>
        public static IEnumerable<UriCharacter> UriCharacters
        {
            get { return EveMonClient.Characters.OfType<UriCharacter>(); }
        }

        /// <summary>
        /// Finds the API key with access to the specified API method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The API key with access to the specified method or null if non found.</returns>
        public APIKey FindAPIKeyWithAccess(APIGenericMethods method)
        {
            return CharacterTypeAPIKeys.FirstOrDefault(apiKey => (int)method == (apiKey.AccessMask & (int)method));
        }

        /// <summary>
        /// Finds the API key with access to the specified API method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The API key with access to the specified method or null if non found.</returns>
        public APIKey FindAPIKeyWithAccess(APICharacterMethods method)
        {
            return CharacterTypeAPIKeys.FirstOrDefault(apiKey => (int)method == (apiKey.AccessMask & (int)method));
        }

        /// <summary>
        /// Finds the API key with access to the specified API method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The API key with access to the specified method or null if non found.</returns>
        public APIKey FindAPIKeyWithAccess(APICorporationMethods method)
        {
            return CorporationTypeAPIKeys.FirstOrDefault(apiKey => (int)method == (apiKey.AccessMask & (int)method));
        }
    }
}