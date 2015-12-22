using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Models.Extended
{
    public static class APIMethods
    {
        private static readonly List<Enum> s_items = new List<Enum>();

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            s_items.AddRange(EnumExtensions.GetValues<CCPAPIGenericMethods>().Cast<Enum>());
            s_items.AddRange(EnumExtensions.GetValues<CCPAPICharacterMethods>().Where(
                apiCharacterMethod => apiCharacterMethod != CCPAPICharacterMethods.None).Cast<Enum>());
            s_items.AddRange(EnumExtensions.GetValues<CCPAPICorporationMethods>().Where(
                apiCorporationMethod => apiCorporationMethod != CCPAPICorporationMethods.None).Cast<Enum>());
        }

        /// <summary>
        /// Gets the methods.
        /// </summary>
        /// <value>The methods.</value>
        public static IEnumerable<Enum> Methods
        {
            get { return s_items; }
        }

        /// <summary>
        /// Gets the non account related API generic methods.
        /// </summary>
        /// <value>The non account generic methods.</value>
        public static IEnumerable<Enum> NonAccountGenericMethods
        {
            get
            {
                return s_items.OfType<CCPAPIGenericMethods>().Where(
                    method => method != CCPAPIGenericMethods.APIKeyInfo &&
                              method != CCPAPIGenericMethods.CharacterList
                              && !AllSupplementalMethods.Contains(method)).Cast<Enum>();
            }
        }

        /// <summary>
        /// Gets the character related supplemental methods.
        /// </summary>
        /// <value>The character related supplemental methods.</value>
        public static IEnumerable<Enum> CharacterSupplementalMethods
        {
            get
            {
                return s_items.OfType<CCPAPIGenericMethods>().Where(
                    method => method == CCPAPIGenericMethods.ContractItems ||
                              method == CCPAPIGenericMethods.ContractBids ||
                              method == CCPAPIGenericMethods.PlanetaryColonies ||
                              method == CCPAPIGenericMethods.PlanetaryPins ||
                              method == CCPAPIGenericMethods.PlanetaryRoutes ||
                              method == CCPAPIGenericMethods.PlanetaryLinks ||
                              method == CCPAPIGenericMethods.IndustryJobsHistory ||
                              method == CCPAPIGenericMethods.Blueprints).Cast<Enum>();
            }
        }

        /// <summary>
        /// Gets the corporation related supplemental methods.
        /// </summary>
        /// <value>The corporation supplemental methods.</value>
        public static IEnumerable<Enum> CorporationSupplementalMethods
        {
            get
            {
                return s_items.OfType<CCPAPIGenericMethods>().Where(
                    method => method == CCPAPIGenericMethods.CorporationContractItems ||
                              method == CCPAPIGenericMethods.CorporationContractBids ||
                              method == CCPAPIGenericMethods.CorporationCustomsOffices ||
                              method == CCPAPIGenericMethods.CorporationIndustryJobsHistory ||
                              method == CCPAPIGenericMethods.CorporationBlueprints).Cast<Enum>();
            }
        }

        /// <summary>
        /// Gets all the supplemental methods.
        /// </summary>
        /// <value>All supplemental methods.</value>
        public static IEnumerable<Enum> AllSupplementalMethods
        {
            get
            {
                return CharacterSupplementalMethods.Concat(CorporationSupplementalMethods);
            }
        }
    }
}