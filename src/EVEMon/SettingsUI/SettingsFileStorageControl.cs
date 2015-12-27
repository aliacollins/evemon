using System;
using System.Drawing;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.CloudStorageServices;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Factories;

namespace EVEMon.SettingsUI
{
    public partial class SettingsFileStorageControl : UserControl
    {
        private static bool s_queryPending;
        private static CloudStorageServiceProvider s_provider;


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


        #region Local Events

        /// <summary>
        /// Occurs when the control loads.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void SettingsFileStorageControl_Load(object sender, EventArgs e)
        {
            Font = FontFactory.GetFont("Tahoma");
            s_provider = Settings.CloudStorageServiceProvider.Provider;

            if (s_provider != null)
            {
                s_provider.CredentialsChecked += CloudStorageServiceProvider_CredentialsChecked;
                s_provider.FileUploaded += CloudStorageServiceProvider_FileUploaded;
                s_provider.FileDownloaded += CloudStorageServiceProvider_FileDownloaded;
            }
            Disposed += OnDisposed;

            alwaysUploadCheckBox.Checked = CloudStorageServicesSettings.Default.UploadAlways;
            alwaysDownloadCheckBox.Checked = CloudStorageServicesSettings.Default.DownloadAlways;
            useImmediatelyCheckBox.Checked = CloudStorageServicesSettings.Default.UseImmediately;
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
            if (s_provider != null)
            {
                s_provider.CredentialsChecked -= CloudStorageServiceProvider_CredentialsChecked;
                s_provider.FileUploaded -= CloudStorageServiceProvider_FileUploaded;
                s_provider.FileDownloaded -= CloudStorageServiceProvider_FileDownloaded;
            }
            Disposed -= OnDisposed;
        }

        /// <summary>
        /// When the control becomes visible again, we update the content.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            Enabled = Settings.CloudStorageServiceProvider.Provider != null &&
                      Settings.CloudStorageServiceProvider.Provider.HasCredentialsStored;
        }

        /// <summary>
        /// Occurs when the checkbox state changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void alwaysUploadCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CloudStorageServicesSettings.Default.UploadAlways = alwaysUploadCheckBox.Checked;
            CloudStorageServicesSettings.Default.Save();
        }

        /// <summary>
        /// Occurs when the checkbox state changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void alwaysDownloadCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            useImmediatelyCheckBox.Enabled = alwaysDownloadCheckBox.Checked;
            CloudStorageServicesSettings.Default.DownloadAlways = alwaysDownloadCheckBox.Checked;
            CloudStorageServicesSettings.Default.Save();
        }

        /// <summary>
        /// Occurs when the checkbox state changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void useImmediatelyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CloudStorageServicesSettings.Default.UseImmediately = useImmediatelyCheckBox.Checked;
            CloudStorageServicesSettings.Default.Save();
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

            if (s_queryPending)
                return;

            s_queryPending = true;
            throbber.Visible = true;
            throbber.State = ThrobberState.Rotating;

            Settings.SaveImmediate();

            EveMonClient.Trace("{0}.UploadSettingsFileAsync - Initiated", s_provider?.Name);

            s_provider?.UploadSettingsFileAsync();
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

            if (s_queryPending)
                return;

            s_queryPending = true;
            throbber.Visible = true;
            throbber.State = ThrobberState.Rotating;

            EveMonClient.Trace("{0}.DownloadSettingsFileAsync - Initiated", s_provider?.Name);

            s_provider?.DownloadSettingsFileAsync();
        }

        #endregion


        #region Global Events

        /// <summary>
        /// Occurs when the cloud storage service provider credentials get authenticated.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void CloudStorageServiceProvider_CredentialsChecked(object sender, EventArgs e)
        {
            Enabled = s_provider != null && s_provider.HasCredentialsStored;
        }

        /// <summary>
        /// Occurs when the file has been uploaded to the cloud storage service provider.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CloudStorageServiceProviderEventArgs"/> instance containing the event data.</param>
        private void CloudStorageServiceProvider_FileUploaded(object sender, CloudStorageServiceProviderEventArgs e)
        {
            s_queryPending = false;

            throbber.State = ThrobberState.Stopped;
            throbber.Visible = false;

            if (e.HasError)
            {
                apiResponseLabel.ForeColor = Color.Red;
                apiResponseLabel.Text = e.ErrorMessage;
                return;
            }

            apiResponseLabel.ForeColor = Color.Green;
            apiResponseLabel.Text = @"File uploaded successfully.";

            EveMonClient.Trace("{0}.UploadSettingsFileAsync - Completed", s_provider?.Name);
        }

        /// <summary>
        /// Occurs when the file has been dwloaded from the cloud storage service provider.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CloudStorageServiceProviderEventArgs"/> instance containing the event data.</param>
        private void CloudStorageServiceProvider_FileDownloaded(object sender, CloudStorageServiceProviderEventArgs e)
        {
            s_queryPending = false;

            throbber.State = ThrobberState.Stopped;
            throbber.Visible = false;

            if (e.HasError)
            {
                apiResponseLabel.ForeColor = Color.Red;
                apiResponseLabel.Text = e.ErrorMessage;
                return;
            }

            EveMonClient.Trace("{0}.DownloadSettingsFileAsync - Completed", s_provider?.Name);
        }

        #endregion
    }
}