using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace EVEMon.SkillPlanner
{
    public partial class ChangePriorityForm : Form
    {
        public ChangePriorityForm()
        {
            InitializeComponent();
        }

        public int Priority
        {
            get { return (int)nudPriority.Value; }
            set { nudPriority.Value = value; }
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ChangePriorityForm_Load(object sender, EventArgs e)
        {
            nudPriority.Select(0, 3);
        }
    }

}