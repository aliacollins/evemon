using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Models.Extended
{
    public static class ESIMethods
    {
        private static readonly List<Enum> s_items = new List<Enum>();

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            s_items.AddRange(EnumExtensions.GetValues<ESIAPIGenericMethods>().Cast<Enum>());
        }

        /// <summary>
        /// Gets the methods.
        /// </summary>
        /// <value>The methods.</value>
        public static IEnumerable<Enum> Methods => s_items;

        /// <summary>
        /// Gets the non account related API generic methods.
        /// </summary>
        /// <value>The non account generic methods.</value>
        public static IEnumerable<Enum> NonAccountGenericMethods => s_items.OfType<ESIAPIGenericMethods>().Cast<Enum>();
    }
}
