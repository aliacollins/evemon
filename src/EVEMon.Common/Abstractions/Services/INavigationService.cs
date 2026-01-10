using System;
using EVEMon.Common.Models;

namespace EVEMon.Common.Abstractions.Services
{
    /// <summary>
    /// Service interface for navigation operations.
    /// Abstracts window/view navigation to allow for different UI frameworks.
    /// </summary>
    public interface INavigationService
    {
        #region Character Navigation

        /// <summary>
        /// Navigates to a character's details view.
        /// </summary>
        /// <param name="character">The character to show.</param>
        void NavigateToCharacter(Character character);

        /// <summary>
        /// Navigates to the skill planner for a character.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="plan">Optional plan to open.</param>
        void NavigateToSkillPlanner(Character character, Plan plan = null);

        #endregion

        #region Window Navigation

        /// <summary>
        /// Opens the settings window.
        /// </summary>
        void OpenSettings();

        /// <summary>
        /// Opens the about window.
        /// </summary>
        void OpenAbout();

        /// <summary>
        /// Opens the add character wizard.
        /// </summary>
        void OpenAddCharacter();

        /// <summary>
        /// Opens the plan editor window.
        /// </summary>
        /// <param name="plan">The plan to edit.</param>
        void OpenPlanEditor(Plan plan);

        /// <summary>
        /// Opens the skill browser.
        /// </summary>
        /// <param name="character">The character context.</param>
        void OpenSkillBrowser(Character character);

        /// <summary>
        /// Opens the ship browser.
        /// </summary>
        /// <param name="character">The character context.</param>
        void OpenShipBrowser(Character character);

        /// <summary>
        /// Opens the item browser.
        /// </summary>
        /// <param name="character">The character context.</param>
        void OpenItemBrowser(Character character);

        /// <summary>
        /// Opens the blueprint browser.
        /// </summary>
        /// <param name="character">The character context.</param>
        void OpenBlueprintBrowser(Character character);

        #endregion

        #region External Navigation

        /// <summary>
        /// Opens a URL in the default browser.
        /// </summary>
        /// <param name="url">The URL to open.</param>
        void OpenInBrowser(string url);

        /// <summary>
        /// Opens a URL in the default browser.
        /// </summary>
        /// <param name="uri">The URI to open.</param>
        void OpenInBrowser(Uri uri);

        #endregion

        #region Tab Navigation

        /// <summary>
        /// Navigates to a specific tab for the current character.
        /// </summary>
        /// <param name="tabName">The name of the tab.</param>
        void NavigateToTab(string tabName);

        #endregion
    }
}
