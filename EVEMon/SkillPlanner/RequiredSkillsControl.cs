using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using System.Globalization;
using EVEMon.Common.Controls;
using EVEMon.Common.Data;

namespace EVEMon.SkillPlanner
{
    /// <summary>
    /// User control to display required skills for a given eveobject and update a plan object for requirements not met
    /// </summary>
    public partial class RequiredSkillsControl : UserControl
    {
        private Item m_object;
        private Plan m_plan;

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public RequiredSkillsControl()
        {
            InitializeComponent();
            this.Disposed += new EventHandler(OnDisposed);
            EveClient.PlanChanged += new EventHandler<PlanChangedEventArgs>(EveClient_PlanChanged);
        }

        /// <summary>
        /// Unsubscribe events on disposing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDisposed(object sender, EventArgs e)
        {
            this.Disposed -= new EventHandler(OnDisposed);
            EveClient.PlanChanged -= new EventHandler<PlanChangedEventArgs>(EveClient_PlanChanged);
        }

        /// <summary>
        /// Occurs when the plan changes, when update the display.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EveClient_PlanChanged(object sender, PlanChangedEventArgs e)
        {
            UpdateDisplay();
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// An EveObject for which we want to show required skills
        /// </summary>
        public Item Object
        {
            get { return m_object; }
            set
            {
                m_object = value;
                UpdateDisplay();
            }
        }
        /// <summary>
        /// The target Plan object to add any required skills
        /// </summary>
        public Plan Plan
        {
            get { return m_plan; }
            set
            {
                m_plan = value;
                UpdateDisplay();
            }
        }

        #endregion

        #region Content creation
        /// <summary>
        /// Updates control contents
        /// </summary>
        private void UpdateDisplay()
        {
            // Default all known flag to true. Will be set to false in getSkillNode() if a requirement is not met
            bool allSkillsKnown = true;

            // Default unplanned skills flag to false. Will be set to true in getSkillNode() if a requirement is neither met nor planned
            bool skillsUnplanned = false;

            // Treeview update
            tvSkillList.BeginUpdate();
            try
            {
                tvSkillList.Nodes.Clear();
                if (m_object != null && m_plan != null)
                {
                    // Recursively create nodes
                    foreach (StaticSkillLevel prereq in m_object.Prerequisites)
                    {
                        tvSkillList.Nodes.Add(GetSkillNode(prereq, ref allSkillsKnown, ref skillsUnplanned));
                    }
                }
            }
            finally
            {
                tvSkillList.EndUpdate();
            }

            // Set training time required label
            if (allSkillsKnown)
            {
                lblTimeRequired.Text = "No training required";
            }
            else
            {
                TimeSpan trainTime = m_plan.Character.GetTrainingTimeToMultipleSkills(m_object.Prerequisites);
                lblTimeRequired.Text = Skill.TimeSpanToDescriptiveText(trainTime, DescriptiveTextOptions.IncludeCommas);
            }

            // Set minimun control size
            Size timeRequiredTextSize = TextRenderer.MeasureText(lblTimeRequired.Text, Font);
            Size newMinimumSize = new Size(timeRequiredTextSize.Width + btnAddSkills.Width, 0);
            if (this.MinimumSize.Width < newMinimumSize.Width) this.MinimumSize = newMinimumSize;

            // Enable / disable button
            btnAddSkills.Enabled = skillsUnplanned;
        }

        /// <summary>
        /// Recursive method to generate treenodes for tvSkillList
        /// </summary>
        /// <param name="requiredSkill">An EntityRequiredSkill object</param>
        /// <returns></returns>
        private TreeNode GetSkillNode(StaticSkillLevel prereq, ref bool allSkillsKnown, ref bool skillsUnplanned)
        {
            var character = (Character)m_plan.Character;
            Skill skill = character.Skills[prereq.Skill];

            TreeNode node = new TreeNode(prereq.ToString());
            node.Tag = new SkillLevel(skill, prereq.Level);

            // Skill requirement met
            if (skill.Level >= prereq.Level)
            {
                node.ImageIndex = 1;
                node.SelectedImageIndex = 1;
            }
            // Requirement not met, but planned
            else if (m_plan.IsPlanned(skill, prereq.Level))
            {
                node.ImageIndex = 2;
                node.SelectedImageIndex = 2;
                allSkillsKnown = false;
            }
            // Requirement not met
            else
            {
                node.ImageIndex = 0;
                node.SelectedImageIndex = 0;
                allSkillsKnown = false;
                skillsUnplanned = true;
            }

            // Generate child nodes if required
            foreach (StaticSkillLevel childPrereq in skill.StaticData.Prerequisites)
            {
                node.Nodes.Add(GetSkillNode(childPrereq, ref allSkillsKnown, ref skillsUnplanned));
            }

            return node;
        }
        #endregion

        #region Controls' handlers
        /// <summary>
        /// Event handler method for Add Skills button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddSkills_Click(object sender, EventArgs e)
        {
            // Add skills to plan
            var operation = m_plan.TryAddSet(m_object.Prerequisites, m_object.Name);
            PlanHelper.Perform(operation);

            // Refresh display to reflect plan changes
            UpdateDisplay();
        }

        /// <summary>
        /// On a right-click, we ensure the node is selected before the menu is opened.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvSkillList_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            tvSkillList.SelectedNode = e.Node;
        }

        /// <summary>
        /// Event handler for treenode double click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvSkillList_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // Get selected node
            TreeNode thisNode = e.Node as TreeNode;

            // Make sure we have a skill to use
            if (thisNode.Tag == null) return;

            // Open skill browser tab for this skill
            PlanWindow pw = WindowsFactory<PlanWindow>.GetByTag(m_plan);
            Skill skill = ((SkillLevel) thisNode.Tag).Skill;
            pw.ShowSkillInBrowser(skill);
        }
        #endregion

        #region Context menu
        /// <summary>
        /// Context menu opening, updates the "plan to" menus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void contextMenu_Opening(object sender, CancelEventArgs e)
        {
            if (tvSkillList.SelectedNode == null)
            {
                e.Cancel = true;
                return;
            }

            // "Plan to N" menus
            var skillLevel = (SkillLevel)tvSkillList.SelectedNode.Tag;
            var skill = skillLevel.Skill;
            for (int i = 0; i <= 5; i++)
            {
                PlanHelper.UpdatesRegularPlanToMenu(planToMenu.DropDownItems[i], m_plan, skill, i);
            }
        }

        /// <summary>
        /// Context > Show in Skills Browser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showInSkillsBrowserMenu_Click(object sender, EventArgs e)
        {
            // Retrieve the owner window
            PlanWindow npw = WindowsFactory<PlanWindow>.GetByTag(m_plan);
            if (npw == null || npw.IsDisposed) return;

            // Open the skill explorer
            var skillLevel = (SkillLevel)tvSkillList.SelectedNode.Tag;
            npw.ShowSkillInBrowser(skillLevel.Skill);
        }

        /// <summary>
        /// Context > Show in Skill Explorer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showInSkillsExplorerMenu_Click(object sender, EventArgs e)
        {
            // Retrieve the owner window
            PlanWindow npw = WindowsFactory<PlanWindow>.GetByTag(m_plan);
            if (npw == null || npw.IsDisposed) return;

            // Open the skill explorer
            var skillLevel = (SkillLevel)tvSkillList.SelectedNode.Tag;
            npw.ShowSkillInExplorer(skillLevel.Skill);
        }

        /// <summary>
        /// Context > Plan To > Level N
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void planToMenuItem_Click(object sender, EventArgs e)
        {
            var menu = (ToolStripMenuItem)sender;
            var operation = (IPlanOperation)menu.Tag;
            PlanHelper.PerformSilently(operation);
        }
        #endregion
    }
    
    #region ReqSkillsTreeView
    /// <summary>
    /// Derived from TreeView class
    /// <para>Overrides standard node double click behaviour to prevent node expand / collapse actions</para>
    /// </summary>
    class ReqSkillsTreeView : System.Windows.Forms.TreeView
    {
        private const int WM_LBUTTONDBLCLK = 0x203;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_LBUTTONDBLCLK)
            {
                handleDoubleClick(ref m);
            }
            else { base.WndProc(ref m); };
        }

        private void handleDoubleClick(ref Message m)
        {
            // Get mouse location from message.lparam
            // x is low order word, y is high order word
            string lparam = m.LParam.ToString("X08");
            int x = int.Parse(lparam.Substring(4, 4), NumberStyles.HexNumber);
            int y = int.Parse(lparam.Substring(0, 4), NumberStyles.HexNumber);
            // Test for a treenode at this location
            TreeViewHitTestInfo info = this.HitTest(x, y);
            if (info.Node != null)
            {
                // Raise NodeMouseDoubleClick event
                TreeNodeMouseClickEventArgs e = new TreeNodeMouseClickEventArgs(info.Node, MouseButtons.Left, 2, x, y);
                this.OnNodeMouseDoubleClick(e);
            }
        }

        protected override void CreateHandle()
       {
           if (!this.IsDisposed)
               base.CreateHandle();
       }
    }
    #endregion
}
