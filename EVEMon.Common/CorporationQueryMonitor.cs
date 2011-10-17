using System;
using System.Linq;

namespace EVEMon.Common
{
    public sealed class CorporationQueryMonitor<T> : QueryMonitor<T>
    {
        private readonly Character m_character;
        private APIKey m_apiKey;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="method"></param>
        public CorporationQueryMonitor(Character character, Enum method)
            : base(method)
        {
            m_character = character;
        }

        /// <summary>
        /// Gets the required API key information are known.
        /// </summary>
        /// <returns>False if an API key was required and not found.</returns>
        protected override bool HasAPIKey
        {
            get { return m_character.Identity.APIKeys.Any(apiKey => apiKey.Type == APIKeyType.Corporation); }
        }

        /// <summary>
        /// Gets a value indicating whether this monitor has access to data.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this monitor has access; otherwise, <c>false</c>.
        /// </value>
        public override bool HasAccess
        {
            get
            {
                m_apiKey = m_character.Identity.FindAPIKeyWithAccess((APICorporationMethods)Method);
                return m_apiKey != null;
            }
        }

        /// <summary>
        /// Performs the query to the provider, passing the required arguments.
        /// </summary>
        /// <param name="provider">The API provider to use.</param>
        /// <param name="callback">The callback invoked on the UI thread after a result has been queried.</param>
        protected override void QueryAsyncCore(APIProvider provider, QueryCallback<T> callback)
        {
            provider.QueryMethodAsync(Method, m_apiKey.ID, m_apiKey.VerificationCode, m_character.CharacterID, callback);
        }
    }
}