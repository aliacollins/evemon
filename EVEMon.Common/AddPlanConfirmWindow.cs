using System;
using System.Collections.Generic;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon.Common 
{
    public partial class AddPlanConfirmWindow : EVEMonForm
    {
        public AddPlanConfirmWindow()
        {
            InitializeComponent();
        }

        public AddPlanConfirmWindow(IEnumerable<Plan.Entry> entries)
            : this()
        {
            m_entries = entries;
        }

        private IEnumerable<Plan.Entry> m_entries;

        private void AddPlanConfirmWindow_Load(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            foreach (Plan.Entry pe in m_entries)
            {
                string m_skill = pe.SkillName + " " + Skill.GetRomanForInt(pe.Level);
                if (pe.AddNoteonly)
                {
                    m_skill = m_skill + " (planned)";
                }
                listBox1.Items.Add(m_skill);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public static bool ConfirmSkillAdd(List<Plan.Entry> planEntries,int lowestPrereqPriority)
        {
            bool res = false;
            using (AddPlanConfirmWindow f = new AddPlanConfirmWindow(planEntries))
            {
                if (lowestPrereqPriority == Int32.MinValue)
                {
                    f.nudPriority.Value = Plan.Entry.DEFAULT_PRIORITY;
                    f.groupBox1.Visible = false;
                }
                else
                {
                    f.nudPriority.Value = lowestPrereqPriority;
                    f.nudPriority.Minimum = lowestPrereqPriority;
                    f.lbPriority.Text = "The highest priority you can set is " + lowestPrereqPriority.ToString();
                }
                DialogResult dr = f.ShowDialog();
                res = dr == DialogResult.OK;
                if (dr == DialogResult.OK)
                {
                    // sort out priorities
                    foreach (Plan.Entry pe in planEntries)
                    {
                        if (!pe.AddNoteonly)
                            pe.Priority = (int)f.nudPriority.Value;
                    }
                    
                }
            }
            return res;
        }
    }
}
