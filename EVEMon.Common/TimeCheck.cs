using System;
using EVEMon.Common.Net;
using System.Globalization;

namespace EVEMon.Common
{
    public delegate void TimeSynchronisationCallback(bool? isSynchronised, DateTime serverTime, DateTime localTime);

    /// <summary>
    /// Ensures synchronisation of local time to a knonw time source
    /// </summary>
    public class TimeCheck
    {
        /// <summary>
        /// Asynchronous method to determine if the user's clock is syncrhonised to Battleclinic time
        /// </summary>
        /// <param name="callback"></param>
        public static void CheckIsSynchronised(TimeSynchronisationCallback callback)
        {
            SyncState state = new SyncState(callback);
            CommonContext.HttpWebService.DownloadStringAsync("http://www.battleclinic.com/time.php", SyncDownloadCompleted, state);
        }

        /// <summary>
        /// Callback method for synchronisation check. The user's clock is deemed to be in sync with Battleclinic time
        /// if it is no more than 60 seconds different to Battleclinic time as local time
        /// </summary>
        /// <param name="e"></param>
        /// <param name="userState"></param>
        private static void SyncDownloadCompleted(DownloadStringAsyncResult e, object userState)
        {
            DateTime completionTime = DateTime.Now;
            SyncState state = (SyncState)userState;
            bool? isSynchronised = null;
            DateTime timeNow = DateTime.MinValue;
            if (e.Error == null)
            {
                string timeString = e.Result;
                if (timeString != string.Empty)
                {
                    try
                    {
                        timeNow = DateTime.ParseExact(timeString, "yyyy'-'MM'-'dd'T'HH':'mm':'sszzz", CultureInfo.InvariantCulture);
                        double timediff = Math.Abs((timeNow.ToLocalTime() - completionTime).TotalSeconds);
                        isSynchronised = timediff < 60;
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.LogException(ex, true);
                    }
                }
            }
            state.Callback(isSynchronised, timeNow, completionTime);
        }

        /// <summary>
        /// Helper class for asyncronous time sync requests
        /// </summary>
        private class SyncState
        {
            private readonly TimeSynchronisationCallback _callback;

            internal SyncState (TimeSynchronisationCallback callback)
            {
                _callback = callback;
            }

            internal TimeSynchronisationCallback Callback
            {
                get { return _callback; }
            }
        }
    }
}
