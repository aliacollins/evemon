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
    public partial class AttributesOptimizationForm : Form
    {
        private CharacterInfo m_char;
        private Plan m_plan;

        private Thread m_thread;

        private const int minPerSkill = 5;
        private const int maxPerSkill = 10;

        public AttributesOptimizationForm()
        {
            InitializeComponent();
            this.attributesOptimizationControl.Visible = false;
            this.tblayoutSummary.Visible = false;
        }

        public AttributesOptimizationForm(CharacterInfo gci, Plan p)
            : this()
        {
            m_char = gci;
            m_plan = p;
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
            // Current time -- empty scratchpad is fine
            TimeSpan currentTime = m_plan.GetTotalTime(new EveAttributeScratchpad());

            // Best scratchpad
            TimeSpan bestTime;
            EveAttributeScratchpad bestScratchpad = Optimize(currentTime, out bestTime);

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
                this.attributesOptimizationControl.Update(m_char, bestScratchpad);

                // Update the current time control
                this.lbCurrentTime.Text = Skill.TimeSpanToDescriptiveText(currentTime, DescriptiveTextOptions.IncludeCommas);

                // Update the optimized time control
                this.lbOptimizedTime.Text = Skill.TimeSpanToDescriptiveText(bestTime, DescriptiveTextOptions.IncludeCommas);

                // Update the time benefit control
                if (bestTime < currentTime)
                {
                    this.lbGain.Text = Skill.TimeSpanToDescriptiveText(currentTime - bestTime, DescriptiveTextOptions.IncludeCommas) + " better than current";
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

                m_thread = null;
            });
        }

        private EveAttributeScratchpad Optimize(TimeSpan currentTime, out TimeSpan bestTime)
        {
            // Get the number of points currently spent into each
            // attribute
            int memBase = m_char.GetBaseAttribute(EveAttribute.Memory) - minPerSkill;
            int chaBase = m_char.GetBaseAttribute(EveAttribute.Charisma) - minPerSkill;
            int wilBase = m_char.GetBaseAttribute(EveAttribute.Willpower) - minPerSkill;
            int perBase = m_char.GetBaseAttribute(EveAttribute.Perception) - minPerSkill;
            int intBase = m_char.GetBaseAttribute(EveAttribute.Intelligence) - minPerSkill;

            // calculate the total points to spend in case CCP do something
            // odd with attributes in the future.
            int totalPoints = memBase + chaBase + wilBase + perBase + intBase;

            // Now, we have the points to spend, let's perform all the
            // combinations (less than 11^4 = 14,641)
            bestTime = currentTime;
            EveAttributeScratchpad bestScratchpad = new EveAttributeScratchpad();
            EveAttributeScratchpad tempScratchpad = new EveAttributeScratchpad();

            // PER
            for (int per = 0; per <= maxPerSkill; per++)
            {
                // WIL
                int maxWillpower = totalPoints - per;
                for (int will = 0; will <= maxWillpower && will <= maxPerSkill; will++)
                {
                    // INT
                    int maxIntelligence = maxWillpower - will;
                    for (int intell = 0; intell <= maxIntelligence && intell <= maxPerSkill; intell++)
                    {
                        // MEM
                        int maxMemory = maxIntelligence - intell;
                        for (int mem = 0; mem <= maxMemory && mem <= maxPerSkill; mem++)
                        {

                            // CHA
                            int cha = maxMemory - mem;

                            // Reject invalid combinations
                            if (cha <= maxPerSkill)
                            {
                                tempScratchpad.SetLearningLevelBonus(0);
                                tempScratchpad.SetAttributeBonus(EveAttribute.Perception, per - perBase);
                                tempScratchpad.SetAttributeBonus(EveAttribute.Willpower, will - wilBase);
                                tempScratchpad.SetAttributeBonus(EveAttribute.Intelligence, intell - intBase);
                                tempScratchpad.SetAttributeBonus(EveAttribute.Memory, mem - memBase);
                                tempScratchpad.SetAttributeBonus(EveAttribute.Charisma, cha - chaBase);

                                // Compute plan time
                                TimeSpan tempTime = m_plan.GetTotalTime(tempScratchpad);

                                // Compare it to the best time so far
                                if (tempTime.Ticks < bestTime.Ticks)
                                {
                                    bestTime = tempTime;
                                    bestScratchpad.SetAttributeBonus(EveAttribute.Perception, per - perBase);
                                    bestScratchpad.SetAttributeBonus(EveAttribute.Willpower, will - wilBase);
                                    bestScratchpad.SetAttributeBonus(EveAttribute.Intelligence, intell - intBase);
                                    bestScratchpad.SetAttributeBonus(EveAttribute.Memory, mem - memBase);
                                    bestScratchpad.SetAttributeBonus(EveAttribute.Charisma, cha - chaBase);
                                }

                                // If the thread has been canceled, we stop right now to (optional, optimization)
                                if (m_thread == null) return bestScratchpad;
                            }
                        }
                    }
                }
            }

            // Return the best scratchpad found
            return bestScratchpad;
        }
    }
}
