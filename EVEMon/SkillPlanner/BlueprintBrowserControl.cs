using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.Data;

namespace EVEMon.SkillPlanner
{
    internal partial class BlueprintBrowserControl : EveObjectBrowserControl
    {
        private readonly object[] m_laboratories = {
                                                       "Mobile Laboratory",
                                                       "Advance Mobile Laboratory",
                                                       "Any Other Laboratory"
                                                   };

        private double m_materialFacilityMultiplier;
        private bool m_hasManufacturing;
        private bool m_hasCopying;
        private bool m_hasResearchingMaterialEfficiency;
        private bool m_hasResearchingTimeEfficiency;
        private bool m_hasInvention;

        private Character m_character;
        private Blueprint m_blueprint;
        private BlueprintActivity m_activity;
        private readonly Point m_gbManufOriginalLocation;
        private readonly Point m_gbResearchingOriginalLocation;
        private readonly Point m_gbInventionOriginalLocation;


        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public BlueprintBrowserControl()
        {
            InitializeComponent();

            lblNoItemManufacturing.Font = FontFactory.GetFont("Tahoma", 11.25F, FontStyle.Bold);
            lblNoItemCopy.Font = FontFactory.GetFont("Tahoma", 11.25F, FontStyle.Bold);
            lblNoItemME.Font = FontFactory.GetFont("Tahoma", 11.25F, FontStyle.Bold);
            lblNoItemPE.Font = FontFactory.GetFont("Tahoma", 11.25F, FontStyle.Bold);
            lblNoItemInvention.Font = FontFactory.GetFont("Tahoma", 11.25F, FontStyle.Bold);
            lblNoItemReverseEngineering.Font = FontFactory.GetFont("Tahoma", 11.25F, FontStyle.Bold);

            m_gbManufOriginalLocation = gbManufacturing.Location;
            m_gbResearchingOriginalLocation = gbResearching.Location;
            m_gbInventionOriginalLocation = gbInvention.Location;

            scObjectBrowser.RememberDistanceKey = "BlueprintBrowser_Left";
            SelectControl = blueprintSelectControl;
            PropertiesList = lvManufacturing;
        }

        #endregion


        #region Inherited Events

        /// <summary>
        /// Occurs when the control is loaded.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            // Return on design mode
            if (DesignMode || this.IsDesignModeHosted())
                return;

            // Call the base method
            base.OnLoad(e);

            lblHelp.Text = "Use the tree on the left to select a blueprint to view.";
            gbDescription.Text = "Attributes";
            pnlAttributes.AutoScroll = true;

            // Update ImplantSet Modifier
            UpdateImplantSetModifier();
        }

        /// <summary>
        /// Updates the controls when the selection is changed.
        /// </summary>
        protected override void OnSelectionChanged()
        {
            // Call the base method
            base.OnSelectionChanged();

            if (SelectedObject == null)
                return;

            m_blueprint = SelectedObject as Blueprint;

            // Update Tabs
            UpdateTabs();

            // Get the acitvity
            m_activity = GetActivity();

            // Update Required Skills
            requiredSkillsControl.Object = SelectedObject;
            requiredSkillsControl.Activity = m_activity;

            // Update Facility Modifier
            UpdateFacilityModifier();
        }

        /// <summary>
        /// Updates the plan when the selection is changed.
        /// </summary>
        protected override void OnPlanChanged()
        {
            base.OnPlanChanged();
            requiredSkillsControl.Plan = Plan;

            // We recalculate the right panels minimum size
            int reqSkillControlMinWidth = requiredSkillsControl.MinimumSize.Width;
            int reqSkillPanelMinWidth = scDetails.Panel2MinSize;
            scDetails.Panel2MinSize = (reqSkillPanelMinWidth > reqSkillControlMinWidth
                                           ? reqSkillPanelMinWidth
                                           : reqSkillControlMinWidth);
        }

        /// <summary>
        /// Updates the implant modifier list when the settings changed.
        /// </summary>
        protected override void OnSettingsChanged()
        {
            base.OnSettingsChanged();
            UpdateImplantSetModifier();
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Create the necessary tabs.
        /// </summary>
        private void UpdateTabs()
        {
            // Hide the tab control if the selected tab is not the first
            // (avoids tab creation be visible to user)
            if (tabControl.SelectedIndex != 0)
                tabControl.Hide();

            tabControl.SuspendLayout();
            try
            {
                RefreshTabs();
            }
            finally
            {
                tabControl.ResumeLayout(true);
                tabControl.Show();
            }
        }

        /// <summary>
        /// Refreshes the tabs.
        /// </summary>
        private void RefreshTabs()
        {            
            // Determine the blueprints' activities  
            m_hasManufacturing = m_blueprint.Prerequisites.Any(x => x.Activity == BlueprintActivity.Manufacturing)
                              || m_blueprint.MaterialRequirements.Any(x => x.Activity == BlueprintActivity.Manufacturing)
                              || m_blueprint.ProductionTime > 0d;

            m_hasCopying = m_blueprint.Prerequisites.Any(x => x.Activity == BlueprintActivity.Copying)
                              || m_blueprint.MaterialRequirements.Any(x => x.Activity == BlueprintActivity.Copying)
                              || m_blueprint.ResearchCopyTime > 0d;

            m_hasResearchingMaterialEfficiency =
                m_blueprint.Prerequisites.Any(x => x.Activity == BlueprintActivity.ResearchingMaterialEfficiency)
                || m_blueprint.MaterialRequirements.Any(x => x.Activity == BlueprintActivity.ResearchingMaterialEfficiency)
                || m_blueprint.ResearchMaterialTime > 0d;

            m_hasResearchingTimeEfficiency =
                m_blueprint.Prerequisites.Any(x => x.Activity == BlueprintActivity.ResearchingTimeEfficiency)
                || m_blueprint.MaterialRequirements.Any(x => x.Activity == BlueprintActivity.ResearchingTimeEfficiency)
                || m_blueprint.ResearchProductivityTime > 0d;

            m_hasInvention = m_blueprint.Prerequisites.Any(x => x.Activity == BlueprintActivity.Invention)
                             || m_blueprint.MaterialRequirements.Any(x => x.Activity == BlueprintActivity.Invention)
                             || m_blueprint.ResearchInventionTime > 0d;

            // Store the visible selector control for later use
            Control visibleSelector;
            if (blueprintSelectControl.tvItems.Visible)
                visibleSelector = blueprintSelectControl.tvItems;
            else
                visibleSelector = blueprintSelectControl.lbSearchList;

            // Store the selected tab index for later use
            int storedTabIndex = tabControl.SelectedIndex;

            tabControl.TabPages.Clear();

            // Add the appropriate tabs
            if (m_hasManufacturing)
                tabControl.TabPages.Add(tpManufacturing);

            if (m_hasCopying)
                tabControl.TabPages.Add(tpCopying);

            if (m_hasResearchingMaterialEfficiency)
                tabControl.TabPages.Add(tpResearchME);

            if (m_hasResearchingTimeEfficiency)
                tabControl.TabPages.Add(tpResearchPE);

            if (!m_hasCopying && !m_hasResearchingMaterialEfficiency && !m_hasResearchingTimeEfficiency)
                tabControl.TabPages.Add(tpReverseEngineering);

            if (m_hasInvention)
                tabControl.TabPages.Add(tpInvention);

            // Restore the index of the previous selected tab,
            // if the index doesn't exist it smartly selects
            // the first one by its own
            tabControl.SelectedIndex = storedTabIndex;

            // Return focus to selector
            visibleSelector.Focus();
        }

        /// <summary>
        /// Update the attributes info.
        /// </summary>
        private void UpdateAttributes()
        {
            // Produce item
            lblItem.ForeColor = Color.Blue;
            lblItem.Text = m_blueprint.ProducesItem == null || m_blueprint.ProducesItem.ID == 0
                ? String.Empty
                : m_blueprint.ProducesItem.Name;
            lblItem.Tag = m_blueprint.ProducesItem;

            // Invented blueprints
            InventBlueprintListBox.Items.Clear();
            foreach (KeyValuePair<Blueprint, double> item in m_blueprint.InventBlueprints)
            {
                InventBlueprintListBox.Items.Add(item.Key);
            }

            // Success probability
            lblSuccessProbability.Visible = lblProbability.Visible = m_blueprint.InventBlueprints.Any();
            if (lblProbability.Visible)
                lblProbability.Text = m_blueprint.InventBlueprints.Max(x => x.Value).ToString("P1");

            // Runs per copy
            lblRunsPerCopy.Text = m_blueprint.RunsPerCopy.ToString(CultureConstants.DefaultCulture);

            // Manufacturing base time
            double timeEffModifier = 1 - ((double)nudTE.Value / 100);
            double activityTime = m_blueprint.ProductionTime * timeEffModifier * GetFacilityMultiplier();
            lblProductionBaseTime.Text = BaseActivityTime(activityTime);

            // Manufacturing character time
            activityTime *= GetImplantMultiplier(DBConstants.ManufacturingModifyingImplantIDs);
            lblProductionCharTime.Text = CharacterActivityTime(activityTime, DBConstants.IndustrySkillID);

            // Researching material efficiency base time
            activityTime = m_blueprint.ResearchMaterialTime *
                           GetFacilityMultiplier(BlueprintActivity.ResearchingMaterialEfficiency);
            lblResearchMEBaseTime.Text = BaseActivityTime(activityTime);

            // Researching material efficiency character time
            activityTime *= GetImplantMultiplier(DBConstants.ResearchMaterialEfficiencyTimeModifyingImplantIDs);
            lblResearchMECharTime.Text = CharacterActivityTime(activityTime, DBConstants.MetallurgySkillID);

            // Researching copy base time
            activityTime = m_blueprint.ResearchCopyTime * GetFacilityMultiplier(BlueprintActivity.Copying);
            lblResearchCopyBaseTime.Text = BaseActivityTime(activityTime);

            // Researching copy character time
            activityTime *= GetImplantMultiplier(DBConstants.ResearchCopyTimeModifyingImplantIDs);
            lblResearchCopyCharTime.Text = CharacterActivityTime(activityTime, DBConstants.ScienceSkillID);

            // Researching time efficiency base time
            activityTime = m_blueprint.ResearchProductivityTime *
                           GetFacilityMultiplier(BlueprintActivity.ResearchingTimeEfficiency);
            lblResearchTEBaseTime.Text = BaseActivityTime(activityTime);

            // Researching time efficiency character time
            activityTime *= GetImplantMultiplier(DBConstants.ResearchTimeEfficiencyTimeModifyingImplantIDs);
            lblResearchTECharTime.Text = CharacterActivityTime(activityTime, DBConstants.ResearchSkillID);

            gbManufacturing.Visible = m_hasManufacturing;
            gbResearching.Location = gbManufacturing.Visible
                ? m_gbResearchingOriginalLocation
                : m_gbManufOriginalLocation;
            gbResearching.Visible = m_hasCopying || m_hasResearchingMaterialEfficiency || m_hasResearchingTimeEfficiency;
            gbInvention.Text = (gbResearching.Visible ? "INVENTION" : "REVERSE ENGINEERING");
            lblInventionTime.Text = (gbResearching.Visible ? "Invention Time:" : "Reverse Engineering Time:");
            lblInvention.Text = (gbResearching.Visible ? "Invents:" : "Reverse Engineers:");
            gbInvention.Location = (gbResearching.Visible ? m_gbInventionOriginalLocation : gbResearching.Location);
            gbInvention.Visible = (!gbResearching.Visible || m_hasInvention);

            if (!gbInvention.Visible)
                return;

            // Invention or Reverse Engineering time base time
            activityTime = (m_hasInvention
                ? m_blueprint.ResearchInventionTime
                : m_blueprint.ReverseEngineeringTime) *
                           GetFacilityMultiplier(m_hasInvention
                               ? BlueprintActivity.Invention
                               : BlueprintActivity.ReverseEngineering);
            lblInventionBaseTime.Text = BaseActivityTime(activityTime);

            // Invention or Reverse Engineering character time
            activityTime *= 1;
            lblInventionCharTime.Text = CharacterActivityTime(activityTime);


        }

        /// <summary>
        /// Update the required materials list.
        /// </summary>
        private void UpdateRequiredMaterialsList()
        {
            int scrollBarPosition = PropertiesList.GetVerticalScrollBarPosition();

            // Store the selected item (if any) to restore it after the update
            int selectedItem = (PropertiesList.SelectedItems.Count > 0
                                    ? PropertiesList.SelectedItems[0].Tag.GetHashCode()
                                    : 0);

            PropertiesList.BeginUpdate();
            try
            {
                // Clear everything
                PropertiesList.Items.Clear();
                PropertiesList.Groups.Clear();
                PropertiesList.Columns.Clear();

                // Create the columns
                PropertiesList.Columns.Add("item","Item");
                PropertiesList.Columns.Add("qBase", "Quantity (Base)");
                PropertiesList.Columns.Add("quant", "Quantity (You)");

                IEnumerable<ListViewItem> items = AddGroups();

                // Add the items
                PropertiesList.Items.AddRange(items.OrderBy(x => x.Text).ToArray());

                // Show/Hide the "no item required" label and autoresize the columns 
                PropertiesList.Visible = PropertiesList.Items.Count > 0;

                if (PropertiesList.Items.Count > 0)
                    AdjustColumns();

                if (selectedItem <= 0)
                    return;

                // Restore the selected item (if any)
                foreach (ListViewItem lvItem in PropertiesList.Items.Cast<ListViewItem>().Where(
                    lvItem => lvItem.Tag.GetHashCode() == selectedItem))
                {
                    lvItem.Selected = true;
                }
            }
            finally
            {
                PropertiesList.EndUpdate();
                PropertiesList.SetVerticalScrollBarPosition(scrollBarPosition);
            }
        }

        /// <summary>
        /// Adds the groups.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<ListViewItem> AddGroups()
        {
            List<ListViewItem> items = new List<ListViewItem>();
            double materiaEffModifier = 1 - ((double)nudME.Value / 100);

            foreach (MarketGroup marketGroup in StaticItems.AllGroups)
            {
                // Create the groups
                ListViewGroup group = new ListViewGroup(marketGroup.CategoryPath);
                bool hasItem = false;
                foreach (StaticRequiredMaterial material in m_blueprint.MaterialRequirements.Where(
                    material => material.Activity == m_activity && marketGroup.Items.Any(y => y.ID == material.ID)))
                {
                    hasItem = true;

                    // Create the item
                    ListViewItem item = new ListViewItem(group)
                    { Tag = StaticItems.GetItemByID(material.ID), Text = material.Name };

                    // Add the item to the list
                    items.Add(item);

                    // Calculate the base material quantity
                    long baseMaterialQuantity = material.Quantity;

                    // Calculate the base material quantity
                    long actualMaterialQuantity = (long)Math.Ceiling(material.Quantity * m_materialFacilityMultiplier * materiaEffModifier);

                    // Add the base quantity for every item
                    ListViewItem.ListViewSubItem subItemBase =
                        new ListViewItem.ListViewSubItem(item, baseMaterialQuantity.ToString(CultureConstants.DefaultCulture));
                    item.SubItems.Add(subItemBase);

                    // Add the quantity needed by according to the charater's skiils for every item
                    ListViewItem.ListViewSubItem subItem =
                        new ListViewItem.ListViewSubItem(item, actualMaterialQuantity.ToString(CultureConstants.DefaultCulture));
                    item.SubItems.Add(subItem);
                }

                // Add the group that has an item
                if (hasItem)
                    PropertiesList.Groups.Add(group);
            }

            return items;
        }

        /// <summary>
        /// Update the facility modifier list. 
        /// </summary>
        private void UpdateFacilityModifier()
        {
            cbFacility.Items.Clear();

            cbFacility.Items.Add("NPC Station");

            Item producedItem = m_blueprint.ProducesItem;

            // This should not happen but be prepared if something changes in CCP DB
            if (producedItem == null)
            {
                cbFacility.SelectedIndex = 0;
                return;
            }

            switch (m_activity)
            {
                case BlueprintActivity.Manufacturing:
                    if (producedItem.MarketGroup.BelongsIn(DBConstants.DronesMarketGroupID)
                        && !producedItem.MarketGroup.BelongsIn(DBConstants.SmallToXLargeShipsMarketGroupIDs))
                        cbFacility.Items.Add("Drone Assembly Array");

                    if (producedItem.MarketGroup.BelongsIn(DBConstants.AmmosAndChargesMarketGroupID))
                        cbFacility.Items.Add("Ammunition Assembly Array");

                    if (producedItem.MarketGroup.BelongsIn(DBConstants.ShipEquipmentsMarketGroupID))
                    {
                        cbFacility.Items.Add("Equipment Assembly Array");
                        cbFacility.Items.Add("Rapid Equipment Assembly Array");
                    }

                    if (producedItem.MarketGroup.BelongsIn(DBConstants.ComponentsMarketGroupID))
                        cbFacility.Items.Add("Component Assembly Array");

                    if (producedItem.MarketGroup.BelongsIn(DBConstants.StrategicComponentsMarketGroupIDs))
                        cbFacility.Items.Add("Subsystem Assembly Array");

                    if (producedItem.MarketGroup.BelongsIn(DBConstants.SmallToXLargeShipsMarketGroupIDs))
                        cbFacility.Items.Add("Ship Assembly Array (Ship Size)");

                    if (producedItem.MarketGroup.BelongsIn(DBConstants.CapitalShipsMarketGroupIDs))
                        cbFacility.Items.Add("Capital Assembly Array");

                    if (producedItem.MarketGroup.BelongsIn(DBConstants.AdvancedSmallToLargeShipsMarketGroupIDs))
                        cbFacility.Items.Add("Advanced Ship Assembly Array (Ship Size)");

                    break;
                default:
                    cbFacility.Items.AddRange(m_laboratories);
                    break;
            }

            // Update the selected index
            if (m_activity == BlueprintActivity.Manufacturing)
            {
                cbFacility.SelectedIndex = (Settings.UI.BlueprintBrowser.ProductionFacilityIndex <
                                            cbFacility.Items.Count
                                                ? Settings.UI.BlueprintBrowser.ProductionFacilityIndex
                                                : 0);
            }
            else
                cbFacility.SelectedIndex = Settings.UI.BlueprintBrowser.ResearchFacilityIndex;
        }

        /// <summary>
        /// Update the implant set modifier list. 
        /// </summary>
        private void UpdateImplantSetModifier()
        {
            m_character = (Character)Plan.Character;
            cbImplantSet.Items.Clear();
            foreach (ImplantSet set in m_character.ImplantSets)
            {
                cbImplantSet.Items.Add(set);
            }

            // Update the selected index
            cbImplantSet.SelectedIndex = (Settings.UI.BlueprintBrowser.ImplantSetIndex < cbImplantSet.Items.Count
                                              ? Settings.UI.BlueprintBrowser.ImplantSetIndex
                                              : 0);
        }

        /// <summary>
        /// Calculate the base activity time.
        /// </summary>
        /// <param name="activityTime"></param>
        /// <returns></returns>
        private static string BaseActivityTime(double activityTime)
        {
            if (Double.IsNaN(activityTime))
                return TimeSpanToText(TimeSpan.FromSeconds(0d), false);

            TimeSpan time = TimeSpan.FromSeconds(activityTime);
            return TimeSpanToText(time, time.Seconds != 0);
        }

        /// <summary>
        /// Calculate the character's activity time.
        /// </summary>
        /// <param name="activityTime">The activity time.</param>
        /// <param name="skillID">The skill ID.</param>
        /// <returns></returns>
        private string CharacterActivityTime(double activityTime, int skillID = 0)
        {
            double activityTimeModifier = 1d;
            const double AdvancedIndustrySkillBonusFactor = 0.03d;
            if (skillID != 0)
            {
                double skillBonusFactor;
                switch (skillID)
                {
                    case DBConstants.IndustrySkillID:
                        skillBonusFactor = 0.04d;
                        break;
                    default:
                        skillBonusFactor = 0.05d;
                        break;
                }

                Int64 skillLevel = m_character.Skills[skillID].LastConfirmedLvl;
                Int64 advancedIndustrySkillLevel = m_character.Skills[DBConstants.AdvancedIndustrySkillID].LastConfirmedLvl;
                activityTimeModifier = (1d - (skillBonusFactor * skillLevel)) *
                                       (1d - (AdvancedIndustrySkillBonusFactor * advancedIndustrySkillLevel));
            }

            TimeSpan time = TimeSpan.FromSeconds(activityTime * activityTimeModifier);
            return String.Format(CultureConstants.DefaultCulture, "{0} (You)", TimeSpanToText(time, time.Seconds != 0));
        }

        /// <summary>
        /// Transpose the timespan to text.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="includeSeconds"></param>
        /// <returns></returns>
        private static string TimeSpanToText(TimeSpan time, bool includeSeconds)
        {
            return time.ToDescriptiveText(
                DescriptiveTextOptions.FirstLetterUppercase
                | DescriptiveTextOptions.SpaceText
                | DescriptiveTextOptions.FullText
                | DescriptiveTextOptions.IncludeCommas,
                includeSeconds);
        }

        /// <summary>
        /// Gets the activity from the selected tab.
        /// </summary>
        /// <returns></returns>
        private BlueprintActivity GetActivity()
        {
            if (tabControl.SelectedTab == null)
                return BlueprintActivity.None;

            switch (tabControl.SelectedTab.Text)
            {
                case "Manufacturing":
                    PropertiesList = lvManufacturing;
                    return BlueprintActivity.Manufacturing;
                case "Copying":
                    PropertiesList = lvCopying;
                    return BlueprintActivity.Copying;
                case "Researching Material Efficiency":
                    PropertiesList = lvResearchME;
                    return BlueprintActivity.ResearchingMaterialEfficiency;
                case "Researching Time Efficiency":
                    PropertiesList = lvResearchPE;
                    return BlueprintActivity.ResearchingTimeEfficiency;
                case "Invention":
                    PropertiesList = lvInvention;
                    return BlueprintActivity.Invention;
                case "Reverse Engineering":
                    PropertiesList = lvReverseEngineering;
                    return BlueprintActivity.ReverseEngineering;
                default:
                    PropertiesList = lvManufacturing;
                    return BlueprintActivity.Manufacturing;
            }
        }

        /// <summary>
        /// Show the item in its appropriate browser.
        /// </summary>
        /// <param name="item"></param>
        private void ShowInBrowser(Item item)
        {
            PlanWindow planWindow = WindowsFactory.GetByTag<PlanWindow, Plan>(Plan);
            if (planWindow == null || planWindow.IsDisposed)
                return;

            if (item is Ship)
                planWindow.ShowShipInBrowser(item);
            else
                planWindow.ShowItemInBrowser(item);
        }

        /// <summary>
        /// Gets the manufacturing time and material multiplier of a facility.
        /// </summary>
        /// <returns></returns>
        private double GetFacilityMultiplier()
        {
            string text = cbFacility.Text;
            m_materialFacilityMultiplier = 1.0d;

            if (m_activity != BlueprintActivity.Manufacturing)
                return 1.0d;

            if (text.StartsWith("Rapid", StringComparison.Ordinal))
            {
                m_materialFacilityMultiplier = 1.2d;
                return 0.65d;
            }

            if (text.StartsWith("Subsystem", StringComparison.Ordinal) ||
                text.StartsWith("Capital", StringComparison.Ordinal) ||
                text.StartsWith("NPC", StringComparison.Ordinal))
            {
                return 1.0d;
            }

            if (text.StartsWith("Advanced", StringComparison.Ordinal))
                m_materialFacilityMultiplier = 1.1d;

            return 0.75d;
        }

        /// <summary>
        /// Gets the research time multiplier of a facility.
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        private double GetFacilityMultiplier(BlueprintActivity activity)
        {
            string text = cbFacility.Text;

            if (text.StartsWith("Mobile", StringComparison.Ordinal))
            {
                switch (activity)
                {
                    case BlueprintActivity.Invention:
                        return 0.5d;
                    default:
                        return 0.75d;
                }
            }

            if (!text.StartsWith("Advance Mobile", StringComparison.Ordinal))
                return 1.0d;

            switch (activity)
            {
                case BlueprintActivity.ResearchingMaterialEfficiency:
                    return 0.75d;
                case BlueprintActivity.Copying:
                    return 0.65d;
                case BlueprintActivity.Invention:
                    return 0.5d;
            }

            return 1.0d;
        }

        /// <summary>
        /// Gets the multiplier of an implant.
        /// </summary>
        /// <returns></returns>
        private double GetImplantMultiplier(ICollection<int> implantIDs)
        {
            ImplantSet implantSet = (ImplantSet)cbImplantSet.Tag;

            Implant implant = implantSet.FirstOrDefault(x => implantIDs.Contains(x.ID));

            if (implant == null)
                return 1.0d;

            double bonus = implant.Properties.FirstOrDefault(
                x => DBConstants.IndustryModifyingPropertyIDs.IndexOf(x.Property.ID) != -1).Int64Value;
            double multiplier = 1.0d + (bonus / 100);

            return multiplier;
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// Occurs when a tab is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Updates the required skills control according to the seleted tab</remarks>
        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex == -1)
                return;

            m_activity = GetActivity();
            requiredSkillsControl.Activity = m_activity;
            UpdateFacilityModifier();
        }

        /// <summary>
        /// Occurs when the value of the control changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nudME_ValueChanged(object sender, EventArgs e)
        {
            int value = (int)((NumericUpDown)sender).Value;

            switch (value)
            {
                case -1:
                case -49:
                    ((NumericUpDown)sender).Value = -25;
                    break;
                case -26:
                case -74:
                    ((NumericUpDown)sender).Value = -50;
                    break;
                case -51:
                case -99:
                    ((NumericUpDown)sender).Value = -75;
                    break;
                case -76:
                    ((NumericUpDown)sender).Value = -100;
                    break;
                case -24:
                    ((NumericUpDown)sender).Value = 0;
                    break;
            }

            UpdateAttributes();
            UpdateRequiredMaterialsList();
        }

        /// <summary>
        /// Occurs when the value of the control changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nudTE_ValueChanged(object sender, EventArgs e)
        {
            int value = (int)((NumericUpDown)sender).Value;

            switch (value)
            {
                case -2:
                case -98:
                    ((NumericUpDown)sender).Value = -50;
                    break;
                case -52:
                    ((NumericUpDown)sender).Value = -100;
                    break;
                case -48:
                    ((NumericUpDown)sender).Value = 0;
                    break;
            }

            UpdateAttributes();
        }

        /// <summary>
        /// Occurs on facility selection change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbFacility_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_activity == BlueprintActivity.Manufacturing)
                Settings.UI.BlueprintBrowser.ProductionFacilityIndex = cbFacility.SelectedIndex;
            else
                Settings.UI.BlueprintBrowser.ResearchFacilityIndex = cbFacility.SelectedIndex;

            UpdateAttributes();
            UpdateRequiredMaterialsList();
        }

        /// <summary>
        /// Occurs on implant set selection change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbImplantSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.UI.BlueprintBrowser.ImplantSetIndex = cbImplantSet.SelectedIndex;
            cbImplantSet.Tag = cbImplantSet.SelectedItem;

            if (m_blueprint == null)
                return;

            UpdateAttributes();
            UpdateRequiredMaterialsList();
        }

        /// <summary>
        /// Occurs when clicking on the control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblItem_Click(object sender, EventArgs e)
        {
            Item item = (Item)lblItem.Tag;

            if (item == null)
                return;

            ShowInBrowser(item);

            lblItem.ForeColor = Color.Purple;
        }

        /// <summary>
        /// Occurs when clicking on a list box item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbInventBlueprint_SelectedIndexChanged(object sender, EventArgs e)
        {
            Blueprint blueprint = (Blueprint)InventBlueprintListBox.SelectedItem;

            if (blueprint == null)
                return;

            SelectedObject = blueprint;
            InventBlueprintListBox.ClearSelected();
        }

        /// <summary>
        /// Occurs when double clicking on a list view item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void propertiesList_DoubleClick(object sender, EventArgs e)
        {
            ListViewItem listItem = ((ListView)sender).FocusedItem;
            Item item = (Item)listItem.Tag;

            if (item == null)
                return;

            ShowInBrowser(item);
        }

        /// <summary>
        /// Exports activity info to CSV format.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportToCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewExporter.CreateCSV(PropertiesList);
        }

        #endregion
    }
}