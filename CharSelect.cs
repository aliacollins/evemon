using System;
using System.Collections.Generic;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon
{
    public partial class CharSelect : EVEMonForm
    {
        public CharSelect()
        {
            InitializeComponent();
        }

        //public CharSelect(IEnumerable<string> charEnum, string preferChar)
        //    : this()
        //{
        //    int c = 0;
        //    lbChars.Items.Clear();
        //    foreach (string s in charEnum)
        //    {
        //        c++;
        //        lbChars.Items.Add(s);
        //    }
        //    if (c == 1)
        //        m_result = lbChars.Items[0] as string;
        //    if (lbChars.Items.Contains(preferChar))
        //        m_result = preferChar;
        //}

        public CharSelect(List<string> s)
            : this()
        {
            lbChars.Items.Clear();
            foreach (string charName in s)
            {
                lbChars.Items.Add(charName);
            }
            if (s.Count > 1)
                lbChars.Items.Add("(All Characters)");
        }

        private void lbChars_DoubleClick(object sender, EventArgs e)
        {
            HandleSelect();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            HandleSelect();
        }

        private string m_result;

        public string Result
        {
            get { return m_result; }
        }

        private void HandleSelect()
        {
            if (lbChars.SelectedItem != null)
            {
                this.DialogResult = DialogResult.OK;
                m_result = lbChars.SelectedItem as String;
                this.Close();
            }
        }

        private void lbChars_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSelect.Enabled = (lbChars.SelectedItem != null);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}