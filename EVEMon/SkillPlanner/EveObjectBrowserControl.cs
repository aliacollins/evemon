using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.Data;

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
        private bool m_forceShipsPropertyToBeVisible;
        private EveObjectSelectControl m_selectControl;
        private ListView m_propertiesList;
        private Plan m_plan;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public EveObjectBrowserControl()
        {
            InitializeComponent();
            this.lblEveObjName.Font = FontFactory.GetFont("Tahoma", 11.25F, System.Drawing.FontStyle.Bold);

            EveClient.SettingsChanged += new EventHandler(EveClient_SettingsChanged);
            this.Disposed += new EventHandler(OnDisposed);
        }

        /// <summary>
        /// Unsubscribe events on disposing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDisposed(object sender, EventArgs e)
        {
            EveClient.SettingsChanged -= new EventHandler(EveClient_SettingsChanged);
            this.Disposed -= new EventHandler(OnDisposed);
        }

        /// <summary>
        /// Used to complete the control's intialization before <see cref="OnLoad"/>.
        /// </summary>
        /// <param name="propertiesList"></param>
        /// <param name="selectControl"></param>
        protected void Initialize(ListView propertiesList, EveObjectSelectControl selectControl, bool forceShipsPropertyToBeVisible)
        {
            m_selectControl = selectControl;
            m_propertiesList = propertiesList;
            m_forceShipsPropertyToBeVisible = forceShipsPropertyToBeVisible;
        }

        /// <summary>
        /// Occurs when the control is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveObjectBrowserControl_Load(object sender, EventArgs e)
        {
            if (this.DesignMode || this.IsDesignModeHosted())
                return;

            // Watch for selection changes
            this.m_selectControl.SelectionChanged += new EventHandler(OnSelectionChanged);

            // Reposition the help text along side the treeview
            Control[] result = this.m_selectControl.Controls.Find("panel2", true);
            if (result.Length > 0)
                lblHelp.Location = new Point(lblHelp.Location.X, result[0].Location.Y);

            // Updates the control
            EveClient_SettingsChanged(null, null);

            // Force a refresh
            OnSelectionChanged(null, null);
        }

        /// <summary>
        /// Occurs when the settings changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void EveClient_SettingsChanged(object sender, EventArgs e)
        {
            if (Settings.UI.SafeForWork)
            {
                eoImage.ImageSize = EveImage.EveImageSize.x0;
                lblEveObjCategory.Location = new Point(3, lblEveObjCategory.Location.Y);
                lblEveObjName.Location = new Point(3, lblEveObjName.Location.Y);
            }
            else
            {
                eoImage.ImageSize = EveImage.EveImageSize.x64;
                if (m_selectControl.SelectedObject != null)
                    eoImage.EveItem = m_selectControl.SelectedObject;

                lblEveObjCategory.Location = new Point(70, lblEveObjCategory.Location.Y);
                lblEveObjName.Location = new Point(70, lblEveObjName.Location.Y);
            }
        }

        /// <summary>
        /// Gets or sets the current plan for this planner window.
        /// </summary>
        public Plan Plan
        {
            get { return m_plan; }
            set
            {
                m_plan = value;
                this.m_selectControl.Plan = value;
                OnPlanChanged();
            }
        }

        /// <summary>
        /// Called whenever the plan changes.
        /// </summary>
        protected virtual void OnPlanChanged()
        {
        }


        #region Selection management
        /// <summary>
        /// Gets or sets the currently selected object.
        /// </summary>
        [Browsable(false)]
        public Item SelectedObject
        {
            get 
            {
                if (m_selectControl == null) 
                    return null;

                return m_selectControl.SelectedObject; 
            }
            set 
            {
                if (m_selectControl == null) 
                    return;

                m_selectControl.SelectedObject = value; 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnSelectionChanged(object sender, EventArgs e)
        {
            // Updates the header and the panels visibility.
            var firstSelected = m_selectControl.SelectedObject;
            if (firstSelected == null)
            {
                // Hide details and header
                pnlBrowserHeader.Visible = false;
                pnlDetails.Visible = false;

                // View help message
                lblHelp.Visible = true;

                // Listview
                m_propertiesList.Items.Clear();

                // Done
                return;
            }

            // Hide help message
            lblHelp.Visible = false;

            // View details and header
            pnlDetails.Visible = true;
            pnlBrowserHeader.Visible = true;

            // Display details
            eoImage.EveItem = firstSelected;
            lblEveObjName.Text = firstSelected.Name;
            lblEveObjCategory.Text = firstSelected.GetCategoryPath();

            // Fill the list view
            UpdatePropertiesList();

        }

        /// <summary>
        /// Refresh the properties list
        /// </summary>
        private void UpdatePropertiesList()
        {
            // TODO: Refactor into a shorter method

            m_propertiesList.BeginUpdate();
            try
            {
                // Refresh columns
                m_propertiesList.Columns.Clear();
                m_propertiesList.Columns.Add("Attribute");
                foreach(var obj in m_selectControl.SelectedObjects)
                {
                    m_propertiesList.Columns.Add(obj.Name);
                }

                // Prepare properties list
                m_propertiesList.Groups.Clear();
                var items = new List<ListViewItem>();
                foreach(var category in StaticProperties.AllCategories)
                {
                    var group = new ListViewGroup(category.DisplayName);
                    bool hasProps = false;

                    foreach(var prop in category)
                    {
                        // Checks whether we must display this property
                        bool visibleProperty = false;

                        // Some properties should be always visible (fitting, shields resists, etc)
                        if (m_forceShipsPropertyToBeVisible)
                            visibleProperty = prop.AlwaysVisibleForShips;

                        // Or we check whether any object has this property
                        if (!visibleProperty)
                            visibleProperty = m_selectControl.SelectedObjects.Any(x => x.Properties[prop].HasValue);

                        // Some properties should be hidden if they have the default value (sensor strenght, em damage, etc)
                        if (prop.HideIfDefault)
                            visibleProperty = m_selectControl.SelectedObjects.Any(x => x.Properties[prop].HasValue && (prop.DefaultValue != x.Properties[prop].Value.Value));

                        // Jump to next property if not visible
                        if (!visibleProperty)
                            continue;

                        hasProps = true;

                        // Retrieve the data to put in the columns
                        var labels = m_selectControl.SelectedObjects.Select(x => prop.GetLabelOrDefault(x)).ToArray();
                        var values = m_selectControl.SelectedObjects.Select(x => prop.GetNumericValue(x)).ToArray();
                        var min = values.Min();
                        var max = values.Max();
                        var allEqual = values.All(x => x == min);
                        if (!prop.HigherIsBetter)
                        {
                            var temp = min;
                            min = max;
                            max = temp;
                        }

                        // Create the list view item
                        ListViewItem item = new ListViewItem(group);
                        item.ToolTipText = prop.Description;
                        item.Text = prop.Name;
                        items.Add(item);

                        // Add the value for every ship
                        int index = 0;
                        foreach(var obj in m_selectControl.SelectedObjects)
                        {
                            // Create the subitem and choose its forecolor
                            var subItem = new ListViewItem.ListViewSubItem(item, labels[index]);
                            if (!allEqual)
                            {
                                if (values[index] == max)
                                {
                                    subItem.ForeColor = Color.DarkGreen;
                                }
                                else if (values[index] == min)
                                {
                                    subItem.ForeColor = Color.DarkRed;
                                }

                                item.UseItemStyleForSubItems = false;
                            }
                            else if (m_selectControl.SelectedObjects.Count > 1)
                            {
                                subItem.ForeColor = Color.DarkGray;
                                item.UseItemStyleForSubItems = false;
                            }

                            item.SubItems.Add(subItem);
                            index++;
                        }
                    }

                    // Check if the objects belong to an item family that has fitting slot property 
                    if (m_selectControl.SelectedObjects.Any(x => x.Family == ItemFamily.Item || x.Family == ItemFamily.Drone))
                    {
                        // Create the list view item
                        ListViewItem item = new ListViewItem(group);
                        var labels = m_selectControl.SelectedObjects.Select(x => x.FittingSlot.ToString()).ToArray();
                        if (category.Name == "General" && m_selectControl.SelectedObjects.Any(x => x.FittingSlot != ItemSlot.None && x.FittingSlot != ItemSlot.Empty))
                        {
                            item.ToolTipText = "The slot that this item fits in";
                            item.Text = "Fitting Slot";
                            items.Add(item);
                        }

                        // Add the value for every item
                        int index = 0;
                        foreach (var obj in m_selectControl.SelectedObjects)
                        {
                            // Create the subitem and choose its forecolor
                            var subItem = new ListViewItem.ListViewSubItem(item, labels[index]);
                            subItem.ForeColor = m_selectControl.SelectedObjects.Count > 1 ? Color.DarkGray : Color.Black;
                            item.UseItemStyleForSubItems = false;

                            item.SubItems.Add(subItem);
                            index++;
                        }
                    }

                    // Add properties
                    if (hasProps)
                    {
                        m_propertiesList.Groups.Add(group);
                    }
                }

                // Fetch the new items to the list view
                m_propertiesList.Items.Clear();
                m_propertiesList.Items.AddRange(items.ToArray());
                m_propertiesList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
            finally
            {
                m_propertiesList.EndUpdate();
            }
        }
        #endregion
    }
}
