using System;
using System.Collections.Generic;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common.CustomEventArgs
{
    public sealed class APIKeyCreationEventArgs : EventArgs
    {
        public APIKeyCreationEventArgs(long id, string verificationCode,
                                       APIResult<SerializableAPIKeyInfo> apiKeyInfo)
        {
            ID = id;
            VerificationCode = verificationCode;
            KeyTestError = String.Empty;
            APIKeyInfo = apiKeyInfo;
            Identities = new List<CharacterIdentity>();

            // Determine the API key type
            Type = APIKey.GetCredentialsType(apiKeyInfo);

            // On error, retrieve the error message and quit
            if (Type == APIKeyType.Unknown)
            {
                KeyTestError = apiKeyInfo.ErrorMessage;
                CCPError = apiKeyInfo.CCPError ?? new APICCPError();
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
                    identity.Name = character.Name;
                else
                {
                    // Create an identity if necessary
                    identity = EveMonClient.CharacterIdentities.Add(character.ID, character.Name);
                }

                Identities.Add(identity);
            }
        }

        public long ID { get; set; }

        public string VerificationCode { get; set; }

        public long AccessMask { get; private set; }

        public APIKeyType Type { get; set; }

        public DateTime Expiration { get; private set; }

        public string KeyTestError { get; set; }

        public APICCPError CCPError { get; set; }

        /// <summary>
        /// Gets the result which occurred when the API key info was queried.
        /// </summary>
        public APIResult<SerializableAPIKeyInfo> APIKeyInfo { get; private set; }

        /// <summary>
        /// Gets the list of identities available from this API key.
        /// </summary>
        public List<CharacterIdentity> Identities { get; private set; }

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
    }
}
