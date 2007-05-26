using System;
using System.IO;
using System.Net;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon
{
    public partial class UpdateDownloadForm : EVEMonForm
    {
        public UpdateDownloadForm()
        {
            InitializeComponent();
        }

        public UpdateDownloadForm(string url, string filename)
            : this()
        {
            m_url = url;
            m_fileName = filename;
        }

        private string m_url;
        private string m_fileName;

        private HttpWebRequest m_request;
        private HttpWebResponse m_response;
        private FileStream m_targetFile;
        private Stream m_netStream;

        private void UpdateDownloadForm_Shown(object sender, EventArgs e)
        {
            try
            {
                m_request = EVEMonWebRequest.GetWebRequest(m_url);
                m_request.BeginGetResponse(new AsyncCallback(GotResponse), null);
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex, true);
                DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private int m_bytesRead = 0;
        private byte[] m_buffer = null;

        private void GotResponse(IAsyncResult ar)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                                                  {
                                                      GotResponse(ar);
                                                  }));
                return;
            }

            try
            {
                m_response = (HttpWebResponse)m_request.EndGetResponse(ar);
                m_netStream = m_response.GetResponseStream();
                m_targetFile = new FileStream(m_fileName, FileMode.Create, FileAccess.Write, FileShare.None);
                UpdateStatusText();
                BeginRead();
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e, true);
                DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void BeginRead()
        {
            if (m_buffer == null)
            {
                m_buffer = new byte[4096];
            }

            try
            {
                IAsyncResult ar = null;
                while ((ar == null || ar.CompletedSynchronously) && !m_done)
                {
                    ar = m_netStream.BeginRead(m_buffer, 0, m_buffer.Length, new AsyncCallback(EndRead), null);
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e, true);
                DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }


        private bool m_done = false;

        private void EndRead(IAsyncResult ar)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                                                  {
                                                      EndRead(ar);
                                                  }));
                return;
            }

            try
            {
                int thisBytesRead = m_netStream.EndRead(ar);
                if (thisBytesRead == 0)
                {
                    m_done = true;
                    FinishDownload();
                }
                else
                {
                    m_targetFile.Write(m_buffer, 0, thisBytesRead);
                    m_bytesRead += thisBytesRead;
                    UpdateStatusText();
                    if (!ar.CompletedSynchronously)
                    {
                        BeginRead();
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e, true);
                DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void FinishDownload()
        {
            try
            {
                m_targetFile.Close();
                m_targetFile = null;
                m_netStream.Close();
                m_netStream = null;
                m_response.Close();
                m_response = null;
                DialogResult = DialogResult.OK;
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e, true);
                DialogResult = DialogResult.Cancel;
            }
            Close();
        }

        private void UpdateStatusText()
        {
            if (m_response.ContentLength > 0)
            {
                label1.Text = String.Format("Downloading update ({0}%, {1} of {2} bytes received)...",
                                            m_bytesRead * 100 / m_response.ContentLength, m_bytesRead,
                                            m_response.ContentLength);
                pbProgress.Style = ProgressBarStyle.Blocks;
                pbProgress.Minimum = 0;
                pbProgress.Maximum = Convert.ToInt32(m_response.ContentLength);
                pbProgress.Value = m_bytesRead;
            }
            else
            {
                label1.Text = String.Format("Downloading update ({0} bytes received)...",
                                            m_bytesRead);
                if (pbProgress.Style != ProgressBarStyle.Marquee)
                {
                    pbProgress.Style = ProgressBarStyle.Marquee;
                }
            }
        }

        private void UpdateDownloadForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (m_targetFile != null)
            {
                try
                {
                    m_targetFile.Close();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.LogException(ex, false);
                }
            }
            if (m_netStream != null)
            {
                try
                {
                    m_netStream.Close();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.LogException(ex, false);
                }
            }
            if (m_response != null)
            {
                try
                {
                    m_response.Close();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.LogException(ex, false);
                }
            }
            if (DialogResult == DialogResult.Cancel && File.Exists(m_fileName))
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

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}