using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EVEMon.Common.Serialization;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public class EveNotificationType
    {
        private static List<SerializableNotificationRefTypesListItem> s_notificationRefTypes =
            new List<SerializableNotificationRefTypesListItem>();
        private static DateTime s_cachedUntil;
        private static bool s_queryPending;
        private static bool s_loaded;

        private const string Filename = "NotificationRefTypes";


        /// <summary>
        /// Gets the description of the notification type.
        /// </summary>
        /// <param name="typeID">The type ID.</param>
        /// <returns></returns>
        internal static string GetName(int typeID)
        {
            if (EveMonClient.IsDebugBuild)
                EnsureInitialized();
            else
                EnsureImportation();

            SerializableNotificationRefTypesListItem type = s_notificationRefTypes.FirstOrDefault(x => x.TypeID == typeID);
            return type != null ? type.TypeName : EVEMonConstants.UnknownText;
        }

        /// <summary>
        /// Gets the subject layout.
        /// </summary>
        /// <param name="typeID">The type identifier.</param>
        /// <returns></returns>
        internal static string GetSubjectLayout(int typeID)
        {
            if (EveMonClient.IsDebugBuild)
                EnsureInitialized();
            else
                EnsureImportation();

            SerializableNotificationRefTypesListItem type = s_notificationRefTypes.FirstOrDefault(x => x.TypeID == typeID);
            return type != null ? type.SubjectLayout : EVEMonConstants.UnknownText;
        }

        /// <summary>
        /// Gets the text layout.
        /// </summary>
        /// <param name="typeID">The type identifier.</param>
        /// <returns></returns>
        internal static string GetTextLayout(int typeID)
        {
            if (EveMonClient.IsDebugBuild)
                EnsureInitialized();
            else
                EnsureImportation();

            SerializableNotificationRefTypesListItem type = s_notificationRefTypes.FirstOrDefault(x => x.TypeID == typeID);
            return type != null ? type.TextLayout : String.Empty;
        }

        /// <summary>
        /// Ensures the notification types data have been intialized.
        /// </summary>
        private static void EnsureInitialized()
        {
            if (s_loaded)
                return;

            var result =
                Util.DeserializeAPIResultFromString<SerializableNotificationRefTypes>(Properties.Resources.NotificationRefTypes,
                    APIProvider.RowsetsTransform);

            Import(result.Result);
        }

        /// <summary>
        /// Ensures the importation.
        /// </summary>
        private static void EnsureImportation()
        {
            string file = LocalXmlCache.GetFile(Filename).FullName;

            // Update the file if we don't have it or the data have expired
            if (!File.Exists(file) || (s_loaded && s_cachedUntil < DateTime.UtcNow))
            {
                UpdateFile();
                return;
            }

            // Exit if we have already imported the list
            if (s_loaded)
                return;

            s_cachedUntil = File.GetLastWriteTimeUtc(file).AddDays(1);

            // In case the file has an error or it's an old one, we try to get a fresh copy
            if (s_cachedUntil < DateTime.UtcNow)
            {
                UpdateFile();
                return;
            }

            // Deserialize the xml file
            APIResult<SerializableNotificationRefTypes> result = Util.DeserializeAPIResultFromFile<SerializableNotificationRefTypes>(
                file, APIProvider.RowsetsTransform);

            // Import the data
            Import(result.Result);
        }

        /// <summary>
        /// Imports the specified result.
        /// </summary>
        /// <param name="result">The result.</param>
        private static void Import(SerializableNotificationRefTypes result)
        {
            if (result == null)
            {
                EveMonClient.Trace("NotificationRefTypes.Import - failed");
                return;
            }

            EveMonClient.Trace("NotificationRefTypes.Import - begin");

            s_notificationRefTypes = result.Types.ToList();
            s_loaded = true;

            EveMonClient.Trace("NotificationRefTypes.Import - done");
        }

        /// <summary>
        /// Updates the file.
        /// </summary>
        private static void UpdateFile()
        {
            // Quit if query is pending
            if (s_queryPending)
                return;

            var url = new Uri(
                String.Format(CultureConstants.InvariantCulture, "{0}{1}", NetworkConstants.BitBucketWikiBase,
                    NetworkConstants.NotificationRefTypes));

            Util.DownloadAPIResultAsync<SerializableNotificationRefTypes>(url, OnDownloaded, true,
                transform: APIProvider.RowsetsTransform);

            s_queryPending = true;
        }

        /// <summary>
        /// Processes the queried notification ref type.
        /// </summary>
        /// <param name="result">The result.</param>
        private static void OnDownloaded(APIResult<SerializableNotificationRefTypes> result)
        {
            if (!String.IsNullOrEmpty(result.ErrorMessage))
            {
                // Reset query pending flag
                s_queryPending = false;

                EveMonClient.Trace("NotificationRefTypes.UpdateFile - failed");
                EveMonClient.Trace(result.ErrorMessage);

                // Fallback
                EnsureInitialized();
                return;
            }

            s_cachedUntil = DateTime.UtcNow.AddDays(1);

            // Import the list
            Import(result.Result);

            // Reset query pending flag
            s_queryPending = false;

            // Notify the subscribers
            EveMonClient.OnNotificationRefTypesUpdated();

            // Save the file in cache
            LocalXmlCache.Save(Filename, result.XmlDocument);
        }
    }
}