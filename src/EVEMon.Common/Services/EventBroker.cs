using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EVEMon.Common.Abstractions.Events;

namespace EVEMon.Common.Services
{
    /// <summary>
    /// Thread-safe implementation of <see cref="IEventBroker"/>.
    /// Provides centralized publish/subscribe messaging to replace EveMonClient's static events.
    /// </summary>
    public sealed class EventBroker : IEventBroker, IDisposable
    {
        private readonly ConcurrentDictionary<Type, List<Delegate>> _syncHandlers = new();
        private readonly ConcurrentDictionary<Type, List<Delegate>> _asyncHandlers = new();
        private readonly SynchronizationContext _syncContext;
        private readonly object _lockObject = new();
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of <see cref="EventBroker"/>.
        /// </summary>
        public EventBroker()
        {
            // Capture the current synchronization context (usually UI thread)
            _syncContext = SynchronizationContext.Current;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="EventBroker"/> with a specific synchronization context.
        /// </summary>
        /// <param name="syncContext">The synchronization context to use for dispatching events.</param>
        public EventBroker(SynchronizationContext syncContext)
        {
            _syncContext = syncContext;
        }

        /// <inheritdoc />
        public void Publish<TEvent>(TEvent eventData) where TEvent : IApplicationEvent
        {
            if (_disposed)
                return;

            var eventType = typeof(TEvent);

            // Handle synchronous subscribers
            if (_syncHandlers.TryGetValue(eventType, out var syncHandlers))
            {
                // Take a snapshot to avoid collection modification during iteration
                Delegate[] handlers;
                lock (_lockObject)
                {
                    handlers = syncHandlers.ToArray();
                }

                foreach (var handler in handlers)
                {
                    try
                    {
                        InvokeHandler(() => ((Action<TEvent>)handler)(eventData));
                    }
                    catch (Exception ex)
                    {
                        // Log but don't throw - one handler failure shouldn't stop others
                        System.Diagnostics.Trace.WriteLine($"EventBroker: Handler exception for {eventType.Name}: {ex.Message}");
                    }
                }
            }

            // Handle asynchronous subscribers (fire and forget)
            if (_asyncHandlers.TryGetValue(eventType, out var asyncHandlers))
            {
                Delegate[] handlers;
                lock (_lockObject)
                {
                    handlers = asyncHandlers.ToArray();
                }

                foreach (var handler in handlers)
                {
                    try
                    {
                        var asyncHandler = (Func<TEvent, Task>)handler;
                        // Fire and forget - but log exceptions
                        _ = InvokeAsyncHandler(asyncHandler, eventData);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.WriteLine($"EventBroker: Async handler exception for {eventType.Name}: {ex.Message}");
                    }
                }
            }
        }

        /// <inheritdoc />
        public IDisposable Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IApplicationEvent
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var eventType = typeof(TEvent);
            var handlers = _syncHandlers.GetOrAdd(eventType, _ => new List<Delegate>());

            lock (_lockObject)
            {
                handlers.Add(handler);
            }

            return new Subscription(() =>
            {
                lock (_lockObject)
                {
                    handlers.Remove(handler);
                }
            });
        }

        /// <inheritdoc />
        public IDisposable Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : IApplicationEvent
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var eventType = typeof(TEvent);
            var handlers = _asyncHandlers.GetOrAdd(eventType, _ => new List<Delegate>());

            lock (_lockObject)
            {
                handlers.Add(handler);
            }

            return new Subscription(() =>
            {
                lock (_lockObject)
                {
                    handlers.Remove(handler);
                }
            });
        }

        /// <summary>
        /// Invokes a handler on the appropriate thread.
        /// </summary>
        private void InvokeHandler(Action action)
        {
            if (_syncContext != null && SynchronizationContext.Current != _syncContext)
            {
                // Marshal to the UI thread
                _syncContext.Post(_ => action(), null);
            }
            else
            {
                // Already on the right thread or no context
                action();
            }
        }

        /// <summary>
        /// Invokes an async handler and logs any exceptions.
        /// </summary>
        private async Task InvokeAsyncHandler<TEvent>(Func<TEvent, Task> handler, TEvent eventData)
        {
            try
            {
                await handler(eventData).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"EventBroker: Async handler exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Disposes the event broker and clears all subscriptions.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            lock (_lockObject)
            {
                _syncHandlers.Clear();
                _asyncHandlers.Clear();
            }
        }
    }

    /// <summary>
    /// Represents a subscription that can be disposed to unsubscribe.
    /// </summary>
    public sealed class Subscription : IDisposable
    {
        private Action _unsubscribe;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of <see cref="Subscription"/>.
        /// </summary>
        /// <param name="unsubscribe">The action to invoke when disposing.</param>
        public Subscription(Action unsubscribe)
        {
            _unsubscribe = unsubscribe ?? throw new ArgumentNullException(nameof(unsubscribe));
        }

        /// <summary>
        /// Unsubscribes from the event.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            _unsubscribe?.Invoke();
            _unsubscribe = null;
        }
    }

    /// <summary>
    /// Manages multiple subscriptions and disposes them all at once.
    /// </summary>
    public sealed class CompositeDisposable : IDisposable
    {
        private readonly List<IDisposable> _disposables = new();
        private readonly object _lock = new();
        private bool _disposed;

        /// <summary>
        /// Adds a disposable to the collection.
        /// </summary>
        /// <param name="disposable">The disposable to add.</param>
        public void Add(IDisposable disposable)
        {
            if (disposable == null)
                return;

            lock (_lock)
            {
                if (_disposed)
                {
                    disposable.Dispose();
                    return;
                }

                _disposables.Add(disposable);
            }
        }

        /// <summary>
        /// Disposes all contained disposables.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            lock (_lock)
            {
                _disposed = true;

                foreach (var disposable in _disposables)
                {
                    try
                    {
                        disposable?.Dispose();
                    }
                    catch
                    {
                        // Ignore disposal exceptions
                    }
                }

                _disposables.Clear();
            }
        }
    }
}
