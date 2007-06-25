using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using EVEMon.Common;

namespace EVEMon
{
    public partial class LoginCharSelect : EVEMonForm
    {
        public LoginCharSelect()
        {
            InitializeComponent();
        }

        private void tbUsername_TextChanged(object sender, EventArgs e)
        {
            SetNoCharacter();
        }

        private bool m_charOk = false;
        private bool m_fileOk = false;

        private bool CharOk
        {
            set
            {
                m_charOk = value;
                CheckValidation();
            }
        }

        private bool FileOk
        {
            set
            {
                m_fileOk = value;
                CheckValidation();
            }
        }

        private void CheckValidation()
        {
            switch (cbCharacterType.SelectedIndex)
            {
                case 0:
                    btnOk.Enabled = m_charOk;
                    break;
                case 1:
                    btnOk.Enabled = m_fileOk;
                    break;
                default:
                    btnOk.Enabled = false;
                    break;
            }
        }

        private void SetNoCharacter()
        {
            tbCharName.Text = "(None)";
            CharOk = false;
        }

        private void tbApiKey_TextChanged(object sender, EventArgs e)
        {
            SetNoCharacter();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            switch (cbCharacterType.SelectedIndex)
            {
                case 0:
                    m_isLogin = true;
                    m_isFile = false;
                    m_userid = tbUserId.Text;
                    m_apiKey = tbAuthKey.Text;
                    m_characterName = tbCharName.Text;
                    break;
                case 1:
                    m_isLogin = false;
                    m_isFile = true;
                    m_fileName = tbFilename.Text;
                    m_monitorFile = cbMonitorFile.Checked;
                    break;
                default:
                    return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private bool m_isLogin;
        private string m_userid;
        private string m_apiKey;
        private string m_characterName;
        private bool m_isFile;
        private string m_fileName;
        private bool m_monitorFile;

        public bool IsLogin
        {
            get { return m_isLogin; }
        }

        public string UserId
        {
            get { return m_userid; }
        }

        public string ApiKey
        {
            get { return m_apiKey; }
        }

        public string CharacterName
        {
            get { return m_characterName; }
        }

        public bool IsFile
        {
            get { return m_isFile; }
        }

        public string FileName
        {
            get { return m_fileName; }
        }

        public bool MonitorFile
        {
            get { return m_monitorFile; }
        }

        private void btnCharSelect_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(tbUserId.Text) || String.IsNullOrEmpty(tbAuthKey.Text))
            {
                SetNoCharacter();
                MessageBox.Show("Enter your API information first.",
                                "Credentials Required", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            string errm = string.Empty;
            EveSession s = null;
            using (BusyDialog.GetScope())
            {
                int userId = 0;
                try 
                {
                    userId = Int32.Parse(tbUserId.Text);
                }
                catch (Exception) { }
                s = EveSession.GetSession(userId, tbAuthKey.Text, out errm);

                if (s == null)
                {
                    SetNoCharacter();
                    MessageBox.Show(errm, "Invalid API Credentials",
                                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                List<Pair<string, int>> chars = s.GetCharacterListUncached();
                if (chars.Count == 0)
                {
                    SetNoCharacter();
                    MessageBox.Show("No characters were found on with those API Credentials.",
                                    "No Characters Found. This may be because CCP have disabled the API.",
                                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
            }

            using (CharSelect f = new CharSelect(s))
            {
                f.ShowDialog();
                if (f.DialogResult == DialogResult.OK)
                {
                    tbCharName.Text = f.Result;
                    CharOk = true;
                }
            }
        }

        private void LoginCharSelect_Load(object sender, EventArgs e)
        {
            cbCharacterType.SelectedIndex = 0;
        }

        private void cbCharacterType_SelectedIndexChanged(object sender, EventArgs e)
        {
            gbEveLogin.Visible = (cbCharacterType.SelectedIndex == 0);
            gbSavedXML.Visible = (cbCharacterType.SelectedIndex == 1);
            CheckValidation();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            DialogResult dr = ofdOpenXml.ShowDialog();
            if (dr == DialogResult.OK)
            {
                tbFilename.Text = ofdOpenXml.FileName;
                FileOk = true;

                //anders - find the real character.  This probably burns too much time to stay here, but I like it
                XmlDocument xdoc = new XmlDocument();
                xdoc.Load(ofdOpenXml.FileName);
                XmlElement cElement = xdoc.DocumentElement.SelectSingleNode("//result/name") as XmlElement;
                if (cElement != null)
                {
                    // new style API xml
                    tbFileCharName.Text = cElement.InnerText;
                }
                else
                {
                    cElement = SerializableCharacterInfo.FindCharacterElement(xdoc);
                    if (cElement != null)
                    {
                        // old style xml
                        tbFileCharName.Text = cElement.Attributes["name"].Value;
                    }
                    else
                    {
                        tbFileCharName.Text = "Invalid XML File";
                    }
                }
            }
        }

        private void lnkAPI_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(EveSession.ApiKeyUrl);
        }
    }
}
