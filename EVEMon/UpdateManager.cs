using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using EVEMon.Common;
using EVEMon.Common.Net;
using EVEMon.Common.Threading;

namespace EVEMon
{
    /// <summary>
    /// Takes care of looking for new versions of EVEMon and its datafiles.
    /// </summary>
    public static class UpdateManager
    {
        public static event UpdateAvailableHandler UpdateAvailable;
        public static event DataUpdateAvailableHandler DataUpdateAvailable;

        private static readonly object s_lockObject = new object();
        private static bool s_firstCheck;
        private static bool s_enabled;
        private static TimeSpan s_frequency = TimeSpan.FromMinutes(120);

        /// <summary>
        /// Delete the installation files on a previous autodupdate.
        /// </summary>
        public static void DeleteInstallationFiles()
        {
            foreach (string s in Directory.GetFiles(EveClient.EVEMonDataDir, "EVEMon-install-*.exe", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    File.Delete(s);
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
                if (!s_enabled) return;

                // If the first check has already been performed once, 
                // there is an action already scheduled at any time.
                if (!s_firstCheck)
                {
                    s_firstCheck = true;
                    Dispatcher.Schedule(TimeSpan.FromSeconds(10), () => BeginCheck());
                }
            }
        }

        /// <summary>
        /// Performs the check asynchronously.
        /// </summary>
        /// <remarks>Invoked on the UI thread the first time and on some background thread the rest of the time.</remarks>
        /// <returns></returns>
        private static void BeginCheck()
        {
            // No connection ? Recheck in one minute.
            if (!NetworkMonitor.IsNetworkAvailable)
            {
                Dispatcher.Schedule(TimeSpan.FromMinutes(1), () => BeginCheck());
            }
            // When disabled, reschedule in ten minutes
            else if (!s_enabled)
            {
                Dispatcher.Schedule(s_frequency, () => BeginCheck());
            }
            // Otherwise, query Batlleclinic.
            else
            {
                var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                var url = NetworkConstants.BattleclinicUpdates + "?ver=" + currentVersion;
                EveClient.HttpWebService.DownloadXmlAsync(url, null, OnCheckCompleted, null);
            }
        }

        /// <summary>
        /// When BC returned us results, we parse them.
        /// </summary>
        /// <remarks>Invoked on some background thread.</remarks>
        /// <param name="e"></param>
        /// <param name="userState"></param>
        private static void OnCheckCompleted(DownloadXmlAsyncResult e, object userState)
        {
            // If disabled, reschedule in ten minutes and quits
            if (!s_enabled)
            {
                Dispatcher.Schedule(s_frequency, () => BeginCheck());
                return;
            }

            // Was there an HTTP error ??
            if (e.Error != null)
            {
                // Stores the error and reschedule in ten minutes
                Trace.WriteLine("UpdateManager: " + e.Error.Message);
                Dispatcher.Schedule(s_frequency, () => BeginCheck());
                return;
            }

            // No http error, let's try to deserialize
            try
            {
                ScanUpdateFeed(e.Result);
            }
            // An error occured during the deserialization
            catch (InvalidOperationException exc)
            {
                ExceptionHandler.LogException(exc, true);
            }
            finally
            {
                // Reschedule in one hour
                Dispatcher.Schedule(s_frequency, () => BeginCheck());
            }
        }


        /// <summary>
        /// Scans the feed returned by BC.
        /// </summary>
        /// <remarks>Invoked on some background thread.</remarks>
        /// <param name="xdoc"></param>
        private static void ScanUpdateFeed(XmlDocument xdoc)
        {
            if (xdoc.DocumentElement.Name != "evemon") return;

            XmlElement newestEl = xdoc.DocumentElement.SelectSingleNode("newest") as XmlElement;
            if (newestEl == null) return;

            Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            Version newestVersion = new Version(newestEl.SelectSingleNode("version").InnerText);
            string updateUrl = newestEl.SelectSingleNode("url").InnerText;
            string updateMessage = newestEl.SelectSingleNode("message").InnerText;

            bool canAutoInstall = false;
            string installArgs = String.Empty;
            string installUrl = String.Empty;
            string additionalArgs = String.Empty;
            XmlElement argEl = newestEl.SelectSingleNode("autopatchargs") as XmlElement;
            XmlElement iUrlEl = newestEl.SelectSingleNode("autopatchurl") as XmlElement;
            XmlElement argAdd = newestEl.SelectSingleNode("additionalargs") as XmlElement;
            if (iUrlEl != null && argEl != null)
            {
                canAutoInstall = true;
                installUrl = iUrlEl.InnerText;
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
                // Requests a notification to subscribers and quit.
                Dispatcher.BeginInvoke(() => 
                    OnUpdateAvailable(updateUrl, updateMessage, newestVersion, currentVersion, 
                    canAutoInstall, installArgs, installUrl));
                return;
            }

            // Code is up to date. Lets try the data.
            XmlElement datafilesEl = xdoc.DocumentElement as XmlElement;
            XmlSerializer xs = new XmlSerializer(typeof(DatafileVersions));
            DatafileVersions dfv = null;
            using (XmlNodeReader xnr = new XmlNodeReader(datafilesEl))
            {
                dfv = (DatafileVersions)xs.Deserialize(xnr);
            }
            if (dfv.FilesHaveChanged)
            {
                // Requests a notification to subscribers and quit.
                Dispatcher.BeginInvoke(() => OnDataUpdateAvailable(updateUrl, dfv.ChangedDataFiles));
                return;
            }
        }

        /// <summary>
        /// Notify subscribers about an update of the binaries.
        /// </summary>
        /// <remarks>Invoked on the UI thread.</remarks>
        private static void OnUpdateAvailable(string updateUrl, string updateMessage,
                                       Version newestVersion, Version currentVersion, bool canAutoInstall,
                                       string installArgs, string installUrl)
        {
            if (UpdateAvailable == null)
                return;

            UpdateAvailableEventArgs e = new UpdateAvailableEventArgs();
            e.CurrentVersion = currentVersion;
            e.NewestVersion = newestVersion;
            e.UpdateMessage = updateMessage;
            e.UpdateUrl = updateUrl;
            e.CanAutoInstall = canAutoInstall;
            e.AutoInstallUrl = installUrl;
            e.AutoInstallArguments = installArgs;
            UpdateAvailable(null, e);
        }

        /// <summary>
        /// Notify subscribers about an update of the datafiles.
        /// </summary>
        /// <remarks>Invoked on the UI thread.</remarks>
        private static void OnDataUpdateAvailable(string updateUrl, List<DatafileVersion> changedFiles)
        {
            if (DataUpdateAvailable == null)
                return;
            
            DataUpdateAvailableEventArgs e = new DataUpdateAvailableEventArgs();
            e.UpdateUrl = updateUrl;
            e.ChangedFiles = changedFiles;
            DataUpdateAvailable(null, e);
        }
    }


    #region DatafileVersions
    [XmlRoot("evemon")]
    public class DatafileVersions
    {
        [XmlArray("datafiles"), XmlArrayItem("datafile", typeof(DatafileVersion))]
        public ArrayList Datafiles = new ArrayList();

        private List<DatafileVersion> m_changedList = new List<DatafileVersion>();
        public List<DatafileVersion> ChangedDataFiles
        {
            get { return m_changedList; }
        }

        [XmlIgnore]
        public bool FilesHaveChanged
        {
            get
            {
                m_changedList.Clear();

                foreach (DatafileVersion dfv in Datafiles)
                {
                    foreach (var datafile in EveClient.Datafiles)
                    {
                        if (datafile.Filename == dfv.Name)
                        {
                            if (datafile.MD5Sum != dfv.Md5)
                            {
                                m_changedList.Add(dfv);
                            }
                            break;
                        }
                    }
                }

                return m_changedList.Count > 0;
            }

        }
    }
    #endregion


    #region DatafileVersion
    public class DatafileVersion
    {
        [XmlElement("name")]
        public string Name;

        [XmlElement("date")]
        public string DateChanged;

        [XmlElement("md5")]
        public string Md5;

        [XmlElement("message")]
        public string Message;

        [XmlElement("url")]
        public string Url;

    }
    #endregion


    #region UpdateAvailableHandler
    public delegate void UpdateAvailableHandler(object sender, UpdateAvailableEventArgs e);

    public class UpdateAvailableEventArgs
    {
        private string m_updateUrl;

        public string UpdateUrl
        {
            get { return m_updateUrl; }
            set { m_updateUrl = value; }
        }

        private string m_updateMessage;

        public string UpdateMessage
        {
            get { return m_updateMessage; }
            set { m_updateMessage = value; }
        }

        private Version m_currentVersion;

        public Version CurrentVersion
        {
            get { return m_currentVersion; }
            set { m_currentVersion = value; }
        }

        private Version m_newestVersion;

        public Version NewestVersion
        {
            get { return m_newestVersion; }
            set { m_newestVersion = value; }
        }

        private bool m_canAutoInstall = false;

        public bool CanAutoInstall
        {
            get { return m_canAutoInstall; }
            set { m_canAutoInstall = value; }
        }

        private string m_autoInstallUrl = String.Empty;

        public string AutoInstallUrl
        {
            get { return m_autoInstallUrl; }
            set { m_autoInstallUrl = value; }
        }

        private string m_autoInstallArguments = String.Empty;

        public string AutoInstallArguments
        {
            get { return m_autoInstallArguments; }
            set { m_autoInstallArguments = value; }
        }
    }
    #endregion


    #region DataUpdateAvailableEventArgs
    public delegate void DataUpdateAvailableHandler(object sender, DataUpdateAvailableEventArgs e);

    public class DataUpdateAvailableEventArgs
    {
        private string m_updateUrl;

        public string UpdateUrl
        {
            get { return m_updateUrl; }
            set { m_updateUrl = value; }
        }

        private List<DatafileVersion> m_changedFiles;

        public List<DatafileVersion> ChangedFiles
        {
            get { return m_changedFiles; }
            set { m_changedFiles = value; }
        }

    }
    #endregion
}