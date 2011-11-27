using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.BlankCharacter
{
    public partial class BlankCharacterWindow : EVEMonForm
    {
        private string m_filename;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlankCharacterWindow"/> class.
        /// </summary>
        public BlankCharacterWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Load event of the BlankCharacterWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void BlankCharacterWindow_Load(object sender, EventArgs e)
        {
            EveMonClient.TimerTick += EveMonClient_TimerTick;
            Disposed += OnDisposed;

            buttonOK.Text = "Save";
            buttonOK.Enabled = false;
        }


        #region Event Handlers

        /// <summary>
        /// Handles the TimerTick event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EveMonClient_TimerTick(object sender, EventArgs e)
        {
            buttonOK.Enabled = !String.IsNullOrEmpty(blankCharacterControl.CharacterName);
        }

        /// <summary>
        /// Called when [disposed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnDisposed(object sender, EventArgs e)
        {
            EveMonClient.TimerTick -= EveMonClient_TimerTick;
            Disposed -= OnDisposed;
        }

        #endregion


        #region Control Handlers

        /// <summary>
        /// Handles the Click event of the buttonOK control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            // Three choices for one button
            switch (buttonOK.DialogResult)
            {
                    // Save blank character
                case DialogResult.None:
                    Save(blankCharacterControl.CreateCharacter());
                    break;
                    // Add blank character
                case DialogResult.Yes:
                    AddBlankCharacter();
                    break;
                    // Close window
                case DialogResult.OK:
                    Close();
                    break;
            }
        }

        /// <summary>
        /// Handles the Click event of the buttonCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Saves the blank character.
        /// </summary>
        /// <param name="serial">The serial.</param>
        private void Save(ISerializableCharacterIdentity serial)
        {
            using (SaveFileDialog fileDialog = new SaveFileDialog())
            {
                fileDialog.Title = "Save Blank Character";
                fileDialog.Filter = "Blank Character CCPXML (*.xml) | *.xml";
                fileDialog.FileName = String.Format(CultureConstants.DefaultCulture, "{0}.xml", serial.Name);
                fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

                if (fileDialog.ShowDialog() != DialogResult.OK)
                    return;

                // Disabling control edit ability
                blankCharacterControl.Enabled = false;

                XmlDocument xmlDoc = (XmlDocument)Util.SerializeToXmlDocument(typeof(SerializableCCPCharacter), serial);
                string content = Util.GetXMLStringRepresentation(xmlDoc);
                FileHelper.OverwriteOrWarnTheUser(fileDialog.FileName,
                                                  fs =>
                                                      {
                                                          using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8))
                                                          {
                                                              writer.Write(content);
                                                              writer.Flush();
                                                              fs.Flush();
                                                          }
                                                          return true;
                                                      });

                m_filename = fileDialog.FileName;
                buttonOK.DialogResult = DialogResult.Yes;
                buttonOK.Text = "Import";
            }
        }

        /// <summary>
        /// Adds the blank character.
        /// </summary>
        private void AddBlankCharacter()
        {
            // Add blank character
            GlobalCharacterCollection.TryAddOrUpdateFromUriAsync(new Uri(m_filename),
                                                                 (send, args) =>
                                                                     {
                                                                         if (args == null || args.HasError)
                                                                             return;

                                                                         UriCharacter character = args.CreateCharacter();
                                                                         character.Monitored = true;
                                                                         
                                                                         buttonOK.Text = "Close";
                                                                         buttonOK.DialogResult = DialogResult.OK;
                                                                     });
        }

        #endregion
    }
}