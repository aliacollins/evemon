using System;
using System.IO;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Net;
using EVEMon.Common.Controls;

namespace EVEMon
{
    public partial class UpdateDownloadForm : EVEMonForm
    {
        private readonly string m_url;
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
        /// <param name="filename">The filename.</param>
        public UpdateDownloadForm(string url, string filename)
            : this()
        {
            m_url = url;
            m_fileName = filename;
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
                m_request = EveMonClient.HttpWebService.DownloadFileAsync(m_url, m_fileName, DownloadCompletedCallback,
                                                                          ProgressChangedCallback);
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex, true);
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        /// <summary>
        /// Progresses the changed callback.
        /// </summary>
        /// <param name="e">The e.</param>
        private void ProgressChangedCallback(DownloadProgressChangedArgs e)
        {
            Invoke((MethodInvoker)(() => ProgressChanged(e)));
        }

        /// <summary>
        /// Progresses the changed.
        /// </summary>
        /// <param name="e">The e.</param>
        private void ProgressChanged(DownloadProgressChangedArgs e)
        {
            if (e.TotalBytesToReceive > 0)
            {
                label1.Text = String.Format(CultureConstants.TidyInteger,
                                            "Downloading update ({0}%, {1:n} of {2:n} bytes received)...",
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
                label1.Text = String.Format(CultureConstants.TidyInteger, "Downloading update ({0:n} bytes received)...",
                                            e.BytesReceived);
                pbProgress.Style = ProgressBarStyle.Marquee;
            }
        }

        /// <summary>
        /// Downloads the completed callback.
        /// </summary>
        /// <param name="e">The e.</param>
        private void DownloadCompletedCallback(DownloadFileAsyncResult e)
        {
            Invoke((MethodInvoker)(() => DownloadCompleted(e)));
        }

        /// <summary>
        /// Downloads the completed.
        /// </summary>
        /// <param name="e">The e.</param>
        private void DownloadCompleted(DownloadFileAsyncResult e)
        {
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
                    catch (Exception ex)
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
                EveMonClient.HttpWebService.CancelRequest(m_request);
        }
    }
}