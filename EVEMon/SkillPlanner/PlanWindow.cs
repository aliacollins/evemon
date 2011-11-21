using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Data;
using EVEMon.Common.SettingsObjects;
using EVEMon.Controls;

namespace EVEMon.SkillPlanner
{
    /// <summary>
    /// Represents the plan window.
    /// </summary>
    public partial class PlanWindow : EVEMonForm
    {
        private static PlanWindow s_lastActivated;

        // Blank image list for 'Safe for work' setting
        private readonly ImageList m_emptyImageList = new ImageList();

        private Plan m_plan;
        private ImplantCalculator m_implantCalcWindow;
        private AttributesOptimizationForm m_attributesOptimizerWindow;


        #region Initialization and Lifecycle

        /// <summary>
        /// Default constructor for designer.
        /// </summary>
        private PlanWindow()
        {
            InitializeComponent();
            RememberPositionKey = "PlanWindow";

            // ToolStripLabels don't support AutoEllipsis so we user a custom renderer
            // via: http://discuss.joelonsoftware.com/default.asp?dotnet.12.597246.5
            MainStatusStrip.Renderer = new AutoEllipsisToolStripRenderer();
        }

        /// <summary>
        /// Constructor used in code.
        /// </summary>
        /// <param name="plan"></param>
        public PlanWindow(Plan plan)
            : this()
        {
            Plan = plan;
        }

        /// <summary>
        /// On load, we update the controls.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (DesignMode)
                return;

            m_emptyImageList.ImageSize = new Size(24, 24);
            m_emptyImageList.Images.Add(new Bitmap(24, 24));

            // Global events (unsubscribed on window closing)
            EveMonClient.PlanChanged += EveMonClient_PlanChanged;
            EveMonClient.SettingsChanged += EveMonClient_SettingsChanged;
            ResizeEnd += PlanWindow_ResizeEnd;

            // Force an update
            tsddbPlans.DropDownItems.Add("<New Plan>");

            // Compatibility mode : Mac OS
            if (Settings.Compatibility == CompatibilityMode.Wine)
            {
                // Under Wine, the upper tool bar is not displayed
                // We move it at the top of the first tab
                Controls.Remove(upperToolStrip);
                tabControl.TabPages[0].Controls.Add(upperToolStrip);
                tabControl.TabPages[0].Controls.SetChildIndex(upperToolStrip, 0);
            }

            // Show the hint tip
            TipWindow.ShowTip(this, "planner",
                              "Welcome to the Skill Planner",
                              "Select skills to add to your plan using the list on the left. To " +
                              "view the list of skills you've added to your plan, choose " +
                              "\"View Plan\" from the drop down in the upper left.");

            //Update the controls
            UpdateControlsVisibility();

            //Update the status bar
            UpdateStatusBar();
        }

        /// <summary>
        /// On activation, we import the up-to-date plan column settings.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (s_lastActivated == this)
                return;

            s_lastActivated = this;
            planEditor.ImportColumnSettings(Settings.UI.PlanWindow.Columns);
        }

        /// <summary>
        /// On closing, we unsubscribe the global events to help the GC.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            // Save settings if this one is the last activated and up-to-date
            if (s_lastActivated == this)
            {
                Settings.UI.PlanWindow.AddRange(planEditor.ExportColumnSettings());
                s_lastActivated = null;
            }

            // Unsubscribe global events
            EveMonClient.PlanChanged -= EveMonClient_PlanChanged;
            EveMonClient.SettingsChanged -= EveMonClient_SettingsChanged;
            Settings.Save();

            // We're closing down
            if (e.CloseReason != CloseReason.ApplicationExitCall && // and Application.Exit() was not called
                e.CloseReason != CloseReason.TaskManagerClosing &&
                // and the user isn't trying to shut the program down for some reason
                e.CloseReason != CloseReason.WindowsShutDown) // and Windows is not shutting down
            {
                // Tell the skill explorer we're closing down
                SkillExplorerWindow skillExplorerWindow = WindowsFactory.GetByTag<SkillExplorerWindow, PlanWindow>(this);
                WindowsFactory.CloseByTag(skillExplorerWindow, this);

                // Tell the attributes optimization window we're closing down
                if (m_attributesOptimizerWindow != null)
                    m_attributesOptimizerWindow.Close();

                // Tell the implant window we're closing down
                if (m_implantCalcWindow != null)
                    m_implantCalcWindow.Close();
            }

            base.OnFormClosing(e);
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the current character.
        /// </summary>
        public Character Character
        {
            get { return (Character)m_plan.Character; }
        }

        /// <summary>
        /// Gets the plan represented by this window.
        /// </summary>
        public Plan Plan
        {
            get { return m_plan; }
            private set
            {
                if (m_plan == value)
                    return;

                // If the EFTLoadoutImportationForm is open, assign the new plan
                // We do the check here as we need to catch the previous plan value
                EFTLoadoutImportationForm eftloadoutImportation = WindowsFactory.GetByTag<EFTLoadoutImportationForm, Plan>(m_plan);
                if (eftloadoutImportation != null)
                    eftloadoutImportation.Plan = value;

                // If the ShipLoadoutSelectWindow is open, assign the new plan
                ShipLoadoutSelectWindow loadoutSelect = WindowsFactory.GetByTag<ShipLoadoutSelectWindow, Plan>(m_plan);
                if (loadoutSelect != null)
                    loadoutSelect.Plan = value;

                m_plan = value;

                // The tag is used by WindowsFactory.ShowByTag
                Tag = value;

                // Check to see if one or more obsolete entries were found,
                // we this now so the related label is immediately visible
                CheckObsoleteEntries();

                // Jump to the appropriate tab depending on whether
                // or not the plan is empty
                tabControl.SelectedTab = (m_plan.Count == 0 ? tpSkillBrowser : tpPlanQueue);

                // Update controls
                Text = String.Format(CultureConstants.DefaultCulture, "{0} [{1}] - EVEMon Skill Planner", Character.Name,
                                     m_plan.Name);

                // Assign the new plan to the children
                planEditor.Plan = m_plan;
                shipBrowser.Plan = m_plan;
                itemBrowser.Plan = m_plan;
                certBrowser.Plan = m_plan;
                skillBrowser.Plan = m_plan;
                blueprintBrowser.Plan = m_plan;

                // Check to see if one or more invalid entries were 
                // found, we do this last so as not to cause problems
                // for background update tasks.
                CheckInvalidEntries();
            }
        }

        #endregion


        #region Helper methods

        /// <summary>
        /// Updates the controls visibility.
        /// </summary>
        private void UpdateControlsVisibility()
        {
            tabControl.ImageList = (!Settings.UI.SafeForWork
                                        ? ilTabIcons
                                        : m_emptyImageList);

            foreach (ToolStripItem button in upperToolStrip.Items)
            {
                button.DisplayStyle = (!Settings.UI.SafeForWork
                                           ? ToolStripItemDisplayStyle.ImageAndText
                                           : ToolStripItemDisplayStyle.Text);
            }

            foreach (ToolStripItem label in MainStatusStrip.Items)
            {
                label.DisplayStyle = (!Settings.UI.SafeForWork
                                          ? ToolStripItemDisplayStyle.ImageAndText
                                          : ToolStripItemDisplayStyle.Text);
            }
        }

        /// <summary>
        /// Opens this skill in the skill browser and switches to this tab.
        /// </summary>
        /// <param name="gs"></param>
        public void ShowSkillInBrowser(Skill gs)
        {
            tabControl.SelectedTab = tpSkillBrowser;
            skillBrowser.SelectedSkill = gs;
        }

        /// <summary>
        /// Opens this skill in the skill explorer and switches to this tab.
        /// </summary>
        /// <param name="skill"></param>
        public void ShowSkillInExplorer(Skill skill)
        {
            skillBrowser.ShowSkillInExplorer(skill);
        }

        /// <summary>
        /// Opens this ship in the ship browser and switches to this tab.
        /// </summary>
        /// <param name="ship"></param>
        public void ShowShipInBrowser(Item ship)
        {
            tabControl.SelectedTab = tpShipBrowser;
            shipBrowser.SelectedObject = ship;
        }

        /// <summary>
        /// Opens this blueprint in the blueprint browser and switches to this tab.
        /// </summary>
        /// <param name="blueprint"></param>
        public void ShowBlueprintInBrowser(Item blueprint)
        {
            tabControl.SelectedTab = tpBlueprintBrowser;
            blueprintBrowser.SelectedObject = blueprint;
        }

        /// <summary>
        /// Opens this item in the item browser and switches to this tab.
        /// </summary>
        /// <param name="item"></param>
        public void ShowItemInBrowser(Item item)
        {
            tabControl.SelectedTab = tpItemBrowser;
            itemBrowser.SelectedObject = item;
        }

        /// <summary>
        /// Opens this certificate in the certificate browser and switches to this tab.
        /// </summary>
        /// <param name="certificate"></param>
        public void ShowCertInBrowser(Certificate certificate)
        {
            tabControl.SelectedTab = tpCertificateBrowser;
            certBrowser.SelectedCertificate(certificate);
        }

        /// <summary>
        /// Identifies if there are obsolete entries in the skill plan,
        /// displays message if required.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void CheckObsoleteEntries()
        {
            switch (Settings.UI.PlanWindow.ObsoleteEntryRemovalBehaviour)
            {
                case ObsoleteEntryRemovalBehaviour.AlwaysAsk:
                    ObsoleteEntriesStatusLabel.Visible = m_plan.ContainsObsoleteEntries;
                    break;
                case ObsoleteEntryRemovalBehaviour.RemoveAll:
                    m_plan.CleanObsoleteEntries(ObsoleteRemovalPolicy.RemoveAll);
                    break;
                case ObsoleteEntryRemovalBehaviour.RemoveConfirmed:
                    m_plan.CleanObsoleteEntries(ObsoleteRemovalPolicy.ConfirmedOnly);
                    ObsoleteEntriesStatusLabel.Visible = m_plan.ContainsObsoleteEntries;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Identifies if there are invalid entries in the skill plan,
        /// displays message if required.
        /// </summary>
        private void CheckInvalidEntries()
        {
            if (!m_plan.ContainsInvalidEntries)
                return;

            StringBuilder message = new StringBuilder();

            message.AppendLine(
                "When loading the plan one or more skills were not found. " +
                "This can be caused by loading a plan from a previous version of EVEMon or CCP have renamed a skill.");
            message.AppendLine();

            foreach (InvalidPlanEntry entry in m_plan.InvalidEntries)
            {
                message.AppendFormat(CultureConstants.DefaultCulture, " - {0} planned to {1}{2}", entry.SkillName,
                                     entry.PlannedLevel, Environment.NewLine);
            }

            message.AppendLine();
            message.AppendLine(
                "Do you wish to keep these entries?\r\n- " +
                "If you select \"Yes\" the entries will be removed from the plan\r  and will be stored in settings.\r\n- " +
                "If you select \"No\" the entries will be discarded.");

            DialogResult result = MessageBox.Show(message.ToString(), "Invalid Entries Detected",
                                                  MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

            switch (result)
            {
                case DialogResult.No:
                    m_plan.ClearInvalidEntries();
                    break;
                case DialogResult.Yes:
                    m_plan.AcknoledgeInvalidEntries();
                    break;
            }
        }

        /// <summary>
        /// Updates the plan editor's skill selection control.
        /// </summary>
        internal void UpdatePlanEditorSkillSelection()
        {
            SkillSelectControl skillSelectControl = (SkillSelectControl)planEditor.SkillSelectControl;
            skillSelectControl.UpdateContent();
        }

        /// <summary>
        /// Updates the skill browser.
        /// </summary>
        internal void UpdateSkillBrowser()
        {
            skillBrowser.UpdateContent();
        }

        #endregion


        #region Global events

        /// <summary>
        /// Occurs when a plan changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_PlanChanged(object sender, PlanChangedEventArgs e)
        {
            if (m_plan != e.Plan)
                return;

            UpdateEnables();
        }

        /// <summary>
        /// Occurs when plan window gets resized.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlanWindow_ResizeEnd(object sender, EventArgs e)
        {
            UpdateStatusBar();
        }

        /// <summary>
        /// Occurs when the settings changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EveMonClient_SettingsChanged(object sender, EventArgs e)
        {
            UpdateControlsVisibility();
        }

        #endregion


        #region Content creation

        /// <summary>
        /// Enables or disabled some menus.
        /// </summary>
        private void UpdateEnables()
        {
            tsbCopyForum.Enabled = m_plan.Count > 0;
            tsmiPlan.Enabled = m_plan.Count > 0;
        }

        /// <summary>
        /// Updates the skill status label.
        /// </summary>
        /// <param name="selected">if set to <c>true</c> [selected].</param>
        /// <param name="skillCount">The skill count.</param>
        /// <param name="uniqueCount">The unique count.</param>
        internal void UpdateSkillStatusLabel(bool selected, int skillCount, int uniqueCount)
        {
            SkillsStatusLabel.Text = String.Format(CultureConstants.DefaultCulture, "{0} skill{1} {2} ({3} unique)",
                                                   skillCount,
                                                   skillCount == 1 ? String.Empty : "s",
                                                   selected ? "selected" : "planned",
                                                   uniqueCount);
        }

        /// <summary>
        /// Updates the time status label.
        /// </summary>
        /// <param name="skillCount">The skill count.</param>
        /// <param name="totalTime">The total time.</param>
        private void UpdateTimeStatusLabel(int skillCount, TimeSpan totalTime)
        {
            UpdateTimeStatusLabel(false, skillCount, totalTime);
        }

        /// <summary>
        /// Updates the time status label.
        /// </summary>
        /// <param name="selected">if set to <c>true</c> [selected].</param>
        /// <param name="skillCount">The skill count.</param>
        /// <param name="totalTime">The total time.</param>
        internal void UpdateTimeStatusLabel(bool selected, int skillCount, TimeSpan totalTime)
        {
            TimeStatusLabel.AutoToolTip = false;
            TimeStatusLabel.Text = String.Format(CultureConstants.DefaultCulture, "{0} to train {1}",
                                                 totalTime.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas),
                                                 selected
                                                     ? String.Format(CultureConstants.DefaultCulture, "selected skill{0}",
                                                                     skillCount == 1 ? String.Empty : "s")
                                                     : "whole plan");
        }

        /// <summary>
        /// Updates the cost status label.
        /// </summary>
        /// <param name="selected">if set to <c>true</c> [selected].</param>
        /// <param name="totalcost">The totalcost.</param>
        /// <param name="cost">The cost.</param>
        internal void UpdateCostStatusLabel(bool selected, long totalcost, long cost)
        {
            CostStatusLabel.AutoToolTip = totalcost <= 0;

            if (totalcost > 0)
            {
                CostStatusLabel.ToolTipText = String.Format(CultureConstants.DefaultCulture,
                                                            "{0:0,0,0} ISK required to purchase {1} skill{2} anew",
                                                            totalcost,
                                                            selected ? "selected" : "all",
                                                            m_plan.UniqueSkillsCount == 1 ? String.Empty : "s");
            }

            CostStatusLabel.Text = cost > 0
                                       ? String.Format(CultureConstants.DefaultCulture, "{0:0,0,0} ISK required", cost)
                                       : "0 ISK required";
        }

        /// <summary>
        /// Autonomously updates the status bar with the plan's training time.
        /// </summary>
        internal void UpdateStatusBar()
        {
            // Training time
            CharacterScratchpad scratchpad = m_plan.ChosenImplantSet != null
                                                 ? m_plan.Character.After(m_plan.ChosenImplantSet)
                                                 : new CharacterScratchpad(Character);

            TimeSpan totalTime = planEditor.DisplayPlan.GetTotalTime(scratchpad, true);

            UpdateSkillStatusLabel(false, m_plan.Count, m_plan.UniqueSkillsCount);
            UpdateTimeStatusLabel(m_plan.Count, totalTime);
            UpdateCostStatusLabel(false, m_plan.TotalBooksCost, m_plan.NotKnownSkillBooksCost);
        }

        #endregion


        #region Controls handlers

        /// <summary>
        /// Toolbar > Delete.
        /// Prompts the user and delete the currently selected plan.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbDeletePlan_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Are you sure you want to delete this plan?", "Delete Plan",
                                              MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                                              MessageBoxDefaultButton.Button2);

            if (dr != DialogResult.Yes)
                return;

            // Close the skill explorer
            SkillExplorerWindow skillExplorerWindow = WindowsFactory.GetByTag<SkillExplorerWindow, PlanWindow>(this);
            WindowsFactory.CloseByTag(skillExplorerWindow, this);

            // Remove the plan
            int index = Character.Plans.IndexOf(m_plan);
            Character.Plans.Remove(m_plan);

            // Choose which plan to show next
            // By default we choose the next one,
            // if it was the last in the list we select the previous one
            if (index > Character.Plans.Count - 1)
                index--;

            // When no plans exists after deletion we close the window
            if (index < 0)
            {
                Close();
                return;
            }

            // New plan to show
            Plan newplan = Character.Plans[index];
            Plan = newplan;
        }

        /// <summary>
        /// When the selected tab changes we update the list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DesignMode)
                return;

            // Force update of column widths in case we've just created a new plan from within the planner window
            if (tabControl.SelectedIndex == 0)
                planEditor.UpdateListColumns();
        }

        /// <summary>
        /// Status bar > Obsolete entries label.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void obsoleteEntriesToolStripStatusLabel_Click(object sender, EventArgs e)
        {
            ObsoleteEntriesAction action = ObsoleteEntriesForm.ShowDialog(m_plan);

            switch (action)
            {
                case ObsoleteEntriesAction.RemoveAll:
                    planEditor.ClearObsoleteEntries(ObsoleteRemovalPolicy.RemoveAll);
                    ObsoleteEntriesStatusLabel.Visible = false;
                    break;
                case ObsoleteEntriesAction.RemoveConfirmed:
                    planEditor.ClearObsoleteEntries(ObsoleteRemovalPolicy.ConfirmedOnly);
                    ObsoleteEntriesStatusLabel.Visible = false;
                    break;
                case ObsoleteEntriesAction.KeepAll:
                case ObsoleteEntriesAction.None:
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// When the plans menu is opening, we update the items.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsddbPlans_DropDownOpening(object sender, EventArgs e)
        {
            tsddbPlans.DropDownItems.Clear();
            tsddbPlans.DropDownItems.Add("<New Plan>");

            Character.Plans.AddTo(
                tsddbPlans.DropDownItems,
                (menuPlanItem, plan) =>
                    {
                        menuPlanItem.Tag = plan;
                        menuPlanItem.ToolTipText = plan.DescriptionNL;

                        // Put current plan to bold
                        if (plan == m_plan)
                            menuPlanItem.Enabled = false;
                            // Is it already opened in another plan ?
                        else if (WindowsFactory.GetByTag<PlanWindow, Plan>(plan) != null)
                            menuPlanItem.Font = FontFactory.GetFont(menuPlanItem.Font, FontStyle.Italic);
                    });
        }

        /// <summary>
        /// Occurs when the user clicks one of the children of the "Plans" menu.
        /// The item may be an existing plan or the "New plan" item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsddbPlans_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Tag == m_plan)
                return;

            // Is it another plan ?
            if (e.ClickedItem.Tag != null)
            {
                Plan plan = (Plan)e.ClickedItem.Tag;
                PlanWindow window = WindowsFactory.GetByTag<PlanWindow, Plan>(plan);

                // Opens the existing window when there is one, or switch to this plan when no window opened
                if (window != null)
                    window.BringToFront();
                else
                    Plan = plan;

                return;
            }

            // So it is "new plan"
            using (NewPlanWindow npw = new NewPlanWindow())
            {
                DialogResult dr = npw.ShowDialog();
                if (dr == DialogResult.Cancel)
                    return;

                Plan plan = new Plan(Character)
                                {
                                    Name = npw.PlanName,
                                    Description = npw.PlanDescription
                                };
                Character.Plans.Add(plan);
                Plan = plan;
            }
        }

        /// <summary>
        /// Toolbar > Export > Plan.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiPlan_Click(object sender, EventArgs e)
        {
            UIHelper.ExportPlan(m_plan);
        }

        /// <summary>
        /// Toolbar > Export > After Plan Character.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiAfterPlanCharacter_Click(object sender, EventArgs e)
        {
            UIHelper.ExportAfterPlanCharacter(Character, m_plan);
        }

        /// <summary>
        /// Opens the EFTLoadout form and passes it the current Plan.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbEFTImport_Click(object sender, EventArgs e)
        {
            WindowsFactory.ShowByTag<EFTLoadoutImportationForm, Plan>(m_plan);
        }

        /// <summary>
        /// Toolbar > Copy to clipboard.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbCopyForum_Click(object sender, EventArgs e)
        {
            // Prompt the user for settings. When null, the user cancelled.
            PlanExportSettings settings = UIHelper.PromptUserForPlanExportSettings(m_plan);
            if (settings == null)
                return;

            string output = PlanIOHelper.ExportAsText(m_plan, settings);

            // Copy the result to the clipboard.
            try
            {
                Clipboard.Clear();
                Clipboard.SetText(output);

                MessageBox.Show("The skill plan has been copied to the clipboard in a " +
                                "format suitable for forum posting.", "Plan Copied", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            catch (ExternalException ex)
            {
                ExceptionHandler.LogException(ex, true);

                MessageBox.Show("The copy to clipboard has failed. You may retry later", "Plan Copy Failure",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Toolbar > Implants calculator.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbImplantCalculator_Click(object sender, EventArgs e)
        {
            PlanEditorControl control = (tabControl.SelectedIndex == 0) ? planEditor : null;

            if (m_implantCalcWindow == null)
            {
                m_implantCalcWindow = new ImplantCalculator(m_plan);
                m_implantCalcWindow.FormClosed += (form, args) => m_implantCalcWindow = null;
                m_implantCalcWindow.SetPlanEditor(control);
                m_implantCalcWindow.Show(this);
            }
            else
            {
                m_implantCalcWindow.Visible = true;
                m_implantCalcWindow.BringToFront();
                m_implantCalcWindow.SetPlanEditor(control);
            }
        }

        /// <summary>
        /// Toolbar > Attributes optimizer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbAttributesOptimization_Click(object sender, EventArgs e)
        {
            if (m_attributesOptimizerWindow == null)
            {
                // Display the settings window
                using (AttributesOptimizationSettingsForm settingsForm = new AttributesOptimizationSettingsForm(m_plan))
                {
                    settingsForm.ShowDialog(this);

                    if (settingsForm.DialogResult != DialogResult.OK)
                        return;

                    // Now displays the computation window
                    m_attributesOptimizerWindow = settingsForm.OptimizationForm;
                    m_attributesOptimizerWindow.PlanEditor = (tabControl.SelectedIndex == 0) ? planEditor : null;
                    m_attributesOptimizerWindow.FormClosed += (form, args) => m_attributesOptimizerWindow = null;
                    m_attributesOptimizerWindow.Show(this);
                }
                return;
            }

            // Bring the window to front
            m_attributesOptimizerWindow.Visible = true;
            m_attributesOptimizerWindow.BringToFront();
            m_attributesOptimizerWindow.PlanEditor = (tabControl.SelectedIndex == 0) ? planEditor : null;
        }

        /// <summary>
        /// Toolbar > Print.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbPrintPlan_Click(object sender, EventArgs e)
        {
            PlanPrinter.Print(m_plan);
        }

        #endregion
    }
}