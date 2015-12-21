using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using EVEMon.Common.Constants;
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
    public partial class AttributesOptimizationForm : EVEMonForm, IPlanOrderPluggable
    {
        private readonly Dictionary<AttributesOptimizationControl, RemappingResult>
            m_remappingDictionary;

        private readonly BaseCharacter m_baseCharacter;
        private readonly Character m_character;
        private readonly AttributeOptimizationStrategy m_strategy;
        private readonly BasePlan m_plan;
        private readonly string m_description;

        private Thread m_thread;
        private PlanEditorControl m_planEditor;
        private CharacterScratchpad m_statisticsScratchpad;
        private bool m_areRemappingPointsActive;
        private bool m_update;

        // Variables for manual edition of a plan
        private RemappingPoint m_manuallyEditedRemappingPoint;
        private RemappingResult m_remapping;

        /// <summary>
        /// Constructor for designer.
        /// </summary>
        private AttributesOptimizationForm()
        {
            InitializeComponent();
            m_remappingDictionary = new Dictionary<AttributesOptimizationControl, RemappingResult>();
        }

        /// <summary>
        /// Constructor for use in code when optimizing remapping.
        /// </summary>
        /// <param name="character">Character information</param>
        /// <param name="plan">Plan to optimize for</param>
        /// <param name="strategy">Optimization strategy</param>
        /// <param name="name">Title of this form</param>
        /// <param name="description">Description of the optimization operation</param>
        public AttributesOptimizationForm(Character character, BasePlan plan, AttributeOptimizationStrategy strategy, string name,
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
            base.Text = name;
        }

        /// <summary>
        /// Constructor for use in code when the user wants to manually edit a remapping point.
        /// </summary>
        /// <param name="character">Character information</param>
        /// <param name="plan">Plan to optimize for</param>
        /// <param name="point">The point.</param>
        public AttributesOptimizationForm(Character character, Plan plan, RemappingPoint point)
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
            base.Text = String.Format(CultureConstants.DefaultCulture, "Remapping point manual editing ({0})", plan.Name);
        }

        /// <summary>
        /// Gets or sets a <see cref="PlanEditorControl"/>.
        /// </summary>
        public PlanEditorControl PlanEditor
        {
            get { return m_planEditor; }
            set { m_planEditor = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            lvPoints.Font = FontFactory.GetFont("Arial", 9F);
            throbber.State = ThrobberState.Rotating;

            m_thread = new Thread(Run);
            m_thread.Start();

            lvPoints.Font = FontFactory.GetDefaultFont(9F);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            // Stop the thread
            if (m_thread != null)
            {
                m_thread.Abort();
                m_thread = null;
            }

            m_planEditor = null;

            // Base call
            base.OnClosed(e);
        }

        /// <summary>
        /// Starts optimization.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void Run()
        {
            // Compute best scratchpad
            RemappingResult remapping = null;
            ICollection<RemappingResult> remappingList = null;

            switch (m_strategy)
            {
                case AttributeOptimizationStrategy.ManualRemappingPointEdition:
                    m_areRemappingPointsActive = true;
                    if (m_update)
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

            if (m_update)
            {
                // Update the controls for every attribute on the already shown form
                UpdateForm(remapping, remappingList);
            }
            else
            {
                // Update the controls for every attribute
                Dispatcher.Invoke(() => UpdateForm(remapping, remappingList));
            }
        }

        /// <summary>
        /// Updates controls on the form.
        /// </summary>
        /// <param name="remapping">An <see cref="RemappingResult"/> object</param>
        /// <param name="remappingList">List of remappings</param>
        private void UpdateForm(RemappingResult remapping, ICollection<RemappingResult> remappingList)
        {
            // If the thread has been canceled, we stop right now to prevent an exception
            if (m_thread == null)
                return;

            // Hide the throbber and the waiting message
            throbber.State = ThrobberState.Stopped;
            panelWait.Visible = false;

            tabControl.Controls.Clear();

            // Update the attributes
            if (remapping != null)
            {
                m_statisticsScratchpad = remapping.BestScratchpad.Clone();
                UpdateForRemapping(remapping);
            }
            else
                UpdateForRemappingList(remappingList);

            // Update the plan order's column
            if (m_planEditor != null && (remapping != null || remappingList.Count != 0))
                m_planEditor.ShowWithPluggable(this);
        }

        /// <summary>
        /// Updates the UI once the computation has been done (for whole plan or character from birth)
        /// </summary>
        /// <param name="remapping"></param>
        private void UpdateForRemapping(RemappingResult remapping)
        {
            // Create control
            AttributesOptimizationControl ctl = CreateAttributesOptimizationControl(remapping, m_description);
            Controls.Add(ctl);
            ctl.BringToFront();
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
                panelNoResult.Visible = true;
                return;
            }

            // Adds a tab page for the summary
            tabControl.Controls.Add(tabSummary);

            // Updates the summary informations
            UpdateSummaryInformation(remappingList);

            // Adds a tab page for every remapping
            int index = 1;
            foreach (RemappingResult remap in remappingList)
            {
                AddTabPage(remap, "#" + index, m_description);
                index++;
            }

            tabControl.Visible = true;
            tabSummary.Focus();
        }

        /// <summary>
        /// Updates information in summary page.
        /// </summary>
        /// <param name="remappingList">List of remappings</param>
        private void UpdateSummaryInformation(IEnumerable<RemappingResult> remappingList)
        {
            TimeSpan baseDuration = m_plan.GetTotalTime(m_character.After(m_plan.ChosenImplantSet), false);
            lvPoints.Items.Clear();

            // Add global informations
            ListViewGroup globalGroup = new ListViewGroup("Global informations");
            lvPoints.Groups.Add(globalGroup);

            TimeSpan savedTime = remappingList.Aggregate(TimeSpan.Zero,
                                                         (current, remap) =>
                                                         current.Add(remap.BaseDuration.Subtract(remap.BestDuration)));

            lvPoints.Items.Add(new ListViewItem(String.Format(CultureConstants.DefaultCulture, "Current time : {0}",
                                                              baseDuration.ToDescriptiveText(
                                                                  DescriptiveTextOptions.IncludeCommas)), globalGroup));

            if (savedTime != TimeSpan.Zero)
            {
                lvPoints.Items.Add(
                    new ListViewItem(String.Format(CultureConstants.DefaultCulture, "Optimized time : {0}",
                                                   baseDuration.Subtract(savedTime).ToDescriptiveText(
                                                       DescriptiveTextOptions.IncludeCommas)), globalGroup));

                if (savedTime < TimeSpan.Zero)
                {
                    ListViewItem savedTimeItem = lvPoints.Items.Add(
                        new ListViewItem(String.Format(CultureConstants.DefaultCulture, "{0} slower than current.",
                                                       (-savedTime).ToDescriptiveText(DescriptiveTextOptions.IncludeCommas)),
                                         globalGroup));
                    savedTimeItem.ForeColor = Color.DarkRed;
                }
                else
                {
                    lvPoints.Items.Add(
                        new ListViewItem(String.Format(CultureConstants.DefaultCulture, "{0} better than current.",
                                                       savedTime.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas)),
                                         globalGroup));
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
            string text = String.Format(CultureConstants.DefaultCulture, "{0} at {1}", remap,
                                        remap.StartTime.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas));
            ListViewGroup group = new ListViewGroup(text);
            lvPoints.Groups.Add(group);

            // Check there are at least one year between each remap
            TimeSpan timeSinceLastRemap = remap.StartTime.Subtract(lastRemap);
            if (timeSinceLastRemap < TimeSpan.FromDays(365) && remap.StartTime != TimeSpan.Zero)
            {
                ListViewItem item =
                    new ListViewItem(String.Format(CultureConstants.DefaultCulture,
                                                   "The previous remap point was only {0} ago.",
                                                   timeSinceLastRemap.ToDescriptiveText(
                                                       DescriptiveTextOptions.IncludeCommas)), group)
                        { ForeColor = Color.DarkRed };
                lvPoints.Items.Add(item);
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
            AttributesOptimizationControl ctl = CreateAttributesOptimizationControl(remapping, description);

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
                if (tempPage != null)
                    tempPage.Dispose();
            }
        }

        /// <summary>
        /// Creates a <see cref="AttributesOptimizationControl"/> for a given remapping.
        /// </summary>
        /// <param name="remapping">The remapping object to represents.</param>
        /// <param name="description">The description.</param>
        /// <returns>The created control.</returns>
        private AttributesOptimizationControl CreateAttributesOptimizationControl(RemappingResult remapping, string description)
        {
            AttributesOptimizationControl control;
            AttributesOptimizationControl ctl = null;
            try
            {
                ctl = new AttributesOptimizationControl(m_character, m_plan, remapping, description);
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
                if (ctl != null)
                    ctl.Dispose();
            }

            return control;
        }

        /// <summary>
        /// Racalculating plan and summary page after change of a <see cref="AttributesOptimizationControl"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="AttributeChangedEventArgs"/> instance containing the event data.</param>
        private void AttributesOptimizationControl_AttributeChanged(object sender, AttributeChangedEventArgs e)
        {
            // Update the plan order's column
            if (m_planEditor == null)
                return;

            AttributesOptimizationControl control = (AttributesOptimizationControl)sender;

            if (m_strategy == AttributeOptimizationStrategy.RemappingPoints)
            {
                m_remappingDictionary[control] = e.Remapping;
                UpdateSummaryInformation(m_remappingDictionary.Values);
            }

            m_statisticsScratchpad = e.Remapping.BestScratchpad.Clone();
            m_planEditor.ShowWithPluggable(this);
            m_remapping = e.Remapping;
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
        public void UpdateOnImplantSetChange()
        {
            m_update = true;
            Run();
        }

        #endregion
    }

    /// <summary>
    /// Remapping strategy.
    /// </summary>
    public enum AttributeOptimizationStrategy
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
        Character,

        /// <summary>
        /// Used when the user double-click a remapping point to manually edit it.
        /// </summary>
        ManualRemappingPointEdition
    }
}