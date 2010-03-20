using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Runtime.InteropServices;

namespace EVEMon.Common.Controls
{
    public partial class StaticDataErrorForm : EVEMonForm
    {
        private readonly string m_errorMessage;
        private static StaticDataErrorForm m_errorForm;
        
        public StaticDataErrorForm()
        {
            InitializeComponent();
        }

        public StaticDataErrorForm(string errorMessage)
            : this()
        {
            m_errorMessage = errorMessage;
        }

        public static void ShowError(string errorMessage)
        {
            if (m_errorForm == null)
            {
                m_errorForm = new StaticDataErrorForm(errorMessage);
                m_errorForm.Show();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            m_errorForm = null;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            warningIcon.Image = SystemIcons.Exclamation.ToBitmap();
            SetErrorDetails();
            autoupdateDetailsGroupBox.Visible = !Settings.Updates.CheckEVEMonVersion;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            closeButton.Focus();
        }

        private void SetErrorDetails()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(m_errorMessage);
            sb.AppendLine();
            sb.AppendLine();
            sb.Append("Diagnostic information");
            sb.AppendLine();
            sb.Append("--------------------------------------------");
            sb.AppendLine();
            sb.AppendFormat(CultureConstants.DefaultCulture, "EVEMon Version {0}", Application.ProductVersion);
            sb.AppendLine();
            sb.AppendFormat(CultureConstants.DefaultCulture, "AutoUpdate {0}", Settings.Updates.CheckEVEMonVersion ? "Enabled" : "Disabled");
            sb.AppendLine();
            foreach (var datafile in EveClient.Datafiles)
            {
                sb.AppendFormat(CultureConstants.DefaultCulture, "{0}: {1}", datafile.Filename, datafile.MD5Sum);
                sb.AppendLine();
            }
            errorMessageTextBox.Text = sb.ToString();
        }

        private void copyToClipboardLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Clipboard.SetText(errorMessageTextBox.Text, TextDataFormat.Text);
                MessageBox.Show("The error details have been copied to the clipboard.", "Copy", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            catch (ExternalException ex)
            {
                // Occurs when another process is using the clipboard
                ExceptionHandler.LogException(ex, true);
                MessageBox.Show("Couldn't complete the operation, the clipboard is being used by another process.");
            }
        }

        private void enableAutoUpdateButton_Click(object sender, EventArgs e)
        {
            Settings.Updates.CheckEVEMonVersion = true;
            Close();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
