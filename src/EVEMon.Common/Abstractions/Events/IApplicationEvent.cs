using System;

namespace EVEMon.Common.Abstractions.Events
{
    /// <summary>
    /// Marker interface for all application events.
    /// All event types used with <see cref="IEventBroker"/> must implement this interface.
    /// </summary>
    /// <remarks>
    /// This is part of the MVVM abstraction layer that enables:
    /// - Framework-agnostic event handling
    /// - Decoupling from EveMonClient's static events
    /// - Testability through mocking
    /// - Future cross-platform UI support (Avalonia)
    /// </remarks>
    public interface IApplicationEvent
    {
        /// <summary>
        /// Gets the UTC timestamp when this event was created.
        /// </summary>
        DateTime Timestamp { get; }
    }

    /// <summary>
    /// Base record for application events providing common functionality.
    /// </summary>
    public abstract record ApplicationEventBase : IApplicationEvent
    {
        /// <inheritdoc />
        public DateTime Timestamp { get; } = DateTime.UtcNow;
    }
}
