using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.FittingTools;

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

        private readonly string[] m_orderedNodeNames = new[]
                                                           {
                                                               "High", "Med", "Low", "Rigs",
                                                               "Subsystems", "Ammunition", "Drones"
                                                           };

        private static string[] s_lines;
        private static SerializableFittings s_fittings;

        private Plan m_plan;
        private BaseCharacter m_character;
        private LoadoutFormat m_loadoutFormat;
        private string m_loadoutName;

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

            string clipboardText = Clipboard.GetText();

            if (!IsLoadout(clipboardText))
                return;

            ExplanationLabel.Text = String.Format(CultureConstants.DefaultCulture,
                                                  "The parsed {0} formated loadout is shown below.", m_loadoutFormat);
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
            IPlanOperation operation = m_plan.TryAddSet(m_skillsToAdd, m_loadoutName);
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

        private bool IsLoadout(string text)
        {
            if (IsEFTFormat(text))
            {
                m_loadoutFormat = LoadoutFormat.EFT;
                return true;
            }

            if (IsXMLFormat(text))
            {
                m_loadoutFormat = LoadoutFormat.XML;
                return true;
            }

            if (IsDNAFormat(text))
            {
                m_loadoutFormat = LoadoutFormat.DNA;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the loadout is in EFT format.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        /// 	<c>true</c> if the loadout is in EFT format; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsEFTFormat(string text)
        {
            s_lines = text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            // Nothing to evaluate
            if (s_lines.Length == 0)
                return false;

            // Error on first line ?
            string line = s_lines.First();
            if (String.IsNullOrEmpty(line) || !line.StartsWith("[", StringComparison.CurrentCulture) || !line.Contains(","))
                return false;

            // Retrieve the ship
            int commaIndex = line.IndexOf(',');
            string shipTypeName = line.Substring(1, commaIndex - 1);

            return StaticItems.ShipsMarketGroup.AllItems.Any(x => x.Name == shipTypeName);
        }

        /// <summary>
        /// Determines whether the loadout is in XML format.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        /// 	<c>true</c> if the loadout is in XML format; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsXMLFormat(string text)
        {
            XmlRootAttribute xmlRoot = new SerializableFittings().GetType().GetCustomAttributes(
                typeof(XmlRootAttribute), false).Cast<XmlRootAttribute>().FirstOrDefault();

            if (xmlRoot == null)
                return false;

            using (TextReader reader = new StringReader(text))
            {
                if (Util.GetXmlRootElement(reader) != xmlRoot.ElementName)
                    return false;
            }

            s_fittings = Util.DeserializeXmlFromString<SerializableFittings>(text);
            return StaticItems.ShipsMarketGroup.AllItems.Any(x => x.Name == s_fittings.Fitting.ShipType.Name);
        }

        /// <summary>
        /// Determines whether the loadout is in DNA format.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        /// 	<c>true</c> if the loadout is in DNA format; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsDNAFormat(string text)
        {
            s_lines = text.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            // Nothing to evaluate
            if (s_lines.Length == 0)
                return false;

            // Error on first line ?
            int shipID;
            string line = s_lines.First();
            if (String.IsNullOrEmpty(line) || !Int32.TryParse(line, out shipID))
                return false;

            return StaticItems.ShipsMarketGroup.AllItems.Any(x => x.ID == shipID);
        }

        /// <summary>
        /// Builds the tree view.
        /// </summary>
        private void BuildTreeView()
        {
            m_objects.Clear();
            ResultsTreeView.Nodes.Clear();

            // Decode EFT format
            if (m_loadoutFormat == LoadoutFormat.EFT)
                DecodeEFTFormat();

            // Decode XML format
            if (m_loadoutFormat == LoadoutFormat.XML)
                DecodeXMLFormat();

            // Decode DNA format
            if (m_loadoutFormat == LoadoutFormat.DNA)
                DecodeDNAFormat();

            // Order the nodes
            TreeNode[] orderNodes = ResultsTreeView.Nodes.Cast<TreeNode>().OrderBy(
                node => m_orderedNodeNames.IndexOf(String.Intern(node.Name))).ToArray();
            ResultsTreeView.Nodes.Clear();
            ResultsTreeView.Nodes.AddRange(orderNodes);

            // Update the controls
            UpdatePlanStatus();
            ResultsTreeView.ExpandAll();
            ResultsTreeView.Enabled = true;
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Decodes an EFT loadout text and adds the items to the TreeView.
        /// </summary>
        private void DecodeEFTFormat()
        {
            foreach (string line in s_lines.Where(line => !String.IsNullOrEmpty(line)))
            {
                // Retrieve the ship
                if (line == s_lines.First())
                {
                    // Retrieve the loadout name
                    int commaIndex = line.IndexOf(',');
                    m_loadoutName = line.Substring(commaIndex + 1, (line.Length - commaIndex - 2)).Trim();
                    LoadoutNameLabel.Text = String.Format(CultureConstants.DefaultCulture,
                                                          "Loadout Name: {0}", m_loadoutName);

                    string shipTypeName = line.Substring(1, commaIndex - 1);
                    ShipTypeNameLabel.Text = String.Format(CultureConstants.DefaultCulture, "Ship: {0}", shipTypeName);
                    Item ship = StaticItems.ShipsMarketGroup.AllItems.First(x => x.Name == shipTypeName);
                    m_objects.Add(ship);
                    continue;
                }

                // Retrieve the item (might be a drone)
                string itemName = line.Contains(",") ? line.Substring(0, line.LastIndexOf(',')) : line;
                itemName = itemName.Contains(" x")
                               ? itemName.Substring(0, line.LastIndexOf(" x", StringComparison.CurrentCulture))
                               : itemName;
                Item item = StaticItems.GetItemByName(itemName);

                // Retrieve the charge
                string chargeName = line.Contains(",") ? line.Substring(line.LastIndexOf(',') + 2) : null;
                Item charge = !String.IsNullOrEmpty(chargeName) ? StaticItems.GetItemByName(chargeName) : null;

                AddNode(item, charge);
            }
        }

        /// <summary>
        /// Decodes the XML loadout text and adds the items to the TreeView.
        /// </summary>
        private void DecodeXMLFormat()
        {
            // Retrieve the description
            DescriptionLabel.Text = String.Format(CultureConstants.DefaultCulture,
                                                  "Description: {0}", s_fittings.Fitting.Description.Text);

            // Retrieve the loadout name
            m_loadoutName = s_fittings.Fitting.Name;
            LoadoutNameLabel.Text = String.Format(CultureConstants.DefaultCulture, "Loadout Name: {0}", m_loadoutName);

            // Retrieve the ship
            string shipTypeName = s_fittings.Fitting.ShipType.Name;
            ShipTypeNameLabel.Text = String.Format(CultureConstants.DefaultCulture, "Ship: {0}", shipTypeName);
            Item ship = StaticItems.ShipsMarketGroup.AllItems.First(x => x.Name == shipTypeName);
            m_objects.Add(ship);

            foreach (SerializableFittingHardware hardware in s_fittings.Fitting.FittingHardware)
            {
                // Retrieve the item
                Item item = hardware.Item;

                // Retrieve the charge
                Item charge = hardware.Item.MarketGroup.BelongsIn(DBConstants.AmmosAndChargesMarketGroupID)
                                  ? hardware.Item
                                  : null;

                // Reset item if it is a charge
                if (charge != null)
                    item = null;

                AddNode(item, charge);
            }
        }

        /// <summary>
        /// Decodes the DNA loadout text and adds the items to the TreeView.
        /// </summary>
        private void DecodeDNAFormat()
        {
            foreach (string line in s_lines.Where(line => !String.IsNullOrEmpty(line)))
            {
                // Retrieve the ship
                if (line == s_lines.First())
                {
                    int shipID = Int32.Parse(line, CultureConstants.InvariantCulture);
                    Item ship = StaticItems.ShipsMarketGroup.AllItems.First(x => x.ID == shipID);
                    ShipTypeNameLabel.Text = String.Format(CultureConstants.DefaultCulture, "Ship: {0}", ship.Name);
                    LoadoutNameLabel.Text = String.Format(CultureConstants.DefaultCulture, "Loadout Name: {0} - DNA loadout",
                                                          ship.Name);
                    m_objects.Add(ship);
                    continue;
                }

                // Retrieve the item
                int itemID;
                byte quantity = 0;
                Item item = Int32.TryParse(line.Substring(0, line.LastIndexOf(';')), out itemID)
                                ? StaticItems.GetItemByID(itemID)
                                : null;

                if (item != null)
                    quantity = Byte.Parse(line.Substring(line.LastIndexOf(';') + 1), CultureConstants.InvariantCulture);

                // Retrieve the charge
                Item charge = item != null && item.MarketGroup.BelongsIn(DBConstants.AmmosAndChargesMarketGroupID)
                                  ? item
                                  : null;

                // Reset item if it is a charge
                if (charge != null)
                    item = null;

                for (int i = 0; i < quantity; i++)
                {
                    AddNode(item, charge);
                }
            }
        }

        /// <summary>
        /// Adds the node to the TreeView.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="charge">The charge.</param>
        private void AddNode(Item item, Item charge)
        {
            TreeNode slotNode;
            string nodeName = String.Empty;

            // Regular item ?
            if (item != null)
            {
                // Retrieve the tree node name for the slot
                switch (item.FittingSlot)
                {
                    case ItemSlot.High:
                        nodeName = m_orderedNodeNames[0];
                        break;
                    case ItemSlot.Medium:
                        nodeName = m_orderedNodeNames[1];
                        break;
                    case ItemSlot.Low:
                        nodeName = m_orderedNodeNames[2];
                        break;
                }

                // Is it a rig?
                if (item.MarketGroup.BelongsIn(DBConstants.ShipModificationsMarketGroupID))
                    nodeName = m_orderedNodeNames[3];

                // Is it a subsystem?
                if (item.MarketGroup.BelongsIn(DBConstants.SubsystemsMarketGroupID))
                    nodeName = m_orderedNodeNames[4];

                // Is it a drone?
                if (item.MarketGroup.BelongsIn(DBConstants.DronesMarketGroupID))
                    nodeName = m_orderedNodeNames[6];

                // Gets or create the node for the slot
                slotNode = !ResultsTreeView.Nodes.ContainsKey(nodeName)
                               ? ResultsTreeView.Nodes.Add(nodeName, nodeName)
                               : ResultsTreeView.Nodes[nodeName];

                // Add a new node
                TreeNode itemNode = new TreeNode { Text = item.Name, Tag = item };
                slotNode.Nodes.Add(itemNode);

                m_objects.Add(item);
            }

            // Has charges ? 
            if (charge == null)
                return;

            nodeName = m_orderedNodeNames[5];
            slotNode = !ResultsTreeView.Nodes.ContainsKey(nodeName)
                           ? ResultsTreeView.Nodes.Add(nodeName, nodeName)
                           : ResultsTreeView.Nodes[nodeName];

            TreeNode ammoNode = new TreeNode { Text = charge.Name, Tag = charge };
            slotNode.Nodes.Add(ammoNode);

            m_objects.Add(charge);
        }

        /// <summary>
        /// Updates the form controls to reflect the status of the Plan specified by the Plan property.
        /// </summary>
        private void UpdatePlanStatus()
        {
            if (s_lines.Length == 0)
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

        private enum LoadoutFormat
        {
            EFT,
            XML,
            DNA
        }

        #endregion
    }
}