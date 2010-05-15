using System;
using System.ComponentModel;
using System.Xml.Serialization;

using EVEMon.Common.Attributes;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.Common
{
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
        All = Amarr|Minmatar|Caldari|Gallente|Faction|Jove|Ore
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
    /// Represents the class of a learning skill
    /// </summary>
    public enum LearningClass
    {
        None,
        Learning,
        LowerTierAttribute,
        UpperTierAttribute
    }

    /// <summary>
    /// Represents the options one can use with <see cref="CharacterSchartchpad.Learn()"/>. Those are only optimizations
    /// </summary>
    [Flags] public enum LearningOptions
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
    /// Represents the activity of a blueprint.
    /// </summary>
    public enum BlueprintActivity
    {
        None = 0,
        Manufacturing = 1,
        ResearchingTechnology = 2,
        ResearchingTimeProductivity = 3,
        ResearchingMaterialProductivity = 4,
        Copying = 5,
        Duplicating = 6,
        ReverseEngineering = 7,
        Invention = 8
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
        /// /// Mail messages for a character. Only downloaded when a full API key is provided.
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
        /// The personal issued market orders of a character. Only downloaded when a full API key is provided.
        /// </summary>
        [FullKey]
        [Header("Market Orders")]
        [Description("The market orders of a character.")]
        [Update(UpdatePeriod.Hours1, UpdatePeriod.Hours1, CacheStyle.Long)]
        MarketOrders,

        /// <summary>
        /// A frequently updated wallet balance. Only used for testing whether the API key is full or limited.
        /// </summary>
        [FullKey]
        CharacterAccountBalance,

        /// <summary>
        /// The corporation issued market orders of a character. Only downloaded when a full API key is provided.
        /// </summary>
        [FullKey]
        [Update(UpdatePeriod.Hours1, UpdatePeriod.Hours1, CacheStyle.Long)]
        CorporationMarketOrders,

        /// <summary>
        /// The conquerable station list. Only downloaded when a full API key is provided.
        /// </summary>
        [FullKey]
        ConquerableStationList,

        /// <summary>
        /// The skill in training of a character.
        /// </summary>
        CharacterSkillInTraining
    }
}