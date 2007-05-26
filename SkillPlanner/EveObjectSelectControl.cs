using System;
using System.Collections.Generic;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon.SkillPlanner
{
    public partial class EveObjectSelectControl : UserControl
    {
        public EveObjectSelectControl()
        {
            InitializeComponent();
        }

        protected Settings m_settings;
        protected Plan m_plan;
        public Plan Plan
        {
            get { return m_plan; }
            set { m_plan = value; }
        }

        protected virtual void EveObjectSelectControl_Load(object sender, EventArgs e)
        {
            m_filterDelegate = SelectAll;
            if (this.DesignMode)
            {
                return;
            }

            try
            {
                m_settings = Settings.GetInstance();
                //cbSkillFilter.SelectedIndex = 0;
                lbSearchTextHint.Visible = String.IsNullOrEmpty(tbSearchText.Text);
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
        protected delegate bool EveObjectFilter(EveObject eo);
        protected EveObjectFilter m_filterDelegate;
        #endregion

        #region delegate functions

        /// <summary>
        /// Filter for all items
        /// </summary>
        /// <param name="eo"></param>
        /// <returns></returns>
        protected bool SelectAll(EveObject eo) { return true; }
        
        /// <summary>
        /// Filter for items that can be used (prereqs met)
        /// </summary>
        /// <param name="eo"></param>
        /// <returns></returns>
        protected bool CanUse(EveObject eo)
        {
             Skill skill = null;
             for (int i = 0; i < eo.RequiredSkills.Count; i++)
             {
                 try
                 {
                     skill = m_plan.GrandCharacterInfo.GetSkill(eo.RequiredSkills[i].Name);
                     if (skill.Level < eo.RequiredSkills[i].Level) return false;
                 }
                 catch
                 {
                     // unknown or no skill - assume we can use it
                     return true;
                 }
             }
             return true;
         }

        protected bool CannotUse(EveObject eo)
        {
            return !CanUse(eo);
        }
        #endregion

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
            SearchTextChanged();
        }

        protected void SearchTextChanged()
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
            SortedList<string, EveObject> filteredItems = new SortedList<string, EveObject>();
            foreach (TreeNode n in tvItems.Nodes)
            {
                SearchNode(n,searchText,filteredItems);
            }

            lbSearchList.BeginUpdate();
            try
            {
                lbSearchList.Items.Clear();
                if (filteredItems.Count > 0)
                {
                    foreach (EveObject eo in filteredItems.Values)
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

        private void SearchNode(TreeNode tn, string searchText, SortedList<string, EveObject> filteredItems)
        {
            EveObject itm = tn.Tag as EveObject;
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
                filteredItems.Add(itm.Name, itm);
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
        protected List<EveObject> m_selectedObjects = null;

        /// <summary>
        /// All the selected objects (through multi-select)
        /// </summary>
        public List<EveObject> SelectedObjects
        {
            get { return m_selectedObjects; }
            set { m_selectedObjects = value; }
        }

        /// <summary>
        /// The primary selected object
        /// </summary>
        public EveObject SelectedObject
        {
            get
            {
                if (m_selectedObjects != null && m_selectedObjects.Count != 0)
                {
                    return m_selectedObjects[0];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                List<EveObject> selectedObjects = new List<EveObject>();
                selectedObjects.Add(value);
                m_selectedObjects = selectedObjects;
            }
        }

        public event EventHandler<EventArgs> SelectedObjectChanged;

        protected void SetSelectedObjects(List<EveObject> s)
        {
            m_selectedObjects = s;
            if (SelectedObjectChanged != null)
            {
                SelectedObjectChanged(this, new EventArgs());
            }
        }

        private void updateSelectedTreeNodes()
        {
            if (tvItems.SelectedNodes.Count != 0)
            {
                List<EveObject> selectedObjects = new List<EveObject>();
                foreach (TreeNode node in tvItems.SelectedNodes)
                {
                    selectedObjects.Add(node.Tag as EveObject);
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
            updateSelectedTreeNodes();
        }

        private void lbSearchList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbSearchList.SelectedItems.Count != 0)
            {
                List<EveObject> selectedObjects = new List<EveObject>();
                foreach (Object node in lbSearchList.SelectedItems)
                {
                    selectedObjects.Add(node as EveObject);
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
    }
}
