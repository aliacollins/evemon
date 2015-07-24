using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Serialization.Settings;
using EVEMon.Common.SettingsObjects;
using EVEMon.MarketUnifiedUploader;

namespace EVEMon.SettingsUI
{
    public partial class MarketUnifiedUploaderControl : UserControl
    {
        private MarketUnifiedUploaderSettings m_marketUnifiedUploaderSettings;
        private List<SerializableLocalhostEndPoint> m_localhosts;


        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public MarketUnifiedUploaderControl()
        {
            InitializeComponent();
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        internal MarketUnifiedUploaderSettings Settings
        {
            get { return m_marketUnifiedUploaderSettings; }
            set
            {
                if (value == null || m_marketUnifiedUploaderSettings == value)
                    return;

                m_marketUnifiedUploaderSettings = value;
                m_localhosts = m_marketUnifiedUploaderSettings.EndPoints.OfType<SerializableLocalhostEndPoint>().ToList();
                InitializeLocalhostEndPointsDropDown();
                UpdateEndPointsList();
            }
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

            InitializeLocalhostEndPointsDropDown();
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


        /// <summary>
        /// Handles the Click event of the AddButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void AddButton_Click(object sender, EventArgs e)
        {
            SerializableLocalhostEndPoint newLocalhostEndPoint = new SerializableLocalhostEndPoint
                                                                     {
                                                                         Method = HttpMethod.Post,
                                                                         UploadKey = String.Empty,
                                                                         DataCompression = DataCompression.None,
                                                                         Enabled = true
                                                                     };

            using (LocalhostSettingsForm localhostSettingsForm = new LocalhostSettingsForm(m_localhosts, newLocalhostEndPoint))
            {
                DialogResult result = localhostSettingsForm.ShowDialog();
                if (result != DialogResult.OK)
                    return;

                m_localhosts.Add(newLocalhostEndPoint);
                InitializeLocalhostEndPointsDropDown();
                LocalhostComboBox.SelectedIndex = LocalhostComboBox.Items.Count - 1;
            }
        }

        /// <summary>
        /// Handles the Click event of the EditButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EditButton_Click(object sender, EventArgs e)
        {
            // Search for the localhost with the selected name
            SerializableLocalhostEndPoint localhostEndPoint =
                m_localhosts.First(provider => provider.Name == (string)LocalhostComboBox.SelectedItem);

            // Open the config form for this provider
            using (LocalhostSettingsForm localhostSettingsForm = new LocalhostSettingsForm(m_localhosts, localhostEndPoint))
            {
                localhostSettingsForm.ShowDialog();
            }
        }

        /// <summary>
        /// Handles the Click event of the DeleteButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void DeleteButton_Click(object sender, EventArgs e)
        {
            string localhostName = (string)LocalhostComboBox.SelectedItem;
            DialogResult result =
                MessageBox.Show(
                    String.Format(CultureConstants.DefaultCulture, "Delete localhost \"{0}\"?", localhostName),
                    "Delete localhost?", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);

            if (result != DialogResult.Yes)
                return;

            // Search for the localhost with the selected name
            SerializableLocalhostEndPoint localhostToRemove =
                m_localhosts.FirstOrDefault(localhost => localhostName == localhost.Name);

            // Remove it
            if (localhostToRemove == null)
                return;

            m_localhosts.Remove(localhostToRemove);
            InitializeLocalhostEndPointsDropDown();
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Updates the endpoints list.
        /// </summary>
        private void UpdateEndPointsList()
        {
            if (m_marketUnifiedUploaderSettings == null)
                return;

            // Display the informative label if uploader isn't enabled
            if (!m_marketUnifiedUploaderSettings.Enabled)
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

        /// <summary>
        /// Populates the combobox for Localhost EndPoints.
        /// </summary>
        private void InitializeLocalhostEndPointsDropDown()
        {
            if (m_marketUnifiedUploaderSettings == null)
                return;

            LocalhostComboBox.Items.Clear();
            foreach (SerializableLocalhostEndPoint localhost in m_localhosts)
            {
                LocalhostComboBox.Items.Add(localhost.Name);
            }

            LocalhostComboBox.Enabled = LocalhostComboBox.Items.Count > 0;

            // Selects the first local host if any
            if (LocalhostComboBox.Enabled)
                LocalhostComboBox.SelectedIndex = 0;

            EditButton.Enabled = LocalhostComboBox.Items.Count > 0;
            DeleteButton.Enabled = LocalhostComboBox.Items.Count > 0;
        }

        #endregion


        #region Exportation

        /// <summary>
        /// Updates the settings for the endpoints.
        /// </summary>
        internal void UpdateEndPointSettings()
        {
            // Quit if the list is empty
            if (!m_localhosts.Any() && EndPointsCheckedListBox.Items.Count == 0)
                return;

            // Update endpoints 
            foreach (string item in EndPointsCheckedListBox.Items.Cast<string>())
            {
                int index = EndPointsCheckedListBox.Items.IndexOf(item);
                bool isChecked = EndPointsCheckedListBox.GetItemChecked(index);
                
                EndPoint endPoint = Uploader.EndPoints.FirstOrDefault(endpoint => endpoint.Name == item);
                if (endPoint != null)
                    endPoint.Enabled = isChecked;

                SerializableLocalhostEndPoint localhost = m_localhosts.FirstOrDefault(endpoint => endpoint.Name == item);
                if (localhost != null)
                    localhost.Enabled = isChecked;
            }

            // Update localhost endpoints
            // (retain this order of updating as this method triggers the endpoint list updating)
            Uploader.EndPoints.UpdateLocalhosts(m_localhosts);
            
            // Update the endpoints settings
            Uploader.EndPoints.UpdateSettings();
        }

        #endregion
    }
}
