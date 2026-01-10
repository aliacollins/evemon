using System;
using EVEMon.Common.Abstractions.Events;
using EVEMon.Common.Abstractions.Services;

namespace EVEMon.Common.Abstractions
{
    /// <summary>
    /// Service locator interface for dependency resolution.
    /// Provides access to all application services in a framework-agnostic way.
    /// </summary>
    /// <remarks>
    /// While dependency injection via constructor is preferred, this service locator
    /// provides a migration path from the static EveMonClient pattern. ViewModels
    /// can use this to access services without direct coupling to EveMonClient.
    ///
    /// Usage:
    /// <code>
    /// // Get a specific service
    /// var eventBroker = ServiceLocator.Current.GetService&lt;IEventBroker&gt;();
    ///
    /// // Or use the static helper
    /// var characters = ServiceLocator.GetService&lt;ICharacterService&gt;();
    /// </code>
    /// </remarks>
    public interface IServiceLocator
    {
        /// <summary>
        /// Gets a service by type.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <returns>The service instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the service is not registered.</exception>
        TService GetService<TService>() where TService : class;

        /// <summary>
        /// Tries to get a service by type.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <returns>The service instance, or null if not registered.</returns>
        TService TryGetService<TService>() where TService : class;

        /// <summary>
        /// Gets a service by type.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <returns>The service instance.</returns>
        object GetService(Type serviceType);

        /// <summary>
        /// Tries to get a service by type.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <returns>The service instance, or null if not registered.</returns>
        object TryGetService(Type serviceType);
    }

    /// <summary>
    /// Static accessor for the current service locator.
    /// </summary>
    public static class ServiceLocator
    {
        private static IServiceLocator s_current;

        /// <summary>
        /// Gets or sets the current service locator.
        /// </summary>
        public static IServiceLocator Current
        {
            get => s_current ?? throw new InvalidOperationException("ServiceLocator has not been initialized. Call ServiceLocator.Initialize() first.");
            set => s_current = value;
        }

        /// <summary>
        /// Gets whether the service locator has been initialized.
        /// </summary>
        public static bool IsInitialized => s_current != null;

        /// <summary>
        /// Gets a service by type from the current service locator.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <returns>The service instance.</returns>
        public static TService GetService<TService>() where TService : class
            => Current.GetService<TService>();

        /// <summary>
        /// Tries to get a service by type from the current service locator.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <returns>The service instance, or null if not registered.</returns>
        public static TService TryGetService<TService>() where TService : class
            => Current.TryGetService<TService>();

        /// <summary>
        /// Gets the event broker from the current service locator.
        /// </summary>
        public static IEventBroker EventBroker => GetService<IEventBroker>();

        /// <summary>
        /// Gets the character service from the current service locator.
        /// </summary>
        public static ICharacterService Characters => GetService<ICharacterService>();

        /// <summary>
        /// Gets the settings service from the current service locator.
        /// </summary>
        public static ISettingsService Settings => GetService<ISettingsService>();

        /// <summary>
        /// Gets the timer service from the current service locator.
        /// </summary>
        public static ITimerService Timer => GetService<ITimerService>();

        /// <summary>
        /// Gets the dialog service from the current service locator.
        /// </summary>
        public static IDialogService Dialogs => GetService<IDialogService>();

        /// <summary>
        /// Gets the navigation service from the current service locator.
        /// </summary>
        public static INavigationService Navigation => GetService<INavigationService>();
    }
}
