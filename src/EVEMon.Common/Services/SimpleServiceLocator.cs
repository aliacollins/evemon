using System;
using System.Collections.Generic;
using EVEMon.Common.Abstractions;

namespace EVEMon.Common.Services
{
    /// <summary>
    /// Simple service locator implementation for EVEMon.
    /// Provides a migration path from static EveMonClient pattern.
    /// </summary>
    public sealed class SimpleServiceLocator : IServiceLocator
    {
        private readonly Dictionary<Type, object> _services = new();
        private readonly Dictionary<Type, Func<object>> _factories = new();
        private readonly object _lock = new();

        /// <summary>
        /// Registers a service instance.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="instance">The service instance.</param>
        public void Register<TService>(TService instance) where TService : class
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            lock (_lock)
            {
                _services[typeof(TService)] = instance;
            }
        }

        /// <summary>
        /// Registers a service factory for lazy instantiation.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="factory">The factory function.</param>
        public void RegisterFactory<TService>(Func<TService> factory) where TService : class
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            lock (_lock)
            {
                _factories[typeof(TService)] = () => factory();
            }
        }

        /// <inheritdoc />
        public TService GetService<TService>() where TService : class
        {
            var service = TryGetService<TService>();
            if (service == null)
                throw new InvalidOperationException($"Service of type {typeof(TService).Name} is not registered.");

            return service;
        }

        /// <inheritdoc />
        public TService TryGetService<TService>() where TService : class
        {
            lock (_lock)
            {
                var serviceType = typeof(TService);

                // Check for registered instance
                if (_services.TryGetValue(serviceType, out var instance))
                    return (TService)instance;

                // Check for factory
                if (_factories.TryGetValue(serviceType, out var factory))
                {
                    var service = (TService)factory();
                    _services[serviceType] = service; // Cache the instance
                    return service;
                }

                return null;
            }
        }

        /// <inheritdoc />
        public object GetService(Type serviceType)
        {
            var service = TryGetService(serviceType);
            if (service == null)
                throw new InvalidOperationException($"Service of type {serviceType.Name} is not registered.");

            return service;
        }

        /// <inheritdoc />
        public object TryGetService(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));

            lock (_lock)
            {
                // Check for registered instance
                if (_services.TryGetValue(serviceType, out var instance))
                    return instance;

                // Check for factory
                if (_factories.TryGetValue(serviceType, out var factory))
                {
                    var service = factory();
                    _services[serviceType] = service; // Cache the instance
                    return service;
                }

                return null;
            }
        }

        /// <summary>
        /// Clears all registered services and factories.
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                // Dispose any disposable services
                foreach (var service in _services.Values)
                {
                    if (service is IDisposable disposable)
                    {
                        try
                        {
                            disposable.Dispose();
                        }
                        catch
                        {
                            // Ignore disposal exceptions
                        }
                    }
                }

                _services.Clear();
                _factories.Clear();
            }
        }
    }
}
