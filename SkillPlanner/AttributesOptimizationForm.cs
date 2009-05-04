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
        public enum Strategy
        {
            RemappingPoints,
            OneYearPlan,
            Character
        }

        private readonly List<AttributesOptimizationControl> optimizationControls = new List<AttributesOptimizationControl>();
        private PlanOrderEditorControl m_planEditor;
        private CharacterInfo m_char;
        private Strategy m_strategy;
        private Plan m_plan;

        private Thread m_thread;
        private string m_description;

        private bool m_pluggableProvidesNewScratchpad;
        private EveAttributeScratchpad m_pluggableScratchpad;


        private AttributesOptimizationForm()
        {
            InitializeComponent();
        }

        public AttributesOptimizationForm(CharacterInfo character, Plan plan, Strategy strategy, string name, string description)
            : this()
        {
            m_char = character;
            m_strategy = strategy;
            m_description = description;
            m_plan = plan;
            this.Text = name;
            this.labelDescription.Text = description;
            this.tabControl.TabPages.Remove(this.tabNoResult);
            this.tabControl.TabPages.Remove(this.tabSummary);
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
            foreach (var ctl in this.optimizationControls)
            {
                ctl.CleanUp();
            }
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
            TimeSpan bestDuration = TimeSpan.Zero;
            AttributesOptimizer.Remapping remapping = null;
            List<AttributesOptimizer.Remapping> remappingList = null;

            switch (m_strategy)
            {
                case Strategy.Character:
                    remapping = AttributesOptimizer.OptimizeFromCharacter(m_char);
                    break;
                case Strategy.OneYearPlan:
                    remapping = AttributesOptimizer.OptimizeFromPlan(m_plan);
                    break;
                case Strategy.RemappingPoints:
                    remappingList = AttributesOptimizer.OptimizeFromPlanAndRemappingPoints(m_plan, out bestDuration);
                    break;
                default:
                    throw new NotImplementedException();

            }

            // Update the controls for every attribute
            this.Invoke((MethodInvoker)delegate 
            {
                // If the thread has been canceled, we stop right now to prevent an exception
                if (m_thread == null) return;

                // Hide the throbber and the waiting message
                this.throbber.State = Throbber.ThrobberState.Stopped;
                this.tabControl.TabPages.Remove(this.tabWait);

                // Update the attributes
                if (remapping != null)
                {
                    AddTabPage(remapping, "Result");

                    // Pluggable information for the planner window
                    m_pluggableProvidesNewScratchpad = true;
                    m_pluggableScratchpad = remapping.BestScratchpad;
                }
                else
                {
                    if (remappingList.Count == 0)
                    {
                        this.tabControl.TabPages.Add(tabNoResult);
                        tabNoResult.Focus();
                    }
                    else
                    {
                        this.tabControl.TabPages.Add(tabSummary);
                        tabSummary.Focus();
                    }

                    // Add pages
                    int index = 1;
                    TimeSpan lastRemap = TimeSpan.Zero;
                    foreach (var remap in remappingList)
                    {
                        AddTabPage(remap, "#" + index.ToString());
                        index++;

                        // Create the group
                        string text = remap.ToString(m_char) + " at " + Skill.TimeSpanToDescriptiveText(remap.Time, DescriptiveTextOptions.IncludeCommas);
                        ListViewGroup group = new ListViewGroup(text);
                        this.lvPoints.Groups.Add(group);

                        // Add five items, one for each attribute
                        AddItemForAttribute(remap, group, EveAttribute.Intelligence);
                        AddItemForAttribute(remap, group, EveAttribute.Perception);
                        AddItemForAttribute(remap, group, EveAttribute.Charisma);
                        AddItemForAttribute(remap, group, EveAttribute.Willpower);
                        AddItemForAttribute(remap, group, EveAttribute.Memory);

                        // Check there are at least one year between each remap
                        TimeSpan timeSinceLastRemap = remap.Time - lastRemap;
                        if (timeSinceLastRemap < TimeSpan.FromDays(365.0) && lastRemap != TimeSpan.Zero)
                        {
                            var item = new ListViewItem("The previous remap was only " + Skill.TimeSpanToDescriptiveText(timeSinceLastRemap, DescriptiveTextOptions.IncludeCommas) + " ago.", group);
                            item.ForeColor = Color.DarkRed;
                            lvPoints.Items.Add(item);
                        }
                        lastRemap = remap.Time;

                    }

                    // Add global informations
                    ListViewGroup globalGroup = new ListViewGroup("Global informations");
                    this.lvPoints.Groups.Add(globalGroup);

                    TimeSpan savedTime = TimeSpan.Zero;
                    foreach (var remap in remappingList)
                    {
                        savedTime += (remap.BaseDuration - remap.BestDuration);
                    }
                    this.lvPoints.Items.Add(new ListViewItem("Current time : " +
                        Skill.TimeSpanToDescriptiveText(savedTime + bestDuration, DescriptiveTextOptions.IncludeCommas), globalGroup));

                    if (savedTime != TimeSpan.Zero)
                    {
                        this.lvPoints.Items.Add(new ListViewItem("Optimized time : " +
                            Skill.TimeSpanToDescriptiveText(bestDuration, DescriptiveTextOptions.IncludeCommas), globalGroup));
                        this.lvPoints.Items.Add(new ListViewItem(
                            Skill.TimeSpanToDescriptiveText(savedTime, DescriptiveTextOptions.IncludeCommas) + 
                            " better than current", globalGroup));
                    }
                    else
                    {
                        this.lvPoints.Items.Add(new ListViewItem("Your attributes are already optimal", globalGroup));
                    }

                    // Pluggable information for the planner window
                    m_pluggableProvidesNewScratchpad = false;
                    m_pluggableScratchpad = new EveAttributeScratchpad();
                    this.columnHeader.Width = this.lvPoints.ClientSize.Width;
                }

                // Make everything visible
                this.tabWait.Hide();

                // Update the plan order's column
                if (m_planEditor != null && (remapping != null || remappingList.Count != 0))
                {
                    this.m_planEditor.ShowWithPluggable(this);
                }

                m_thread = null;
            });
        }

        private void AddItemForAttribute(AttributesOptimizer.Remapping remap, ListViewGroup group, EveAttribute attrib)
        {
            StringBuilder builder = new StringBuilder(attrib.ToString());
            int difference = remap.GetBaseAttributeDifference(attrib);

            // Add the list view item for this attribute
            string itemText = Plan.RemappingPoint.GetStringForAttribute(attrib, m_char, remap.BaseScratchpad, remap.BestScratchpad);
            lvPoints.Items.Add(new ListViewItem(itemText, group));
        }

        private void AddTabPage(AttributesOptimizer.Remapping remapping, string tabName)
        {
            var ctl = new AttributesOptimizationControl(m_char, remapping);
            this.optimizationControls.Add(ctl);
            ctl.Dock = DockStyle.Fill;
            TabPage page = new TabPage(tabName);
            page.Controls.Add(ctl);
            this.tabControl.TabPages.Add(page);
        }

        #region IPlanOrderPluggable Members
        public bool UseRemappingPointsForNew
        {
            get { return !m_pluggableProvidesNewScratchpad; }
        }

        public bool UseRemappingPointsForOld
        {
            get { return false; }
        }

        public EveAttributeScratchpad GetScratchpad(out bool isNew)
        {
            isNew = m_pluggableProvidesNewScratchpad;
            return m_pluggableScratchpad;
        }
        #endregion
    }
}
