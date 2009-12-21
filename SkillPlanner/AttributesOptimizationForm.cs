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
    /// <summary>
    /// Allows to wiew and change attribute remappings.
    /// </summary>
    public partial class AttributesOptimizationForm : Form, IPlanOrderPluggable
    {
        /// <summary>
        /// Remapping strategy.
        /// </summary>
        public enum Strategy
        {
            /// <summary>
            /// Stratagy based on remapping points.
            /// </summary>
            RemappingPoints,
            /// <summary>
            /// Strategy based on the first year from a plan.
            /// </summary>
            OneYearPlan,
            /// <summary>
            /// Strategy based on already trained skills.
            /// </summary>
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

        private TimeSpan bestDuration;
        private Dictionary<AttributesOptimizationControl, AttributesOptimizer.Remapping> remappingDictionary;

        /// <summary>
        /// Internal constructor.
        /// </summary>
        private AttributesOptimizationForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes new instance of the EVEMon.SkillPlanner.AttributesOptimizationForm.
        /// </summary>
        /// <param name="character">Character information</param>
        /// <param name="plan">Plan to optimize for</param>
        /// <param name="strategy">Optimization strategy</param>
        /// <param name="name">Title of this form</param>
        /// <param name="description">Description of the optimization operation</param>
        public AttributesOptimizationForm(CharacterInfo character, Plan plan, Strategy strategy, string name, string description)
            : this()
        {
            m_char = character;
            m_strategy = strategy;
            m_description = description;
            m_plan = plan;
            this.Text = name;
            this.labelDescription.Text = description;
        }

        /// <summary>
        /// Gets or sets an EVEMon.SkillPlanner.PlanOrderEditorControl.
        /// </summary>
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

            this.m_planEditor = null;

            // Base call
            base.OnClosed(e);
        }

        /// <summary>
        /// Start throbber and thread.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AttributesOptimizationForm_Load(object sender, EventArgs e)
        {
            this.throbber.State = Throbber.ThrobberState.Rotating;

            m_thread = new Thread(new ThreadStart(Run));
            m_thread.Start();

            lvPoints.Font = FontHelper.GetDefaultFont(9F, FontStyle.Regular);
        }

        /// <summary>
        /// Starts optimization.
        /// </summary>
        private void Run()
        {
            // Compute best scratchpad
            bestDuration = TimeSpan.Zero;
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
            this.Invoke(new UpdateFormDelegate(UpdateForm), new object[] { remapping, remappingList });
        }

        /// <summary>
        /// Delegate for UpdateForm() method.
        /// </summary>
        /// <param name="remapping">An EVEMon.Common.AttributesOptimizer.Remapping object</param>
        /// <param name="remappingList">List of remappings</param>
        private delegate void UpdateFormDelegate(AttributesOptimizer.Remapping remapping, List<AttributesOptimizer.Remapping> remappingList);

        /// <summary>
        /// Updates controls on the form.
        /// </summary>
        /// <param name="remapping">An EVEMon.Common.AttributesOptimizer.Remapping object</param>
        /// <param name="remappingList">List of remappings</param>
        private void UpdateForm(AttributesOptimizer.Remapping remapping, List<AttributesOptimizer.Remapping> remappingList)
        {
            // If the thread has been canceled, we stop right now to prevent an exception
            if (m_thread == null)
                return;

            // Hide the throbber and the waiting message
            this.throbber.State = Throbber.ThrobberState.Stopped;
            this.panelWait.Visible = false;

            // Update the attributes
            if (remapping != null)
            {
                UpdateForRemapping(remapping);
            }
            else
            {
                UpdateForRemappingList(remappingList);
            }

            // Update the plan order's column
            if (m_planEditor != null && (remapping != null || remappingList.Count != 0))
            {
                this.m_planEditor.ShowWithPluggable(this);
            }

            m_thread = null;
        }

        /// <summary>
        /// Updates controls on the form by data from remapping.
        /// </summary>
        /// <param name="remapping">A EVEMon.Common.AttributesOptimizer.Remapping object</param>
        private void UpdateForRemapping(AttributesOptimizer.Remapping remapping)
        {
            var ctl = CreateAttributesOptimizationControl(remapping);
            this.Controls.Add(ctl);
            ctl.BringToFront();

            // Pluggable information for the planner window
            m_pluggableProvidesNewScratchpad = true;
            m_pluggableScratchpad = remapping.BestScratchpad;
        }

        /// <summary>
        /// Updates controls on the form by data from remapping list. Creates required tab pages.
        /// </summary>
        /// <param name="remappingList">List of remappings</param>
        private void UpdateForRemappingList(List<AttributesOptimizer.Remapping> remappingList)
        {
            if (remappingList.Count == 0)
            {
                panelNoResult.Visible = true;
            }
            else
            {
                this.tabControl.Visible = true;
                tabSummary.Focus();
            }

            // Summary page
            UpdateSummaryInformation(remappingList);

            // Page for each remapping
            remappingDictionary = new Dictionary<AttributesOptimizationControl, AttributesOptimizer.Remapping>();
            int index = 1;
            foreach (var remap in remappingList)
            {
                AddTabPage(remap, "#" + index.ToString());
                index++;
            }
        }

        /// <summary>
        /// Adds item in the list.
        /// </summary>
        /// <param name="remap">Remapping to add from</param>
        /// <param name="group">Group to add into</param>
        /// <param name="attrib">Attribute for which to add</param>
        private void AddItemForAttribute(AttributesOptimizer.Remapping remap, ListViewGroup group, EveAttribute attrib)
        {
            StringBuilder builder = new StringBuilder(attrib.ToString());
            int difference = remap.GetBaseAttributeDifference(attrib);

            // Add the list view item for this attribute
            string itemText = Plan.RemappingPoint.GetStringForAttribute(attrib, m_char, remap.BaseScratchpad, remap.BestScratchpad);
            lvPoints.Items.Add(new ListViewItem(itemText, group));
        }

        /// <summary>
        /// Updates information on summary page.
        /// </summary>
        /// <param name="remappings">List of remappings</param>
        private void UpdateSummaryInformation(IEnumerable<AttributesOptimizer.Remapping> remappings)
        {
            this.lvPoints.Items.Clear();
            this.lvPoints.Groups.Clear();

            // Add global informations
            ListViewGroup globalGroup = new ListViewGroup("Global informations");
            this.lvPoints.Groups.Add(globalGroup);

            TimeSpan savedTime = TimeSpan.Zero;
            foreach (var remap in remappings)
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

            // Notify plan updated
            var lvi = new ListViewItem("Your plan has been updated.", globalGroup);
            lvi.Font = FontHelper.GetFont(lvPoints.Font, FontStyle.Bold);
            this.lvPoints.Items.Add(lvi);

            TimeSpan lastRemap = TimeSpan.Zero;
            foreach (var remap in remappings)
            {
                AddSummaryForRemapping(remap, lastRemap);
                lastRemap = remap.Time;
            }

            // Pluggable information for the planner window
            m_pluggableProvidesNewScratchpad = false;
            m_pluggableScratchpad = new EveAttributeScratchpad();
            this.columnHeader.Width = this.lvPoints.ClientSize.Width;
        }

        /// <summary>
        /// Adds summury information for given remapping.
        /// </summary>
        /// <param name="remap">Remapping object</param>
        /// <param name="lastRemap">Time of previous remapping</param>
        void AddSummaryForRemapping(AttributesOptimizer.Remapping remap, TimeSpan lastRemap)
        {
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
        }

        /// <summary>
        /// Creates EVEMon.SkillPlanner.AttributesOptimizationControl for a given remapping.
        /// </summary>
        /// <param name="remapping">An EVEMon.CommonAttributesOptimizer.Remapping object</param>
        /// <returns>Created EVEMon.SkillPlanner.AttributesOptimizationControl</returns>
        private AttributesOptimizationControl CreateAttributesOptimizationControl(AttributesOptimizer.Remapping remapping)
        {
            var ctl = new AttributesOptimizationControl(m_char, remapping);
            ctl.AttributeChanged += new AttributeChangedHandler(AttributesOptimizationControl_AttributeChanged);
            this.optimizationControls.Add(ctl);
            ctl.Dock = DockStyle.Fill;

            return ctl;
        }

        /// <summary>
        /// Adds tap page with EVEMon.SkillPlanner.AttributesOptimizationControl created for the remapping.
        /// </summary>
        /// <param name="remapping">An EVEMon.CommonAttributesOptimizer.Remapping object</param>
        /// <param name="tabName">Title of the page</param>
        private void AddTabPage(AttributesOptimizer.Remapping remapping, string tabName)
        {
            var ctl = CreateAttributesOptimizationControl(remapping);
            TabPage page = new TabPage(tabName);
            page.Controls.Add(ctl);
            this.tabControl.TabPages.Add(page);
            remappingDictionary.Add(ctl, remapping);
        }

        /// <summary>
        /// Racalculating plan and summary page after change of an EVEMon.SkillPlanner.AttributesOptimizationControl.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="remapping"></param>
        void AttributesOptimizationControl_AttributeChanged(AttributesOptimizationControl control, AttributesOptimizer.Remapping remapping)
        {
            // Update the plan order's column
            if (m_planEditor != null)
            {
                switch (m_strategy)
                {
                    case Strategy.OneYearPlan:
                    case Strategy.Character:
                        m_pluggableScratchpad = remapping.BestScratchpad.Clone();
                        break;

                    case Strategy.RemappingPoints:
                        remappingDictionary[control] = remapping;
                        UpdateSummaryInformation(remappingDictionary.Values);
                        break;
                }
                this.m_planEditor.ShowWithPluggable(this);
            }
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
            return m_pluggableScratchpad.Clone();
        }
        #endregion
    }
}
