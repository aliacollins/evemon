using System;
using System.Drawing;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;

namespace EVEMon.SkillPlanner
{
    /// <summary>
    /// This controls allows the user to see how its plan's comutation times would change wth different implants.
    /// </summary>
    public partial class ImplantCalculator : EVEMonForm, IPlanOrderPluggable
    {
        private bool m_isUpdating = false;
        private Character m_baseCharacter;
        private BaseCharacter m_character;
        private PlanEditorControl m_planEditor;
        private Plan m_plan;

        /// <summary>
        /// Default constructor for designer.
        /// </summary>
        public ImplantCalculator()
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
            m_character = plan.Character;
            m_baseCharacter = (Character)plan.Character;
        }

        /// <summary>
        /// Sets the owner control
        /// </summary>
        public PlanEditorControl PlanEditor
        {
            set { m_planEditor = value; }
        }

        /// <summary>
        /// On load, update the controls states.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            UpdateContent();
 	        base.OnLoad(e);
        }

        /// <summary>
        /// Update all the numeric boxes
        /// </summary>
        private void UpdateContent()
        {
            m_isUpdating = true;
            nudCharisma.Value = m_character.Charisma.PreLearningEffectiveAttribute;
            nudWillpower.Value = m_character.Willpower.PreLearningEffectiveAttribute;
            nudIntelligence.Value = m_character.Intelligence.PreLearningEffectiveAttribute;
            nudPerception.Value = m_character.Perception.PreLearningEffectiveAttribute;
            nudMemory.Value = m_character.Memory.PreLearningEffectiveAttribute;
            m_isUpdating = false;

            UpdateTimes();
        }

        /// <summary>
        /// Updates the labels on the right of the numeric box.
        /// </summary>
        /// <param name="attrib"></param>
        /// <param name="myValue"></param>
        /// <param name="lblAdjust"></param>
        /// <param name="lblEffective"></param>
        private void UpdateAttributeLabels(EveAttribute attrib, int myValue, Label lblAdjust, Label lblEffective)
        {
            int baseAttr = m_character[attrib].PreLearningEffectiveAttribute - m_character[attrib].ImplantBonus;
            int adjust = myValue - baseAttr;

            if (adjust >= 0)
            {
                lblAdjust.ForeColor = SystemColors.ControlText;
                lblAdjust.Text = "+" + adjust.ToString();
            }
            else
            {
                lblAdjust.ForeColor = Color.Red;
                lblAdjust.Text = adjust.ToString();
            }

            lblEffective.Text = m_character[attrib].EffectiveValue.ToString("#0.00");
        }

        /// <summary>
        /// Update all the times on the right pane (base time, best time, etc).
        /// </summary>
        private void UpdateTimes()
        {
            if (m_isUpdating) return;
            if (m_planEditor != null) m_planEditor.ShowWithPluggable(this);

            // Current (with implants)
            TimeSpan currentSpan = UpdateTimesForCharacter(m_character, lblCurrentSpan, lblCurrentDate);

            // Current (without implants)
            var noneImplantSet = m_baseCharacter.ImplantSets.None;
            TimeSpan baseSpan = UpdateTimesForCharacter(m_baseCharacter.After(noneImplantSet), lblBaseSpan, lblBaseDate);

            // This
            var scratchpad = CreateModifiedScratchpad();
            TimeSpan thisSpan = UpdateTimesForCharacter(scratchpad, lblThisSpan, lblThisDate);

            // Are the new attributes better than current (without implants) ?
            if (thisSpan > baseSpan)
            {
                lblComparedToBase.ForeColor = Color.Red;
                lblComparedToBase.Text = Skill.TimeSpanToDescriptiveText(thisSpan - baseSpan,DescriptiveTextOptions.IncludeCommas) 
                    + " slower than current base";
            }
            else
            {
                lblComparedToBase.ForeColor = SystemColors.ControlText;
                lblComparedToBase.Text = Skill.TimeSpanToDescriptiveText(baseSpan - thisSpan, DescriptiveTextOptions.IncludeCommas) 
                    + " better than current base";
            }

            // Are the new attributes better than current (with implants) ?
            if (thisSpan > currentSpan)
            {
                lblComparedToCurrent.ForeColor = Color.Red;
                lblComparedToCurrent.Text = Skill.TimeSpanToDescriptiveText(thisSpan - currentSpan, DescriptiveTextOptions.IncludeCommas) 
                    + " slower than current";
            }
            else
            {
                lblComparedToCurrent.ForeColor = SystemColors.ControlText;
                lblComparedToCurrent.Text = Skill.TimeSpanToDescriptiveText(currentSpan - thisSpan, DescriptiveTextOptions.IncludeCommas) 
                    + " better than current";
            }
        }

        /// <summary>
        /// Update the times labels from the given character
        /// </summary>
        /// <param name="character"></param>
        /// <param name="lblSpan"></param>
        /// <param name="lblDate"></param>
        /// <returns></returns>
        private TimeSpan UpdateTimesForCharacter(BaseCharacter character, Label lblSpan, Label lblDate)
        {
            
            TimeSpan ts = character.GetTrainingTimeToMultipleSkills(m_plan);
            DateTime dt = DateTime.Now + ts;

            lblSpan.Text = Skill.TimeSpanToDescriptiveText(ts, DescriptiveTextOptions.IncludeCommas);
            lblDate.Text = dt.ToString();

            return ts;
        }

        #region Controls events
        /// <summary>
        /// When the intelligence numeric box changed, we update the attributes labels and the times on the right pane.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nudIntelligence_ValueChanged(object sender, EventArgs e)
        {
            UpdateAttributeLabels(EveAttribute.Intelligence, Convert.ToInt32(nudIntelligence.Value),
                            lblAdjustIntelligence, lblEffectiveIntelligence);
            UpdateTimes();
        }

        /// <summary>
        /// When the charisma numeric box changed, we update the attributes labels and the times on the right pane.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nudCharisma_ValueChanged(object sender, EventArgs e)
        {
            UpdateAttributeLabels(EveAttribute.Charisma, Convert.ToInt32(nudCharisma.Value),
                            lblAdjustCharisma, lblEffectiveCharisma);
            UpdateTimes();
        }

        /// <summary>
        /// When the perception numeric box changed, we update the attributes labels and the times on the right pane.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nudPerception_ValueChanged(object sender, EventArgs e)
        {
            UpdateAttributeLabels(EveAttribute.Perception, Convert.ToInt32(nudPerception.Value),
                            lblAdjustPerception, lblEffectivePerception);
            UpdateTimes();
            if (m_planEditor != null) m_planEditor.ShowWithPluggable(this);
        }

        /// <summary>
        /// When the memory numeric box changed, we update the attributes labels and the times on the right pane.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nudMemory_ValueChanged(object sender, EventArgs e)
        {
            UpdateAttributeLabels(EveAttribute.Memory, Convert.ToInt32(nudMemory.Value),
                            lblAdjustMemory, lblEffectiveMemory);
            UpdateTimes();
        }

        /// <summary>
        /// When the willpower numeric box changed, we update the attributes labels and the times on the right pane.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nudWillpower_ValueChanged(object sender, EventArgs e)
        {
            UpdateAttributeLabels(EveAttribute.Willpower, Convert.ToInt32(nudWillpower.Value),
                            lblAdjustWillpower, lblEffectiveWillpower);
            UpdateTimes();
        }

        /// <summary>
        /// When the "load attributes" menu is opening, we add the items.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnLoadAtts_DropDownOpening(object sender, EventArgs e)
        {
            mnLoadAtts.DropDownItems.Clear();

            // Add the "none" menu
            var item = mnLoadAtts.DropDownItems.Add("none");
            item.Click += new EventHandler(implantSetMenuitem_Click);

            // Add the menus for the sets
            foreach (var set in m_baseCharacter.ImplantSets)
            {
                item = mnLoadAtts.DropDownItems.Add(set.Name);
                item.Click += new EventHandler(implantSetMenuitem_Click);
                item.Tag = set;
            }
        }

        /// <summary>
        /// When an implant set menu item is clicked, we update the member for the current character.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void implantSetMenuitem_Click(object sender, EventArgs e)
        {
            // Update the character to a scratchpad using the implant set attacthed to the sender menu as its tag.
            var menu = (ToolStripItem)sender;
            var set = menu.Tag as ImplantSet;
            if (set == null)  m_character = m_baseCharacter;
            else m_character = m_baseCharacter.After(set);

            UpdateContent();
        }

        /// <summary>
        /// Creates a scrahcpad with the new implants values.
        /// </summary>
        /// <returns></returns>
        private CharacterScratchpad CreateModifiedScratchpad()
        {
            // Creates a scratchpad with new implants
            CharacterScratchpad scratchpad = new CharacterScratchpad(m_character);

            scratchpad.Memory.ImplantBonus += (int)this.nudMemory.Value - m_character.Memory.PreLearningEffectiveAttribute;
            scratchpad.Charisma.ImplantBonus += (int)this.nudCharisma.Value - m_character.Charisma.PreLearningEffectiveAttribute;
            scratchpad.Intelligence.ImplantBonus += (int)this.nudIntelligence.Value - m_character.Intelligence.PreLearningEffectiveAttribute;
            scratchpad.Perception.ImplantBonus += (int)this.nudPerception.Value - m_character.Perception.PreLearningEffectiveAttribute;
            scratchpad.Willpower.ImplantBonus += (int)this.nudWillpower.Value - m_character.Willpower.PreLearningEffectiveAttribute;

            return scratchpad;
        }
        #endregion


        #region IPlanOrderPluggable implementation
        /// <summary>
        /// Updates the statistics for the plan editor.
        /// </summary>
        /// <param name="plan"></param>
        /// <param name="areRemappingPointsActive"></param>
        public void UpdateStatistics(BasePlan plan, out bool areRemappingPointsActive)
        {
            areRemappingPointsActive = true;

            var scratchpad = CreateModifiedScratchpad();
            plan.UpdateStatistics(scratchpad, true, true);
            plan.UpdateOldTrainingTimes();
        }
        #endregion
    }
}