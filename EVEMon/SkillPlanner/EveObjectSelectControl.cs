using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.Data;

namespace EVEMon.SkillPlanner
{
    public partial class EveObjectSelectControl : UserControl
    {
        public event EventHandler SelectionChanged;

        /// <summary>
        /// Constructor.
        /// </summary>
        protected EveObjectSelectControl()
        {
            InitializeComponent();
        }


        #region Properties

        /// <summary>
        /// Gets or sets the plan.
        /// </summary>
        [Browsable(false)]
        public Plan Plan { get; set; }

        /// <summary>
        /// Gets or sets the usability predicate.
        /// </summary>
        /// <value>The usability predicate.</value>
        protected Func<Item, Boolean> UsabilityPredicate { get; set; }

        /// <summary>
        /// Gets or sets the activity filter.
        /// </summary>
        /// <value>The activity filter.</value>
        protected ObjectActivityFilter ActivityFilter { get; set; }

        /// <summary>
        /// Gets or sets the activity.
        /// </summary>
        /// <value>The activity.</value>
        protected BlueprintActivity Activity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [all expanded].
        /// </summary>
        /// <value><c>true</c> if [all expanded]; otherwise, <c>false</c>.</value>
        protected bool AllExpanded { get; set; }

        #endregion


        /// <summary>
        /// Occurs when the control is loaded.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            UsabilityPredicate = SelectAll;

            if (DesignMode || this.IsDesignModeHosted())
                return;

            // Subscribe the events
            EveMonClient.SettingsChanged += EveMonClient_SettingsChanged;
            Disposed += OnDisposed;

            // Update the controls
            UpdateControlVisibility();
        }

        /// <summary>
        /// Handles the SettingsChanged event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EveMonClient_SettingsChanged(object sender, EventArgs e)
        {
            UpdateControlVisibility();
        }

        /// <summary>
        /// Called when [disposed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnDisposed(object sender, EventArgs e)
        {
            EveMonClient.SettingsChanged -= EveMonClient_SettingsChanged;
            Disposed -= OnDisposed;
        }

        /// <summary>
        /// Updates the control visibility.
        /// </summary>
        private void UpdateControlVisibility()
        {
            pbSearchImage.Visible = !Settings.UI.SafeForWork;
        }


        #region Search

        /// <summary>
        /// Occurs when clicking on the search text control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbSearchTextHint_Click(object sender, EventArgs e)
        {
            tbSearchText.Focus();
        }

        /// <summary>
        /// Occurs upon entering the search text control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSearchText_Enter(object sender, EventArgs e)
        {
            lbSearchTextHint.Visible = false;
        }

        /// <summary>
        /// Occurs upon leaving the search text control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSearchText_Leave(object sender, EventArgs e)
        {
            lbSearchTextHint.Visible = String.IsNullOrEmpty(tbSearchText.Text);
        }

        /// <summary>
        /// Occurs when the search text changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSearchText_TextChanged(object sender, EventArgs e)
        {
            OnSearchTextChanged(tbSearchText.Text);
        }

        /// <summary>
        /// Updates the control when the search text changes.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        protected virtual void OnSearchTextChanged(string searchText)
        {
            if (!tbSearchText.Focused)
                lbSearchTextHint.Visible = String.IsNullOrEmpty(searchText);

            BuildListView();
        }

        /// <summary>
        /// Parses the tree node and extracts all the items to build the content of the list box. 
        /// It also deals with text filtering and the treeview/listbox visibility.
        /// </summary>
        protected void BuildListView()
        {
            string searchText = tbSearchText.Text.Trim().ToLower(CultureConstants.DefaultCulture);

            if (String.IsNullOrEmpty(searchText))
            {
                tvItems.Visible = true;
                lbSearchList.Visible = false;
                lbNoMatches.Visible = false;
                return;
            }

            // Find everything in the current tree that matches the search string
            List<Item> filteredItems = new List<Item>();
            foreach (TreeNode n in tvItems.Nodes)
            {
                SearchNode(n, searchText, filteredItems);
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

        /// <summary>
        /// Creates the node.
        /// </summary>
        /// <param name="tn"></param>
        /// <param name="searchText"></param>
        /// <param name="filteredItems"></param>
        private static void SearchNode(TreeNode tn, string searchText, List<Item> filteredItems)
        {
            Item item = tn.Tag as Item;
            if (item == null)
            {
                foreach (TreeNode subNode in tn.Nodes)
                {
                    SearchNode(subNode, searchText, filteredItems);
                }
                return;
            }

            if (item.Name.ToLower(CultureConstants.DefaultCulture).Contains(searchText)
                || item.Description.ToLower(CultureConstants.DefaultCulture).Contains(searchText))
                filteredItems.Add(item);
        }

        /// <summary>
        /// Occurs when pressing a key while inside the search text control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSearchText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 0x01)
                return;

            tbSearchText.SelectAll();
            e.Handled = true;
        }

        #endregion


        #region Selected Objects

        /// <summary>
        /// All the selected objects (through multi-select).
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DefaultValue(null), Browsable(false)]
        public IEnumerable<Item> SelectedObjects { get; private set; }

        /// <summary>
        /// The primary selected object.
        /// </summary>
        [Browsable(false)]
        public Item SelectedObject
        {
            get
            {
                if (SelectedObjects == null || !SelectedObjects.Any())
                    return null;

                return SelectedObjects.First();
            }
            set
            {
                List<Item> selectedObjects = new List<Item>();
                if (value != null)
                    selectedObjects.Add(value);

                SetSelectedObjects(selectedObjects);
            }
        }

        /// <summary>
        /// Selects the given nodes.
        /// </summary>
        /// <param name="items"></param>
        private void SetSelectedObjects(IEnumerable<Item> items)
        {
            // Updates selection
            SelectedObjects = (items == null ? new List<Item>() : new List<Item>(items));

            // Selects the proper nodes
            if (SelectedObjects.Count() == 1)
            {
                // If the object is not already selected
                Item obj = SelectedObjects.First();
                tvItems.SelectNodeWithTag(obj);
            }

            // Notify subscribers
            if (SelectionChanged != null)
                SelectionChanged(this, new EventArgs());
        }

        /// <summary>
        /// Update the selected tree node.
        /// </summary>
        private void UpdateSelectedTreeNodes()
        {
            if (tvItems.SelectedNodes.Count != 0)
            {
                List<Item> selectedObjects = tvItems.SelectedNodes.Select(node => node.Tag).OfType<Item>().ToList();
                SetSelectedObjects(selectedObjects);
                return;
            }

            SetSelectedObjects(null);
        }

        #endregion


        #region Events

        /// <summary>
        /// Occurs when selection changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvItems_SelectionsChanged(object sender, EventArgs e)
        {
            UpdateSelectedTreeNodes();
        }

        /// <summary>
        /// Occurs when the search list selection changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbSearchList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbSearchList.SelectedItems.Count != 0)
            {
                List<Item> selectedObjects = lbSearchList.SelectedItems.OfType<Item>().ToList();
                SetSelectedObjects(selectedObjects);
                return;
            }

            SetSelectedObjects(null);
        }

        /// <summary>
        /// Treeview's context menu > Expand all.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmiExpandAll_Click(object sender, EventArgs e)
        {
            tvItems.ExpandAll();
            AllExpanded = true;
        }

        /// <summary>
        /// Treeview's context menu > Collapse all.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmiCollapseAll_Click(object sender, EventArgs e)
        {
            tvItems.CollapseAll();
            AllExpanded = false;
        }

        /// <summary>
        /// Treeview's context menu > Expand.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmiExpandSelected_Click(object sender, EventArgs e)
        {
            tvItems.SelectedNode.Expand();
        }

        /// <summary>
        /// Treeview's context menu > Collapse.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmiCollapseSelected_Click(object sender, EventArgs e)
        {
            tvItems.SelectedNode.Collapse();
        }

        /// <summary>
        /// Occurs upon opening the context menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenu_Opening(object sender, CancelEventArgs e)
        {
            TreeNode node = tvItems.SelectedNode;

            tsSeparator.Visible = (node != null && node.GetNodeCount(true) > 0);

            // "Expand" and "Collapse" selected menu
            cmiExpandSelected.Visible = (node != null && node.GetNodeCount(true) > 0 && !node.IsExpanded);
            cmiCollapseSelected.Visible = (node != null && node.GetNodeCount(true) > 0 && node.IsExpanded);

            cmiExpandSelected.Text = (node != null && node.GetNodeCount(true) > 0 && !node.IsExpanded
                                          ? String.Format(CultureConstants.DefaultCulture, "Expand \"{0}\"",
                                                          node.Text.Replace("&", "&&"))
                                          : String.Empty);
            cmiCollapseSelected.Text = (node != null && node.GetNodeCount(true) > 0 && node.IsExpanded
                                            ? String.Format(CultureConstants.DefaultCulture, "Collapse \"{0}\"",
                                                            node.Text.Replace("&", "&&"))
                                            : String.Empty);

            // "Expand All" and "Collapse All" menu
            cmiCollapseAll.Enabled = cmiCollapseAll.Visible = AllExpanded;
            cmiExpandAll.Enabled = cmiExpandAll.Visible = !cmiCollapseAll.Enabled;
        }

        #endregion


        #region Predicates

        /// <summary>
        /// Filter for all items.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected bool SelectAll(Item item)
        {
            return true;
        }

        /// <summary>
        /// Filter for items which can be used (prereqs met).
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected bool CanUse(Item item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            IEnumerable<StaticSkillLevel> prerequisites =
                item.Prerequisites.Where(x => x.Activity != BlueprintActivity.ReverseEngineering);
            bool bpBrowserControl = this is BlueprintSelectControl;

            // Is item a blueprint and supports the selected activity ?  
            if (bpBrowserControl)
            {
                bool hasSelectedActivity = prerequisites.Any(x => x.Activity == Activity)
                                           || ((Blueprint)item).MaterialRequirements.Any(x => x.Activity == Activity);

                // Can not be used when item doesn't support the selected activity
                if ((ActivityFilter == ObjectActivityFilter.Manufacturing || ActivityFilter == ObjectActivityFilter.Invention)
                    && !hasSelectedActivity)
                    return false;

                // Enumerates the prerequisites skills to the selected activity 
                if (ActivityFilter != ObjectActivityFilter.All && ActivityFilter != ObjectActivityFilter.Any)
                    prerequisites = prerequisites.Where(x => x.Activity == Activity);
            }

            // Item doesn't have prerequisites skills
            if (!prerequisites.Any())
                return true;

            // Is this the "Blueprint Browser" and the activity filter is set to "Any" ?
            List<Boolean> prereqTrained = new List<Boolean>();
            if (bpBrowserControl && ActivityFilter == ObjectActivityFilter.Any)
            {
                List<BlueprintActivity> prereqActivity = new List<BlueprintActivity>();

                // Create a list with the activities this item supports
                foreach (StaticSkillLevel prereq in prerequisites.Where(x => !prereqActivity.Contains(x.Activity)))
                {
                    prereqActivity.Add(prereq.Activity);
                }

                // Create a list with each prereq skill trained status for the questioned activity
                foreach (BlueprintActivity activity in prereqActivity)
                {
                    prereqTrained.Clear();

                    prereqTrained.AddRange(prerequisites.Where(x => x.Activity == activity).Select(
                        prereq => new
                                      {
                                          prereq,
                                          level = Plan.Character.GetSkillLevel(prereq.Skill)
                                      }).Select(y => y.level >= y.prereq.Level));

                    // Has the character trained all prereq skills for this activity ?
                    if (!prerequisites.Any() || prereqTrained.All(x => x))
                        return true;
                }
                return false;
            }

            // Do a simple predication and create a list with each prereq skill trained status
            prereqTrained.AddRange(prerequisites.Select(
                prereq => new
                              {
                                  prereq,
                                  level = Plan.Character.GetSkillLevel(prereq.Skill)
                              }).Select(y => y.level >= y.prereq.Level));

            // Has the character trained all prereq skills ?
            bool d = (prereqTrained.All(x => x));
            return d;
        }

        /// <summary>
        /// Filter for items which can not be used (prereqs not met).
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected bool CannotUse(Item item)
        {
            Blueprint blueprint = item as Blueprint;
            bool hasActivity = blueprint == null
                               || (ActivityFilter == ObjectActivityFilter.All || ActivityFilter == ObjectActivityFilter.Any
                                   || blueprint.Prerequisites.Any(x => x.Activity.ToString() == ActivityFilter.ToString())
                                   || blueprint.MaterialRequirements.Any(x => x.Activity.ToString() == ActivityFilter.ToString()));

            return !CanUse(item) && hasActivity;
        }

        #endregion
    }
}