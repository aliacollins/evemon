using System;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;
using System.Globalization;

namespace EVEMon
{
    public partial class DataUpdateNotifyForm : EVEMonForm
    {
        public DataUpdateNotifyForm()
        {
            InitializeComponent();
        }

        private DataUpdateAvailableEventArgs m_args;

        public DataUpdateNotifyForm(DataUpdateAvailableEventArgs args)
            : this()
        {
            m_args = args;
        }

        private void DataUpdateNotifyForm_Load(object sender, EventArgs e)
        {
            StringBuilder changedFiles = new StringBuilder();
            StringBuilder notes = new StringBuilder("UPDATE NOTES:\n");
            foreach (DatafileVersion dfv in m_args.ChangedFiles)
            {
                changedFiles.AppendFormat(CultureInfo.CurrentCulture, "Filename: {0}\t\tDated: {1}{3}Url: {2}/{0}{3}{3}", dfv.Name, dfv.DateChanged, dfv.Url, Environment.NewLine);
                notes.AppendLine(dfv.Message);
            }
            tbFiles.Lines = changedFiles.ToString().Split('\n');
            tbNotes.Lines = notes.ToString().Split('\n');
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            foreach (DatafileVersion dfv in m_args.ChangedFiles)
            {
                string urn = String.Format(CultureInfo.CurrentCulture, "{0}/{1}", dfv.Url, dfv.Name);
                string oldFilename = Path.Combine(EveClient.EVEMonDataDir, dfv.Name);

                string newFilename = String.Format(CultureInfo.CurrentCulture, "{0}.tmp", oldFilename);
                bool checksumOK = false;
                int attempt = 0;
                do
                {
                    File.Delete(newFilename);
                    using (UpdateDownloadForm f = new UpdateDownloadForm(urn, newFilename))
                    {
                        f.ShowDialog();
                        if (f.DialogResult == DialogResult.OK)
                        {
                            string filename = Path.GetFileName(newFilename);
                            Datafile datafile = new Datafile(filename);

                            if (datafile.MD5Sum != dfv.Md5)
                            {
                                attempt++;
                                File.Delete(newFilename);
                            }
                            else
                            {
                                try
                                {
                                    File.Delete(oldFilename + ".bak");
                                    File.Copy(oldFilename, oldFilename + ".bak");
                                    File.Delete(oldFilename);
                                    File.Move(newFilename, oldFilename);
                                }
                                catch (IOException ex)
                                {
                                    ExceptionHandler.LogException(ex, true);
                                }

                                checksumOK = true;
                            }
                        }
                    }
                } while (!checksumOK && attempt < 3);
            }
            Settings.SaveImmediate();

            MessageBox.Show("Your datafiles have been updated. Please restart EVEMon for them to take effect.", "Data Files Updated", MessageBoxButtons.OK);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnLater_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}