using System;
using System.Windows.Forms;
using EVEMon.Common.Controls;
using EVEMon.Common.Helpers;

namespace EVEMon.SkillPlanner
{
    internal partial class ItemBrowserControl : EveObjectBrowserControl
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ItemBrowserControl()
        {
            InitializeComponent();
            scObjectBrowser.RememberDistanceKey = "ItemBrowser_Left";
            SelectControl = itemSelectControl;
            PropertiesList = lvItemProperties;

            PropertiesList.MouseDown += PropertiesList_MouseDown;
            PropertiesList.MouseMove += PropertiesList_MouseMove;
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// Exports item info to CSV format.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportToCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewExporter.CreateCSV(PropertiesList, true);
        }

        /// <summary>
        /// When the mouse gets pressed, we change the cursor.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void PropertiesList_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            PropertiesList.Cursor = Cursors.Default;
        }

        /// <summary>
        /// When the mouse moves over the list, we change the cursor.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void PropertiesList_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                return;

            PropertiesList.Cursor = CustomCursors.ContextMenu;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Updates the controls when the selection is changed.
        /// </summary>
        protected override void OnSelectionChanged()
        {
            base.OnSelectionChanged();
            if (SelectedObject == null)
                return;

            // Description
            tbDescription.Text = SelectedObject.Description;

            // Required Skills
            requiredSkillsControl.Object = SelectedObject;
        }

        /// <summary>
        /// Updates whenever the selected plan changed.
        /// </summary>
        protected override void OnSelectedPlanChanged()
        {
            base.OnSelectedPlanChanged();
            requiredSkillsControl.Plan = Plan;

            // We recalculate the right panels minimum size
            int reqSkillControlMinWidth = requiredSkillsControl.MinimumSize.Width;
            int reqSkillPanelMinWidth = scDetails.Panel2MinSize;
            scDetails.Panel2MinSize = reqSkillPanelMinWidth > reqSkillControlMinWidth
                ? reqSkillPanelMinWidth
                : reqSkillControlMinWidth;
        }

        #endregion
    }
}