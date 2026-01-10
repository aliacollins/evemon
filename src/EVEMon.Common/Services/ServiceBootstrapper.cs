using System;
using EVEMon.Common.Abstractions;
using EVEMon.Common.Abstractions.Events;
using EVEMon.Common.Abstractions.Services;

namespace EVEMon.Common.Services
{
    /// <summary>
    /// Bootstrapper for initializing all EVEMon services.
    /// Call <see cref="Initialize"/> after EveMonClient.Initialize() to set up the service layer.
    /// </summary>
    public static class ServiceBootstrapper
    {
        private static bool _initialized;

        /// <summary>
        /// Initializes the service layer including EventBroker, services, and bridges.
        /// </summary>
        /// <remarks>
        /// This method should be called after EveMonClient.Initialize() completes
        /// but before any UI is shown. The typical call site is in Program.cs
        /// after the splash screen completes loading.
        /// </remarks>
        public static void Initialize()
        {
            if (_initialized)
                return;

            // Create the service locator
            var serviceLocator = new SimpleServiceLocator();

            // Create and register the event broker
            var eventBroker = new EventBroker();
            serviceLocator.Register<IEventBroker>(eventBroker);

            // Register services
            serviceLocator.Register<ICharacterService>(new CharacterService());
            serviceLocator.Register<ISettingsService>(new SettingsService());
            serviceLocator.Register<ITimerService>(new TimerService());

            // Note: IDialogService and INavigationService are UI-specific
            // They will be registered by the UI layer (WinForms or Avalonia)

            // Set the global service locator
            ServiceLocator.Current = serviceLocator;

            // Initialize the bridge to connect legacy events to EventBroker
            EveMonClientBridge.Initialize(eventBroker);

            _initialized = true;

            System.Diagnostics.Trace.WriteLine("ServiceBootstrapper: Services initialized successfully");
        }

        /// <summary>
        /// Registers UI-specific services. Called by the UI layer after Initialize().
        /// </summary>
        /// <param name="dialogService">The dialog service implementation.</param>
        /// <param name="navigationService">The navigation service implementation.</param>
        public static void RegisterUIServices(IDialogService dialogService, INavigationService navigationService)
        {
            if (!_initialized)
                throw new InvalidOperationException("ServiceBootstrapper.Initialize() must be called first.");

            var locator = ServiceLocator.Current as SimpleServiceLocator;
            if (locator == null)
                throw new InvalidOperationException("ServiceLocator is not a SimpleServiceLocator.");

            if (dialogService != null)
                locator.Register(dialogService);

            if (navigationService != null)
                locator.Register(navigationService);
        }

        /// <summary>
        /// Gets whether the service layer has been initialized.
        /// </summary>
        public static bool IsInitialized => _initialized;

        /// <summary>
        /// Shuts down the service layer. Called during application exit.
        /// </summary>
        public static void Shutdown()
        {
            if (!_initialized)
                return;

            // Dispose the event broker
            if (ServiceLocator.IsInitialized)
            {
                var eventBroker = ServiceLocator.TryGetService<IEventBroker>();
                if (eventBroker is IDisposable disposable)
                    disposable.Dispose();

                // Clear all services
                if (ServiceLocator.Current is SimpleServiceLocator locator)
                    locator.Clear();
            }

            _initialized = false;

            System.Diagnostics.Trace.WriteLine("ServiceBootstrapper: Services shut down");
        }
    }
}
