using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;
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
        private Plan m_plan;
        private BaseCharacter m_character;
        private string m_loadoutName = String.Empty;
        private readonly List<Item> m_objects = new List<Item>();
        private readonly List<StaticSkillLevel> m_skillsToAdd = new List<StaticSkillLevel>();

        /// <summary>
        /// Constructor
        /// </summary>
        public EFTLoadoutImportationForm(Plan plan)
        {
            InitializeComponent();
            this.topSplitContainer.RememberDistanceKey = "EFTLoadoutImportationForm";

            m_plan = plan;
            m_character = m_plan.Character;

            EveClient.CharacterChanged += new EventHandler<CharacterChangedEventArgs>(EveClient_CharacterChanged);
            EveClient.PlanChanged += new EventHandler<PlanChangedEventArgs>(EveClient_PlanChanged);
        }

		/// <summary>
		/// Checks and pastes loadout from clipboard.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);
            
			if (!this.Visible)
				return;

            if (this.PasteTextBox.Text.Length > 0)
                return;

            if (!Clipboard.ContainsText())
                return;

			string clipboardText = Clipboard.GetText();

			if (!IsLoadout(clipboardText))
				return;

			PasteTextBox.Text = Clipboard.GetText();
		}

        /// <summary>
        /// unsubscribe events on closing.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            EveClient.CharacterChanged -= new EventHandler<CharacterChangedEventArgs>(EveClient_CharacterChanged);
            EveClient.PlanChanged -= new EventHandler<PlanChangedEventArgs>(EveClient_PlanChanged);
            base.OnClosing(e);
        }

        /// <summary>
        /// Gets the plan to which the extracted skills of the loadout should be added.
        /// </summary>
        public Plan Plan
        {
            get { return m_plan; }
            set
            {
                if (m_plan != value)
                {
                    m_plan = value;
                    m_character = (Character)m_plan.Character;
                    UpdatePlanStatus();
                }
            }
        }

        /// <summary>
        /// When the plan changed, we need to update the training time and such.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EveClient_PlanChanged(object sender, PlanChangedEventArgs e)
        {
            if(e.Plan == m_plan) UpdatePlanStatus();
        }

        /// <summary>
        /// When the character changed, we need to update training time and such.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EveClient_CharacterChanged(object sender, CharacterChangedEventArgs e)
        {
            if (e.Character != m_character) return;
            UpdatePlanStatus();
        }

		/// <summary>
		/// Checks loadout for valid header.
		/// </summary>
		/// <param name="text">Loadout text.</param>
		/// <returns>Is loadout valid.</returns>
		private bool IsLoadout(string text)
		{
			string[] lines = text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
			if (lines.Length == 0)
				return false;

			// Error on first line ?
			string line = lines[0];
			if (String.IsNullOrEmpty(line) || !line.StartsWith("[") || !line.Contains(","))
				return false;

			// Retrieve the ship
			int commaIndex = line.IndexOf(',');
			string shipTypeName = line.Substring(1, commaIndex - 1);
			Item ship = StaticItems.Ships.AllItems.FirstOrDefault(x => x.Name == shipTypeName);
			if (ship == null)
				return false;

			return true;
		}

        /// <summary>
        /// Occur when the user changed the text box whiere he should paste the data from EFT.
        /// We rebuimd the objects list and update the right pane.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Arguments of the event.</param>
        private void tbEFTLoadout_TextChanged(object sender, EventArgs e)
        {
            m_objects.Clear();
            m_loadoutName = String.Empty;
            ResultsTreeView.Nodes.Clear();

            // If the box is empty, error
            if (PasteTextBox.Lines.Length == 0)
            {
                ResultsTreeView.Nodes.Add("Cannot determine ship type");
                ResultsTreeView.Enabled = false;
                return;
            }

            // Error on first line ?
            string line = PasteTextBox.Lines[0];
            if (String.IsNullOrEmpty(line) || !line.StartsWith("[") || !line.Contains(","))
            {
                ResultsTreeView.Nodes.Add("Cannot determine ship type");
                ResultsTreeView.Enabled = false;
                return;
            }

            // Retrieve the ship
            int commaIndex = line.IndexOf(',');
            string shipTypeName = line.Substring(1, commaIndex - 1);
            Item ship = StaticItems.Ships.AllItems.FirstOrDefault(x => x.Name == shipTypeName);
            if (ship != null)
            {
                m_objects.Add(ship);
            }
            else
            {
                // Couldn't find that ship
                ResultsTreeView.Nodes.Add("Cannot determine ship type");
                ResultsTreeView.Enabled = false;
                return;
            }

            // Retrieve the loadout name
            int lineLength = line.Length;
            m_loadoutName = line.Substring(commaIndex + 1, (lineLength - commaIndex - 2)); 

            // Add the items
            for (int i = 1; i < PasteTextBox.Lines.Length; i++)
            {
                line = PasteTextBox.Lines[i];
                if (!String.IsNullOrEmpty(line)) AddItem(line);
            }

            // Update the controls
            UpdatePlanStatus();
            ResultsTreeView.ExpandAll();
            ResultsTreeView.Enabled = true;
            Cursor.Current = Cursors.Default; 
        }

        /// <summary>
        /// Parses one line of loadout text and adds the required
        /// skills for the items to the _SkillsToAdd list.
        /// </summary>
        /// <remarks>
        /// parsed items are also added to the TreeView.
        /// </remarks>
        /// <param name="line">Line of text to be parsed.</param>
        private void AddItem(string line)
        {
            // Retrieve the item and its charge
            string itemName = line.Contains(",") ? line.Substring(0, line.LastIndexOf(',')) : line;
            string chargeName = line.Contains(",") ? line.Substring(line.LastIndexOf(',') + 2) : null;

            Item item = StaticItems.GetItemByName(itemName);
            Item charge = !String.IsNullOrEmpty(chargeName) ? StaticItems.GetItemByName(chargeName) : null;
            
            // Regular item ?
            if (item != null)
            {
                // Retrieve the tree node name for the slot
                string slotNodeName = String.Empty;
                switch (item.FittingSlot)
                {
                    case ItemSlot.High:
                        slotNodeName = "High";
                        break;
                    case ItemSlot.Medium:
                        slotNodeName = "Med";
                        break;
                    case ItemSlot.Low:
                        slotNodeName = "Low";
                        break;
                }

                // Gets or create the node for the slot
                TreeNode slotNode = !ResultsTreeView.Nodes.ContainsKey(slotNodeName) ? ResultsTreeView.Nodes.Add(slotNodeName, slotNodeName) : ResultsTreeView.Nodes[slotNodeName];

                // Add a new node
                TreeNode itemNode = new TreeNode();
                itemNode.Text = item.Name;
                itemNode.Tag = item;
                slotNode.Nodes.Add(itemNode);
            }
            // Might be drone
            else
            {
                // Retrieve the item
                itemName = line.Contains(" x") ? line.Substring(0, line.LastIndexOf(" x")) : line;
                item = StaticItems.GetItemByName(itemName);

                // Add it to the drones node
                if (item != null)
                {
                    TreeNode slotNode = !ResultsTreeView.Nodes.ContainsKey("Drones") ? ResultsTreeView.Nodes.Add("Drones", "Drones") : ResultsTreeView.Nodes["Drones"];
                    TreeNode itemNode = new TreeNode();
                    itemNode.Text = item.Name;
                    itemNode.Tag = item;
                    slotNode.Nodes.Add(itemNode);
                }
            }

            // Has charges ? 
            if (charge != null)
            {
                TreeNode slotNode = !ResultsTreeView.Nodes.ContainsKey("Ammunition") ? ResultsTreeView.Nodes.Add("Ammunition", "Ammunition") : ResultsTreeView.Nodes["Ammunition"];

                TreeNode ammoNode = new TreeNode();
                ammoNode.Text = charge.Name;
                ammoNode.Tag = charge;
                slotNode.Nodes.Add(ammoNode);
            }

            // Add the item and its charge to the objects list
            if (item != null) m_objects.Add(item);
            if (charge != null) m_objects.Add(charge);
        }

        /// <summary>
        /// Updates the form controls to reflect the status of the Plan specified by the Plan property.
        /// </summary>
        private void UpdatePlanStatus()
        {
            // Compute the skills to add
            m_skillsToAdd.Clear();
            var scratchpad = new CharacterScratchpad(m_character);
            foreach (var obj in m_objects) scratchpad.Train(obj.Prerequisites);
            m_skillsToAdd.AddRange(scratchpad.TrainedSkills);

            // All skills already known ?
            if (m_skillsToAdd.Count == 0)
            {
                AddToPlanButton.Enabled = false;
                PlanedLabel.Visible = true;
                PlanedLabel.Text = "All skills already known.";
                TrainTimeLabel.Visible = false;
            }
            // Are skills already planned ?
            else if (m_plan.AreSkillsPlanned(m_skillsToAdd))
            {
                AddToPlanButton.Enabled = false;
                PlanedLabel.Visible = true;
                PlanedLabel.Text = "All skills already known or planned.";
                TrainTimeLabel.Visible = false;
            }
            // There is at least one unknown and non-planned skill
            else
            {
                AddToPlanButton.Enabled = true;
                PlanedLabel.Text = String.Empty;
                PlanedLabel.Visible = false;
                TrainTimeLabel.Visible = true;

                // Compute training time
                TimeSpan trainingTime = m_character.GetTrainingTimeToMultipleSkills(m_skillsToAdd);
                TrainTimeLabel.Text = Skill.TimeSpanToDescriptiveText(trainingTime, DescriptiveTextOptions.IncludeCommas | DescriptiveTextOptions.SpaceText);
            }
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
            var operation = m_plan.TryAddSet(m_skillsToAdd, m_loadoutName);
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
            if (ResultsTreeView.SelectedNode != null)
            {
                Item item = ResultsTreeView.SelectedNode.Tag as Item;
                if (item != null)
                {
                    PlanWindow opener = WindowsFactory<PlanWindow>.GetByTag(m_plan);
                    opener.ShowItemInBrowser(item);
                }
            }
        }

        /// <summary>
        /// Pops up the context menu for the TreeView.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Arguments of the event.</param>
        private void tvLoadout_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // Show menu only if the right mouse button is clicked.
            if (e.Button == MouseButtons.Right)
            {
                // Point where the mouse is clicked.
                Point p = new Point(e.X, e.Y);

                // Get the node that the user has clicked.
                TreeNode node = ResultsTreeView.GetNodeAt(p);
                if (node != null && node.Tag != null)
                {
                    // Select the node the user has clicked.
                    ResultsTreeView.SelectedNode = node;
                    RightClickContextMenuStrip.Show(ResultsTreeView, p);
                }
            }
        }
    }
}
