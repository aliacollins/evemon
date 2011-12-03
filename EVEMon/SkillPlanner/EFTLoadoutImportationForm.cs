using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Data;

namespace EVEMon.SkillPlanner
{
    /// <summary>
    /// Presents the user with an interface in which an EFT exported
    /// ship loadout can be pasted. When given a meaningful ship
    /// loadout is presented the form then parses it into a) a data 
    /// structure that can be passed to the provided and b) a TreeView
    /// which can be used to browse the items in the loadout and to
    /// check if the loadout has been correctly parsed.
    /// </summary>
    public partial class EFTLoadoutImportationForm : EVEMonForm
    {
        private readonly List<Item> m_objects = new List<Item>();
        private readonly List<StaticSkillLevel> m_skillsToAdd = new List<StaticSkillLevel>();

        private Plan m_plan;
        private BaseCharacter m_character;


        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        private EFTLoadoutImportationForm()
        {
            InitializeComponent();

            topSplitContainer.RememberDistanceKey = "EFTLoadoutImportationForm";
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="plan">The plan.</param>
        public EFTLoadoutImportationForm(Plan plan)
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

            if (PasteTextBox.Text.Length > 0)
                return;

            if (!Clipboard.ContainsText())
                return;

            string clipboardText = Clipboard.GetText();

            if (!IsLoadout(clipboardText))
                return;

            ExplanationLabel.Text = "The EFT formated loadout is shown below.";
            PasteTextBox.Text = Clipboard.GetText();
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
        /// Occur when the user changed the text box whiere he should paste the data from EFT.
        /// We rebuild the objects list and update the right panel.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Arguments of the event.</param>
        private void tbEFTLoadout_TextChanged(object sender, EventArgs e)
        {
            BuildTreeView();
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
            // Retrieve the loadout name
            string line = PasteTextBox.Lines.First();
            int commaIndex = line.IndexOf(',');
            string loadoutName = line.Substring(commaIndex + 1, (line.Length - commaIndex - 2)).Trim();

            IPlanOperation operation = m_plan.TryAddSet(m_skillsToAdd, loadoutName);
            PlanHelper.Perform(operation);
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
            if (ResultsTreeView.SelectedNode == null)
                return;

            Item item = ResultsTreeView.SelectedNode.Tag as Item;
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

            // Point where the mouse is clicked
            Point p = new Point(e.X, e.Y);

            // Get the node that the user has clicked
            TreeNode node = ResultsTreeView.GetNodeAt(p);
            if (node == null || node.Tag == null)
                return;

            // Select the node the user has clicked
            ResultsTreeView.SelectedNode = node;
            RightClickContextMenuStrip.Show(ResultsTreeView, p);
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Checks loadout for valid header.
        /// </summary>
        /// <param name="text">Loadout text.</param>
        /// <returns>Is loadout valid.</returns>
        private static bool IsLoadout(string text)
        {
            string[] lines = text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            // Nothing to evaluate
            if (lines.Length == 0)
                return false;

            // Error on first line ?
            string line = lines.First();
            if (String.IsNullOrEmpty(line) || !line.StartsWith("[", StringComparison.CurrentCulture) || !line.Contains(","))
                return false;

            // Retrieve the ship
            int commaIndex = line.IndexOf(',');
            string shipTypeName = line.Substring(1, commaIndex - 1);

            return StaticItems.ShipsMarketGroup.AllItems.FirstOrDefault(x => x.Name == shipTypeName) != null;
        }

        /// <summary>
        /// Builds the tree view.
        /// </summary>
        private void BuildTreeView()
        {
            m_objects.Clear();
            ResultsTreeView.Nodes.Clear();

            // Add the items
            foreach (string line in PasteTextBox.Lines.Where(line => !String.IsNullOrEmpty(line)))
            {
                AddItem(line);
            }

            // Update the controls
            UpdatePlanStatus();
            ResultsTreeView.ExpandAll();
            ResultsTreeView.Enabled = true;
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Parses a line of loadout text and adds the item to the TreeView.
        /// </summary>
        /// <param name="line">Line of text to be parsed.</param>
        private void AddItem(string line)
        {
            // Retrieve the ship
            if (line == PasteTextBox.Lines.First())
            {
                string shipTypeName = line.Substring(1, line.IndexOf(',') - 1);
                Item ship = StaticItems.ShipsMarketGroup.AllItems.First(x => x.Name == shipTypeName);
                m_objects.Add(ship);
            }

            // Retrieve the item (might be a drone)
            string itemName = line.Contains(",") ? line.Substring(0, line.LastIndexOf(',')) : line;
            itemName = itemName.Contains(" x") ? itemName.Substring(0, line.LastIndexOf(" x", StringComparison.CurrentCulture)) : itemName;
            Item item = StaticItems.GetItemByName(itemName);

            // Retrieve the charge
            string chargeName = line.Contains(",") ? line.Substring(line.LastIndexOf(',') + 2) : null;
            Item charge = !String.IsNullOrEmpty(chargeName) ? StaticItems.GetItemByName(chargeName) : null;

            // Regular item ?
            if (item != null)
            {
                // Retrieve the tree node name for the slot
                string nodeName = String.Empty;
                switch (item.FittingSlot)
                {
                    case ItemSlot.High:
                        nodeName = "High";
                        break;
                    case ItemSlot.Medium:
                        nodeName = "Med";
                        break;
                    case ItemSlot.Low:
                        nodeName = "Low";
                        break;
                }

                // Is it a rig?
                if (item.MarketGroup.BelongsIn(new[] { DBConstants.ShipModificationsMarketGroupID }))
                    nodeName = "Rigs";

                // Is it a subsystem?
                if (item.MarketGroup.BelongsIn(new[] { DBConstants.SubsystemsMarketGroupID }))
                    nodeName = "Subsystems";

                // Is it a drone?
                if (item.MarketGroup.BelongsIn(new[] { DBConstants.DronesMarketGroupID }))
                    nodeName = "Drones";

                // Gets or create the node for the slot
                TreeNode slotNode = !ResultsTreeView.Nodes.ContainsKey(nodeName)
                                        ? ResultsTreeView.Nodes.Add(nodeName, nodeName)
                                        : ResultsTreeView.Nodes[nodeName];

                // Add a new node
                TreeNode itemNode = new TreeNode { Text = item.Name, Tag = item };
                slotNode.Nodes.Add(itemNode);
            }

            // Has charges ? 
            if (charge != null)
            {
                TreeNode slotNode = !ResultsTreeView.Nodes.ContainsKey("Ammunition")
                                        ? ResultsTreeView.Nodes.Add("Ammunition", "Ammunition")
                                        : ResultsTreeView.Nodes["Ammunition"];

                TreeNode ammoNode = new TreeNode { Text = charge.Name, Tag = charge };
                slotNode.Nodes.Add(ammoNode);
            }

            // Add the item and its charge to the objects list
            if (item != null)
                m_objects.Add(item);

            if (charge != null)
                m_objects.Add(charge);
        }

        /// <summary>
        /// Updates the form controls to reflect the status of the Plan specified by the Plan property.
        /// </summary>
        private void UpdatePlanStatus()
        {
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
                PlanedLabel.Text = "All skills already trained.";
                TrainTimeLabel.Visible = false;
            }
                // Are skills already planned ?
            else if (m_plan.AreSkillsPlanned(m_skillsToAdd))
            {
                AddToPlanButton.Enabled = false;
                PlanedLabel.Visible = true;
                PlanedLabel.Text = "All skills already trained or planned.";
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