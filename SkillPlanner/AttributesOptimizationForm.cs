using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using System.Drawing.Imaging;
using System.Threading;

namespace EVEMon.SkillPlanner
{
    public partial class AttributesOptimizationForm : Form, IPlanOrderPluggable
    {
        private AttributesOptimizer.SkillTraining[] m_training;
        private EveAttributeScratchpad m_bestScratchPad;
        private CharacterInfo m_char;
        private TimeSpan m_maxDuration;
        private string m_description;
        private bool m_isPlan;
        private PlanOrderEditorControl m_planEditor;

        private Thread m_thread;

        private AttributesOptimizationForm()
        {
            InitializeComponent();
            this.attributesOptimizationControl.Visible = false;
            this.tblayoutSummary.Visible = false;
        }

        public AttributesOptimizationForm(CharacterInfo character, AttributesOptimizer.SkillTraining[] training, TimeSpan maxDuration, string name, string description, bool isPlan)
            : this()
        {
            m_char = character;
            m_training = training;
            m_maxDuration = maxDuration;
            m_description = description;
            m_isPlan = isPlan;
            this.Text = name;
            this.labelDescription.Text = description;
        }

        public PlanOrderEditorControl PlanEditor
        {
            get { return this.m_planEditor; }
            set { this.m_planEditor = value; }
        }

        protected override void OnClosed(EventArgs e)
        {
            // Stop the thread
            if (m_thread != null)
            {
                m_thread.Abort();
                m_thread = null;
            }

            // Clean up the brushes used by this control
            this.attributesOptimizationControl.CleanUp();
            this.m_planEditor = null;

            // Base call
            base.OnClosed(e);
        }

        private void AttributesOptimizationForm_Load(object sender, EventArgs e)
        {
            this.throbber.State = Throbber.ThrobberState.Rotating;

            m_thread = new Thread(new ThreadStart(Run));
            m_thread.Start();
        }

        private void Run()
        {
            // Compute best scratchpad
            TimeSpan bestTime, baseTime;
            m_bestScratchPad = AttributesOptimizer.Optimize(m_training, m_char, m_maxDuration, m_isPlan, out bestTime, out baseTime);

            // Update the controls for every attribute
            this.Invoke((MethodInvoker)delegate 
            {
                // If the thread has been canceled, we stop right now to prevent an exception
                if (m_thread == null) return;

                // Hide the throbber and the waiting message
                this.throbber.State = Throbber.ThrobberState.Stopped;
                this.throbber.Visible = false;
                this.lbWait.Visible = false;

                // Update the attributes
                this.attributesOptimizationControl.Update(m_char, m_bestScratchPad);

                // Update the current time control
                this.lbCurrentTime.Text = Skill.TimeSpanToDescriptiveText(baseTime, DescriptiveTextOptions.IncludeCommas);

                // Update the optimized time control
                this.lbOptimizedTime.Text = Skill.TimeSpanToDescriptiveText(bestTime, DescriptiveTextOptions.IncludeCommas);

                // Update the time benefit control
                if (bestTime < baseTime)
                {
                    this.lbGain.Text = Skill.TimeSpanToDescriptiveText(baseTime - bestTime, DescriptiveTextOptions.IncludeCommas) + " better than current";
                }
                else
                {
                    this.lbGain.Text = "Your skills are already optimized";
                }

                // A plan may not have a years worth of skills in it,
                // only fair to warn the user
                this.lbWarning.Visible = bestTime < new TimeSpan(365,0,0,0);

                // Make everything visible
                this.attributesOptimizationControl.Visible = true;
                this.tblayoutSummary.Visible = true;

                // Update the plan order's column
                if (m_planEditor != null)
                {
                    this.m_planEditor.ShowWithPluggable(this);
                }

                m_thread = null;
            });
        }


        #region IPlanOrderPluggable Members
        public int getEffectiveAttributeWithoutLearning(EveAttribute attrib)
        {
            return this.m_bestScratchPad.GetAttributeBonus(attrib) + (int)m_char.GetEffectiveAttribute(attrib, null, false, true);
        }
        #endregion
    }
}
