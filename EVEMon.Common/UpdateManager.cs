using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using EVEMon.Common.Net;
using EVEMon.Common.Serialization.BattleClinic;
using EVEMon.Common.Threading;

namespace EVEMon.Common
{
    /// <summary>
    /// Takes care of looking for new versions of EVEMon and its datafiles.
    /// </summary>
    public static class UpdateManager
    {
        private static readonly TimeSpan s_frequency = TimeSpan.FromMinutes(Settings.Updates.UpdateFrequency);

        private static bool s_checkScheduled;
        private static bool s_enabled;

        /// <summary>
        /// Delete the installation files on a previous autoupdate.
        /// </summary>
        public static void DeleteInstallationFiles()
        {
            foreach (string file in Directory.GetFiles(
                EveMonClient.EVEMonDataDir, "EVEMon-install-*.exe", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    File.Delete(file);
                }
                catch (UnauthorizedAccessException e)
                {
                    ExceptionHandler.LogException(e, false);
                }
            }
        }

        /// <summary>
        /// Delete the data files on an autoupdate.
        /// </summary>
        public static void DeleteDataFiles()
        {
            foreach (string file in Directory.GetFiles(
                EveMonClient.EVEMonDataDir, "*.xml.gz", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    File.Delete(file);
                }
                catch (UnauthorizedAccessException e)
                {
                    ExceptionHandler.LogException(e, false);
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the autoupdater is enabled.
        /// </summary>
        public static bool Enabled
        {
            get { return s_enabled; }
            set
            {
                s_enabled = value;

                if (!s_enabled)
                    return;

                if (s_checkScheduled)
                    return;

                // Schedule a check in 10 seconds
                ScheduleCheck(TimeSpan.FromSeconds(10));
            }
        }

        /// <summary>
        /// Schedules a check a specified time period in the future.
        /// </summary>
        /// <param name="time">Time period in the future to start check.</param>
        private static void ScheduleCheck(TimeSpan time)
        {
            s_checkScheduled = true;
            Dispatcher.Schedule(time, BeginCheck);
            EveMonClient.Trace("UpdateManager.ScheduleCheck in {0}", time);
        }

        /// <summary>
        /// Performs the check asynchronously.
        /// </summary>
        /// <remarks>Invoked on the UI thread the first time and on some background thread the rest of the time.</remarks>
        /// <returns></returns>
        private static void BeginCheck()
        {
            // If update manager has been disabled since the last
            // update was triggered quit out here
            if (!s_enabled)
            {
                s_checkScheduled = false;
                return;
            }

            // No connection ? Recheck in one minute
            if (!NetworkMonitor.IsNetworkAvailable)
            {
                ScheduleCheck(TimeSpan.FromMinutes(1));
                return;
            }

            EveMonClient.Trace("UpdateManager.BeginCheck");

            // Otherwise, query for the patch file
            // First look up for an emergency patch
            Util.DownloadXMLAsync<SerializablePatch>(
                new Uri(String.Format(CultureConstants.DefaultCulture,
                                      "{0}-emergency.xml", Settings.Updates.UpdatesAddress.Replace(".xml", String.Empty))),
                (result, errorMessage) =>
                    {
                        // If no emergency patch found proceed with the regular
                        if (!String.IsNullOrEmpty(errorMessage))
                        {
                            Util.DownloadXMLAsync<SerializablePatch>(new Uri(Settings.Updates.UpdatesAddress), OnCheckCompleted);
                            return;
                        }

                        // Proccess the emergency patch
                        OnCheckCompleted(result, errorMessage);
                    });
        }

        /// <summary>
        /// Called when patch file check completed.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="errorMessage">The error message.</param>
        private static void OnCheckCompleted(SerializablePatch result, string errorMessage)
        {
            // If update manager has been disabled since the last
            // update was triggered quit out here
            if (!s_enabled)
            {
                s_checkScheduled = false;
                return;
            }

            // Was there an error ?
            if (!String.IsNullOrEmpty(errorMessage))
            {
                // Logs the error and reschedule
                EveMonClient.Trace("UpdateManager: {0}", errorMessage);
                ScheduleCheck(s_frequency);
                return;
            }

            // No error, let's try to deserialize
            try
            {
                ScanUpdateFeed(result);
            }
                // An error occurred during the deserialization
            catch (InvalidOperationException exc)
            {
                ExceptionHandler.LogException(exc, true);
            }
            finally
            {
                // Reschedule
                ScheduleCheck(s_frequency);
            }

            EveMonClient.Trace("UpdateManager.OnCheckCompleted");
        }

        /// <summary>
        /// Scans the update feed.
        /// </summary>
        /// <param name="result">The result.</param>
        private static void ScanUpdateFeed(SerializablePatch result)
        {
            Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            Version newestVersion = new Version(result.Release.Version);

            // Is the program out of date ?
            if (newestVersion > currentVersion)
            {
                Uri forumUrl = result.Release.TopicUrl;
                Uri installerUrl = result.Release.Url;
                string updateMessage = result.Release.Message;
                string installArgs = result.Release.InstallerArgs;
                string md5Sum = result.Release.MD5Sum;
                string additionalArgs = result.Release.AdditionalArgs;
                bool canAutoInstall = (!String.IsNullOrEmpty(installerUrl.AbsoluteUri) && !String.IsNullOrEmpty(installArgs));

                if (!String.IsNullOrEmpty(additionalArgs) && additionalArgs.Contains("%EVEMON_EXECUTABLE_PATH%"))
                {
                    string appPath = Path.GetDirectoryName(Application.ExecutablePath);
                    installArgs = String.Format(CultureConstants.DefaultCulture, "{0} {1}", installArgs, additionalArgs);
                    installArgs = installArgs.Replace("%EVEMON_EXECUTABLE_PATH%", appPath);
                }

                // Requests a notification to subscribers and quit
                EveMonClient.OnUpdateAvailable(forumUrl, installerUrl, updateMessage, currentVersion,
                                               newestVersion, md5Sum, canAutoInstall, installArgs);
                return;
            }

            if (!result.FilesHaveChanged)
                return;

            // Requests a notification to subscribers
            EveMonClient.OnDataUpdateAvailable(result.ChangedDatafiles);
        }
    }
}