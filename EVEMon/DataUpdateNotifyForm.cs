using System;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using EVEMon.Common;
using EVEMon.Common.Controls;
using System.Globalization;

namespace EVEMon
{
    public partial class DataUpdateNotifyForm : EVEMonForm
    {
        private DataUpdateAvailableEventArgs m_args;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DataUpdateNotifyForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public DataUpdateNotifyForm(DataUpdateAvailableEventArgs args)
            : this()
        {
            m_args = args;
        }

        /// <summary>
        /// On load we update the informations.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataUpdateNotifyForm_Load(object sender, EventArgs e)
        {
            StringBuilder changedFiles = new StringBuilder();
            StringBuilder notes = new StringBuilder("UPDATE NOTES:\n");
            foreach (DatafileVersion dfv in m_args.ChangedFiles)
            {
                changedFiles.AppendFormat(CultureConstants.DefaultCulture, "Filename: {0}\t\tDated: {1}{3}Url: {2}/{0}{3}{3}", dfv.Name, dfv.DateChanged, dfv.Url, Environment.NewLine);
                notes.AppendLine(dfv.Message);
            }
            tbFiles.Lines = changedFiles.ToString().Split('\n');
            tbNotes.Lines = notes.ToString().Split('\n');
        }

        /// <summary>
        /// Occurs on "update" button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            DialogResult result = DialogResult.Yes;

            while (m_args.ChangedFiles.Count != 0 && result == DialogResult.Yes)
            {
                DownloadUpdates();

                if (m_args.ChangedFiles.Count == 0)
                    break;

                // one or more files failed
                string message = String.Format(
                    CultureConstants.DefaultCulture, 
                    "{0} file{1} failed to download, do you wish to try again?", 
                    m_args.ChangedFiles.Count, m_args.ChangedFiles.Count == 1 ? String.Empty : "s");

                result = MessageBox.Show(message, "Failed Download", MessageBoxButtons.YesNo);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void DownloadUpdates()
        {
            List<DatafileVersion> datafiles = new List<DatafileVersion>();

            // copy the list of datafiles
            m_args.ChangedFiles.ForEach(x => datafiles.Add(x));

            foreach (var dfv in datafiles)
            {
                // work out the new names of the files
                string urn = String.Format(CultureConstants.DefaultCulture, "{0}/{1}", dfv.Url, dfv.Name);
                string oldFilename = Path.Combine(EveClient.EVEMonDataDir, dfv.Name);
                string newFilename = String.Format(CultureConstants.DefaultCulture, "{0}.tmp", oldFilename);

                // if the file already exists delete it
                if (File.Exists(newFilename))
                    File.Delete(newFilename);

                // show the download dialog, which will download the file
                using (UpdateDownloadForm f = new UpdateDownloadForm(urn, newFilename))
                {
                    if (f.ShowDialog() == DialogResult.OK)
                    {
                        string filename = Path.GetFileName(newFilename);
                        Datafile datafile = new Datafile(filename);

                        if (datafile.MD5Sum != dfv.Md5)
                        {
                            File.Delete(newFilename);
                            continue;
                        }

                        ReplaceDatafile(oldFilename, newFilename);
                        m_args.ChangedFiles.Remove(dfv);
                    }
                }
            }
        }

        private static void ReplaceDatafile(string oldFilename, string newFilename)
        {
            try
            {
                File.Delete(oldFilename + ".bak");
                File.Copy(oldFilename, oldFilename + ".bak");
                File.Delete(oldFilename);
                File.Move(newFilename, oldFilename);
            }
            catch (IOException ex)
            {
                ExceptionHandler.LogException(ex, true);
            }
        }

        /// <summary>
        /// Occurs on "remind me later" button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLater_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}