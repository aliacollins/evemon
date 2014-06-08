using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;

namespace EVEMon.PatchXmlCreator
{
    internal static class Helper
    {
        private static Action s_action;
        private static MessageBoxCustom s_msgBox;

        /// <summary>
        /// Gets or sets a value indicating whether an application exit is requested.
        /// </summary>
        /// <value><c>true</c> if application exit is requested; otherwise, <c>false</c>.</value>
        internal static bool ExitRequested { get; private set; }

        /// <summary>
        /// Ensures that the prerequisites to run the application are met.
        /// </summary>
        internal static void EnsurePrerequisites()
        {
            string text;
            string eveMonExecFilePath = Path.Combine(PatchXmlCreatorWindow.EVEMonExecDir, PatchXmlCreatorWindow.EVEMonExecFilename);

            // Ensure that a release version of EVEMon has been created
            if (!File.Exists(eveMonExecFilePath))
            {
                text = String.Format(CultureConstants.DefaultCulture,
                                     "An EVEMon release version has to be created first\r\nbefore you can use {0}.",
                                     PatchXmlCreatorWindow.Caption);

                ShowMessage(text);
                return;
            }

            // Ensure that the installer file has been created
            if (PatchXmlCreatorWindow.GetInstallerPath().Exists)
                return;

            text = String.Format(CultureConstants.DefaultCulture,
                                 "An EVEMon installer file has to be created first\r\nbefore you can use {0}.",
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
            ExitRequested = true;
        }

        /// <summary>
        /// Selects the action.
        /// </summary>
        /// <returns></returns>
        internal static Action SelectAction()
        {
            s_msgBox = new MessageBoxCustom();
            s_msgBox.Button1.Click += OnButtonClick;
            s_msgBox.Button2.Click += OnButtonClick;
            s_msgBox.Button3.Click += OnButtonClick;
            s_msgBox.Text = String.Format(CultureConstants.DefaultCulture, "{0} - Action Selector", PatchXmlCreatorWindow.Caption);
            s_msgBox.Message.Text = @"Select an action for patch file creation.";
            s_msgBox.Button1.Text = @"Datafiles Only";
            s_msgBox.Button2.Text = @"Release Only";
            s_msgBox.Button3.Text = @"Release && Datafiles";
            s_msgBox.Button1.AutoSize = true;
            s_msgBox.Button2.AutoSize = true;
            s_msgBox.Button3.AutoSize = true;
            s_msgBox.PictureBox.Image = SystemIcons.Question.ToBitmap();
            s_msgBox.CheckBox.Visible = false;

            s_msgBox.ShowDialog();

            if (s_action == Action.None)
                ExitRequested = true;

            return s_action;
        }

        /// <summary>
        /// Called when a button is clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void OnButtonClick(object sender, EventArgs e)
        {
            ButtonBase button = sender as Button;

            if (button == null)
                return;

            s_action = GetAction(button.Text);
            s_msgBox.Close();
        }

        /// <summary>
        /// Returns action based on button text.
        /// </summary>
        /// <param name="buttonText">Text on selected button.</param>
        /// <returns>Corresponding <see cref="Action"/>.</returns>
        private static Action GetAction(string buttonText)
        {
            switch (buttonText)
            {
                case "Release && Datafiles":
                    return Action.ReleaseAndDatafiles;
                case "Release Only":
                    return Action.ReleaseOnly;
                case "Datafiles Only":
                    return Action.DatafilesOnly;
                default:
                    return Action.None;
            }
        }
    }
}
