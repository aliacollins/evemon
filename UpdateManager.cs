using System;
using System.Reflection;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using EVEMon.Common;
using System.Collections.Generic;
using EVEMon.Common.Net;


namespace EVEMon
{
    public class UpdateManager
    {
        private UpdateManager()
        {
        }

        private static UpdateManager m_updateManager = null;

        public static UpdateManager GetInstance()
        {
            if (m_updateManager == null)
            {
                m_updateManager = new UpdateManager();
            }
            return m_updateManager;
        }

        private Timer m_timer = null;
        private object m_lockObject = new object();
        private bool m_running = false;

        public bool IsRunning
        {
            get { return m_running; }
            set { m_running = value; }
        }


        public void Start()
        {
            lock (m_lockObject)
            {
                m_timer = new Timer(new TimerCallback(TimerTrigger));
                m_timer.Change(10, -1);
                m_running = true;
            }
        }

        public void Stop()
        {
            lock (m_lockObject)
            {
                m_timer.Change(-1, -1);
                m_timer.Dispose();
                m_timer = null;
                m_running = false;
            }
        }

        private const string UPDATE_URL = "http://evemon.battleclinic.com/patch.xml";

        private void TimerTrigger(object state)
        {
            lock (m_lockObject)
            {
                if (!m_running)
                {
                    return;
                }

                try
                {
                    Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

                    XmlDocument xdoc;
                    try
                    {
                        xdoc = CommonContext.HttpWebService.DownloadXml(UPDATE_URL + "?ver=" + currentVersion);
                    }
                    catch (Exception e)
                    {
                        ExceptionHandler.LogException(e, true);
                        return;
                    }

                    if (xdoc.DocumentElement.Name != "evemon")
                    {
                        return;
                    }

                    XmlElement newestEl = xdoc.DocumentElement.SelectSingleNode("newest") as XmlElement;
                    if (newestEl != null)
                    {
                        Version newestVersion = new Version(newestEl.SelectSingleNode("version").InnerText);
                        string updateUrl = newestEl.SelectSingleNode("url").InnerText;
                        string updateMessage = newestEl.SelectSingleNode("message").InnerText;

                        bool canAutoInstall = false;
                        string installArgs = String.Empty;
                        string installUrl = String.Empty;
                        XmlElement argEl = newestEl.SelectSingleNode("autopatchargs") as XmlElement;
                        XmlElement iUrlEl = newestEl.SelectSingleNode("autopatchurl") as XmlElement;
                        if (iUrlEl != null && argEl != null)
                        {
                            canAutoInstall = true;
                            installUrl = iUrlEl.InnerText;
                            installArgs = argEl.InnerText;
                        }

                        if (newestVersion > currentVersion)
                        {
                            // Use ThreadPool to avoid deadlock if the callback tries to
                            // call Stop() on the UpdateManager.
                            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
                                                                              {
                                                                                  OnUpdateAvailable(updateUrl,
                                                                                                    updateMessage,
                                                                                                    newestVersion,
                                                                                                    currentVersion,
                                                                                                    canAutoInstall,
                                                                                                    installArgs,
                                                                                                    installUrl);
                                                                              }));
                        }
                        // Code is up to date. Lets try the data.
                        else
                        {
                            XmlElement datafilesEl = xdoc.DocumentElement as XmlElement;
                            XmlSerializer xs = new XmlSerializer(typeof(DatafileVersions));
                            DatafileVersions dfv = null;
                            using (XmlNodeReader xnr = new XmlNodeReader(datafilesEl))
                            {
                                dfv = (DatafileVersions)xs.Deserialize(xnr);
                            }
                            if (dfv.FilesHaveChanged)
                            {
                                // Use ThreadPool to avoid deadlock if the callback tries to
                                // call Stop() on the UpdateManager.
                                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
                                                                                  {
                                                                                      OnDataUpdateAvailable(updateUrl, dfv.ChangedDatafiles);
                                                                                  }));
                            }
                        }
                    }
                }
                finally
                {
                    m_timer.Change(Convert.ToInt32(TimeSpan.FromHours(2).TotalMilliseconds), -1);
                }
            }
        }

        public event UpdateAvailableHandler UpdateAvailable;

        private void OnUpdateAvailable(string updateUrl, string updateMessage,
                                       Version newestVersion, Version currentVersion, bool canAutoInstall,
                                       string installArgs, string installUrl)
        {
            if (UpdateAvailable != null)
            {
                UpdateAvailableEventArgs e = new UpdateAvailableEventArgs();
                e.CurrentVersion = currentVersion;
                e.NewestVersion = newestVersion;
                e.UpdateMessage = updateMessage;
                e.UpdateUrl = updateUrl;
                e.CanAutoInstall = canAutoInstall;
                e.AutoInstallUrl = installUrl;
                e.AutoInstallArguments = installArgs;
                UpdateAvailable(this, e);
            }
        }

        public event DataUpdateAvailableHandler DataUpdateAvailable;

        private void OnDataUpdateAvailable(string updateUrl, List<DatafileVersion> changedFiles) 
        {
            if (DataUpdateAvailable != null)
            {
                DataUpdateAvailableEventArgs e = new DataUpdateAvailableEventArgs();
                e.UpdateUrl = updateUrl;
                e.ChangedFiles = changedFiles;
                DataUpdateAvailable(this, e);
            }
        }
    }


    [XmlRoot("evemon")]
    public class DatafileVersions
    {
        [XmlArray("datafiles"),XmlArrayItem("datafile", typeof(DatafileVersion))]
        public System.Collections.ArrayList Datafiles =
           new System.Collections.ArrayList();

        private  List<DatafileVersion> m_changedList=new List<DatafileVersion>();
        public List<DatafileVersion> ChangedDatafiles
        {
            get { return m_changedList; }
        }

        [XmlIgnore]
        public bool FilesHaveChanged
        {
            get 
            {
                List<Pair<string, string>> expectedMD5List = Settings.GetInstance().DatafileChecksums;
                foreach (Object o in Datafiles)
                {
                    DatafileVersion dfv = o as DatafileVersion;
                    foreach(Pair<string,string> expectedMD5 in expectedMD5List)
                    {
                        if (expectedMD5.A == dfv.Name)
                        {
                            if (expectedMD5.B != dfv.Md5)
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

}