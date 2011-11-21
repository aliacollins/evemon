using System;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon.SkillPlanner
{
    /// <summary>
    /// Helper for the "Plan To" and "Remove" menus.
    /// </summary>
    public static class PlanHelper
    {
        /// <summary>
        /// Updates a regular "Plan to X" menu : text, tag, enable/disable.
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="plan"></param>
        /// <param name="skill"></param>
        /// <param name="level"></param>
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
        /// <param name="plan"></param>
        /// <param name="skill"></param>
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
        /// <param name="operation"></param>
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
        /// <param name="operation"></param>
        /// <returns></returns>
        private static void PerformSilently(IPlanOperation operation)
        {
            PlanWindow window = WindowsFactory.GetByTag<PlanWindow, Plan>(operation.Plan);
            PerformSilently(window, operation);
        }

        /// <summary>
        /// Performs the action for the "Plan To N" and "Remove" menu options, in a silent way whenever possible.
        /// </summary>
        /// <param name="parentForm"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        private static void PerformSilently(IWin32Window parentForm, IPlanOperation operation)
        {
            if (operation == null)
                return;

            // A window is required
            if (RequiresWindow(operation) && Perform(parentForm, operation) != DialogResult.OK)
                return;

            // Silent way
            operation.Perform();
        }

        /// <summary>
        /// Performs the action for the "Plan To N" and "Remove" menu options.
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        public static void Perform(IPlanOperation operation)
        {
            if (operation == null)
                throw new ArgumentNullException("operation");

            PlanWindow window = WindowsFactory.GetByTag<PlanWindow, Plan>(operation.Plan);
            Perform(window, operation);
        }

        /// <summary>
        /// Performs the action for the "Plan To N" and "Remove" menu options.
        /// </summary>
        /// <param name="parentForm"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        private static DialogResult Perform(IWin32Window parentForm, IPlanOperation operation)
        {
            using (PlanToOperationForm window = new PlanToOperationForm(operation))
            {
                window.ShowDialog(parentForm);
                return window.DialogResult;
            }
        }

        /// <summary>
        /// Selects which type of Perform will be called according to user settings.
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        public static void SelectPerform(IPlanOperation operation)
        {
            if (operation == null)
                throw new ArgumentNullException("operation");

            if (Settings.UI.PlanWindow.UseAdvanceEntryAddition && operation.Type == PlanOperations.Addition)
                Perform(operation);

            PerformSilently(operation);
        }
    }
}