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

        public AttributesOptimizationForm OptimizationForm
        {
            get { return this.m_optimizationForm; }
        }

        private void buttonRemappingPoints_Click(object sender, EventArgs e)
        {
            string title = "Attributes optimization (" + m_plan.Name + ", remapping points)";
            string description = "Based on " + m_plan.Name + "; using the remapping points you defined.";
            this.m_optimizationForm = new AttributesOptimizationForm(m_character, m_plan, 
                AttributesOptimizationForm.Strategy.RemappingPoints, title, description);
        }

        private void buttonWholePlan_Click(object sender, EventArgs e)
        {
            string title = "Attributes optimization (" + m_plan.Name + ", first year)";
            string description = "Based on " + m_plan.Name + "; best attributes for the first year.";
            this.m_optimizationForm = new AttributesOptimizationForm(m_character, m_plan,
                AttributesOptimizationForm.Strategy.OneYearPlan, title, description);
        }

        private void buttonCharacter_Click(object sender, EventArgs e)
        {
            string title = "Attributes optimization (" + m_character.Name + ")";
            string description = "Based on " + m_character.Name;
            description += (description.EndsWith("s") ? "' skills" : "'s skills");
            this.m_optimizationForm = new AttributesOptimizationForm(m_character, m_plan,
                AttributesOptimizationForm.Strategy.Character, title, description);
        }
    }
}
