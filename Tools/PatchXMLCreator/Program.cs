using System;
using System.IO;
using System.Windows.Forms;

namespace PatchXmlCreator
{
    internal static class Program
    {
        private static bool s_exitRequested;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Ensure that the applications prerequisites are met
            EnsurePrerequisites();

            // When prerequisites are not met we exit before Run()
            if (s_exitRequested)
                return;

            bool newRelease = SelectAction();
            Application.Run(new PatchXmlCreatorWindow(newRelease));
        }

        /// <summary>
        /// Ensures that the prerequisites to run the application are met.
        /// </summary>
        private static void EnsurePrerequisites()
        {
            string text;
            string eveMonExecFilePath = Path.Combine(PatchXmlCreatorWindow.EVEMonExecDir, PatchXmlCreatorWindow.EVEMonExecFilename);

            // Ensure that a release version of EVEMon has been created
            if (!File.Exists(eveMonExecFilePath))
            {
                text = String.Format("An EVEMon release version has to be created first\r\nbefore you can use {0}.",
                                     PatchXmlCreatorWindow.Caption);

                ShowMessage(text);
                return;
            }

            string installerFile = String.Format(PatchXmlCreatorWindow.InstallerFilename, PatchXmlCreatorWindow.AssemblyVersion);
            string installerPath = String.Format("{1}{0}{2}", Path.DirectorySeparatorChar, PatchXmlCreatorWindow.InstallerDir,
                                                 installerFile);

            // Ensure that the installer file has been created
            if (File.Exists(installerPath))
                return;

            text = String.Format("An EVEMon installer file has to be created first\r\nbefore you can use {0}.",
                                 PatchXmlCreatorWindow.Caption);

            ShowMessage(text);
        }

        /// <summary>
        /// Shows the message.
        /// </summary>
        /// <param name="text">The text.</param>
        private static void ShowMessage(string text)
        {
            MessageBox.Show(text, PatchXmlCreatorWindow.Caption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            s_exitRequested = true;
        }

        /// <summary>
        /// Selects the action.
        /// </summary>
        /// <returns></returns>
        private static bool SelectAction()
        {
            DialogResult dialogResult = MessageBox.Show(
                "Create patch file for a new EVEMon release ?\r\n\r\nSelect 'No' if you are creating a patch file for new data files.",
                PatchXmlCreatorWindow.Caption, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            return dialogResult == DialogResult.Yes;
        }
    }
}