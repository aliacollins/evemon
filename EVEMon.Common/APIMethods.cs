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
    }
}