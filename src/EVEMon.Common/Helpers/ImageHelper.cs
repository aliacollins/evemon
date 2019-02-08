using System;
using EVEMon.Common.Constants;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Service;

namespace EVEMon.Common.Helpers
{
    public static class ImageHelper
    {
        private const string NO_TYPE = "";

        /// <summary>
        /// Gets the URL for the image of the specified type and ID.
        /// </summary>
        /// <param name="useFallbackUri">if set to <c>true</c> [use fallback URI].</param>
        /// <param name="type">the type of the ID.</param>
        /// <param name="id">the ID for the given type.</param>
        /// <param name="size">the image size.</param>
        /// <returns></returns>
        public static Uri GetImageUrl(bool useFallbackUri, string type, long id, int size = (int)EveImageSize.x32)
        {
            return GetImageUrlHelper(NetworkConstants.CCPIconsFromImageServer, useFallbackUri, type, id, size);
        }

        /// <summary>
        /// Gets the URL for the portrait of the specified character.
        /// </summary>
        /// <param name="useFallbackUri">if set to <c>true</c> [use fallback URI].</param>
        /// <param name="id">the ID for the character.</param>
        /// <param name="size">the image size.</param>
        /// <returns></returns>
        public static Uri GetPortraitUrl(bool useFallbackUri, long id, int size = (int)EveImageSize.x32)
        {
            return GetImageUrlHelper(NetworkConstants.CCPPortraits, useFallbackUri, NO_TYPE, id, size);
        }

        /// <summary>
        /// Helper function to get a potrait or icon URL, based on parameters.
        /// </summary>
        private static Uri GetImageUrlHelper(string format, bool useFallbackUri, string type, long id, int size)
        {
            string path = type == NO_TYPE
                ? string.Format(CultureConstants.InvariantCulture, format, id, size)
                : string.Format(CultureConstants.InvariantCulture, format, type, id, size);

            return useFallbackUri
                ? ImageService.GetImageServerBaseUri(path)
                : ImageService.GetImageServerCdnUri(path);
        }
    }
}
