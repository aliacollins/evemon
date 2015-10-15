using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Constants;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Data;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Factories;
using EVEMon.Common.Interfaces;
using EVEMon.Common.Models;

namespace EVEMon.SkillPlanner
{
    public partial class RecommendedCertificatesControl : UserControl
    {
        private const int GrantedIcon = 0;
        private const int ClaimableIcon = 1;
        private const int UnknownButTrainableIcon = 2;
        private const int UnknownIcon = 3;
        private const int CertificateIcon = 4;
        private const int SkillIcon = 5;
        private const int Planned = 6;

        private Item m_object;
        private Plan m_plan;
        private TimeSpan m_trainTime;


        #region Constructor

        /// <summary>
        /// User control to display required certificates for a given eveobject and update a plan object for requirements not met.
        /// </summary>
        public RecommendedCertificatesControl()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.ContainerControl |
                     ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();

            InitializeComponent();

            tvCertList.DrawNode += tvCertList_DrawNode;
            tvCertList.MouseDown += tvCertList_MouseDown;

            EveMonClient.PlanChanged += EveMonClient_PlanChanged;
            EveMonClient.SettingsChanged += EveMonClient_SettingsChanged;
            Disposed += OnDisposed;
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// Unsubscribe events on disposing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDisposed(object sender, EventArgs e)
        {
            EveMonClient.PlanChanged -= EveMonClient_PlanChanged;
            EveMonClient.SettingsChanged -= EveMonClient_SettingsChanged;
            Disposed -= OnDisposed;
        }

        /// <summary>
        /// Occurs when the plan changes, when update the display.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_PlanChanged(object sender, PlanChangedEventArgs e)
        {
            UpdateDisplay();
        }

        /// <summary>
        /// Occurs when the settings change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_SettingsChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// An EveObject for which we want to show required skills.
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
        /// The target Plan object to add any required skills.
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
        /// Updates control contents.
        /// </summary>
        private void UpdateDisplay()
        {
            // We have nothing to display
            if (m_object == null || m_plan == null)
                return;

            m_trainTime = TimeSpan.Zero;

            // Default all known flag to true. Will be set to false in UpdateNode() if a requirement is not met
            bool allCertsKnown = true;

            // Default unplanned certificates flag to false. Will be set to true in UpdateNode() if a requirement is neither met nor planned
            bool certsUnplanned = false;

            // Treeview update
            tvCertList.BeginUpdate();
            try
            {
                tvCertList.Nodes.Clear();
                if (m_object != null && m_plan != null)
                {
                    // Recursively create nodes
                    foreach (StaticCertificate cert in StaticCertificates.AllCertificates
                        .Where(x => x.Recommendations.Contains(m_object)))
                    {
                        tvCertList.Nodes.Add(GetCertNode(cert));
                    }
                }

                // Update the nodes
                foreach (TreeNode node in tvCertList.Nodes)
                {
                    UpdateNode(node, ref allCertsKnown, ref certsUnplanned);
                }
            }
            finally
            {
                tvCertList.EndUpdate();
            }

            // Set training time required label
            lblTimeRequired.Text = allCertsKnown
                                       ? "No training required"
                                       : m_trainTime.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas);

            // Set minimun control size
            Size timeRequiredTextSize = TextRenderer.MeasureText(lblTimeRequired.Text, Font);
            Size newMinimumSize = new Size(timeRequiredTextSize.Width + btnAddCerts.Width, 0);
            if (MinimumSize.Width < newMinimumSize.Width)
                MinimumSize = newMinimumSize;

            // Enable / disable button
            btnAddCerts.Enabled = certsUnplanned;
        }

        /// <summary>
        /// Recursive method to generate treenodes for tvCertList.
        /// </summary>
        /// <param name="certificate">An EntityRecommendedCertificate object</param>
        /// <returns></returns>
        private TreeNode GetCertNode(StaticCertificate certificate)
        {
            Character character = (Character)m_plan.Character;
            Certificate cert = character.Certificates[certificate.ID];

            TreeNode node = new TreeNode(cert.ToString()) { Tag = cert.LevelFive };         

            // Generate prerequisites skill nodes if required
            foreach (StaticSkillLevel prereq in cert.StaticData.PrerequisiteSkills.SelectMany(skill => skill.Value).Distinct())
            {
                node.Nodes.Add(GetSkillNode(prereq));
            }

            return node;
        }

        /// <summary>
        /// Recursive method to generate treenodes for tvCertList.
        /// </summary>
        /// <param name="prereq">The prereq.</param>
        /// <returns></returns>
        private TreeNode GetSkillNode(ISkillLevel prereq)
        {
            if (prereq.Skill == null)
                return new TreeNode();

            Character character = (Character)m_plan.Character;
            Skill skill = character.Skills[prereq.Skill.ID];
            TreeNode node;
            
            

            node = new TreeNode(prereq.ToString()) { Tag = new SkillLevel(skill, prereq.Level) };

            // Generate child prerequisite skill nodes if required
            foreach (StaticSkillLevel childPrereq in skill.StaticData.Prerequisites.Where(childPrereq => childPrereq != prereq))
            {
                node.Nodes.Add(GetSkillNode(childPrereq));
            }

            return node;
        }

        /// <summary>
        /// Updates the index image of the specified node and its children.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="allCertsKnown">if set to <c>true</c> [all certs known].</param>
        /// <param name="certsUnplanned">if set to <c>true</c> [certs unplanned].</param>
        private void UpdateNode(TreeNode node, ref bool allCertsKnown, ref bool certsUnplanned)
        {
            var certLevel = node.Tag as CertificateLevel;

            // The node represents a certificate
            if (certLevel != null)
            {
                switch (certLevel.Status)
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
                Character character = (Character)m_plan.Character;
                Skill skill = character.Skills[skillPrereq.Skill.ID];

                // Skill requirement met
                if (skillPrereq.IsTrained)
                    node.ImageIndex = GrantedIcon;
                    // Requirement not met, but planned
                else if (m_plan.IsPlanned(skill, skillPrereq.Level))
                {
                    node.ImageIndex = Planned;
                    allCertsKnown = false;
                }
                    // Requirement not met, but not planned
                else if (skill.IsKnown)
                {
                    node.ImageIndex = UnknownButTrainableIcon;
                    allCertsKnown = false;
                    certsUnplanned = true;
                }
                    // Requirement not met
                else
                {
                    node.ImageIndex = UnknownIcon;
                    allCertsKnown = false;
                    certsUnplanned = true;
                }
            }

            // When selected, the image remains the same
            node.SelectedImageIndex = node.ImageIndex;

            // Update the children
            foreach (TreeNode child in node.Nodes)
            {
                UpdateNode(child, ref allCertsKnown, ref certsUnplanned);
            }
        }

        /// <summary>
        /// Custom draw for the label.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvCertList_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            // Prevents a bug that causes every item to be redrawn at the top left corner
            if (e.Bounds.Left <= 10)
            {
                e.DrawDefault = true;
                return;
            }

            string line;
            int supIcon;

            // Is it a certificate ?
            var certLevel = e.Node.Tag as CertificateLevel;
            if (certLevel != null)
            {
                CertificateStatus status = certLevel.Status;
                line = certLevel.ToString();
                supIcon = CertificateIcon;

                // When not granted or claimable, let's calculate the training time
                if (status != CertificateStatus.Claimable && status != CertificateStatus.Granted)
                    m_trainTime += certLevel.GetTrainingTime;
            }
                // Or a skill prerequisite ?
            else
            {
                SkillLevel skillPrereq = (SkillLevel)e.Node.Tag;
                line = skillPrereq.ToString();
                supIcon = SkillIcon;

                // When not known to the require level, let's calculate the training time
                Skill skill = skillPrereq.Skill;
                if (!skillPrereq.IsTrained)
                    m_trainTime += skill.GetLeftTrainingTimeToLevel(skillPrereq.Level);
            }

            // Choose colors according to selection
            bool isSelected = ((e.State & TreeNodeStates.Selected) == TreeNodeStates.Selected);
            Color backColor = (isSelected ? SystemColors.Highlight : tvCertList.BackColor);
            Color foreColor = (isSelected ? SystemColors.HighlightText : tvCertList.ForeColor);

            // Draws the background
            using (SolidBrush background = new SolidBrush(backColor))
            {
                int width = tvCertList.ClientSize.Width - e.Bounds.Left;
                e.Graphics.FillRectangle(background, new Rectangle(e.Bounds.Left, e.Bounds.Top, width, e.Bounds.Height));
            }

            // Performs the drawing
            using (SolidBrush foreground = new SolidBrush(foreColor))
            {
                int left = e.Bounds.Left + imageList.ImageSize.Width + 2;
                e.Graphics.DrawString(line, Font, foreground, new PointF(left, e.Bounds.Top));
            }

            // Draws the icon for skill/cert on the far right
            if (Settings.UI.SafeForWork)
                return;

            int imgOfssetX = e.Bounds.Left;
            float imgOffsetY = Math.Max(0.0f, (e.Bounds.Height - imageList.ImageSize.Height) * 0.5f);
            e.Graphics.DrawImageUnscaled(imageList.Images[supIcon], (imgOfssetX), (int)(e.Bounds.Top + imgOffsetY));
        }

        /// <summary>
        /// When the treeview is clicked, we manually select nodes since the bounding boxes are incorrect due to custom draw.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvCertList_MouseDown(object sender, MouseEventArgs e)
        {
            // Perform the selection manually since the bound's width and x are incorrect in owndraw
            TreeNode selection = null;
            for (TreeNode node = tvCertList.TopNode; node != null; node = node.NextVisibleNode)
            {
                if (node.Bounds.Top > e.Y || node.Bounds.Bottom < e.Y)
                    continue;

                // If the user clicked the "arrow zone", we do not change the selection and just return
                if (e.X < (node.Bounds.Left - 32))
                    return;

                selection = node;
                break;
            }

            // Updates the selection
            if (selection == tvCertList.SelectedNode)
                return;

            tvCertList.SelectedNode = selection;
        }

        #endregion


        #region Controls' handlers

        /// <summary>
        /// Event handler method for Add Certs button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddCerts_Click(object sender, EventArgs e)
        {
            // Add Certificates to plan
            List<StaticSkillLevel> skillsToAdd = new List<StaticSkillLevel>();
            IEnumerable<IEnumerable<StaticSkillLevel>> certPrereqSkills =
                (tvCertList.Nodes.Cast<TreeNode>().Select(certificate => certificate.Tag))
                    .OfType<Certificate>().Select(cert => cert.StaticData.AllTopPrerequisiteSkills);

            foreach (IEnumerable<StaticSkillLevel> skills in certPrereqSkills)
            {
                skillsToAdd.AddRange(skills);
            }

            IPlanOperation operation = m_plan.TryAddSet(skillsToAdd, m_object.Name);
            PlanHelper.Perform(operation);

            // Refresh display to reflect plan changes
            UpdateDisplay();
        }

        /// <summary>
        /// Forces the selection update when a node is right-clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvCertList_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                tvCertList.SelectedNode = e.Node;
        }

        /// <summary>
        /// Event handler for treenode double click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvCertList_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // Get selected node
            TreeNode selectedNode = e.Node;

            // Make sure we have a tag
            if (selectedNode.Tag == null)
                return;

            var certLevel = selectedNode.Tag as CertificateLevel;
            PlanWindow planWindow = WindowsFactory.GetByTag<PlanWindow, Plan>(m_plan);
            if (certLevel != null)
            {
                if (planWindow != null && !planWindow.IsDisposed)
                    planWindow.ShowCertInBrowser(certLevel);
            }
            else
            {
                // Open skill browser tab for this skill
                Skill skill = ((SkillLevel)selectedNode.Tag).Skill;
                planWindow.ShowSkillInBrowser(skill);
            }
        }

        #endregion


        #region Context menu

        /// <summary>
        /// Context menu opening, we update the menus' statuses.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenu_Opening(object sender, CancelEventArgs e)
        {
            if (tvCertList.SelectedNode == null)
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
                var certLevel = tvCertList.SelectedNode.Tag as CertificateLevel;
                showInMenuSeparator.Visible = true;
                showInBrowserMenu.Visible = true;

                // When a certificate is selected
                if (certLevel != null)
                {
                    // Update "add to" menu
                    tsmAddToPlan.Enabled = !m_plan.WillGrantEligibilityFor(certLevel);
                    tsmAddToPlan.Text = String.Format(CultureConstants.DefaultCulture, "&Plan \"{0}\"", certLevel);

                    // Update "show in..." menu
                    showInBrowserMenu.Text = "Show in &Certificates";

                    // Update "show in skill explorer"
                    showInExplorerMenu.Visible = false;
                }
                    // When a skill is selected
                else
                {
                    // Update "add to" menu
                    SkillLevel prereq = (SkillLevel)tvCertList.SelectedNode.Tag;
                    Skill skill = prereq.Skill;
                    tsmAddToPlan.Enabled = skill.Level < prereq.Level && !m_plan.IsPlanned(skill, prereq.Level);
                    tsmAddToPlan.Text = String.Format(CultureConstants.DefaultCulture, "Plan \"{0} {1}\"", skill,
                                                      Skill.GetRomanFromInt(prereq.Level));

                    // Update "show in..." menu
                    showInBrowserMenu.Enabled = true;
                    showInBrowserMenu.Text = "Show in Skill &Browser";

                    // Update "show in skill explorer"
                    showInExplorerMenu.Visible = true;
                }
            }
        }

        /// <summary>
        /// Context menu > Show "skill" in browser | Show "certificate class" certificates.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showInSkillBrowserMenu_Click(object sender, EventArgs e)
        {
            // Make sure we have a tag
            if (tvCertList.SelectedNode.Tag == null)
                return;

            // Retrieve the owner window
            PlanWindow planWindow = WindowsFactory.GetByTag<PlanWindow, Plan>(m_plan);
            if (planWindow == null || planWindow.IsDisposed)
                return;

            var certLevel = tvCertList.SelectedNode.Tag as CertificateLevel;
            // When a certificate is selected
            if (certLevel != null)
                planWindow.ShowCertInBrowser(certLevel);
                // When a skill is selected
            else
            {
                Skill skill = ((SkillLevel)tvCertList.SelectedNode.Tag).Skill;
                if (skill != null)
                    planWindow.ShowSkillInBrowser(skill);
            }
        }

        /// <summary>
        /// Context > Show in Skill Explorer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showInSkillExplorerMenu_Click(object sender, EventArgs e)
        {
            // Make sure we have a tag
            if (tvCertList.SelectedNode.Tag == null)
                return;

            // Retrieve the owner window
            PlanWindow planWindow = WindowsFactory.GetByTag<PlanWindow, Plan>(m_plan);
            if (planWindow == null || planWindow.IsDisposed)
                return;

            // Open the skill explorer
            Skill skill = ((SkillLevel)tvCertList.SelectedNode.Tag).Skill;
            if (skill != null)
                planWindow.ShowSkillInExplorer(skill);
        }

        /// <summary>
        /// Treeview's context menu > Expand All.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmExpandAll_Click(object sender, EventArgs e)
        {
            tvCertList.ExpandAll();
        }

        /// <summary>
        /// Treeview's context menu > Collapse All.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmCollapseAll_Click(object sender, EventArgs e)
        {
            tvCertList.CollapseAll();
        }

        /// <summary>
        /// Context > Plan "(selection)"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmAddToPlan_Click(object sender, EventArgs e)
        {
            var certLevel = tvCertList.SelectedNode.Tag as CertificateLevel;
            if (certLevel != null)
            {
                IPlanOperation operation = m_plan.TryPlanTo(certLevel);
                PlanHelper.SelectPerform(operation);
            }
            else
            {
                SkillLevel prereq = (SkillLevel)tvCertList.SelectedNode.Tag;
                IPlanOperation operation = m_plan.TryPlanTo(prereq.Skill, prereq.Level);
                PlanHelper.SelectPerform(operation);
            }
        }

        #endregion
    }
}