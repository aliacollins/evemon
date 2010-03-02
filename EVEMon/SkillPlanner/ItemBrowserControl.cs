using System;

namespace EVEMon.SkillPlanner
{
    public partial class ItemBrowserControl : EveObjectBrowserControl
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ItemBrowserControl()
        {
            InitializeComponent();
            this.scObjectBrowser.RememberDistanceKey = "ItemBrowser_Left";
            this.Initialize(lvItemProperties, itemSelectControl, false);
        }

        #region Event Handlers
        /// <summary>
        /// Updates Required Skills control when selected plan is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveObjectBrowserSimple_PlanChanged(object sender, EventArgs e)
        {
            this.requiredSkillsControl.Plan = this.Plan;
        }

        /// <summary>
        /// Exports item info to CSV format.
        /// </summary>
        private void exportToCSVToolStripMenuItem_Click(object sender, EventArgs e) 
        {
            ListViewExporter.CreateCSV(lvItemProperties);
        }
        #endregion

        #region Methods
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

            // Required Skills
            this.requiredSkillsControl.Object = SelectedObject;
        }

        /// <summary>
        /// Updates the plan when the selection is changed.
        /// </summary>
        protected override void OnPlanChanged()
        {
            base.OnPlanChanged();
            requiredSkillsControl.Plan = Plan;

            // We recalculate the right panels minimum size
            int reqSkillControlMinWidth = requiredSkillsControl.MinimumSize.Width;
            int reqSkillPanelMinWidth = scDetails.Panel2MinSize;
            scDetails.Panel2MinSize = (reqSkillPanelMinWidth > reqSkillControlMinWidth ?
                                         reqSkillPanelMinWidth : reqSkillControlMinWidth );
        }
        #endregion
    }
}
