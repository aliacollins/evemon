using System.ComponentModel;
using EVEMon.Common.Attributes;
using EVEMon.Common.Enumerations.UISettings;

namespace EVEMon.Common.Enumerations.CCPAPI
{
    /// <summary>
    /// Enumeration of the generic API methods. Those methods do not have access mask.
    /// Each method should have an entry in APIMethodsEnum and
    /// an equivalent string entry in NetworkConstants indicating the default path of the method.
    /// </summary>
    public enum CCPAPIGenericMethods : ulong
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
        /// A list of the planets on which the character has a command center located.
        /// </summary>
        /// <remarks>
        /// This method has the same access mask as the character AssetList as specified by CCP.
        /// It should never change place as its order number in this enumeration matches
        /// that of the character AssetList.
        /// </remarks>
        [Header("Planetary Colonies")]
        [Description("The planetary colonies of a character.")]
        [Update(UpdatePeriod.Hours1, UpdatePeriod.Hours1, CacheStyle.Short)]
        PlanetaryColonies,

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

        /// <summary>
        /// The personal blueprints.
        /// </summary>
        Blueprints,

        /// <summary>
        /// The corporation blueprints.
        /// </summary>
        CorporationBlueprints,
    }
}