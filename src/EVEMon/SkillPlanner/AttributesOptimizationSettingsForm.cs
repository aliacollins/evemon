using System;
using EVEMon.Common.Constants;
using EVEMon.Common.Controls;
using EVEMon.Common.Factories;
using EVEMon.Common.Models;

namespace EVEMon.SkillPlanner
{
    public partial class AttributesOptimizationSettingsForm : EVEMonForm
    {
        private readonly Character m_character;
        private readonly Plan m_plan;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributesOptimizationSettingsForm"/> class.
        /// </summary>
        private AttributesOptimizationSettingsForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributesOptimizationSettingsForm"/> class.
        /// </summary>
        /// <param name="plan">The plan.</param>
        public AttributesOptimizationSettingsForm(Plan plan)
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
        public AttributesOptimizationForm OptimizationForm { get; private set; }

        /// <summary>
        /// Handles the Click event of the buttonRemappingPoints control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void buttonRemappingPoints_Click(object sender, EventArgs e)
        {
            string title = $"Attributes optimization ({m_plan.Name}, remapping points)";
            string description = $"Based on {m_plan.Name}; using the remapping points you defined.";
            OptimizationForm = new AttributesOptimizationForm(m_character, m_plan,
                                                              AttributeOptimizationStrategy.RemappingPoints, title,
                                                              description);
        }

        /// <summary>
        /// Handles the Click event of the buttonWholePlan control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void buttonWholePlan_Click(object sender, EventArgs e)
        {
            string title = $"Attributes optimization ({m_plan.Name}, first year)";
            string description = $"Based on {m_plan.Name}; best attributes for the first year.";
            OptimizationForm = new AttributesOptimizationForm(m_character, m_plan,
                                                              AttributeOptimizationStrategy.OneYearPlan, title, description);
        }

        /// <summary>
        /// Handles the Click event of the buttonCharacter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void buttonCharacter_Click(object sender, EventArgs e)
        {
            string title = $"Attributes optimization ({m_character.Name})";
            string description = $"Based on {m_character.Name}";
            description += description.EndsWith("s", StringComparison.CurrentCulture) ? "' skills" : "'s skills";
            OptimizationForm = new AttributesOptimizationForm(m_character, m_plan,
                                                              AttributeOptimizationStrategy.Character, title, description);
        }
    }
}