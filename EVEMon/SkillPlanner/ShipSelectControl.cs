using System;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.Data;

namespace EVEMon.SkillPlanner
{
    public sealed partial class ShipSelectControl : EveObjectSelectControl
    {
        private Func<Item, Boolean> m_racePredicate = x => true;


        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public ShipSelectControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// On load, we read the settings and fill the tree.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            // Return on design mode
            if (DesignMode || this.IsDesignModeHosted())
                return;

            // Call the base method
            base.OnLoad(e);

            // Initialize the "skills" combo box
            cbUsabilityFilter.Items[0] = "All Ships";
            cbUsabilityFilter.Items[1] = "Ships I can fly";
            cbUsabilityFilter.Items[2] = "Ships I cannot fly";

            // Read the settings
            if (Settings.UI.UseStoredSearchFilters)
            {
                cbUsabilityFilter.SelectedIndex = (int)Settings.UI.ShipBrowser.UsabilityFilter;
                tbSearchText.Text = Settings.UI.ShipBrowser.TextSearch;
                lbSearchTextHint.Visible = String.IsNullOrEmpty(tbSearchText.Text);

                cbAmarr.Checked = (Settings.UI.ShipBrowser.RacesFilter & Race.Amarr) != Race.None;
                cbCaldari.Checked = (Settings.UI.ShipBrowser.RacesFilter & Race.Caldari) != Race.None;
                cbGallente.Checked = (Settings.UI.ShipBrowser.RacesFilter & Race.Gallente) != Race.None;
                cbMinmatar.Checked = (Settings.UI.ShipBrowser.RacesFilter & Race.Minmatar) != Race.None;
                cbFaction.Checked = (Settings.UI.ShipBrowser.RacesFilter & Race.Faction) != Race.None;
                cbORE.Checked = (Settings.UI.ShipBrowser.RacesFilter & Race.Ore) != Race.None;
            }
            else
            {
                cbUsabilityFilter.SelectedIndex = 0;
                cbAmarr.Checked = true;
                cbCaldari.Checked = true;
                cbGallente.Checked = true;
                cbMinmatar.Checked = true;
                cbFaction.Checked = true;
                cbORE.Checked = true;
            }
        }

        #endregion


        #region Callbacks

        /// <summary>
        /// When the combo for filter changes, we update the settings and the control content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void cbUsabilityFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update settings
            Settings.UI.ShipBrowser.UsabilityFilter = (ObjectUsabilityFilter)cbUsabilityFilter.SelectedIndex;

            // Update the filter delegate
            switch (Settings.UI.ShipBrowser.UsabilityFilter)
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
        /// When one of the races combo is checked/unchecked, we update the settings and the control content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbRace_SelectedChanged(object sender, EventArgs e)
        {
            // Retrieve the race
            Race race = Race.None;
            if (cbAmarr.Checked)
                race |= Race.Amarr;
            if (cbCaldari.Checked)
                race |= Race.Caldari;
            if (cbGallente.Checked)
                race |= Race.Gallente;
            if (cbMinmatar.Checked)
                race |= Race.Minmatar;
            if (cbFaction.Checked)
                race |= Race.Faction;
            if (cbORE.Checked)
                race |= Race.Ore;

            // Update the settings
            Settings.UI.ShipBrowser.RacesFilter |= race;

            // Update the predicate
            m_racePredicate = x => (x.Race & race) != Race.None;

            // Update content
            UpdateContent();
        }

        /// <summary>
        /// When the search text changed, we store the next settings
        /// and update the list view and the list/tree visibilities.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        protected override void OnSearchTextChanged(string searchText)
        {
            Settings.UI.ShipBrowser.TextSearch = searchText;
            base.OnSearchTextChanged(searchText);
        }

        #endregion


        #region Content creation

        /// <summary>
        /// Refresh the controls.
        /// </summary>
        private void UpdateContent()
        {
            BuildTreeView();
            BuildListView();
        }

        /// <summary>
        /// Rebuild the tree view.
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
                foreach (MarketGroup group in StaticItems.ShipsMarketGroup.SubGroups)
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
        /// Create the tree nodes for the given group and add them to the given nodes collection
        /// </summary>
        /// <param name="group"></param>
        /// <param name="nodeCollection"></param>
        /// <returns></returns>
        private int BuildSubtree(MarketGroup group, TreeNodeCollection nodeCollection)
        {
            // Total items count in this category and its subcategories
            int result = 0;

            // Add all subcategories
            foreach (MarketGroup childGroup in group.SubGroups)
            {
                TreeNode node = new TreeNode
                                    {
                                        Text = childGroup.Name,
                                        Tag = childGroup
                                    };

                // Add this subcategory's items count
                result += BuildSubtree(childGroup, node.Nodes);

                // Only add if this subcategory has children
                if (node.GetNodeCount(true) > 0)
                    nodeCollection.Add(node);
            }

            // Add all items
            foreach (TreeNode node in group.Items.Where(x => UsabilityPredicate(x)
                                                             && m_racePredicate(x)).Select(
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