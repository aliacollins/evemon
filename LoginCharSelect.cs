using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using EVEMon.Common;
using System.Threading;
using System.IO;

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

        private List<string> m_charsToAdd = new List<string>();
        public List<string> CharsToAdd
        {
            get { return m_charsToAdd; }

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
                    if (tbCharName.Text != "(All Characters)")
                    {
                        m_charsToAdd.Clear();
                        m_charsToAdd.Add(tbCharName.Text);
                    }
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

        private void btnAddExisting_Click(object sender, EventArgs e)
        {
            using (CharSelect f = new CharSelect(m_unmonitoredChars))
            {
                f.ShowDialog();
                if (f.DialogResult == DialogResult.OK)
                {
                    tbCharName.Text = f.Result;
                    m_charsToAdd = m_unmonitoredChars; // to make the "all characters" work
                    CharOk = true;
                }
            }
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

            BusyDialog b = new BusyDialog();
            Thread worker = new Thread(new ThreadStart(delegate
            {
                int userId = 0;
                try
                {
                    userId = Int32.Parse(tbUserId.Text);
                }
                catch (Exception) { }
                s = EveSession.GetSession(userId, tbAuthKey.Text, out errm);
                List<Pair<string, int>> chars = new List<Pair<string, int>>();
                if (s != null)
                {
                    try
                    {
                        chars = s.GetCharacterListUncached();
                    }
                    catch (InvalidDataException)
                    {
                        //this should get handled by the next block
                    }
                    if (chars.Count == 0)
                    {
                        SetNoCharacter();
                        MessageBox.Show("No characters were found on with those API Credentials. This may be because CCP have disabled the API.",
                                        "No Characters Found.",
                                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        m_charsToAdd.Clear();
                        b.Invoke(new MethodInvoker(b.Dispose));
                        b = null;
                        return;
                    }

                    // now see if there aer any new characters to add
                    Settings settings = Settings.GetInstance();
                    List<string> onlineChars = new List<string>();
                    foreach (CharLoginInfo cli in settings.CharacterList)
                    {
                        onlineChars.Add(cli.CharacterName);
                    }

                    m_charsToAdd.Clear();
                    foreach (Pair<string, int> chr in chars)
                    {
                        if (!onlineChars.Contains(chr.A))
                        {
                            m_charsToAdd.Add(chr.A);
                        }
                    }
                }
                b.Invoke(new MethodInvoker(b.Dispose));
                b = null;
            }));

            worker.Start();
            if (b != null && !b.IsDisposed)
                b.ShowDialog();
            worker.Join();


            if (s == null)
            {
                //if (EVEMonWebRequest.LastException != null)
                //{
                //    errm += " (" + EVEMonWebRequest.LastException.Message + ")";
                //}
                SetNoCharacter();
                MessageBox.Show(errm, "An error occurred retrieving the character list",
                                MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            if (m_charsToAdd.Count == 0)
            {
                SetNoCharacter();
                MessageBox.Show("No new characters were found on the account with those API Credentials.",
                                "No New Characters Found.",
                                MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }


            using (CharSelect f = new CharSelect(m_charsToAdd))
            {
                f.ShowDialog();
                if (f.DialogResult == DialogResult.OK)
                {
                    tbCharName.Text = f.Result;
                    CharOk = true;
                }
            }
        }

        private List<string> m_unmonitoredChars;

        private void LoginCharSelect_Load(object sender, EventArgs e)
        {
            cbCharacterType.SelectedIndex = 0;
            FindUnmonitoredAccounts();
            lbUnmonitored.Enabled = btnAddExisting.Enabled = m_unmonitoredChars.Count > 0;
        }

        private void FindUnmonitoredAccounts()
        {
            // find any unmonitored accounts
            Settings m_settings = Settings.GetInstance();
            m_unmonitoredChars = new List<string>();
            foreach (AccountDetails acc in m_settings.Accounts)
            {
                foreach (string charname in acc.GetCharacterNames())
                {
                    if (!m_settings.LogonCharExists(charname))
                    {
                        m_unmonitoredChars.Add(charname);
                    }
                }
            }
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
                try
                {
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

                  tbFilename.Text = ofdOpenXml.FileName;
                  FileOk = true;
                }
                catch (XmlException)
                {
                  // show yes/no dialog asking them to load new file
                  DialogResult result = MessageBox.Show(this,
                                                        "The file that you attempted to load is not recognised.\n\nWould you like to try a different file?",
                                                        "Invalid XML file",
                                                        MessageBoxButtons.YesNo,
                                                        MessageBoxIcon.Error);

                  if (result == DialogResult.Yes)
                  {
                    btnBrowse_Click(sender, e);
                  }
                }
            }
        }

        private void lnkAPI_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            EveSession.BrowserLinkClicked(EveSession.ApiKeyUrl);
        }

    }
}
