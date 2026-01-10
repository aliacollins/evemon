using System.Collections.Generic;
using EVEMon.Common.Models;

namespace EVEMon.Common.Abstractions.Events
{
    #region Character Data Events

    /// <summary>
    /// Fired when a character's data has been updated.
    /// </summary>
    public sealed record CharacterUpdatedEvent(Character Character) : ApplicationEventBase;

    /// <summary>
    /// Fired when a character's info has been updated.
    /// </summary>
    public sealed record CharacterInfoUpdatedEvent(Character Character) : ApplicationEventBase;

    /// <summary>
    /// Fired when a character's portrait has been updated.
    /// </summary>
    public sealed record CharacterPortraitUpdatedEvent(Character Character) : ApplicationEventBase;

    /// <summary>
    /// Fired when a character's label has been changed.
    /// </summary>
    public sealed record CharacterLabelChangedEvent(Character Character, IReadOnlyList<string> KnownLabels) : ApplicationEventBase;

    /// <summary>
    /// Fired when a character's skill queue has been updated.
    /// </summary>
    public sealed record CharacterSkillQueueUpdatedEvent(Character Character) : ApplicationEventBase;

    /// <summary>
    /// Fired when multiple characters have been updated within a coalesce window.
    /// </summary>
    public sealed record CharactersBatchUpdatedEvent(IReadOnlyList<Character> Characters) : ApplicationEventBase;

    /// <summary>
    /// Fired when multiple skill queues have been updated within a coalesce window.
    /// </summary>
    public sealed record SkillQueuesBatchUpdatedEvent(IReadOnlyList<Character> Characters) : ApplicationEventBase;

    /// <summary>
    /// Fired when queued skills have been completed.
    /// </summary>
    public sealed record QueuedSkillsCompletedEvent(Character Character, IEnumerable<QueuedSkill> CompletedSkills) : ApplicationEventBase;

    #endregion

    #region Character Collection Events

    /// <summary>
    /// Fired when the character collection has changed.
    /// </summary>
    public sealed record CharacterCollectionChangedEvent() : ApplicationEventBase;

    /// <summary>
    /// Fired when the monitored character collection has changed.
    /// </summary>
    public sealed record MonitoredCharacterCollectionChangedEvent() : ApplicationEventBase;

    /// <summary>
    /// Fired when a character's implant set collection has changed.
    /// </summary>
    public sealed record CharacterImplantSetCollectionChangedEvent(Character Character) : ApplicationEventBase;

    /// <summary>
    /// Fired when a character's plan collection has changed.
    /// </summary>
    public sealed record CharacterPlanCollectionChangedEvent(Character Character) : ApplicationEventBase;

    #endregion

    #region Character Standings & Faction Events

    /// <summary>
    /// Fired when a character's standings have been updated.
    /// </summary>
    public sealed record CharacterStandingsUpdatedEvent(Character Character) : ApplicationEventBase;

    /// <summary>
    /// Fired when a character's factional warfare stats have been updated.
    /// </summary>
    public sealed record CharacterFactionalWarfareStatsUpdatedEvent(Character Character) : ApplicationEventBase;

    #endregion

    #region Character Assets Events

    /// <summary>
    /// Fired when a character's assets have been updated.
    /// </summary>
    public sealed record CharacterAssetsUpdatedEvent(Character Character) : ApplicationEventBase;

    #endregion

    #region Market Order Events

    /// <summary>
    /// Fired when both personal and corporation market orders have been updated.
    /// </summary>
    public sealed record MarketOrdersUpdatedEvent(Character Character) : ApplicationEventBase;

    /// <summary>
    /// Fired when personal market orders have been updated.
    /// </summary>
    public sealed record CharacterMarketOrdersUpdatedEvent(Character Character, IEnumerable<MarketOrder> EndedOrders) : ApplicationEventBase;

    /// <summary>
    /// Fired when corporation market orders have been updated.
    /// </summary>
    public sealed record CorporationMarketOrdersUpdatedEvent(Character Character, IEnumerable<MarketOrder> EndedOrders) : ApplicationEventBase;

    #endregion

    #region Contract Events

    /// <summary>
    /// Fired when both personal and corporation contracts have been updated.
    /// </summary>
    public sealed record ContractsUpdatedEvent(Character Character) : ApplicationEventBase;

    /// <summary>
    /// Fired when personal contracts have been updated.
    /// </summary>
    public sealed record CharacterContractsUpdatedEvent(Character Character, IEnumerable<Contract> EndedContracts) : ApplicationEventBase;

    /// <summary>
    /// Fired when corporation contracts have been updated.
    /// </summary>
    public sealed record CorporationContractsUpdatedEvent(Character Character, IEnumerable<Contract> EndedContracts) : ApplicationEventBase;

    /// <summary>
    /// Fired when personal contract bids have been downloaded.
    /// </summary>
    public sealed record CharacterContractBidsDownloadedEvent(Character Character) : ApplicationEventBase;

    /// <summary>
    /// Fired when corporation contract bids have been downloaded.
    /// </summary>
    public sealed record CorporationContractBidsDownloadedEvent(Character Character) : ApplicationEventBase;

    /// <summary>
    /// Fired when personal contract items have been downloaded.
    /// </summary>
    public sealed record CharacterContractItemsDownloadedEvent(Character Character) : ApplicationEventBase;

    /// <summary>
    /// Fired when corporation contract items have been downloaded.
    /// </summary>
    public sealed record CorporationContractItemsDownloadedEvent(Character Character) : ApplicationEventBase;

    #endregion

    #region Wallet Events

    /// <summary>
    /// Fired when a character's wallet journal has been updated.
    /// </summary>
    public sealed record CharacterWalletJournalUpdatedEvent(Character Character) : ApplicationEventBase;

    /// <summary>
    /// Fired when a character's wallet transactions have been updated.
    /// </summary>
    public sealed record CharacterWalletTransactionsUpdatedEvent(Character Character) : ApplicationEventBase;

    #endregion

    #region Industry Job Events

    /// <summary>
    /// Fired when both personal and corporation industry jobs have been updated.
    /// </summary>
    public sealed record IndustryJobsUpdatedEvent(Character Character) : ApplicationEventBase;

    /// <summary>
    /// Fired when personal industry jobs have been updated.
    /// </summary>
    public sealed record CharacterIndustryJobsUpdatedEvent(Character Character) : ApplicationEventBase;

    /// <summary>
    /// Fired when corporation industry jobs have been updated.
    /// </summary>
    public sealed record CorporationIndustryJobsUpdatedEvent(Character Character) : ApplicationEventBase;

    /// <summary>
    /// Fired when personal industry jobs have been completed.
    /// </summary>
    public sealed record CharacterIndustryJobsCompletedEvent(Character Character, IEnumerable<IndustryJob> CompletedJobs) : ApplicationEventBase;

    /// <summary>
    /// Fired when corporation industry jobs have been completed.
    /// </summary>
    public sealed record CorporationIndustryJobsCompletedEvent(Character Character, IEnumerable<IndustryJob> CompletedJobs) : ApplicationEventBase;

    #endregion

    #region Planetary Events

    /// <summary>
    /// Fired when planetary pins have been completed.
    /// </summary>
    public sealed record CharacterPlanetaryPinsCompletedEvent(Character Character, IEnumerable<PlanetaryPin> CompletedPins) : ApplicationEventBase;

    /// <summary>
    /// Fired when planetary colonies have been updated.
    /// </summary>
    public sealed record CharacterPlanetaryColoniesUpdatedEvent(Character Character) : ApplicationEventBase;

    /// <summary>
    /// Fired when planetary layout has been updated.
    /// </summary>
    public sealed record CharacterPlanetaryLayoutUpdatedEvent(Character Character) : ApplicationEventBase;

    #endregion

    #region Research Events

    /// <summary>
    /// Fired when research points have been updated.
    /// </summary>
    public sealed record CharacterResearchPointsUpdatedEvent(Character Character) : ApplicationEventBase;

    #endregion

    #region Mail Events

    /// <summary>
    /// Fired when EVE mail messages have been updated.
    /// </summary>
    public sealed record CharacterEVEMailMessagesUpdatedEvent(Character Character) : ApplicationEventBase;

    /// <summary>
    /// Fired when EVE mailing lists have been updated.
    /// </summary>
    public sealed record CharacterEVEMailingListsUpdatedEvent(Character Character) : ApplicationEventBase;

    /// <summary>
    /// Fired when an EVE mail body has been downloaded.
    /// </summary>
    public sealed record CharacterEVEMailBodyDownloadedEvent(Character Character) : ApplicationEventBase;

    #endregion

    #region Notification Events

    /// <summary>
    /// Fired when EVE notifications have been updated.
    /// </summary>
    public sealed record CharacterEVENotificationsUpdatedEvent(Character Character) : ApplicationEventBase;

    #endregion

    #region Contact Events

    /// <summary>
    /// Fired when contacts have been updated.
    /// </summary>
    public sealed record CharacterContactsUpdatedEvent(Character Character) : ApplicationEventBase;

    #endregion

    #region Medal Events

    /// <summary>
    /// Fired when character medals have been updated.
    /// </summary>
    public sealed record CharacterMedalsUpdatedEvent(Character Character) : ApplicationEventBase;

    /// <summary>
    /// Fired when corporation medals have been updated.
    /// </summary>
    public sealed record CorporationMedalsUpdatedEvent(Character Character) : ApplicationEventBase;

    #endregion

    #region Calendar Events

    /// <summary>
    /// Fired when upcoming calendar events have been updated.
    /// </summary>
    public sealed record CharacterUpcomingCalendarEventsUpdatedEvent(Character Character) : ApplicationEventBase;

    /// <summary>
    /// Fired when calendar event attendees have been downloaded.
    /// </summary>
    public sealed record CharacterCalendarEventAttendeesDownloadedEvent(Character Character) : ApplicationEventBase;

    #endregion

    #region Kill Log Events

    /// <summary>
    /// Fired when kill log has been updated.
    /// </summary>
    public sealed record CharacterKillLogUpdatedEvent(Character Character) : ApplicationEventBase;

    #endregion

    #region Loyalty Points Events

    /// <summary>
    /// Fired when loyalty point balances have been updated.
    /// </summary>
    public sealed record CharacterLoyaltyPointsUpdatedEvent(Character Character) : ApplicationEventBase;

    #endregion

    #region Plan Events

    /// <summary>
    /// Fired when a plan has changed.
    /// </summary>
    public sealed record PlanChangedEvent(Plan Plan) : ApplicationEventBase;

    /// <summary>
    /// Fired when a plan's name has changed.
    /// </summary>
    public sealed record PlanNameChangedEvent(Plan Plan) : ApplicationEventBase;

    #endregion
}
