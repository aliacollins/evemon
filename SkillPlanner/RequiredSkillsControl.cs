using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using System.Globalization;

namespace EVEMon.SkillPlanner
{
    /// <summary>
    /// User control to display required skills for a given eveobject and update a plan object for requirements not met
    /// </summary>
    public partial class RequiredSkillsControl : UserControl
    {
        #region Private Properties
        private EveObject   m_EveItem;
        private Plan        m_Plan;
        private bool        m_allSkillsKnown;
        private bool        m_skillsUnplanned;
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public RequiredSkillsControl()
        {
            InitializeComponent();
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// An EveObject for which we want to show required skills
        /// </summary>
        public EveObject EveItem
        {
            get { return m_EveItem; }
            set
            {
                m_EveItem = value;
                UpdateDisplay();
            }
        }
        /// <summary>
        /// The target Plan object to add any required skills
        /// </summary>
        public Plan Plan
        {
            get { return m_Plan; }
            set { m_Plan = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Updates control contents
        /// </summary>
        public void UpdateDisplay()
        {
            // List of skills for which to calculate training time
            List<Pair<Skill, int>> reqSkills = new List<Pair<Skill, int>>();

            // Default all known flag to true. Will be set to false in getSkillNode() if a requirement is not met
            m_allSkillsKnown = true;

            // Default unplanned skills flag to false. Will be set to true in getSkillNode() if a requirement is neither met nor planned
            m_skillsUnplanned = false;

            // Treeview update
            tvSkillList.BeginUpdate();
            tvSkillList.Nodes.Clear();
            if (m_EveItem != null)
            {
                foreach (EntityRequiredSkill requiredSkill in m_EveItem.RequiredSkills)
                {
                    // Add required skill to treeview root
                    tvSkillList.Nodes.Add(GetSkillNode(requiredSkill));

                    // Add required skill to reqSkills list to calculate training time
                    Pair<Skill, int> p = new Pair<Skill, int>();
                    p.A = m_Plan.GrandCharacterInfo.GetSkill(requiredSkill.Name);
                    p.B = requiredSkill.Level;
                    reqSkills.Add(p);
                }
            }
            tvSkillList.EndUpdate();

            // Set training time required label
            if (m_allSkillsKnown)
            {
                lblTimeRequired.Text = "No training required";
            }
            else
            {
                TimeSpan trainTime = m_Plan.GrandCharacterInfo.GetTrainingTimeToMultipleSkills(reqSkills);
                lblTimeRequired.Text = Skill.TimeSpanToDescriptiveText(trainTime, DescriptiveTextOptions.IncludeCommas);
            }

            // Enable / disable button
            btnAddSkills.Enabled = m_skillsUnplanned;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Recursive method to generate treenodes for tvSkillList
        /// </summary>
        /// <param name="requiredSkill">An EntityRequiredSkill object</param>
        /// <returns></returns>
        private TreeNode GetSkillNode(EntityRequiredSkill requiredSkill)
        {
            // Get Skill for requiredSkill
            Skill thisSkill = m_Plan.GrandCharacterInfo.GetSkill(requiredSkill.Name);

            // Create new node
            TreeNode skillNode = new TreeNode(requiredSkill.Name + " " + Skill.GetRomanForInt(requiredSkill.Level));

            // Store skill for use by double click event
            skillNode.Tag = thisSkill;

            #region Check requirements and apply node icons
            // Skill requirement met
            if (thisSkill.Level >= requiredSkill.Level)
            {
                skillNode.ImageIndex = 1;
                skillNode.SelectedImageIndex = 1;
            }
            // Requirement not met, but planned
            else if (m_Plan.IsPlanned(thisSkill, requiredSkill.Level))
            {
                skillNode.ImageIndex = 2;
                skillNode.SelectedImageIndex = 2;
                m_allSkillsKnown = false;
            }
            // Requirement not met
            else
            {
                skillNode.ImageIndex = 0;
                skillNode.SelectedImageIndex = 0;
                m_allSkillsKnown = false;
                m_skillsUnplanned = true;
            }
            #endregion

            // Generate child nodes if required
            foreach (Skill.Prereq prerequisite in thisSkill.Prereqs)
            {
                skillNode.Nodes.Add(GetSkillNode(prerequisite as EntityRequiredSkill));
            }

            return skillNode;
        }
        /// <summary>
        /// Event handler method for Add Skills button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddSkills_Click(object sender, EventArgs e)
        {
            // List to hold required skills and level
            List<Pair<string, int>> skillsToAdd = new List<Pair<string, int>>();

            // Populate list
            foreach (EntityRequiredSkill requiredSkill in m_EveItem.RequiredSkills)
            {
                skillsToAdd.Add(new Pair<string, int>(requiredSkill.Name, requiredSkill.Level));
            }

            // Update plan
            m_Plan.PlanSetTo(skillsToAdd, m_EveItem.Name, true);

            // Refresh display to reflect plan changes
            UpdateDisplay();
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
            if (thisNode.Tag != null)
            {
                // Open skill browser tab for this skill
                NewPlannerWindow pw = m_Plan.PlannerWindow.Target as NewPlannerWindow;
                pw.ShowSkillInTree(thisNode.Tag as Skill);
            }
        }
        #endregion

    }

    /// <summary>
    /// Derived from TreeView class
    /// <para>Overrides standard node double click behaviour to prevent node expand / collapse actions</para>
    /// </summary>
    class ReqSkillsTreeView : TreeView
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
    }

}
