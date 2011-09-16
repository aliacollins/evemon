using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Attributes;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents the API providers defined by the user.
    /// </summary>
    [EnforceUIThreadAffinity]
    public sealed class GlobalAPIProviderCollection : ReadonlyVirtualCollection<APIProvider>
    {
        private readonly List<APIProvider> m_customProviders = new List<APIProvider>();
        private APIProvider m_currentProvider;

        /// <summary>
        /// Private constructor, only the mother class can instantiate it.
        /// </summary>
        internal GlobalAPIProviderCollection()
        {
            CurrentProvider = DefaultProvider;
        }


        #region Public properties and methods

        /// <summary>
        /// Gets the default provider.
        /// </summary>
        public static APIProvider DefaultProvider
        {
            get { return APIProvider.DefaultProvider; }
        }

        /// <summary>
        /// Gets the default provider.
        /// </summary>
        public static APIProvider TestProvider
        {
            get { return APIProvider.TestProvider; }
        }

        /// <summary>
        /// Gets an enumeration over the used-defined providers.
        /// </summary>
        public IEnumerable<APIProvider> CustomProviders
        {
            get { return m_customProviders.AsReadOnly(); }
        }

        /// <summary>
        /// Gets the used provider.
        /// </summary>
        /// <exception cref="InvalidOperationException">The given provider is not in the list</exception>
        public APIProvider CurrentProvider
        {
            get { return m_currentProvider; }
            set
            {
                // Is it a custom provider stored in this collection ?
                if (m_customProviders.Contains(value))
                    m_currentProvider = value;
                    // Is it the default provider ?
                else if (APIProvider.DefaultProvider == value)
                    m_currentProvider = value;
                    // is it the test provider
                else if (APIProvider.TestProvider == value)
                    m_currentProvider = value;
                    // Then it's a non-register provider, we messed up since it should be in this global collection
                else
                    throw new InvalidOperationException("The given provider is not in the list");
            }
        }

        /// <summary>
        /// Gets an API provider by its name, returning null when not found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The wanted API provider when found; null otherwise.</returns>
        private APIProvider this[string name]
        {
            get
            {
                // Is it the default provider ? If not look among custom providers
                return DefaultProvider.Name == name
                           ? DefaultProvider
                           : m_customProviders.FirstOrDefault(provider => provider.Name == name);
            }
        }

        #endregion


        #region Importation/exportation and other internals

        /// <summary>
        /// Update the providers with the provided serialization object.
        /// </summary>
        /// <param name="serial"></param>
        internal void Import(SerializableAPIProviders serial)
        {
            m_customProviders.Clear();
            CurrentProvider = DefaultProvider;

            // Providers
            foreach (SerializableAPIProvider sProvider in serial.CustomProviders)
            {
                APIProvider provider = new APIProvider { Name = sProvider.Name, Url = sProvider.Url };

                // Providers' methods
                foreach (SerializableAPIMethod sMethod in sProvider.Methods)
                {
                    APIMethod method = provider.GetMethod(sMethod.Method);
                    if (method != null)
                        method.Path = sMethod.Path;
                }

                // Add this provider to our inner list
                m_customProviders.Add(provider);
            }

            // Current provider
            APIProvider newCurrentProvider = this[serial.CurrentProviderName];

            if (newCurrentProvider != null)
                CurrentProvider = newCurrentProvider;

            if (serial.CurrentProviderName == TestProvider.Name)
                CurrentProvider = TestProvider;
        }

        /// <summary>
        /// Exports the providers to a serialization object.
        /// </summary>
        /// <returns></returns>
        internal SerializableAPIProviders Export()
        {
            SerializableAPIProviders serial = new SerializableAPIProviders { CurrentProviderName = CurrentProvider.Name };

            // Providers
            foreach (APIProvider provider in CustomProviders)
            {
                SerializableAPIProvider serialProvider = new SerializableAPIProvider { Name = provider.Name, Url = provider.Url };
                serial.CustomProviders.Add(serialProvider);

                // Methods
                serialProvider.Methods.Clear();
                foreach (APIMethod method in provider.Methods.Where(
                    method => !String.IsNullOrWhiteSpace(method.Path)).Where(
                        method => method.Method != APIMethods.CorporationMarketOrders &&
                                  method.Method != APIMethods.CorporationIndustryJobs)
                    )
                {
                    serialProvider.Methods.Add(new SerializableAPIMethod { Method = method.Method, Path = method.Path });
                }
            }

            return serial;
        }

        #endregion


        /// <summary>
        /// Core method to implement for collection services.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<APIProvider> Enumerate()
        {
            yield return APIProvider.DefaultProvider;

            foreach (APIProvider provider in m_customProviders)
            {
                yield return provider;
            }
        }
    }
}