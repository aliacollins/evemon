using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.CustomEventArgs;

namespace EVEMon
{
    public partial class UpdateNotifyForm : EVEMonForm
    {
        private UpdateAvailableEventArgs m_args;

        /// <summary>
        /// Default constructor.
        /// </summary>
        private UpdateNotifyForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public UpdateNotifyForm(UpdateAvailableEventArgs args)
            : this()
        {
            m_args = args;
        }

        /// <summary>
        /// Occurs on "ignore" button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnIgnore_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Are you sure you want to ignore this update? You will not " +
                                              "be prompted again until a newer version is released.",
                                              "Ignore Update?",
                                              MessageBoxButtons.YesNo,
                                              MessageBoxIcon.Question,
                                              MessageBoxDefaultButton.Button2);
            if (dr == DialogResult.No)
                return;

            Settings.Updates.MostRecentDeniedUpgrade = m_args.NewestVersion.ToString();
            Settings.Save();
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Occurs on "update" button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (cbAutoInstall.Enabled && cbAutoInstall.Checked)
            {
                DialogResult result = DialogResult.Yes;
                while (!DownloadUpdate() && result == DialogResult.Yes)
                {
                    // File download failed
                    string message = String.Format(CultureConstants.DefaultCulture,
                                                   "File failed to download correctly, do you wish to try again?");

                    result = MessageBox.Show(message, "Failed Download", MessageBoxButtons.YesNo);
                }
            }
            else
            {
                Util.OpenURL(m_args.ForumUrl);
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        /// <summary>
        /// Downloads the update.
        /// </summary>
        private bool DownloadUpdate()
        {
            string filename = Path.GetFileName(m_args.InstallerUrl);
            if (filename == null)
                return false;

            string localFilename = Path.Combine(EveMonClient.EVEMonDataDir, filename);

            // If the file already exists delete it
            if (File.Exists(localFilename))
                File.Delete(localFilename);

            using (UpdateDownloadForm form = new UpdateDownloadForm(m_args.InstallerUrl, localFilename))
            {
                form.ShowDialog();

                if (form.DialogResult == DialogResult.OK)
                {
                    string downloadedFileMD5Sum = Util.CreateMD5From(localFilename);
                    if (m_args.MD5Sum != null && m_args.MD5Sum != downloadedFileMD5Sum)
                        return false;

                    ExecutePatcher(localFilename, m_args.AutoInstallArguments);
                }
            }
            return true;
        }

        /// <summary>
        /// Initiates the auto installer.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="args"></param>
        private void ExecutePatcher(string filename, string args)
        {
            try
            {
                Process.Start(filename, args);
            }
            catch (Exception e)
            {
                ExceptionHandler.LogRethrowException(e);
                if (File.Exists(filename))
                {
                    try
                    {
                        File.Delete(filename);
                    }
                    catch
                    {
                        ExceptionHandler.LogException(e, false);
                    }
                }
                throw;
            }
            DialogResult = DialogResult.OK;
            Close();
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
        /// On form shown we subcribe the event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateNotifyForm_Shown(object sender, EventArgs e)
        {
            UpdateInformation();
            cbAutoInstall.Checked = m_args.CanAutoInstall;
            EveMonClient.UpdateAvailable += UpdateNotifyForm_UpdateAvailable;
        }

        /// <summary>
        /// When an update is available, we update the informations.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateNotifyForm_UpdateAvailable(object sender, UpdateAvailableEventArgs e)
        {
            m_args = e;
            UpdateInformation();
        }

        /// <summary>
        /// Initilizes the contents and state of the controls.
        /// </summary>
        private void UpdateInformation()
        {
            // Set the basic update information
            StringBuilder labelText = new StringBuilder();
            labelText.AppendLine("An EVEMon update is available.");
            labelText.AppendLine();
            labelText.AppendFormat(CultureConstants.DefaultCulture, "Current version: {0}{1}", m_args.CurrentVersion,
                                   Environment.NewLine);
            labelText.AppendFormat(CultureConstants.DefaultCulture, "Newest version: {0}{1}", m_args.NewestVersion,
                                   Environment.NewLine);
            labelText.AppendLine("The newest version has the following updates:");
            label1.Text = labelText.ToString();

            // Set the detailed update information (from the XML)
            string updMessage = m_args.UpdateMessage;
            updMessage = updMessage.Replace("\r", String.Empty);
            textBox1.Lines = updMessage.Split('\n');

            cbAutoInstall.Enabled = m_args.CanAutoInstall;
        }

        /// <summary>
        /// On form closed we unsuscribe the event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateNotifyForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            EveMonClient.UpdateAvailable -= UpdateNotifyForm_UpdateAvailable;
        }
    }
}