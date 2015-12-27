using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Constants;
using EVEMon.Common.Controls;
using EVEMon.Common.Data;
using EVEMon.Common.Extensions;
using EVEMon.Common.Factories;
using EVEMon.Common.Helpers;
using EVEMon.Common.Properties;

namespace EVEMon.ExceptionHandling
{
    /// <summary>
    /// Form to handle the display of the error report for easy bug reporting.
    /// </summary>
    public partial class UnhandledExceptionWindow : EVEMonForm
    {
        private readonly Exception m_exception;


        #region Constructor

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

        #endregion


        #region Inherited Events

        /// <summary>
        /// Loads resources, generates the report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnhandledExceptionWindow_Load(object sender, EventArgs e)
        {
            WhatCanYouDoLabel.Font = FontFactory.GetFont("Tahoma", 10F);

            SetBugImage();

            BuildExceptionMessage();
        }

        #endregion


        #region Content Management

        /// <summary>
        /// Builds the exception message.
        /// </summary>
        private void BuildExceptionMessage()
        {
            EveMonClient.StopTraceLogging();

            try
            {
                StringBuilder exceptionReport = new StringBuilder();
                OperatingSystem os = Environment.OSVersion;

                exceptionReport.AppendFormat(CultureConstants.DefaultCulture, "EVEMon Version: {0}{1}", EveMonClient.FileVersionInfo.FileVersion,
                                             Environment.NewLine);
                exceptionReport.AppendFormat(CultureConstants.DefaultCulture, ".NET Runtime Version: {0}{1}", Environment.Version,
                                             Environment.NewLine);
                exceptionReport.AppendFormat(CultureConstants.DefaultCulture, "Operating System: {0}{1}", os.VersionString,
                                             Environment.NewLine);
                exceptionReport.AppendFormat(CultureConstants.DefaultCulture, "Executable Path: {0}{1}", Environment.CommandLine,
                                             Environment.NewLine);
                exceptionReport.AppendLine();
                exceptionReport.Append(GetRecursiveStackTrace()).AppendLine();
                exceptionReport.AppendLine();
                exceptionReport.Append(GetDatafileReport()).AppendLine();
                exceptionReport.AppendLine();
                exceptionReport.Append("Diagnostic Log:").AppendLine();
                exceptionReport.Append(GetTraceLog().Trim()).AppendLine();

                TechnicalDetailsTextBox.Text = exceptionReport.ToString();
            }
            catch (InvalidOperationException ex)
            {
                ExceptionHandler.LogException(ex, true);
                TechnicalDetailsTextBox.Text = "Error retrieving error data. Wow, things are *really* screwed up!";
            }
        }

        /// <summary>
        /// Gets the trace log.
        /// </summary>
        /// <returns></returns>
        private static string GetTraceLog()
        {
            string trace;
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
            return trace;
        }

        /// <summary>
        /// Sets the bug image.
        /// </summary>
        private void SetBugImage()
        {
            try
            {
                Bitmap bug = Resources.Bug;

                int oHeight = bug.Height;
                int oWidth = bug.Width;
                if (bug.Height <= BugPictureBox.ClientSize.Height)
                    return;

                double scale = (double)(BugPictureBox.ClientSize.Height) / bug.Height;
                oHeight = (int)(oHeight * scale);
                oWidth = (int)(oWidth * scale);
                BugPictureBox.Image = new Bitmap(bug, new Size(oWidth, oHeight));
                BugPictureBox.ClientSize = new Size(oWidth, oHeight);
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogRethrowException(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the datafile report.
        /// </summary>
        /// <value>The datafile report.</value>
        private static string GetDatafileReport()
        {
            StringBuilder datafileReport = new StringBuilder();

            try
            {
                datafileReport.AppendLine("Datafile report:");

                foreach (string datafile in Datafile.GetFilesFrom(EveMonClient.EVEMonDataDir, Datafile.DatafilesExtension))
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

        /// <summary>
        /// Gets the recursive stack trace.
        /// </summary>
        /// <value>The recursive stack trace.</value>
        private string GetRecursiveStackTrace()
        {
            StringBuilder stackTraceBuilder = new StringBuilder();
            Exception ex = m_exception;

            stackTraceBuilder.Append(ex).AppendLine();

            while (ex.InnerException != null)
            {
                ex = ex.InnerException;

                stackTraceBuilder.AppendLine().Append(ex).AppendLine();
            }

            // Remove project local path from message
            return stackTraceBuilder.ToString().RemoveProjectLocalPath();
        }

        #endregion


        #region Local Events

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
        /// Handles the Click event of the DataDirectoryButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void DataDirectoryButton_Click(object sender, EventArgs e)
        {
            Util.OpenURL(new Uri(EveMonClient.EVEMonDataDir));
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

        #endregion
    }
}