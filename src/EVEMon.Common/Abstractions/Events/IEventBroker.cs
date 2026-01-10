using System;
using System.Threading.Tasks;

namespace EVEMon.Common.Abstractions.Events
{
    /// <summary>
    /// Centralized event broker for publish/subscribe messaging.
    /// Replaces EveMonClient's 74 static events with a testable, mockable interface.
    /// </summary>
    /// <remarks>
    /// Usage:
    /// <code>
    /// // Publishing an event
    /// eventBroker.Publish(new CharacterUpdatedEvent(character));
    ///
    /// // Subscribing to events (returns IDisposable for cleanup)
    /// var subscription = eventBroker.Subscribe&lt;CharacterUpdatedEvent&gt;(e =>
    /// {
    ///     // Handle the event
    /// });
    ///
    /// // Unsubscribe when done
    /// subscription.Dispose();
    /// </code>
    /// </remarks>
    public interface IEventBroker
    {
        /// <summary>
        /// Publishes an event to all subscribers.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="eventData">The event data to publish.</param>
        void Publish<TEvent>(TEvent eventData) where TEvent : IApplicationEvent;

        /// <summary>
        /// Subscribes to events of a specific type.
        /// </summary>
        /// <typeparam name="TEvent">The event type to subscribe to.</typeparam>
        /// <param name="handler">The handler to invoke when the event is published.</param>
        /// <returns>An <see cref="IDisposable"/> that unsubscribes when disposed.</returns>
        IDisposable Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IApplicationEvent;

        /// <summary>
        /// Subscribes to events of a specific type with async handler support.
        /// </summary>
        /// <typeparam name="TEvent">The event type to subscribe to.</typeparam>
        /// <param name="handler">The async handler to invoke when the event is published.</param>
        /// <returns>An <see cref="IDisposable"/> that unsubscribes when disposed.</returns>
        IDisposable Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : IApplicationEvent;
    }
}
