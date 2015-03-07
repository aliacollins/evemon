using System;
using EVEMon.Common.Net;

namespace EVEMon.Common
{
    public delegate void TimeSynchronisationCallback(bool isSynchronised, DateTime serverTime, DateTime localTime);

    /// <summary>
    /// Ensures synchronization of local time to a know time source.
    /// </summary>
    public static class TimeCheck
    {
        private static string s_macAddressSHA1Sum;

        /// <summary>
        /// Asynchronous method to determine if the user's clock is syncrhonized to BattleClinic time.
        /// </summary>
        /// <param name="callback"></param>
        /// <remarks>We are sending also a unique id to use for statistical purposes.</remarks>
        public static void CheckIsSynchronised(TimeSynchronisationCallback callback)
        {
            SyncState state = new SyncState(callback);
            Uri url =
                new Uri(String.Format(CultureConstants.InvariantCulture, "{0}{1}", NetworkConstants.BattleClinicBase,
                    NetworkConstants.BatlleClinicTimeSynch));
            string id = s_macAddressSHA1Sum ?? (s_macAddressSHA1Sum = Util.CreateSHA1SumFromMacAddress());
            string postdata = String.Format(CultureConstants.InvariantCulture, "id={0}", id);
            HttpWebService.DownloadStringAsync(url, SyncDownloadCompleted, state, HttpMethod.Post, false, postdata);
        }

        /// <summary>
        /// Callback method for synchronisation check. The user's clock is deemed to be in sync with BattleClinic time
        /// if it is no more than 60 seconds different to BattleClinic time as local time.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="userState"></param>
        private static void SyncDownloadCompleted(DownloadStringAsyncResult e, object userState)
        {
            DateTime localTime = DateTime.Now;
            SyncState state = (SyncState)userState;
            bool isSynchronised = true;
            DateTime serverTime = DateTime.MinValue;
            if (!String.IsNullOrEmpty(e.Result))
            {
                serverTime = e.Result.TimeStringToDateTime().ToLocalTime();
                double timediff = Math.Abs(serverTime.Subtract(localTime).TotalSeconds);
                isSynchronised = timediff < 60;
            }
            state.Callback(isSynchronised, serverTime, localTime);
        }

        /// <summary>
        /// Helper class for asyncronous time sync requests.
        /// </summary>
        private class SyncState
        {
            private readonly TimeSynchronisationCallback m_callback;

            /// <summary>
            /// Initializes a new instance of the <see cref="SyncState"/> class.
            /// </summary>
            /// <param name="callback">The callback.</param>
            internal SyncState(TimeSynchronisationCallback callback)
            {
                m_callback = callback;
            }

            /// <summary>
            /// Gets the callback.
            /// </summary>
            internal TimeSynchronisationCallback Callback
            {
                get { return m_callback; }
            }
        }
    }
}