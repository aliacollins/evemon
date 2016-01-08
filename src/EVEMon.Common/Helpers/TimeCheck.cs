using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using EVEMon.Common.Constants;
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
        public static async void CheckIsSynchronisedToNistTime(TimeSynchronisationCallback callback)
        {
            SyncResult result = await GetNistTimeAsync(new Uri(NetworkConstants.NISTTimeServer));
            Dispatcher.Invoke(() => callback.Invoke(result.IsSynchronised, result.ServerTimeToLocalTime, result.LocalTime));
        }

        /// <summary>
        /// Gets the nist time asynchronously.
        /// </summary>
        /// <param name="url">The URL.</param>
        private static async Task<SyncResult> GetNistTimeAsync(Uri url)
        {
            DateTime serverTimeToLocalTime = DateTime.MinValue.ToLocalTime();
            DateTime localTime = DateTime.Now;
            TimeSpan timediff = TimeSpan.Zero;

            try
            {
                await Dns.GetHostAddressesAsync(url.Host)
                    .ContinueWith(async task1 =>
                    {
                        if (task1.Result.Any())
                        {
                            DateTime dateTimeNowUtc;

                            using (TcpClient tcpClient = new TcpClient())
                            {
                                await tcpClient.ConnectAsync(task1.Result.First(), url.Port);

                                using (NetworkStream netStream = tcpClient.GetStream())
                                {
                                    byte[] data = new byte[24];
                                    netStream.Read(data, 0, data.Length);
                                    data = data.Skip(7).Take(17).ToArray();

                                    dateTimeNowUtc = DateTime.ParseExact(Encoding.ASCII.GetString(data),
                                        "yy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture.DateTimeFormat);
                                }
                            }

                            serverTimeToLocalTime = dateTimeNowUtc.ToLocalTime();
                            timediff = TimeSpan.FromSeconds(Math.Abs(serverTimeToLocalTime.Subtract(localTime).TotalSeconds));
                        }
                    });
            }
            catch (Exception exc)
            {
                ExceptionHandler.LogException(exc, true);
            }

            return new SyncResult(timediff < TimeSpan.FromSeconds(60), serverTimeToLocalTime, localTime);
        }

        ///// <summary>
        ///// Helper class for the result of asyncronous time sync requests.
        ///// </summary>
        private class SyncResult
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SyncResult"/> class.
            /// </summary>
            /// <param name="isSynchronised">if set to <c>true</c> the user's machine is synchronised.</param>
            /// <param name="serverTimeToLocalTime">The server time to local time.</param>
            /// <param name="localTime">The local time.</param>
            internal SyncResult(bool isSynchronised, DateTime serverTimeToLocalTime, DateTime localTime)
            {
                IsSynchronised = isSynchronised;
                ServerTimeToLocalTime = serverTimeToLocalTime;
                LocalTime = localTime;
            }

            /// <summary>
            /// Gets a value indicating whether this instance is synchronised.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance is synchronised; otherwise, <c>false</c>.
            /// </value>
            public bool IsSynchronised { get; }

            /// <summary>
            /// Gets the server time to local time.
            /// </summary>
            /// <value>
            /// The server time to local time.
            /// </value>
            public DateTime ServerTimeToLocalTime { get; }

            /// <summary>
            /// Gets the local time.
            /// </summary>
            /// <value>
            /// The local time.
            /// </value>
            public DateTime LocalTime { get; }
        }
    }
}