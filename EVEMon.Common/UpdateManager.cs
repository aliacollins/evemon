using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

using EVEMon.Common.Net;
using EVEMon.Common.Threading;
using EVEMon.Common.Serialization.BattleClinic;


namespace EVEMon.Common
{
    /// <summary>
    /// Takes care of looking for new versions of EVEMon and its datafiles.
    /// </summary>
    public static class UpdateManager
    {
        private static bool s_checkScheduled = false;
        private static bool s_enabled;
        private static TimeSpan s_frequency = TimeSpan.FromMinutes(Settings.Updates.UpdateFrequency);

        /// <summary>
        /// Delete the installation files on a previous autoupdate.
        /// </summary>
        public static void DeleteInstallationFiles()
        {
            foreach (string file in Directory.GetFiles(EveClient.EVEMonDataDir, "EVEMon-install-*.exe", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception e)
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
            Dispatcher.Schedule(time, () => BeginCheck());
            EveClient.Trace("UpdateManager.ScheduleCheck() in {0}", time);
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

            EveClient.Trace("UpdateManager.BeginCheck()");

            // Otherwise, query Batlleclinic.
            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            var url = String.Format("{0}?ver={1}", Settings.Updates.UpdatesUrl, currentVersion);
            EveClient.HttpWebService.DownloadXmlAsync(url, null, OnCheckCompleted, null);
        }

        /// <summary>
        /// When BC returned us results, we parse them.
        /// </summary>
        /// <remarks>Invoked on some background thread.</remarks>
        /// <param name="e"></param>
        /// <param name="userState"></param>
        private static void OnCheckCompleted(DownloadXmlAsyncResult e, object userState)
        {
            // If update manager has been disabled since the last
            // update was triggered quit out here
            if (!s_enabled)
            {
                s_checkScheduled = false;
                return;
            }

            // Was there an HTTP error ??
            if (e.Error != null)
            {
                // Logs the error and reschedule
                EveClient.Trace("UpdateManager: {0}", e.Error.Message);
                ScheduleCheck(s_frequency);
                return;
            }

            // No http error, let's try to deserialize
            try
            {
                ScanUpdateFeed(e.Result);
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

            EveClient.Trace("UpdateManager.OnCheckCompleted()");
        }
        
        /// <summary>
        /// Scans the feed returned by BattleClinic.
        /// </summary>
        /// <remarks>Invoked on some background thread.</remarks>
        /// <param name="xdoc"></param>
        private static void ScanUpdateFeed(XmlDocument xdoc)
        {
            if (xdoc.DocumentElement.Name != "evemon")
                return;

            XmlElement newestEl = xdoc.DocumentElement.SelectSingleNode("newest") as XmlElement;
            if (newestEl == null)
                return;

            Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            Version newestVersion = new Version(newestEl.SelectSingleNode("version").InnerText);
            string forumUrl = newestEl.SelectSingleNode("url").InnerText;
            string updateMessage = newestEl.SelectSingleNode("message").InnerText;

            bool canAutoInstall = false;
            string installArgs = String.Empty;
            string installerUrl = String.Empty;
            string additionalArgs = String.Empty;
            XmlElement argEl = newestEl.SelectSingleNode("autopatchargs") as XmlElement;
            XmlElement iUrlEl = newestEl.SelectSingleNode("autopatchurl") as XmlElement;
            XmlElement argAdd = newestEl.SelectSingleNode("additionalargs") as XmlElement;
            if (iUrlEl != null && argEl != null)
            {
                canAutoInstall = true;
                installerUrl = iUrlEl.InnerText;
                installArgs = argEl.InnerText;
                additionalArgs = argAdd.InnerText;

                if (additionalArgs != null && additionalArgs.Contains("%EVEMON_EXECUTABLE_PATH%"))
                {
                    string appPath = Path.GetDirectoryName(Application.ExecutablePath);
                    installArgs = String.Format(CultureConstants.DefaultCulture, "{0} {1}", installArgs, additionalArgs);
                    installArgs = installArgs.Replace("%EVEMON_EXECUTABLE_PATH%", appPath);
                }
            }

            // Is the program out of date ?
            if (newestVersion > currentVersion)
            {
                // Requests a notification to subscribers and quit
                EveClient.OnUpdateAvailable(forumUrl, installerUrl, updateMessage, currentVersion,
                                            newestVersion, canAutoInstall, installArgs);
                return;
            }

            // Code is up to date. Lets try the data.
            XmlElement datafilesEl = xdoc.DocumentElement as XmlElement;
            XmlSerializer xs = new XmlSerializer(typeof(SerializablePatch));
            SerializablePatch dfv = null;
            using (XmlNodeReader xnr = new XmlNodeReader(datafilesEl))
            {
                dfv = (SerializablePatch)xs.Deserialize(xnr);
            }

            if (dfv.FilesHaveChanged)
            {
                // Requests a notification to subscribers and quit.
                EveClient.OnDataUpdateAvailable(forumUrl, dfv.ChangedDataFiles);
                return;
            }
        }
    }
}
