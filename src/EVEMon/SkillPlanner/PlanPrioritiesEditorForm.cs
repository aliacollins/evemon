using System;
using System.Windows.Forms;
using EVEMon.Common.Controls;

namespace EVEMon.SkillPlanner
{
    public partial class PlanPrioritiesEditorForm : EVEMonForm
    {
        public PlanPrioritiesEditorForm()
        {
            InitializeComponent();
        }

        public int Priority
        {
            get { return (int)nudPriority.Value; }
            set { nudPriority.Value = value; }
        }

        /// <summary>
        /// Handles the Click event of the btnOk control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Handles the Load event of the ChangePriorityForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ChangePriorityForm_Load(object sender, EventArgs e)
        {
            nudPriority.Select(0, 3);
        }
    }
}