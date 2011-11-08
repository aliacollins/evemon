using System;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.Data;

namespace EVEMon.SkillPlanner
{
    public partial class BlueprintSelectControl : EveObjectSelectControl
    {
        private Func<Item, Boolean> m_metaGroupPredicate = x => true;


        #region Initialization

        public BlueprintSelectControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// On load, we read the settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void EveObjectSelectControl_Load(object sender, EventArgs e)
        {
            // Return on design mode
            if (DesignMode || this.IsDesignModeHosted())
                return;

            // Call the base method
            base.EveObjectSelectControl_Load(sender, e);

            // Initialize the "filter" combo box
            cbUsabilityFilter.Items[0] = "All Blueprints";
            cbUsabilityFilter.Items[1] = "Blueprints I can use";
            cbUsabilityFilter.Items[2] = "Blueprints I cannot use";

            // Read the settings
            if (Settings.UI.UseStoredSearchFilters)
            {
                cbUsabilityFilter.SelectedIndex = (int)Settings.UI.BlueprintBrowser.UsabilityFilter;
                cbActivityFilter.SelectedIndex = (int)Settings.UI.BlueprintBrowser.ActivityFilter;
                tbSearchText.Text = Settings.UI.BlueprintBrowser.TextSearch;
                lbSearchTextHint.Visible = String.IsNullOrEmpty(tbSearchText.Text);

                cbTech1.Checked =
                    (Settings.UI.BlueprintBrowser.MetagroupFilter & ItemMetaGroup.T1) != ItemMetaGroup.None;
                cbTech2.Checked =
                    (Settings.UI.BlueprintBrowser.MetagroupFilter & ItemMetaGroup.T2) != ItemMetaGroup.None;
                cbTech3.Checked =
                    (Settings.UI.BlueprintBrowser.MetagroupFilter & ItemMetaGroup.T3) != ItemMetaGroup.None;
                cbFaction.Checked =
                    (Settings.UI.BlueprintBrowser.MetagroupFilter & ItemMetaGroup.Faction) != ItemMetaGroup.None;
                cbStoryline.Checked =
                    (Settings.UI.BlueprintBrowser.MetagroupFilter & ItemMetaGroup.Storyline) != ItemMetaGroup.None;
                cbOfficer.Checked =
                    (Settings.UI.BlueprintBrowser.MetagroupFilter & ItemMetaGroup.Officer) != ItemMetaGroup.None;
            }
            else
            {
                cbUsabilityFilter.SelectedIndex = 0;
                cbActivityFilter.SelectedIndex = 0;
                cbTech1.Checked = true;
                cbTech2.Checked = true;
                cbTech3.Checked = true;
                cbFaction.Checked = true;
                cbStoryline.Checked = true;
                cbOfficer.Checked = true;
            }
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// When the combo for filter changes, we update the settings and the control content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbUsabilityFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update settings
            Settings.UI.BlueprintBrowser.UsabilityFilter = (ObjectUsabilityFilter)cbUsabilityFilter.SelectedIndex;

            // Enable/Disable the activity filter
            cbActivityFilter.Enabled = Settings.UI.BlueprintBrowser.UsabilityFilter != ObjectUsabilityFilter.All;

            // Update the filter delegate
            switch (Settings.UI.BlueprintBrowser.UsabilityFilter)
            {
                case ObjectUsabilityFilter.All:
                    UsabilityPredicate = SelectAll;
                    break;

                case ObjectUsabilityFilter.Usable:
                    UsabilityPredicate = CanUse;
                    break;

                case ObjectUsabilityFilter.Unusable:
                    UsabilityPredicate = CannotUse;
                    break;

                default:
                    throw new NotImplementedException();
            }

            // Update content
            UpdateContent();
        }

        /// <summary>
        /// When the combo for activity filter changes, we update the settings and the control content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbActivity_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update settings
            Settings.UI.BlueprintBrowser.ActivityFilter = (ObjectActivityFilter)cbActivityFilter.SelectedIndex;
            ActivityFilter = Settings.UI.BlueprintBrowser.ActivityFilter;

            switch (ActivityFilter)
            {
                case ObjectActivityFilter.Manufacturing:
                    Activity = BlueprintActivity.Manufacturing;
                    break;

                case ObjectActivityFilter.Copying:
                    Activity = BlueprintActivity.Copying;
                    break;

                case ObjectActivityFilter.ResearchingMaterialProductivity:
                    Activity = BlueprintActivity.ResearchingMaterialProductivity;
                    break;

                case ObjectActivityFilter.ResearchingTimeProductivity:
                    Activity = BlueprintActivity.ResearchingTimeProductivity;
                    break;

                case ObjectActivityFilter.Invention:
                    Activity = BlueprintActivity.Invention;
                    break;

                default:
                    Activity = BlueprintActivity.None;
                    break;
            }

            cbUsabilityFilter_SelectedIndexChanged(sender, e);
        }

        /// <summary>
        /// When one of the metagroups combo is checked/unchecked, we update the settings and the control content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbMetagroup_CheckedChanged(object sender, EventArgs e)
        {
            // Retrieve the metagroup
            ItemMetaGroup metagroup = ItemMetaGroup.None;
            if (cbTech1.Checked)
                metagroup |= ItemMetaGroup.T1;
            if (cbTech2.Checked)
                metagroup |= ItemMetaGroup.T2;
            if (cbTech3.Checked)
                metagroup |= ItemMetaGroup.T3;
            if (cbFaction.Checked)
                metagroup |= ItemMetaGroup.Faction;
            if (cbStoryline.Checked)
                metagroup |= ItemMetaGroup.Storyline;
            if (cbOfficer.Checked)
                metagroup |= ItemMetaGroup.Officer;

            // Update the settings
            Settings.UI.BlueprintBrowser.MetagroupFilter |= metagroup;

            // Update the predicate
            m_metaGroupPredicate = x => (x.MetaGroup & metagroup) != ItemMetaGroup.None;

            // Update content
            UpdateContent();
        }

        /// <summary>
        /// When the search text changed, we store the next settings and update the list view and the list/tree visibilities.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void tbSearchText_TextChanged(object sender, EventArgs e)
        {
            Settings.UI.BlueprintBrowser.TextSearch = tbSearchText.Text;
            base.tbSearchText_TextChanged(sender, e);
        }

        #endregion


        #region Content creation

        /// <summary>
        /// Refresh the controls
        /// </summary>
        private void UpdateContent()
        {
            BuildTreeView();
            BuildListView();
        }

        /// <summary>
        /// Rebuild the tree view
        /// </summary>
        private void BuildTreeView()
        {
            // Store the selected node (if any) to restore it after the update
            int selectedItemHash = (tvItems.SelectedNodes.Count > 0
                                        ? tvItems.SelectedNodes[0].Tag.GetHashCode()
                                        : 0);

            int numberOfItems = 0;
            tvItems.BeginUpdate();
            try
            {
                tvItems.Nodes.Clear();

                // Create the nodes
                foreach (BlueprintMarketGroup group in StaticBlueprints.BlueprintMarketGroups)
                {
                    TreeNode node = new TreeNode
                                        {
                                            Text = group.Name,
                                            Tag = group
                                        };

                    int result = BuildSubtree(group, node.Nodes);

                    if (result == 0)
                        continue;

                    numberOfItems += result;
                    tvItems.Nodes.Add(node);
                }

                TreeNode selectedNode = null;

                // Restore the selected node (if any)
                if (selectedItemHash > 0)
                {
                    foreach (TreeNode node in tvItems.GetAllNodes().Where(node => node.Tag.GetHashCode() == selectedItemHash))
                    {
                        tvItems.SelectNodeWithTag(node.Tag);
                        selectedNode = node;
                    }
                }

                // Reset if the node doesn't exist anymore
                if (selectedNode == null)
                {
                    tvItems.UnselectAllNodes();
                    SelectedObject = null;
                }
            }
            finally
            {
                tvItems.EndUpdate();
                AllExpanded = false;

                // If the filtered set is small enough to fit all nodes on screen, call expandAll()
                if (numberOfItems < (tvItems.DisplayRectangle.Height / tvItems.ItemHeight))
                {
                    tvItems.ExpandAll();
                    AllExpanded = true;
                }
            }
        }

        /// <summary>
        /// Create the tree nodes for the given category and add them to the given nodes collection
        /// </summary>
        /// <param name="group"></param>
        /// <param name="nodeCollection"></param>
        /// <returns></returns>
        private int BuildSubtree(BlueprintMarketGroup group, TreeNodeCollection nodeCollection)
        {
            // Total blueprints count in this category and its subcategories
            int result = 0;

            // Add all subcategories
            foreach (BlueprintMarketGroup childGroup in group.SubGroups)
            {
                TreeNode node = new TreeNode
                                    {
                                        Text = childGroup.Name,
                                        Tag = childGroup
                                    };

                // Add this subcategory's blueprints count
                result += BuildSubtree(childGroup, node.Nodes);

                // Only add if this subcategory has children
                if (node.GetNodeCount(true) > 0)
                    nodeCollection.Add(node);
            }

            // Add all blueprints
            foreach (TreeNode node in group.Blueprints.Where(x => UsabilityPredicate(x)
                                                                  && m_metaGroupPredicate(x)).Select(
                                                                      childItem => new TreeNode
                                                                                       {
                                                                                           Text = childItem.Name,
                                                                                           Tag = childItem
                                                                                       }))
            {
                nodeCollection.Add(node);
                result++;
            }
            return result;
        }

        #endregion
    }
}