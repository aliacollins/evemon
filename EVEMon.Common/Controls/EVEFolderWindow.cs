using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EVEMon.Common.Controls
{
    public partial class EveFolderWindow : EVEMonForm
    {
        private readonly string[] m_defaultFolderLocation = EveMonClient.DefaultEvePortraitCacheFolders.ToArray();
        private string[] m_specifiedPortraitFolder = new[] { String.Empty };

        /// <summary>
        /// Initializes a new instance of the <see cref="EveFolderWindow"/> class.
        /// </summary>
        public EveFolderWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Load event of the EVEFolderWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EVEFolderWindow_Load(object sender, EventArgs e)
        {
            if (m_defaultFolderLocation == null)
            {
                SpecifyFolderRadioButton.Checked = true;
                DefaultFolderRadioButton.Enabled = false;
            }

            if (m_specifiedPortraitFolder == m_defaultFolderLocation)
            {
                DefaultFolderRadioButton.Checked = true;
                BrowseButton.Enabled = false;
            }
            else
            {
                SpecifyFolderRadioButton.Checked = true;
                BrowseButton.Enabled = true;
            }
        }

        /// <summary>
        /// Gets the EVE portrait cache folder.
        /// </summary>
        /// <value>The EVE portrait cache folder.</value>
        public IEnumerable<string> EVEPortraitCacheFolder
        {
            get { return m_specifiedPortraitFolder; }
        }

        /// <summary>
        /// Handles the Click event of the BrowseButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void BrowseButton_Click(object sender, EventArgs e)
        {
            DialogResult dr = OpenDirFolderBrowserDialog.ShowDialog();
            if (dr != DialogResult.OK)
                return;

            FilenameTextBox.Text = OpenDirFolderBrowserDialog.SelectedPath;
            m_specifiedPortraitFolder[0] = FilenameTextBox.Text;
            OKButton.Enabled = true;
        }

        /// <summary>
        /// Handles the Click event of the OKButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OKButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Handles the CheckedChanged event of the DefaultFolderRadioButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void DefaultFolderRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!DefaultFolderRadioButton.Checked)
                return;

            m_specifiedPortraitFolder = m_defaultFolderLocation;
            OKButton.Enabled = true;
        }

        /// <summary>
        /// Handles the CheckedChanged event of the SpecifyFolderRadioButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void SpecifyFolderRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            BrowseButton.Enabled = SpecifyFolderRadioButton.Checked;
            OKButton.Enabled = (SpecifyFolderRadioButton.Checked && FilenameTextBox.Text.Length != 0);
        }
    }
}