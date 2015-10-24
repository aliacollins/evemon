using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Constants;
using EVEMon.Common.Controls;
using EVEMon.Common.Data;
using EVEMon.Common.Factories;
using EVEMon.Common.Helpers;
using EVEMon.Common.Interfaces;
using EVEMon.Common.Models;

namespace EVEMon.SkillPlanner
{
    internal partial class ShipBrowserControl : EveObjectBrowserControl
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ShipBrowserControl()
        {
            InitializeComponent();
            scObjectBrowser.RememberDistanceKey = "ShipsBrowser_Left";
            SelectControl = shipSelectControl;
            PropertiesList = lvShipProperties;
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// Opens the BattleClinic Loadout window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblBattleclinic_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShipLoadoutSelectWindow window = WindowsFactory.ShowByTag<ShipLoadoutSelectWindow, Plan>(Plan);
            window.Ship = SelectedObject;
        }

        /// <summary>
        /// Exports item info to CSV format.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportToCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewExporter.CreateCSV(PropertiesList, true);
        }

        private void tsPlanToLevelOne_Click(object sender, EventArgs e)
        {
            PlanTo(masteryTreeDisplayControl.MasteryShip.FirstOrDefault(m => m.Level == 1));
        }

        private void tsPlanToLevelTwo_Click(object sender, EventArgs e)
        {
            PlanTo(masteryTreeDisplayControl.MasteryShip.FirstOrDefault(m => m.Level == 2));
        }

        private void tsPlanToLevelThree_Click(object sender, EventArgs e)
        {
            PlanTo(masteryTreeDisplayControl.MasteryShip.FirstOrDefault(m => m.Level == 3));
        }

        private void tsPlanToLevelFour_Click(object sender, EventArgs e)
        {
            PlanTo(masteryTreeDisplayControl.MasteryShip.FirstOrDefault(m => m.Level == 4));
        }

        private void tsPlanToLevelFive_Click(object sender, EventArgs e)
        {
            PlanTo(masteryTreeDisplayControl.MasteryShip.FirstOrDefault(m => m.Level == 5));
        }

        /// <summary>
        /// Handles the DropDownOpening event of the tsPlanToMenu control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void tsPlanToMenu_DropDownOpening(object sender, EventArgs e)
        {
            //UpdateEligibility();
        }

        #endregion


        #region Inherited Events

        /// <summary>
        /// Occurs when the settings changed.
        /// </summary>
        protected override void OnSettingsChanged()
        {
            base.OnSettingsChanged();
            UpdateControlVisibility();
        }

        /// <summary>
        /// Occurs when the conrol loads.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            // Return on design mode
            if (DesignMode || this.IsDesignModeHosted())
                return;

            base.OnLoad(e);

            UpdateControlVisibility();
        }

        /// <summary>
        /// Updates the controls when the selection is changed.
        /// </summary>
        protected override void OnSelectionChanged()
        {
            base.OnSelectionChanged();
            if (SelectedObject == null)
                return;

            // Description
            tbDescription.Text = SelectedObject.Description;

            // Required Skills
            requiredSkillsControl.Object = SelectedObject;

            // Update the Mastery tab
            masteryTreeDisplayControl.Ship = SelectedObject;

            ShipLoadoutSelectWindow loadoutSelect = WindowsFactory.GetByTag<ShipLoadoutSelectWindow, Plan>(Plan);
            if (loadoutSelect != null && !loadoutSelect.IsDisposed)
                loadoutSelect.Ship = shipSelectControl.SelectedObject;

            // Update toolstrip
            UpdateEligibility();
        }

        /// <summary>
        /// Updates the plan when the selection is changed.
        /// </summary>
        protected override void OnPlanChanged()
        {
            base.OnPlanChanged();
            requiredSkillsControl.Plan = Plan;
            masteryTreeDisplayControl.Plan = Plan;

            // We recalculate the right panels minimum size
            int reqSkillControlMinWidth = requiredSkillsControl.MinimumSize.Width;
            int reqSkillPanelMinWidth = scDetails.Panel2MinSize;
            scDetails.Panel2MinSize = (reqSkillPanelMinWidth > reqSkillControlMinWidth
                                           ? reqSkillPanelMinWidth
                                           : reqSkillControlMinWidth);

            UpdateEligibility();
        }

        #endregion


        #region Helper Methods

        private void PlanTo(Mastery mastery)
        {
            IPlanOperation operation = Plan.TryPlanTo(mastery);
            if (operation == null)
                return;

            PlanWindow window = WindowsFactory.ShowByTag<PlanWindow, Plan>(operation.Plan);
            if (window == null || window.IsDisposed)
                return;

            PlanHelper.SelectPerform(new PlanToOperationForm(operation), window, operation);

            UpdateEligibility();
        }

        /// <summary>
        /// Updates the contol visibility.
        /// </summary>
        private void UpdateControlVisibility()
        {
            lblBattleclinic.Location = Settings.UI.SafeForWork
                                           ? new Point(Pad, lblBattleclinic.Location.Y)
                                           : new Point(eoImage.Width + Pad * 2, lblBattleclinic.Location.Y);
        }

        /// <summary>
        /// Updates eligibility label and planning menus.
        /// </summary>
        private void UpdateEligibility()
        {
            if (SelectedObject == null)
                return;

            MasteryShip masteryShip = StaticMasteries.GetMasteryShipByID(SelectedObject.ID);
            
            if (masteryShip == null)
                return;

            // Update the training status of each mastery level
            masteryShip.TryUpdateCertificateStatus((Character)Plan.Character);

            // First we search the highest eligible certificate after this plan
            IEnumerable<Mastery> eligibleMasteryLevel = masteryShip.TakeWhile(masteryLevel => Plan.WillGrantEligibilityFor(masteryLevel)).ToList();

            Mastery lastEligibleMasteryLevel = null;
            if (!eligibleMasteryLevel.Any())
            {
                tslbEligible.Text = @"(none)";
            }
            else
            {
                lastEligibleMasteryLevel = eligibleMasteryLevel.Last();
                tslbEligible.Text = lastEligibleMasteryLevel.ToString();

                if (masteryShip.HighestTrainedLevel == null)
                {
                    tslbEligible.Text += @" (improved from ""none"")";
                }
                else if (lastEligibleMasteryLevel.Level > masteryShip.HighestTrainedLevel.Level)
                {
                    tslbEligible.Text += string.Format(CultureConstants.DefaultCulture, " (improved from \"{0}\")", masteryShip.HighestTrainedLevel);
                }
                else
                {
                    tslbEligible.Text += @" (no change)";
                }
            }
            
            UpdatePlanningMenuStatus(tsPlanToLevelOne, masteryShip.FirstOrDefault(m => m.Level == 1), lastEligibleMasteryLevel);
            UpdatePlanningMenuStatus(tsPlanToLevelTwo, masteryShip.FirstOrDefault(m => m.Level == 2), lastEligibleMasteryLevel);
            UpdatePlanningMenuStatus(tsPlanToLevelThree, masteryShip.FirstOrDefault(m => m.Level == 3), lastEligibleMasteryLevel);
            UpdatePlanningMenuStatus(tsPlanToLevelFour, masteryShip.FirstOrDefault(m => m.Level == 4), lastEligibleMasteryLevel);
            UpdatePlanningMenuStatus(tsPlanToLevelFive, masteryShip.FirstOrDefault(m => m.Level == 5), lastEligibleMasteryLevel);
        }

        /// <summary>
        /// Updates a "plan to" menu.
        /// </summary>
        /// <param name="menu">The menu to update</param>
        /// <param name="level">The level represent by this menu</param>
        /// <param name="lastEligibleMasteryLevel">The highest eligible mastery after this plan</param>
        private static void UpdatePlanningMenuStatus(ToolStripItem menu, Mastery level, Mastery lastEligibleMasteryLevel)
        {
            Mastery mastery = level;
            menu.Visible = mastery != null;
            menu.Enabled = mastery != null && (lastEligibleMasteryLevel == null || mastery.Level > lastEligibleMasteryLevel.Level);
        }

        #endregion 
    }
}