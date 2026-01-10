namespace EVEMon.Common.Abstractions.Events
{
    /// <summary>
    /// Fired when settings have changed.
    /// </summary>
    public sealed record SettingsChangedEvent() : ApplicationEventBase;

    /// <summary>
    /// Fired when the scheduler has changed.
    /// </summary>
    public sealed record SchedulerChangedEvent() : ApplicationEventBase;
}
