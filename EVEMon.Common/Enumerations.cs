using System;
using System.ComponentModel;
using System.Xml.Serialization;

using EVEMon.Common.Attributes;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.Common
{

    #region Flag Enumerations

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
        None = 1,
        T1 = 2,
        T2 = 4,
        T3 = 8,
        Named = 0x100,
        Faction = 0x200,
        Officer = 0x400,
        Deadspace = 0x800,
        Storyline = 0x1000,
        Other = 0x2000,

        Empty = 0,
        AllTechLevel = T1 | T2 | T3,
        AllNonTechLevel = Named | Faction | Officer | Deadspace | Storyline | Other,
        All = AllTechLevel | AllNonTechLevel | None
    }

    /// <summary>
    /// Flags for the items slots
    /// </summary>
    [Flags]
    public enum ItemSlot
    {
        None = 1,
        Low = 2,
        Medium = 4,
        High = 8,

        Empty = 0,
        All = Low | Medium | High | None
    }

    /// <summary>
    /// Flags options for the text representation format of a skill
    /// </summary>
    [Flags]
    public enum DescriptiveTextOptions
    {
        Default = 0,
        FullText = 1,
        UppercaseText = 2,
        SpaceText = 4,
        IncludeCommas = 8,
        IncludeZeroes = 16,
        SpaceBetween = 32,
        FirstLetterUppercase = 64
    }

    /// <summary>
    /// Represents the options one can use with <see cref="CharacterSchartchpad.Learn()"/>. Those are only optimizations
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

    #endregion


    #region Simple Enumerations

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
        /// <summary>
        /// 
        /// </summary>
        None,
        /// <summary>
        /// 
        /// </summary>
        Cost,
        /// <summary>
        /// 
        /// </summary>
        Rank,
        /// <summary>
        /// 
        /// </summary>
        Name,
        /// <summary>
        /// 
        /// </summary>
        Priority,
        /// <summary>
        /// 
        /// </summary>
        PlanGroup,
        /// <summary>
        /// 
        /// </summary>
        SPPerHour,
        /// <summary>
        /// 
        /// </summary>
        TrainingTime,
        /// <summary>
        /// 
        /// </summary>
        TrainingTimeNatural,
        /// <summary>
        /// 
        /// </summary>
        PrimaryAttribute,
        /// <summary>
        /// 
        /// </summary>
        SecondaryAttribute,
        /// <summary>
        /// 
        /// </summary>
        SkillGroupDuration,
        /// <summary>
        /// 
        /// </summary>
        PercentCompleted,
        /// <summary>
        /// 
        /// </summary>
        TimeDifference,
        /// <summary>
        /// 
        /// </summary>
        PlanType,
        /// <summary>
        /// 
        /// </summary>
        Notes
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

    /// <summary>
    /// Describes the kind of an API key
    /// </summary>
    public enum CredentialsLevel
    {
        /// <summary>
        /// The account credentials wouldn't be checked because of an error.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// This is a limited API key.
        /// </summary>
        Limited = 1,
        /// <summary>
        /// This is a full API key.
        /// </summary>
        Full = 2
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
    /// <remarks>The integer value determines the value assigned in <see cref="IndustryJob(SerializableAPIJob src)"/>.</remarks>
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
        All = 0,
        NoLv5 = 1,
        Known = 2,
        Lv1Ready = 3,
        Unknown = 4,
        UnknownButOwned = 5,
        UnknownButTrainable = 6,
        UnknownAndNotOwned = 7,
        UnknownAndNotTrainable = 8,
        NotPlanned = 9,
        NotPlannedButTrainable = 10,
        PartiallyTrained = 11,
        Planned = 12,
        Trainable = 13,
        TrailAccountFriendly = 14
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

    #endregion


    #region Enumerations with attributes

    /// <summary>
    /// Enumeration of the attributes in Eve. None is -1, other range from 0 to 4, matching the attributes order on the ingame character sheets.
    /// </summary>
    public enum EveAttribute
    {
        [XmlEnum("perception")]
        Perception = 1,
        [XmlEnum("memory")]
        Memory = 4,
        [XmlEnum("willpower")]
        Willpower = 3,
        [XmlEnum("intelligence")]
        Intelligence = 0,
        [XmlEnum("charisma")]
        Charisma = 2,
        [XmlEnum("none")]
        None = -1
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
    /// Enumeration of the supported API methods. Each method should have an entry in APIMethods and
    /// an equivalent string entry in APIConstants indicating the default path of the method.
    /// </summary>
    public enum APIMethods
    {
        None,

        /// <summary>
        /// The Tranquility server status
        /// </summary>
        [Header("Tranquility Status")]
        [Description("The status of the Tranquility server.")]
        [Update(UpdatePeriod.Minutes5, UpdatePeriod.Never, UpdatePeriod.Hours1, CacheStyle.Short)]
        ServerStatus,

        /// <summary>
        /// The characters available on an account.
        /// </summary>
        [Header("Characters on Account")]
        [Description("The retrieval of the characters list available on every account.")]
        [Update(UpdatePeriod.Day, UpdatePeriod.Hours1, CacheStyle.Short)]
        [ForcedOnStartup]
        CharacterList,

        /// <summary>
        /// A character sheet (bio, skills, implants, etc).
        /// </summary>
        [Header("Character Sheet")]
        [Description("A character's sheet listing biography, skills, attributes and implants informations.")]
        [Update(UpdatePeriod.Hours1, UpdatePeriod.Hours1, CacheStyle.Short)]
        CharacterSheet,

        /// <summary>
        /// A character's skill queue.
        /// </summary>
        [Header("Skill Queue")]
        [Description("A character's skill queue.")]
        [Update(UpdatePeriod.Hours1, UpdatePeriod.Hours1, CacheStyle.Short)]
        SkillQueue,

        /// <summary>
        /// The personal issued market orders of a character. Only downloaded when a full API key is provided.
        /// </summary>
        [FullKey]
        [Header("Market Orders")]
        [Description("The market orders of a character.")]
        [Update(UpdatePeriod.Hours1, UpdatePeriod.Hours1, CacheStyle.Long)]
        MarketOrders,

        /// <summary>
        /// The corporation issued market orders of a character. Only downloaded when a full API key is provided.
        /// </summary>
        [FullKey]
        [Update(UpdatePeriod.Hours1, UpdatePeriod.Hours1, CacheStyle.Long)]
        CorporationMarketOrders,

        /// <summary>
        /// The personal issued industry jobs of a character. Only downloaded when a full API key is provided.
        /// </summary>
        [FullKey]
        [Header("Industry Jobs")]
        [Description("The industry jobs of a character.")]
        [Update(UpdatePeriod.Minutes15, UpdatePeriod.Minutes15, CacheStyle.Short)]
        IndustryJobs,

        /// <summary>
        /// The research points of a character. Only downloaded when a full API key is provided.
        /// </summary>
        [FullKey]
        [Header("Research")]
        [Description("Research Points for a character.")]
        [Update(UpdatePeriod.Minutes15, UpdatePeriod.Minutes15, CacheStyle.Short)]
        ResearchPoints,

        /// <summary>
        /// The corporation issued industry jobs of a character. Only downloaded when a full API key is provided.
        /// </summary>
        [FullKey]
        [Update(UpdatePeriod.Minutes15, UpdatePeriod.Minutes15, CacheStyle.Long)]
        CorporationIndustryJobs,

        /// <summary>
        /// Mail messages for a character. Only downloaded when a full API key is provided.
        /// </summary>
        [FullKey]
        [Header("Mail Messages")]
        [Description("Mail messages for a character.")]
        [Update(UpdatePeriod.Minutes30, UpdatePeriod.Minutes30, CacheStyle.Long)]
        MailMessages,

        /// <summary>
        /// Notifications for a character. Only downloaded when a full API key is provided.
        /// </summary>
        [FullKey]
        [Header("Notifications")]
        [Description("Notifications messages for a character.")]
        [Update(UpdatePeriod.Minutes30, UpdatePeriod.Minutes30, CacheStyle.Long)]
        Notifications,

        /// <summary>
        /// A frequently updated wallet balance. Only used for testing whether the API key is full or limited.
        /// </summary>
        [FullKey]
        CharacterAccountBalance,

        /// <summary>
        /// The conquerable station list. Only downloaded when a full API key is provided.
        /// </summary>
        [FullKey]
        ConquerableStationList,

        /// <summary>
        /// The skill in training of a character. Used to determine if an account has a character in training.
        /// </summary>
        CharacterSkillInTraining
    }

    #endregion

}