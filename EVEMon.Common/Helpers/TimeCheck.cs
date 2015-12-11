using System;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using EVEMon.Common.Constants;
using EVEMon.Common.Extensions;
using EVEMon.Common.Net;
using EVEMon.Common.Threading;

namespace EVEMon.Common.Helpers
{
    public delegate void TimeSynchronisationCallback(bool isSynchronised, DateTime serverTime, DateTime localTime);

    /// <summary>
    /// Ensures synchronization of local time to a known time source.
    /// </summary>
    public static class TimeCheck
    {
        /// <summary>
        /// Asynchronous method to determine if the user's clock is syncrhonised to NIST time.
        /// </summary>
        /// <param name="callback">The callback.</param>
        public static void CheckIsSynchronisedToNistTime(TimeSynchronisationCallback callback)
        {
            GetNistTimeAsync(new Uri(NetworkConstants.NISTTimeServer), SyncDownloadCompleted, new SyncState(callback));
        }

        /// <summary>
        /// Gets the nist time asynchronously.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="syncNistCompleted">The synchronize nist completed.</param>
        /// <param name="state">The state.</param>
        private static void GetNistTimeAsync(Uri url, DownloadStringCompletedCallback syncNistCompleted, SyncState state)
        {
            Dispatcher.BackgroundInvoke(() =>
            {
                DateTime dateTimeNowUtc = DateTime.MinValue;
                HttpWebServiceException error = null;
                try
                {
                    using (TcpClient tcpClient = new TcpClient(url.Host, url.Port))
                    using (NetworkStream netStream = tcpClient.GetStream())
                    {
                        if (!netStream.CanRead)
                            return;

                        byte[] data = new byte[24];
                        netStream.Read(data, 0, data.Length);
                        data = data.Skip(7).Take(17).ToArray();

                        dateTimeNowUtc = DateTime.ParseExact(Encoding.ASCII.GetString(data),
                            "yy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture.DateTimeFormat);
                    }
                }
                catch (Exception exc)
                {
                    error = HttpWebServiceException.Exception(url, exc);
                }

                DownloadStringAsyncResult result = new DownloadStringAsyncResult(dateTimeNowUtc.DateTimeToTimeString(), error);
                syncNistCompleted.Invoke(result, state);
            });
        }

        /// <summary>
        /// Callback method for synchronisation check. The user's clock is deemed to be in sync with BattleClinic time
        /// if it is no more than 60 seconds different to BattleClinic time as local time.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="userState"></param>
        private static void SyncDownloadCompleted(DownloadStringAsyncResult e, object userState)
        {
            SyncState state = (SyncState)userState;
            bool isSynchronised = true;
            DateTime serverTimeToLocalTime = DateTime.MinValue.ToLocalTime();
            DateTime localTime = DateTime.Now;
            if (!String.IsNullOrEmpty(e.Result))
            {
                serverTimeToLocalTime = e.Result.TimeStringToDateTime().ToLocalTime();
                double timediff = Math.Abs(serverTimeToLocalTime.Subtract(localTime).TotalSeconds);
                isSynchronised = timediff < 60;
            }
            Dispatcher.Invoke(() => state.Callback(isSynchronised, serverTimeToLocalTime, localTime));
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