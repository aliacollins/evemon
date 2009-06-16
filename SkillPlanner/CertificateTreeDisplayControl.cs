using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon.SkillPlanner
{
    public partial class CertificateTreeDisplayControl : UserControl
    {
        private const int GrantedIcon = 0;
        private const int ClaimableIcon = 1;
        private const int UnknownButTrainableIcon = 2;
        private const int UnknownIcon = 3;
        private const int CertificateIcon = 4;
        private const int SkillIcon = 5;

        private Plan m_plan;
        private CharacterInfo m_character;
        private CertificateClass m_class;
        private Font m_boldFont;

        public event EventHandler SelectionChanged;

        public CertificateTreeDisplayControl()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            InitializeComponent();

            treeView.DrawNode += new DrawTreeNodeEventHandler(treeView_DrawNode);
            treeView.MouseDown += new MouseEventHandler(treeView_MouseDown);

            cmListSkills.Opening += new CancelEventHandler(cmListSkills_Opening);
            m_boldFont = FontHelper.GetFont(this.Font, FontStyle.Bold);
            this.treeView.Font = FontHelper.GetFont("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
        }

        /// <summary>
        /// Gets or sets the character this control is bound to
        /// </summary>
        public CharacterInfo Character
        {
            get { return this.m_character; }
            set 
            {
                if (m_character != null) m_character.SkillChanged -= new SkillChangedHandler(OnSkillChanged);
                this.m_character = value;
                if (m_character != null) m_character.SkillChanged += new SkillChangedHandler(OnSkillChanged);
                this.treeView.Nodes.Clear();
                UpdateTree();
            }
        }

        /// <summary>
        /// Gets or sets the current plan
        /// </summary>
        public Plan Plan
        {
            get { return m_plan; }
            set { m_plan = value; }
        }

        /// <summary>
        /// Gets or sets the certificate class (i.e. "Core competency")
        /// </summary>
        public CertificateClass CertificateClass
        {
            get { return this.m_class; }
            set 
            {
                if (value != m_class)
                {
                    this.m_class = value;
                    UpdateTree();
                }
            }
        }

        /// <summary>
        /// Gets the selected certificate, null if none is selected. 
        /// Note that the selected certificate may be a prerequsite and therefore not have the same class than 
        /// the one represented by this control.
        /// </summary>
        public Certificate SelectedCertificate
        {
            get
            {
                if (this.treeView.SelectedNode == null) return null;
                return this.treeView.SelectedNode.Tag as Certificate;
            }
        }

        /// <summary>
        /// Gets cert of the displayed class which contains the current selection
        /// </summary>
        public Certificate SelectedCertificateLevel
        {
            get
            {
                TreeNode curr = treeView.SelectedNode;
                while (curr != null)
                {
                    Certificate c = curr.Tag as Certificate;
                    if (c != null && c.Class.Equals(this.CertificateClass))
                    {
                        return c;
                    }
                    curr = curr.Parent;
                }
                return null;
            }
        }

        
        /// <summary>
        /// Expands the node representing this certificate
        /// </summary>
        /// <param name="cert"></param>
        public void ExpandCert(Certificate cert)
        {
            foreach (TreeNode n in this.treeView.Nodes)
            {
                if (n.Tag.Equals(cert))
                {
                    this.treeView.SelectedNode = n;
                    n.Expand();
                    if (this.SelectionChanged != null)
                    {
                        this.SelectionChanged(this, new EventArgs());
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Fired when one of the character's skill changed (SP changed, learnt, etc)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSkillChanged(Object sender, SkillChangedEventArgs e)
        {
            //UpdateTree();
        }

        /// <summary>
        /// When the treeview is clicked, we manually select nodes since the bounding boxes are incorrect due to custom draw
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView_MouseDown(object sender, MouseEventArgs e)
        {
            // Perform the selection manuall since the bound's width and x are incorrect in owndraw
            TreeNode selection = null;
            for (TreeNode node = treeView.TopNode; node != null; node = node.NextVisibleNode)
            {
                if (node.Bounds.Top <= e.Y && node.Bounds.Bottom >= e.Y)
                {
                    // If the user clicked the "arrow zone", we do not change the selection and just return
                    if (e.X < (node.Bounds.Left - 32)) return;
                    selection = node;
                    break;
                }
            }

            // Updates the selection and fire the appropriate event
            if (selection != treeView.SelectedNode)
            {
                treeView.SelectedNode = selection;
                if (this.SelectionChanged != null)
                {
                    this.SelectionChanged(this, new EventArgs());
                }
            }
        }

        #region Tree building
        /// <summary>
        /// Update the whole tree
        /// </summary>
        private void UpdateTree()
        {
            Certificate oldSelection = this.SelectedCertificate;
            TreeNode newSelection = null;

            this.treeView.BeginUpdate();
            try
            {
                // Clear the old items
                this.treeView.Nodes.Clear();

                // No update when not fully initialized
                if (m_character == null || m_class == null) return;

                // Create the nodes when not done, yet
                if (this.treeView.Nodes.Count == 0)
                {
                    foreach (var cert in this.m_class.Certificates) 
                    {
                        var node = CreateNode(cert);
                        this.treeView.Nodes.Add(node);

                        // Does the old selection still exists ?
                        if (cert == oldSelection) newSelection = node;

                    }
                }
                
                // Update the nodes
                foreach (TreeNode node in this.treeView.Nodes) UpdateNode(node);

                // Is the old selection kept ? Then we select the matching node
                if (newSelection != null)
                {
                    this.treeView.SelectedNode = newSelection;
                }
            }
            finally
            {
                this.treeView.EndUpdate();
            }

            // Fire the SelectionChanged event if the old selection doesn't exist anymore
            if (newSelection == null)
            {
                if (this.SelectionChanged != null)
                {
                    this.SelectionChanged(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// Create a node from a prerequisite certificate
        /// </summary>
        /// <param name="cert"></param>
        /// <returns></returns>
        private TreeNode CreateNode(Certificate cert)
        {
            TreeNode node = new TreeNode();
            node.Tag = cert;

            foreach (var prereqCert in cert.PrerequisiteCertificates)
            {
                node.Nodes.Add(CreateNode(prereqCert));
            }
            foreach (var prereqSkill in cert.PrerequisiteSkills)
            {
                node.Nodes.Add(CreateNode(prereqSkill));
            }

            return node;
        }

        /// <summary>
        /// Create a node from a prerequisite skill
        /// </summary>
        /// <param name="skillPrereq"></param>
        /// <returns></returns>
        private TreeNode CreateNode(StaticSkill.Prereq skillPrereq)
        {
            TreeNode node = new TreeNode();
            node.Tag = skillPrereq;

            foreach (var prereqSkill in skillPrereq.Skill.Prereqs)
            {
                node.Nodes.Add(CreateNode(prereqSkill));
            }

            return node;
        }

        /// <summary>
        /// Updates the specified node and its children
        /// </summary>
        /// <param name="node"></param>
        private void UpdateNode(TreeNode node)
        {
            Certificate cert = node.Tag as Certificate;

            // The node represents a certificate
            if (cert != null)
            {
                node.Text = cert.Class.Name + " " + cert.Grade.ToString();
                node.ToolTipText = cert.Description;
                node.StateImageIndex = CertificateIcon;

                var status = m_character.GetCertificateStatus(cert);
                switch (status)
	            {
		            case CertificateStatus.Granted:
                        node.ImageIndex = GrantedIcon;
                        break;
                    case CertificateStatus.Claimable:
                        node.ImageIndex = ClaimableIcon;
                        break;
                    case CertificateStatus.PartiallyTrained:
                        node.ImageIndex = UnknownButTrainableIcon;
                        break;
                    case CertificateStatus.Untrained:
                        node.ImageIndex = UnknownIcon;
                        break;
                    default:
                        throw new NotImplementedException();
	            }
            }
            // The node represents a skill prerequisite
            else
            {
                StaticSkill.Prereq skillPrereq = (StaticSkill.Prereq)node.Tag;
                var skill = m_character.AllSkillsByTypeID[skillPrereq.Skill.Id];

                node.Text = skill.Name;
                node.ToolTipText = skill.Description;
                node.StateImageIndex = SkillIcon;

                if (skill.LastConfirmedLvl >= skillPrereq.Level) node.ImageIndex = GrantedIcon;
                else if (skill.Known) node.ImageIndex = UnknownButTrainableIcon;
                else node.ImageIndex = UnknownIcon;
            }

            // When selected, the image remains the same
            node.SelectedImageIndex = node.ImageIndex;

            // Update the children
            foreach (TreeNode child in node.Nodes)
            {
                UpdateNode(child);
            }
        }

        /// <summary>
        /// Custom draw for the label
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            // Prevents a bug that causes every item to be redrawn at the top left corner
            if (e.Bounds.Left <= 10)
            {
                e.DrawDefault = true;
                return;
            }

            string line1 = "";
            string line2 = "-";
            int supIcon = -1;

            // Is it a certificate ?
            Certificate cert = e.Node.Tag as Certificate;
            if (cert != null)
            {
                line1 = cert.ToString();
                supIcon = CertificateIcon;
                var status = m_character.GetCertificateStatus(cert);

                // When not granted or claimable, let's display the training time
                if (status != CertificateStatus.Claimable && status != CertificateStatus.Granted)
                {
                    var time = cert.ComputeTrainingTime(m_character);
                    line2 = Skill.TimeSpanToDescriptiveText(time, DescriptiveTextOptions.IncludeCommas);
                }
            }
            // Or a skill prerequsiite ?
            else
            {
                supIcon = SkillIcon;

                StaticSkill.Prereq skillPrereq = e.Node.Tag as StaticSkill.Prereq;
                line1 = skillPrereq.Name + " " + Skill.GetRomanForInt(skillPrereq.Level);

                // When not known to the require level, let's display the training time
                var skill = m_character.GetSkill(skillPrereq.Name);
                if (skill.LastConfirmedLvl < skillPrereq.Level)
                {
                    var time = skill.GetTrainingTimeToLevel(skillPrereq.Level);
                    line2 = Skill.TimeSpanToDescriptiveText(time, DescriptiveTextOptions.IncludeCommas);
                }
            }

            // Choose colors according to selection
            bool isSelected = ((e.State & TreeNodeStates.Selected) == TreeNodeStates.Selected);
            var backColor = (isSelected ? SystemColors.Highlight : this.treeView.BackColor);
            var foreColor = (isSelected ? SystemColors.HighlightText : this.treeView.ForeColor);
            var lightForeColor = (isSelected ? SystemColors.HighlightText : SystemColors.GrayText);

            // Draws the background
            using (var background = new SolidBrush(backColor))
            {
                var width = this.treeView.ClientSize.Width - e.Bounds.Left;
                e.Graphics.FillRectangle(background, new Rectangle(e.Bounds.Left, e.Bounds.Top, width, e.Bounds.Height));
            }

            // Performs the drawing
            using (var foreground = new SolidBrush(foreColor))
            {
                var left = e.Bounds.Left + this.imageList.ImageSize.Width + 2;

                if (!String.IsNullOrEmpty(line2))
                {
                    e.Graphics.DrawString(line1, m_boldFont, foreground, new PointF(left, e.Bounds.Top));

                    using (var lightForeground = new SolidBrush(lightForeColor))
                    {
                        e.Graphics.DrawString(line2, this.Font, lightForeground, new PointF(left, e.Bounds.Top + 16.0f));
                    }
                }
                else
                {
                    var height = e.Graphics.MeasureString(line1, m_boldFont).Height;
                    //var yOffset = Math.Max(0.0f, (e.Bounds.Height - height) * 0.5f);
                    var yOffset = 0;
                    e.Graphics.DrawString(line1, m_boldFont, foreground, new PointF(left, e.Bounds.Top + yOffset));
                }
            }

            // Draws the icon for skill/cert on the far right
            if (supIcon != -1)
            {
                var imgOfssetX = e.Bounds.Left;
                var imgOffsetY = Math.Max(0.0f, (e.Bounds.Height - imageList.ImageSize.Height) * 0.5f);
                e.Graphics.DrawImageUnscaled(imageList.Images[supIcon], 
                    (int)(imgOfssetX), 
                    (int)(e.Bounds.Top + imgOffsetY));
            }
        }
        #endregion


        #region Context menus
        void cmListSkills_Opening(object sender, CancelEventArgs e)
        {
            if (this.treeView.SelectedNode == null)
            {
                // Update "add to" menu
                tsmAddToPlan.Enabled = false;
                tsmAddToPlan.Text = "Plan...";

                // Update "browse" menu
                tsmBrowse.Enabled = false;
                tsmBrowse.Text = "Show...";
            }
            else
            {
                var cert = this.treeView.SelectedNode.Tag as Certificate;
                // When a certificate is selected
                if (cert != null)
                {
                    // Update "add to" menu
                    var status = m_character.GetCertificateStatus(cert);
                    tsmAddToPlan.Enabled = !m_plan.WillGrantEligibilityFor(cert);
                    tsmAddToPlan.Text = "Plan \"" + cert.ToString() + "\"";

                    // Update "browse" menu
                    tsmBrowse.Enabled = m_class != cert.Class;
                    tsmBrowse.Text = "Show \"" + cert.Class.ToString() + "\" certificates.";
                }
                // When a skill is selected
                else
                {
                    // Update "add to" menu
                    var prereq = (StaticSkill.Prereq)this.treeView.SelectedNode.Tag;
                    var skill = m_character.GetSkill(prereq.Name);
                    tsmAddToPlan.Enabled = skill.LastConfirmedLvl < prereq.Level && !m_plan.IsPlanned(skill, prereq.Level);
                    tsmAddToPlan.Text = "Plan \"" + skill.ToString() + " " + Skill.GetRomanForInt(prereq.Level) + "\"";

                    // Update "browse" menu
                    tsmBrowse.Enabled = true;
                    tsmBrowse.Text = "Show \"" + skill.ToString() + "\" in skills browser.";
                }
            }
        }

        private void tsmAddToPlan_Click(object sender, EventArgs e)
        {
            var cert = this.treeView.SelectedNode.Tag as Certificate;
            if (cert != null)
            {
                m_plan.PlanTo(cert, false);
            }
            else
            {
                var prereq = (StaticSkill.Prereq)this.treeView.SelectedNode.Tag;
                var skill = m_character.GetSkill(prereq.Name);
                m_plan.PlanTo(skill, prereq.Level);
            }
        }

        private void tsmExpandAll_Click(object sender, EventArgs e)
        {
            this.treeView.ExpandAll();
        }

        private void tsmCollapseAll_Click(object sender, EventArgs e)
        {
            this.treeView.CollapseAll();
        }

        private void tsmBrowse_Click(object sender, EventArgs e)
        {
            // Retrieve the owner window
            NewPlannerWindow npw = m_plan.PlannerWindow.Target as NewPlannerWindow;
            if (npw == null) return;

            Certificate cert = this.treeView.SelectedNode.Tag as Certificate;
            // When a certificate is selected, we select its class in the left tree
            if (cert != null)
            {
                npw.ShowCertInBrowser(cert);
            }
            // When a skill is selected, we select it in the skills browser
            else
            {
                StaticSkill.Prereq prereq = (StaticSkill.Prereq)this.treeView.SelectedNode.Tag;
                Skill skill = m_character.GetSkill(prereq.Name);
                npw.ShowSkillInTree(skill);
            }
        }
        #endregion
    }
}