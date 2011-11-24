using System;

namespace EVEMon.Common
{
    public sealed class APIKeyQueryMonitor<T> : QueryMonitor<T>
    {
        private readonly APIKey m_apiKey;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="method"></param>
        /// <param name="onUpdated"></param>
        internal APIKeyQueryMonitor(APIKey apiKey, Enum method, QueryCallback<T> onUpdated)
            : base(method, onUpdated)
        {
            m_apiKey = apiKey;
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
                if (Method is APIGenericMethods)
                    return true;

                APICharacterMethods method = (APICharacterMethods)Method;
                return (int)method == (m_apiKey.AccessMask & (int)method);
            }
        }

        /// <summary>
        /// Performs the query to the provider, passing the required arguments.
        /// </summary>
        /// <param name="provider">The API provider to use.</param>
        /// <param name="callback">The callback invoked on the UI thread after a result has been queried.</param>
        protected override void QueryAsyncCore(APIProvider provider, QueryCallback<T> callback)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            provider.QueryMethodAsync(Method, m_apiKey.ID, m_apiKey.VerificationCode, callback);
        }
    }
}