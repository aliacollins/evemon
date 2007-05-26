using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon
{
    public partial class UpdateNotifyForm : EVEMonForm
    {
        public UpdateNotifyForm()
        {
            InitializeComponent();
        }

        private Settings m_settings;
        private UpdateAvailableEventArgs m_args;

        public UpdateNotifyForm(Settings settings, UpdateAvailableEventArgs args)
            : this()
        {
            m_args = args;
            m_settings = settings;
        }

        private void btnIgnore_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show(
                "Are you sure you want to ignore this update? You will not " +
                "be prompted again until a newer version is released.",
                "Ignore Update?",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);
            if (dr == DialogResult.No)
            {
                return;
            }
            m_settings.IgnoreUpdateVersion = m_args.NewestVersion.ToString();
            m_settings.Save();
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (cbAutoInstall.Enabled && cbAutoInstall.Checked)
            {
                Uri i = new Uri(m_args.AutoInstallUrl);
                string fn = Path.GetFileName(i.AbsolutePath);
                using (UpdateDownloadForm f = new UpdateDownloadForm(
                    m_args.AutoInstallUrl, fn))
                {
                    f.ShowDialog();
                    if (f.DialogResult == DialogResult.OK)
                    {
                        ExecPatcher(fn, m_args.AutoInstallArguments);
                    }
                }
            }
            else
            {
                Process.Start(m_args.UpdateUrl);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void ExecPatcher(string fn, string args)
        {
            try
            {
                Process.Start(fn, args);
            }
            catch (Exception e)
            {
                ExceptionHandler.LogRethrowException(e);
                if (File.Exists(fn))
                {
                    try
                    {
                        File.Delete(fn);
                    }
                    catch
                    {
                        ExceptionHandler.LogException(e, false);
                    }
                }
                throw;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnLater_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void UpdateNotifyForm_Shown(object sender, EventArgs e)
        {
            UpdateInformation();
            cbAutoInstall.Checked = m_args.CanAutoInstall;
            UpdateManager.GetInstance().UpdateAvailable += new UpdateAvailableHandler(UpdateNotifyForm_UpdateAvailable);
        }

        private void UpdateNotifyForm_UpdateAvailable(object sender, UpdateAvailableEventArgs e)
        {
            m_args = e;
            UpdateInformation();
        }

        private void UpdateInformation()
        {
            string updMessage = m_args.UpdateMessage;
            updMessage.Replace("\r", "");
            textBox1.Lines = updMessage.Split('\n');
            label1.Text =
                String.Format(
                    @"An EVEMon update is available.

Current version: {0}
Newest version: {1}

The newest version has the following updates:",
                    m_args.CurrentVersion, m_args.NewestVersion);

            cbAutoInstall.Enabled = m_args.CanAutoInstall;
        }

        private void UpdateNotifyForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            UpdateManager.GetInstance().UpdateAvailable -= new UpdateAvailableHandler(UpdateNotifyForm_UpdateAvailable);
        }
    }
}