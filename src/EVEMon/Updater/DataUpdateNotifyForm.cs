using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Constants;
using EVEMon.Common.Controls;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Data;
using EVEMon.Common.Helpers;
using EVEMon.Common.Serialization.PatchXml;

namespace EVEMon.Updater
{
    public partial class DataUpdateNotifyForm : EVEMonForm
    {
        private readonly DataUpdateAvailableEventArgs m_args;
        private bool m_formClosing;

        /// <summary>
        /// Default constructor.
        /// </summary>
        private DataUpdateNotifyForm()
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
            StringBuilder notes = new StringBuilder();

            notes.AppendLine("UPDATE NOTES:");

            foreach (SerializableDatafile versionDatafile in m_args.ChangedFiles)
            {
                changedFiles
                    .AppendLine($"Filename: {versionDatafile.Name}\t\tDated: {versionDatafile.Date}")
                    .AppendLine($"Url: {versionDatafile.Address}/{versionDatafile.Name}")
                    .AppendLine();

                notes
                    .AppendLine(versionDatafile.Message)
                    .AppendLine();
            }
            tbFiles.Lines = changedFiles.ToString().Split(Environment.NewLine.ToCharArray());
            tbNotes.Lines = notes.ToString().Split(Environment.NewLine.ToCharArray());
        }

        /// <summary>
        /// Occurs on "update" button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            DialogResult result = DialogResult.Yes;
            int changedFilesCount = m_args.ChangedFiles.Count;

            while (m_args.ChangedFiles.Count != 0 && result == DialogResult.Yes)
            {
                if (m_formClosing)
                    break;

                DownloadUpdates();

                if (m_args.ChangedFiles.Count == 0)
                    break;

                // One or more files failed
                string message = $"{m_args.ChangedFiles.Count} " +
                                 $"file{(m_args.ChangedFiles.Count == 1 ? String.Empty : "s")} " +
                                 "failed to download, do you wish to try again?";

                result = MessageBox.Show(message, @"Failed Download", MessageBoxButtons.YesNo);
            }

            // If no files were updated, abort the update process
            DialogResult = m_args.ChangedFiles.Count == changedFilesCount ? DialogResult.Abort : DialogResult.OK;

            Close();
        }

        /// <summary>
        /// Downloads the updates.
        /// </summary>
        private void DownloadUpdates()
        {
            List<SerializableDatafile> datafiles = new List<SerializableDatafile>();

            // Copy the new datafiles to a new list
            datafiles.AddRange(m_args.ChangedFiles);

            foreach (SerializableDatafile versionDatafile in datafiles)
            {
                // Work out the new names of the files
                string oldFilename = Path.Combine(EveMonClient.EVEMonDataDir, versionDatafile.Name);
                string newFilename = $"{oldFilename}.tmp";

                // If the file already exists delete it
                if (File.Exists(newFilename))
                    FileHelper.DeleteFile(newFilename);

                Uri url = new Uri($"{versionDatafile.Address}/{versionDatafile.Name}");

                // Show the download dialog, which will download the file
                using (UpdateDownloadForm form = new UpdateDownloadForm(url, newFilename))
                {
                    if (form.ShowDialog() != DialogResult.OK)
                        continue;

                    Datafile downloadedDatafile = new Datafile(Path.GetFileName(newFilename));

                    if (versionDatafile.MD5Sum != null && versionDatafile.MD5Sum != downloadedDatafile.MD5Sum)
                    {
                        FileHelper.DeleteFile(newFilename);
                        continue;
                    }

                    UpdateManager.ReplaceDatafile(oldFilename, newFilename);
                    m_args.ChangedFiles.Remove(versionDatafile);
                }
            }
        }

        /// <summary>
        /// Occurs on "remind me later" button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLater_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Handles the FormClosing event of the DataUpdateNotifyForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FormClosingEventArgs"/> instance containing the event data.</param>
        private void DataUpdateNotifyForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Visible ||
                (e.CloseReason != CloseReason.ApplicationExitCall && e.CloseReason != CloseReason.TaskManagerClosing &&
                 e.CloseReason != CloseReason.WindowsShutDown))
            {
                return;
            }

            m_formClosing = true;
        }
    }
}