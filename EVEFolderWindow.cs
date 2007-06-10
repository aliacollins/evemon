using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace EVEMon
{
    public partial class EVEFolderWindow : Form
    {
        public EVEFolderWindow()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            fbdOpenDir.SelectedPath = m_EVEFolder;
            DialogResult dr = fbdOpenDir.ShowDialog();
            if (dr == DialogResult.OK)
            {
                // Test to see if EVE executable exists
                FileInfo eveEXE = new FileInfo(Path.Combine(fbdOpenDir.SelectedPath, "eve.exe"));

                if (eveEXE.Exists)
                {
                    // if it exists we are good to go, update the text box and let the user click OK
                    tbFilename.Text = fbdOpenDir.SelectedPath;
                    btnOk.Enabled = true;
                }
                else
                {
                    // if not let the user know and leave the text box unchanged
                    MessageBox.Show("EVE Executable (eve.exe) not found in " + fbdOpenDir.SelectedPath + " please select a valid EVE installation folder.", "Invalid Folder", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }

                //                FileOk = true;
                //
                //                //anders - find the real character.  This probably burns too much time to stay here, but I like it
                //                XmlDocument xdoc = new XmlDocument();
                //                xdoc.Load(ofdOpenXml.FileName);
                //                XmlElement cElement = SerializableCharacterInfo.FindCharacterElement(xdoc);
                //                tbFileCharName.Text = cElement.Attributes["name"].Value;
            }

        }

        private string m_EVEFolder;

        public string EVEFolder
        {
            get { return m_EVEFolder; }
            set
            {
                m_EVEFolder = value;
                tbFilename.Text = value;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            m_EVEFolder = tbFilename.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }


    }
}
