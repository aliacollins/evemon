using System;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.MarketUnifiedUploader;

namespace EVEMon.SettingsUI
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MarketUnifiedUploaderControl : UserControl
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public MarketUnifiedUploaderControl()
        {
            InitializeComponent();
        }

        #endregion


        #region Inherited Events

        /// <summary>
        /// On load, subscribe events.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (DesignMode || this.IsDesignModeHosted())
                return;

            EndPointsCheckedListBox.Focus();
            EndPointsCheckedListBox.SendToBack();

            Uploader.StatusChanged += Uploader_StatusChanged;
            Uploader.EndPointsUpdated += Uploader_EndPointsUpdated;
            Uploader.ProgressTextChanged += Uploader_ProgressTextChanged;
            Disposed += OnDisposed;

            UpdateEndPointsList();
        }

        /// <summary>
        /// On dispose unsubscribe the events.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnDisposed(object sender, EventArgs e)
        {
            Uploader.StatusChanged -= Uploader_StatusChanged;
            Uploader.EndPointsUpdated -= Uploader_EndPointsUpdated;
            Uploader.ProgressTextChanged -= Uploader_ProgressTextChanged;
            Disposed -= OnDisposed;
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// Handles the StatusChanged event of the Uploader control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Uploader_StatusChanged(object sender, EventArgs e)
        {
            if (Uploader.Status != UploaderStatus.Initializing)
                return;

            NoEndPointsLabel.Text = "Looking for available online endpoints.";
            EndPointsCheckedListBox.Visible = false;
        }

        /// <summary>
        /// Handles the EndPointsUpdated event of the Uploader control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Uploader_EndPointsUpdated(object sender, EventArgs e)
        {
            UpdateEndPointsList();
        }

        /// <summary>
        /// Handles the ProgressTextChanged event of the Uploader control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Uploader_ProgressTextChanged(object sender, EventArgs e)
        {
            if (ProgressTextBox.Lines.Length > 1000)
                ProgressTextBox.ResetText();

            ProgressTextBox.AppendText(Uploader.ProgressText);
            ProgressTextBox.SelectionStart = ProgressTextBox.Text.Length;
            ProgressTextBox.ScrollToCaret();
        }


        #endregion


        #region Helper Methods

        /// <summary>
        /// Updates the endpoints list.
        /// </summary>
        private void UpdateEndPointsList()
        {
            // Display the informative label if uploader isn't enabled
            if (!Settings.MarketUnifiedUploader.Enabled)
                return;

            EndPointsCheckedListBox.Items.Clear();

            if (!Uploader.EndPoints.Any())
            {
                NoEndPointsLabel.Text = "No endpoints are available.";
                EndPointsCheckedListBox.Visible = false;
                return;
            }

            foreach (EndPoint endPoint in Uploader.EndPoints)
            {
                EndPointsCheckedListBox.Items.Add(endPoint.Name, endPoint.Enabled);
            }

            EndPointsCheckedListBox.Visible = Uploader.EndPoints.Any();
            EndPointsCheckedListBox.BringToFront();
        }

        /// <summary>
        /// Shows the info label.
        /// </summary>
        internal void ShowInfoLabel()
        {
            NoEndPointsLabel.Text = "Enable the uploader and click \"Apply\" to fetch online endpoints.";
            EndPointsCheckedListBox.Visible = false;
        }


        #endregion


        #region Exportation

        /// <summary>
        /// Updates the settings for the endpoints.
        /// </summary>
        internal void UpdateEndPointSettings()
        {
            // Quit if the list is empty
            if (EndPointsCheckedListBox.Items.Count == 0)
                return;

            foreach (string item in EndPointsCheckedListBox.Items.Cast<string>())
            {
                int index = EndPointsCheckedListBox.Items.IndexOf(item);
                bool isChecked = EndPointsCheckedListBox.GetItemChecked(index);
                Uploader.EndPoints.First(endpoint => endpoint.Name == item).Enabled = isChecked;
            }
            EndPointCollection.UpdateEndPointSettings();
        }

        #endregion

    }
}
