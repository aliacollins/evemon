using System;
using System.IO;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Constants;
using EVEMon.Common.Controls;
using EVEMon.Common.Helpers;
using EVEMon.Common.Net;
using EVEMon.Common.Threading;

namespace EVEMon.Updater
{
    public partial class UpdateDownloadForm : EVEMonForm
    {
        private readonly Uri m_url;
        private readonly string m_fileName;
        private object m_request;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateDownloadForm"/> class.
        /// </summary>
        private UpdateDownloadForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateDownloadForm"/> class.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="fileName">The filename.</param>
        public UpdateDownloadForm(Uri url, string fileName)
            : this()
        {
            m_url = url;
            m_fileName = fileName;
        }

        /// <summary>
        /// Handles the Shown event of the UpdateDownloadForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void UpdateDownloadForm_Shown(object sender, EventArgs e)
        {
            try
            {
                m_request = HttpWebService.DownloadFileAsync(m_url, m_fileName, DownloadCompleted, ProgressChanged);
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogRethrowException(ex);
                throw;
            }
        }

        /// <summary>
        /// Progresses the changed.
        /// </summary>
        /// <param name="e">The event.</param>
        private void ProgressChanged(DownloadProgressChangedArgs e)
        {
            if (InvokeRequired)
            {
                Dispatcher.Invoke(() => ProgressChanged(e));
                return;
            }

            if (e.TotalBytesToReceive > 0)
            {
                ProgressLabel.Text = String.Format(CultureConstants.DefaultCulture,
                    "Downloading update ({0}%, {1:N0} of {2:N0} bytes received)...",
                    e.ProgressPercentage, e.BytesReceived, e.TotalBytesToReceive);
                pbProgress.Style = ProgressBarStyle.Blocks;
                pbProgress.Minimum = 0;
                pbProgress.Maximum = 100;

                // Under Vista and Windows 7 there is a lag when progress bar updates too quick.
                // This hackish way though solves this issue (in a way) as explained in
                // http://stackoverflow.com/questions/977278/how-can-i-make-the-progress-bar-update-fast-enough/1214147#1214147.
                pbProgress.Value = e.ProgressPercentage;
                pbProgress.Value = (e.ProgressPercentage == 0 ? e.ProgressPercentage : e.ProgressPercentage - 1);
            }
            else
            {
                ProgressLabel.Text = String.Format(CultureConstants.DefaultCulture,
                    "Downloading update ({0:N0} bytes received)...",
                    e.BytesReceived);
                pbProgress.Style = ProgressBarStyle.Marquee;
            }
        }

        /// <summary>
        /// Called when download is completed.
        /// </summary>
        /// <param name="e">The event.</param>
        private void DownloadCompleted(DownloadFileAsyncResult e)
        {
            if (InvokeRequired)
            {
                Dispatcher.Invoke(() => DownloadCompleted(e));
                return;
            }

            if (e.Error != null)
            {
                ExceptionHandler.LogException(e.Error, true);
                DialogResult = DialogResult.Cancel;
            }
            else if (e.Cancelled)
            {
                DialogResult = DialogResult.Cancel;
                if (File.Exists(m_fileName))
                {
                    try
                    {
                        File.Delete(m_fileName);
                    }
                    catch (ArgumentException ex)
                    {
                        ExceptionHandler.LogException(ex, false);
                    }
                    catch (IOException ex)
                    {
                        ExceptionHandler.LogException(ex, false);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        ExceptionHandler.LogException(ex, false);
                    }
                }
            }
            else
                DialogResult = DialogResult.OK;

            Close();
        }

        /// <summary>
        /// Handles the Click event of the btCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btCancel_Click(object sender, EventArgs e)
        {
            if (m_request != null)
                HttpWebService.CancelRequest(m_request);
        }

        /// <summary>
        /// Handles the FormClosing event of the UpdateDownloadForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FormClosingEventArgs"/> instance containing the event data.</param>
        private void UpdateDownloadForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Visible || (e.CloseReason != CloseReason.ApplicationExitCall && e.CloseReason != CloseReason.TaskManagerClosing &&
                e.CloseReason != CloseReason.WindowsShutDown))
            {
                return;
            }

            if (m_request != null)
                HttpWebService.CancelRequest(m_request);
        }
    }
}