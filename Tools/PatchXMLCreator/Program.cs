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

            Application.Run(new PatchXmlCreatorWindow());
        }

        /// <summary>
        /// Ensures that the prerequisites to run the application are met.
        /// </summary>
        private static void EnsurePrerequisites()
        {
            // Ensure that a release version of EVEMon and the installer file has been created
            string eveMonExecFilePath = Path.Combine(PatchXmlCreatorWindow.EVEMonExecDir, PatchXmlCreatorWindow.EVEMonExecFilename);
            string installerFile = String.Format(PatchXmlCreatorWindow.InstallerFilename, PatchXmlCreatorWindow.AssemblyVersion);
            string installerPath = String.Format("{1}{0}{2}", Path.DirectorySeparatorChar, PatchXmlCreatorWindow.InstallerDir,
                                                 installerFile);

            if (File.Exists(eveMonExecFilePath) && File.Exists(installerPath))
                return;

            string text = String.Empty;

            if (!File.Exists(eveMonExecFilePath))
            {
                text = String.Format(
                    "An EVEMon release version has to be created first\r\nbefore you can use {0}.", PatchXmlCreatorWindow.Caption);
            }

            if (!File.Exists(installerPath))
            {
                text = String.Format(
                    "An EVEMon installer file has to be created first\r\nbefore you can use {0}.", PatchXmlCreatorWindow.Caption);
            }

            MessageBox.Show(text, PatchXmlCreatorWindow.Caption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            s_exitRequested = true;
        }
    }
}