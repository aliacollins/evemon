using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;
using System.Security.Cryptography;

namespace EVEMon
{
    /// <summary>
    /// Form to handle the display of the error report for easy bug reporting
    /// </summary>
    public partial class UnhandledExceptionWindow : EVEMonForm
    {
        public UnhandledExceptionWindow()
        {
            InitializeComponent();
            this.WhatCanYouDoLabel.Font = FontFactory.GetFont("Tahoma", 10F);
        }

        public UnhandledExceptionWindow(Exception err)
            : this()
        {
            m_exception = err;
        }

        private Exception m_exception;

        /// <summary>
        /// Generates a MD5 sum of a filename
        /// </summary>
        /// <param name="filename">Fully qualified path of the file</param>
        /// <returns>String containing the MD5 hash of the file</returns>
        private string QuickMD5(string filename)
        {
            MD5 md5Hasher = MD5.Create();
            StringBuilder hashString = new StringBuilder();
            byte[] hash;

            using (Stream fileStream = new FileStream(filename, FileMode.Open))
            {
                using (Stream bufferedStream = new BufferedStream(fileStream, 1200000))
                {
                    hash = md5Hasher.ComputeHash(bufferedStream);
                    foreach (byte b in hash)
                    {
                        hashString.Append(b.ToString("x2").ToLower());
                    }
                }
            }

            return hashString.ToString();
        }

        /// <summary>
        /// Loads resources, generates the report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnhandledExceptionWindow_Load(object sender, EventArgs e)
        {
            try
            {
                var i = Properties.Resources.Bug;

                int oHeight = i.Height;
                int oWidth = i.Width;
                if (i.Height > BugPictureBox.ClientSize.Height)
                {
                    Double scale = Convert.ToDouble(BugPictureBox.ClientSize.Height) / Convert.ToDouble(i.Height);
                    oHeight = Convert.ToInt32(oHeight * scale);
                    oWidth = Convert.ToInt32(oWidth * scale);
                    Bitmap b = new Bitmap(i, new Size(oWidth, oHeight));

                    int oRight = BugPictureBox.Right;
                    BugPictureBox.ClientSize = new Size(oWidth, oHeight);
                    BugPictureBox.Image = b;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex, false);
            }

            string trace;
            EveClient.StopTraceLogging();
            
            try
            {
                using (FileStream traceStream = new FileStream(EveClient.TraceFileName, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader traceReader = new StreamReader(traceStream))
                    {
                        trace = traceReader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex, false);
                trace = "Unable to load trace file for inclusion in report";
            }

            try
            {
                StringBuilder exceptionReport = new StringBuilder();
                OperatingSystem os = Environment.OSVersion;

                exceptionReport.AppendLine(String.Format("EVEMon Version: {0}", Application.ProductVersion));
                exceptionReport.AppendLine(String.Format(".NET Runtime Version: {0}", System.Environment.Version));
                exceptionReport.AppendLine(String.Format("Operating System: {0}", os.VersionString));
                exceptionReport.AppendLine(String.Format("Executable Path: {0}", System.Environment.CommandLine));
                exceptionReport.AppendLine();
                exceptionReport.AppendLine(RecursiveStackTrace);
                exceptionReport.AppendLine();
                exceptionReport.AppendLine(DatafileReport);
                exceptionReport.AppendLine();
                exceptionReport.AppendLine("Diagnostic Log:");
                exceptionReport.AppendLine(trace.Trim());

                TechnicalDetailsTextBox.Text = exceptionReport.ToString();
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex, true);
                TechnicalDetailsTextBox.Text = "Error retrieving error data. Wow, things are *really* screwed up!";
            }
        }

        private string DatafileReport
        {
            get
            {
                StringBuilder datafileReport = new StringBuilder();

                try
                {
                    string[] datafiles = Directory.GetFiles(EveClient.EVEMonDataDir, "*.gz", SearchOption.TopDirectoryOnly);

                    datafileReport.AppendLine("Datafile report");

                    foreach (string datafile in datafiles)
                    {
                        FileInfo info = new FileInfo(datafile);
                        datafileReport.AppendFormat("  {0} ({1}KiB - {2})", info.Name, info.Length / 1024, QuickMD5(datafile));
                        datafileReport.AppendLine();
                    }
                }
                catch
                {
                    datafileReport.AppendLine("Unable to create datafile report");
                }
                return datafileReport.ToString();
            }
        }

        private string RecursiveStackTrace
        {
            get
            {
                StringBuilder stackTraceBuilder = new StringBuilder();
                Exception ex = m_exception;

                stackTraceBuilder.AppendLine(ex.ToString());

                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;

                    stackTraceBuilder.AppendLine();
                    stackTraceBuilder.AppendLine(ex.ToString());
                }

                stackTraceBuilder = stackTraceBuilder.Replace("D:\\EVEMon\\", "");
                return stackTraceBuilder.ToString();
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void CopyDetailsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Clipboard.SetText(TechnicalDetailsTextBox.Text, TextDataFormat.Text);
                MessageBox.Show("The error details have been copied to the clipboard.", "Copy", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            catch (ExternalException ex)
            {
                // Occurs when another process is using the clipboard
                ExceptionHandler.LogException(ex, true);
                MessageBox.Show("Couldn't complete the operation, the clipboard is being used by another process. Wait a few moments and try again.");
            }
        }

        private void ResetButtonLinkLabel_Click(object sender, EventArgs e)
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
                Settings.Reset();
                MessageBox.Show("Your settings have been reset.",
                                "Settings Reset", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void llblReport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Util.OpenURL(NetworkConstants.EVEMonBugReport);
        }

        private void llblKnownProblems_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Util.OpenURL(NetworkConstants.EVEMonKnownProblems);
        }

        private void llblLatestBinaries_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Util.OpenURL(NetworkConstants.EVEMonMainPage);
        }
    }
}
