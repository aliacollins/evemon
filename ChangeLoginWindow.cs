using System;
using System.Windows.Forms;
using EVEMon.Common;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;

namespace EVEMon
{
    public partial class ChangeLoginWindow : EVEMonForm
    {
        public ChangeLoginWindow()
        {
            InitializeComponent();
        }

        private string m_charName = "this character";

        public string CharacterName
        {
            get { return m_charName; }
            set { m_charName = value; }
        }

        private int m_userId = 0;

        public int UserId
        {
            get { return m_userId; }
            set { m_userId = value; }
        }

        private string m_apiKey = String.Empty;

        public string ApiKey
        {
            get { return m_apiKey; }
        }

        public bool ShowInvalidKey
        {
            set
            {
            	this.Text = (value ? "Invalid Or Empty API Key" : "Change API Key");
            }
        }

        private void ChangeLoginWindow_Load(object sender, EventArgs e)
        {
            label1.Text = String.Format("Enter EVE Online API Credentials for {0}:", m_charName);
            if (m_userId >  0)
            {
                tbUserId.Text = Convert.ToString(m_userId);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            bool isOk = false;
            bool isLoginOk = false;
            BusyDialog b = new BusyDialog();
            Thread worker = new Thread(new ThreadStart(delegate
            {
                int uid = 0;
                try
                {
                    uid = Int32.Parse(tbUserId.Text);
                }
                catch (Exception) { }
                try
                {
                    EveSession s = EveSession.GetSession(uid, tbAuthKey.Text);
                    isLoginOk = true;
                    if (s.GetCharacterId(m_charName) > 0)
                    {
                        isOk = true;
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.LogException(ex, true);
                    isLoginOk = false;
                }
                b.Invoke(new MethodInvoker(b.Dispose));
                b = null;
            }));

            worker.Start();
            if (b != null && !b.IsDisposed)
                b.ShowDialog();
            worker.Join();
            if (isOk)
            {
                m_userId = 0;
                try
                {
                    m_userId = Int32.Parse(tbUserId.Text);
                }
                catch (Exception) { }
                m_apiKey = tbAuthKey.Text;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                if (!isLoginOk)
                {
                    MessageBox.Show("Invalid User Id or API key.", "Unable to Validate",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Could not find " + m_charName + " using that User Id and API Key.",
                                    "Could Not Find Character", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }

        private void tbUserId_TextChanged(object sender, EventArgs e)
        {
            CheckDisables();
        }

        private void tbAuthKey_TextChanged(object sender, EventArgs e)
        {
            CheckDisables();
        }

        private void CheckDisables()
        {
            btnOk.Enabled = (!String.IsNullOrEmpty(tbUserId.Text) &&
                             !String.IsNullOrEmpty(tbAuthKey.Text));
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Util.BrowserLinkClicked(EveSession.ApiKeyUrl);
        }
    }
}
