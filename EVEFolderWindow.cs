using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using EVEMon.Common;

namespace EVEMon
{
    public partial class EVEFolderWindow : Form
    {
        private string DefaultFolderLocation;

        public EVEFolderWindow()
        {
            InitializeComponent();

            DefaultFolderLocation = LocalFileSystem.PortraitCacheFolder;

            if (DefaultFolderLocation == null)
            {
                SpecifyFolderRadioButton.Checked = true;
                DefaultFolderRadioButton.Enabled = false;
            }
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            OpenDirFolderBrowserDialog.SelectedPath = m_portraitFolder;
            DialogResult dr = OpenDirFolderBrowserDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                FilenameTextBox.Text = OpenDirFolderBrowserDialog.SelectedPath;
                OKButton.Enabled = true;
            }
        }

        private string m_portraitFolder;

        public string EVEFolder
        {
            get { return m_portraitFolder; }
            set
            {
                m_portraitFolder = value;
                FilenameTextBox.Text = value;
            }
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            if (SpecifyFolderRadioButton.Checked)
            {
                m_portraitFolder = FilenameTextBox.Text;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void DefaultFolderRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (DefaultFolderRadioButton.Checked)
            {
                OKButton.Enabled = true;
                m_portraitFolder = DefaultFolderLocation;
            }
        }

        private void SpecifyFolderRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            BrowseButton.Enabled = SpecifyFolderRadioButton.Checked;

            if (SpecifyFolderRadioButton.Checked)
            {
                OKButton.Enabled = (FilenameTextBox.Text != "");
            }
        }

        private void EVEFolderWindow_Load(object sender, EventArgs e)
        {
            if (m_portraitFolder == DefaultFolderLocation)
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
    }
}
