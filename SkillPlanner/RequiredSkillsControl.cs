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
    public partial class RequiredSkillsControl : UserControl
    {
        public RequiredSkillsControl()
        {
            InitializeComponent();
        }

        private EveObject m_EveItem;

        public EveObject EveItem
        {
            get { return m_EveItem; }
            set
            {
                m_EveItem = value;
                updateDisplay();
            }
        }

        private Plan m_Plan;

        public Plan Plan
        {
            get { return m_Plan; }
            set { m_Plan = value; }
        }

        private bool m_allSkillsKnown;
        private bool m_skillsUnplanned;

        private void updateDisplay()
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
                    tvSkillList.Nodes.Add(getSkillNode(requiredSkill));

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

        private TreeNode getSkillNode(EntityRequiredSkill requiredSkill)
        {
            TreeNode skillNode = new TreeNode(requiredSkill.Name + " " + Skill.GetRomanForInt(requiredSkill.Level));
            Skill thisSkill = m_Plan.GrandCharacterInfo.GetSkill(requiredSkill.Name);
            skillNode.Tag = thisSkill;
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
            foreach (Skill.Prereq prerequisite in thisSkill.Prereqs)
            {
                skillNode.Nodes.Add(getSkillNode(prerequisite as EntityRequiredSkill));
            }
            return skillNode;
        }

        private void btnAddSkills_Click(object sender, EventArgs e)
        {
            List<Pair<string, int>> skillsToAdd = new List<Pair<string, int>>();
            foreach (EntityRequiredSkill requiredSkill in m_EveItem.RequiredSkills)
            {
                skillsToAdd.Add(new Pair<string, int>(requiredSkill.Name, requiredSkill.Level));
            }
            m_Plan.PlanSetTo(skillsToAdd, m_EveItem.Name, true);
            updateDisplay();
        }

        private void tvSkillList_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode thisNode = e.Node as TreeNode;
            if (thisNode.Tag == null) return;
            NewPlannerWindow pw = m_Plan.PlannerWindow.Target as NewPlannerWindow;
            pw.ShowSkillInTree(thisNode.Tag as Skill);
        }

    }

    /// <summary>
    /// ReqSkillsTreeView class
    /// Derived from TreeView
    /// Overrides standard node double click behaviour to prevent node expand / collapse actions
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
