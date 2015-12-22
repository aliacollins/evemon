using System;
using System.Collections.ObjectModel;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Models;
using EVEMon.Common.Serialization.Eve;

namespace EVEMon.Common.CustomEventArgs
{
    public sealed class APIKeyCreationEventArgs : EventArgs
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="APIKeyCreationEventArgs"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="verificationCode">The verification code.</param>
        /// <param name="apiKeyInfo">The API key info.</param>
        public APIKeyCreationEventArgs(long id, string verificationCode,
                                       CCPAPIResult<SerializableAPIKeyInfo> apiKeyInfo)
        {
            if (apiKeyInfo == null)
                throw new ArgumentNullException("apiKeyInfo");

            ID = id;
            VerificationCode = verificationCode;
            KeyTestError = String.Empty;
            APIKeyInfo = apiKeyInfo;
            Identities = new Collection<CharacterIdentity>();

            // Determine the API key type
            Type = APIKey.GetCredentialsType(apiKeyInfo);

            // On error, retrieve the error message and quit
            if (Type == CCPAPIKeyType.Unknown)
            {
                KeyTestError = apiKeyInfo.ErrorMessage;
                CCPError = apiKeyInfo.CCPError ?? new CCPAPIError();
                return;
            }

            AccessMask = apiKeyInfo.Result.Key.AccessMask;
            Expiration = apiKeyInfo.Result.Key.Expiration;

            // Retrieves the characters list
            foreach (SerializableCharacterListItem character in apiKeyInfo.Result.Key.Characters)
            {
                // Look for an existing character ID and update its name
                CharacterIdentity identity = EveMonClient.CharacterIdentities[character.ID];
                if (identity != null)
                {
                    identity.CharacterName = character.Name;
                    identity.CorporationID = character.CorporationID;
                    identity.CorporationName = character.CorporationName;
                    identity.AllianceID = character.AllianceID;
                    identity.AllianceName = character.AllianceName;
                    identity.FactionID = character.FactionID;
                    identity.FactionName = character.FactionName;
                }
                else
                {
                    // Create an identity if necessary
                    identity = EveMonClient.CharacterIdentities.Add(character.ID, character.Name,
                                                                    character.CorporationID, character.CorporationName,
                                                                    character.AllianceID, character.AllianceName,
                                                                    character.FactionID, character.FactionName);
                }

                Identities.Add(identity);
            }
        }


        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public long ID { get; private set; }

        /// <summary>
        /// Gets or sets the verification code.
        /// </summary>
        /// <value>The verification code.</value>
        public string VerificationCode { get; private set; }

        /// <summary>
        /// Gets or sets the access mask.
        /// </summary>
        /// <value>The access mask.</value>
        public long AccessMask { get; private set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public CCPAPIKeyType Type { get; private set; }

        /// <summary>
        /// Gets or sets the expiration.
        /// </summary>
        /// <value>The expiration.</value>
        public DateTime Expiration { get; private set; }

        /// <summary>
        /// Gets or sets the key test error.
        /// </summary>
        /// <value>The key test error.</value>
        public string KeyTestError { get; private set; }

        /// <summary>
        /// Gets or sets the CCP error.
        /// </summary>
        /// <value>The CCP error.</value>
        public CCPAPIError CCPError { get; private set; }

        /// <summary>
        /// Gets the result which occurred when the API key info was queried.
        /// </summary>
        public CCPAPIResult<SerializableAPIKeyInfo> APIKeyInfo { get; private set; }

        /// <summary>
        /// Gets the list of identities available from this API key.
        /// </summary>
        public Collection<CharacterIdentity> Identities { get; private set; }

        #endregion


        #region Methods

        /// <summary>
        /// Creates the or update.
        /// </summary>
        /// <returns></returns>
        public APIKey CreateOrUpdate()
        {
            // Checks whether this API key already exists to update it
            APIKey apiKey = EveMonClient.APIKeys[ID];
            if (apiKey != null)
            {
                apiKey.Update(this);

                // Fires the event regarding the API key info update
                EveMonClient.OnAPIKeyInfoUpdated(apiKey);
            }
            else
            {
                apiKey = new APIKey(ID);
                apiKey.Update(this);
                EveMonClient.APIKeys.Add(apiKey);
            }

            return apiKey;
        }

        #endregion
    }
}
