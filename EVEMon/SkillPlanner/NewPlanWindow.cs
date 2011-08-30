using System;
using System.Windows.Forms;
using EVEMon.Common.Controls;

namespace EVEMon.SkillPlanner
{
    public partial class NewPlanWindow : EVEMonForm
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewPlanWindow"/> class.
        /// </summary>
        public NewPlanWindow()
        {
            InitializeComponent();
            Result = String.Empty;
            PlanName = String.Empty;
        }

        /// <summary>
        /// Gets or sets the name of the plan.
        /// </summary>
        /// <value>The name of the plan.</value>
        public string PlanName { private get; set; }

        public string Result { get; private set; }

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Handles the Click event of the btnOk control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnOk_Click(object sender, EventArgs e)
        {
            Result = textBox1.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Handles the TextChanged event of the textBox1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            btnOk.Enabled = (!String.IsNullOrEmpty(textBox1.Text));
        }

        /// <summary>
        /// Handles the Shown event of the NewPlanWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void NewPlanWindow_Shown(object sender, EventArgs e)
        {
            textBox1.Text = PlanName;
            textBox1.SelectAll();
        }

    }
}
