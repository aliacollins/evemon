using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.Data;

namespace EVEMon.SkillPlanner
{
    public partial class EveObjectSelectControl : UserControl
    {
        public event EventHandler SelectionChanged;

        protected Func<Item, Boolean> m_usabilityPredicate;
        protected List<Item> m_selectedObjects = null;
        protected Plan m_plan;

        /// <summary>
        /// Constructor
        /// </summary>
        public EveObjectSelectControl()
        {
            InitializeComponent();
        }

        public Plan Plan
        {
            get { return m_plan; }
            set { m_plan = value; }
        }

        protected virtual void EveObjectSelectControl_Load(object sender, EventArgs e)
        {
            m_usabilityPredicate = SelectAll;
            if (this.DesignMode || this.IsDesignModeHosted()) return;
        }

        #region Search
        protected void lbSearchTextHint_Click(object sender, EventArgs e)
        {
            tbSearchText.Focus();
        }

        protected void tbSearchText_Enter(object sender, EventArgs e)
        {
            lbSearchTextHint.Visible = false;
        }

        protected void tbSearchText_Leave(object sender, EventArgs e)
        {
            lbSearchTextHint.Visible = String.IsNullOrEmpty(tbSearchText.Text);
        }

        protected virtual void tbSearchText_TextChanged(object sender, EventArgs e)
        {
            if (!tbSearchText.Focused)
            {
                lbSearchTextHint.Visible = String.IsNullOrEmpty(tbSearchText.Text);
            }
            BuildListView();
        }

        /// <summary>
        /// Parses the tree node and extracts all the items to build the content of the list box. 
        /// It also deals with text filtering and the treeview/listbox visibility.
        /// </summary>
        protected void BuildListView()
        {
            string searchText = tbSearchText.Text.Trim().ToLower();
           
            if (String.IsNullOrEmpty(searchText))
            {
                tvItems.Visible = true;
                lbSearchList.Visible = false;
                lbNoMatches.Visible = false;
                return;
            }
            // find everything in the current tree that matches the search string
            List<Item> filteredItems = new List<Item>();
            foreach (TreeNode n in tvItems.Nodes)
            {
                SearchNode(n,searchText, filteredItems);
            }
            filteredItems.Sort((x, y) => String.CompareOrdinal(x.Name, y.Name));

            lbSearchList.BeginUpdate();
            try
            {
                lbSearchList.Items.Clear();
                if (filteredItems.Count > 0)
                {
                    foreach (Item eo in filteredItems)
                    {   
                        lbSearchList.Items.Add(eo);
                    }
                }
            }
            finally
            {
                lbSearchList.EndUpdate();
            }
            lbSearchList.Visible = true;
            tvItems.Visible = false;
            lbNoMatches.Visible = (filteredItems.Count == 0);
        }

        private void SearchNode(TreeNode tn, string searchText, List<Item> filteredItems)
        {
            Item itm = tn.Tag as Item;
            if (itm == null)
            {
                foreach (TreeNode subNode in tn.Nodes)
                {
                    SearchNode(subNode, searchText, filteredItems);
                }
                return;
            }
            if (itm.Name.ToLower().Contains(searchText) || itm.Description.ToLower().Contains(searchText))
            {
                filteredItems.Add(itm);
            }
        }
        
        private void tbSearchText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 0x01)
            {
                tbSearchText.SelectAll();
                e.Handled = true;
            }
        }
        #endregion 

        #region Selected Objects
        /// <summary>
        /// All the selected objects (through multi-select)
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(null)]
        [Browsable(false)]
        public List<Item> SelectedObjects
        {
            get { return m_selectedObjects; }
        }

        /// <summary>
        /// The primary selected object
        /// </summary>
        [Browsable(false)]
        public Item SelectedObject
        {
            get
            {
                if (m_selectedObjects == null || m_selectedObjects.Count == 0) return null;
                return m_selectedObjects[0];
            }
            set
            {
                List<Item> selectedObjects = new List<Item>();
                if (value != null) selectedObjects.Add(value);
                SetSelectedObjects(selectedObjects);
            }
        }

        /// <summary>
        /// Selects the given nodes.
        /// </summary>
        /// <param name="s"></param>
        protected void SetSelectedObjects(IEnumerable<Item> s)
        {
            // Updates selection
            m_selectedObjects = (s == null ? new List<Item>() : new List<Item>(s));

            // Selects the proper nodes
            if (m_selectedObjects.Count == 1)
            {
                // If the object is not already selected
                var obj = m_selectedObjects[0];
                tvItems.SelectNodeWithTag(obj);
            }

            // Notify subscribers
            if (SelectionChanged != null)
            {
                SelectionChanged(this, new EventArgs());
            }
        }

        private void UpdateSelectedTreeNodes()
        {
            if (tvItems.SelectedNodes.Count != 0)
            {
                List<Item> selectedObjects = new List<Item>();
                foreach (TreeNode node in tvItems.SelectedNodes)
                {
                    var obj = node.Tag as Item;
                    if (obj != null) selectedObjects.Add(obj);
                }
                SetSelectedObjects(selectedObjects);
            }
            else
            {
                SetSelectedObjects(null);
            }
        }
        #endregion

        #region Events
        private void tvItems_SelectionsChanged(object sender, EventArgs e)
        {
            UpdateSelectedTreeNodes();
        }

        private void lbSearchList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbSearchList.SelectedItems.Count != 0)
            {
                List<Item> selectedObjects = new List<Item>();
                foreach (Object node in lbSearchList.SelectedItems)
                {
                    var obj = node as Item;
                    if (obj != null) selectedObjects.Add(obj);
                }
                SetSelectedObjects(selectedObjects);
            }
            else
            {
                SetSelectedObjects(null);
            }
        }
        #endregion

        private void cmiExpandAll_Click(object sender, EventArgs e)
        {
            tvItems.ExpandAll();
        }

        private void cmiCollapseAll_Click(object sender, EventArgs e)
        {
            tvItems.CollapseAll();
        }

        private void cmiExpandSelected_Click(object sender, EventArgs e)
        {
            tvItems.SelectedNode.ExpandAll();
        }

        private void cmiCollapseSelected_Click(object sender, EventArgs e)
        {
            tvItems.SelectedNode.Collapse();
        }

        private void contextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            cmiExpandSelected.Enabled = cmiCollapseSelected.Enabled = (tvItems.SelectedNode != null && tvItems.SelectedNode.GetNodeCount(true) > 0);
            string aString;
            if (cmiCollapseSelected.Enabled)
                aString = tvItems.SelectedNode.Text;
            else
                aString = "Selected";

            cmiExpandSelected.Text = "Expand " + aString;
            cmiCollapseSelected.Text = "Collapse " + aString;
        }


        #region Predicates
        /// <summary>
        /// Filter for all items
        /// </summary>
        /// <param name="eo"></param>
        /// <returns></returns>
        protected bool SelectAll(Item eo) { return true; }

        /// <summary>
        /// Filter for items which can be used (prereqs met)
        /// </summary>
        /// <param name="eo"></param>
        /// <returns></returns>
        protected bool CanUse(Item eo)
        {
            foreach (var prereq in eo.Prerequisites)
            {
                int level = m_plan.Character.GetSkillLevel(prereq.Skill);
                if (level < prereq.Level) return false;
            }
            return true;
        }

        /// <summary>
        /// Filter for items wich canniot be used (prereqs not met).
        /// </summary>
        /// <param name="eo"></param>
        /// <returns></returns>
        protected bool CannotUse(Item eo)
        {
            return !CanUse(eo);
        }
        #endregion

    }
}
