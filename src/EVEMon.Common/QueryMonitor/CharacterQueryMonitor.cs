using System;
using System.Linq;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Extensions;
using EVEMon.Common.Models;
using EVEMon.Common.Serialization.Eve;

namespace EVEMon.Common.QueryMonitor
{
    /// <summary>
    /// Represents a monitor for all queries related to characters and their corporations.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class CharacterQueryMonitor<T> : QueryMonitor<T>
    {
        private readonly Character m_character;
        private APIKey m_apiKey;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="method"></param>
        /// <param name="onUpdated"></param>
        internal CharacterQueryMonitor(Character character, Enum method, Action<CCPAPIResult<T>> onUpdated)
            : base(method, onUpdated)
        {
            m_character = character;
        }

        /// <summary>
        /// Gets the required API key information are known.
        /// </summary>
        /// <returns>False if an API key was required and not found.</returns>
        protected override bool HasAPIKey => m_character.Identity.APIKeys.Any(apiKey => apiKey.IsCharacterOrAccountType);

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
                m_apiKey = m_character.Identity.FindAPIKeyWithAccess((CCPAPICharacterMethods)Method);
                return m_apiKey != null;
            }
        }

        /// <summary>
        /// Performs the query to the provider, passing the required arguments.
        /// </summary>
        /// <param name="provider">The API provider to use.</param>
        /// <param name="callback">The callback invoked on the UI thread after a result has been queried.</param>
        /// <exception cref="System.ArgumentNullException">provider</exception>
        protected override void QueryAsyncCore(APIProvider provider, Action<CCPAPIResult<T>> callback)
        {
            provider.ThrowIfNull(nameof(provider));

            provider.QueryMethodAsync(Method, m_apiKey.ID, m_apiKey.VerificationCode, m_character.CharacterID, callback);
        }
    }
}