using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using EVEMon.Common.Constants;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Extensions;
using EVEMon.Common.Net;
using EVEMon.Common.Threading;

namespace EVEMon.Common.Helpers
{
    /// <summary>
    /// Ensures synchronization of local time to a known time source.
    /// </summary>
    public static class TimeCheck
    {
        /// <summary>
        /// Occurs when time check completed.
        /// </summary>
        public static event EventHandler<TimeCheckSyncEventArgs> TimeCheckCompleted;

        /// <summary>
        /// Check for time synchronization,
        /// or reschedule it for later if no connection is available.
        /// </summary>
        public static void ScheduleCheck(TimeSpan time)
        {
            Dispatcher.Schedule(time, BeginCheck);
            EveMonClient.Trace($"in {time}");
        }

        /// <summary>
        /// Method to determine if the user's clock is syncrhonised to NIST time.
        /// </summary>
        private static void BeginCheck()
        {
            if (!NetworkMonitor.IsNetworkAvailable)
            {
                // Reschedule later otherwise
                ScheduleCheck(TimeSpan.FromMinutes(1));
                return;
            }

            EveMonClient.Trace();

            Uri url = new Uri(NetworkConstants.NISTTimeServer);
            DateTime serverTimeToLocalTime;
            bool isSynchronised;

            Dns.GetHostAddressesAsync(url.Host)
                .ContinueWith(async task =>
                {
                    IPAddress[] ipAddresses = task.Result;

                    if (!ipAddresses.Any())
                        return;

                    try
                    {
                        DateTime dateTimeNowUtc;
                        DateTime localTime = DateTime.Now;
                        using (TcpClient tcpClient = new TcpClient())
                        {
                            await tcpClient.ConnectAsync(ipAddresses.First(), url.Port);

                            using (NetworkStream netStream = tcpClient.GetStream())
                            {
                                byte[] data = new byte[24];
                                netStream.Read(data, 0, data.Length);
                                data = data.Skip(7).Take(17).ToArray();
                                string dateTimeText = Encoding.ASCII.GetString(data);
                                dateTimeNowUtc = DateTime.ParseExact(dateTimeText,
                                    "yy-MM-dd HH:mm:ss",
                                    CultureInfo.CurrentCulture.DateTimeFormat,
                                    DateTimeStyles.AssumeUniversal);
                            }
                        }

                        serverTimeToLocalTime = dateTimeNowUtc.ToLocalTime();
                        TimeSpan timediff =
                            TimeSpan.FromSeconds(Math.Abs(serverTimeToLocalTime.Subtract(localTime).TotalSeconds));
                        isSynchronised = timediff < TimeSpan.FromSeconds(60);

                        OnCheckCompleted(isSynchronised, serverTimeToLocalTime, localTime);
                    }
                    catch (Exception exc)
                    {
                        CheckFailure(exc);
                    }
                }, EveMonClient.CurrentSynchronizationContext).ConfigureAwait(false);
        }

        /// <summary>
        /// Called when the check fails.
        /// </summary>
        /// <param name="exc">The exc.</param>
        private static void CheckFailure(Exception exc)
        {
            EveMonClient.Trace(exc.Message);
            ScheduleCheck(TimeSpan.FromMinutes(1));
        }

        /// <summary>
        /// Called when time check completed.
        /// </summary>
        /// <param name="isSynchronised">if set to <c>true</c> [is synchronised].</param>
        /// <param name="serverTimeToLocalTime">The server time to local time.</param>
        /// <param name="localTime">The local time.</param>
        private static void OnCheckCompleted(bool isSynchronised, DateTime serverTimeToLocalTime, DateTime localTime)
        {
            EveMonClient.Trace(Settings.Updates.CheckTimeOnStartup ?  "Synchronised" : "Disabled");

            TimeCheckCompleted?.ThreadSafeInvoke(null, new TimeCheckSyncEventArgs(isSynchronised, serverTimeToLocalTime, localTime));

            // Reschedule
            ScheduleCheck(TimeSpan.FromDays(1));
        }
    }
}