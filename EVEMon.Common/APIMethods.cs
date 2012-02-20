using System;
using System.Collections.Generic;
using System.Linq;

namespace EVEMon.Common
{
    public static class APIMethods
    {
        private static readonly List<Enum> s_items = new List<Enum>();

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public static void Initialize()
        {
            s_items.AddRange(EnumExtensions.GetValues<APIGenericMethods>().Cast<Enum>());
            s_items.AddRange(EnumExtensions.GetValues<APICharacterMethods>().Where(
                apiCharacterMethod => apiCharacterMethod != APICharacterMethods.None).Cast<Enum>());
            s_items.AddRange(EnumExtensions.GetValues<APICorporationMethods>().Where(
                apiCorporationMethod => apiCorporationMethod != APICorporationMethods.None).Cast<Enum>());
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
                return s_items.OfType<APIGenericMethods>().Where(
                    method => method != APIGenericMethods.APIKeyInfo &&
                              method != APIGenericMethods.CharacterList
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
                return s_items.OfType<APIGenericMethods>().Where(
                    method => method == APIGenericMethods.ContractItems ||
                              method == APIGenericMethods.ContractBids).Cast<Enum>();
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
                return s_items.OfType<APIGenericMethods>().Where(
                    method => method == APIGenericMethods.CorporationContractItems ||
                              method == APIGenericMethods.CorporationContractBids).Cast<Enum>();
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