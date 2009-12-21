using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using System.Drawing.Imaging;

namespace EVEMon.SkillPlanner
{
    /// <summary>
    /// Represents the method that will handle change of attributes in optimization control.
    /// </summary>
    /// <param name="control">Source attribute optimization control</param>
    /// <param name="remapping">Current remapping in control</param>
    public delegate void AttributeChangedHandler(AttributesOptimizationControl control, AttributesOptimizer.Remapping remapping);

    /// <summary>
    /// Control that shows attribute remapping and allows to adjust it.
    /// </summary>
    public partial class AttributesOptimizationControl : UserControl
    {
        private readonly CharacterInfo m_character;
        private readonly AttributesOptimizer.Remapping m_remapping;

        /// <summary>
        /// Occurs when attributes changes. 
        /// </summary>
        [Category("Behavior")]
        public event AttributeChangedHandler AttributeChanged;

        /// <summary>
        /// Initializes a new instance of EVEMon.SkillPlanner.AttributesOptimizationControl.
        /// </summary>
        /// <param name="character">Character information</param>
        /// <param name="remapping">Optimized remapping</param>
        public AttributesOptimizationControl(CharacterInfo character, AttributesOptimizer.Remapping remapping)
        {
            InitializeComponent();
            
            m_character = character;
            m_remapping = remapping;

            UpdateControls(m_character, m_remapping);
        }

        /// <summary>
        /// Updates bars and labels with given attributes from remapping.
        /// </summary>
        /// <param name="character">Character information</param>
        /// <param name="remapping">Remapping with attributes and training time</param>
        private void UpdateControls(CharacterInfo character, AttributesOptimizer.Remapping remapping)
        {
            UpdateAttributeControls(character, remapping, EveAttribute.Perception, lbPER, pbPERBase, pbPERImplants, pbPERSkills);
            UpdateAttributeControls(character, remapping, EveAttribute.Willpower, lbWIL, pbWILBase, pbWILImplants, pbWILSkills);
            UpdateAttributeControls(character, remapping, EveAttribute.Memory, lbMEM, pbMEMBase, pbMEMImplants, pbMEMSkills);
            UpdateAttributeControls(character, remapping, EveAttribute.Intelligence, lbINT, pbINTBase, pbINTImplants, pbINTSkills);
            UpdateAttributeControls(character, remapping, EveAttribute.Charisma, lbCHA, pbCHABase, pbCHAImplants, pbCHASkills);

            // Update the current time control
            this.lbCurrentTime.Text = Skill.TimeSpanToDescriptiveText(remapping.BaseDuration, DescriptiveTextOptions.IncludeCommas);

            // Update the optimized time control
            this.lbOptimizedTime.Text = Skill.TimeSpanToDescriptiveText(remapping.BestDuration, DescriptiveTextOptions.IncludeCommas);

            // Update the time benefit control
            if (remapping.BestDuration < remapping.BaseDuration)
            {
                this.lbGain.ForeColor = Color.Black;
                this.lbGain.Text = Skill.TimeSpanToDescriptiveText(remapping.BaseDuration - remapping.BestDuration,
                    DescriptiveTextOptions.IncludeCommas) + " better than current";
            }
            else
                if (remapping.BaseDuration < remapping.BestDuration)
                {
                    this.lbGain.ForeColor = Color.Red;
                    this.lbGain.Text = Skill.TimeSpanToDescriptiveText(remapping.BestDuration - remapping.BaseDuration,
                    DescriptiveTextOptions.IncludeCommas) + " slower than current";
                }
                else
                {
                    this.lbGain.ForeColor = Color.Black;
                    this.lbGain.Text = "Same as current";
                }

            // A plan may not have a years worth of skills in it,
            // only fair to warn the user
            this.lbWarning.Visible = remapping.BestDuration < new TimeSpan(365, 0, 0, 0);
        }

        /// <summary>
        /// Updates bars and labels for specified attribute.
        /// </summary>
        /// <param name="character">Character information</param>
        /// <param name="remapping">Attribute remapping</param>
        /// <param name="attrib">Attribute that will be used to update controls</param>
        /// <param name="lb">Label control</param>
        /// <param name="pbBase">Attribute bar for base value</param>
        /// <param name="pbImplants">Attribute bar for implants</param>
        /// <param name="pbSkills">Attribute bar for skills</param>
        private void UpdateAttributeControls(
            CharacterInfo character,
            AttributesOptimizer.Remapping remapping,
            EveAttribute attrib,
            Label lb,
            AttributeBarControl pbBase,
            AttributeBarControl pbImplants,
            AttributeBarControl pbSkills)
        {
            // Compute base and effective attributes
            double effectiveAttribute = character.GetEffectiveAttribute(attrib, remapping.BestScratchpad);
            int oldBaseAttribute = character.GetBaseAttribute(attrib);
            int baseAttribute = oldBaseAttribute + (remapping.BestScratchpad.GetAttributeBonus(attrib) - remapping.BaseScratchpad.GetAttributeBonus(attrib));
            int implantsBonus = (int)character.getImplantValue(attrib);
            int skillsBonus = (int)effectiveAttribute - (baseAttribute + implantsBonus);

            // Update the label
            lb.Text = effectiveAttribute.ToString("##.##") + " (new : " + baseAttribute.ToString() + " ; old : " + oldBaseAttribute.ToString() + ")";

            // Update the bars
            pbBase.Value = baseAttribute;
            pbImplants.Value = implantsBonus;
            pbSkills.Value = skillsBonus;
        }

        /// <summary>
        /// Calculates new remapping from values of controls.
        /// </summary>
        private void Recalculate()
        {
            int perDiff = pbPERBase.Value - m_character.GetBaseAttribute(EveAttribute.Perception) +
                m_remapping.BaseScratchpad.GetAttributeBonus(EveAttribute.Perception);
            int wilDiff = pbWILBase.Value - m_character.GetBaseAttribute(EveAttribute.Willpower) +
                m_remapping.BaseScratchpad.GetAttributeBonus(EveAttribute.Willpower);
            int intDiff = pbINTBase.Value - m_character.GetBaseAttribute(EveAttribute.Intelligence) +
                m_remapping.BaseScratchpad.GetAttributeBonus(EveAttribute.Intelligence);
            int memDiff = pbMEMBase.Value -    m_character.GetBaseAttribute(EveAttribute.Memory) +
                m_remapping.BaseScratchpad.GetAttributeBonus(EveAttribute.Memory);
            int chaDiff = pbCHABase.Value - m_character.GetBaseAttribute(EveAttribute.Charisma) +
                m_remapping.BaseScratchpad.GetAttributeBonus(EveAttribute.Charisma);

            // Fill scratchpad with attributes from controls
            EveAttributeScratchpad manualScratchpad = new EveAttributeScratchpad();
            manualScratchpad.Reset(perDiff, wilDiff, intDiff, memDiff, chaDiff, 0);

            // Get remapping for provided attributes
            AttributesOptimizer.Remapping manualRemapping = AttributesOptimizer.OptimizeManually(m_character, m_remapping, manualScratchpad);
            UpdateControls(m_character, manualRemapping);

            if (AttributeChanged != null)
                AttributeChanged(this, manualRemapping);
        }

        #region Events

        /// <summary>
        /// Change of any attribute must be adjusted if there is no enough free points in the pool.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="deltaValue"></param>
        private void pbBase_ValueChanging(AttributeBarControl sender, ref int deltaValue)
        {
            // Adjust delta if there is no enough free points
            if (pbUnassigned.Value < deltaValue)
                deltaValue = pbUnassigned.Value;

            // Add/remove pionts from pool
            pbUnassigned.Value -= deltaValue;
        }

        /// <summary>
        /// Recalculate the time after change of an attribute.
        /// </summary>
        /// <param name="sender"></param>
        private void pb_ValueChanged(AttributeBarControl sender)
        {
            Recalculate();
        }

        /// <summary>
        /// Correct highlight if selected cell is inaccessable.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="highlightValue"></param>
        private void pbBase_Highlighting(AttributeBarControl sender, ref int highlightValue)
        {
            // Adjust possible highlight using free points in pool
            if (highlightValue - sender.Value > pbUnassigned.Value)
                highlightValue = sender.Value + pbUnassigned.Value;
        }

        /// <summary>
        /// Reset to original optimized remapping.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOptimize_Click(object sender, EventArgs e)
        {
            // Set all labels and bars to calculated optimized remap
            if (m_remapping.Point != null)
                m_remapping.Point.SetBaseAttributes(m_character, m_remapping.BaseScratchpad, m_remapping.BestScratchpad);
            UpdateControls(m_character, m_remapping);
            pbUnassigned.Value = 0;
            if (AttributeChanged != null)
                AttributeChanged(this, m_remapping);
        }

        /// <summary>
        /// Reset to remapping with current attributes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCurrent_Click(object sender, EventArgs e)
        {
            EveAttributeScratchpad baseScratchpad = m_remapping.BaseScratchpad.Clone();
            // Make unoptimized remap
            AttributesOptimizer.Remapping zeroRemapping = AttributesOptimizer.OptimizeManually(m_character, m_remapping, baseScratchpad);
            UpdateControls(m_character, zeroRemapping);
            pbUnassigned.Value = 0;
            if (AttributeChanged != null)
                AttributeChanged(this, zeroRemapping);
        }

        /// <summary>
        /// One of +/- buttons was pressed.
        /// Check is it possible to change requested attribute and do it if we can.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void attributeButton_Click(object sender, EventArgs e)
        {
            AttributeButtonControl button = (sender as AttributeButtonControl);
            if (button == null)
                return;

            if (button.AttributeBar == null)
                return;

            // Adjust delta
            int deltaValue = button.ValueChange;
            if (pbUnassigned.Value < deltaValue)
                deltaValue = pbUnassigned.Value;

            if (deltaValue < 0 && button.AttributeBar.Value <= button.AttributeBar.BaseValue)
                return;

            if (button.AttributeBar.Value + deltaValue < button.AttributeBar.BaseValue)
                deltaValue = Math.Max(button.AttributeBar.Value - button.AttributeBar.BaseValue, deltaValue);

            if (button.AttributeBar.Value + deltaValue > button.AttributeBar.MaxPoints)
                deltaValue = button.AttributeBar.MaxPoints - button.AttributeBar.Value;

            if (deltaValue == 0)
                return;

            button.AttributeBar.Value += deltaValue;
            pbUnassigned.Value -= deltaValue;
            Recalculate();
        }

        #endregion
    }
}
