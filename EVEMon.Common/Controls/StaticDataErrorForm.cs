using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Runtime.InteropServices;

namespace EVEMon.Common.Controls
{
    public partial class StaticDataErrorForm : EVEMonForm
    {
        private readonly string _errorMessage;
        private static StaticDataErrorForm _errorForm;

        public StaticDataErrorForm() :this(string.Empty){}

        public StaticDataErrorForm(string errorMessage)
        {
            InitializeComponent();
            _errorMessage = errorMessage;
        }

        public static void ShowError(string errorMessage)
        {
            if (_errorForm == null)
            {
                _errorForm = new StaticDataErrorForm(errorMessage);
                _errorForm.Show();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _errorForm = null;
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
            sb.Append(_errorMessage);
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append("Diagnostic information");
            sb.Append(Environment.NewLine);
            sb.Append("--------------------------------------------");
            sb.Append(Environment.NewLine);
            sb.AppendFormat("EVEMon Version {0}", Application.ProductVersion);
            sb.Append(Environment.NewLine);
            sb.AppendFormat("AutoUpdate {0}", Settings.Updates.CheckEVEMonVersion ? "Enabled" : "Disabled");
            sb.Append(Environment.NewLine);
            foreach (var datafile in EveClient.Datafiles)
            {
                sb.AppendFormat("{0}: {1}", datafile.Filename, datafile.Sum);
                sb.Append(Environment.NewLine);
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

