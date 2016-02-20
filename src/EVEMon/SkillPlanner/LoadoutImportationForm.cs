using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Collections;
using EVEMon.Common.Constants;
using EVEMon.Common.Controls;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Data;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Factories;
using EVEMon.Common.Helpers;
using EVEMon.Common.Interfaces;
using EVEMon.Common.Models;

namespace EVEMon.SkillPlanner
{
    /// <summary>
    /// Presents the user with an interface in which a fitting tool
    /// exported ship loadout is parsed it into a TreeView
    /// which can be used to browse the items in the loadout.
    /// </summary>
    public partial class LoadoutImportationForm : EVEMonForm
    {
        #region Fields

        private readonly List<Item> m_objects = new List<Item>();
        private readonly List<StaticSkillLevel> m_skillsToAdd = new List<StaticSkillLevel>();


        private Plan m_plan;
        private BaseCharacter m_character;
        private LoadoutFormat m_loadoutFormat;
        private ILoadoutInfo m_loadoutInfo;
        private string m_clipboardText;

        #endregion


        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        private LoadoutImportationForm()
        {
            InitializeComponent();
            RememberPositionKey = "LoadoutImportationForm";
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="plan">The plan.</param>
        public LoadoutImportationForm(Plan plan)
            : this()
        {
            if (plan == null)
                throw new ArgumentNullException("plan");

            m_plan = plan;
            m_character = m_plan.Character;

            EveMonClient.CharacterUpdated += EveMonClient_CharacterUpdated;
            EveMonClient.PlanChanged += EveMonClient_PlanChanged;
        }

        #endregion


        #region Overridden Methods

        /// <summary>
        /// Checks and pastes loadout from clipboard.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            if (!Visible)
                return;

            if (ResultsTreeView.Nodes.Count > 0)
                return;

            if (!Clipboard.ContainsText())
                return;

            m_clipboardText = Clipboard.GetText();

            if (!LoadoutHelper.IsLoadout(m_clipboardText, out m_loadoutFormat))
                return;

            ExplanationLabel.Text = $"The parsed {m_loadoutFormat} formated loadout is shown below.";
            BuildTreeView();
        }

        /// <summary>
        /// Unsubscribe events on closing.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            EveMonClient.CharacterUpdated -= EveMonClient_CharacterUpdated;
            EveMonClient.PlanChanged -= EveMonClient_PlanChanged;
            base.OnClosing(e);
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the plan to which the extracted skills of the loadout should be added.
        /// </summary>
        public Plan Plan
        {
            get { return m_plan; }
            set
            {
                if (m_plan == value)
                    return;

                m_plan = value;

                // The tag is used by WindowsFactory.ShowByTag
                Tag = value;

                m_character = m_plan.Character;
                UpdatePlanStatus();
            }
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// When the plan changed, we need to update the training time and such.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_PlanChanged(object sender, PlanChangedEventArgs e)
        {
            if (e.Plan == m_plan)
                UpdatePlanStatus();
        }

        /// <summary>
        /// When the character changed, we need to update training time and such.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_CharacterUpdated(object sender, CharacterChangedEventArgs e)
        {
            if (e.Character != m_character)
                return;

            UpdatePlanStatus();
        }

        /// <summary>
        /// Sets the DialogResult to Cancel and closes the form.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Arguments of the event.</param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Adds the required skills to the Plan specified by the Plan property.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPlan_Click(object sender, EventArgs e)
        {
            IPlanOperation operation = m_plan.TryAddSet(m_skillsToAdd, m_loadoutInfo.Loadouts.First().Name);
            if (operation == null)
                return;

            PlanWindow window = WindowsFactory.ShowByTag<PlanWindow, Plan>(operation.Plan);
            if (window == null || window.IsDisposed)
                return;

            PlanHelper.Perform(new PlanToOperationForm(operation), window);
            UpdatePlanStatus();
        }

        /// <summary>
        /// Browses the form that opened this instance of EFTLoadout to
        /// the item that was double clicked in the TreeView.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Arguments of the event.</param>
        private void tvLoadout_DoubleClick(object sender, EventArgs e)
        {
            Item item = ResultsTreeView.SelectedNode?.Tag as Item;

            if (item == null)
                return;

            PlanWindow planWindow = WindowsFactory.GetByTag<PlanWindow, Plan>(m_plan);

            if (planWindow == null || planWindow.IsDisposed)
                return;

            planWindow.ShowItemInBrowser(item);
        }

        /// <summary>
        /// Pops up the context menu for the TreeView.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Arguments of the event.</param>
        private void tvLoadout_MouseUp(object sender, MouseEventArgs e)
        {
            // Show menu only if the right mouse button is clicked
            if (e.Button != MouseButtons.Right)
                return;

            ResultsTreeView.Cursor = Cursors.Default;

            // Get the node that the user has clicked
            ResultsTreeView.SelectedNode = ResultsTreeView.GetNodeAt(e.Location);

            // Select the node the user has clicked
            contextMenu.Show(ResultsTreeView, e.Location);
        }

        /// <summary>
        /// When the mouse moves over the list, we change the cursor.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void tvLoadout_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                return;

            ResultsTreeView.Cursor = ResultsTreeView.GetNodeAt(e.Location)?.Tag != null
                ? CustomCursors.ContextMenu
                : Cursors.Default;
        }

        /// <summary>
        /// When the context menu opens, we update the menus status.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CancelEventArgs"/> instance containing the event data.</param>
        private void contextMenu_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = ResultsTreeView.SelectedNode?.Tag == null;
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Builds the tree view.
        /// </summary>
        private void BuildTreeView()
        {
            m_objects.Clear();
            ResultsTreeView.Nodes.Clear();

            // Decode EFT format
            if (m_loadoutFormat == LoadoutFormat.EFT)
                m_loadoutInfo = LoadoutHelper.DeserializeEftFormat(m_clipboardText);

            // Decode XML format
            if (m_loadoutFormat == LoadoutFormat.XML)
                m_loadoutInfo = LoadoutHelper.DeserializeXmlFormat(m_clipboardText);

            // Decode DNA format
            if (m_loadoutFormat == LoadoutFormat.DNA)
                m_loadoutInfo = LoadoutHelper.DeserializeDnaFormat(m_clipboardText);

            // Decode CLF format
            if (m_loadoutFormat == LoadoutFormat.CLF)
                m_loadoutInfo = LoadoutHelper.DeserializeClfFormat(m_clipboardText);

            if (m_loadoutInfo == null || !m_loadoutInfo.Loadouts.Any())
                return;

            LoadoutNameLabel.Text = $"Name: {m_loadoutInfo.Loadouts.First().Name}{(m_loadoutFormat == LoadoutFormat.DNA ? " - DNA loadout" : String.Empty)}"
                .WordWrap(55);

            ShipTypeNameLabel.Text = $"Ship: {(m_loadoutInfo.Ship != null ? m_loadoutInfo.Ship.Name : String.Empty)}"
                .WordWrap(55);

            DescriptionLabel.Text = $"Description: {m_loadoutInfo.Loadouts.First().Description}"
                .WordWrap(55);

            m_objects.Add(m_loadoutInfo.Ship);

            BuildTreeNodes(m_loadoutInfo.Loadouts.First().Items);

            // Order the nodes
            TreeNode[] orderNodes = ResultsTreeView.Nodes.Cast<TreeNode>().OrderBy(
                node => LoadoutHelper.OrderedSlotNames.IndexOf(String.Intern(node.Text))).ToArray();
            ResultsTreeView.Nodes.Clear();
            ResultsTreeView.Nodes.AddRange(orderNodes);

            // Update the controls
            UpdatePlanStatus();
            ResultsTreeView.ExpandAll();
            ResultsTreeView.Enabled = true;
            Cursor = Cursors.Default;
        }

        /// <summary>
        /// Builds the tree nodes.
        /// </summary>
        /// <param name="items">The items.</param>
        private void BuildTreeNodes(IEnumerable<Item> items)
        {
            foreach (Item item in items)
            {
                TreeNode slotNode;
                string nodeName = LoadoutHelper.OrderedSlotNames[7];

                // Regular item ?
                if (!item.MarketGroup.BelongsIn(DBConstants.AmmosAndChargesMarketGroupID))
                {
                    // Retrieve the tree node name for the slot
                    switch (item.FittingSlot)
                    {
                        case ItemSlot.High:
                            nodeName = LoadoutHelper.OrderedSlotNames[0];
                            break;
                        case ItemSlot.Medium:
                            nodeName = LoadoutHelper.OrderedSlotNames[1];
                            break;
                        case ItemSlot.Low:
                            nodeName = LoadoutHelper.OrderedSlotNames[2];
                            break;
                    }

                    // Is it a rig?
                    if (item.MarketGroup.BelongsIn(DBConstants.ShipModificationsMarketGroupID))
                        nodeName = LoadoutHelper.OrderedSlotNames[3];
                    // Is it a subsystem?
                    else if (item.MarketGroup.BelongsIn(DBConstants.SubsystemsMarketGroupID))
                        nodeName = LoadoutHelper.OrderedSlotNames[4];
                    // Is it a drone?
                    else if (item.MarketGroup.BelongsIn(DBConstants.DronesMarketGroupID))
                        nodeName = LoadoutHelper.OrderedSlotNames[6];

                    // Gets or create the node for the slot
                    slotNode = !ResultsTreeView.Nodes.ContainsKey(nodeName)
                        ? ResultsTreeView.Nodes.Add(nodeName, nodeName)
                        : ResultsTreeView.Nodes[nodeName];

                    // Add a new node
                    TreeNode itemNode = new TreeNode { Text = item.Name, Tag = item };
                    slotNode.Nodes.Add(itemNode);

                    m_objects.Add(item);

                    continue;
                }

                // Item is a charge ? 
                nodeName = LoadoutHelper.OrderedSlotNames[5];
                slotNode = !ResultsTreeView.Nodes.ContainsKey(nodeName)
                    ? ResultsTreeView.Nodes.Add(nodeName, nodeName)
                    : ResultsTreeView.Nodes[nodeName];

                TreeNode ammoNode = new TreeNode { Text = item.Name, Tag = item };
                slotNode.Nodes.Add(ammoNode);

                m_objects.Add(item);
            }
        }

        /// <summary>
        /// Updates the form controls to reflect the status of the Plan specified by the Plan property.
        /// </summary>
        private void UpdatePlanStatus()
        {
            if (m_loadoutFormat == LoadoutFormat.None)
                return;

            // Compute the skills to add
            m_skillsToAdd.Clear();
            CharacterScratchpad scratchpad = new CharacterScratchpad(m_character);
            Character character = (Character)m_character;
            foreach (Item obj in m_objects)
            {
                scratchpad.Train(obj.Prerequisites.Where(x => character.Skills[x.Skill.ID].Level < x.Level));
            }
            m_skillsToAdd.AddRange(scratchpad.TrainedSkills);

            // All skills already trained ?
            if (m_skillsToAdd.Count == 0)
            {
                AddToPlanButton.Enabled = false;
                PlanedLabel.Visible = true;
                PlanedLabel.Text = @"All skills already trained.";
                TrainTimeLabel.Visible = false;
            }
            // Are skills already planned ?
            else if (m_plan.AreSkillsPlanned(m_skillsToAdd))
            {
                AddToPlanButton.Enabled = false;
                PlanedLabel.Visible = true;
                PlanedLabel.Text = @"All skills already trained or planned.";
                TrainTimeLabel.Visible = false;
            }
            // There is at least one untrained or non-planned skill
            else
            {
                AddToPlanButton.Enabled = true;
                PlanedLabel.Text = String.Empty;
                PlanedLabel.Visible = false;
                TrainTimeLabel.Visible = true;

                // Compute training time
                TimeSpan trainingTime = m_character.GetTrainingTimeToMultipleSkills(m_skillsToAdd);
                TrainTimeLabel.Text = trainingTime.ToDescriptiveText(
                    DescriptiveTextOptions.IncludeCommas | DescriptiveTextOptions.SpaceText);
            }
        }

        #endregion
    }
}