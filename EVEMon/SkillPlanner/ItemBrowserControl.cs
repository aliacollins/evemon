using System;
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
            scDetails.Panel2MinSize = (reqSkillPanelMinWidth > reqSkillControlMinWidth
                                           ? reqSkillPanelMinWidth
                                           : reqSkillControlMinWidth);
        }

        #endregion
    }
}