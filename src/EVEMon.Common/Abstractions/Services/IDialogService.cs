using System.Threading.Tasks;

namespace EVEMon.Common.Abstractions.Services
{
    /// <summary>
    /// Service interface for dialog operations.
    /// Abstracts dialog display to allow for different UI frameworks (WinForms, Avalonia).
    /// </summary>
    public interface IDialogService
    {
        #region Message Dialogs

        /// <summary>
        /// Shows an information message to the user.
        /// </summary>
        /// <param name="title">The dialog title.</param>
        /// <param name="message">The message to display.</param>
        void ShowInformation(string title, string message);

        /// <summary>
        /// Shows a warning message to the user.
        /// </summary>
        /// <param name="title">The dialog title.</param>
        /// <param name="message">The message to display.</param>
        void ShowWarning(string title, string message);

        /// <summary>
        /// Shows an error message to the user.
        /// </summary>
        /// <param name="title">The dialog title.</param>
        /// <param name="message">The message to display.</param>
        void ShowError(string title, string message);

        /// <summary>
        /// Shows a confirmation dialog and returns the user's choice.
        /// </summary>
        /// <param name="title">The dialog title.</param>
        /// <param name="message">The message to display.</param>
        /// <returns>True if the user confirmed, false otherwise.</returns>
        bool ShowConfirmation(string title, string message);

        /// <summary>
        /// Shows a confirmation dialog with async support.
        /// </summary>
        /// <param name="title">The dialog title.</param>
        /// <param name="message">The message to display.</param>
        /// <returns>True if the user confirmed, false otherwise.</returns>
        Task<bool> ShowConfirmationAsync(string title, string message);

        #endregion

        #region File Dialogs

        /// <summary>
        /// Shows an open file dialog.
        /// </summary>
        /// <param name="title">The dialog title.</param>
        /// <param name="filter">The file filter (e.g., "Text files (*.txt)|*.txt").</param>
        /// <param name="initialDirectory">The initial directory, or null for default.</param>
        /// <returns>The selected file path, or null if cancelled.</returns>
        string ShowOpenFileDialog(string title, string filter, string initialDirectory = null);

        /// <summary>
        /// Shows an open file dialog with async support.
        /// </summary>
        Task<string> ShowOpenFileDialogAsync(string title, string filter, string initialDirectory = null);

        /// <summary>
        /// Shows a save file dialog.
        /// </summary>
        /// <param name="title">The dialog title.</param>
        /// <param name="filter">The file filter.</param>
        /// <param name="defaultFileName">The default file name.</param>
        /// <param name="initialDirectory">The initial directory, or null for default.</param>
        /// <returns>The selected file path, or null if cancelled.</returns>
        string ShowSaveFileDialog(string title, string filter, string defaultFileName = null, string initialDirectory = null);

        /// <summary>
        /// Shows a save file dialog with async support.
        /// </summary>
        Task<string> ShowSaveFileDialogAsync(string title, string filter, string defaultFileName = null, string initialDirectory = null);

        /// <summary>
        /// Shows a folder browser dialog.
        /// </summary>
        /// <param name="title">The dialog title.</param>
        /// <param name="initialDirectory">The initial directory, or null for default.</param>
        /// <returns>The selected folder path, or null if cancelled.</returns>
        string ShowFolderBrowserDialog(string title, string initialDirectory = null);

        /// <summary>
        /// Shows a folder browser dialog with async support.
        /// </summary>
        Task<string> ShowFolderBrowserDialogAsync(string title, string initialDirectory = null);

        #endregion

        #region Input Dialogs

        /// <summary>
        /// Shows an input dialog to get text from the user.
        /// </summary>
        /// <param name="title">The dialog title.</param>
        /// <param name="prompt">The prompt message.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The entered text, or null if cancelled.</returns>
        string ShowInputDialog(string title, string prompt, string defaultValue = null);

        /// <summary>
        /// Shows an input dialog with async support.
        /// </summary>
        Task<string> ShowInputDialogAsync(string title, string prompt, string defaultValue = null);

        #endregion
    }
}
