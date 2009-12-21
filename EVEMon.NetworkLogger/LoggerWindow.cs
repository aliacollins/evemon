using System;
using System.Windows.Forms;
using EVEMon.Common;
using System.IO;

namespace EVEMon.NetworkLogger
{
    public partial class LoggerWindow : EVEMonForm
    {
        public LoggerWindow()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            m_fileName = tbFilename.Text;
            if (rbNoUsername.Checked)
            {
                m_loggingLevel = LoggingLevel.NoUsernameOrPassword;
            }
            else if (rbNoPassword.Checked)
            {
                m_loggingLevel = LoggingLevel.NoPassword;
            }
            else if (rbAll.Checked)
            {
                m_loggingLevel = LoggingLevel.All;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private LoggingLevel m_loggingLevel = LoggingLevel.NoUsernameOrPassword;
        private string m_fileName = String.Empty;

        public LoggingLevel LoggingLevel
        {
            get { return m_loggingLevel; }
        }

        public string FileName
        {
            get { return m_fileName; }
        }

        private void LoggerWindow_Load(object sender, EventArgs e)
        {
            tbFilename.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                              "EVEMon Network Log.txt");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sfdSaveLocation.FileName = tbFilename.Text;
            DialogResult dr = sfdSaveLocation.ShowDialog();
            if (dr == DialogResult.Cancel)
            {
                return;
            }

            tbFilename.Text = sfdSaveLocation.FileName;
        }
    }
}
