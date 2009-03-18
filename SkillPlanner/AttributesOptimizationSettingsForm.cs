using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon.SkillPlanner
{
    public partial class AttributesOptimizationSettingsForm : Form
    {
        private AttributesOptimizationForm m_optimizationForm;
        private readonly CharacterInfo m_character;
        private readonly Plan m_plan;

        public AttributesOptimizationSettingsForm(CharacterInfo character, Plan plan)
        {
            InitializeComponent();
            this.m_plan = plan;
            this.m_character = character;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            foreach (var entry in m_plan.Entries)
            {
                this.comboStartSkill.Items.Add(entry.ToString());
            }

            // Empty plan ?
            if (this.comboStartSkill.Items.Count == 0)
            {
                this.radioPartialPlan.Enabled = false;
                this.radioWholePlan.Enabled = false;
            }
            else
            {
                this.comboStartSkill.SelectedIndex = 0;
            }
        }

        public AttributesOptimizationForm OptimizationForm
        {
            get { return this.m_optimizationForm; }
        }

        private void radioPartialPlan_CheckedChanged(object sender, EventArgs e)
        {
            // Disable/enable the group for partial plan's settings
            this.groupPartialPlan.Enabled = this.radioPartialPlan.Checked;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this.radioPartialPlan.Checked)
            {
                int days = (int)(this.numYears.Value * 365 + this.numMonths.Value * 30);
                var training = AttributesOptimizer.GetTraining(m_plan, this.comboStartSkill.SelectedIndex);
                string formTitle = "Attributes optimization (" + m_plan.Name + " - partial)";
                string description = "Based on a partial subset of plan \"" + m_plan.Name + "\"\r\n" + 
                    "* start at " + this.comboStartSkill.SelectedItem.ToString() +
                    "\r\n* stop after " + ((int)this.numYears.Value).ToString() + " year(s) and " + ((int)this.numMonths.Value).ToString() + " month(s)";
                this.m_optimizationForm = new AttributesOptimizationForm(this.m_character, training, TimeSpan.FromDays(days), formTitle, description, true);
            }
            else if (this.radioWholePlan.Checked)
            {
                var training = AttributesOptimizer.GetTraining(m_plan, 0);
                string formTitle = "Attributes optimization (" + m_plan.Name + ")";
                string description = "Based on plan \"" + m_plan.Name + "\"";
                this.m_optimizationForm = new AttributesOptimizationForm(this.m_character, training, TimeSpan.MaxValue, formTitle, description, true);
            }
            else
            {
                var training = AttributesOptimizer.GetTraining(m_character);
                string formTitle = "Attributes optimization (" + m_character.Name + ")";
                string description = "Based on " + m_character.Name;
                description += (description.EndsWith("s") ? "' skills" : "'s skills");
                this.m_optimizationForm = new AttributesOptimizationForm(this.m_character, training, TimeSpan.MaxValue, formTitle, description, false);
            }
        }
    }
}
