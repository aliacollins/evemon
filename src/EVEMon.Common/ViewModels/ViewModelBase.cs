using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using EVEMon.Common.Abstractions;
using EVEMon.Common.Abstractions.Events;
using EVEMon.Common.Helpers;

namespace EVEMon.Common.ViewModels
{
    /// <summary>
    /// Base class for all ViewModels providing property change notification,
    /// event subscription management, and proper disposal.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        private readonly CompositeDisposable _subscriptions = new();
        private bool _disposed;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Sets a property value and raises PropertyChanged if the value changed.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="field">Reference to the backing field.</param>
        /// <param name="value">The new value.</param>
        /// <param name="propertyName">The property name (auto-filled by compiler).</param>
        /// <returns>True if the value changed, false otherwise.</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises PropertyChanged for multiple properties.
        /// </summary>
        /// <param name="propertyNames">The property names.</param>
        protected void OnPropertiesChanged(params string[] propertyNames)
        {
            foreach (var name in propertyNames)
            {
                OnPropertyChanged(name);
            }
        }

        /// <summary>
        /// Subscribes to an event and automatically manages the subscription lifetime.
        /// The subscription will be disposed when this ViewModel is disposed.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="handler">The event handler.</param>
        protected void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IApplicationEvent
        {
            if (_disposed)
                return;

            var eventBroker = ServiceLocator.TryGetService<IEventBroker>();
            if (eventBroker == null)
            {
                System.Diagnostics.Trace.WriteLine($"ViewModelBase: Cannot subscribe to {typeof(TEvent).Name} - EventBroker not available");
                return;
            }

            var subscription = eventBroker.Subscribe(handler);
            _subscriptions.Add(subscription);
        }

        /// <summary>
        /// Gets whether this ViewModel has been disposed.
        /// </summary>
        protected bool IsDisposed => _disposed;

        /// <summary>
        /// Disposes the ViewModel and all its subscriptions.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes managed and unmanaged resources.
        /// </summary>
        /// <param name="disposing">True if called from Dispose(), false if from finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _subscriptions.Dispose();
                OnDisposing();
            }

            _disposed = true;
        }

        /// <summary>
        /// Called during disposal. Override to perform custom cleanup.
        /// </summary>
        protected virtual void OnDisposing()
        {
        }
    }
}
