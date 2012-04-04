using System;
using System.ComponentModel;
using System.Xml.Serialization;
using EVEMon.Common.Attributes;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.Common
{


    #region Flag Enumerations

    /// <summary>
    /// Describes the kind of changes which occurred.
    /// </summary>
    [Flags]
    public enum PlanChange
    {
        None = 0,
        Notification = 1,
        Prerequisites = 2,
        All = Notification | Prerequisites
    }

    /// <summary>
    /// Flags for the races.
    /// </summary>
    [Flags]
    public enum Race
    {
        [XmlEnum("Caldari")]
        Caldari = 1,

        [XmlEnum("Minmatar")]
        Minmatar = 2,

        [XmlEnum("Amarr")]
        Amarr = 4,

        [XmlEnum("Gallente")]
        Gallente = 8,

        [XmlEnum("Jove")]
        Jove = 16,

        [XmlEnum("Faction")]
        Faction = 32,

        [XmlEnum("ORE")]
        Ore = 64,

        None = 0,
        All = Amarr | Minmatar | Caldari | Gallente | Faction | Jove | Ore
    }

    /// <summary>
    /// Represents the metagroup of an item
    /// </summary>
    [Flags]
    public enum ItemMetaGroup
    {
        T1 = 2,
        T2 = 4,
        T3 = 8,
        Faction = 0x10,
        Officer = 0x20,
        Deadspace = 0x40,
        Storyline = 0x80,

        // Obsolete enumerations, can be safely removed
        // after version 1.3.4
        Named = 257,
        Other = 513,

        None = 0,
        AllTechLevel = T1 | T2 | T3,
        AllNonTechLevel = Faction | Officer | Deadspace | Storyline,
        All = AllTechLevel | AllNonTechLevel
    }

    /// <summary>
    /// Flags for the items slots
    /// </summary>
    [Flags]
    public enum ItemSlot
    {
        None = 0,
        NoSlot = 1,
        Low = 2,
        Medium = 4,
        High = 8,

        All = Low | Medium | High | NoSlot
    }

    /// <summary>
    /// Flags options for the text representation format of a skill
    /// </summary>
    [Flags]
    public enum DescriptiveTextOptions
    {
        None = 0,
        FullText = 1,
        UppercaseText = 2,
        SpaceText = 4,
        IncludeCommas = 8,
        IncludeZeroes = 16,
        SpaceBetween = 32,
        FirstLetterUppercase = 64
    }

    /// <summary>
    /// Represents the options one can use with <see cref="CharacterScratchpad.SetSkillLevel"/>. Those are only optimizations
    /// </summary>
    [Flags]
    public enum LearningOptions
    {
        /// <summary>
        /// None, regular learning.
        /// </summary>
        None = 0,

        /// <summary>
        /// Do not update the total SP count.
        /// </summary>
        FreezeSP = 1,

        /// <summary>
        /// Do not update the training time and the trained skills enumeration.
        /// </summary>
        IgnoreTraining = 2,

        /// <summary>
        /// Assume the prerequisites are already known.
        /// </summary>
        IgnorePrereqs = 4,

        /// <summary>
        /// Ignore the changes when the given target level is lower than the current one
        /// </summary>
        UpgradeOnly = 8
    }

    /// <summary>
    /// Represents the image size of an EVE icon.
    /// </summary>
    [Flags]
    public enum EveImageSize
    {
        None = 0,
        x0 = 1,
        x16 = 16,
        x32 = 32,
        x64 = 64,
        x128 = 128,
        x256 = 256
    }

    /// <summary>
    /// Enumerations to support APIMethods.
    /// </summary>
    [Flags]
    public enum APIMethodsExtensions
    {
        None = 0,

        /// <summary>
        /// The basic character features of APIMethods.
        /// </summary>
        BasicCharacterFeatures =
            APICharacterMethods.CharacterSheet | APICharacterMethods.CharacterInfo | APICharacterMethods.SkillQueue |
            APICharacterMethods.SkillInTraining,

        /// <summary>
        /// The advanced character features of APIMethods.
        /// </summary>
        AdvancedCharacterFeatures =
            APICharacterMethods.AccountStatus | APICharacterMethods.MarketOrders | APICharacterMethods.Contracts |
            APICharacterMethods.IndustryJobs | APICharacterMethods.ResearchPoints | APICharacterMethods.Standings |
            APICharacterMethods.MailMessages | APICharacterMethods.MailBodies | APICharacterMethods.MailingLists |
            APICharacterMethods.Notifications | APICharacterMethods.NotificationTexts,

        /// <summary>
        /// The advanced corporation features of APIMethods.
        /// </summary>
        AdvancedCorporationFeatures = APICorporationMethods.CorporationMarketOrders | APICorporationMethods.CorporationContracts
                                      | APICorporationMethods.CorporationIndustryJobs,

        /// <summary>
        /// All character features of APIMethods.
        /// </summary>
        AllCharacterFeatures = BasicCharacterFeatures | AdvancedCharacterFeatures,
    }

    /// <summary>
    /// Enumeration of the character related API methods. Each method has an access mask.
    /// Each method should have an entry in APIMethods and
    /// an equivalent string entry in NetworkConstants indicating the default path of the method.
    /// </summary>
    [Flags]
    public enum APICharacterMethods
    {
        None = 0,

        /// <summary>
        /// A character sheet (bio, skills, implants, etc).
        /// </summary>
        [Header("Character Sheet")]
        [Description("A character's sheet listing biography, skills, attributes and implants informations.")]
        [Update(UpdatePeriod.Hours1, UpdatePeriod.Hours1, CacheStyle.Short)]
        CharacterSheet = 1 << 3,

        /// <summary>
        /// A character's skill queue.
        /// </summary>
        [Header("Skill Queue")]
        [Description("A character's skill queue.")]
        [Update(UpdatePeriod.Hours1, UpdatePeriod.Hours1, CacheStyle.Short)]
        SkillQueue = 1 << 18,

        /// <summary>
        /// A character's standings towards NPC's.
        /// </summary>
        [Header("NPC Standings")]
        [Description("A character's NPC standings.")]
        [Update(UpdatePeriod.Hours3, UpdatePeriod.Hours3, CacheStyle.Short)]
        Standings = 1 << 19,

        /// <summary>
        /// The account status. Used to retreive account create and expiration date.
        /// </summary>
        [Header("Account Status")]
        [Description("The status of an account.")]
        [Update(UpdatePeriod.Day, UpdatePeriod.Hours1, CacheStyle.Short)]
        [ForcedOnStartup]
        AccountStatus = 1 << 25,

        /// <summary>
        /// The personal issued market orders of a character.
        /// </summary>
        [Header("Market Orders")]
        [Description("The market orders of a character.")]
        [Update(UpdatePeriod.Hours1, UpdatePeriod.Hours1, CacheStyle.Long)]
        MarketOrders = 1 << 12,

        /// <summary>
        /// The personal issued contracts of a character.
        /// </summary>
        [Header("Contracts")]
        [Description("The contracts of a character.")]
        [Update(UpdatePeriod.Minutes15, UpdatePeriod.Minutes15, CacheStyle.Short)]
        Contracts = 1 << 26,

        /// <summary>
        /// The personal issued industry jobs of a character.
        /// </summary>
        [Header("Industry Jobs")]
        [Description("The industry jobs of a character.")]
        [Update(UpdatePeriod.Minutes15, UpdatePeriod.Minutes15, CacheStyle.Short)]
        IndustryJobs = 1 << 7,

        /// <summary>
        /// The research points of a character.
        /// </summary>
        [Header("Research Points")]
        [Description("Research Points for a character.")]
        [Update(UpdatePeriod.Minutes15, UpdatePeriod.Minutes15, CacheStyle.Short)]
        ResearchPoints = 1 << 16,

        /// <summary>
        /// Mail messages for a character.
        /// </summary>
        [Header("EVE Mail Messages")]
        [Description("Mail messages for a character.")]
        [Update(UpdatePeriod.Minutes30, UpdatePeriod.Minutes30, CacheStyle.Long)]
        MailMessages = 1 << 11,

        /// <summary>
        /// Notifications for a character.
        /// </summary>
        [Header("EVE Notifications")]
        [Description("Notifications messages for a character.")]
        [Update(UpdatePeriod.Minutes30, UpdatePeriod.Minutes30, CacheStyle.Long)]
        Notifications = 1 << 14,

        /// <summary>
        /// The skill in training of a character. Used to determine if an account type API key has a character in training.
        /// </summary>
        SkillInTraining = 1 << 17,

        /// <summary>
        /// A character's wallet balance.
        /// </summary>
        AccountBalance = 1 << 0,

        /// <summary>
        /// The character mailing lists. Used to convert mailing list IDs to Names.
        /// </summary>
        MailingLists = 1 << 10,

        /// <summary>
        /// The body text of an EVE mail message.
        /// </summary>
        MailBodies = 1 << 9,

        /// <summary>
        /// The body text of an EVE notification.
        /// </summary>
        NotificationTexts = 1 << 15,

        /// <summary>
        /// The character info. Used to fetch active ship, security status and last known location. 
        /// </summary>
        CharacterInfo = 1 << 23 | 1 << 24,

        /// <summary>
        /// Asset list of a character.
        /// </summary>
        AssetList = 1 << 1,

        /// <summary>
        /// Tha attendees to a character's calendar event.
        /// </summary>
        CalendarEventAttendees = 1 << 2,

        /// <summary>
        /// The contact list of a character.
        /// </summary>
        ContactList = 1 << 4,

        /// <summary>
        /// Contact notifications for a character.
        /// </summary>
        ContactNotifications = 1 << 5,

        /// <summary>
        /// Factional warfare statistics for a character.
        /// </summary>
        FactionalWarfareStats = 1 << 6,

        /// <summary>
        /// The Kill log for a character (Kill mails).
        /// </summary>
        KillLog = 1 << 8,

        /// <summary>
        /// The medals of a character.
        /// </summary>
        Medals = 1 << 13,

        /// <summary>
        /// The upcoming calendar events for a character.
        /// </summary>
        UpcomingCalendarEvents = 1 << 20,

        /// <summary>
        /// A character's wallet journal.
        /// </summary>
        WalletJournal = 1 << 21,
        
        /// <summary>
        /// A character's wallet transactions.
        /// </summary>
        WalletTransactions = 1 << 22,

        /// <summary>
        /// Allows the fetching of coordinate and name data for items owned by the character.
        /// </summary>
        Locations = 1 << 27
    }

    /// <summary>
    /// Enumeration of the corporation related API methods. Each method has an access mask.
    /// Each method should have an entry in APIMethods and
    /// an equivalent string entry in NetworkConstants indicating the default path of the method.
    /// </summary>
    [Flags]
    public enum APICorporationMethods
    {
        None = 0,

        /// <summary>
        /// The corporation issued market orders.
        /// </summary>
        [Header("Market Orders")]
        [Description("The corporation market orders of a character.")]
        [Update(UpdatePeriod.Hours1, UpdatePeriod.Hours1, CacheStyle.Long)]
        CorporationMarketOrders = 1 << 12,

        /// <summary>
        /// The corporation issued contracts.
        /// </summary>
        [Header("Contracts")]
        [Description("The corporation contracts of a character.")]
        [Update(UpdatePeriod.Minutes15, UpdatePeriod.Minutes15, CacheStyle.Long)]
        CorporationContracts = 1 << 23,

        /// <summary>
        /// The corporation issued industry jobs.
        /// </summary>
        [Header("Industry Jobs")]
        [Description("The corporation industry jobs of a character.")]
        [Update(UpdatePeriod.Minutes15, UpdatePeriod.Minutes15, CacheStyle.Long)]
        CorporationIndustryJobs = 1 << 7,

        /// <summary>
        /// A corporation's wallet balances.
        /// </summary>
        CorporationAccountBalance = 1 << 0,

        /// <summary>
        /// Asset list of a corporation.
        /// </summary>
        CorporationAssetList = 1 << 1,

        /// <summary>
        /// A list of corporation contacts.
        /// </summary>
        CorporationContactList = 1 << 4,

        /// <summary>
        /// The log of the corporation's containers.
        /// </summary>
        CorporationContainerLog = 1 << 5,

        /// <summary>
        /// A corporation sheet.
        /// </summary>
        CorporationSheet = 1 << 3,

        /// <summary>
        /// Factional warfare statistics for a corporation.
        /// </summary>
        CorporationFactionalWarfareStats = 1 << 6,

        /// <summary>
        /// The Kill log for a corporation (Kill mails).
        /// </summary>
        CorporationKillLog = 1 << 8,

        /// <summary>
        /// List of all medals created by the corporation.
        /// </summary>
        CorporationMedals = 1 << 13,

        /// <summary>
        /// List of medals awarded to corporation members.
        /// </summary>
        CorporationMemberMedals = 1 << 2,

        /// <summary>
        /// Member roles and titles.
        /// </summary>
        CorporationMemberSecurity = 1 << 9,

        /// <summary>
        /// Member role and title change log.
        /// </summary>
        CorporationMemberSecurityLog = 1 << 10,

        /// <summary>
        /// Limited Member information.
        /// </summary>
        CorporationMemberTrackingLimited = 1 << 11,

        /// <summary>
        /// Extensive Member information. Time of last logoff, last known location and ship.
        /// </summary>
        CorporationMemberTrackingExtended = 1 << 25,

        /// <summary>
        /// List of all outposts controlled by the corporation.
        /// </summary>
        CorporationOutpostList = 1 << 14,

        /// <summary>
        /// List of all service settings of corporate outposts.
        /// </summary>
        CorporationOutpostServiceDetail = 1 << 15,

        /// <summary>
        /// Shareholders of the corporation.
        /// </summary>
        CorporationShareholders = 1 << 16,

        /// <summary>
        /// NPC Standings towards corporation.
        /// </summary>
        CorporationStandings = 1 << 18,

        /// <summary>
        /// List of all settings of corporate starbases.
        /// </summary>
        CorporationStarbaseDetails = 1 << 17,

        /// <summary>
        /// List of all corporate starbases.
        /// </summary>
        CorporationStarbaseList = 1 << 19,

        /// <summary>
        /// Titles of corporation and the roles they grant.
        /// </summary>
        CorporationTitles = 1 << 22,

        /// <summary>
        /// Wallet journal for all corporate accounts.
        /// </summary>
        CorporationWalletJournal = 1 << 20,

        /// <summary>
        /// Market transactions of all corporate accounts.
        /// </summary>
        CorporationWalletTransactions = 1 << 21,

        /// <summary>
        /// Allows the fetching of coordinate and name data for items owned by the corporation.
        /// </summary>
        CorporationLocations = 1 << 24
    }

    #endregion


    #region Simple Enumerations

    /// <summary>
    /// Enumeration of the generic API methods. Those methods do not have access mask.
    /// Each method should have an entry in APIMethods and
    /// an equivalent string entry in NetworkConstants indicating the default path of the method.
    /// </summary>
    public enum APIGenericMethods
    {
        /// <summary>
        /// The EVE server status.
        /// </summary>
        [Header("EVE Server Status")]
        [Description("The status of the EVE server.")]
        [Update(UpdatePeriod.Minutes5, UpdatePeriod.Never, UpdatePeriod.Hours1, CacheStyle.Short)]
        ServerStatus,

        /// <summary>
        /// The characters available on an API key.
        /// </summary>
        [Header("Characters on API key")]
        [Description("The retrieval of the characters list available by the API key.")]
        [Update(UpdatePeriod.Day, UpdatePeriod.Hours1, CacheStyle.Short)]
        [ForcedOnStartup]
        CharacterList,

        /// <summary>
        /// The info of the provided API key.
        /// </summary>
        /// <remarks>
        /// It also provides the characters list available by the API key.
        /// The update period is bound to the CharacterList's period in Settings.
        /// </remarks>
        [Update(UpdatePeriod.Day, UpdatePeriod.Hours1, CacheStyle.Short)]
        [ForcedOnStartup]
        APIKeyInfo,

        /// <summary>
        /// A list of the API calls that have access mask.
        /// </summary>
        CallList,

        /// <summary>
        /// The conquerable station list.
        /// </summary>
        ConquerableStationList,

        /// <summary>
        /// The character name. Used to convert IDs to Names.
        /// </summary>
        CharacterName,

        /// <summary>
        /// The character id. Used to convert Names to IDs.
        /// </summary>
        CharacterID,

        /// <summary>
        /// The items contained in a character's contract.
        /// </summary>
        ContractItems,

        /// <summary>
        /// The items contained in a corporation's contract.
        /// </summary>
        CorporationContractItems,

        /// <summary>
        /// The bids for a character's auctioned contracts.
        /// </summary>
        ContractBids,

        /// <summary>
        /// The bids for the corporation's auctioned contracts.
        /// </summary>
        CorporationContractBids,

        /// <summary>
        /// List of alliances in EVE.
        /// </summary>
        AllianceList,

        /// <summary>
        /// List of certificates in EVE.
        /// </summary>
        CertificateTree,

        /// <summary>
        /// List of API errors.
        /// </summary>
        ErrorList,

        /// <summary>
        /// Factional warfare statistics for all EVE.
        /// </summary>
        EVEFactionalWarfareStats,

        /// <summary>
        /// Factional warfare top 100 statistics for all EVE.
        /// </summary>
        EVEFactionalWarfareTopStats,

        /// <summary>
        /// List of references of typeIDs.
        /// </summary>
        RefTypes,

        /// <summary>
        /// List of skills in EVE.
        /// </summary>
        SkillTree,

        /// <summary>
        /// The type id. Used to convert typeIDs to typeNames.
        /// </summary>
        TypeName,
    }

    public enum ThrobberState
    {
        Stopped,
        Rotating,
        Strobing
    }

    /// <summary>
    /// The policy to apply when removing obsolete entries from a plan
    /// </summary>
    public enum ObsoleteRemovalPolicy
    {
        None = 0,
        RemoveAll = 1,
        ConfirmedOnly = 2
    }

    public enum PlanSort
    {
        Name = 0,
        Time = 1,
        SkillsCount = 2,
        Description = 3
    }

    public enum PlanFormat
    {
        None = 0,
        Emp = 1,
        Xml = 2,
        Text = 3
    }

    /// <summary>
    /// Describes whether it has already been computed or not
    /// </summary>
    public enum RemappingPointStatus
    {
        NotComputed,
        UpToDate
    }

    /// <summary>
    /// Enumerations of the implants slots. None is -1, other range from 0 to 4, matching the order of the implants ingame
    /// </summary>
    public enum ImplantSlots
    {
        None = -1,
        Perception = 0,
        Memory = 1,
        Willpower = 2,
        Intelligence = 3,
        Charisma = 4,
        Slot6 = 5,
        Slot7 = 6,
        Slot8 = 7,
        Slot9 = 8,
        Slot10 = 9
    }

    /// <summary>
    /// Represents the type of an item.
    /// </summary>
    public enum ItemFamily
    {
        Item = 0,
        Implant = 1,
        StarbaseStructure = 2,
        Drone = 3,
        Ship = 4,
        Bpo = 5
    }

    /// <summary>
    /// Represents a certificate's status from a character's point of view
    /// </summary>
    public enum CertificateStatus
    {
        /// <summary>
        /// The certificate has been granted to this character
        /// </summary>
        Granted,

        /// <summary>
        /// The certificate can be claimed by the char, all prerequisites are met.
        /// </summary>
        Claimable,

        /// <summary>
        /// The certificate is not claimable yet but at least one prerequisite is satisfied
        /// </summary>
        PartiallyTrained,

        /// <summary>
        /// The certificate is not claimable and none of its prerequisites are satisfied
        /// </summary>
        Untrained
    }

    /// <summary>
    /// Represents a certificate grade.
    /// </summary>
    public enum CertificateGrade
    {
        Basic = 0,
        Standard = 1,
        Improved = 2,
        Elite = 3
    }

    /// <summary>
    /// Represents a plan entry sort.
    /// </summary>
    public enum PlanEntrySort
    {
        None,
        Cost,
        Rank,
        Name,
        Priority,
        PlanGroup,
        SPPerHour,
        TrainingTime,
        TrainingTimeNatural,
        PrimaryAttribute,
        SecondaryAttribute,
        SkillGroupDuration,
        PercentCompleted,
        TimeDifference,
        PlanType,
        Notes,
        SkillPointsRequired
    }

    /// <summary>
    /// Represents how much current SP and levels are taken into account for a training time computation.
    /// </summary>
    public enum TrainingOrigin
    {
        /// <summary>
        /// The training starts at level 0 with no SP.
        /// </summary>
        FromScratch,

        /// <summary>
        /// The training starts at the end of the previous level, the skill not being partially trained.
        /// </summary>
        FromPreviousLevel,

        /// <summary>
        /// The training starts from the current SP, including the ones for the partially trained level.
        /// </summary>
        FromCurrent,

        /// <summary>
        /// The training starts at the end of the previous level, or current if this level is already partially trained
        /// </summary>
        FromPreviousLevelOrCurrent
    }

    /// <summary>
    /// Represents the type of a plan operation
    /// </summary>
    public enum PlanOperations
    {
        /// <summary>
        /// None, there is nothing to do.
        /// </summary>
        None,

        /// <summary>
        /// The operation is an addition.
        /// </summary>
        Addition,

        /// <summary>
        /// The operation is a suppression.
        /// </summary>
        Suppression
    }

    /// <summary>
    /// Describes whether this entry is a prerequisite of another entry 
    /// </summary>
    public enum PlanEntryType
    {
        /// <summary>
        /// This entry is a top-level one, no entries depend on it.
        /// </summary>
        Planned,

        /// <summary>
        /// This entry is required by another entry
        /// </summary>
        Prerequisite
    }

    /// <summary>
    /// Represents a server status
    /// </summary>
    public enum ServerStatus
    {
        /// <summary>
        /// The server is offline
        /// </summary>
        Offline,

        /// <summary>
        /// The server is online
        /// </summary>
        Online,

        /// <summary>
        /// The API couldn't be queried or has not been queried yet.
        /// </summary>
        Unknown,

        /// <summary>
        /// The server's status checks have been disabled.
        /// </summary>
        CheckDisabled
    }

    /// <summary>
    /// Represents the status of the Internet connection
    /// </summary>
    public enum ConnectionStatus
    {
        /// <summary>
        /// Everything normal, we're online
        /// </summary>
        Online,

        /// <summary>
        /// The user requested to stay offline after connection failures
        /// </summary>
        Offline,

        /// <summary>
        /// The connection has not been tested yet
        /// </summary>
        Unknown
    }

    public enum APIKeyType
    {
        /// <summary>
        /// The API key type wouldn't be checked because of an error.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// This is an account wide API key.
        /// </summary>
        Account = 1,

        /// <summary>
        /// This is a character wide API key.
        /// </summary>
        Character = 2,

        /// <summary>
        /// This is a corporation wide API key.
        /// </summary>
        Corporation = 3

    }

    /// <summary>
    /// For which the object was issued.
    /// </summary>
    public enum IssuedFor
    {
        None,
        Character,
        Corporation,
        All
    }

    /// <summary>
    /// A blueprint's type.
    /// </summary>
    /// <remarks>The integer value determines the value assigned in <see cref="IndustryJob"/>.</remarks>
    public enum BlueprintType
    {
        Original = 0,
        Copy = 1
    }

    /// <summary>
    /// Enumeration of skill browser filter.
    /// </summary>
    public enum SkillFilter
    {
        None = -1,
        All = 0,
        ByAttributes = 1,
        NoLv5 = 2,
        Known = 3,
        Lv1Ready = 4,
        Unknown = 5,
        UnknownButOwned = 6,
        UnknownButTrainable = 7,
        UnknownAndNotOwned = 8,
        UnknownAndNotTrainable = 9,
        NotPlanned = 10,
        NotPlannedButTrainable = 11,
        PartiallyTrained = 12,
        Planned = 13,
        Trainable = 14,
        TrailAccountFriendly = 15
    }

    /// <summary>
    /// Enumeration of skill browser sorter.
    /// </summary>
    public enum SkillSort
    {
        None = 0,
        TimeToNextLevel = 1,
        TimeToLevel5 = 2,
        Rank = 3,
        SPPerHour = 4
    }

    /// <summary>
    /// Enumeration of browser usability filter.
    /// </summary>
    public enum ObjectUsabilityFilter
    {
        All = 0,
        Usable = 1,
        Unusable = 2
    }

    /// <summary>
    /// Enumeration of blueprint browser activity filter.
    /// </summary>
    public enum ObjectActivityFilter
    {
        Any = 0,
        All = 1,
        Manufacturing = 2,
        Copying = 3,
        ResearchingMaterialProductivity = 4,
        ResearchingTimeProductivity = 5,
        Invention = 6
    }

    /// <summary>
    /// Enumeration of a character's bloodline
    /// </summary>
    public enum Bloodline
    {
        Amarr = 0,
        Ni_Kunni = 1,
        Khanid = 2,
        Deteis = 3,
        Civire = 4,
        Achura = 5,
        Gallente = 6,
        Intaki = 7,
        Jin_Mei = 8,
        Sebiestor = 9,
        Brutor = 10,
        Vherokior = 11
    }

    /// <summary>
    /// Enumeration of a character's ancestry
    /// </summary>
    public enum Ancestry
    {
        Liberal_Holders = 0,
        Wealthy_Commoners = 1,
        Religious_Reclaimers = 2,
        Free_Merchants = 3,
        Border_Runners = 4,
        Navy_Veterans = 5,
        Cyber_Knights = 6,
        Unionists = 7,
        Zealots = 8,
        Merchandisers = 9,
        Scientists = 10,
        Tube_Child = 11,
        Entrepreneurs = 12,
        Mercs = 13,
        Dissenters = 14,
        Inventors = 15,
        Monks = 16,
        Stargazers = 17,
        Activists = 18,
        Miners = 19,
        Immigrants = 20,
        Artists = 21,
        Diplomats = 22,
        Reborn = 23,
        Sang_Do_Caste = 24,
        Saan_Go_Caste = 25,
        Jing_Ko_Caste = 26,
        Tinkerers = 27,
        Traders = 28,
        Rebels = 29,
        Workers = 30,
        Tribal_Traditionalists = 31,
        Slave_Child = 32,
        Drifters = 33,
        Mystics = 34,
        Retailers = 35
    }

    /// <summary>
    /// Enumeration of a character's gender
    /// </summary>
    public enum Gender
    {
        Female = 0,
        Male = 1
    }

    /// <summary>
    /// The abbreviation format status of a market orders value.
    /// </summary>
    public enum AbbreviationFormat
    {
        AbbreviationWords,
        AbbreviationSymbols
    }

    /// <summary>
    /// Enumeration of a standing status
    /// </summary>
    public enum StandingStatus
    {
        Excellent,
        Good,
        Neutral,
        Bad,
        Terrible
    }

    public enum SortOrder
    {
        Ascending = 0,
        Descending = 1
    };

    public enum CharacterSortCriteria
    {
        /// <summary>
        /// Characters are sorted by their names
        /// </summary>
        Name = 0,

        /// <summary>
        /// Characters are sorted by their training completion time or, when not in training, their names.
        /// </summary>
        TrainingCompletion = 1,
    };

    public enum EveImageSizeMode
    {
        Normal,
        AutoSize,
        StretchImage
    };

    public enum PathSearchCriteria
    {
        FewerJumps,
        ShortestDistance
    }

    #endregion


    #region Enumerations with attributes

    public enum QueryStatus
    {
        /// <summary>
        /// The query will be updated on due time.
        /// </summary>
        [Description("Pending...")]
        Pending,

        /// <summary>
        /// The query is being updated.
        /// </summary>
        [Description("Updating...")]
        Updating,

        /// <summary>
        /// The query is disabled.
        /// </summary>
        [Description("Disabled.")]
        Disabled,

        /// <summary>
        /// There is no network connection.
        /// </summary>
        [Description("No TCP/IP connection.")]
        NoNetwork,

        /// <summary>
        /// The character is not on any API key.
        /// </summary>
        [Description("No associated API key.")]
        NoAPIKey,

        /// <summary>
        /// The API key has no access to query the call.
        /// </summary>
        [Description("No access via the API key.")]
        NoAccess
    }

    /// <summary>
    /// Enumeration of the attributes in Eve. None is -1, other range from 0 to 4,
    /// matching the attributes order on the ingame character sheets.
    /// </summary>
    public enum EveAttribute
    {
        [XmlEnum("none")]
        None = -1,

        [XmlEnum("intelligence")]
        Intelligence = 0,

        [XmlEnum("perception")]
        Perception = 1,

        [XmlEnum("charisma")]
        Charisma = 2,

        [XmlEnum("willpower")]
        Willpower = 3,

        [XmlEnum("memory")]
        Memory = 4
    }

    /// <summary>
    /// Represents the activity of a blueprint.
    /// </summary>
    public enum BlueprintActivity
    {
        [Description("None")]
        None = 0,

        [Description("Manufacturing")]
        Manufacturing = 1,

        [Description("Researching Technology")]
        ResearchingTechnology = 2,

        [Description("Time Efficiency Research")]
        ResearchingTimeProductivity = 3,

        [Description("Material Research")]
        ResearchingMaterialProductivity = 4,

        [Description("Copying")]
        Copying = 5,

        [Description("Duplicating")]
        Duplicating = 6,

        [Description("Reverse Engineering")]
        ReverseEngineering = 7,

        [Description("Invention")]
        Invention = 8
    }

    /// <summary>
    /// The contract type.
    /// </summary>
    public enum ContractType
    {
        None,

        [Description("Item Exchange")]
        ItemExchange,

        [Description("Courier")]
        Courier,

        [Description("Loan")]
        Loan,

        [Description("Auction")]
        Auction
    }


    /// <summary>
    /// The contract availability.
    /// </summary>
    public enum ContractAvailability
    {
        None,

        [Description("Public")]
        Public,

        [Description("Private")]
        Private
    }

    /// <summary>
    /// The status of a market order.
    /// </summary>
    /// <remarks>The integer value determines the sort order.</remarks>
    public enum OrderState
    {
        [Header("Active orders")]
        Active = 0,

        [Header("Canceled orders")]
        Canceled = 1,

        [Header("Expired orders")]
        Expired = 2,

        [Header("Fulfilled orders")]
        Fulfilled = 3,

        [Header("Modified orders")]
        Modified = 4
    }

    /// <summary>
    /// The status of a contract.
    /// </summary>
    /// <remarks>The integer value determines the sort order.</remarks>
    public enum ContractState
    {
        [Header("Assigned contracts")]
        Assigned = 0,

        [Header("Created contracts")]
        Created = 1,

        [Header("Canceled contracts")]
        Canceled = 2,

        [Header("Deleted contracts")]
        Deleted = 3,

        [Header("Expired contracts")]
        Expired = 4,

        [Header("Rejected contracts")]
        Rejected = 5,

        [Header("Finished contracts")]
        Finished = 6,

        [Header("Unknown contracts")]
        Unknown = 7
    }

    /// <summary>
    /// The status of a industry job.
    /// </summary>
    /// <remarks>The integer value determines the sort order in "Group by...".</remarks>
    public enum JobState
    {
        [Header("Active jobs")]
        Active = 0,

        [Header("Delivered jobs")]
        Delivered = 1,

        [Header("Canceled jobs")]
        Canceled = 2,

        [Header("Paused jobs")]
        Paused = 3,

        [Header("Failed jobs")]
        Failed = 4
    }

    /// <summary>
    /// The status of an active job.
    /// </summary>
    public enum ActiveJobState
    {
        None,

        [Description("Pending")]
        Pending,

        [Description("In progress")]
        InProgress,

        [Description("Ready")]
        Ready
    }

    /// <summary>
    /// The status of an EVE mail message.
    /// </summary>
    /// <remarks>The integer value determines the sort order in "Group by...".</remarks>
    public enum EVEMailState
    {
        [Header("Inbox")]
        Inbox = 0,

        [Header("Sent Items")]
        SentItem = 1,
    }

    #endregion
}