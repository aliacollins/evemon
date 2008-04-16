using System;
using System.Collections.Generic;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon.SkillPlanner
{
    public partial class ItemSelectControl : EveObjectSelectControl 
    {
        public ItemSelectControl()
        {
            InitializeComponent();
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
                cbSkillFilter.SelectedIndex = m_settings.ItemSkillFilter;
                cbSlotFilter.SelectedIndex = m_settings.ItemSlotFilter;
                cbTech1.Checked = m_settings.ShowT1Items;
                cbNamed.Checked = m_settings.ShowNamedItems;
                cbTech2.Checked = m_settings.ShowT2Items;
                cbOfficer.Checked = m_settings.ShowOfficerItems;
                cbFaction.Checked = m_settings.ShowFactionItems;
                cbDeadspace.Checked = m_settings.ShowDeadspaceItems;

                this.cbSkillFilter.Items[0] = "All Items";
                this.cbSkillFilter.Items[1] = "Items I can use";
                this.cbSkillFilter.Items[2] = "Items I cannot use";

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

        #region Filters
        private delegate bool ItemFilter(Item i);
        private ItemFilter slotFilter = delegate { return true; };
        private ItemFilter shipFittingFilter = delegate { return true; };

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

        private bool ClassFilter(Item i)
        {
            switch (i.Metagroup)
            {
                case "Tech I":
                   return cbTech1.Checked;
                case "Named":
                    return cbNamed.Checked;
                case "Tech II":
                    return cbTech2.Checked;
                case "Officer":
                case "Storyline":
                    return cbOfficer.Checked;
                case "Faction":
                    return cbFaction.Checked;
                case "Deadspace":
                    return cbDeadspace.Checked;
                default:
                    return false;
             }
        }

        private void cbSlotFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbSlotFilter.SelectedIndex > 0)
            {
                //If we pick anything other than "all items", reset the ship fitting
                //filter to prevent interference.
                this.iffsShipFitting.reset();
            }

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

        private void cbClass_SelectedChanged(object sender, EventArgs e)
        {
            if (sender == cbTech1) m_settings.ShowT1Items = cbTech1.Checked;
            if (sender == cbNamed) m_settings.ShowNamedItems = cbNamed.Checked;
            if (sender == cbTech2) m_settings.ShowT2Items = cbTech2.Checked;
            if (sender == cbOfficer) m_settings.ShowOfficerItems = cbOfficer.Checked;
            if (sender == cbFaction) m_settings.ShowFactionItems = cbFaction.Checked;
            if (sender == cbDeadspace) m_settings.ShowDeadspaceItems = cbDeadspace.Checked;
            UpdateDisplay();
        }

        private void iffsShipFitting_ItemFilterDataChanged(object sender, ItemFilteringChangedEvent e)
        {
            ItemFittingFilterData data = e.filterData;
            if (data.allFilteringDisabled())
            {
                this.shipFittingFilter = delegate { return true; };
            }
            else 
            {
                if (!data.allSlotsSelected())
                {
                    //Set the slots combobox to "all items" to prevent interference
                    this.cbSlotFilter.SelectedIndex = 0; //AllItems
                    this.UpdateSlotFilter();
                }
                this.shipFittingFilter = new ShipFittingFilter(data).evaluateItem;
            }
            this.BuildTreeView();
        }

        /// <summary>
        /// Wrapper class for <code>ItemFittingFilterData</code> that decorates it
        /// with a delegate method for the item filtering.
        /// </summary>
        private class ShipFittingFilter
        {
            ItemFittingFilterData data;
            public ShipFittingFilter(ItemFittingFilterData mydata)
            {
                data = mydata;
            }
            public bool evaluateItem(Item i)
            {
                bool result;
                if (data.allSlotsSelected())
                {
                    //No slot filtering, so any item is valid.
                    result = true;
                }
                else
                {
                    //Item is valid if its slot type is in the selection.
                    result = data.highSlotSelected && i.SlotIndex == 1;
                    result = result || (data.medSlotSelected && i.SlotIndex == 2);
                    result = result || (data.lowSlotSelected && i.SlotIndex == 3);
                }
                double? cpuAvailable = null; 
                double? gridAvailable = null; 

                if (data.cpuAvailable.HasValue) cpuAvailable = Convert.ToDouble(data.cpuAvailable.Value);
                if (data.gridAvailable.HasValue) gridAvailable = Convert.ToDouble(data.gridAvailable.Value);

                //Now lets see if the item's CPU/Grid needs are within bounds
                result = result && i.canActivate(cpuAvailable, gridAvailable);

                return result;
            }
        }
        
        #endregion

        #region Display

        private void UpdateDisplay()
        {
            UpdateSkillFilter();
            UpdateSlotFilter();
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
                    && shipFittingFilter(titem)
                    && ClassFilter(titem))
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

        #region Search
  
        protected override void tbSearchText_TextChanged(object sender, EventArgs e)
        {

            if (m_settings.StoreBrowserFilters)
                m_settings.ItemBrowserSearch = tbSearchText.Text;
            base.tbSearchText_TextChanged(sender, e);
        }


        #endregion 


        #region Events
        #endregion
    }
}
