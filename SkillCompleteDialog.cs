using System;
using System.Collections.Generic;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon
{
    public partial class SkillCompleteDialog : EVEMonForm
    {
        public SkillCompleteDialog()
        {
            InitializeComponent();
        }

        public SkillCompleteDialog(List<string> skills)
            : this()
        {
            listBox1.Items.Clear();
            foreach (string s in skills)
            {
                listBox1.Items.Add(s);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}