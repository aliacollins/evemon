using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Runtime.InteropServices;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.SettingsObjects;
using EVEMon.Common.Data;

namespace EVEMon.SkillPlanner
{
    /// <summary>
    /// Represents the plan window.
    /// </summary>
    public partial class PlanWindow : EVEMonForm
    {
        private static PlanWindow s_lastActivated;

        private Plan m_plan;
        private ImplantCalculator m_implantCalcWindow;
        private ShipLoadoutSelectWindow m_loadoutForm;
        private AttributesOptimizationForm m_attributesOptimizerWindow;

        private bool m_obsDialogActive;


        #region Initialization, loading, closing, entries validation checking
        /// <summary>
        /// Default constructor for designer
        /// </summary>
        public PlanWindow()
        {
            InitializeComponent();
            this.RememberPositionKey = "PlanWindow";

            this.ResizeEnd += new EventHandler(PlanWindow_ResizeEnd);
        }

        /// <summary>
        /// Constructor used in code.
        /// </summary>
        /// <param name="plan"></param>
        public PlanWindow(Plan plan)
            : this()
        {
            this.Plan = plan;

            // Global events (unsubscribed on window closing)
            EveClient.PlanChanged += new EventHandler<PlanChangedEventArgs>(EveClient_PlanChanged);
        }

        /// <summary>
        /// On load, we update the controls.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (this.DesignMode) return;

            // Force an update
            tsddbPlans.DropDownItems.Add("<New Plan>");

            // Compatibility mode : Mac OS
            if (Settings.Compatibility == CompatibilityMode.Wine)
            {
                // Under Wine, the upper toolbar is not displayed
                // We move it at the top of the first tab
                this.Controls.Remove(this.upperToolStrip);
                this.tabControl.TabPages[0].Controls.Add(this.upperToolStrip);
                this.tabControl.TabPages[0].Controls.SetChildIndex(this.upperToolStrip, 0);
            }

            // Show the hint tip
            TipWindow.ShowTip(this, "planner",
                              "Welcome to the Skill Planner",
                              "Select skills to add to your plan using the list on the left. To " +
                              "view the list of skills you've added to your plan, choose " +
                              "\"View Plan\" from the dropdown in the upper left.");

            UpdateStatusBar();
        }

        /// <summary>
        /// On activation, we import the up-to-date plan column settings.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (s_lastActivated != this)
            {
                s_lastActivated = this;
                planEditor.ImportColumnSettings(Settings.UI.PlanWindow.Columns);
            }
        }

        /// <summary>
        /// On deactivation, we export this window's plan column settings.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            Settings.UI.PlanWindow.Columns = planEditor.ExportColumnSettings().ToArray();
        }

        /// <summary>
        /// On closing, we unsubscribe the global events to help the GC.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Save settings if this one is the last activated and up-to-date
            if (s_lastActivated == this)
            {
                Settings.UI.PlanWindow.Columns = planEditor.ExportColumnSettings().ToArray();
                s_lastActivated = null;
            }

            // Unsubscribe global events
            EveClient.PlanChanged -= new EventHandler<PlanChangedEventArgs>(EveClient_PlanChanged);
            Settings.Save();

            // Tell the skill explorer we're closing down
            if (!(e.CloseReason == CloseReason.ApplicationExitCall) && // and Application.Exit() was not called
                !(e.CloseReason == CloseReason.TaskManagerClosing) &&  // and the user isn't trying to shut the program down for some reason
                !(e.CloseReason == CloseReason.WindowsShutDown))       // and Windows is not shutting down
            {
                WindowsFactory<SkillExplorerWindow>.CloseByTag(this);
            }

            // Tell the attributes optimization window we're closing
            if (m_attributesOptimizerWindow != null)
            {
                m_attributesOptimizerWindow.Close();
                m_attributesOptimizerWindow = null;
            }

            // Tell the implant window we're closing
            if (m_implantCalcWindow != null)
            {
                m_implantCalcWindow.Close();
                m_implantCalcWindow = null;
            }

            // Tells the loadout browser we're closing
            if (m_loadoutForm != null)
            {
                m_loadoutForm.Close();
                m_loadoutForm = null;
            }

            base.OnFormClosing(e);
        }

        /// <summary>
        /// Gets the current character
        /// </summary>
        public Character Character
        {
            get { return (Character)m_plan.Character; }
        }

        /// <summary>
        /// Gets the obsolete entries dialog box status
        /// </summary>
        public bool IsObsDialogActive
        {
            get { return m_obsDialogActive; }
        }

        /// <summary>
        /// Gets the plan represented by this window
        /// </summary>
        public Plan Plan
        {
            get { return m_plan; }
            protected set
            {
                if (m_plan == value)
                    return;
                m_plan = value;

                // The tag is used by WindowsFactory.ShowByTag
                this.Tag = value;

                // Assign the new plan to the children
                planEditor.Plan = m_plan;
                shipBrowser.Plan = m_plan;
                itemBrowser.Plan = m_plan;
                certBrowser.Plan = m_plan;
                skillBrowser.Plan = m_plan;
                var loadoutSelect = WindowsFactory<ShipLoadoutSelectWindow>.GetUnique();
                if (loadoutSelect != null)
                    loadoutSelect.Plan = m_plan;

                // Jump to the appropriate tab depending on whether
                // or not the plan is empty
                if (m_plan.Count == 0)
                    tabControl.SelectedTab = tpSkillBrowser;
                else tabControl.SelectedTab = tpPlanQueue;

                // Update controls
                this.Text = this.Character.Name + " [" + m_plan.Name + "] - EVEMon Skill Planner";
                planEditor.UpdateListColumns();

                // Check to see if one or more obsolete entries were found
                CheckObsoleteEntries();

                // Check to see if one or more invalid entries were 
                // found, we do this last so as not to cause problems
                // for background update tasks.
                CheckInvalidEntries();
            }
        }

        /// <summary>
        /// Identifies if there are obsolete entries in the skill plan,
        /// displays message if required.
        /// </summary>
        public void CheckObsoleteEntries()
        {
            if (m_plan.ObsoleteEntries)
            {
                bool showDialog = Settings.UI.PlanWindow.ObsEntriesMsgBox.ShowDialogBox;
                DialogResult dlr = Settings.UI.PlanWindow.ObsEntriesMsgBox.DialogResult;

                if (showDialog)
                {
                    m_obsDialogActive = true;

                    string text = String.Concat("The plan contains one or more obsolete entries.",
                        " (Obsolete entry is an entry for a skill level that is already trained).\r\n\r\n",
                           "Do you wish them to be removed ?"),
                    captionText = "Obsolete Entry",
                    cbOptionText = "Do not show this dialog again";
                    
                    // Shows the custom dialog box
                    MessageBoxCustom MsgBoxCustom = new MessageBoxCustom();
                    DialogResult drb = MsgBoxCustom.Show(this, text, captionText, cbOptionText, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                    Settings.UI.PlanWindow.ObsEntriesMsgBox.ShowDialogBox = MsgBoxCustom.cbUnchecked;

                    // When the checkbox is checked we store the dialog result
                    if (!MsgBoxCustom.cbUnchecked)
                        Settings.UI.PlanWindow.ObsEntriesMsgBox.DialogResult = drb;

                    if (drb == DialogResult.Yes)
                        planEditor.ClearObsoleteEntries();

                    m_obsDialogActive = false;
                }
                else
                {
                    if (dlr == DialogResult.Yes)
                        planEditor.ClearObsoleteEntries();
                }
            }
        }

        /// <summary>
        /// Identifies if there are invalid entries in the skill plan,
        /// displays message if required.
        /// </summary>
        private void CheckInvalidEntries()
        {
            if (m_plan.ContainsInvalidEntries)
            {
                StringBuilder message = new StringBuilder();

                message.AppendLine("When loading the plan one or more skills were not found. This can be caused by loading a plan from a previous version of EVEMon or CCP have renamed a skill.");
                message.AppendLine();

                foreach (var entry in m_plan.InvalidEntries)
                {
                    message.AppendFormat(" - {0} planned to {1}{2}", entry.SkillName, entry.PlannedLevel, Environment.NewLine);
                }

                message.AppendLine();
                message.AppendLine("Do you wish to keep these entries?\r\n- If you select \"Yes\" the entries will be removed from the plan\r  and will be stored in settings.\r\n- If you select \"No\" the entries will be discarded.");

                var result = MessageBox.Show(message.ToString(), "Invalid Entries Detected", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

                if (result == DialogResult.No)
                {
                    m_plan.ClearInvalidEntries();
                }
                else if (result == DialogResult.Yes)
                {
                    m_plan.AcknoledgeInvalidEntries();
                }
            }
        }
        #endregion


        #region Helper methods
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
        /// <param name="gs"></param>
        public void ShowSkillInExplorer(Skill gs)
        {
            skillBrowser.ShowSkillInExplorer(gs);
        }

        /// <summary>
        /// Opens this ship in the ship browser and switches to this tab.
        /// </summary>
        /// <param name="gs"></param>
        public void ShowShipInBrowser(Item s)
        {
            tabControl.SelectedTab = tpShipBrowser;
            shipBrowser.SelectedObject = s;
        }

        /// <summary>
        /// Opens this item in the item browser and switches to this tab.
        /// </summary>
        /// <param name="gs"></param>
        public void ShowItemInBrowser(Item i)
        {
            tabControl.SelectedTab = tpItemBrowser;
            itemBrowser.SelectedObject = i;
        }

        /// <summary>
        /// Opens this certificate in the certificate browser and switches to this tab.
        /// </summary>
        /// <param name="gs"></param>
        public void ShowCertInBrowser(Certificate c)
        {
            tabControl.SelectedTab = tpCertificateBrowser;
            certBrowser.SelectedCertificate = c;
        }
        #endregion


        #region Global events
        /// <summary>
        /// Occurs when a plan changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveClient_PlanChanged(object sender, PlanChangedEventArgs e)
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
        /// Update the status bar with the given text (used by the <see cref="PlanEditor"/> control when skills are selected).
        /// </summary>
        /// <param name="txt"></param>
        public void UpdateStatusBarSelected(String txt)
        {
            slblStatusText.Text = txt;
            FixStatusBarTextLength();
        }

        /// <summary>
        /// Autonomously updates the status bar with the plan's training time.
        /// </summary>
        public void UpdateStatusBar()
        {
            // Training time
            var scratchpad = new CharacterScratchpad(Character);
            if (m_plan.ChosenImplantSet != null)
                scratchpad = m_plan.Character.After(m_plan.ChosenImplantSet);
            TimeSpan totalTime = planEditor.DisplayPlan.GetTotalTime(scratchpad, true);
            slblStatusText.Text = String.Format("{0} Skill{1} Planned ({2} Unique Skill{3}). Total training time: {4}. ",
                                                m_plan.Count,
                                                m_plan.Count == 1 ? "" : "s",
                                                m_plan.UniqueSkillsCount,
                                                m_plan.UniqueSkillsCount == 1 ? "" : "s",
                                                Skill.TimeSpanToDescriptiveText(totalTime, DescriptiveTextOptions.IncludeCommas));

            // Books cost
            long totalcost = m_plan.TotalBooksCost;
            long cost = m_plan.NotKnownSkillBooksCost;
            if (totalcost > 0)
            {
                slblStatusText.Text += String.Format("Total skill book{0} cost: {1:0,0,0} ISK. ",
                    m_plan.UniqueSkillsCount == 1 ? "" : "s", totalcost);
            }

            if (cost > 0)
            {
                slblStatusText.Text += String.Format("Not known skill book{0} cost: {1:0,0,0} ISK. ",
                    m_plan.NotKnownSkillsCount == 1 ? "" : "s", cost);
            }

            // Suggestions
            var suggestions = m_plan.GetSuggestions();
            if (suggestions.Count != 0)
            {
                if (this.Visible && !tslSuggestion.Visible)
                {
                    tslSuggestion.Visible = true;
                    TipWindow.ShowTip(this, "suggestion",
                                      "Plan Suggestion",
                                      "EVEMon found learning skills that would lower " +
                                      "the overall training time of the plan. To view those " +
                                      "suggestions and the resulting change in plan time, click the " +
                                      "\"Suggestion\" link in the planner status bar.");
                }
            }
            else tslSuggestion.Visible = false;

            FixStatusBarTextLength();
        }

        /// <summary>
        /// Trims the status text in an ellipsis style to fit the windows width
        /// </summary>
        private void FixStatusBarTextLength()
        {
            string ellipsis = "…";

            int suggestionTextWidth = (tslSuggestion.Visible ? tslSuggestion.Width : 0);
            int statusTextWidth = TextRenderer.MeasureText(slblStatusText.Text, slblStatusText.Font).Width;
            int statusBarTextWidth = statusTextWidth + suggestionTextWidth;
            int ellipsisWidth = TextRenderer.MeasureText(ellipsis, slblStatusText.Font).Width;
            float factor = (float)statusBarTextWidth / (float)slblStatusText.Text.Length;
            int availableTextWidth = this.Width - slblStatusText.Height * 2 - (int)factor;

            // We check the status text lenght to the windows width
            if (statusBarTextWidth > availableTextWidth)
            {
                // We calculate the position to trim the text and remove it
                int position = (int)(availableTextWidth / factor) - ellipsis.Length * 2 - (int)factor;

                // the measurements are a bit of an inexact science
                // check the position is still valid.
                if (position < 0)
                    return;

                if (position >= slblStatusText.Text.Length)
                    return;

                // trim the text
                string newStatusBarText = slblStatusText.Text.Remove(position);

                // Adds the ellipsis
                newStatusBarText += ellipsis;
                slblStatusText.Text = newStatusBarText;
            }
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
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if (dr != DialogResult.Yes)
                return;

            // Close the skill explorer
            WindowsFactory<SkillExplorerWindow>.CloseByTag(this);

            // Remove the plan
            var i = Character.Plans.IndexOf(m_plan);
            Character.Plans.Remove(m_plan);

            // Choose which plan to show next
            // By default we choose the next one,
            // if it was the last in the list we select the previous one
            if (i > Character.Plans.Count - 1)
                i--;
            
            // When no plans exists after deletion we close the window
            if (i < 0)
            {
                this.Close();
                return;
            }

            // New plan to show
            var newplan = Character.Plans[i];
            this.Plan = newplan;
        }

        /// <summary>
        /// When the selected 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.DesignMode)
                return;

            // Force update of column widths in case we've just created a new plan from within the planner window.
            if (tabControl.SelectedIndex == 0)
            {
                planEditor.UpdateListColumns();
            }
        }

        /// <summary>
        /// Status bar > Suggestions button.
        /// Open the suggestions window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tslSuggestion_Click(object sender, EventArgs e)
        {
            using (SuggestionWindow f = new SuggestionWindow(m_plan))
            {
                DialogResult dr = f.ShowDialog();
                if (dr == DialogResult.Cancel)
                    return;
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

            Character.Plans.AddTo(tsddbPlans.DropDownItems, (menuPlanItem, plan) => {
                    menuPlanItem.Tag = plan;

                    // Put current plan to bold
                    if (plan == m_plan)
                    {
                        menuPlanItem.Enabled = false;
                    }
                    // Is it already opened in another plan ?
                    else if (WindowsFactory<PlanWindow>.GetByTag(plan) != null)
                    {
                        menuPlanItem.Font = FontFactory.GetFont(menuPlanItem.Font, FontStyle.Italic);
                    }
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
                var plan = (Plan)e.ClickedItem.Tag;
                var window = WindowsFactory<PlanWindow>.GetByTag(plan);

                // Opens the existing window when there is one, or switch to this plan when no window opened.
                if (window != null)
                    window.BringToFront();
                else this.Plan = plan;

                return;
            }

            // So it is "new plan"
            using (NewPlanWindow npw = new NewPlanWindow())
            {
                DialogResult dr = npw.ShowDialog();
                if (dr == DialogResult.Cancel)
                    return;

                var plan = new Plan(Character);
                plan.Name = npw.Result;
                Character.Plans.Add(plan);
                this.Plan = plan;
            }
        }


        /// <summary>
        /// Toolbar > Export > Plan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiPlan_Click(object sender, EventArgs e)
        {
            UIHelper.ExportPlan(m_plan);
        }

        /// <summary>
        /// Toolbar > Export > After Plan Character
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiAfterPlanCharacter_Click(object sender, EventArgs e)
        {
            UIHelper.ExportAfterPlanCharacter(Character, m_plan);
        }

        /// <summary>
        /// Opens the EFTLoadout form and passes it the current Plan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbEFTImport_Click(object sender, EventArgs e)
        {
            EFTLoadoutImportationForm eftLoadout = new EFTLoadoutImportationForm(m_plan);
            eftLoadout.Show(this);
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

            string output = PlanExporter.ExportAsText(m_plan, settings);

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

                MessageBox.Show("The copy to clipboard has failed. You may retry later", "Plan Copy Failure", MessageBoxButtons.OK,
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
            if (m_implantCalcWindow == null)
            {
                m_implantCalcWindow = new ImplantCalculator(m_plan);
                m_implantCalcWindow.FormClosed += (form, args) => m_implantCalcWindow = null;
                m_implantCalcWindow.PlanEditor = (tabControl.SelectedIndex == 0) ? planEditor : null;
                m_implantCalcWindow.Show(this);
            }
            else
            {
                m_implantCalcWindow.Visible = true;
                m_implantCalcWindow.BringToFront();
                m_implantCalcWindow.PlanEditor = (tabControl.SelectedIndex == 0) ? planEditor : null;
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
                var settingsForm = new AttributesOptimizationSettingsForm(m_plan);
                settingsForm.ShowDialog(this);

                if (settingsForm.DialogResult == DialogResult.OK)
                {
                    // Now displays the computation window
                    m_attributesOptimizerWindow = settingsForm.OptimizationForm;
                    m_attributesOptimizerWindow.PlanEditor = (tabControl.SelectedIndex == 0) ? planEditor : null;
                    m_attributesOptimizerWindow.FormClosed += (form, args) => m_attributesOptimizerWindow = null;
                    m_attributesOptimizerWindow.Show(this);
                }
            }
            else
            {
                // Bring the window to front
                m_attributesOptimizerWindow.Visible = true;
                m_attributesOptimizerWindow.BringToFront();
                m_attributesOptimizerWindow.PlanEditor = (tabControl.SelectedIndex == 0) ? planEditor : null;
            }
        }

        /// <summary>
        /// Toolbar > Print
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
