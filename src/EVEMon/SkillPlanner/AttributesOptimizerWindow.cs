using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using EVEMon.Common.Controls;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Factories;
using EVEMon.Common.Helpers;
using EVEMon.Common.Interfaces;
using EVEMon.Common.Models;
using EVEMon.Common.Threading;

namespace EVEMon.SkillPlanner
{
    /// <summary>
    /// Allows to wiew and change attribute remappings.
    /// </summary>
    public partial class AttributesOptimizerWindow : EVEMonForm, IPlanOrderPluggable
    {
        private readonly Dictionary<AttributesOptimizerControl, RemappingResult>
            m_remappingDictionary = new Dictionary<AttributesOptimizerControl, RemappingResult>();

        private readonly BaseCharacter m_baseCharacter;
        private readonly Character m_character;
        private readonly AttributeOptimizationStrategy m_strategy;
        private readonly BasePlan m_plan;
        private readonly string m_description;

        private CharacterScratchpad m_statisticsScratchpad;
        private bool m_areRemappingPointsActive;

        // Variables for manual edition of a plan
        private RemappingPoint m_manuallyEditedRemappingPoint;
        private RemappingResult m_remapping;

        /// <summary>
        /// Constructor for designer.
        /// </summary>
        private AttributesOptimizerWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor for use in code when optimizing remapping.
        /// </summary>
        /// <param name="character">Character information</param>
        /// <param name="plan">Plan to optimize for</param>
        /// <param name="strategy">Optimization strategy</param>
        /// <param name="name">Title of this form</param>
        /// <param name="description">Description of the optimization operation</param>
        public AttributesOptimizerWindow(Character character, BasePlan plan, AttributeOptimizationStrategy strategy, string name,
                                          string description)
            : this()
        {
            if (character == null)
                throw new ArgumentNullException("character");

            if (plan == null)
                throw new ArgumentNullException("plan");

            m_character = character;
            m_baseCharacter = character.After(plan.ChosenImplantSet);
            m_strategy = strategy;
            m_plan = plan;
            m_description = description;
            Text = name;
        }

        /// <summary>
        /// Constructor for use in code when the user wants to manually edit a remapping point.
        /// </summary>
        /// <param name="character">Character information</param>
        /// <param name="plan">Plan to optimize for</param>
        /// <param name="point">The point.</param>
        public AttributesOptimizerWindow(Character character, Plan plan, RemappingPoint point)
            : this()
        {
            if (character == null)
                throw new ArgumentNullException("character");

            if (plan == null)
                throw new ArgumentNullException("plan");

            m_plan = plan;
            m_character = character;
            m_baseCharacter = character.After(plan.ChosenImplantSet);
            m_manuallyEditedRemappingPoint = point;
            m_strategy = AttributeOptimizationStrategy.ManualRemappingPointEdition;
            m_description = "Manual editing of a remapping point";
            Text = $"Remapping point manual editing ({plan.Name})";
        }

        /// <summary>
        /// </summary>
        public sealed override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        /// <summary>
        /// Gets or sets a <see cref="PlanEditorControl"/>.
        /// </summary>
        internal PlanEditorControl PlanEditor { private get; set; }

        /// <summary>
        /// On load, restores the window rectangle from the settings.
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            lvPoints.Font = FontFactory.GetFont("Arial", 9F);
            throbber.State = ThrobberState.Rotating;

            await TaskHelper.RunCPUBoundTaskAsync(() => Run());
        }

        ///// <summary>
        ///// Raises the <see cref="E:System.Windows.Forms.Form.Closed" /> event.
        ///// </summary>
        ///// <param name="e">The <see cref="T:System.EventArgs" /> that contains the event data.</param>
        //protected override void OnClosed(EventArgs e)
        //{
        //    PlanEditor = null;

        //    // Base call
        //    base.OnClosed(e);
        //}

        /// <summary>
        /// Starts optimization.
        /// </summary>
        /// <param name="update">if set to <c>true</c> [update].</param>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        private void Run(bool update = false)
        {
            // Compute best scratchpad
            RemappingResult remapping = null;
            ICollection<RemappingResult> remappingList = null;

            switch (m_strategy)
            {
                case AttributeOptimizationStrategy.ManualRemappingPointEdition:
                    m_areRemappingPointsActive = true;
                    if (update)
                    {
                        remapping = m_remapping;
                        m_manuallyEditedRemappingPoint = remapping.Point.Clone();
                    }
                    else
                    {
                        remapping = AttributesOptimizer.GetResultsFromRemappingPoints(m_plan).Single(
                            x => x.Point == m_manuallyEditedRemappingPoint);
                        m_manuallyEditedRemappingPoint = m_manuallyEditedRemappingPoint.Clone();
                        m_remapping = remapping;
                    }
                    remapping.Optimize(TimeSpan.MaxValue);
                    break;
                case AttributeOptimizationStrategy.Character:
                    m_areRemappingPointsActive = false;
                    remapping = AttributesOptimizer.OptimizeFromCharacter(m_character, m_plan);
                    break;
                case AttributeOptimizationStrategy.OneYearPlan:
                    m_areRemappingPointsActive = false;
                    remapping = AttributesOptimizer.OptimizeFromFirstYearOfPlan(m_plan);
                    break;
                case AttributeOptimizationStrategy.RemappingPoints:
                    m_areRemappingPointsActive = true;
                    remappingList = AttributesOptimizer.OptimizeFromPlanAndRemappingPoints(m_plan);
                    break;
                default:
                    throw new NotImplementedException();
            }

            // Update the controls for every attribute
            Dispatcher.Invoke(() => UpdateForm(remapping, remappingList));
        }

        /// <summary>
        /// Updates controls on the form.
        /// </summary>
        /// <param name="remapping">An <see cref="RemappingResult"/> object</param>
        /// <param name="remappingList">List of remappings</param>
        private void UpdateForm(RemappingResult remapping, ICollection<RemappingResult> remappingList)
        {
            // Update the attributes
            if (remapping != null)
            {
                m_statisticsScratchpad = remapping.BestScratchpad.Clone();
                UpdateForRemapping(remapping);
            }
            else
                UpdateForRemappingList(remappingList);

            // Update the plan order's column
            if ((remapping != null) || (remappingList.Count != 0))
                PlanEditor?.ShowWithPluggable(this);

            // Hide the throbber and the waiting message
            panelWait.Hide();
        }

        /// <summary>
        /// Updates the UI once the computation has been done (for whole plan or character from birth)
        /// </summary>
        /// <param name="remapping"></param>
        private void UpdateForRemapping(RemappingResult remapping)
        {          
            // Create control
            AttributesOptimizerControl ctrl = CreateAttributesOptimizationControl(remapping, m_description);
            Controls.Add(ctrl);

            IList<AttributesOptimizerControl> optControls = Controls.OfType<AttributesOptimizerControl>().ToList();

            if (optControls.Count == 1)
                return;

            Controls.RemoveAt(Controls.IndexOf(optControls.First()));
        }

        /// <summary>
        /// Updates the UI once the computations with remapping points strategy have been done.
        /// </summary>
        /// <param name="remappingList">The remapping list.</param>
        private void UpdateForRemappingList(ICollection<RemappingResult> remappingList)
        {
            // Display "no result" or "summary"
            if (remappingList.Count == 0)
            {
                panelNoResult.Show();
                return;
            }

            tabControl.Controls.Clear();

            // Adds a tab page for the summary
            tabControl.Controls.Add(tabSummary);

            // Updates the summary informations
            UpdateSummaryInformation(remappingList);

            // Adds a tab page for every remapping
            int index = 1;
            foreach (RemappingResult remap in remappingList)
            {
                AddTabPage(remap, "#" + index++, m_description);
            }

            tabControl.Show();
            tabSummary.Focus();
        }

        /// <summary>
        /// Updates information in summary page.
        /// </summary>
        /// <param name="remappingList">List of remappings</param>
        private void UpdateSummaryInformation(ICollection<RemappingResult> remappingList)
        {
            TimeSpan baseDuration = m_plan.GetTotalTime(m_character.After(m_plan.ChosenImplantSet), false);
            lvPoints.Items.Clear();

            // Add global informations
            ListViewGroup globalGroup = new ListViewGroup("Global informations");
            lvPoints.Groups.Add(globalGroup);

            TimeSpan savedTime = remappingList.Aggregate(TimeSpan.Zero,
                                                         (current, remap) =>
                                                         current.Add(remap.BaseDuration.Subtract(remap.BestDuration)));

            lvPoints.Items.Add(new ListViewItem(
                $"Current time : {baseDuration.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas)}", globalGroup));

            if (savedTime != TimeSpan.Zero)
            {
                lvPoints.Items.Add(
                    new ListViewItem(
                        $"Optimized time : {baseDuration.Subtract(savedTime).ToDescriptiveText(DescriptiveTextOptions.IncludeCommas)}",
                        globalGroup));

                if (savedTime < TimeSpan.Zero)
                {
                    ListViewItem savedTimeItem = lvPoints.Items.Add(
                        new ListViewItem(
                            $"{(-savedTime).ToDescriptiveText(DescriptiveTextOptions.IncludeCommas)} slower than current.",
                                         globalGroup));
                    savedTimeItem.ForeColor = Color.DarkRed;
                }
                else
                {
                    ListViewItem savedTimeItem = lvPoints.Items.Add(
                        new ListViewItem(
                            $"{savedTime.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas)} better than current.",
                                         globalGroup));
                    savedTimeItem.ForeColor = Color.DarkGreen;
                }
            }
            else
                lvPoints.Items.Add(new ListViewItem("Your attributes are already optimal.", globalGroup));

            // Notify plan updated
            ListViewItem lvi = new ListViewItem("Your plan has been updated.", globalGroup)
                                   { Font = FontFactory.GetFont(lvPoints.Font, FontStyle.Bold) };
            lvPoints.Items.Add(lvi);

            // Add pages and summary informations
            TimeSpan lastRemap = TimeSpan.Zero;
            foreach (RemappingResult remap in remappingList)
            {
                AddSummaryForRemapping(remap, ref lastRemap);
            }

            columnHeader.Width = lvPoints.ClientSize.Width;
        }

        /// <summary>
        /// Adds summary information for given remapping.
        /// </summary>
        /// <param name="remap">Remapping object</param>
        /// <param name="lastRemap">Time of previous remapping</param>
        private void AddSummaryForRemapping(RemappingResult remap, ref TimeSpan lastRemap)
        {
            // Create the group
            string text = $"{remap} at {remap.StartTime.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas)}";
            ListViewGroup group = new ListViewGroup(text);
            lvPoints.Groups.Add(group);

            // Check there are at least one year between each remap
            TimeSpan timeSinceLastRemap = remap.StartTime.Subtract(lastRemap);
            if (timeSinceLastRemap < TimeSpan.FromDays(365) && remap.StartTime != TimeSpan.Zero)
            {
                ListViewItem item = lvPoints.Items.Add(
                    new ListViewItem(
                        $"The previous remap point was only {timeSinceLastRemap.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas)} ago.",
                        group));
                item.ForeColor = Color.DarkRed;
            }

            lastRemap = remap.StartTime;

            // Add five items, one for each attribute
            AddItemForAttribute(remap, group, EveAttribute.Intelligence);
            AddItemForAttribute(remap, group, EveAttribute.Perception);
            AddItemForAttribute(remap, group, EveAttribute.Charisma);
            AddItemForAttribute(remap, group, EveAttribute.Willpower);
            AddItemForAttribute(remap, group, EveAttribute.Memory);
        }

        /// <summary>
        /// Adds the list item for the given attribute.
        /// </summary>
        /// <param name="remap"></param>
        /// <param name="group"></param>
        /// <param name="attrib"></param>
        private void AddItemForAttribute(RemappingResult remap, ListViewGroup group,
                                         EveAttribute attrib)
        {
            // Add the list view item for this attribute
            string itemText = RemappingPoint.GetStringForAttribute(attrib, remap.BaseScratchpad, remap.BestScratchpad);
            lvPoints.Items.Add(new ListViewItem(itemText, group));
        }

        /// <summary>
        /// Adds the tab page for the given remapping
        /// </summary>
        /// <param name="remapping">The remapping.</param>
        /// <param name="tabName">Name of the tab.</param>
        /// <param name="description">The description.</param>
        private void AddTabPage(RemappingResult remapping, string tabName, string description)
        {
            AttributesOptimizerControl ctl = CreateAttributesOptimizationControl(remapping, description);

            if (ctl == null)
                return;

            m_remappingDictionary[ctl] = remapping;

            TabPage tempPage = null;
            try
            {
                tempPage = new TabPage(tabName);
                tempPage.Controls.Add(ctl);

                TabPage page = tempPage;
                tempPage = null;

                tabControl.TabPages.Add(page);
            }
            finally
            {
                tempPage?.Dispose();
            }
        }

        /// <summary>
        /// Creates a <see cref="AttributesOptimizerControl"/> for a given remapping.
        /// </summary>
        /// <param name="remapping">The remapping object to represents.</param>
        /// <param name="description">The description.</param>
        /// <returns>The created control.</returns>
        private AttributesOptimizerControl CreateAttributesOptimizationControl(RemappingResult remapping, string description)
        {
            AttributesOptimizerControl control;
            AttributesOptimizerControl ctl = null;
            try
            {
                ctl = new AttributesOptimizerControl(m_character, m_plan, remapping, description);
                ctl.AttributeChanged += AttributesOptimizationControl_AttributeChanged;

                // For a manually edited point, we initialize the control with the attributes from the current remapping point
                if (m_strategy == AttributeOptimizationStrategy.ManualRemappingPointEdition &&
                    m_manuallyEditedRemappingPoint.Status == RemappingPointStatus.UpToDate)
                    ctl.UpdateValuesFrom(m_manuallyEditedRemappingPoint);

                control = ctl;
                ctl = null;
            }
            finally
            {
                ctl?.Dispose();
            }

            return control;
        }

        /// <summary>
        /// Racalculating plan and summary page after change of a <see cref="AttributesOptimizerControl"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="AttributeChangedEventArgs"/> instance containing the event data.</param>
        private void AttributesOptimizationControl_AttributeChanged(object sender, AttributeChangedEventArgs e)
        {
            AttributesOptimizerControl control = (AttributesOptimizerControl)sender;

            if (m_strategy == AttributeOptimizationStrategy.RemappingPoints)
            {
                m_remappingDictionary[control] = e.Remapping;
                UpdateSummaryInformation(m_remappingDictionary.Values);
            }

            m_statisticsScratchpad = e.Remapping.BestScratchpad.Clone();
            m_remapping = e.Remapping;

            // Update the plan order's column
            PlanEditor?.ShowWithPluggable(this);
        }


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

            areRemappingPointsActive = m_areRemappingPointsActive;

            if (m_areRemappingPointsActive)
                plan.UpdateStatistics(new CharacterScratchpad(m_baseCharacter.After(plan.ChosenImplantSet)), true, true);
            else
                plan.UpdateStatistics(m_statisticsScratchpad.Clone(), false, true);

            plan.UpdateOldTrainingTimes(new CharacterScratchpad(m_baseCharacter.After(plan.ChosenImplantSet)), false,
                                        true);
        }

        /// <summary>
        /// Updates the times when "choose implant set" changes.
        /// </summary>
        public Task UpdateOnImplantSetChange()
        {
            panelWait.Show();

            return TaskHelper.RunCPUBoundTaskAsync(() => Run(update: true));
        }

        #endregion
    }
}