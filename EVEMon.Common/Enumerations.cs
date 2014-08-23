using System;
using System.ComponentModel;
using System.Xml.Serialization;
using EVEMon.Common.Attributes;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.Common
{


    #region Non Attribute Enumerations

    /// <summary>
    /// Enumeration of the throbber state.
    /// </summary>
    public enum ThrobberState
    {
        Stopped,
        Rotating,
        Strobing
    }

    /// <summary>
    /// The policy to apply when removing obsolete entries from a plan.
    /// </summary>
    public enum ObsoleteRemovalPolicy
    {
        None = 0,
        RemoveAll = 1,
        ConfirmedOnly = 2
    }

    /// <summary>
    /// Enumeration of a plan sort.
    /// </summary>
    public enum PlanSort
    {
        Name = 0,
        Time = 1,
        SkillsCount = 2,
        Description = 3
    }

    /// <summary>
    /// Enumeration of a plan exportation format.
    /// </summary>
    public enum PlanFormat
    {
        None = 0,
        Emp = 1,
        Xml = 2,
        Text = 3
    }

    /// <summary>
    /// Describes whether it has already been computed or not.
    /// </summary>
    public enum RemappingPointStatus
    {
        NotComputed,
        UpToDate
    }

    /// <summary>
    /// Enumerations of the implants slots. None is -1, other range from 0 to 4, matching the order of the implants ingame.
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
        Blueprint = 5
    }

    /// <summary>
    /// Represents a certificate grade.
    /// </summary>
    public enum CertificateGrade
    {
        None = 0,
        Basic = 1,
        Standard = 2,
        Improved = 3,
        Advanced = 4,
        Elite = 5
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
    /// Represents the type of a plan operation.
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
    /// Describes whether this entry is a prerequisite of another entry.
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
    /// Represents a server status.
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
    /// Represents the status of the Internet connection.
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

    /// <summary>
    /// Enumeration of API key types.
    /// </summary>
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
        ResearchingMaterialEfficiency = 4,
        ResearchingTimeEfficiency = 5,
        Invention = 6
    }

    /// <summary>
    /// Enumeration of a character's bloodline.
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
    /// Enumeration of a character's ancestry.
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
    /// Enumeration of a character's gender.
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
    /// Enumeration of a standing status.
    /// </summary>
    public enum StandingStatus
    {
        Excellent,
        Good,
        Neutral,
        Bad,
        Terrible
    }

    /// <summary>
    /// Enumeration of sort order.
    /// </summary>
    public enum SortOrder
    {
        Ascending = 0,
        Descending = 1
    };

    /// <summary>
    /// Enumeration of character sort criteria.
    /// </summary>
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

    /// <summary>
    /// Enumeration of EVE image sizemode.
    /// </summary>
    public enum EveImageSizeMode
    {
        Normal,
        AutoSize,
        StretchImage
    };

    /// <summary>
    /// Enumeration of path between solar systems search criteria.
    /// </summary>
    public enum PathSearchCriteria
    {
        FewerJumps,
        ShortestDistance
    }

    /// <summary>
    /// Enumeration of a transaction type.
    /// </summary>
    public enum TransactionType
    {
        Buy,
        Sell
    }

    /// <summary>
    /// Enumeration of data compression.
    /// </summary>
    public enum DataCompression
    {
        None,
        Gzip,
        Deflate
    }

    /// <summary>
    /// Enumeration of an HTTP method.
    /// </summary>
    public enum HttpMethod
    {
        Get,
        Post,
        Postentity,
        Put
    }

    /// <summary>
    /// Enumeration of contact type.
    /// </summary>
    public enum ContactType
    {
        Character,
        Corporation,
        Alliance,
    }

    /// <summary>
    /// Enumeration of Kill log group.
    /// </summary>
    /// <remarks>The integer value determines the sort order.</remarks>
    public enum KillGroup
    {
        Kills = 0,
        Losses = 1
    }

    #endregion


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

        [XmlEnum("Sleepers")]
        Sleepers = 64,

        [XmlEnum("ORE")]
        Ore = 128,

        None = 0,
        All = Amarr | Minmatar | Caldari | Gallente | Jove | Faction | Sleepers | Ore
    }

    /// <summary>
    /// Represents the metagroup of an item.
    /// </summary>
    [Flags]
    public enum ItemMetaGroup
    {
        T1 = 2,
        T2 = 4,
        T3 = 8,
        Faction = 16,
        Officer = 32,
        Deadspace = 64,
        Storyline = 128,

        None = 0,
        AllTechLevel = T1 | T2 | T3,
        AllNonTechLevel = Faction | Officer | Deadspace | Storyline,
        All = AllTechLevel | AllNonTechLevel
    }

    /// <summary>
    /// Flags for the items slots.
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
    /// Flags options for the text representation format of a skill.
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
    /// Represents the options one can use with <see cref="CharacterScratchpad.SetSkillLevel"/>. Those are only optimizations.
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
        BasicCharacterFeatures = APICharacterMethods.CharacterSheet | APICharacterMethods.CharacterInfo |
                                 APICharacterMethods.SkillQueue | APICharacterMethods.SkillInTraining,

        /// <summary>
        /// The advanced character features of APIMethods.
        /// </summary>
        AdvancedCharacterFeatures = APICharacterMethods.AccountStatus | APICharacterMethods.AssetList |
                                    /*APICharacterMethods.CalendarEventAttendees |*/ APICharacterMethods.ContactList |
                                    APICharacterMethods.Contracts | APICharacterMethods.FactionalWarfareStats |
                                    APICharacterMethods.FactionalWarfareStats | APICharacterMethods.IndustryJobs |
                                    APICharacterMethods.KillLog | APICharacterMethods.MailMessages |
                                    APICharacterMethods.MailBodies | APICharacterMethods.MailingLists |
                                    APICharacterMethods.MarketOrders | APICharacterMethods.Medals |
                                    APICharacterMethods.Notifications | APICharacterMethods.NotificationTexts |
                                    APICharacterMethods.ResearchPoints | APICharacterMethods.Standings |
                                    APICharacterMethods.WalletJournal | APICharacterMethods.WalletTransactions /*|
            APICharacterMethods.UpcomingCalendarEvents*/
        ,

        /// <summary>
        /// The advanced corporation features of APIMethods.
        /// </summary>
        AdvancedCorporationFeatures = APICorporationMethods.CorporationContracts | APICorporationMethods.CorporationMedals |
                                      APICorporationMethods.CorporationMarketOrders |
                                      APICorporationMethods.CorporationIndustryJobs,

        /// <summary>
        /// All character features of APIMethods
        /// </summary>
        AllCharacterFeatures = BasicCharacterFeatures | AdvancedCharacterFeatures,
    }

    /// <summary>
    /// Enumeration of the character related API methods. Each method has an access mask.
    /// Each method should have an entry in APIMethods and an equivalent string entry 
    /// in NetworkConstants indicating the default path of the method.
    /// </summary>
    [Flags]
    public enum APICharacterMethods
    {
        None = 0,

        /// <summary>
        /// The account status. Used to retreive account create and expiration date.
        /// </summary>
        [Header("Account Status")]
        [Description("The status of an account.")]
        [Update(UpdatePeriod.Day, UpdatePeriod.Hours1, CacheStyle.Short)]
        [ForcedOnStartup]
        AccountStatus = 1 << 25,

        /// <summary>
        /// A character sheet (bio, skills, implants, etc).
        /// </summary>
        [Header("Character Sheet")]
        [Description("A character's sheet listing biography, skills, attributes and implants informations.")]
        [Update(UpdatePeriod.Hours1, UpdatePeriod.Hours1, CacheStyle.Short)]
        CharacterSheet = 1 << 3,

        /// <summary>
        /// The skill queue of a character.
        /// </summary>
        [Header("Skill Queue")]
        [Description("The skill queue of a character.")]
        [Update(UpdatePeriod.Hours1, UpdatePeriod.Hours1, CacheStyle.Short)]
        SkillQueue = 1 << 18,

        /// <summary>
        /// A character's standings towards NPC's.
        /// </summary>
        [Header("NPC Standings")]
        [Description("The NPC standings of a character.")]
        [Update(UpdatePeriod.Hours3, UpdatePeriod.Hours3, CacheStyle.Short)]
        Standings = 1 << 19,

        /// <summary>
        /// The factional warfare stats of a character.
        /// </summary>
        [Header("Factional Warfare Stats")]
        [Description("The factional warfare stats of a character.")]
        [Update(UpdatePeriod.Hours1, UpdatePeriod.Hours1, CacheStyle.Short)]
        FactionalWarfareStats = 1 << 6,

        /// <summary>
        /// The assets of a character.
        /// </summary>
        [Header("Assets")]
        [Description("The assets of a character.")]
        [Update(UpdatePeriod.Hours6, UpdatePeriod.Hours6, CacheStyle.Long)]
        AssetList = 1 << 1,

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
        [Description("The research points of a character.")]
        [Update(UpdatePeriod.Minutes15, UpdatePeriod.Minutes15, CacheStyle.Short)]
        ResearchPoints = 1 << 16,

        /// <summary>
        /// Mail messages for a character.
        /// </summary>
        [Header("EVE Mail Messages")]
        [Description("The EVE mails of a character.")]
        [Update(UpdatePeriod.Minutes30, UpdatePeriod.Minutes30, CacheStyle.Short)]
        MailMessages = 1 << 11,

        /// <summary>
        /// The EVE notifications of a character.
        /// </summary>
        [Header("EVE Notifications")]
        [Description("The EVE notifications of a character.")]
        [Update(UpdatePeriod.Minutes30, UpdatePeriod.Minutes30, CacheStyle.Short)]
        Notifications = 1 << 14,

        /// <summary>
        /// The wallet journal of a character.
        /// </summary>
        [Header("Wallet Journal")]
        [Description("The wallet journal of a character.")]
        [Update(UpdatePeriod.Minutes30, UpdatePeriod.Minutes30, CacheStyle.Short)]
        WalletJournal = 1 << 21,

        /// <summary>
        /// The wallet transactions of a character.
        /// </summary>
        [Header("Wallet Transactions")]
        [Description("The wallet transactions of a character.")]
        [Update(UpdatePeriod.Minutes30, UpdatePeriod.Minutes30, CacheStyle.Short)]
        WalletTransactions = 1 << 22,

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
        /// Tha attendees to a character's calendar event.
        /// </summary>
        CalendarEventAttendees = 1 << 2,

        /// <summary>
        /// The contact list of a character.
        /// </summary>
        [Header("Contacts")]
        [Description("The contacts of a character.")]
        [Update(UpdatePeriod.Minutes15, UpdatePeriod.Minutes15, CacheStyle.Short)]
        ContactList = 1 << 4,

        /// <summary>
        /// Contact notifications for a character.
        /// </summary>
        ContactNotifications = 1 << 5,

        /// <summary>
        /// The Kill log for a character (Kill mails).
        /// </summary>
        [Header("Combat Log")]
        [Description("The combat log of a character.")]
        [Update(UpdatePeriod.Hours1, UpdatePeriod.Hours1, CacheStyle.Short)]
        KillLog = 1 << 8,

        /// <summary>
        /// The medals of a character.
        /// </summary>
        [Header("Medals")]
        [Description("The medals of a character.")]
        [Update(UpdatePeriod.Hours6, UpdatePeriod.Hours6, CacheStyle.Short)]
        Medals = 1 << 13,

        /// <summary>
        /// The upcoming calendar events for a character.
        /// </summary>
        //[Header("Calendar Events")]
        //[Description("The upcoming calendar events of a character.")]
        //[Update(UpdatePeriod.Minutes30, UpdatePeriod.Minutes30, CacheStyle.Short)]
        UpcomingCalendarEvents = 1 << 20,

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
        [Header("Medals")]
        [Description("The corporation medals of a character.")]
        [Update(UpdatePeriod.Hours6, UpdatePeriod.Hours6, CacheStyle.Short)]
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


    #region Various Attributes Enumerations

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
        /// Used to convert IDs to Names.
        /// </summary>
        CharacterName,

        /// <summary>
        /// Used to convert Names to IDs.
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
        /// List of custom offices for a corporation.
        /// </summary>
        CorporationCustomsOffices,

        /// <summary>
        /// List of alliances in EVE.
        /// </summary>
        AllianceList,

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
        /// Used to convert typeIDs to typeNames.
        /// </summary>
        TypeName,

        /// <summary>
        /// List of solar systems taking part in Factional Warfare and their occupancy.
        /// </summary>
        FactionalWarfareSystems,

        /// <summary>
        /// List of solar sytems with jump gates.
        /// </summary>
        Jumps,

        /// <summary>
        /// List of solar systems with kills within the last hour.
        /// </summary>
        Kills,

        /// <summary>
        /// List of solar systems that are controled by faction or alliance.
        /// </summary>
        Sovereignty,

        /// <summary>
        /// Used to convert Names to IDs.
        /// </summary>
        OwnerID,

        /// <summary>
        /// Used to convert IDs to character affiliation info.
        /// </summary>
        CharacterAffiliation,

        /// <summary>
        /// A list of the planets on which the character has a command center located.
        /// </summary>
        [Header("Planetary Colonies")]
        [Description("The planetary colonies of a character.")]
        [Update(UpdatePeriod.Hours1, UpdatePeriod.Hours1, CacheStyle.Short)]
        PlanetaryColonies,

        /// <summary>
        /// A list of the pins located on the planet of a character.
        /// </summary>
        PlanetaryPins,

        /// <summary>
        /// A list of the routes defined between pins on the planet of a character.
        /// </summary>
        PlanetaryRoutes,

        /// <summary>
        /// The links defined between the pins on the planet of a character.
        /// </summary>
        PlanetaryLinks,

        /// <summary>
        /// The personal issued industry jobs history of a character.
        /// </summary>
        IndustryJobsHistory,

        /// <summary>
        /// The corporation issued industry jobs history.
        /// </summary>
        CorporationIndustryJobsHistory,
    }

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
        ResearchingTimeEfficiency = 3,

        [Description("Material Efficiency Research")]
        ResearchingMaterialEfficiency = 4,

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
    /// Enumeration of medal group.
    /// </summary>
    public enum MedalGroup
    {
        [Description("Corporation")]
        Corporation,

        [Description("Current Corporation")]
        CurrentCorporation,

        [Description("Other Corporation")]
        OtherCorporation,
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

        [Header("Failed contracts")]
        Failed = 7,

        [Header("Unknown contracts")]
        Unknown = 8
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
    /// The status of a planetary pin.
    /// </summary>
    public enum PlanetaryPinState
    {
        None,

        [Description("Extracting")]
        Extracting,

        [Description("In production")]
        Producing,

        [Description("Idle")]
        Idle
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

    /// <summary>
    /// Enumeration of Amarr Militia ranking.
    /// </summary>
    public enum AmarrMilitiaRank
    {
        [Description("Paladin Crusader")]
        PaladinCrusader = 0,

        [Description("Templar Lieutenant")]
        TemplarLieutenant = 1,

        [Description("Cardinal Lieutenant")]
        CardinalLieutenant = 2,

        [Description("Arch Lieutenant")]
        ArchLieutenant = 3,

        [Description("Imperial Major")]
        ImperialMajor = 4,

        [Description("Marshal Commander")]
        MarshalCommander = 5,

        [Description("Imperator Commander")]
        ImperatorCommander = 6,

        [Description("Tribunus Colonel")]
        TribunusColonel = 7,

        [Description("Legatus Commodore")]
        LegatusCommodore = 8,

        [Description("Divine Commodore")]
        DivineCommodore = 9,
    }

    /// <summary>
    /// Enumeration of Caldari Militia ranking.
    /// </summary>
    public enum CaldariMilitiaRank
    {
        [Description("Protectorate Ensign")]
        ProtectorateEnsign = 0,

        [Description("Second Lieutenant")]
        SecondLieutenant = 1,

        [Description("First Lieutenant")]
        FirstLieutenant = 2,

        [Description("Captain")]
        Captain = 3,

        [Description("Major")]
        Major = 4,

        [Description("Lieutenant Colonel")]
        LieutenantColonel = 5,

        [Description("Colonel")]
        Colonel = 6,

        [Description("Major General")]
        MajorGeneral = 7,

        [Description("Lieutenant General")]
        LieutenantGeneral = 8,

        [Description("Brigadier General")]
        BrigadierGeneral = 9,
    }

    /// <summary>
    /// Enumeration of Gallente Militia ranking.
    /// </summary>
    public enum GallenteMilitiaRank
    {
        [Description("Federation Minuteman")]
        FederationMinuteman = 0,

        [Description("Defender Lieutenant")]
        DefenderLieutenant = 1,

        [Description("Guardian Lieutenant")]
        GuardianLieutenant = 2,

        [Description("Lieutenant Sentinel")]
        LieutenantSentinel = 3,

        [Description("Shield Commander")]
        ShieldCommander = 4,

        [Description("Aegis Commander")]
        AegisCommander = 5,

        [Description("Vice Commander")]
        ViceCommander = 6,

        [Description("Major General")]
        MajorGeneral = 7,

        [Description("Lieutenant General")]
        LieutenantGeneral = 8,

        [Description("Luminaire General")]
        LuminaireGeneral = 9,
    }

    /// <summary>
    /// Enumeration of Minmatar Militia ranking.
    /// </summary>
    public enum MinmatarMilitiaRank
    {
        [Description("Nation Warrior")]
        NationWarrior = 0,

        [Description("Spike Lieutenant")]
        SpikeLieutenant = 1,

        [Description("Spear Lieutenant")]
        SpearLieutenant = 2,

        [Description("Venge Captain")]
        VengeCaptain = 3,

        [Description("Lance Commander")]
        LanceCommander = 4,

        [Description("Blade Commander")]
        BladeCommander = 5,

        [Description("Talon Commander")]
        TalonCommander = 6,

        [Description("Voshud Major")]
        VoshudMajor = 7,

        [Description("Matar Colonel")]
        MatarGeneral = 8,

        [Description("Valklear General")]
        ValklearGeneral = 9,
    }

    /// <summary>
    /// Enumeration of standing group.
    /// </summary>
    public enum StandingGroup
    {
        [Description("Agents")]
        Agents,

        [Description("NPC Corporations")]
        NPCCorporations,

        [Description("Factions")]
        Factions
    }

    /// <summary>
    /// Enumeration of contact group.
    /// </summary>
    /// <remarks>The integer value determines the sort order.</remarks>
    public enum ContactGroup
    {
        [Description("Personal")]
        Personal = 0,

        [Description("Corporation")]
        Corporate = 1,

        [Description("Alliance")]
        Alliance = 2,

        [Description("Agents")]
        Agent = 3,
    }

    /// <summary>
    /// Enumeration of Agent type.
    /// </summary>
    public enum AgentType
    {
        [Description]
        NonAgent,

        [Description("Agents")]
        BasicAgent,

        [Description("Tutorial")]
        TutorialAgent,

        [Description("Research")]
        ResearchAgent,

        [Description("CONCORD")]
        CONCORDAgent,

        [Description("Storyline")]
        GenericStorylineMissionAgent,

        [Description("Stolyline")]
        StorylineMissionAgent,

        [Description("Event")]
        EventMissionAgent,

        [Description("Factional Warfare")]
        FactionalWarfareAgent,

        [Description("Epic")]
        EpicArcAgent,

        [Description("Aura")]
        AuraAgent,

        [Description("Career")]
        CareerAgent
    }

    /// <summary>
    /// Enumeration of Kill log fitting and content group.
    /// </summary>
    public enum KillLogFittingContentGroup
    {
        [Description("Unknown")]
        None = 0,

        [Description("High Power Slots")]
        HighSlot = 1,

        [Description("Medium Power Slots")]
        MediumSlot = 2,

        [Description("Low Power Slots")]
        LowSlot = 3,

        [Description("Subsystem Slots")]
        SubsystemSlot = 4,

        [Description("Rig Slots")]
        RigSlot = 5,

        [Description("Drone Bay")]
        DroneBay = 6,

        [Description("Cargo Bay")]
        Cargo = 7,

        [Description("Implants")]
        Implant = 8,

        [Description("Boosters")]
        Booster = 9,

        [Description("Other")]
        Other = 10
    }

    #endregion

}