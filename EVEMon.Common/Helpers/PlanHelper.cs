using System;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common.Constants;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Interfaces;
using EVEMon.Common.Models;

namespace EVEMon.Common.Helpers
{
    /// <summary>
    /// Helper for the "Plan To" and "Remove" menus.
    /// </summary>
    public static class PlanHelper
    {
        /// <summary>
        /// Updates a regular "Plan to X" menu : text, tag, enable/disable.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <param name="plan">The plan.</param>
        /// <param name="skill">The skill.</param>
        /// <param name="level">The level.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// menu
        /// or
        /// plan
        /// </exception>
        public static bool UpdatesRegularPlanToMenu(ToolStripItem menu, Plan plan, Skill skill, int level)
        {
            if (menu == null)
                throw new ArgumentNullException("menu");

            if (plan == null)
                throw new ArgumentNullException("plan");

            menu.Text = level == 0 ? "Remove" : String.Format(CultureConstants.DefaultCulture, "Level {0}", level);

            menu.Enabled = EnablePlanTo(plan, skill, level);
            if (menu.Enabled)
            {
                IPlanOperation operation = plan.TryPlanTo(skill, level);
                menu.Tag = operation;
                if (RequiresWindow(operation))
                    menu.Text += "...";
            }

            ToolStripMenuItem menuItem = menu as ToolStripMenuItem;
            if (menuItem != null)
                menuItem.Checked = (plan.GetPlannedLevel(skill) == level);

            return menu.Enabled;
        }

        /// <summary>
        /// Checks whether the given skill level can be planned. Used to enable or disable the "Plan To N" and "Remove" menu options.
        /// </summary>
        /// <param name="plan">The plan.</param>
        /// <param name="skill">The skill.</param>
        /// <param name="level">A integer between 0 (remove all entries for this skill) and 5.</param>
        /// <returns></returns>
        private static bool EnablePlanTo(BasePlan plan, Skill skill, int level)
        {
            // The entry actually wants to remove the item
            if (level == 0)
                return plan.IsPlanned(skill);

            // The entry is already known
            if (skill.Level >= level)
                return false;

            // The entry is already planned at this very level ?
            return plan.GetPlannedLevel(skill) != level;
        }

        /// <summary>
        /// Checks whether the given operation absolutely requires a confirmation from the user.
        /// True when there are dependencies to remove.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <returns></returns>
        public static bool RequiresWindow(IPlanOperation operation)
        {
            if (operation == null)
                return false;

            if (operation.Type != PlanOperations.Suppression)
                return false;

            return (operation.SkillsToRemove.Count() != operation.AllEntriesToRemove.Count());
        }

        /// <summary>
        /// Performs the action for the "Plan To N" and "Remove" menu options, in a silent way whenever possible.
        /// </summary>
        /// <param name="operationForm">The operation form.</param>
        /// <param name="parentForm">The parent form.</param>
        /// <param name="operation">The operation.</param>
        private static void PerformSilently(Form operationForm, IWin32Window parentForm, IPlanOperation operation)
        {
            if (operation == null)
                return;

            // A window is required
            if (RequiresWindow(operation) && Perform(operationForm, parentForm) != DialogResult.OK)
                return;

            // Silent way
            operation.Perform();
        }

        /// <summary>
        /// Performs the action for the "Plan To N" and "Remove" menu options.
        /// </summary>
        /// <param name="operationForm">The operation form.</param>
        /// <param name="parentForm">The parent form.</param>
        /// <returns></returns>
        public static DialogResult Perform(Form operationForm, IWin32Window parentForm)
        {
            using (Form window = operationForm)
            {
                window.ShowDialog(parentForm);
                return window.DialogResult;
            }
        }

        /// <summary>
        /// Selects which type of Perform will be called according to user settings.
        /// </summary>
        /// <param name="operationForm">The operation form.</param>
        /// <param name="parentForm">The parent form.</param>
        /// <param name="operation">The operation.</param>
        public static void SelectPerform(Form operationForm, IWin32Window parentForm, IPlanOperation operation)
        {
            if (operation == null)
                return;

            if (Settings.UI.PlanWindow.UseAdvanceEntryAddition && operation.Type == PlanOperations.Addition)
            {
                Perform(operationForm, parentForm);
                return;
            }

            PerformSilently(operationForm, parentForm, operation);
        }
    }
}