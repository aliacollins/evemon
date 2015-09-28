using System.Linq;
using EVEMon.Common.Constants;
using EVEMon.Common.Models;
using EVEMon.Common.Serialization;

namespace EVEMon.Common.Service
{
    public static class EveFlag
    {
        private static SerializableEveFlags s_eveFlags;
        private static bool s_isLoaded;

        /// <summary>
        /// Gets the description of the flag.
        /// </summary>
        /// <param name="id">The flag id.</param>
        /// <returns></returns>
        internal static string GetFlagText(int id)
        {
            EnsureInitialized();

            SerializableEveFlagsListItem flag = s_eveFlags != null
                                                    ? s_eveFlags.EVEFlags.FirstOrDefault(x => x.ID == id)
                                                    : null;
            return flag != null ? flag.Text : EVEMonConstants.UnknownText;
        }

        /// <summary>
        /// Ensures the notification types data have been intialized.
        /// </summary>
        private static void EnsureInitialized()
        {
            if (s_isLoaded)
                return;

            // Deserialize the resource file
            s_eveFlags = Util.DeserializeXmlFromString<SerializableEveFlags>(Properties.Resources.Flags, APIProvider.RowsetsTransform);

            s_isLoaded = true;
        }
    }
}
