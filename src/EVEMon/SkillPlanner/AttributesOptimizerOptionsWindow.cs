using System;
using EVEMon.Common.Controls;
using EVEMon.Common.Factories;
using EVEMon.Common.Models;

namespace EVEMon.SkillPlanner
{
    public partial class AttributesOptimizerOptionsWindow : EVEMonForm
    {
        private readonly Character m_character;
        private readonly Plan m_plan;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributesOptimizerOptionsWindow"/> class.
        /// </summary>
        private AttributesOptimizerOptionsWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributesOptimizerOptionsWindow"/> class.
        /// </summary>
        /// <param name="plan">The plan.</param>
        public AttributesOptimizerOptionsWindow(Plan plan)
            : this()
        {
            if (plan == null)
                throw new ArgumentNullException("plan");

            buttonWholePlan.Font = FontFactory.GetFont("Microsoft Sans Serif", 10F);
            buttonCharacter.Font = FontFactory.GetFont("Microsoft Sans Serif", 10F);
            buttonRemappingPoints.Font = FontFactory.GetFont("Microsoft Sans Serif", 10F);

            m_plan = plan;
            m_character = (Character)plan.Character;
        }

        /// <summary>
        /// Gets the optimization form.
        /// </summary>
        /// <value>The optimization form.</value>
        public AttributesOptimizerWindow OptimizationForm { get; private set; }

        /// <summary>
        /// Handles the Click event of the buttonRemappingPoints control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void buttonRemappingPoints_Click(object sender, EventArgs e)
        {
            OptimizationForm = new AttributesOptimizerWindow(m_character, m_plan, AttributeOptimizationStrategy.RemappingPoints);
        }

        /// <summary>
        /// Handles the Click event of the buttonWholePlan control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void buttonWholePlan_Click(object sender, EventArgs e)
        {
            OptimizationForm = new AttributesOptimizerWindow(m_character, m_plan, AttributeOptimizationStrategy.OneYearPlan);
        }

        /// <summary>
        /// Handles the Click event of the buttonCharacter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void buttonCharacter_Click(object sender, EventArgs e)
        {
            OptimizationForm = new AttributesOptimizerWindow(m_character, m_plan, AttributeOptimizationStrategy.Character);
        }
    }
}