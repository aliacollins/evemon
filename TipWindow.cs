using System;
using System.Drawing;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon
{
    public partial class TipWindow : EVEMonForm
    {
        public TipWindow()
        {
            InitializeComponent();
        }

        public TipWindow(string title, string tiptext)
            : this()
        {
            this.Text = title;
            label1.Text = tiptext;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (cbDontShowAgain.Checked)
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
            }

            this.Close();
        }

        private void TipWindow_Load(object sender, EventArgs e)
        {
            Bitmap b = new Bitmap(32, 32);
            using (Graphics g = Graphics.FromImage(b))
            {
                g.DrawIcon(SystemIcons.Information, 0, 0);
            }
            pictureBox1.Image = b;
        }

        private static object m_lockObject = new object();

        public static void ShowTip(string key, string title, string tiptext)
        {
            lock (m_lockObject)
            {
                Settings s = Program.Settings;

                if (!s.ConfirmedTips.Contains(key))
                {
                    using (TipWindow tw = new TipWindow(title, tiptext))
                    {
                        DialogResult dr = tw.ShowDialog();
                        if (dr == DialogResult.OK)
                        {
                            s.ConfirmedTips.Add(key);
                            s.Save();
                        }
                    }
                }
            }
        }
    }
}