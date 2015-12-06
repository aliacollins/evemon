using System.Collections.ObjectModel;
using System.Linq;
using EVEMon.Common.Constants;
using EVEMon.Common.Enumerations.API;
using EVEMon.Common.Serialization.Eve;

namespace EVEMon.Common.Service
{
    public static class EveRefType
    {
        private static Collection<SerializableRefTypesListItem> s_refTypes = new Collection<SerializableRefTypesListItem>();
        private static bool s_isQuerying;
        private static bool s_loaded;


        #region Importation

        /// <summary>
        /// Ensures the list has been imported.
        /// </summary>
        private static void EnsureImportation()
        {
            // Exit if we have already imported the list or are currently querying
            if (s_loaded || s_isQuerying)
                return;

            s_isQuerying = true;

            // Query the API
            EveMonClient.APIProviders.CurrentProvider
                .QueryMethodAsync<SerializableAPIRefTypes>(APIGenericMethods.RefTypes, OnUpdated);
        }

        /// <summary>
        /// Processes the refTypes list.
        /// </summary>
        /// <param name="result">The result.</param>
        private static void OnUpdated(APIResult<SerializableAPIRefTypes> result)
        {
            // Checks if EVE database is out of service
            if (result.EVEDatabaseError)
                return;

            // Was there an error ?
            if (result.HasError)
            {
                EveMonClient.Notifications.NotifyRefTypesError(result);
                return;
            }

            EveMonClient.Notifications.InvalidateAPIError();

            s_refTypes = result.Result.RefTypes;

            s_loaded = true;
            s_isQuerying = false;

            // Notify the subscribers
            EveMonClient.OnRefTypesUpdated();
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Gets the name of the provided refTypeID.
        /// </summary>
        /// <param name="refTypeID">The ref type ID.</param>
        /// <returns></returns>
        public static string GetRefTypeIDToName(int refTypeID)
        {
            EnsureImportation();

            SerializableRefTypesListItem refType = s_refTypes.FirstOrDefault(type => type.ID == refTypeID);
            return refType != null ? refType.Name : EVEMonConstants.UnknownText;
        }

        #endregion
    }
}