using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.CustomEventArgs;

namespace EVEMon.SkillPlanner
{
    /// <summary>
    /// UserControl to display a tree of certificates.
    /// </summary>
    public sealed partial class CertificateTreeDisplayControl : UserControl
    {
        private const int GrantedIcon = 0;
        private const int ClaimableIcon = 1;
        private const int UnknownButTrainableIcon = 2;
        private const int UnknownIcon = 3;
        private const int CertificateIcon = 4;
        private const int SkillIcon = 5;
        private const int Planned = 6;

        // Blank image list for 'Safe for work' setting
        private readonly ImageList m_emptyImageList = new ImageList();

        private Plan m_plan;
        private Character m_character;
        private CertificateClass m_class;
        private Font m_boldFont;

        private bool m_allExpanded;

        public event EventHandler SelectionChanged;


        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public CertificateTreeDisplayControl()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.ContainerControl |
                     ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();

            InitializeComponent();
        }

        #endregion



        #region Public Properties

        /// <summary>
        /// Gets or sets the current plan.
        /// </summary>
        public Plan Plan
        {
            get { return m_plan; }
            set
            {
                m_plan = value;
                if (m_plan == null)
                    return;

                m_character = (Character)m_plan.Character;
                UpdateTree();
            }
        }

        /// <summary>
        /// Gets or sets the certificate class (i.e. "Core competency").
        /// </summary>
        public CertificateClass CertificateClass
        {
            get { return m_class; }
            set
            {
                if (value == m_class)
                    return;

                m_class = value;
                UpdateTree();
            }
        }

        /// <summary>
        /// Gets the selected certificate, null if none is selected. 
        /// Note that the selected certificate may be a prerequisite and therefore
        /// not have the same class than the one represented by this control.
        /// </summary>
        public Certificate SelectedCertificate
        {
            get
            {
                if (treeView.SelectedNode == null)
                    return null;

                return treeView.SelectedNode.Tag as Certificate;
            }
        }

        /// <summary>
        /// Gets cert of the displayed class which contains the current selection.
        /// </summary>
        public Certificate SelectedCertificateLevel
        {
            get
            {
                TreeNode node = treeView.SelectedNode;
                while (node != null)
                {
                    Certificate certificate = node.Tag as Certificate;
                    if (certificate != null && certificate.Class == CertificateClass)
                        return certificate;

                    node = node.Parent;
                }
                return null;
            }
        }

        /// <summary>
        /// Expands the node representing this certificate.
        /// </summary>
        /// <param name="cert"></param>
        public void ExpandCert(Certificate cert)
        {
            foreach (TreeNode node in treeView.Nodes.Cast<TreeNode>().Where(node => node.Tag == cert))
            {
                treeView.SelectedNode = node;
                node.Expand();
                if (SelectionChanged != null)
                    SelectionChanged(this, new EventArgs());
                break;
            }
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// On load, complete the initialization.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            if (DesignMode || this.IsDesignModeHosted())
                return;

            treeView.DrawNode += treeView_DrawNode;
            treeView.MouseDown += treeView_MouseDown;

            cmListSkills.Opening += cmListSkills_Opening;
            
            m_boldFont = FontFactory.GetFont(Font, FontStyle.Bold);
            treeView.Font = FontFactory.GetFont("Microsoft Sans Serif", 8.25F);
            treeView.ItemHeight = (treeView.Font.Height * 2) + 6;

            m_emptyImageList.ImageSize = new Size(24, 24);
            m_emptyImageList.Images.Add(new Bitmap(24, 24));

            EveMonClient.SettingsChanged += EveMonClient_SettingsChanged;
            EveMonClient.CharacterUpdated += EveMonClient_CharacterUpdated;
            EveMonClient.PlanChanged += EveMonClient_PlanChanged;
            Disposed += OnDisposed;
        }

        /// <summary>
        /// Unsubscribe events on disposing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDisposed(object sender, EventArgs e)
        {
            EveMonClient.SettingsChanged -= EveMonClient_SettingsChanged;
            EveMonClient.CharacterUpdated -= EveMonClient_CharacterUpdated;
            EveMonClient.PlanChanged -= EveMonClient_PlanChanged;
            Disposed -= OnDisposed;
        }

        /// <summary>
        /// On settings change, we update the tree.
        /// </summary>
        /// <remarks>Relates to safe for work setting</remarks>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EveMonClient_SettingsChanged(object sender, EventArgs e)
        {
            UpdateTree();
        }

        /// <summary>
        /// Fired when one of the character changed (skill completion, update from CCP, etc).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_CharacterUpdated(object sender, CharacterChangedEventArgs e)
        {
            if (m_plan == null)
                return;

            if (e.Character != m_plan.Character)
                return;

            UpdateTree();
        }

        /// <summary>
        /// Occurs when the plan changes, we update the tree.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_PlanChanged(object sender, PlanChangedEventArgs e)
        {
            UpdateTree();
        }

        /// <summary>
        /// When the treeview is clicked, we manually select nodes since the bounding boxes are incorrect due to custom draw.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView_MouseDown(object sender, MouseEventArgs e)
        {
            // Perform the selection manually since the bound's width and x are incorrect in owndraw
            TreeNode selection = null;
            for (TreeNode node = treeView.TopNode; node != null; node = node.NextVisibleNode)
            {
                if (node.Bounds.Top > e.Y || node.Bounds.Bottom < e.Y)
                    continue;

                // If the user clicked the "arrow zone", we do not change the selection and just return
                if (e.X < (node.Bounds.Left - 32))
                    return;

                selection = node;
                break;
            }

            // Updates the selection and fire the appropriate event
            if (selection == treeView.SelectedNode)
                return;

            treeView.SelectedNode = selection;
            if (SelectionChanged != null)
                SelectionChanged(this, new EventArgs());
        }

        /// <summary>
        /// Forces the selection update when a node is right-clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                treeView.SelectedNode = e.Node;
        }

        /// <summary>
        /// Event handler for treenode double click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            showInBrowserMenu_Click(sender, e);
        }

        #endregion


        #region Tree building

        /// <summary>
        /// Update the whole tree.
        /// </summary>
        private void UpdateTree()
        {
            Certificate oldSelection = SelectedCertificate;
            TreeNode newSelection = null;

            treeView.ImageList = (Settings.UI.SafeForWork ? m_emptyImageList : imageList);

            treeView.BeginUpdate();
            try
            {
                // Clear the old items
                treeView.Nodes.Clear();

                // No update when not fully initialized
                if (m_character == null || m_class == null)
                    return;

                // Create the nodes when not done, yet
                if (treeView.Nodes.Count == 0)
                {
                    foreach (Certificate cert in m_class)
                    {
                        TreeNode node = CreateNode(cert);
                        treeView.Nodes.Add(node);

                        // Does the old selection still exists ?
                        if (cert == oldSelection)
                            newSelection = node;
                    }
                }

                // Update the nodes
                foreach (TreeNode node in treeView.Nodes)
                {
                    UpdateNode(node);
                }

                // Is the old selection kept ? Then we select the matching node
                if (newSelection != null)
                    treeView.SelectedNode = newSelection;
            }
            finally
            {
                treeView.EndUpdate();
            }

            // Fire the SelectionChanged event if the old selection doesn't exist anymore
            if (newSelection != null)
                return;

            if (SelectionChanged != null)
                SelectionChanged(this, new EventArgs());
        }

        /// <summary>
        /// Create a node from a prerequisite certificate.
        /// </summary>
        /// <param name="cert"></param>
        /// <returns></returns>
        private static TreeNode CreateNode(Certificate cert)
        {
            TreeNode node = new TreeNode
                                {
                                    Text = cert.ToString(),
                                    Tag = cert
                                };

            foreach (Certificate prereqCert in cert.PrerequisiteCertificates)
            {
                node.Nodes.Add(CreateNode(prereqCert));
            }

            foreach (SkillLevel prereqSkill in cert.PrerequisiteSkills)
            {
                node.Nodes.Add(CreateNode(prereqSkill));
            }

            return node;
        }

        /// <summary>
        /// Create a node from a prerequisite skill.
        /// </summary>
        /// <param name="skillPrereq"></param>
        /// <returns></returns>
        private static TreeNode CreateNode(SkillLevel skillPrereq)
        {
            TreeNode node = new TreeNode
                                {
                                    Text = skillPrereq.ToString(),
                                    Tag = skillPrereq
                                };

            // Add this skill's prerequisites
            foreach (SkillLevel prereqSkill in skillPrereq.Skill.Prerequisites
                .Where(prereqSkill => prereqSkill.Skill != skillPrereq.Skill))
            {
                node.Nodes.Add(CreateNode(prereqSkill));
            }

            return node;
        }

        /// <summary>
        /// Updates the specified node and its children.
        /// </summary>
        /// <param name="node"></param>
        private void UpdateNode(TreeNode node)
        {
            Certificate cert = node.Tag as Certificate;

            // The node represents a certificate
            if (cert != null)
            {
                switch (cert.Status)
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
                SkillLevel skillPrereq = (SkillLevel)node.Tag;
                Skill skill = m_character.Skills[skillPrereq.Skill];

                if (skillPrereq.IsTrained)
                    node.ImageIndex = GrantedIcon;
                else if (m_plan.IsPlanned(skill, skillPrereq.Level))
                    node.ImageIndex = Planned;
                else if (skill.IsKnown)
                    node.ImageIndex = UnknownButTrainableIcon;
                else
                    node.ImageIndex = UnknownIcon;
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
        /// Custom draw for the label.
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

            string line1;
            string line2 = "-";
            int supIcon = -1;

            // Is it a certificate ?
            Certificate cert = e.Node.Tag as Certificate;
            if (cert != null)
            {
                line1 = cert.ToString();
                if (!Settings.UI.SafeForWork)
                    supIcon = CertificateIcon;
                CertificateStatus status = cert.Status;

                // When not granted or claimable, let's display the training time
                if (status != CertificateStatus.Claimable && status != CertificateStatus.Granted)
                {
                    TimeSpan time = cert.GetTrainingTime;
                    line2 = time.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas);
                }
            }
                // Or a skill prerequisite ?
            else
            {
                if (!Settings.UI.SafeForWork)
                    supIcon = SkillIcon;

                SkillLevel skillPrereq = (SkillLevel)e.Node.Tag;
                line1 = skillPrereq.ToString();

                // When not known to the require level, let's display the training time
                Skill skill = skillPrereq.Skill;
                if (!skillPrereq.IsTrained)
                {
                    TimeSpan time = skill.GetLeftTrainingTimeToLevel(skillPrereq.Level);
                    line2 = time.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas);
                }
            }

            // Choose colors according to selection
            bool isSelected = ((e.State & TreeNodeStates.Selected) == TreeNodeStates.Selected);
            Color backColor = (isSelected ? SystemColors.Highlight : treeView.BackColor);
            Color foreColor = (isSelected ? SystemColors.HighlightText : treeView.ForeColor);
            Color lightForeColor = (isSelected ? SystemColors.HighlightText : SystemColors.GrayText);

            // Draws the background
            using (SolidBrush background = new SolidBrush(backColor))
            {
                int width = treeView.ClientSize.Width - e.Bounds.Left;
                e.Graphics.FillRectangle(background, new Rectangle(e.Bounds.Left, e.Bounds.Top, width, e.Bounds.Height));
            }

            // Performs the drawing
            using (SolidBrush foreground = new SolidBrush(foreColor))
            {
                int left = e.Bounds.Left + imageList.ImageSize.Width + 2;
                Size line1Size = TextRenderer.MeasureText(line1, m_boldFont);

                if (!String.IsNullOrEmpty(line2))
                {
                    e.Graphics.DrawString(line1, m_boldFont, foreground, new PointF(left, e.Bounds.Top));

                    using (SolidBrush lightForeground = new SolidBrush(lightForeColor))
                    {
                        e.Graphics.DrawString(line2, Font, lightForeground, new PointF(left, e.Bounds.Top + line1Size.Height));
                    }
                }
                else
                    e.Graphics.DrawString(line1, m_boldFont, foreground, new PointF(left, e.Bounds.Top));
            }

            // Draws the icon for skill/cert on the far right
            if (supIcon == -1)
                return;

            int imgOfssetX = e.Bounds.Left;
            float imgOffsetY = Math.Max(0.0f, (e.Bounds.Height - imageList.ImageSize.Height) * 0.5f);
            e.Graphics.DrawImageUnscaled(imageList.Images[supIcon],
                                         (imgOfssetX),
                                         (int)(e.Bounds.Top + imgOffsetY));
        }

        #endregion


        #region Context menus

        /// <summary>
        /// Context menu opening, we update the menus' statuses.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmListSkills_Opening(object sender, CancelEventArgs e)
        {
            TreeNode node = treeView.SelectedNode;

            if (node == null)
            {
                // Update "add to" menu
                tsmAddToPlan.Enabled = false;
                tsmAddToPlan.Text = "Plan...";

                // Update "show in skill browser" menu
                showInMenuSeparator.Visible = false;
                showInExplorerMenu.Visible = false;
                showInBrowserMenu.Visible = false;
            }
            else
            {
                Certificate cert = node.Tag as Certificate;
                showInMenuSeparator.Visible = true;
                showInBrowserMenu.Visible = true;

                // When a certificate is selected
                if (cert != null)
                {
                    // Update "add to" menu
                    tsmAddToPlan.Enabled = !m_plan.WillGrantEligibilityFor(cert);
                    tsmAddToPlan.Text = String.Format("Plan \"{0}\"", cert);

                    // Update "browser" menu
                    showInBrowserMenu.Enabled = m_class != cert.Class;
                    showInBrowserMenu.Text = "Show Certificates";

                    // Update "show in skill explorer" menu
                    showInExplorerMenu.Visible = false;
                }
                    // When a skill is selected
                else
                {
                    // Update "add to" menu
                    SkillLevel prereq = (SkillLevel)node.Tag;
                    Skill skill = prereq.Skill;
                    tsmAddToPlan.Enabled = skill.Level < prereq.Level && !m_plan.IsPlanned(skill, prereq.Level);
                    tsmAddToPlan.Text = String.Format("Plan \"{0} {1}\"", skill, Skill.GetRomanFromInt(prereq.Level));

                    // Update "show in skill browser" menu
                    showInBrowserMenu.Enabled = true;
                    showInBrowserMenu.Text = "Show in Skill Browser...";

                    // Update "show in skill explorer" menu
                    showInExplorerMenu.Visible = true;
                }
            }

            tsSeparatorToggle.Visible = (node != null && node.GetNodeCount(true) > 0);

            // "Collapse" and "Expand" menus
            tsmCollapseSelected.Visible = (node != null && node.GetNodeCount(true) > 0 && node.IsExpanded);
            tsmExpandSelected.Visible = (node != null && node.GetNodeCount(true) > 0 && !node.IsExpanded);

            tsmExpandSelected.Text = (node != null && node.GetNodeCount(true) > 0 && !node.IsExpanded
                                          ? String.Format("Expand {0}", node.Text)
                                          : String.Empty);
            tsmCollapseSelected.Text = (node != null && node.GetNodeCount(true) > 0 && node.IsExpanded
                                            ? String.Format("Collapse {0}", node.Text)
                                            : String.Empty);

            // "Expand All" and "Collapse All" menus
            tsmCollapseAll.Enabled = tsmCollapseAll.Visible = m_allExpanded;
            tsmExpandAll.Enabled = tsmExpandAll.Visible = !tsmCollapseAll.Enabled;
        }

        /// <summary>
        /// Treeview's context menu > Plan "(selection)".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmAddToPlan_Click(object sender, EventArgs e)
        {
            Certificate cert = treeView.SelectedNode.Tag as Certificate;
            if (cert != null)
            {
                IPlanOperation operation = m_plan.TryPlanTo(cert);
                PlanHelper.SelectPerform(operation);
            }
            else
            {
                SkillLevel prereq = (SkillLevel)treeView.SelectedNode.Tag;
                IPlanOperation operation = m_plan.TryPlanTo(prereq.Skill, prereq.Level);
                PlanHelper.SelectPerform(operation);
            }
        }

        /// <summary>
        /// Treeview's context menu > Expand.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmExpandSelected_Click(object sender, EventArgs e)
        {
            treeView.SelectedNode.Expand();
        }

        /// <summary>
        /// Treeview's context menu > Collapse.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmCollapseSelected_Click(object sender, EventArgs e)
        {
            treeView.SelectedNode.Collapse();
        }

        /// <summary>
        /// Treeview's context menu > Expand All.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmExpandAll_Click(object sender, EventArgs e)
        {
            treeView.ExpandAll();
            m_allExpanded = true;
        }

        /// <summary>
        /// Treeview's context menu > Collapse All.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmCollapseAll_Click(object sender, EventArgs e)
        {
            treeView.CollapseAll();
            m_allExpanded = false;
        }

        /// <summary>
        /// Context menu > Show "skill" in browser | Show "certificate class" certificates.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showInBrowserMenu_Click(object sender, EventArgs e)
        {
            // Retrieve the owner window
            PlanWindow npw = WindowsFactory<PlanWindow>.GetByTag(m_plan);
            if (npw == null || npw.IsDisposed)
                return;

            // Return when nothing is selected
            if (treeView.SelectedNode == null)
                return;

            Certificate cert = treeView.SelectedNode.Tag as Certificate;
            // When a certificate is selected, we select its class in the left tree
            if (cert != null)
                npw.ShowCertInBrowser(cert);
                // When a skill is selected, we select it in the skill browser
            else
            {
                SkillLevel prereq = (SkillLevel)treeView.SelectedNode.Tag;
                npw.ShowSkillInBrowser(prereq.Skill);
            }
        }

        /// <summary>
        /// Context menu > Show "skill" in explorer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showInExplorerMenu_Click(object sender, EventArgs e)
        {
            // Retrieve the owner window
            PlanWindow npw = WindowsFactory<PlanWindow>.GetByTag(m_plan);
            if (npw == null || npw.IsDisposed)
                return;

            // Return when nothing is selected
            if (treeView.SelectedNode == null)
                return;

            // Open the skill explorer
            SkillLevel prereq = (SkillLevel)treeView.SelectedNode.Tag;
            npw.ShowSkillInExplorer(prereq.Skill);
        }

        #endregion
    }
}