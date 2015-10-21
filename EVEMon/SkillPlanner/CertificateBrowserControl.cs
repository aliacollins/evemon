using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Constants;
using EVEMon.Common.Controls;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Data;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Factories;
using EVEMon.Common.Helpers;
using EVEMon.Common.Interfaces;
using EVEMon.Common.Models;

namespace EVEMon.SkillPlanner
{
    public partial class CertificateBrowserControl : UserControl
    {
        private Plan m_plan;
        private const int HPad = 40;

        /// <summary>
        /// Constructor
        /// </summary>
        public CertificateBrowserControl()
        {
            InitializeComponent();

            rightSplitContainer.RememberDistanceKey = "CertificateBrowser_Right";
            leftSplitContainer.RememberDistanceKey = "CertificateBrowser_Left";
        }


        #region Inherited Events

        /// <summary>
        /// On load.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            lblName.Font = FontFactory.GetFont("Tahoma", 8.25F, FontStyle.Bold);

            certSelectCtl.SelectionChanged += certSelectCtl_SelectionChanged;
            certDisplayCtl.SelectionChanged += certDisplayCtl_SelectionChanged;

            EveMonClient.PlanChanged += EveMonClient_PlanChanged;
            EveMonClient.SettingsChanged += EveMonClient_SettingsChanged;
            Disposed += OnDisposed;

            // Reposition the help text along side the treeview
            Control[] result = certSelectCtl.Controls.Find("pnlResults", true);
            if (result.Length > 0)
                lblHelp.Location = new Point(lblHelp.Location.X, result[0].Location.Y);

            // Update the controls visibility
            UpdateControlVisibility();
        }

        /// <summary>
        /// Unsubscribe events on disposing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDisposed(object sender, EventArgs e)
        {
            certSelectCtl.SelectionChanged -= certSelectCtl_SelectionChanged;
            certDisplayCtl.SelectionChanged -= certDisplayCtl_SelectionChanged;

            EveMonClient.PlanChanged -= EveMonClient_PlanChanged;
            EveMonClient.SettingsChanged -= EveMonClient_SettingsChanged;
            Disposed -= OnDisposed;
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets or sets the character this control is bound to.
        /// </summary>
        [Browsable(false)]
        public Character Character
        {
            get { return m_plan.Character as Character; }
        }

        /// <summary>
        /// Gets or sets the current plan
        /// </summary>
        [Browsable(false)]
        public Plan Plan
        {
            get { return m_plan; }
            set
            {
                m_plan = value;

                certSelectCtl.Plan = m_plan;
                certDisplayCtl.Plan = m_plan;
                UpdateEligibility();
            }
        }

        /// <summary>
        /// This is the way to get and set the selected certificate class.
        /// </summary>
        [Browsable(false)]
        public CertificateClass SelectedCertificateClass
        {
            get { return certSelectCtl.SelectedCertificateClass; }
            set { certSelectCtl.SelectedCertificateClass = value; }
        }

        #endregion


        #region Content Update

        /// <summary>
        /// Updates the content.
        /// </summary>
        private void UpdateContent()
        {
            CertificateClass certClass = certSelectCtl.SelectedCertificateClass;

            // When no certificate class is selected, we just hide the right panel.
            if (certClass == null)
            {
                // View help message
                lblHelp.Visible = true;

                panelRight.Visible = false;
                return;
            }

            // Hide help message
            lblHelp.Visible = false;

            // Updates controls visibility
            panelRight.Visible = true;

            lblName.Text = String.Format(CultureConstants.DefaultCulture, "{0}", certClass.Name);
            lblCategory.Text = certClass.Category.Name;

            // Training time per certificate level
            UpdateLevelLabel(lblLevel1Time, certClass.Certificate.LevelOne);
            UpdateLevelLabel(lblLevel2Time, certClass.Certificate.LevelTwo);
            UpdateLevelLabel(lblLevel3Time, certClass.Certificate.LevelThree);
            UpdateLevelLabel(lblLevel4Time, certClass.Certificate.LevelFour);
            UpdateLevelLabel(lblLevel5Time, certClass.Certificate.LevelFive);

            // Only read the recommendations from one level, because they are all the same
            PersistentSplitContainer rSplCont = rightSplitContainer;
            List<Control> newItems = new List<Control>();
            SortedList<string, Item> ships = new SortedList<string, Item>();
            foreach (Item ship in certClass.Certificate.Recommendations)
            {
                ships.Add(ship.Name, ship);
            }

            Label tempLabel = null;
            try
            {
                tempLabel = new Label();
                tempLabel.Font = new Font(tempLabel.Font, FontStyle.Bold);
                tempLabel.AutoSize = true;
                tempLabel.Dock = DockStyle.Top;
                tempLabel.Text = @"Recommended For";
                tempLabel.Padding = new Padding(5);

                Label tsl = tempLabel;
                tempLabel = null;

                newItems.Add(tsl);

                Size tslTextSize = TextRenderer.MeasureText(tsl.Text, Font);
                int panelMinSize = rSplCont.Panel2MinSize;
                rSplCont.Panel2MinSize = (panelMinSize > tslTextSize.Width + HPad
                    ? panelMinSize
                    : tslTextSize.Width + HPad);
                rSplCont.SplitterDistance = rSplCont.Width - rSplCont.Panel2MinSize;
            }
            finally
            {
                if (tempLabel != null)
                    tempLabel.Dispose();
            }

            foreach (LinkLabel linkLabel in ships.Values
                .Select(ship =>
                {
                    LinkLabel linkLabel;
                    LinkLabel tempLinkLabel = null;
                    try
                    {
                        tempLinkLabel = new LinkLabel();
                        tempLinkLabel.LinkBehavior = LinkBehavior.HoverUnderline;
                        tempLinkLabel.Padding = new Padding(16, 0, 0, 0);
                        tempLinkLabel.Dock = DockStyle.Top;
                        tempLinkLabel.Text = ship.Name;
                        tempLinkLabel.Tag = ship;

                        linkLabel = tempLinkLabel;
                        tempLinkLabel = null;
                    }
                    finally
                    {
                        if (tempLinkLabel != null)
                            tempLinkLabel.Dispose();
                    }

                    return linkLabel;
                }))
            {
                linkLabel.MouseClick += recommendations_MenuItem;
                newItems.Add(linkLabel);
            }


            // Updates the recommendations for this certificate
            UpdateRecommendations(newItems, rSplCont);

            // Update the menus and such
            UpdateEligibility();

            // Update the certificates tree display
            certDisplayCtl.CertificateClass = certClass;
        }

        /// <summary>
        /// Updates the provided label with the training time to the given level.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="certificateLevel">The certificate level.</param>
        private static void UpdateLevelLabel(Control label, CertificateLevel certificateLevel)
        {
            StringBuilder sb = new StringBuilder();

            // "Level III: "
            sb.AppendFormat(CultureConstants.DefaultCulture, "{0}: ", certificateLevel);

            // Is it already trained ?
            if (certificateLevel.IsTrained)
            {
                label.Text = sb.Append("Already trained").ToString();
                return;
            }

            // Training time left for level
            sb.Append(certificateLevel.GetTrainingTime.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas));

            label.Text = sb.ToString();
        }

        /// <summary>
        /// Updates the recommendations.
        /// </summary>
        /// <param name="newItems">The new items list.</param>
        /// <param name="rSplCont">The right splitter container.</param>
        private void UpdateRecommendations(List<Control> newItems, SplitContainer rSplCont)
        {
            rightSplitContainer.Panel2.Controls.Clear();

            if (newItems.Count != 0)
            {
                newItems.Reverse();
                rSplCont.Panel2.Controls.AddRange(newItems.ToArray());
            }
            else
            {
                Label tempLabel = null;
                try
                {
                    tempLabel = new Label();
                    tempLabel.Text = @"No Recommendations";
                    tempLabel.Enabled = false;
                    tempLabel.Dock = DockStyle.Fill;
                    tempLabel.TextAlign = ContentAlignment.MiddleCenter;

                    Label tsl = tempLabel;
                    tempLabel = null;

                    rSplCont.Panel2.Controls.Add(tsl);

                    Size tslTextSize = TextRenderer.MeasureText(tsl.Text, Font);
                    rSplCont.Panel2MinSize = tslTextSize.Width + HPad;
                    rSplCont.SplitterDistance = rSplCont.Width - rSplCont.Panel2MinSize;
                }
                finally
                {
                    if (tempLabel != null)
                        tempLabel.Dispose();
                }
            }
        }

        /// <summary>
        /// Updates eligibility label and planning menus.
        /// </summary>
        private void UpdateEligibility()
        {
            CertificateClass certClass = certSelectCtl.SelectedCertificateClass;

            if (certClass == null)
                return;

            // First we search the highest eligible certificate after this plan
            IEnumerable<CertificateLevel> eligibleCertLevel = certClass.Certificate.AllLevel.TakeWhile(cert => m_plan.WillGrantEligibilityFor(cert)).ToList();

            CertificateLevel lastEligibleCertLevel = null;
            if (!eligibleCertLevel.Any())
                tslbEligible.Text = @"(none)";
            else
            {
                lastEligibleCertLevel = eligibleCertLevel.Last();
                tslbEligible.Text = lastEligibleCertLevel.ToString();

                if (certClass.HighestTrainedLevel == null)
                    tslbEligible.Text += @" (improved from ""none"")";
                else if ((int)lastEligibleCertLevel.Level > (int)certClass.HighestTrainedLevel.Level)
                {
                    tslbEligible.Text += String.Format(CultureConstants.DefaultCulture, " (improved from \"{0}\")",
                                                       certClass.HighestTrainedLevel);
                }
                else
                    tslbEligible.Text += @" (no change)";
            }
            
            UpdatePlanningMenuStatus(tsPlanToLevelOne, certClass.Certificate.LevelOne, lastEligibleCertLevel);
            UpdatePlanningMenuStatus(tsPlanToLevelTwo, certClass.Certificate.LevelTwo, lastEligibleCertLevel);
            UpdatePlanningMenuStatus(tsPlanToLevelThree, certClass.Certificate.LevelThree, lastEligibleCertLevel);
            UpdatePlanningMenuStatus(tsPlanToLevelFour, certClass.Certificate.LevelFour, lastEligibleCertLevel);
            UpdatePlanningMenuStatus(tsPlanToLevelFive, certClass.Certificate.LevelFive, lastEligibleCertLevel);
        }

        /// <summary>
        /// Updates a "plan to" menu.
        /// </summary>
        /// <param name="menu">The menu to update</param>
        /// <param name="level">The level represent by this menu</param>
        /// <param name="lastEligibleCertLevel">The highest eligible certificate after this plan</param>
        private static void UpdatePlanningMenuStatus(ToolStripItem menu, CertificateLevel level, CertificateLevel lastEligibleCertLevel)
        {
            CertificateLevel certLevel = level;
            menu.Visible = certLevel != null;
            menu.Enabled = certLevel != null && (lastEligibleCertLevel == null || certLevel.Level > lastEligibleCertLevel.Level);
        }

        #endregion


        #region Control Events

        /// <summary>
        /// When the user select a new certificate class, we update everything.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void certSelectCtl_SelectionChanged(object sender, EventArgs e)
        {
            UpdateContent();
        }

        /// <summary>
        /// Handler for the ship-links generated for the recommendations.
        /// </summary>
        private void recommendations_MenuItem(object sender, EventArgs e)
        {
            Control tsi = sender as Control;
            if (tsi == null)
                return;

            Item ship = tsi.Tag as Item;
            PlanWindow window = WindowsFactory.GetByTag<PlanWindow, Plan>(m_plan);
            if (ship != null && window != null && !window.IsDisposed)
                window.ShowShipInBrowser(ship);
        }

        /// <summary>
        /// When the display tree's selection changes, we may update the description.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void certDisplayCtl_SelectionChanged(object sender, EventArgs e)
        {
            var certLevel = certDisplayCtl.SelectedCertificateLevel;
            CertificateClass certClass = certSelectCtl.SelectedCertificateClass;

            if (certClass == null)
                return;

            // No certificate or not one of the roots ? Then, we display the description for the lowest grade cert
            if (certLevel == null || certLevel.Certificate.Class != certClass)
            {
                textboxDescription.Text = certClass.Certificate.Description;
                lblName.Text = String.Format(CultureConstants.DefaultCulture, "{0}", certClass.Name);
            }
                // So, one of our cert class's grades has been selected, we use its description
            else
            {
                textboxDescription.Text = certLevel.Certificate.Description;
                lblName.Text = String.Format(CultureConstants.DefaultCulture, "{0}", certClass.Name);
            }
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// When the current plan changes (new skills, etc), we need to update some informations.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_PlanChanged(object sender, PlanChangedEventArgs e)
        {
            if (e.Plan == m_plan)
                UpdateEligibility();
        }

        /// <summary>
        /// When the settings changes, we need to update.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EveMonClient_SettingsChanged(object sender, EventArgs e)
        {
            UpdateControlVisibility();
        }

        #endregion


        #region Context menu

        /// <summary>
        /// Handles the Click event of the tsPlanToLevelOne control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void tsPlanToLevelOne_Click(object sender, EventArgs e)
        {
            IPlanOperation operation = m_plan.TryPlanTo(certSelectCtl.SelectedCertificateClass.Certificate.LevelOne);
            if (operation == null)
                return;

            PlanWindow window = WindowsFactory.ShowByTag<PlanWindow, Plan>(operation.Plan);
            if (window == null || window.IsDisposed)
                return;

            PlanHelper.SelectPerform(new PlanToOperationForm(operation), window, operation);
        }

        /// <summary>
        /// Handles the Click event of the tsPlanToLevelTwo control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void tsPlanToLevelTwo_Click(object sender, EventArgs e)
        {
            IPlanOperation operation = m_plan.TryPlanTo(certSelectCtl.SelectedCertificateClass.Certificate.LevelTwo);
            if (operation == null)
                return;

            PlanWindow window = WindowsFactory.ShowByTag<PlanWindow, Plan>(operation.Plan);
            if (window == null || window.IsDisposed)
                return;

            PlanHelper.SelectPerform(new PlanToOperationForm(operation), window, operation);
        }

        /// <summary>
        /// Handles the Click event of the tsPlanToLevelThree control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void tsPlanToLevelThree_Click(object sender, EventArgs e)
        {
            IPlanOperation operation = m_plan.TryPlanTo(certSelectCtl.SelectedCertificateClass.Certificate.LevelThree);
            if (operation == null)
                return;

            PlanWindow window = WindowsFactory.ShowByTag<PlanWindow, Plan>(operation.Plan);
            if (window == null || window.IsDisposed)
                return;

            PlanHelper.SelectPerform(new PlanToOperationForm(operation), window, operation);
        }

        /// <summary>
        /// Handles the Click event of the tsPlanToLevelFour control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void tsPlanToLevelFour_Click(object sender, EventArgs e)
        {
            IPlanOperation operation = m_plan.TryPlanTo(certSelectCtl.SelectedCertificateClass.Certificate.LevelFour);
            if (operation == null)
                return;

            PlanWindow window = WindowsFactory.ShowByTag<PlanWindow, Plan>(operation.Plan);
            if (window == null || window.IsDisposed)
                return;

            PlanHelper.SelectPerform(new PlanToOperationForm(operation), window, operation);
        }

        /// <summary>
        /// Handles the Click event of the tsPlanToLevelFive control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void tsPlanToLevelFive_Click(object sender, EventArgs e)
        {
            IPlanOperation operation = m_plan.TryPlanTo(certSelectCtl.SelectedCertificateClass.Certificate.LevelFive);
            if (operation == null)
                return;

            PlanWindow window = WindowsFactory.ShowByTag<PlanWindow, Plan>(operation.Plan);
            if (window == null || window.IsDisposed)
                return;

            PlanHelper.SelectPerform(new PlanToOperationForm(operation), window, operation);
        }

        /// <summary>
        /// Handles the DropDownOpening event of the tsPlanToMenu control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void tsPlanToMenu_DropDownOpening(object sender, EventArgs e)
        {
            UpdateEligibility();
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// This is the way to set the selected certificate.
        /// </summary>
        public void SelectedCertificateLevel(CertificateLevel certificateLevel)
        {
            if (certificateLevel == null)
                throw new ArgumentNullException("certificateLevel");

            if (SelectedCertificateClass == certificateLevel.Certificate.Class && certDisplayCtl.SelectedCertificateLevel == certificateLevel)
                return;

            SelectedCertificateClass = certificateLevel.Certificate.Class;
            certDisplayCtl.ExpandCert(certificateLevel);
        }

        /// <summary>
        /// Updates the control visibility.
        /// </summary>
        private void UpdateControlVisibility()
        {
            if (Settings.UI.SafeForWork)
            {
                pictureBox1.Visible = false;
                tsPlanToMenu.DisplayStyle = ToolStripItemDisplayStyle.Text;
            }
            else
            {
                pictureBox1.Visible = true;
                tsPlanToMenu.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            }
        }
        
        #endregion
    }
}