using EVEMon.Common.Constants;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Serialization.Eve;
using System.Collections.Generic;

namespace EVEMon.Common.Service
{
    public static class EveRefType
    {
        private static Dictionary<int, SerializableRefTypesListItem> s_refTypes =
            new Dictionary<int, SerializableRefTypesListItem>(128);
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
                .QueryMethodAsync<SerializableAPIRefTypes>(CCPAPIGenericMethods.RefTypes, OnUpdated);
        }

        /// <summary>
        /// Processes the refTypes list.
        /// </summary>
        /// <param name="result">The result.</param>
        private static void OnUpdated(CCPAPIResult<SerializableAPIRefTypes> result)
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

            foreach (var type in result.Result.RefTypes)
            {
                int id = type.ID;
                if (!s_refTypes.ContainsKey(id))
                    s_refTypes.Add(id, type);
            }

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

            SerializableRefTypesListItem refType;
            s_refTypes.TryGetValue(refTypeID, out refType);
            return refType?.Name ?? EveMonConstants.UnknownText;
        }

        #endregion
    }
}
