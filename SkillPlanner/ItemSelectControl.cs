using System;
using System.Collections.Generic;
using System.Windows.Forms;
using EVEMon.Common;
using System.Text;

namespace EVEMon.SkillPlanner
{
    public partial class ItemSelectControl : EveObjectSelectControl 
    {
        public ItemSelectControl()
        {
            InitializeComponent();

            // Initialize the tech-level combo box
            this.ccbTechFilter.ToolTip = this.toolTip;
            this.ccbTechFilter.ItemCheck += new ItemCheckEventHandler(ccbTechFilter_ItemCheck);
            this.ccbTechFilter.CustomTextBuilder = (list) =>
                {
                    StringBuilder b = new StringBuilder("Tech ");
                    for (int i = 0; i < list.CheckedIndices.Count; i++)
                    {
                        if (i != 0) b.Append(list.ValueSeparator);
                        b.Append(Skill.GetRomanForInt(list.CheckedIndices[i] + 1));
                    }
                    b.Append(", none");
                    return b.ToString();
                };

            this.ccbGroupFilter.ToolTip = this.toolTip;
            this.ccbGroupFilter.ItemCheck += new ItemCheckEventHandler(ccbGroupFilter_ItemCheck);
            this.ccbGroupFilter.CustomTextBuilder = (list) =>
            {
                StringBuilder b = new StringBuilder("Regular, ");
                for (int i = 0; i < list.CheckedIndices.Count; i++)
                {
                    if (i != 0) b.Append(list.ValueSeparator);
                    b.Append(list.CheckedItems[i].ToString());
                }
                return b.ToString();
            };
        }

        private ItemCategory m_rootCategory;

        protected override void EveObjectSelectControl_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                return;
            }
            base.EveObjectSelectControl_Load(sender, e);

            try
            {
                this.ccbGroupFilter.SetItemChecked(0, m_settings.ShowNamedItems);
                this.ccbGroupFilter.SetItemChecked(1, m_settings.ShowFactionItems);
                this.ccbGroupFilter.SetItemChecked(2, m_settings.ShowOfficerItems);
                this.ccbGroupFilter.SetItemChecked(3, m_settings.ShowDeadspaceItems);

                this.ccbTechFilter.SetItemChecked(0, m_settings.ShowT1Items);
                this.ccbTechFilter.SetItemChecked(1, m_settings.ShowT2Items);
                this.ccbTechFilter.SetItemChecked(2, m_settings.ShowT3Items);

                cbSkillFilter.SelectedIndex = m_settings.ItemSkillFilter;
                this.cbSkillFilter.Items[0] = "All Items";
                this.cbSkillFilter.Items[1] = "Items I can use";
                this.cbSkillFilter.Items[2] = "Items I cannot use";

                cbSlotFilter.SelectedIndex = m_settings.ItemSlotFilter;

                m_rootCategory = ItemCategory.GetRootCategory();

                // needs to be after we set the root category.
                if (m_settings.StoreBrowserFilters)
                    tbSearchText.Text = m_settings.ItemBrowserSearch;
                lbSearchTextHint.Visible = String.IsNullOrEmpty(tbSearchText.Text);
                if (m_plan != null)
                    BuildTreeView();
            }
            catch (Exception err)
            {
                // This occurs when we're in the designer. DesignMode doesn't get set
                // when the control is a subcontrol of a user control, so we should handle
                // this here :(
                ExceptionHandler.LogException(err, true);
                return;
            }
        }

        #region Skill filter
        private void cbSkillFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_settings.ItemSkillFilter=cbSkillFilter.SelectedIndex;
            UpdateDisplay();
        }

        private void UpdateSkillFilter()
        {
            switch (cbSkillFilter.SelectedIndex)
            {
                default:
                case 0: m_filterDelegate = SelectAll;
                    break;
                case 1: // Items I Can fly
                    m_filterDelegate = CanUse;
                    break;
                case 2: // Items I Can NOT fly
                    m_filterDelegate = CannotUse;
                    break;
            }
        }
        #endregion


        #region Slot filter
        private ItemFilter slotFilter = delegate { return true; };
        private void cbSlotFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_settings.ItemSlotFilter = cbSlotFilter.SelectedIndex;
            UpdateDisplay();
        }

        private void UpdateSlotFilter()
        {
            switch (cbSlotFilter.SelectedIndex)
            {
                default:
                case 0: // All items
                    slotFilter = delegate { return true; };
                    break;
                case 1: // High slot
                case 2: // Mid slot
                case 3: // Low slot
                    slotFilter = delegate(Item i) { return i.SlotIndex == cbSlotFilter.SelectedIndex; };
                    break;
                case 4: // No-slot items
                    slotFilter = delegate(Item i) { return i.SlotIndex == 0; };
                    break;
            }
        }
        #endregion 


        #region Tech level filter
        void ccbTechFilter_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            bool value = (e.NewValue == CheckState.Checked);
            switch (e.Index)
            {
                case 0:
                    m_settings.ShowT1Items = value;
                    break;
                case 1:
                    m_settings.ShowT2Items = value;
                    break;
                case 2:
                    m_settings.ShowT3Items = value;
                    break;
                default:
                    throw new NotImplementedException();
            }
            UpdateDisplay();
        }

        private bool TechFilter(Item i)
        {
            switch (i.TechLevel)
            {
                case 1:
                    return this.ccbTechFilter.GetItemChecked(0);
                case 2:
                    return this.ccbTechFilter.GetItemChecked(1);
                case 3:
                    return this.ccbTechFilter.GetItemChecked(2);
                default:
                    return true;
            }
        }
        #endregion


        #region Metagroup filter
        void ccbGroupFilter_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            bool value = (e.NewValue == CheckState.Checked);
            switch (e.Index)
            {
                case 0:
                    m_settings.ShowNamedItems = value;
                    break;
                case 1:
                    m_settings.ShowFactionItems = value;
                    break;
                case 2:
                    m_settings.ShowOfficerItems = value;
                    break;
                case 3:
                    m_settings.ShowDeadspaceItems = value;
                    break;
                default:
                    throw new NotImplementedException();
            }
            UpdateDisplay();
        }

        private bool MetaGroupFilter(Item i)
        {
            switch (i.Metagroup)
            {
                case "Named":
                    return this.ccbGroupFilter.GetItemChecked(0);
                case "Officer":
                case "Storyline":
                    return this.ccbGroupFilter.GetItemChecked(2);
                case "Faction":
                    return this.ccbGroupFilter.GetItemChecked(1);
                case "Deadspace":
                    return this.ccbGroupFilter.GetItemChecked(3);
                default:
                    return true;
            }
        }
        #endregion


        #region Fitting filter
        private delegate bool ItemFilter(Item i);
        private ItemFilter itemFittingFilter = delegate { return true; };

        private void UpdateFittingFilter()
        {
            if (!this.numCPU.Enabled && !this.numPowergrid.Enabled)
            {
                this.itemFittingFilter = delegate { return true; };
            }
            else
            {
                double? gridAvailable = null;
                if (numPowergrid.Enabled) gridAvailable = (double)numPowergrid.Value;

                double? cpuAvailable = null;
                if (numCPU.Enabled) cpuAvailable = (double)numCPU.Value;

                this.itemFittingFilter = (item) => item.canActivate(cpuAvailable, gridAvailable);
            }
            this.BuildTreeView();
        }

        private void cbCPU_CheckedChanged(object sender, EventArgs e)
        {
            this.numCPU.Enabled = this.cbCPU.Checked;
            this.UpdateFittingFilter();
        }

        private void cbPowergrid_CheckedChanged(object sender, EventArgs e)
        {
            this.numPowergrid.Enabled = this.cbPowergrid.Checked;
            this.UpdateFittingFilter();
        }

        private void numCPU_ValueChanged(object sender, EventArgs e)
        {
            this.UpdateFittingFilter();
        }

        private void numPowergrid_ValueChanged(object sender, EventArgs e)
        {
            this.UpdateFittingFilter();
        }
        #endregion


        #region Text filter
        protected override void tbSearchText_TextChanged(object sender, EventArgs e)
        {

            if (m_settings.StoreBrowserFilters)
                m_settings.ItemBrowserSearch = tbSearchText.Text;
            base.tbSearchText_TextChanged(sender, e);
        }
        #endregion 



        #region Display
        private void UpdateDisplay()
        {
            UpdateSkillFilter();
            UpdateSlotFilter();
            UpdateFittingFilter();
            if (m_rootCategory != null)
                BuildTreeView();
            SearchTextChanged();
        }

        private void BuildTreeView()
        {
            tvItems.Nodes.Clear();
            tvItems.BeginUpdate();
            int numberOfItems = 0;
            try
            {
                if (m_rootCategory != null)
                {
                    numberOfItems = BuildSubtree(m_rootCategory, tvItems.Nodes);
                }
            }
            finally
            {
                tvItems.EndUpdate();
                //If the filtered set is small enough to fit all nodes on screen, call expandAll()
                if (numberOfItems < (tvItems.DisplayRectangle.Height / tvItems.ItemHeight))
                {
                    tvItems.ExpandAll();
                }
            }
        }

        private int BuildSubtree(ItemCategory cat, TreeNodeCollection nodeCollection)
        {
            int result = 0;
            SortedDictionary<string, ItemCategory> sortedSubcats = new SortedDictionary<string, ItemCategory>();
            foreach (ItemCategory tcat in cat.Subcategories)
            {
                sortedSubcats.Add(tcat.Name, tcat);
            }
            foreach (ItemCategory tcat in sortedSubcats.Values)
            {
                TreeNode tn = new TreeNode();
                tn.Text = tcat.Name;
                result += BuildSubtree(tcat, tn.Nodes);
                if (tn.GetNodeCount(true) > 0)
                    nodeCollection.Add(tn);
            }

            SortedDictionary<string, Item> sortedItems = new SortedDictionary<string, Item>();
            foreach (Item titem in cat.Items)
            {
                if (m_filterDelegate(titem) 
                    && slotFilter(titem) 
                    && itemFittingFilter(titem)
                    && MetaGroupFilter(titem)
                    && TechFilter(titem))
                    sortedItems.Add(titem.Name, titem);
            }
            foreach (Item titem in sortedItems.Values)
            {
                TreeNode tn = new TreeNode();
                tn.Text = titem.Name;
                tn.Tag = titem;
                result++;
                nodeCollection.Add(tn);
            }
            return result;
        }
        #endregion
    }
}
