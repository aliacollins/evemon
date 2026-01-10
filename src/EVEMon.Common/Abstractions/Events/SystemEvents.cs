using System;
using System.Collections.Generic;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Models;
using EVEMon.Common.Notifications;

namespace EVEMon.Common.Abstractions.Events
{
    #region Timer Events

    /// <summary>
    /// Fired every second. Use for skill countdowns and visible UI updates.
    /// </summary>
    public sealed record SecondTickEvent() : ApplicationEventBase;

    /// <summary>
    /// Fired every 5 seconds. Use for API cache checks and moderate-frequency updates.
    /// </summary>
    public sealed record FiveSecondTickEvent() : ApplicationEventBase;

    /// <summary>
    /// Fired every 30 seconds. Use for background tasks like settings save checks.
    /// </summary>
    public sealed record ThirtySecondTickEvent() : ApplicationEventBase;

    #endregion

    #region ESI/API Key Events

    /// <summary>
    /// Fired when the ESI key collection has changed.
    /// </summary>
    public sealed record ESIKeyCollectionChangedEvent() : ApplicationEventBase;

    /// <summary>
    /// Fired when an ESI key's monitored state has changed.
    /// </summary>
    public sealed record ESIKeyMonitoredChangedEvent() : ApplicationEventBase;

    /// <summary>
    /// Fired when ESI key info has been updated.
    /// </summary>
    public sealed record ESIKeyInfoUpdatedEvent(ESIKey ESIKey) : ApplicationEventBase;

    /// <summary>
    /// Fired when account status has been updated.
    /// </summary>
    public sealed record AccountStatusUpdatedEvent(ESIKey ESIKey) : ApplicationEventBase;

    /// <summary>
    /// Fired when the character list for an ESI key has been updated.
    /// </summary>
    public sealed record CharacterListUpdatedEvent(ESIKey ESIKey) : ApplicationEventBase;

    #endregion

    #region Server Events

    /// <summary>
    /// Fired when server status has been updated.
    /// </summary>
    public sealed record ServerStatusUpdatedEvent(
        EveServer Server,
        ServerStatus PreviousStatus,
        ServerStatus CurrentStatus) : ApplicationEventBase;

    /// <summary>
    /// Fired when the conquerable station list has been updated.
    /// </summary>
    public sealed record ConquerableStationListUpdatedEvent() : ApplicationEventBase;

    /// <summary>
    /// Fired when EVE factional warfare statistics have been updated.
    /// </summary>
    public sealed record EveFactionalWarfareStatsUpdatedEvent() : ApplicationEventBase;

    #endregion

    #region Global Data Events

    /// <summary>
    /// Fired when the EveIDToName list has been updated.
    /// </summary>
    public sealed record EveIDToNameUpdatedEvent() : ApplicationEventBase;

    /// <summary>
    /// Fired when the RefTypes list has been updated.
    /// </summary>
    public sealed record RefTypesUpdatedEvent() : ApplicationEventBase;

    /// <summary>
    /// Fired when the NotificationRefTypes list has been updated.
    /// </summary>
    public sealed record NotificationRefTypesUpdatedEvent() : ApplicationEventBase;

    /// <summary>
    /// Fired when the EveFlags list has been updated.
    /// </summary>
    public sealed record EveFlagsUpdatedEvent() : ApplicationEventBase;

    #endregion

    #region Notification Events

    /// <summary>
    /// Fired when a notification (API errors, skill completed, etc.) is sent.
    /// </summary>
    public sealed record NotificationSentEvent(NotificationEventArgs Notification) : ApplicationEventBase;

    /// <summary>
    /// Fired when a notification is invalidated.
    /// </summary>
    public sealed record NotificationInvalidatedEvent(NotificationInvalidationEventArgs InvalidationArgs) : ApplicationEventBase;

    #endregion

    #region Update Events

    /// <summary>
    /// Fired when an application update is available.
    /// </summary>
    public sealed record UpdateAvailableEvent(
        Uri ForumUrl,
        Uri InstallerUrl,
        string UpdateMessage,
        Version CurrentVersion,
        Version NewestVersion,
        string Md5Sum,
        bool CanAutoInstall,
        string InstallArgs) : ApplicationEventBase;

    /// <summary>
    /// Fired when a data files update is available.
    /// </summary>
    public sealed record DataUpdateAvailableEvent(IReadOnlyCollection<object> ChangedFiles) : ApplicationEventBase;

    #endregion

    #region Loadout Events

    /// <summary>
    /// Fired when a loadout feed has been updated.
    /// </summary>
    public sealed record LoadoutFeedUpdatedEvent(object LoadoutFeed, string ErrorMessage) : ApplicationEventBase;

    /// <summary>
    /// Fired when a loadout has been updated.
    /// </summary>
    public sealed record LoadoutUpdatedEvent(object Loadout, string ErrorMessage) : ApplicationEventBase;

    #endregion

    #region Price Events

    /// <summary>
    /// Fired when item prices have been updated.
    /// </summary>
    public sealed record ItemPricesUpdatedEvent() : ApplicationEventBase;

    #endregion
}
