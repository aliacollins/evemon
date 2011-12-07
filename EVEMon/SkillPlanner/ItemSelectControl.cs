using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.Data;

namespace EVEMon.SkillPlanner
{
    public partial class ItemSelectControl : EveObjectSelectControl
    {
        private readonly List<MarketGroup> m_presetGroups = new List<MarketGroup>();
        private readonly List<ItemMetaGroup> m_metaGroups = new List<ItemMetaGroup>();

        private Func<Item, Boolean> m_slotPredicate = x => true;
        private Func<Item, Boolean> m_metaGroupPredicate = x => true;
        private Func<Item, Boolean> m_fittingPredicate = x => true;


        #region Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        public ItemSelectControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// On load, we read the settings.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            // Return on design mode
            if (DesignMode || this.IsDesignModeHosted())
                return;

            // Call the base method
            base.OnLoad(e);


            m_metaGroups.AddRange(EnumExtensions.GetBitValues<ItemMetaGroup>());

            // Set the preset groups
            if (!StaticItems.MarketGroups.IsEmpty())
            {
                m_presetGroups.Add(StaticItems.MarketGroups.First(x => x.ID == DBConstants.AmmosAndChargesMarketGroupID));
                m_presetGroups.Add(StaticItems.MarketGroups.First(x => x.ID == DBConstants.ImplantsAndBoostersMarketGroupID));
                m_presetGroups.Add(StaticItems.MarketGroups.First(x => x.ID == DBConstants.StarbaseStructuresMarketGroupID));
                m_presetGroups.Add(StaticItems.MarketGroups.First(x => x.ID == DBConstants.ShipModificationsMarketGroupID));
                m_presetGroups.Add(StaticItems.MarketGroups.First(x => x.ID == DBConstants.ShipEquipmentsMarketGroupID));
                m_presetGroups.Add(StaticItems.MarketGroups.First(x => x.ID == DBConstants.DronesMarketGroupID));
            }

            // Initialize the "skills" combo box
            cbUsabilityFilter.Items[0] = "All Items";
            cbUsabilityFilter.Items[1] = "Items I can use";
            cbUsabilityFilter.Items[2] = "Items I cannot use";

            // Initialize the metagroup combo
            ccbGroupFilter.Items.Clear();
            ccbGroupFilter.Items.AddRange(m_metaGroups.Cast<Object>().ToArray());
            ccbGroupFilter.ToolTip = toolTip;

            // Read the settings
            if (Settings.UI.UseStoredSearchFilters)
            {
                // Usability combo
                cbUsabilityFilter.SelectedIndex = (int)Settings.UI.ItemBrowser.UsabilityFilter;

                // Slots combo
                switch (Settings.UI.ItemBrowser.SlotFilter)
                {
                    case ItemSlot.All:
                        cbSlotFilter.SelectedIndex = 0;
                        break;
                    case ItemSlot.High:
                        cbSlotFilter.SelectedIndex = 1;
                        break;
                    case ItemSlot.Medium:
                        cbSlotFilter.SelectedIndex = 2;
                        break;
                    case ItemSlot.Low:
                        cbSlotFilter.SelectedIndex = 3;
                        break;
                    case ItemSlot.None:
                    case ItemSlot.NoSlot:
                        cbSlotFilter.SelectedIndex = 4;
                        break;
                    default:
                        throw new NotImplementedException();
                }

                // Metagroups combo
                for (int i = 0; i < m_metaGroups.Count; i++)
                {
                    ccbGroupFilter.SetItemChecked(i,
                                                  (Settings.UI.ItemBrowser.MetagroupFilter & m_metaGroups[i]) !=
                                                  ItemMetaGroup.None);
                }

                tbSearchText.Text = Settings.UI.ItemBrowser.TextSearch;
                lbSearchTextHint.Visible = String.IsNullOrEmpty(tbSearchText.Text);
            }
            else
            {
                cbUsabilityFilter.SelectedIndex = 0;
                cbSlotFilter.SelectedIndex = 0;
                for (int i = 0; i < m_metaGroups.Count; i++)
                {
                    ccbGroupFilter.SetItemChecked(i, true);
                }
            }

            // We subscribe the 'ItemCheck' here to avoid event triggering while initializing
            ccbGroupFilter.ItemCheck += ccbGroupFilter_ItemCheck;
        }

        #endregion


        #region Events handlers

        /// <summary>
        /// When the search text changed, we store the next settings
        /// and update the list view and the list/tree visibilities.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        protected override void OnSearchTextChanged(string searchText)
        {
            Settings.UI.ItemBrowser.TextSearch = searchText;
            base.OnSearchTextChanged(searchText);
        }

        /// <summary>
        /// When the skill filter combo is changed, we update the settings, the predicate and the content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbUsabilityFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update the settings
            Settings.UI.ItemBrowser.UsabilityFilter = (ObjectUsabilityFilter)cbUsabilityFilter.SelectedIndex;

            // Update the predicate
            switch (Settings.UI.ItemBrowser.UsabilityFilter)
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

            // Update the control's content
            UpdateContent();
        }

        /// <summary>
        /// When the slot filter combo changed, we update the settings, the predicate and the content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbSlotFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update the settings
            switch (cbSlotFilter.SelectedIndex)
            {
                default:
                    Settings.UI.ItemBrowser.SlotFilter = ItemSlot.All;
                    break;
                case 1:
                    Settings.UI.ItemBrowser.SlotFilter = ItemSlot.High;
                    break;
                case 2:
                    Settings.UI.ItemBrowser.SlotFilter = ItemSlot.Medium;
                    break;
                case 3:
                    Settings.UI.ItemBrowser.SlotFilter = ItemSlot.Low;
                    break;
                case 4:
                    Settings.UI.ItemBrowser.SlotFilter = ItemSlot.NoSlot;
                    break;
            }

            // Update the predicate
            ItemSlot slot = Settings.UI.ItemBrowser.SlotFilter;
            m_slotPredicate = x => (x.FittingSlot & slot) != ItemSlot.None;

            // Update the control's content
            UpdateContent();
        }

        /// <summary>
        /// When the meta group combo changed, we update the settings, the predicate and the content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ccbGroupFilter_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Update the settings
            Settings.UI.ItemBrowser.MetagroupFilter = ItemMetaGroup.None;
            for (int i = 0; i < m_metaGroups.Count; i++)
            {
                if (ccbGroupFilter.GetItemChecked(i))
                    Settings.UI.ItemBrowser.MetagroupFilter |= m_metaGroups[i];
            }

            // Update the predicate
            ItemMetaGroup filter = Settings.UI.ItemBrowser.MetagroupFilter;
            m_metaGroupPredicate = x => (x.MetaGroup & filter) != ItemMetaGroup.None;

            // Update the control's content
            UpdateContent();
        }

        /// <summary>
        /// When the CPU's numeric box changed, we update the predicate, the content (no settings)
        /// and the numeric box's availability.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbCPU_CheckedChanged(object sender, EventArgs e)
        {
            numCPU.Enabled = cbCPU.Checked;
            UpdateFittingPredicate();
            UpdateContent();
        }

        /// <summary>
        /// When the powergrid's numeric box changed, we update the predicate, the content (no settings)
        /// and the numeric box's availability.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbPowergrid_CheckedChanged(object sender, EventArgs e)
        {
            numPowergrid.Enabled = cbPowergrid.Checked;
            UpdateFittingPredicate();
            UpdateContent();
        }

        /// <summary>
        /// When the CPU's numeric box changed, we update the predicate and the content (no settings).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numCPU_ValueChanged(object sender, EventArgs e)
        {
            UpdateFittingPredicate();
            UpdateContent();
        }

        /// <summary>
        /// When the powergrid's numeric box changed, we update the predicate and the content (no settings).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numPowergrid_ValueChanged(object sender, EventArgs e)
        {
            UpdateFittingPredicate();
            UpdateContent();
        }

        /// <summary>
        /// When the "show all items" checkbox changed, we update the settings and trigger a content update.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showAllGroupsCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.UI.ItemBrowser.ShowAllGroups = showAllGroupsCheckbox.Checked;
            UpdateContent();
        }

        /// <summary>
        /// Updates the fitting predicate.
        /// </summary>
        private void UpdateFittingPredicate()
        {
            if (!numCPU.Enabled && !numPowergrid.Enabled)
                m_fittingPredicate = x => true;
            else
            {
                double? gridAvailable = null;
                if (numPowergrid.Enabled)
                    gridAvailable = (double)numPowergrid.Value;

                double? cpuAvailable = null;
                if (numCPU.Enabled)
                    cpuAvailable = (double)numCPU.Value;

                m_fittingPredicate = item => item.CanActivate(cpuAvailable, gridAvailable);
            }
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
                foreach (MarketGroup group in StaticItems.MarketGroups)
                {
                    // Skip some groups
                    if (!showAllGroupsCheckbox.Checked && !m_presetGroups.Contains(group))
                        continue;

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
        /// Create the tree nodes for the given category and add them to the given nodes collection.
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
                                                             && m_slotPredicate(x)
                                                             && m_metaGroupPredicate(x)
                                                             && m_fittingPredicate(x)).Select(
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