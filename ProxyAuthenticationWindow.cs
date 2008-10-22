using System;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Net;

namespace EVEMon
{
    public partial class ProxyAuthenticationWindow : EVEMonForm
    {
        public ProxyAuthenticationWindow()
        {
            InitializeComponent();
        }

        private ProxySetting m_proxySetting = new ProxySetting();

        public ProxySetting ProxySetting
        {
            get { return m_proxySetting; }
            set
            {
                m_proxySetting = value;
                UpdateFields();
            }
        }

        private void UpdateFields()
        {
            if (m_proxySetting != null)
            {
                switch (m_proxySetting.AuthType)
                {
                    case ProxyAuthType.None:
                        rbNoAuth.Checked = true;
                        break;
                    case ProxyAuthType.SystemDefault:
                        rbSystemDefault.Checked = true;
                        break;
                    case ProxyAuthType.Specified:
                        tbUsername.Text = m_proxySetting.Username;
                        tbPassword.Text = m_proxySetting.Password;
                        rbSuppliedAuth.Checked = true;
                        break;
                }
            }
        }

        private void rbSuppliedAuth_CheckedChanged(object sender, EventArgs e)
        {
            tlpSpecifiedAuth.Enabled = rbSuppliedAuth.Checked;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (rbNoAuth.Checked)
            {
                m_proxySetting.AuthType = ProxyAuthType.None;
            }
            else if (rbSystemDefault.Checked)
            {
                m_proxySetting.AuthType = ProxyAuthType.SystemDefault;
            }
            else if (rbSuppliedAuth.Checked)
            {
                m_proxySetting.AuthType = ProxyAuthType.Specified;
                m_proxySetting.Username = tbUsername.Text;
                m_proxySetting.Password = tbPassword.Text;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}