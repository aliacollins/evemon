using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using EVEMon.Common;
using EVEMon.Common.Data;
using EVEMon.Controls;

namespace EVEMon.SkillPlanner
{
    /// <summary>
    /// Base class for EveObject browsers.
    /// Provides basic split container layout and item header including icon, name and category, 
    /// along with event handling for item selection and worksafeMode changes. Derived classes
    /// should override DisplayItemDetails() as required
    /// </summary>
    /// <remarks>
    /// Should be an abstract class but Visual Studio Designer throws a wobbler when you
    /// try to design a class that inherits from an abstract class.</remarks>
    public partial class EveObjectBrowserControl : UserControl
    {
        protected const int Pad = 3;

        private Plan m_plan;


        #region Initialization

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected EveObjectBrowserControl()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Unsubscribe events on disposing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDisposed(object sender, EventArgs e)
        {
            SelectControl.SelectionChanged -= OnSelectionChanged;
            EveMonClient.SettingsChanged -= EveMonClient_SettingsChanged;
            Disposed -= OnDisposed;
        }

        /// <summary>
        /// Occurs when the control is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveObjectBrowserControl_Load(object sender, EventArgs e)
        {
            if (DesignMode || this.IsDesignModeHosted())
                return;

            ListViewHelper.EnableDoubleBuffer(PropertiesList);

            lblEveObjName.Font = FontFactory.GetFont("Tahoma", 11.25F, FontStyle.Bold);

            // Watch for selection changes
            SelectControl.SelectionChanged += OnSelectionChanged;

            EveMonClient.SettingsChanged += EveMonClient_SettingsChanged;
            Disposed += OnDisposed;

            // Reposition the help text along side the treeview
            Control[] result = SelectControl.Controls.Find("lowerPanel", true);
            if (result.Length > 0)
                lblHelp.Location = new Point(lblHelp.Location.X, result[0].Location.Y);

            // Updates the controls visibility
            UpdateControlsVisibility();

            // Force a refresh
            OnSelectionChanged(null, null);
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// Occurs when the settings changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void EveMonClient_SettingsChanged(object sender, EventArgs e)
        {
            UpdateControlsVisibility();
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets or sets the current plan for this planner window.
        /// </summary>
        public Plan Plan
        {
            get { return m_plan; }
            set
            {
                m_plan = value;
                SelectControl.Plan = value;
                OnPlanChanged();
            }
        }

        /// <summary>
        /// Gets or sets the currently selected object.
        /// </summary>
        [Browsable(false)]
        public Item SelectedObject
        {
            get { return SelectControl == null ? null : SelectControl.SelectedObject; }
            set
            {
                if (SelectControl == null)
                    return;

                SelectControl.SelectedObject = value;
            }
        }

        #endregion


        #region Protected Properties

        /// <summary>
        /// Gets or sets the select control.
        /// </summary>
        /// <value>The select control.</value>
        protected EveObjectSelectControl SelectControl { get; set; }

        /// <summary>
        /// Gets or sets the properties list.
        /// </summary>
        /// <value>The properties list.</value>
        protected ListView PropertiesList { get; set; }

        #endregion


        #region Events

        /// <summary>
        /// Called whenever the plan changes.
        /// </summary>
        /// <remarks>This virtual method is implemented in classes that inherit from EveObjectBrowserControl.</remarks>
        protected virtual void OnPlanChanged()
        {
        }

        #endregion


        #region Selection management

        /// <summary>
        /// Updates the controls when the selection is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnSelectionChanged(object sender, EventArgs e)
        {
            // Updates the header and the panels visibility.
            Item firstSelected = SelectControl.SelectedObject;
            if (firstSelected == null)
            {
                // Hide details and header
                pnlDetails.Visible = false;
                pnlBrowserHeader.Visible = false;

                // View help message
                lblHelp.Visible = true;

                // Listview
                PropertiesList.Items.Clear();

                // Done
                return;
            }

            // Hide help message
            lblHelp.Visible = false;

            // View details and header
            pnlBrowserHeader.Visible = true;
            pnlDetails.Visible = true;

            // Display details
            eoImage.EveItem = firstSelected;
            lblEveObjName.Text = firstSelected.Name;
            lblEveObjCategory.Text = firstSelected.GetCategoryPath();

            // Stop here if it's the blueprint tab
            if (SelectControl is BlueprintSelectControl)
                return;

            // Fill the list view
            UpdatePropertiesList();

        }

        /// <summary>
        /// Refresh the properties list.
        /// </summary>
        private void UpdatePropertiesList()
        {
            PropertiesList.BeginUpdate();
            try
            {
                // Refresh columns
                PropertiesList.Columns.Clear();
                PropertiesList.Columns.Add("Attribute");
                foreach (Item obj in SelectControl.SelectedObjects)
                {
                    PropertiesList.Columns.Add(obj.Name);
                }

                // Prepare properties list
                List<ListViewItem> items = AddPropertyGroups();

                // Fetch the new items to the list view
                PropertiesList.Items.Clear();
                PropertiesList.Items.AddRange(items.ToArray());
                PropertiesList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
            finally
            {
                PropertiesList.EndUpdate();
            }
        }

        /// <summary>
        /// Adds the property groups.
        /// </summary>
        /// <returns></returns>
        private List<ListViewItem> AddPropertyGroups()
        {
            PropertiesList.Groups.Clear();
            List<ListViewItem> items = new List<ListViewItem>();
            foreach (EvePropertyCategory category in StaticProperties.AllCategories)
            {
                ListViewGroup group = new ListViewGroup(category.DisplayName);
                bool hasProps = false;

                foreach (EveProperty prop in category)
                {
                    // Checks whether we must display this property
                    bool visibleProperty = false;

                    // Some properties should be always visible (fitting, shields resists, etc)
                    if (this is ShipBrowserControl)
                        visibleProperty = prop.AlwaysVisibleForShips;

                    // Or we check whether any object has this property
                    if (!visibleProperty)
                        visibleProperty = SelectControl.SelectedObjects.Any(x => x.Properties[prop].HasValue);

                    // Some properties should be hidden if they have the default value (sensor strenght, em damage, etc)
                    if (prop.HideIfDefault)
                        visibleProperty = SelectControl.SelectedObjects
                            .Any(x =>
                                     {
                                         EvePropertyValue? eveProperty = x.Properties[prop];
                                         return (eveProperty != null && (prop.DefaultValue != eveProperty.Value.Value));
                                     });

                    // We hide the reprocessing skill here and make it visible in the "Reprocessing Info" section
                    if (prop.ID == DBConstants.ReprocessingSkillPropertyID)
                        visibleProperty = false;

                    // Jump to next property if not visible
                    if (!visibleProperty)
                        continue;

                    hasProps = true;

                    // Retrieve the data to put in the columns
                    AddPropertyValue(items, group, prop);
                }

                // Check if the objects belong to an item family that has fitting slot property 
                if (category.Name == "General" &&
                    SelectControl.SelectedObjects.Any(x => (x.Family == ItemFamily.Item || x.Family == ItemFamily.Drone) &&
                                                           x.FittingSlot != ItemSlot.None && x.FittingSlot != ItemSlot.Empty))
                    AddFittingSlotProperty(items, group);

                // Add properties
                if (hasProps)
                    PropertiesList.Groups.Add(group);
            }

            // Add the reprocessing-refining info 
            AddReprocessingInfo(items);

            return items;
        }

        /// <summary>
        /// Adds the property value.
        /// </summary>
        /// <param name="items">The list of items.</param>
        /// <param name="group">The listGroup.</param>
        /// <param name="prop">The property.</param>
        private void AddPropertyValue(ICollection<ListViewItem> items, ListViewGroup group, EveProperty prop)
        {
            string[] labels = SelectControl.SelectedObjects.Select(prop.GetLabelOrDefault).ToArray();
            float[] values = SelectControl.SelectedObjects.Select(prop.GetNumericValue).ToArray();

            // Create the list view item
            ListViewItem item = new ListViewItem(group) { ToolTipText = prop.Description, Text = prop.Name };
            items.Add(item);

            AddValueForSelectedObjects(prop, item, labels, values);
        }

        /// <summary>
        /// Adds the value for selected objects.
        /// </summary>
        /// <param name="obj">The evaluated object.</param>
        /// <param name="item">The list of items.</param>
        /// <param name="labels">The labels.</param>
        /// <param name="values">The values.</param>
        private void AddValueForSelectedObjects(Object obj, ListViewItem item, string[] labels, float[] values)
        {
            float min = 0f;
            float max = 0f;
            bool allEqual = true;

            if (values.Count() > 0)
            {
                min = values.Min();
                max = values.Max();
                allEqual = values.All(x => Math.Abs(x - min) < float.Epsilon);
                if (obj is EveProperty && !((EveProperty)obj).HigherIsBetter)
                {
                    float temp = min;
                    min = max;
                    max = temp;
                }
            }

            // Add the value for every selected item
            for (int index = 0; index < SelectControl.SelectedObjects.Count; index++)
            {
                // Create the subitem and choose its forecolor
                ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem(item, labels[index]);
                if (!allEqual)
                {
                    if (Math.Abs(values[index] - max) < float.Epsilon)
                    {
                        subItem.ForeColor = Color.DarkGreen;
                    }
                    else if (Math.Abs(values[index] - min) < float.Epsilon)
                    {
                        subItem.ForeColor = Color.DarkRed;
                    }

                    item.UseItemStyleForSubItems = false;
                }
                else if (SelectControl.SelectedObjects.Count > 1)
                {
                    subItem.ForeColor = Color.DarkGray;
                    item.UseItemStyleForSubItems = false;
                }

                item.SubItems.Add(subItem);
            }
        }

        /// <summary>
        /// Adds the fitting slot property.
        /// </summary>
        /// <param name="items">The list of items.</param>
        /// <param name="group">The listGroup.</param>
        private void AddFittingSlotProperty(List<ListViewItem> items, ListViewGroup group)
        {
            string[] labels = SelectControl.SelectedObjects.Select(x => x.FittingSlot.ToString()).ToArray();

            // Create the list view item
            ListViewItem item = new ListViewItem(group) { ToolTipText = "The slot that this item fits in", Text = "Fitting Slot" };
            items.Add(item);

            // Add the value for every selected item
            AddValueForSelectedObjects(null, item, labels, new float[] { });
        }

        /// <summary>
        /// Adds the reprocessing info.
        /// </summary>
        /// <param name="items">The list of items.</param>
        private void AddReprocessingInfo(List<ListViewItem> items)
        {
            if (SelectControl.SelectedObjects.All(x => x.ReprocessingMaterials == null))
                return;

            string groupName = "Reprocessing - Refining Info";

            if (SelectControl.SelectedObjects.All(x => x.ReprocessingSkill.ID != DBConstants.ScrapMetalProcessingSkillID))
                groupName = "Refining Info";

            if (SelectControl.SelectedObjects.All(x => x.ReprocessingSkill.ID == DBConstants.ScrapMetalProcessingSkillID))
                groupName = "Reprocessing Info";

            ListViewGroup group = new ListViewGroup(groupName);

            // Add the reprocessing skill
            AddReprocessingSkill(group, items);

            IEnumerable<Material> reprocessingMaterials = SelectControl.SelectedObjects.Where(
                x => x.ReprocessingMaterials != null).SelectMany(x => x.ReprocessingMaterials);

            foreach (Item item in StaticItems.AllItems.OrderBy(x => x.ID))
            {
                if (!reprocessingMaterials.Any(x => x.Item == item))
                    continue;

                // Create the list of reprocessing materials we need to scroll through
                List<Material> materials = new List<Material>();
                foreach (Item obj in SelectControl.SelectedObjects)
                {
                    // Compansate for missing entries
                    if (obj.ReprocessingMaterials == null)
                    {
                        materials.Add(null);
                        continue;
                    }
                    materials.Add(obj.ReprocessingMaterials.FirstOrDefault(y => y.Item == item));
                }

                // Create the list of labels and values
                List<string> labels = new List<string>();
                List<float> values = new List<float>();
                foreach (Material material in materials)
                {
                    // Add default labels and values for non existing materials
                    if (material == null)
                    {
                        labels.Add("0 ");
                        values.Add(0f);
                        continue;
                    }
                    labels.Add(material.Quantity.ToString("N0"));
                    values.Add(material.Quantity);
                }

                // Create the list view item
                ListViewItem lvItem = new ListViewItem(group) { ToolTipText = item.Description, Text = item.Name };
                items.Add(lvItem);

                AddValueForSelectedObjects(null, lvItem, labels.ToArray(), values.ToArray());
            }

            PropertiesList.Groups.Add(group);
        }

        /// <summary>
        /// Adds the reprocessing skill.
        /// </summary>
        /// <param name="group">The listGroup.</param>
        /// <param name="items">The list of items.</param>
        private void AddReprocessingSkill(ListViewGroup group, List<ListViewItem> items)
        {
            // Create the list of labels
            List<string> labels = new List<string>();
            foreach (Item obj in SelectControl.SelectedObjects)
            {
                if (obj.ReprocessingMaterials == null)
                {
                    labels.Add("None");
                    continue;
                }
                labels.Add(obj.ReprocessingSkill.Name);
            }

            // Create the list view item
            ListViewItem item = new ListViewItem(group)
                                    {
                                        ToolTipText =
                                            StaticProperties.GetPropertyByID(DBConstants.ReprocessingSkillPropertyID).
                                            Description,
                                        Text =
                                            StaticProperties.GetPropertyByID(DBConstants.ReprocessingSkillPropertyID).
                                            Name
                                    };
            items.Add(item);

            // Add the value for every selected item
            AddValueForSelectedObjects(null, item, labels.ToArray(), new float[] { });
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Updates the controls visibility.
        /// </summary>
        private void UpdateControlsVisibility()
        {
            if (Settings.UI.SafeForWork)
            {
                eoImage.ImageSize = EveImageSize.x0;
                eoImage.Visible = false;
                lblEveObjCategory.Location = new Point(Pad, lblEveObjCategory.Location.Y);
                lblEveObjName.Location = new Point(Pad, lblEveObjName.Location.Y);
            }
            else
            {
                eoImage.ImageSize = EveImageSize.x64;
                eoImage.Visible = true;
                if (SelectControl.SelectedObject != null)
                    eoImage.EveItem = SelectControl.SelectedObject;

                lblEveObjCategory.Location = new Point(eoImage.Width + Pad * 2, lblEveObjCategory.Location.Y);
                lblEveObjName.Location = new Point(eoImage.Width + Pad * 2, lblEveObjName.Location.Y);
            }
        }

        #endregion
    }
}
