using System;
using System.Drawing;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon.SkillPlanner
{
    public partial class ShipBrowserControl : EveObjectBrowserControl
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ShipBrowserControl()
        {
            InitializeComponent();
            scObjectBrowser.RememberDistanceKey = "ShipsBrowser_Left";
            SelectControl = shipSelectControl;
            PropertiesList = lvShipProperties;
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// Opens the BattleClinic Loadout window.
        /// </summary>
        private void lblBattleclinic_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShipLoadoutSelectWindow window = WindowsFactory<ShipLoadoutSelectWindow>.ShowByTag(Plan);
            window.Ship = SelectedObject;
        }

        /// <summary>
        /// Exports item info to CSV format.
        /// </summary>
        private void exportToCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewExporter.CreateCSV(PropertiesList);
        }

        #endregion


        #region Inherited Events

        /// <summary>
        /// Occurs when the settings changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void EveMonClient_SettingsChanged(object sender, EventArgs e)
        {
            base.EveMonClient_SettingsChanged(sender, e);
            UpdateControlVisibility();
        }

        /// <summary>
        /// Occurs when the conrol loads.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            UpdateControlVisibility();
        }

        /// <summary>
        /// Updates the controls when the selection is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnSelectionChanged(object sender, EventArgs e)
        {
            base.OnSelectionChanged(sender, e);
            if (SelectedObject == null)
                return;

            // Description
            tbDescription.Text = SelectedObject.Description;

            // Recommended Certificates
            recommendedCertificatesControl.Object = SelectedObject;

            // Required Skills
            requiredSkillsControl.Object = SelectedObject;

            ShipLoadoutSelectWindow loadoutSelect = WindowsFactory<ShipLoadoutSelectWindow>.GetByTag(Plan);
            if (loadoutSelect != null)
                loadoutSelect.Ship = shipSelectControl.SelectedObject;
        }

        /// <summary>
        /// Updates the plan when the selection is changed.
        /// </summary>
        protected override void OnPlanChanged()
        {
            base.OnPlanChanged();
            recommendedCertificatesControl.Plan = Plan;
            requiredSkillsControl.Plan = Plan;

            // We recalculate the right panels minimum size
            int reqSkillControlMinWidth = requiredSkillsControl.MinimumSize.Width;
            int reqSkillPanelMinWidth = scDetails.Panel2MinSize;
            scDetails.Panel2MinSize = (reqSkillPanelMinWidth > reqSkillControlMinWidth
                                           ? reqSkillPanelMinWidth
                                           : reqSkillControlMinWidth);
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Updates the contol visibility.
        /// </summary>
        private void UpdateControlVisibility()
        {
            lblBattleclinic.Location = Settings.UI.SafeForWork
                                           ? new Point(Pad, lblBattleclinic.Location.Y)
                                           : new Point(eoImage.Width + Pad * 2, lblBattleclinic.Location.Y);
        }

        #endregion
    }
}