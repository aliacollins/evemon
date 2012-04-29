using System;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common.Threading;
using EVEMon.MarketUnifiedUploader;

namespace EVEMon.SettingsUI
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MarketUnifiedUploaderControl : UserControl
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MarketUnifiedUploaderControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// On load, subscribe events.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (DesignMode || this.IsDesignModeHosted())
                return;

            EndPointsCheckedListView.Focus();
            EndPointsCheckedListView.SendToBack();

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
            Uploader.EndPointsUpdated -= Uploader_EndPointsUpdated;
            Uploader.ProgressTextChanged -= Uploader_ProgressTextChanged;
            Disposed -= OnDisposed;
        }

        /// <summary>
        /// Handles the EndPointsUpdated event of the Uploader control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Uploader_EndPointsUpdated(object sender, EventArgs e)
        {
            Dispatcher.Invoke(UpdateEndPointsList);
        }

        /// <summary>
        /// Handles the ProgressTextChanged event of the Uploader control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Uploader_ProgressTextChanged(object sender, EventArgs e)
        {
            Dispatcher.Invoke(UpdateProgressText);
        }

        /// <summary>
        /// Handles the ItemSelectionChanged event of the EndpointsCheckedListView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.ListViewItemSelectionChangedEventArgs"/> instance containing the event data.</param>
        private void EndpointsCheckedListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (!e.IsSelected)
                return;

            e.Item.Checked = !e.Item.Checked;
            Uploader.EndPoints.First(x => x.Name == e.Item.Text).Enabled = e.Item.Checked;

            // Update the settings
            EndPointCollection.UpdateEndPointSettings();
        }

        /// <summary>
        /// Updates the endpoints list.
        /// </summary>
        private void UpdateEndPointsList()
        {
            EndPointsCheckedListView.Items.Clear();

            if (!Uploader.EndPoints.Any())
            {
                NoEndPointsLabel.Text = "No endpoints are available.";
                EndPointsCheckedListView.Visible = false;
                return;
            }

            foreach (EndPoint endPoint in Uploader.EndPoints)
            {
                EndPointsCheckedListView.Items.Add(new ListViewItem(endPoint.Name)
                                                       {
                                                           Checked = endPoint.Enabled,
                                                       });
            }

            EndPointsCheckedListView.Visible = Uploader.EndPoints.Any();
            EndPointsCheckedListView.BringToFront();
        }

        /// <summary>
        /// Updates the progress text.
        /// </summary>
        private void UpdateProgressText()
        {
            if (ProgressTextBox.Lines.Length > 1000)
                ProgressTextBox.ResetText();

            ProgressTextBox.AppendText(Uploader.ProgressText);
            ProgressTextBox.SelectionStart = ProgressTextBox.Text.Length;
            ProgressTextBox.ScrollToCaret();
        }
    }
}
