using System;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon
{
    public partial class DataUpdateNotifyForm : EVEMonForm
    {
        public DataUpdateNotifyForm()
        {
            InitializeComponent();
        }

        private Settings m_settings;
        private DataUpdateAvailableEventArgs m_args;

        public DataUpdateNotifyForm(Settings settings, DataUpdateAvailableEventArgs args)
            : this()
        {
            m_args = args;
            m_settings = settings;
        }

        private void DataUpdateNotifyForm_Load(object sender, EventArgs e)
        {
            StringBuilder changedFiles = new StringBuilder();
            StringBuilder notes = new StringBuilder("UPDATE NOTES:\n");
            foreach (DatafileVersion dfv in m_args.ChangedFiles)
            {
                changedFiles.Append(String.Format("{0} dated {1}. Url: {2}/{0}\n",dfv.Name,dfv.DateChanged,dfv.Url));
                notes.Append(dfv.Message);
                notes.Append("\n");
            }
            tbFiles.Lines = changedFiles.ToString().Split('\n');
            tbNotes.Lines = notes.ToString().Split('\n');
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            foreach (DatafileVersion dfv in m_args.ChangedFiles)
            {
                string urn = String.Format("{0}/{1}",dfv.Url,dfv.Name);
                Uri i = new Uri(urn);
//                string oldFilename =  String.Format("{1}Resources{0}{2}",
//                              Path.DirectorySeparatorChar,
//                              System.AppDomain.CurrentDomain.BaseDirectory,
//                              dfv.Name);
                string oldFilename = String.Format("{0}{1}{2}",
                              Settings.EveMonDataDir,
                              Path.DirectorySeparatorChar,
                              dfv.Name);

                string newFilename = oldFilename + ".tmp";
                File.Delete(newFilename);
                string fn = Path.GetFileName(i.AbsolutePath);
                using (UpdateDownloadForm f = new UpdateDownloadForm(urn, newFilename))
                {
                    f.ShowDialog();
                    if (f.DialogResult == DialogResult.OK)
                    { 
                        try
                        {
                            File.Delete(oldFilename + ".bak");
                            File.Copy(oldFilename,oldFilename + ".bak");
                            File.Delete(oldFilename);
                            File.Move(newFilename,oldFilename);
                        }
                        catch (Exception) { }
                        // download, patch and update settings MD5.
                    }
                }
            }
            m_settings.CalculateChecksums();
            m_settings.SaveImmediate();
            MessageBox.Show("Your datafiles have been updated. Please restart EVEMon for them to take effect.", "Data Files Updatad", MessageBoxButtons.OK);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ExecPatcher(string fn, string args)
        {
            try
            {
                Process.Start(fn, args);
            }
            catch (Exception e)
            {
                ExceptionHandler.LogRethrowException(e);
                if (File.Exists(fn))
                {
                    try
                    {
                        File.Delete(fn);
                    }
                    catch
                    {
                        ExceptionHandler.LogException(e, false);
                    }
                }
                throw;
            }
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