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
            // Ensure that a release version of EVEMon has been created
            string eveMonExecFilePath = Path.Combine(PatchXmlCreatorWindow.EVEMonExecDir, PatchXmlCreatorWindow.EVEMonExecFilename);
            if (File.Exists(eveMonExecFilePath))
                return;

            string text = String.Format(
                "An EVEMon release version has to be created first\r\nbefore you can use {0}.", PatchXmlCreatorWindow.Caption);
            MessageBox.Show(text, PatchXmlCreatorWindow.Caption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            s_exitRequested = true;
        }
    }
}