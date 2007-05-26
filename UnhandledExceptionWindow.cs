using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon
{
    public partial class UnhandledExceptionWindow : EVEMonForm
    {
        public UnhandledExceptionWindow()
        {
            InitializeComponent();
        }

        public UnhandledExceptionWindow(Exception err)
            : this()
        {
            m_exception = err;
        }

        private Exception m_exception;

        private void UnhandledExceptionWindow_Load(object sender, EventArgs e)
        {
            try
            {
                Assembly casm = Assembly.GetExecutingAssembly();
                using (Stream s = casm.GetManifestResourceStream("EVEMon.bee.jpg"))
                using (Image i = Image.FromStream(s))
                {
                    int oHeight = i.Height;
                    int oWidth = i.Width;
                    if (i.Height > pbBugImage.ClientSize.Height)
                    {
                        Double scale = Convert.ToDouble(pbBugImage.ClientSize.Height) / Convert.ToDouble(i.Height);
                        oHeight = Convert.ToInt32(oHeight * scale);
                        oWidth = Convert.ToInt32(oWidth * scale);
                        Bitmap b = new Bitmap(i, new Size(oWidth, oHeight));

                        int oRight = pbBugImage.Right;
                        pbBugImage.ClientSize = new Size(oWidth, oHeight);
                        pbBugImage.Image = b;
                        pbBugImage.Left = oRight - pbBugImage.Width;
                        pbBugImage.Top = (panel1.ClientSize.Height / 2) - (pbBugImage.Height / 2);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex, false);
            }

            try
            {
                String exceptionReport;
                OperatingSystem os = Environment.OSVersion;

                exceptionReport = "EVEMon Version: " + Application.ProductVersion;
                exceptionReport += Environment.NewLine;
                exceptionReport += ".NET Runtime Version: " + System.Environment.Version;
                exceptionReport += Environment.NewLine;
                exceptionReport += "Operating System: " + os.VersionString;
                exceptionReport += Environment.NewLine;
                exceptionReport += Environment.NewLine;
                exceptionReport += m_exception.ToString();

                textBox1.Text = exceptionReport;
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex, true);
                textBox1.Text = "Error retrieving error data. Wow, things are *really* screwed up!";
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void llCopy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.SetText(textBox1.Text, TextDataFormat.Text);
            MessageBox.Show("The error details have been copied to the clipboard.", "Copy", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show(
                "This will clear all your saved settings, including skill plans and " +
                "character logins. You should only try this if EVEMon has errored more " +
                "than once.\r\n\r\nClear settings?",
                "Clear Settings?",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);
            if (dr == DialogResult.Yes)
            {
                Program.Settings.NeverSave();
                Settings.Reset();
                MessageBox.Show("Your settings have been reset.",
                                "Settings Reset", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
