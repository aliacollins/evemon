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
            lblNoItemTE.Font = FontFactory.GetFont("Tahoma", 11.25F, FontStyle.Bold);
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

            if (m_hasResearchingMaterialEfficiency)
                tabControl.TabPages.Add(tpResearchME);

            if (m_hasResearchingTimeEfficiency)
                tabControl.TabPages.Add(tpResearchTE);

            if (m_hasCopying)
                tabControl.TabPages.Add(tpCopying);

            if (m_hasInvention)
                tabControl.TabPages.Add(tpInvention);

            if (!m_hasCopying && !m_hasResearchingMaterialEfficiency && !m_hasResearchingTimeEfficiency && !m_hasInvention)
                tabControl.TabPages.Add(tpReverseEngineering);

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
            {
                Double baseProbability = m_blueprint.InventBlueprints.Max(x => x.Value);
                lblProbability.Text = String.Format(CultureConstants.DefaultCulture, "{0:P1} (You: {1:P1})", baseProbability,
                    baseProbability * GetProbabilityModifier());
            }

            // Runs per copy
            lblRunsPerCopy.Text = m_blueprint.RunsPerCopy.ToString(CultureConstants.DefaultCulture);

            // Manufacturing base time
            double activityTime = (int)(m_blueprint.ProductionTime *
                                        GetTimeEfficiencyModifier(BlueprintActivity.Manufacturing)) *
                                  GetFacilityMultiplier();
            lblProductionBaseTime.Text = BaseActivityTime(activityTime);

            // Manufacturing character time
            activityTime *= GetImplantMultiplier(DBConstants.ManufacturingModifyingImplantIDs);
            lblProductionCharTime.Text = CharacterActivityTime(activityTime, DBConstants.IndustrySkillID);

            // Researching material efficiency base time
            activityTime = (int)(m_blueprint.ResearchMaterialTime *
                                 GetTimeEfficiencyModifier(BlueprintActivity.ResearchingMaterialEfficiency)) *
                           GetFacilityMultiplier(BlueprintActivity.ResearchingMaterialEfficiency);
            lblResearchMEBaseTime.Text = BaseActivityTime(activityTime);

            // Researching material efficiency character time
            activityTime *= GetImplantMultiplier(DBConstants.ResearchMaterialEfficiencyTimeModifyingImplantIDs);
            lblResearchMECharTime.Text = CharacterActivityTime(activityTime, DBConstants.MetallurgySkillID);

            // Researching copy base time
            activityTime = (int)(m_blueprint.ResearchCopyTime *
                                 GetTimeEfficiencyModifier(BlueprintActivity.Copying)) *
                           GetFacilityMultiplier(BlueprintActivity.Copying);
            lblResearchCopyBaseTime.Text = BaseActivityTime(activityTime);

            // Researching copy character time
            activityTime *= GetImplantMultiplier(DBConstants.ResearchCopyTimeModifyingImplantIDs);
            lblResearchCopyCharTime.Text = CharacterActivityTime(activityTime, DBConstants.ScienceSkillID);

            // Researching time efficiency base time
            activityTime = (int)(m_blueprint.ResearchProductivityTime *
                                 GetTimeEfficiencyModifier(BlueprintActivity.ResearchingTimeEfficiency)) *
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
            activityTime *= 1d;
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
            double materiaEffModifier = 1d - ((double)nudME.Value / 100);

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
                    long actualMaterialQuantity = (long)Math.Ceiling(material.Quantity * materiaEffModifier * m_materialFacilityMultiplier);

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
                        && !producedItem.MarketGroup.BelongsIn(DBConstants.SmallToLargeShipsMarketGroupIDs))
                    {
                        cbFacility.Items.Add("Drone Assembly Array");
                    }

                    if (producedItem.MarketGroup.BelongsIn(DBConstants.AmmosAndChargesMarketGroupID) ||
                        producedItem.MarketGroup.BelongsIn(DBConstants.FuelBlocksMarketGroupID))
                    {
                        cbFacility.Items.Add("Ammunition Assembly Array");
                    }

                    if (producedItem.MarketGroup.BelongsIn(DBConstants.ShipEquipmentsMarketGroupID))
                    {
                        cbFacility.Items.Add("Equipment Assembly Array");
                        cbFacility.Items.Add("Rapid Equipment Assembly Array");
                    }

                    if (producedItem.MarketGroup.BelongsIn(DBConstants.ComponentsMarketGroupID))
                        cbFacility.Items.Add("Component Assembly Array");

                    if (producedItem.MarketGroup.BelongsIn(DBConstants.StrategicComponentsMarketGroupIDs))
                        cbFacility.Items.Add("Subsystem Assembly Array");

                    if (producedItem.MarketGroup.BelongsIn(DBConstants.SmallToLargeShipsMarketGroupIDs))
                        cbFacility.Items.Add("(Ship Size) Ship Assembly Array");

                    if (producedItem.MarketGroup.BelongsIn(DBConstants.CapitalShipsMarketGroupIDs))
                        cbFacility.Items.Add("Capital Assembly Array");

                    if (producedItem.MarketGroup.BelongsIn(DBConstants.AdvancedSmallToLargeShipsMarketGroupIDs))
                        cbFacility.Items.Add("Advanced (Ship Size) Ship Assembly Array");

                    if (producedItem.MarketGroup.BelongsIn(DBConstants.SupercapitalShipsMarketGroupIDs))
                        cbFacility.Items.Add("Supercapital Assembly Array");

                    if (producedItem.MarketGroup.BelongsIn(DBConstants.StandardCapitalShipComponentsMarketGroupID) ||
                        producedItem.MarketGroup.BelongsIn(DBConstants.AdvancedCapitalComponentsMarketGroupID))
                    {
                        cbFacility.Items.Add("Thukker Component Assembly Array");
                    }

                    if (producedItem.MarketGroup.BelongsIn(DBConstants.BoostersMarketGroupID))
                        cbFacility.Items.Add("Drug Laboratory");

                    break;
                case BlueprintActivity.Copying:
                case BlueprintActivity.Invention:
                    cbFacility.Items.AddRange(new object[]
                    {
                        "Design Laboratory"
                    });
                    break;
                case BlueprintActivity.ReverseEngineering:
                    cbFacility.Items.AddRange(new object[]
                    {
                        "Experimental Laboratory"
                    });
                    break;
                case BlueprintActivity.ResearchingMaterialEfficiency:
                case BlueprintActivity.ResearchingTimeEfficiency:
                    cbFacility.Items.AddRange(new object[]
                    {
                        "Research Laboratory",
                        "Hyasyoda Research Laboratory"
                    });
                    break;
            }

            // Update the selected index
            if (m_activity == BlueprintActivity.Manufacturing)
            {
                cbFacility.SelectedIndex = Settings.UI.BlueprintBrowser.ProductionFacilityIndex < cbFacility.Items.Count
                    ? Settings.UI.BlueprintBrowser.ProductionFacilityIndex
                    : 0;
            }
            else
            {
                cbFacility.SelectedIndex = Settings.UI.BlueprintBrowser.ResearchFacilityIndex < cbFacility.Items.Count
                    ? Settings.UI.BlueprintBrowser.ResearchFacilityIndex
                    : 0;
            }
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

            TimeSpan time = TimeSpan.FromSeconds(Math.Ceiling(activityTime));
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
            Int64 advancedIndustrySkillLevel = m_character.Skills[DBConstants.AdvancedIndustrySkillID].LastConfirmedLvl;
            const Double AdvancedIndustrySkillBonusFactor = 0.03d;
            Double activityTimeModifier = 1d;
            Double skillBonusModifier = 0d;

            if (skillID != 0)
            {
                Double skillBonusFactor;
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
                skillBonusModifier = skillBonusFactor * skillLevel;
            }
            activityTimeModifier = (activityTimeModifier - (skillBonusModifier)) *
                                   (activityTimeModifier - (AdvancedIndustrySkillBonusFactor * advancedIndustrySkillLevel));

            TimeSpan time = TimeSpan.FromSeconds(Math.Ceiling(activityTime * activityTimeModifier));
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
            return time.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas, includeSeconds);
        }

        /// <summary>
        /// Gets the probability modifier.
        /// </summary>
        /// <returns></returns>
        private double GetProbabilityModifier()
        {
            const Double BonusFactor = 0.05d;
            Double skillLevel = m_blueprint.Prerequisites
                .Where(x => x.Activity == BlueprintActivity.Invention || x.Activity == BlueprintActivity.ReverseEngineering)
                .Where(x => x.Skill != null)
                .Max(x => m_character.Skills[x.Skill.ID].LastConfirmedLvl);

            return 1d + (BonusFactor * skillLevel);
        }

        /// <summary>
        /// Gets the time efficiency modifier.
        /// </summary>
        /// <param name="activity">The activity.</param>
        /// <returns></returns>
        private double GetTimeEfficiencyModifier(BlueprintActivity activity)
        {
            if (activity == BlueprintActivity.Manufacturing)
                return 1d - ((double)nudTE.Value / 100);

            if (activity == BlueprintActivity.ResearchingMaterialEfficiency)
            {
                switch ((int)nudME.Value)
                {
                    case 0:
                        return 1d;
                    case 1:
                    case -10:
                        return (250 - 105) / 105d;
                    case 2:
                    case -20:
                        return (595d - 250d) / 105d;
                    case 3:
                    case -30:
                        return (1414 - 595) / 105d;
                    case 4:
                    case -40:
                        return (3360 - 1414) / 105d;
                    case 5:
                    case -50:
                        return (8000 - 3360) / 105d;
                    case 6:
                    case -60:
                        return (19000 - 8000) / 105d;
                    case 7:
                    case -70:
                        return (45255 - 19000) / 105d;
                    case 8:
                    case -80:
                        return (107700 - 45255) / 105d;
                    case 9:
                    case 10:
                    case -90:
                    case -100:
                        return (256000 - 107700) / 105d;
                }
            }

            if (activity == BlueprintActivity.ResearchingTimeEfficiency)
            {
                switch ((int)nudTE.Value)
                {
                    case 0:
                        return 1d;
                    case 2:
                        return (250 - 105) / 105d;
                    case 4:
                        return (595d - 250d) / 105d;
                    case 6:
                        return (1414 - 595) / 105d;
                    case 8:
                        return (3360 - 1414) / 105d;
                    case 10:
                    case -50:
                        return (8000 - 3360) / 105d;
                    case 12:
                        return (19000 - 8000) / 105d;
                    case 14:
                        return (45255 - 19000) / 105d;
                    case 16:
                        return (107700 - 45255) / 105d;
                    case 18:
                    case 20:
                    case -100:
                        return (256000 - 107700) / 105d;
                }
            }

            return 1d;
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
                    PropertiesList = lvResearchTE;
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
                m_materialFacilityMultiplier = 1.05d;
                return 0.65d;
            }

            if (text.StartsWith("Thukker Component", StringComparison.Ordinal))
            {
                m_materialFacilityMultiplier = 0.9d;
                return 0.75d;
            }

            if (text.StartsWith("Subsystem", StringComparison.Ordinal) ||
                text.StartsWith("Supercapital", StringComparison.Ordinal) ||
                text.StartsWith("Drug", StringComparison.Ordinal) ||
                text.StartsWith("NPC", StringComparison.Ordinal))
            {
                return 1.0d;
            }

            m_materialFacilityMultiplier = 0.98d;
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

            if (text.StartsWith("Design", StringComparison.Ordinal))
            {
                switch (activity)
                {
                    case BlueprintActivity.Invention:
                        return 0.5d;
                    case BlueprintActivity.Copying:
                        return 0.6d;
                    default:
                        return 1;
                }
            }

            if (!text.Contains("Research Laboratory"))
                return 1.0d;

            switch (activity)
            {
                case BlueprintActivity.ResearchingMaterialEfficiency:
                case BlueprintActivity.ResearchingTimeEfficiency:
                    return text.StartsWith("Hyasyoda") ? 0.65d : 0.70d;
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
                case -19:
                    ((NumericUpDown)sender).Value = -10;
                    break;
                case -11:
                case -29:
                    ((NumericUpDown)sender).Value = -20;
                    break;
                case -21:
                case -39:
                    ((NumericUpDown)sender).Value = -30;
                    break;
                case -31:
                case -49:
                    ((NumericUpDown)sender).Value = -40;
                    break;
                case -41:
                case -59:
                    ((NumericUpDown)sender).Value = -50;
                    break;
                case -51:
                case -69:
                    ((NumericUpDown)sender).Value = -60;
                    break;
                case -61:
                case -79:
                    ((NumericUpDown)sender).Value = -70;
                    break;
                case -71:
                case -89:
                    ((NumericUpDown)sender).Value = -80;
                    break;
                case -81:
                case -99:
                    ((NumericUpDown)sender).Value = -90;
                    break;
                case -91:
                    ((NumericUpDown)sender).Value = -100;
                    break;
                case -9:
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