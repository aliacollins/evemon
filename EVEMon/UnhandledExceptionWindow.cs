using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;
using CommonProperties = EVEMon.Common.Properties;

namespace EVEMon
{
    /// <summary>
    /// Form to handle the display of the error report for easy bug reporting
    /// </summary>
    public partial class UnhandledExceptionWindow : EVEMonForm
    {
        private readonly Exception m_exception;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnhandledExceptionWindow"/> class.
        /// </summary>
        private UnhandledExceptionWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnhandledExceptionWindow"/> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public UnhandledExceptionWindow(Exception exception)
            : this()
        {
            m_exception = exception;
        }

        /// <summary>
        /// Loads resources, generates the report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnhandledExceptionWindow_Load(object sender, EventArgs e)
        {
            WhatCanYouDoLabel.Font = FontFactory.GetFont("Tahoma", 10F);

            try
            {
                Bitmap bug = CommonProperties.Resources.Bug;

                int oHeight = bug.Height;
                int oWidth = bug.Width;
                if (bug.Height > BugPictureBox.ClientSize.Height)
                {
                    double scale = (double)(BugPictureBox.ClientSize.Height) / bug.Height;
                    oHeight = (int)(oHeight * scale);
                    oWidth = (int)(oWidth * scale);
                    BugPictureBox.Image = new Bitmap(bug, new Size(oWidth, oHeight));
                    BugPictureBox.ClientSize = new Size(oWidth, oHeight);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogRethrowException(ex);
                throw;
            }

            string trace;
            EveMonClient.StopTraceLogging();

            FileStream traceStream = null;
            try
            {
                traceStream = Util.GetFileStream(EveMonClient.TraceFileNameFullPath, FileMode.Open, FileAccess.Read);

                using (StreamReader traceReader = new StreamReader(traceStream))
                {
                    traceStream = null;
                    trace = traceReader.ReadToEnd();
                }
            }
            catch (IOException ex)
            {
                ExceptionHandler.LogException(ex, true);
                trace = "Unable to load trace file for inclusion in report";
            }
            catch (UnauthorizedAccessException ex)
            {
                ExceptionHandler.LogException(ex, true);
                trace = "Unable to load trace file for inclusion in report";
            }
            finally
            {
                if (traceStream != null)
                    traceStream.Dispose();
            }

            try
            {
                StringBuilder exceptionReport = new StringBuilder();
                OperatingSystem os = Environment.OSVersion;

                exceptionReport.AppendFormat(CultureConstants.DefaultCulture, "EVEMon Version: {0}{1}", Application.ProductVersion,
                                             Environment.NewLine);
                exceptionReport.AppendFormat(CultureConstants.DefaultCulture, ".NET Runtime Version: {0}{1}", Environment.Version,
                                             Environment.NewLine);
                exceptionReport.AppendFormat(CultureConstants.DefaultCulture, "Operating System: {0}{1}", os.VersionString,
                                             Environment.NewLine);
                exceptionReport.AppendFormat(CultureConstants.DefaultCulture, "Executable Path: {0}{1}", Environment.CommandLine,
                                             Environment.NewLine);
                exceptionReport.AppendLine();
                exceptionReport.Append(RecursiveStackTrace).AppendLine();
                exceptionReport.AppendLine();
                exceptionReport.Append(DatafileReport).AppendLine();
                exceptionReport.AppendLine();
                exceptionReport.Append("Diagnostic Log:").AppendLine();
                exceptionReport.Append(trace.Trim()).AppendLine();

                TechnicalDetailsTextBox.Text = exceptionReport.ToString();
            }
            catch (InvalidOperationException ex)
            {
                ExceptionHandler.LogException(ex, true);
                TechnicalDetailsTextBox.Text = "Error retrieving error data. Wow, things are *really* screwed up!";
            }
        }

        /// <summary>
        /// Gets the datafile report.
        /// </summary>
        /// <value>The datafile report.</value>
        private static string DatafileReport
        {
            get
            {
                StringBuilder datafileReport = new StringBuilder();

                try
                {
                    string[] datafiles = Directory.GetFiles(EveMonClient.EVEMonDataDir, "*.gz", SearchOption.TopDirectoryOnly);

                    datafileReport.AppendLine("Datafile report:");

                    foreach (string datafile in datafiles)
                    {
                        FileInfo info = new FileInfo(datafile);
                        Datafile file = new Datafile(Path.GetFileName(datafile));

                        datafileReport.AppendFormat(CultureConstants.DefaultCulture, "  {0} ({1}KiB - {2}){3}", info.Name,
                                                    info.Length / 1024, file.MD5Sum, Environment.NewLine);
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    ExceptionHandler.LogException(ex, true);
                    datafileReport.Append("Unable to create datafile report").AppendLine();
                }
                return datafileReport.ToString();
            }
        }

        /// <summary>
        /// Gets the recursive stack trace.
        /// </summary>
        /// <value>The recursive stack trace.</value>
        private string RecursiveStackTrace
        {
            get
            {
                StringBuilder stackTraceBuilder = new StringBuilder();
                Exception ex = m_exception;

                stackTraceBuilder.Append(ex.ToString()).AppendLine();

                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;

                    stackTraceBuilder.AppendLine();
                    stackTraceBuilder.Append(ex.ToString()).AppendLine();
                }

                // Richard Slater's local installer builder path
                stackTraceBuilder = stackTraceBuilder.Replace(@"D:\EVEMon\", String.Empty);
                // Jimi's local installer builder path
                stackTraceBuilder = stackTraceBuilder.Replace(@"G:\Projects\Csharp\EVEMon\Repo\Mercurial\EVEMon", String.Empty);
                // TeamCity's installer builder path
                stackTraceBuilder = stackTraceBuilder.Replace(@"f:\tmp\evemon_installer\", String.Empty);
                // TeamCity's snapshot builder path
                stackTraceBuilder = stackTraceBuilder.Replace(@"f:\tmp\evemon\", String.Empty);
                return stackTraceBuilder.ToString();
            }
        }

        /// <summary>
        /// Handles the Click event of the CloseButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CloseButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Handles the LinkClicked event of the CopyDetailsLinkLabel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
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
                MessageBox.Show("Couldn't complete the operation, the clipboard is being used by another process. " +
                                "Wait a few moments and try again.");
            }
        }

        /// <summary>
        /// Handles the Click event of the ResetButtonLinkLabel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ResetButtonLinkLabel_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("This will clear all your saved settings, including skill plans and " +
                                              "character logins. You should only try this if EVEMon has errored more " +
                                              "than once.\r\n\r\nClear settings?",
                                              "Clear Settings?",
                                              MessageBoxButtons.YesNo,
                                              MessageBoxIcon.Warning,
                                              MessageBoxDefaultButton.Button2);

            if (dr != DialogResult.Yes)
                return;

            Settings.Reset();
            MessageBox.Show("Your settings have been reset.",
                            "Settings Reset", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Handles the LinkClicked event of the llblReport control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        private void llblReport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Util.OpenURL(new Uri(NetworkConstants.EVEMonBugReport));
        }

        /// <summary>
        /// Handles the LinkClicked event of the llblKnownProblems control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        private void llblKnownProblems_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Util.OpenURL(new Uri(NetworkConstants.EVEMonKnownProblems));
        }

        /// <summary>
        /// Handles the LinkClicked event of the llblLatestBinaries control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        private void llblLatestBinaries_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Util.OpenURL(new Uri(NetworkConstants.EVEMonMainPage));
        }
    }
}