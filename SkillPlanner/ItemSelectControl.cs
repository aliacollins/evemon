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
        private  ItemFilter slotFilter = delegate { return true; };

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
            m_settings.ItemSlotFilter=cbSlotFilter.SelectedIndex;
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
            try
            {
                if (m_rootCategory != null)
                {
                    BuildSubtree(m_rootCategory, tvItems.Nodes);
                }
                
            }
            finally
            {
                tvItems.EndUpdate();
            }
        }

        private void BuildSubtree(ItemCategory cat, TreeNodeCollection nodeCollection)
        {
            SortedDictionary<string, ItemCategory> sortedSubcats = new SortedDictionary<string, ItemCategory>();
            foreach (ItemCategory tcat in cat.Subcategories)
            {
                sortedSubcats.Add(tcat.Name, tcat);
            }
            foreach (ItemCategory tcat in sortedSubcats.Values)
            {
                TreeNode tn = new TreeNode();
                tn.Text = tcat.Name;
                BuildSubtree(tcat, tn.Nodes);
                if (tn.GetNodeCount(true) > 0)
                    nodeCollection.Add(tn);
            }

            SortedDictionary<string, Item> sortedItems = new SortedDictionary<string, Item>();
            foreach (Item titem in cat.Items)
            {
                if (m_filterDelegate(titem) && slotFilter(titem) && ClassFilter(titem))
                    sortedItems.Add(titem.Name, titem);
            }
            foreach (Item titem in sortedItems.Values)
            {
                TreeNode tn = new TreeNode();
                tn.Text = titem.Name;
                tn.Tag = titem;
                nodeCollection.Add(tn);
            }
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
