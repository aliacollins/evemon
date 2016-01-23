using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using EVEMon.Common.Constants;
using EVEMon.Common.CustomEventArgs;
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
        private static async void BeginCheck()
        {
            if (!NetworkMonitor.IsNetworkAvailable)
            {
                // Reschedule later otherwise
                ScheduleCheck(TimeSpan.FromMinutes(1));
                return;
            }

            EveMonClient.Trace();

            Uri url = new Uri(NetworkConstants.NISTTimeServer);
            DateTime serverTimeToLocalTime = DateTime.MinValue.ToLocalTime();
            DateTime localTime = DateTime.Now;
            bool isSynchronised = false;

            try
            {
                IPAddress[] ipAdresses = await Dns.GetHostAddressesAsync(url.Host);

                if (ipAdresses.Any())
                {
                    DateTime dateTimeNowUtc;

                    using (TcpClient tcpClient = new TcpClient())
                    {
                        await tcpClient.ConnectAsync(ipAdresses.First(), url.Port);

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
                    TimeSpan timediff = TimeSpan.FromSeconds(Math.Abs(serverTimeToLocalTime.Subtract(localTime).TotalSeconds));
                    isSynchronised = timediff < TimeSpan.FromSeconds(60);
                }
            }
            catch (Exception exc)
            {
                EveMonClient.Trace($"TimeCheck.CheckFailure - {exc.Message}", false);
                ScheduleCheck(TimeSpan.FromMinutes(1));
            }

            OnCheckCompleted(isSynchronised, serverTimeToLocalTime, localTime);
        }

        /// <summary>
        /// Called when time check completed.
        /// </summary>
        /// <param name="isSynchronised">if set to <c>true</c> [is synchronised].</param>
        /// <param name="serverTimeToLocalTime">The server time to local time.</param>
        /// <param name="localTime">The local time.</param>
        private static void OnCheckCompleted(bool isSynchronised, DateTime serverTimeToLocalTime, DateTime localTime)
        {
            if (!Settings.Updates.CheckTimeOnStartup || isSynchronised ||
                (serverTimeToLocalTime == DateTime.MinValue.ToLocalTime()))
            {
                if (!Settings.Updates.CheckTimeOnStartup)
                    EveMonClient.Trace("Disabled");
                else if (isSynchronised)
                    EveMonClient.Trace("Synchronised");
                else if (serverTimeToLocalTime == DateTime.MinValue.ToLocalTime())
                {
                    EveMonClient.Trace("Failed");
                    return;
                }
            }

            TimeCheckCompleted?.Invoke(null, new TimeCheckSyncEventArgs(isSynchronised, serverTimeToLocalTime, localTime));

            // Reschedule
            ScheduleCheck(TimeSpan.FromDays(1));
        }
    }
}