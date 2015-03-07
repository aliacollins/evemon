using System;
using System.Drawing;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Serialization.BattleClinic;

namespace EVEMon.SettingsUI
{
    public partial class SettingsFileStorageControl : UserControl
    {
        private bool m_queryPending;


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsFileStorageControl"/> class.
        /// </summary>
        public SettingsFileStorageControl()
        {
            InitializeComponent();
            apiResponseLabel.Font = FontFactory.GetFont("Tahoma", FontStyle.Bold);
            apiResponseLabel.Text = String.Empty;
            throbber.Visible = false;
            throbber.BringToFront();
        }

        #endregion


        #region Global Events

        /// <summary>
        /// Occurs when the BattleClinic API credentials get authenticated.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="BCAPIEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_BCAPICredentialsUpdated(object sender, BCAPIEventArgs e)
        {
            Enabled = BCAPI.IsAuthenticated;
        }

        #endregion


        #region Local Events

        /// <summary>
        /// Occurs when the control loads.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void SettingsFileStorageControl_Load(object sender, EventArgs e)
        {
            Font = FontFactory.GetFont("Tahoma");

            EveMonClient.BCAPICredentialsUpdated += EveMonClient_BCAPICredentialsUpdated;
            Disposed += OnDisposed;

            alwaysUploadCheckBox.Checked = BCAPISettings.Default.UploadAlways;
            alwaysDownloadCheckBox.Checked = BCAPISettings.Default.DownloadAlways;
            useImmediatelyCheckBox.Checked = BCAPISettings.Default.UseImmediately;
            useImmediatelyCheckBox.Enabled = alwaysDownloadCheckBox.Checked;
            Enabled = false;
        }

        /// <summary>
        /// Occurs when the control gets disposed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnDisposed(object sender, EventArgs e)
        {
            EveMonClient.BCAPICredentialsUpdated -= EveMonClient_BCAPICredentialsUpdated;
            Disposed -= OnDisposed;
        }

        /// <summary>
        /// Occurs when the checkbox state changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void alwaysUploadCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            BCAPISettings.Default.UploadAlways = alwaysUploadCheckBox.Checked;
            BCAPISettings.Default.Save();
        }

        /// <summary>
        /// Occurs when the checkbox state changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void alwaysDownloadCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            useImmediatelyCheckBox.Enabled = alwaysDownloadCheckBox.Checked;
            BCAPISettings.Default.DownloadAlways = alwaysDownloadCheckBox.Checked;
            BCAPISettings.Default.Save();
        }

        /// <summary>
        /// Occurs when the checkbox state changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void useImmediatelyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            BCAPISettings.Default.UseImmediately = useImmediatelyCheckBox.Checked;
            BCAPISettings.Default.Save();
        }

        /// <summary>
        /// Occurs when clicking the "Upload settings file" button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void uploadSettingsFileButton_Click(object sender, EventArgs e)
        {
            apiResponseLabel.ForeColor = SystemColors.ControlText;
            apiResponseLabel.Text = String.Empty;

            if (m_queryPending)
                return;

            m_queryPending = true;
            throbber.Visible = true;
            throbber.State = ThrobberState.Rotating;

            Settings.SaveImmediate();

            EveMonClient.Trace("BCAPI.UploadSettingsFile - Initiated");

            BCAPI.FileSaveAsync(OnFileSave);
        }

        /// <summary>
        /// Occurs when clicking the "Download settings file" button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void downloadSettingsFileButton_Click(object sender, EventArgs e)
        {
            apiResponseLabel.ForeColor = SystemColors.ControlText;
            apiResponseLabel.Text = String.Empty;

            if (m_queryPending)
                return;

            m_queryPending = true;
            throbber.Visible = true;
            throbber.State = ThrobberState.Rotating;

            EveMonClient.Trace("BCAPI.DownloadSettingsFile - Initiated");

            BCAPI.FileGetByNameAsync(OnFileGetByName);
        }

        #endregion


        #region Querying helpers

        /// <summary>
        /// On settings file uploaded we inform the user.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="errorMessage">The error message.</param>
        private void OnFileSave(BCAPIResult<SerializableBCAPIFiles> result, string errorMessage)
        {
            m_queryPending = false;

            throbber.State = ThrobberState.Stopped;
            throbber.Visible = false;
            apiResponseLabel.ForeColor = Color.Red;

            if (!String.IsNullOrEmpty(errorMessage))
            {
                apiResponseLabel.Text = errorMessage;
                return;
            }

            if (result.HasError)
            {
                apiResponseLabel.Text = result.Error.ErrorMessage;
                return;
            }

            apiResponseLabel.ForeColor = Color.Green;
            apiResponseLabel.Text = "File uploaded successfully.";

            EveMonClient.Trace("BCAPI.UploadSettingsFile - Completed");
        }

        /// <summary>
        /// On settings file downloaded we inform the user.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="errorMessage">The error message.</param>
        private void OnFileGetByName(BCAPIResult<SerializableBCAPIFiles> result, string errorMessage)
        {
            m_queryPending = false;

            throbber.State = ThrobberState.Stopped;
            throbber.Visible = false;
            apiResponseLabel.ForeColor = Color.Red;

            if (!String.IsNullOrEmpty(errorMessage))
            {
                apiResponseLabel.Text = errorMessage;
                return;
            }

            if (result.HasError)
            {
                apiResponseLabel.Text = result.Error.ErrorMessage;
                return;
            }

            EveMonClient.Trace("BCAPI.DownloadSettingsFile - Completed");

            BCAPI.SaveSettingsFile(result.Result.Files[0]);
        }

        #endregion
    }
}