using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using EVEMon.Common.Constants;
using EVEMon.Common.Controls;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Helpers;
using EVEMon.Common.Interfaces;
using EVEMon.Common.Models;

namespace EVEMon.SkillPlanner
{
    /// <summary>
    /// This controls allows the user to see how its plan's comutation times would change wth different implants.
    /// </summary>
    public partial class ImplantCalculator : EVEMonForm, IPlanOrderPluggable
    {
        private readonly Plan m_plan;
        private readonly Character m_character;

        /// <summary>
        /// Default constructor for designer.
        /// </summary>
        private ImplantCalculator()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor for the given plan.
        /// </summary>
        /// <param name="plan"></param>
        public ImplantCalculator(Plan plan)
            : this()
        {
            m_plan = plan;
            m_character = (Character)m_plan.Character;
        }

        /// <summary>
        /// On load, update the controls states.
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnLoad(EventArgs e)
        {
            // Set the min and max values of the NumericUpDown controls
            // based on character attributes value
            foreach (object control in AtrributesPanel.Controls)
            {
                NumericUpDown nud = control as NumericUpDown;

                if (nud == null)
                    continue;

                EveAttribute attrib = (EveAttribute)int.Parse((string)nud.Tag, CultureConstants.InvariantCulture);
                nud.Minimum = m_character[attrib].EffectiveValue - m_character[attrib].ImplantBonus;
                nud.Maximum = m_plan.ChosenImplantSet[attrib].Bonus > EveConstants.MaxImplantPoints
                    ? nud.Minimum + m_plan.ChosenImplantSet[attrib].Bonus
                    : nud.Minimum + EveConstants.MaxImplantPoints;
            }

            await UpdateContent();

            PlanEditor?.ShowWithPluggable(this);

            base.OnLoad(e);
        }

        /// <summary>
        /// Gets or sets a <see cref="PlanEditorControl"/>.
        /// </summary>
        public PlanEditorControl PlanEditor { private get; set; }

        /// <summary>
        /// Update all the numeric boxes
        /// </summary>
        private async Task UpdateContent()
        {
            gbAttributes.Text = $"Attributes of \"{m_plan.ChosenImplantSet.Name}\"";

            CharacterScratchpad characterScratchpad = m_plan.Character.After(m_plan.ChosenImplantSet);

            nudCharisma.Value = characterScratchpad.Charisma.EffectiveValue;
            nudWillpower.Value = characterScratchpad.Willpower.EffectiveValue;
            nudIntelligence.Value = characterScratchpad.Intelligence.EffectiveValue;
            nudPerception.Value = characterScratchpad.Perception.EffectiveValue;
            nudMemory.Value = characterScratchpad.Memory.EffectiveValue;

            // If the implant set isn't the active one we notify the user
            lblNotice.Visible = m_plan.ChosenImplantSet != m_character.ImplantSets.Current;

            //  Update all the times on the right pane
            await UpdateTimes();
        }

        /// <summary>
        /// Updates the labels on the right of the numeric box.
        /// </summary>
        /// <param name="attrib"></param>
        /// <param name="myValue"></param>
        /// <param name="lblAdjust"></param>
        /// <param name="lblEffectiveAttribute"></param>
        private void UpdateAttributeLabels(EveAttribute attrib, int myValue, Control lblAdjust, Control lblEffectiveAttribute)
        {
            CharacterScratchpad characterScratchpad = m_plan.Character.After(m_plan.ChosenImplantSet);

            Int64 baseAttr = characterScratchpad[attrib].EffectiveValue - characterScratchpad[attrib].ImplantBonus;
            Int64 adjust = myValue - baseAttr;

            lblAdjust.ForeColor = adjust >= 0 ? SystemColors.ControlText : Color.Red;
            lblAdjust.Text = $"{(adjust >= 0 ? "+" : String.Empty)}{adjust}";
            lblEffectiveAttribute.Text = characterScratchpad[attrib].EffectiveValue.ToNumericString(0);
        }

        /// <summary>
        /// Update all the times on the right pane (base time, best time, etc).
        /// </summary>
        private async Task UpdateTimes()
        {
            // Current (with implants)
            TimeSpan currentSpan = await UpdateTimesForCharacter(m_character.After(m_plan.ChosenImplantSet));

            // Current (without implants)
            ImplantSet noneImplantSet = m_character.ImplantSets.None;
            TimeSpan baseSpan = await UpdateTimesForCharacter(m_character.After(noneImplantSet));

            // This
            CharacterScratchpad scratchpad = CreateModifiedScratchpad(m_character.After(m_plan.ChosenImplantSet));
            TimeSpan thisSpan = await UpdateTimesForCharacter(scratchpad);

            lblCurrentSpan.Text = currentSpan.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas);
            lblCurrentDate.Text = DateTime.Now.Add(currentSpan).ToString(CultureConstants.DefaultCulture);
            lblBaseSpan.Text = baseSpan.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas);
            lblBaseDate.Text = DateTime.Now.Add(baseSpan).ToString(CultureConstants.DefaultCulture);
            lblThisSpan.Text = thisSpan.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas);
            lblThisDate.Text = DateTime.Now.Add(thisSpan).ToString(CultureConstants.DefaultCulture);

            // Are the new attributes better than current (without implants) ?
            lblComparedToBase.ForeColor = thisSpan > baseSpan
                ? Color.Red
                : thisSpan < baseSpan ? Color.Green : SystemColors.ControlText;
            lblComparedToBase.Text = thisSpan > baseSpan
                ? $"{thisSpan.Subtract(baseSpan).ToDescriptiveText(DescriptiveTextOptions.IncludeCommas)} slower than current base"
                : thisSpan < baseSpan
                    ? $"{baseSpan.Subtract(thisSpan).ToDescriptiveText(DescriptiveTextOptions.IncludeCommas)} faster than current base"
                    : @"No time difference than current base";

            // Are the new attributes better than current (with implants) ?
            lblComparedToCurrent.ForeColor = thisSpan > currentSpan
                ? Color.DarkRed
                : thisSpan < currentSpan ? Color.DarkGreen : SystemColors.ControlText;
            lblComparedToCurrent.Text = thisSpan > currentSpan
                ? $"{thisSpan.Subtract(currentSpan).ToDescriptiveText(DescriptiveTextOptions.IncludeCommas)} slower than current"
                : thisSpan < currentSpan
                    ? $"{currentSpan.Subtract(thisSpan).ToDescriptiveText(DescriptiveTextOptions.IncludeCommas)} faster than current"
                    : @"No time difference than current base";
        }

        /// <summary>
        /// Update the times labels from the given character
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        private Task<TimeSpan> UpdateTimesForCharacter(BaseCharacter character)
            => TaskHelper.RunCPUBoundTaskAsync(() => character.GetTrainingTimeToMultipleSkills(m_plan));


        #region Controls events

        /// <summary>
        /// When the intelligence numeric box changed, we update the attributes labels and the times on the right pane.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void nudIntelligence_ValueChanged(object sender, EventArgs e)
        {
            UpdateAttributeLabels(EveAttribute.Intelligence, (int)nudIntelligence.Value,
                lblAdjustIntelligence, lblEffectiveIntelligence);
            await UpdateTimes();
        }

        /// <summary>
        /// When the charisma numeric box changed, we update the attributes labels and the times on the right pane.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void nudCharisma_ValueChanged(object sender, EventArgs e)
        {
            UpdateAttributeLabels(EveAttribute.Charisma, (int)nudCharisma.Value,
                lblAdjustCharisma, lblEffectiveCharisma);
            await UpdateTimes();
        }

        /// <summary>
        /// When the perception numeric box changed, we update the attributes labels and the times on the right pane.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void nudPerception_ValueChanged(object sender, EventArgs e)
        {
            UpdateAttributeLabels(EveAttribute.Perception, (int)nudPerception.Value,
                lblAdjustPerception, lblEffectivePerception);
            await UpdateTimes();
        }

        /// <summary>
        /// When the memory numeric box changed, we update the attributes labels and the times on the right pane.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void nudMemory_ValueChanged(object sender, EventArgs e)
        {
            UpdateAttributeLabels(EveAttribute.Memory, (int)nudMemory.Value,
                lblAdjustMemory, lblEffectiveMemory);
            await UpdateTimes();
        }

        /// <summary>
        /// When the willpower numeric box changed, we update the attributes labels and the times on the right pane.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void nudWillpower_ValueChanged(object sender, EventArgs e)
        {
            UpdateAttributeLabels(EveAttribute.Willpower, (int)nudWillpower.Value,
                lblAdjustWillpower, lblEffectiveWillpower);
            await UpdateTimes();
        }


        /// <summary>
        /// Creates a scratchpad with the new implants values.
        /// </summary>
        /// <returns></returns>
        private CharacterScratchpad CreateModifiedScratchpad(BaseCharacter character)
        {
            // Creates a scratchpad with new implants
            CharacterScratchpad scratchpad = new CharacterScratchpad(character);

            scratchpad.Memory.ImplantBonus += (int)nudMemory.Value - character.Memory.EffectiveValue;
            scratchpad.Charisma.ImplantBonus += (int)nudCharisma.Value - character.Charisma.EffectiveValue;
            scratchpad.Intelligence.ImplantBonus += (int)nudIntelligence.Value - character.Intelligence.EffectiveValue;
            scratchpad.Perception.ImplantBonus += (int)nudPerception.Value - character.Perception.EffectiveValue;
            scratchpad.Willpower.ImplantBonus += (int)nudWillpower.Value - character.Willpower.EffectiveValue;

            return scratchpad;
        }

        #endregion


        #region IPlanOrderPluggable Members

        /// <summary>
        /// Updates the statistics for the plan editor.
        /// </summary>
        /// <param name="plan"></param>
        /// <param name="areRemappingPointsActive"></param>
        public void UpdateStatistics(BasePlan plan, out bool areRemappingPointsActive)
        {
            if (plan == null)
                throw new ArgumentNullException("plan");

            areRemappingPointsActive = true;

            CharacterScratchpad scratchpad = CreateModifiedScratchpad(m_character.After(m_plan.ChosenImplantSet));
            plan.UpdateStatistics(scratchpad, true, true);
            plan.UpdateOldTrainingTimes();
        }

        /// <summary>
        /// Updates the times when "choose implant set" changes.
        /// </summary>
        public Task UpdateOnImplantSetChange() => UpdateContent();

        #endregion
    }
}